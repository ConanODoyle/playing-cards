package DealCards {
	function Armor::onCollision(%this, %obj, %col, %pos, %vel, %speed) {
		if (%col.card !$= "" && %col.canPickup) {
			%obj.addCard(%col.card);
			%col.delete();
			return;
		}
		return parent::onCollision(%this, %obj, %col, %pos, %vel, %speed);
	}
};
activatePackage(DealCards);




function Player::placeDeckCard(%pl, %down) {
	if (!isObject(%pl.deckBrick.deck)) {
		return;
	}

	%deck = %pl.deckBrick.deck;

	if (%deck.numCards <= 0) {
		messageClient(%pl.client, '', "Your deck is out of cards!");
		return;
	}

	%eye = %pl.getEyeVector();
	%start = %pl.getEyeTransform();
	%end = vectorAdd(%start, vectorScale(%eye, 5));
	%ray = containerRaycast(%start, %end, $TypeMasks::fxBrickObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticObjectType | $TypeMasks::StaticShapeObjectType);
	%hitLoc = getWords(%ray, 1, 3);
	
	if (%hitLoc !$= "") {
		placeCard(%pl, %hitloc, %deck.removeCard(0), %down);
	}
}

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

function fxDTSBrick::setDeckBrick(%this, %cl) {
	if (!isObject(%this.deck)) {
		%this.deck = getShuffledDeck();
	}
	%cl.player.deckBrick = %this;
}

function fxDTSBrick::newShuffledDeck(%this) {
	if (isObject(%this.deck)) {
		%this.deck.delete();
	}
	%this.deck = getShuffledDeck();
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