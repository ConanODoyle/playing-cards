datablock AudioProfile(cardPick1Sound){
   filename    = "./sounds/pick1.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPick2Sound){
   filename    = "./sounds/pick2.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPick3Sound){
   filename    = "./sounds/pick3.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPick4Sound){
   filename    = "./sounds/pick4.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPlace1Sound){
   filename    = "./sounds/place1.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPlace2Sound){
   filename    = "./sounds/place2.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPlace3Sound){
   filename    = "./sounds/place3.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(cardPlace4Sound){
   filename    = "./sounds/place4.wav";
   description = AudioClose3d;
   preload = true;
};


datablock fxDTSBrickData(brickCardShoeData) {
	brickFile = "./cardShoe.blb";
	category = "Special";
	subCategory = "Card Bricks";
	uiName = "Card Shoe";
};

datablock fxDTSBrickData(brickCardShufflerData) {
	brickFile = "./cardShuffler.blb";
	category = "Special";
	subCategory = "Card Bricks";
	uiName = "Card Shuffler";
};


package KeepWhenDead {
	function Armor::onDisabled(%this, %obj, %state) {
		if (%this.keepWhenDead) {
			return;
		}
		return parent::onDisabled(%this, %obj, %state);
	}
};
activatePackage(KeepWhenDead);

datablock TSShapeConstructor(CardDTS) {
	baseShape	= "./tex/cards.dts";
	sequence0	= "./tex/cards.dsq root";

	sequence1	= "./tex/cards.dsq"; //has all anims for dts (root, cardUp)
};

datablock PlayerData(CardArmor : PlayerStandardArmor) {
	shapeFile = "./tex/cards.dts";

	uiName = "";

	boundingBox			= vectorScale("20 20 20", 4);
	crouchBoundingBox	= vectorScale("20 20 20", 4);

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	maxForwardProneSpeed = 0;
	maxBackwardProneSpeed = 0;
	maxSideProneSpeed = 0;

	maxForwardWalkSpeed = 0;
	maxBackwardWalkSpeed = 0;
	maxSideWalkSpeed = 0;

	maxUnderwaterForwardSpeed = 0;
	maxUnderwaterBackwardSpeed = 0;
	maxUnderwaterSideSpeed = 0;
	
	jumpForce = 0 * 140; //8.3 * 90;
	canJet = 0;

	keepWhenDead = 1;
};

datablock TSShapeConstructor(CardHolderDTS) {
	baseShape	= "./tex/cardHolder.dts";

	sequence0	= "./tex/cardHolder.dsq"; //has all anims for dts (root, showHand)
};

datablock PlayerData(CardHolderArmor : PlayerStandardArmor) {
	shapeFile = "./tex/cardHolder.dts";

	uiName = "";

	boundingBox			= vectorScale("20 20 20", 4);
	crouchBoundingBox	= vectorScale("20 20 20", 4);

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	maxForwardProneSpeed = 0;
	maxBackwardProneSpeed = 0;
	maxSideProneSpeed = 0;

	maxForwardWalkSpeed = 0;
	maxBackwardWalkSpeed = 0;
	maxSideWalkSpeed = 0;

	maxUnderwaterForwardSpeed = 0;
	maxUnderwaterBackwardSpeed = 0;
	maxUnderwaterSideSpeed = 0;
	
	jumpForce = 0 * 140; //8.3 * 90;
	canJet = 0;

	keepWhenDead = 1;
};

datablock PlayerData(CardChairArmor : PlayerStandardArmor) {
	canJet = 0;

	uiName = "";

	jumpForce = 0;

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	maxForwardProneSpeed = 0;
	maxBackwardProneSpeed = 0;
	maxSideProneSpeed = 0;

	maxForwardWalkSpeed = 0;
	maxBackwardWalkSpeed = 0;
	maxSideWalkSpeed = 0;

	maxUnderwaterForwardSpeed = 0;
	maxUnderwaterBackwardSpeed = 0;
	maxUnderwaterSideSpeed = 0;

	rideable = true;
		lookUpLimit = 0.6;
		lookDownLimit = 0.2;

	canRide = false;

	numMountPoints = 1;
	mountThread[0] = "root";
	mountNode[0] = 2;
};

datablock StaticShapeData(CardShape) {
	shapeFile = "./tex/cards.dts";
};

datablock ItemData(CardsOutItem : HammerItem) {
	shapeFile = "./tex/singleCard.dts";
	image = CardsOutImage;

	uiName = "Cards";
	iconName = "Add-Ons/Item_PlayingCards/cardIcon";

	colorShiftColor = "1 1 1 1";
};

datablock ShapeBaseImageData(CardsOutImage : printGunImage) {
	shapeFile = "base/data/shapes/empty.dts";
  
	item = CardsOutItem;

	projectile = "";
  
	stateName[0] = "Activate";
	stateTimeout[0] = 0.1;
	stateTransitionOnTimeout[0] = "Ready";
	stateSequence[0] = "";

	stateName[1] = "Ready";
	stateTransitionOnTimeout[1] = "";
	stateTransitionOnTriggerDown[1] = "";
	stateTransitionOnTriggerUp[1] = "";
	stateSequence[1] = "";
};

datablock ItemData(DeckOutItem : HammerItem) {
	shapeFile = "./tex/deck.dts";
	image = DeckOutImage;

	uiName = "Card Deck";
	iconName = "Add-Ons/Item_PlayingCards/deckIcon";

	colorShiftColor = "1 1 1 1";
};

datablock ShapeBaseImageData(DeckOutImage : printGunImage) {
	shapeFile = "base/data/shapes/empty.dts";
  
	item = DeckOutItem;

	projectile = "";
  
	stateName[0] = "Activate";
	stateTimeout[0] = 0.1;
	stateTransitionOnTimeout[0] = "Ready";
	stateSequence[0] = "";

	stateName[1] = "Ready";
	stateTransitionOnTimeout[1] = "";
	stateTransitionOnTriggerDown[1] = "";
	stateTransitionOnTriggerUp[1] = "";
	stateSequence[1] = "";
};

function cardDisplay(%pl, %card) {
	%pl.hideNode("ALL");
	%pl.setNodeColor("ALL", "1 1 1 1");

	%pl.unHideNode("cardBack");
	%pl.unHideNode("card");
	%pl.unHideNode(%card);
}