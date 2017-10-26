function Player::cycleCardSelect(%this, %dir) {
	%deck = %this.deck;
	if (!isObject(%deck)) {
		return;
	}


	%lastCard = %this.currentCard + 0;
	%this.currentCard = (%this.currentCard + %dir + %deck.numCards) % %deck.numCards;
 	
	if (isObject(%this.card[%lastCard])) {
		%this.card[%lastCard].playThread(0, root);
	}
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, cardUp);
	}
	serverPlay3D(("cardPick" @ getRandom(1, 4) @ "Sound"), %this.card[%this.currentCard].getPosition());
	bottomprintCardInfo(%this);
}

function Player::startCardSelect(%this) {
	%deck = %this.deck;
	if (!isObject(%deck)) {
		return;
	} else if (!%this.isCardsVisible) {
		%this.displayCards();
	}
    
    if (%this.currentCard $= "") {
		%this.currentCard = 0;
    }
	%this.isSelectingCard = 1;
	%this.selection = "";
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, cardUp);
	}
	serverPlay3D(("cardPick" @ getRandom(1, 4) @ "Sound"), %this.card[%this.currentCard].getPosition());
	bottomprintCardInfo(%this);
}

function Player::stopCardSelect(%this) {
	if (isObject(%this.card[%this.currentCard])) {
		%this.card[%this.currentCard].playThread(0, root);
	}
	%this.currentCard = "";
	%this.isSelectingCard = 0;
	%this.selection = "";

	%i = 0;
	while (isObject(%this.card[%i])) {
		%this.card[%i].setNodeColor("ALL", "1 1 1 1");
		%this.card[%i].isSelected = 0;
		%i++;
	}
	bottomprintCardInfo(%this);
}

function Player::confirmCardSelect(%this){
	%selectCard = %this.currentCard;
	if (!%this.card[%selectCard].isSelected) {
		%this.selection = trim(%this.selection SPC %selectCard);
		%this.card[%selectCard].isSelected = 1;
		%this.card[%selectCard].setNodeColor("ALL", "0.5 1 0.5 1");
	} else {
		%this.selection = removeWord(%this.selection, getWordIndex(%this.selection, %selectCard));
		%this.card[%selectCard].isSelected = 0;
		%this.card[%selectCard].setNodeColor("ALL", "1 1 1 1");
    }
    bottomprintCardInfo(%this);
}

$LEFTCLICK = 0;
$RIGHTCLICK = 4;

package PlayCards {
	function Armor::onTrigger(%this, %obj, %trig, %val) {
		%pl = %obj;
		%cl = %pl.client;
		%s = getWords(%obj.getEyeTransform(), 0, 2);
		%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType;

		if (%trig == $RIGHTCLICK && %val == 1) {
			if (%obj.isCardsVisible) {
				if (!%obj.isSelectingCard) { //Hand: Pick up card near hit location
					%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
					%ray = containerRaycast(%s, %e, %masks, %obj);
					%hitloc = getWords(%ray, 1, 3);

					if (!isObject(getWord(%ray, 0))) {
						return;
					}

					initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
					%next = containerSearchNext();
					// talk("found? " @ %next);
					if (isObject(%next) && %next.card !$= "") {
						%obj.pickUpCard(%next);
						if (%obj.isCardsVisible) {
							%obj.displayCards();
						}
						return;
					}
				} else { //Hand: Cycle card select if in selection mode
					%obj.cycleCardSelect(1);
				}


				return;
			} else if (%obj.isDeckVisible && isObject(%obj.deckBrick.deck)) { //Deck: Pick up card near hit location
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				if (!isObject(getWord(%ray, 0))) {
					return;
				}

				initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
				%next = containerSearchNext();
				// talk("found? " @ %next);
				if (isObject(%next) && %next.card !$= "") {
					%obj.pickUpDeckCard(%next);
					bottomprintCardInfo(%obj);
					return;
				}


				return;
			} else if (!isObject(%obj.getMountedImage(0))) { //Flip card near hit location
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				if (!isObject(getWord(%ray, 0))) {
					return;
				}

				initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
				%next = containerSearchNext();
				// talk("found? " @ %next);
				if (isObject(%next) && %next.card !$= "") {
					if (%next.down) {
						%next.playThread(0, cardFaceUp);
					} else {
						%next.playThread(0, cardFaceDown);
					}

					%next.down = !%next.down;
					serverPlay3D(("cardPick" @ getRandom(1, 4) @ "Sound"), %next.getPosition());
				}


				return;
			} else if (%obj.isChipsVisible && %obj.bet > 0) {
				%obj.bet = "";
				bottomprintChipInfo(%obj);
				return;
			} else if (%obj.canPickUpChips && getSimTime() - %obj.lastChipPickupTime > 500) {
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				if (!isObject(getWord(%ray, 0))) {
					return;
				}

				initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
				%next = containerSearchNext();
				// talk("found? " @ %next);
				if (isObject(%next)) {
					if (!pickUpChips(%next, %cl)) {
						return parent::onTrigger(%this, %obj, %trig, %val);
					}
					%obj.lastChipPickupTime = getSimTime();
					
					bottomprintChipInfo(%obj);
					return;
				}
			}

			//do not premature return if no deck or cards are out
		} else if (%trig == $LEFTCLICK && %val == 1) {
			if (%obj.isCardsVisible && %obj.deck.numCards > 0) {
				if (%obj.isSelectingCard) { //Hand: Attempt to place currently selected card
					if (!%pl.placeCurrentCard()) {
						%cl.centerprint("Invalid location to place!");
						%cl.schedule(50, centerprint, "\c3Invalid location to place!");
						%cl.schedule(100, centerprint, "Invalid location to place!");
						%cl.schedule(150, centerprint, "\c3Invalid location to place!");
						%cl.schedule(200, centerprint, "Invalid location to place!", 2);
						return;
					}

					if (%pl.deck.numCards > 0) {
						%pl.cycleCardSelect(1);
						%pl.cycleCardSelect(-1);
					} else {
						%pl.stopCardSelect();
					}
				} else { //Hand: Start card select
					%obj.startCardSelect();
				}


				return;
			} else if (%obj.isDeckVisible) { //Deck: Attempt to place top card of deck
				if (!%pl.placeDeckCard()) {
					if (%pl.deckBrick.deck.numCards <= 0) {
						return;
					}
					%cl.centerprint("Invalid location to place!");
					%cl.schedule(50, centerprint, "\c3Invalid location to place!");
					%cl.schedule(100, centerprint, "Invalid location to place!");
					%cl.schedule(150, centerprint, "\c3Invalid location to place!");
					%cl.schedule(200, centerprint, "Invalid location to place!", 2);
					return;
				}
				bottomprintCardInfo(%pl);


				return;
			} else if (%obj.isChipsVisible && %obj.bet > 0) {
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				if (!isObject(getWord(%ray, 0)) || getWords(%ray, 4, 6) !$= "0 0 1") {
					return parent::onTrigger(%this, %obj, %trig, %val);
				}

				%obj.client.score -= %obj.bet;
				%obj.placeChips(%obj.bet, %hitloc);
				%obj.bet = "";
				bottomprintChipInfo(%obj);


				return;
			} else if (%obj.addToChips !$= "") {
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				if (!isObject(getWord(%ray, 0))) {
					return;
				}

				initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
				%next = containerSearchNext();
				// talk("found? " @ %next);
				if (isObject(%next)) {
					if (!addToChips(%next, %obj.addToChips, %cl)) {
						return parent::onTrigger(%this, %obj, %trig, %val);
					}
					%obj.addToChips = "";
					bottomprintChipInfo(%obj);
					return;
				}
			}

			//do not premature return if no deck or cards are out
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

	function serverCmdShiftBrick(%cl, %x, %y, %z) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			%dir = getBrickShiftDirection(%cl, %x, %y, %z);
			%selectCard = %pl.currentCard;
			if (%dir $= "LEFT") {
				%pl.cycleCardSelect(-1);
			} else if (%dir $= "RIGHT") {
				%pl.cycleCardSelect(1);
			} else if ((%dir $= "UP" || %dir $= "FORWARD") && !%pl.card[%selectCard].isSelected) {
				// %pl.confirmCardSelect();
			} else if ((%dir $= "DOWN" || %dir $= "BACKWARD") && %pl.card[%selectCard].isSelected) {
				// %pl.confirmCardSelect();
			}
			return;
		}
		return parent::serverCmdShiftBrick(%cl, %x, %y, %z);
	}

	function serverCmdSuperShiftBrick(%cl, %x, %y, %z) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			%dir = getBrickShiftDirection(%cl, %x, %y, %z);
			%selectCard = %pl.currentCard;
			if (%dir $= "LEFT") {
				%pl.cycleCardSelect(-1);
			} else if (%dir $= "RIGHT") {
				%pl.cycleCardSelect(1);
			} else if ((%dir $= "UP" || %dir $= "FORWARD") && !%pl.card[%selectCard].isSelected) {
				// %pl.confirmCardSelect();
			} else if ((%dir $= "DOWN" || %dir $= "BACKWARD") && %pl.card[%selectCard].isSelected) {
				// %pl.confirmCardSelect();
			}
			return;
		}
		return parent::serverCmdSuperShiftBrick(%cl, %x, %y, %z);
	}

	function serverCmdLight(%cl) {
		if (isObject(%pl = %cl.player) && (%pl.isCardsVisible || %pl.isDealingCards)) {
			%pl.placeFaceDown = !%pl.placeFaceDown;
			bottomprintCardInfo(%pl);
			return;
		}
		parent::serverCmdLight(%cl);
	}
};
activatePackage(PlayCards);

function CardsOutImage::onUnmount(%this, %obj, %slot) {
	if (%obj.isCardsVisible) {
		if (%obj.isSelectingCard) {
			%obj.stopCardSelect();
		}
		%obj.hideCards();
	}
}

function CardsOutImage::onMount(%this, %obj, %slot) {
	if (!%obj.isCardsVisible) {
		%obj.displayCards();
	}
}

function Player::placeCurrentCard(%pl) {
	if (%pl.deck.getCard(%pl.currentCard) $= "") {
		return 0;
	}
	%s = getWords(%pl.getEyeTransform(), 0, 2);
	%e = vectorAdd(vectorScale(%pl.getEyeVector(), 5), %s);
	%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType;
	%ray = containerRaycast(%s, %e, %masks, %pl);
	%hitloc = getWords(%ray, 1, 3);

	if (%hitloc $= "") {
		return 0;
	} else if (getWords(%ray, 4, 6) !$= "0 0 1") {
		return 0;
	}

	initContainerBoxSearch(%hitloc, 0.05, $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
	%next = containerSearchNext();
	// talk("found? " @ %next);
	if (isObject(%next) && %next.card !$= "") {
		return 0;
	}

	serverPlay3D(("cardPlace" @ getRandom(1, 4) @ "Sound"), %hitloc);
	placeCard(%pl, %hitloc, %pl.deck.removeCard(%pl.currentCard), %pl.placeFaceDown);
	%pl.displayCards();
	return 1;
}

// function Player::placeSelectedCards(%pl) {
// 	%s = getWords(%this.getEyeTransform(), 0, 2);
// 	%e = vectorAdd(vectorScale(%this.getEyeVector(), 5), %s);
// 	%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType;
// 	%ray = containerRaycast(%s, %e, %masks, %this);
// 	%hitloc = getWords(%ray, 1, 3);


// }

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

	%cardShape.down = %down;

	cardDisplay(%cardShape, getCardName(%card));
}

function Player::pickUpCard(%pl, %cardShape) {
	if (%pl.deck.numCards >= 13) {
		messageClient(%pl.client, '', "You already have 13 cards!");
		return;
	}
	%card = %cardShape.card;
	serverPlay3D(("cardPick" @ getRandom(1, 4) @ "Sound"), %cardShape.getPosition());
	%pl.emote(winStarProjectile, 1);
	%cardShape.delete();
	%pl.addCard(%card);
}

function deleteAllCards() {
	while (isObject(CardShapes)) {
		CardShapes.delete();
	}
}