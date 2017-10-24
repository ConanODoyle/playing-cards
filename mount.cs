package RemoveDecksOnDeath {
	function Armor::onRemove(%this, %obj) {
		%obj.clearCardData();

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
activatePackage(RemoveDecksOnDeath);


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


function Player::displayCards(%this) {
	%deck = %this.deck;

	%this.isCardsVisible = 1;

	//cardholder
	if (!isObject(%this.cardHolder)) {
		%this.cardHolder = new AIPlayer(Cards) {
			datablock = CardHolderArmor;
			owner = %this;
		};
		%this.cardHolder.kill();
		%this.cardHolder.setScale("1 1 1");
		%this.mountObject(%this.cardHolder, 7);
	}

	%this.playThread(1, armReadyBoth);
	%this.playThread(2, root);

	%this.hideNode("lHand");
	%this.hideNode("rHand");
	%this.hideNode("lHook");
	%this.hideNode("rHook");

	%this.cardHolder.hideNode("ALL");
	%this.cardHolder.unHideNode($LHand[%this.client.lhand]);
	%this.cardHolder.unHideNode($RHand[%this.client.rhand]);
	%this.cardHolder.setNodeColor($LHand[%this.client.lhand], %this.client.lhandColor);
	%this.cardHolder.setNodeColor($RHand[%this.client.rhand], %this.client.rhandcolor);

	//cards
	%count = %deck.numCards;
	%start = mFloor(%count / 2) + 7;
	for (%i = 0; %i < %count; %i++) {
		if (!isObject(%this.card[%i])) {
			%this.card[%i] = new AIPlayer(Cards) {
				datablock = CardArmor;
				owner = %this;
			};
			%this.card[%i].kill();
		}
		cardDisplay(%this.card[%i], getCardName(getWord(%deck.cards, %i)));
		%this.card[%i].setTransform("0 0 0 0 0 1 0");
		%this.cardHolder.mountObject(%this.card[%i], %start - %i);
	}
	//cleanup of old cards
	for (%i = %i; %i < 13; %i++) {
		if (isObject(%this.card[%i])) {
			%this.card[%i].delete();
		}
	}

	if (!isObject(%this.cardHolder)) {
		%this.cardHolder = new AIPlayer(Cards) {
			datablock = CardHolderArmor;
			owner = %this;
		};
		%this.cardHolder.kill();
	}

	bottomprintCardInfo(%this);
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

function Player::scheduleClearAddedCardCount(%pl) {
	cancel(%pl.clearAddedCardCountSched1);
	cancel(%pl.clearAddedCardCountSched2);

	%pl.clearAddedCardCountSched1 = schedule(1200, %pl, eval, %pl @ ".recentlyAddedCardCount = 0;");
	%pl.clearAddedCardCountSched2 = schedule(1200, %pl, eval, "bottomprintCardInfo(" @ %pl @ ");");
}