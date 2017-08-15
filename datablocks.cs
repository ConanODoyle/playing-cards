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
	baseShape	= "./cards.dts";
	sequence0	= "./cards.dsq root";

	sequence1	= "./cards.dsq"; //has all anims for dts (root, cardUp)
};

datablock PlayerData(CardArmor : PlayerStandardArmor) {
	shapeFile = "./cards.dts";

	boundingBox			= vectorScale(".5 .5 .2", 4);
	crouchBoundingBox	= vectorScale(".5 .5 .2", 4);

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
	baseShape	= "./cardHolder.dts";
	sequence0	= "./cardHolder.dsq root";

	sequence1	= "./cardHolder.dsq"; //has all anims for dts (root, showHand)
};

datablock PlayerData(CardHolderArmor : PlayerStandardArmor) {
	shapeFile = "./cardHolder.dts";

	boundingBox			= vectorScale(".5 .5 .2", 4);
	crouchBoundingBox	= vectorScale(".5 .5 .2", 4);

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

datablock StaticShapeData(CardShape) {
	shapeFile = "./cards.dts";
};

datablock ShapeBaseImageData(CardsOutImage) {
	shapeFile = "base/data/shapes/empty.dts";
};

function cardDisplay(%pl, %card) {
	if (%pl.getDatablock().getID() != CardArmor.getID()) {
		return;
	}

	%pl.hideNode("ALL");
	%pl.setNodeColor("ALL", "1 1 1 1");

	%pl.unHideNode("cardBack");
	%pl.unHideNode("card");
	%pl.unHideNode(%card);
}