package RemoveCardsAndChipsOnDeath {
	function Armor::onRemove(%this, %obj) {
		%obj.clearCardData();
		if (isObject(%obj.deckShuffle)) {
			%obj.deckShuffle.delete();
		}

		if (isObject(%obj.chipDisplayBrick)) {
			%obj.chipDisplayBrick.clearChips(%obj.client);
		}

		return parent::onRemove(%this, %obj);
	}

	function Armor::onMount(%this, %obj, %vehi, %node) {
		if (%this.getID() == CardHolderArmor.getID() || %this.getID() == CardArmor.getID()) {
			%obj.setTransform("0 0 0 0 0 1 0");
			return;
		}
		return parent::onMount(%this, %obj, %vehi, %node);
	}

	function GameConnection::onDeath(%cl, %a, %b, %c, %d) {
		%cl.player.clearCardData();

		return parent::onDeath(%cl, %a, %b, %c, %d);
	}
};
activatePackage(RemoveCardsAndChipsOnDeath);


////////////////////


function Player::addCard(%this, %card) {
	if (!isObject(%this.deck)) {
		%this.deck = getEmptyDeck();
	} else if (%this.deck.numCards >= 13) {
		error("Cannot add card to player hand: hand is full!");
		$LastCard = %card;
		return;
	}

	%cl = %this.client;

	if (%this.isDealingCards) {
		%this.addDeckCard(%card);

		%this.recentlyAddedCardCount++;
		bottomprintCardInfo(%this);

		%this.scheduleClearAddedCardCount();
	} else {
		%this.deck.addCard(%card);

		if (isObject(%cl)) {
			messageClient(%this.client, '', "\c6You picked up a " @ getLongCardName(%card));
		}
	}
}

function Player::removeCardIndex(%this, %idx) {
	if (!isObject(%this.deck)) {
		%this.deck = getEmptyDeck();
		return -1;
	}

	return %this.deck.removeCard(%idx);
}

function Player::removeCard(%this, %card) {
	if (!isObject(%this.deck)) {
		%this.deck = getEmptyDeck();
		return;
	}

	%idx = getWordIndex(%this.cards, %card);

	if (%idx >= 0) {
		return %this.deck.removeCardIndex(%idx);
	} else {
		return -1;
	}
}

function Player::clearCardData(%this) {
	if (isObject(%this.deck)) {
		%this.deck.delete();
	}

	if (isObject(%this.cardHolder)) {
		%this.cardHolder.delete();
	}

	for (%i = 0; %i < 13; %i++) {
		if (isObject(%this.card[%i])) {
			%this.card[%i].delete();
		}
	}
}


////////////////////


function Player::displayCards(%pl) {
	%deck = %pl.deck;

	%pl.isCardsVisible = 1;

	//cardholder
	if (!isObject(%pl.cardHolder)) {
		%pl.cardHolder = new AIPlayer(Cards) {
			datablock = CardHolderArmor;
			owner = %pl;
		};
		%pl.cardHolder.kill();
		%pl.cardHolder.setScale("1 1 1");
	}
	%pl.mountObject(%pl.cardHolder, 7);

	%cl = %pl.client;

	%pl.playThread(1, armReadyBoth);
	%pl.playThread(2, root);

	%pl.hideNode("lHand");
	%pl.hideNode("rHand");
	%pl.hideNode("lHook");
	%pl.hideNode("rHook");

	%pl.cardHolder.hideNode("ALL");
	%pl.cardHolder.unHideNode($LHand[%cl.lhand]);
	%pl.cardHolder.unHideNode($RHand[%cl.rhand]);
	%pl.cardHolder.setNodeColor($LHand[%cl.lhand], %cl.lhandColor);
	%pl.cardHolder.setNodeColor($RHand[%cl.rhand], %cl.rhandcolor);

	//cards
	%count = %deck.numCards;
	%start = mFloor(%count / 2) + 7;
	for (%i = 0; %i < %count; %i++) {
		if (!isObject(%pl.card[%i])) {
			%pl.card[%i] = new AIPlayer(Cards) {
				datablock = CardArmor;
				owner = %pl;
			};
			%pl.card[%i].kill();
		}
		cardDisplay(%pl.card[%i], getCardName(getWord(%deck.cards, %i)));
		%pl.card[%i].setTransform("0 0 0 0 0 1 0");
		%pl.cardHolder.mountObject(%pl.card[%i], %start - %i);
	}
	//cleanup of old cards
	for (%i = %i; %i < 13; %i++) {
		if (isObject(%pl.card[%i])) {
			%pl.card[%i].delete();
		}
	}

	if (!isObject(%pl.cardHolder)) {
		%pl.cardHolder = new AIPlayer(Cards) {
			datablock = CardHolderArmor;
			owner = %pl;
		};
		%pl.cardHolder.kill();
	}

	bottomprintCardInfo(%pl);
}

function Player::hideCards(%this) {
	%this.isCardsVisible = 0;
	
	if (isObject(%this.cardHolder)) {
		%this.cardHolder.hideNode("ALL");
	}

	for (%i = 0; %i < 13; %i++) {
		if (isObject(%this.card[%i])) {
			%this.card[%i].hideNode("ALL");
		}
	}
	
	if (%this.isSelectingCard) {
		%this.stopCardSelect();
    }

	%this.client.applyBodyParts();
	bottomprintCardInfo(%this);
}

function Player::displayDeck(%pl) {
	%pl.isDeckVisible = 1;
	//cardholder
	if (!isObject(%pl.deckShuffle)) {
		%pl.deckShuffle = new AIPlayer(Cards) {
			datablock = DeckShuffleArmor;
			owner = %pl;
		};
		%pl.deckShuffle.kill();
		%pl.deckShuffle.setScale("1 1 1");
	}
	%pl.mountObject(%pl.deckShuffle, 7);
	%cl = %pl.client;

	%pl.playThread(1, armReadyBoth);
	%pl.playThread(2, root);

	%pl.hideNode("lHand");
	%pl.hideNode("rHand");
	%pl.hideNode("lHook");
	%pl.hideNode("rHook");

	%pl.deckShuffle.hideNode("ALL");
	%pl.deckShuffle.unHideNode($LHand[%cl.lhand]);
	%pl.deckShuffle.unHideNode($RHand[%cl.rhand]);
	%pl.deckShuffle.unHideNode("deck1");
	%pl.deckShuffle.unHideNode("deck2");
	%pl.deckShuffle.unHideNode("deck3");
	%pl.deckShuffle.setNodeColor($LHand[%cl.lhand], %cl.lhandColor);
	%pl.deckShuffle.setNodeColor($RHand[%cl.rhand], %cl.rhandcolor);

	%pl.shuffleAnim();
}

function Player::hideDeck(%pl) {
	%pl.deckShuffle.hideNode("ALL");
	%pl.client.applyBodyParts();
	%pl.client.applyBodyColors();
	%pl.isDeckVisible = 0;
	%pl.shuffleAnim();
}

function Player::shuffleAnim(%pl) {
	cancel(%pl.shuffleAnimSched);
	
	if (!%pl.isDeckVisible) {
		%pl.deckShuffle.playThread(0, root);
		return;
	}

	%doShuffle = getRandom();
	%shuffleStart = getRandom(0, 25);
	%shuffleEnd = getRandom(4, 5);

	if (%doShuffle < 0.1) {
		%pl.deckShuffle.hideNode(deck1);
		%pl.deckShuffle.unHideNode(trapCard);
		%pl.deckShuffle.playthread(0, showCard);

		%pl.deckShuffle.schedule(1000, unHideNode, deck1);
		%pl.deckShuffle.schedule(1000, hideNode, trapCard);
	} else {
		%pl.deckShuffle.schedule(%shuffleStart * 1000, playThread, 0, shuffle);
		%pl.deckShuffle.schedule((%shuffleStart + %shuffleEnd) * 1000, playThread, 0, root);
	}

	%pl.shuffleAnimSched = %pl.schedule(30000, shuffleAnim);
}