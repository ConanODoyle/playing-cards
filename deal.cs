package DealCards {
	function Armor::onCollision(%db, %obj, %col, %pos, %vel, %speed) {
		if (%col.card !$= "" && %col.canPickup) {
			%obj.addCard(%col.card);
			%col.delete();
			return;
		}
		return parent::onCollision(%db, %obj, %col, %pos, %vel, %speed);
	}
};
activatePackage(DealCards);


////////////////////


function Player::dealDeckCard(%pl) {
	if (!isObject(%pl.deckBrick.deck)) {
		return;
	}
	
	%deck = %pl.deckBrick.deck;

	if (%deck.numCards <= 0) {
		messageClient(%pl.client, '', "Your deck is out of cards!");
		return;
	}

	%i = new Item(DealtCardItems) {
		datablock = CardItem;
		canPickup = 0;
		card = %deck.removeCard(0);
	};
	MissionCleanup.add(%i);
	schedule(100, %i, eval, %i @ ".canPickup = 1;");
	
	%pos = %pl.getMuzzlePosition(0);
	%rot = getWords(%pl.getTransform(), 3, 4);
	%i.setTransform(%pos SPC %rot);
	%i.setVelocity(vectorScale(%pl.getEyeVector(), 10));
}

function Player::addDeckCard(%pl, %card) {
	if (!isObject(%pl.deckBrick.deck)) {
		return;
	}
	
	%deck = %pl.deckBrick.deck;

	%deck.addCard(%card);
}

registerOutputEvent("fxDTSBrick", "toggleDeckBrick", "", 1);
registerOutputEvent("fxDTSBrick", "shuffleDeck", "", 1);
registerOutputEvent("fxDTSBrick", "setDeckCount", "int 1 50 1", 1);
registerOutputEvent("GameConnection", "clearCards");
registerOutputEvent("GameConnection", "setDeckCount", "int 1 50 1", 1);
// registerOutputEvent("fxDTSBrick", "sayBrickName", "dataBlock fxDTSBrickData");

// function fxDTSBrick::sayBrickName(%this, %db) {
// 	talk(%db.getName());
// }

function fxDTSBrick::toggleDeckBrick(%b, %cl) {
	if (!isObject(%b.deck)) {
		%b.deck = getEmptyDeck();
	}
	%pl = %cl.player;

	if (isObject(%b.deckOwner) && %b.deckOwner != %pl) {
		messageClient(%cl, '', "This deck is being used by someone else!");
	} else if (isObject(%b.deckOwner) && %b.deckOwner == %pl) {
		messageClient(%cl, '', "\c6Your deck brick has been cleared!");
		%b.deckOwner = "";
		%pl.deckBrick = "";
	} else if (!isObject(%b.deckOwner)) {
		messageClient(%cl, '', "\c6Your deck brick has been set!");
		%b.deckOwner = %pl;
		%pl.deckBrick = %b;
	}
	%origFX = %b.getColorFxID();
	%b.setColorFx(3);
	%b.schedule(100, setColorFx, %origFX);
	%b.schedule(200, setColorFx, 3);
	%b.schedule(300, setColorFx, %origFX);
}

function fxDTSBrick::shuffleDeck(%b, %cl) {
	if (!isObject(%b.deck) || %b.deck.numCards < 0) {
		messageClient(%cl, '', "Cannot shuffle the deck - there is no cards!");
		return;
	}
	%b.deck.shuffle();

	messageClient(%cl, '', "\c6Deck shuffled! Total cards: " @ %b.deck.numCards);
}

function fxDTSBrick::setDeckCount(%b, %num, %cl) {
	if (!isObject(%b.deck)) {
		%b.deck.delete();
	}
	%b.deck = getNewDeck(%num);

	messageClient(%cl, '', "\c6Deck reset! Total cards: " @ %num * 52);
}

function GameConnection::setDeckCount(%cl, %num, %cl2) {
	%b = %cl.player.deckBrick;
	if (!isObject(%b)) {
		messageClient(%cl, '', "You don't have a deck brick!");
		return;
	}
	if (isObject(%b.deck)) {
		%b.deck.delete();
	}
	%b.deck = getNewDeck(%num);

	messageClient(%cl, '', "\c6Deck reset! Total cards: " @ %num * 52);

	%b.setColorFx(3);
	%b.schedule(100, setColorFx, %origFX);
	%b.schedule(200, setColorFx, 3);
	%b.schedule(300, setColorFx, %origFX);
}

function clearAllPlayersCards() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		if (isObject(%cl.player)) {
			%pl.hideCards();
			%pl.clearCardData();
		}
	}
}

function GameConnection::clearCards(%cl) {
	%pl = %cl.client;
	%pl.hideCards();
	%pl.clearCardData();
}


////////////////////


function DeckOutImage::onUnmount(%this, %obj, %slot) {
	%obj.isDealingCards = 0;
	bottomprintCardInfo(%obj);
	%obj.hideDeck();
}

function DeckOutImage::onMount(%this, %obj, %slot) {
	%obj.isDealingCards = 1;
	%obj.clearAddedCardCount();
	bottomprintCardInfo(%obj);
	%obj.displayDeck();
}


////////////////////


function Player::placeDeckCard(%pl) {
	if (%pl.deckBrick.deck.numCards <= 0) {
		return 0;
	}
	%s = getWords(%pl.getEyeTransform(), 0, 2);
	%e = vectorAdd(vectorScale(%pl.getEyeVector(), 5), %s);
	%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType;
	%ray = containerRaycast(%s, %e, %masks, %pl);
	%hitloc = getWords(%ray, 1, 3);
	%hit = getWord(%ray, 0);

	if (%hitloc $= "") {
		return 0;
	} else if (getWords(%ray, 4, 6) !$= "0 0 1") {
		return 0;
	} else if (%hit.getClassName() $= "Player" && isObject(%hit.client)) {
		%hit.addCard(%deck.removeCard(0));
		messageClient(%pl.client, '', "\c6You gave \c3" @ %hit.client.name @ "\c6 a card");
		return;
	}

	initContainerBoxSearch(%hitloc, 0.05, $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
	%next = containerSearchNext();
	// talk("found? " @ %next);
	if (isObject(%next) && %next.card !$= "") {
		return 0;
	}

	serverPlay3D(("cardPlace" @ getRandom(1, 4) @ "Sound"), %hitloc);
	placeCard(%pl, %hitloc, %pl.deckBrick.deck.removeCard(0), %pl.placeFaceDown);

	if (%pl.recentlyAddedCardCount > 0) {
		%pl.recentlyAddedCardCount = 0;
	}

	%pl.recentlyAddedCardCount--;
	%pl.scheduleClearAddedCardCount();
	return 1;
}

function Player::pickUpDeckCard(%pl, %cardShape) {
	%card = %cardShape.card;
	serverPlay3D(("cardPick" @ getRandom(1, 4) @ "Sound"), %cardShape.getPosition());
	%pl.emote(winStarProjectile, 1);
	%cardShape.delete();
	%pl.deckBrick.deck.addCard(%card);

	if (%pl.recentlyAddedCardCount < 0) {
		%pl.recentlyAddedCardCount = 0;
	}

	%pl.recentlyAddedCardCount++;
	%pl.scheduleClearAddedCardCount();
}

function Player::scheduleClearAddedCardCount(%pl) {
	cancel(%pl.clearAddedCardCountSchedule);
	%pl.clearAddedCardCountSchedule = %pl.schedule(2000, clearAddedCardCount);
}

function Player::clearAddedCardCount(%pl) {
	%pl.recentlyAddedCardCount = 0;
	bottomprintCardInfo(%pl);
}