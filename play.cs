function Player::cycleCardSelect(%this, %dir) {
	%deck = %this.deck;
	if (!isObject(%deck)) {
		return;
	}


	%lastCard = %this.currentCard + 0;
	%this.currentCard = (%this.currentCard + %dir + %deck.numCards) % %deck.numCards;
 	
 	// talk(%lastCard SPC %this.currentCard);
	if (isObject(%this.card[%lastCard])) {
		%this.card[%lastCard].playThread(0, root);
		// %this.card[%lastCard].playThread(1, root);
		// %this.card[%lastCard].playThread(2, root);
		// %this.card[%lastCard].playThread(3, root);
	}
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, cardUp);
		// %this.card[%this.currentCard].playThread(1, cardUp);
		// %this.card[%this.currentCard].playThread(2, cardUp);
		// %this.card[%this.currentCard].playThread(3, cardUp);
	}
}

function Player::startCardSelect(%this) {
	%deck = %this.deck;
	if (!isObject(%deck)) {
		return;
	} else if (!%this.isCardsVisible) {
		%this.displayCards();
	}
      
	%this.currentCard = 0;
	%this.isSelectingCard = 1;
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, cardUp);
		// %this.card[%this.currentCard].playThread(1, cardUp);
		// %this.card[%this.currentCard].playThread(2, cardUp);
		// %this.card[%this.currentCard].playThread(3, cardUp);
	}
}

function Player::stopCardSelect(%this) {
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, root);
		// %this.card[%this.currentCard].playThread(1, root);
		// %this.card[%this.currentCard].playThread(2, root);
		// %this.card[%this.currentCard].playThread(3, root);
	}
	%this.currentCard = 0;
	%this.isSelectingCard = 0;
}

$LEFTCLICK = 0;
$RIGHTCLICK = 4;

package SelectCards {
	function Armor::onTrigger(%this, %obj, %trig, %val) {
		if (%obj.isSelectingCard && %val) {
			if (%trig == $LEFTCLICK) {
				%obj.cycleCardSelect(-1);
			} else if (%trig == $RIGHTCLICK) {
				%obj.cycleCardSelect(1);
			}
		}
		return parent::onTrigger(%this, %obj, %trig, %val);
	}

	function serverCmdShiftBrick(%cl, %x, %y, %z) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			if (%x < 0) {
				%obj.cycleCardSelect(-1);
				return;
			} else if (%x > 0) {
				%obj.cycleCardSelect(1);
				return;
			}
		}
		parent::serverCmdShiftBrick(%cl, %x, %y, %z);
	}

	function serverCmdSuperShiftBrick(%cl, %x, %y, %z) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			if (%x < 0) {
				%obj.cycleCardSelect(-1);
				return;
			} else if (%x > 0) {
				%obj.cycleCardSelect(1);
				return;
			}
		}
		parent::serverCmdSuperShiftBrick(%cl, %x, %y, %z);
	}

	function serverCmdPlantBrick(%cl) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			%pl.confirmCardSelection();
			%pl.stopCardSelect();
			return;
		}
		parent::serverCmdPlantBrick(%cl);
	}

	function WeaponImage::onUnmount(%this, %obj, %slot) {
		if (%this.getID() == CardsOutImage.getID() && %obj.isCardsVisible) {
			if (%obj.isSelectingCard) {
				%obj.stopCardSelect();
			}
			%obj.hideCards();
		}
		parent::onMount(%this, %obj, %slot);
	}

	function WeaponImage::onMount(%this, %obj, %slot) {
		if (%this.getID() != CardsOutImage.getID() && %obj.isCardsVisible) {
			if (%obj.isSelectingCard) {
				%obj.stopCardSelect();
			}
			%obj.hideCards();
		}
		parent::onMount(%this, %obj, %slot);
	}

	function Player::activateStuff(%this) {
		if (%this.isSelectingCard) {
			return;
		}
		%s = getWords(%this.getEyeTransform(), 0, 2);
		%e = vectorAdd(vectorScale(%this.getEyeVector(), 10), %s);
		%masks = $TypeMasks::fxBrickObjectType;
		%ray = containerRaycast(%s, %e, %masks, %this);
		%hitloc = getWords(%ray, 1, 3);

		initContainerBoxSearch(%hitloc, "3 3 1", $TypeMasks::StaticShapeObjectType);
		%next = containerSearchNext();
		talk("found? " @ %next);
		if (isObject(%next) && %next.card !$= "") {
			%this.addCard(%next.card);
			%next.delete();
			return;
		}
		return parent::activateStuff(%this);
	}
};
activatePackage(SelectCards);









function placeCard(%pl, %pos, %card, %down) {
	%leftVec = vectorCross(%pl.getUpVector(), %pl.getForwardVector());
	
	%x = getWord(%pl.getForwardVector(), 0);
	%zRot = mACos(vectorDot(%leftVec, "1 0 0")) * (%x/mAbs(%x));
	%rot = eulerToMatrix(0 SPC 0 SPC %zRot);
	
	%cardShape = new StaticShape(CardShapes) {
		dataBlock = CardShape;
		card = %card;
	};
	%cardShape.setTransform(%pos SPC %rot);
	if (!%down) {
		%cardShape.playThread(0, cardFaceUp);
	} else {
		%cardShape.playThread(0, cardFaceDown);
	}

	cardDisplay(%cardShape, %card);
}

function Player::pickUpCard(%this, %cardShape) {
	if (%this.deck.numCards >= 13) {
		messageClient(%this.client, '', "You already have 13 cards!");
		return;
	}
	%card = %cardShape.card;
	%cardShape.delete();
	%this.addCard(%card);
}

function deleteAllCards() {
	while (isObject(CardShapes)) {
		CardShapes.delete();
	}
}




$ChipTypeCount = 5;
$ChipType0 = "1 1 1 1";
$ChipType0Cost = "1";

$ChipType1 = "1 0 0 1";
$ChipType1Cost = "5";

$ChipType2 = "0 0 1 1";
$ChipType2Cost = "20";

$ChipType3 = "0 1 0 1";
$ChipType3Cost = "100";

$ChipType4 = "0.02 0.02 0.02 1";
$ChipType4Cost = "500";

$offset0 = "-0.25 -0.25 0";
$offset1 = "0.25 0.1 0";
$offset2 = "0.1 -0.15 0";
$offset3 = "0.25 -0.15 0";
$offset4 = "-0.1 -0.25 0";

function getChipCounts(%value) {
	for (%i = $ChipTypeCount - 1; %i >= 0; %i--) {
		%ret = %ret SPC mFloor(%value / $ChipType[%i @ "Cost"]);
		%value = %value - mFloor(%value / $ChipType[%i @ "Cost"]) * $ChipType[%i @ "Cost"];
	}
	return trim(%ret);
}

function getValueOfChips(%chipVector) {
	for (%i = 0; %i < getWordCount(%chipVector); %i++) {
		%ret += getWord(%chipVector, %i) * $ChipType[%i @ "Cost"];
	}
	return %ret;
}

function fxDTSBrick::createChips(%this, %value) {
	%chipVector = getChipCounts(%value);
	%count = 0;
	for (%i = 0; %i < getWordCount(%chipVector); %i++) {
		%chipCount = getWord(%chipVector, %i);
		if (!isObject(%this.chip[%i]) && %chipCount != 0) {
			%this.chip[%count] = new Item(ChipItems) {
				datablock = ChipItem;
				canPickup = false;
			};
		} else if (isObject(%this.chip[%count]) && %chipCount == 0) {
			%this.chip[%count].delete();
			continue;
		}

		%this.chip[%count].schedule(1, setNodeColor, "ALL", $ChipType[%i]);
		%this.chip.setScale("1 1 " @ %chipCount);

		%this.chip[%count].setTransform($offset[%i]);
		%count++;
	}

	for (%i = %count; %i < getWordCount(%chipVector); %i++) {
		if (!isObject(%this.chip[%i])) {
			%this.chip[%i].delete();
		}
	}
}