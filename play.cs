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
		if ((%trig == $LEFTCLICK || %trig == $RIGHTCLICK) && %val && %obj.isCardsVisible && !%obj.isSelectingCard) {
			if (%obj.deck.numCards <= 0) {
				%cl = %obj.client;
				%cl.centerprint("You cannot pick up cards with the item out!");
				%cl.schedule(50, centerprint, "\c3You cannot pick up cards with the item out!");
				%cl.schedule(100, centerprint, "You cannot pick up cards with the item out!");
				%cl.schedule(150, centerprint, "\c3You cannot pick up cards with the item out!");
				%cl.schedule(200, centerprint, "You cannot pick up cards with the item out!", 2);
				return;
			} else {
				%obj.startCardSelect();
			}
		} else if (%obj.isSelectingCard && %val == 1 && (%trig == $LEFTCLICK || %trig == $RIGHTCLICK)) {
			if (%trig == $LEFTCLICK) {
				%pl = %obj;
				%cl = %pl.client;
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
			} else {
				%obj.cycleCardSelect(1);
			}
		} else if (%val && %obj.isDealingCards) {
			if (%trig == $LEFTCLICK) {
				%pl = %obj;
				%cl = %pl.client;
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
			} else if (%trig == $RIGHTCLICK) {
				%s = getWords(%obj.getEyeTransform(), 0, 2);
				%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
				%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType;
				%ray = containerRaycast(%s, %e, %masks, %obj);
				%hitloc = getWords(%ray, 1, 3);

				initContainerBoxSearch(%hitloc, "0.5 0.5 0.5", $TypeMasks::StaticObjectType | $TypeMasks::ItemObjectType);
				%next = containerSearchNext();
				// talk("found? " @ %next);
				if (isObject(%next) && %next.card !$= "") {
					%obj.pickUpDeckCard(%next);
					bottomprintCardInfo(%obj);
					return;
				}
			}
		} else if (%trig == $LEFTCLICK && %val && !%obj.isSelectingCard) {
			%s = getWords(%obj.getEyeTransform(), 0, 2);
			%e = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %s);
			%masks = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::TerrainObjectType;
			%ray = containerRaycast(%s, %e, %masks, %obj);
			%hitloc = getWords(%ray, 1, 3);

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

	// function serverCmdPlantBrick(%cl) {
	// 	if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
	// 		%currentCard = %pl.currentCard;
	// 		if (!%pl.placeCurrentCard()) {
	// 			%cl.centerprint("Invalid location to place!");
	// 			%cl.schedule(50, centerprint, "\c3Invalid location to place!");
	// 			%cl.schedule(100, centerprint, "Invalid location to place!");
	// 			%cl.schedule(150, centerprint, "\c3Invalid location to place!");
	// 			%cl.schedule(200, centerprint, "Invalid location to place!", 2);
	// 			return;
	// 		}
	// 		%pl.stopCardSelect();
	// 		serverCmdUnuseTool(%cl);
	// 		return;
	// 	}
	// 	parent::serverCmdPlantBrick(%cl);
	// }

	// function serverCmdLight(%cl) {
	// 	if (isObject(%pl = %cl.player) && %pl.isCardsVisible) {
	// 		if (%pl.isSelectingCard) {
	// 			%pl.stopCardSelect();
	// 			if (%pl.unUsedTool) {
	// 				serverCmdUnuseTool(%cl);
	// 			}
	// 		} else {
	// 			%pl.startCardSelect();
	// 		}
	// 		return;
	// 	}
	// 	parent::serverCmdLight(%cl);
	// }

	function serverCmdShiftBrick(%cl, %x, %y, %z) {
		if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
			%dir = getBrickShiftDirection(%cl, %x, %y, %z);
			%selectCard = %pl.currentCard;
			if (%dir $= "LEFT") {
				%pl.cycleCardSelect(-1);
			} else if (%dir $= "RIGHT") {
				%pl.cycleCardSelect(1);
			} else if ((%dir $= "UP" || %dir $= "FORWARD") && !%pl.card[%selectCard].isSelected) {
				%pl.confirmCardSelect();
			} else if ((%dir $= "DOWN" || %dir $= "BACKWARD") && %pl.card[%selectCard].isSelected) {
				%pl.confirmCardSelect();
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
				%pl.confirmCardSelect();
			} else if ((%dir $= "DOWN" || %dir $= "BACKWARD") && %pl.card[%selectCard].isSelected) {
				%pl.confirmCardSelect();
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

	// function serverCmdUseSprayCan(%cl, %colorID) {
	// 	if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
	// 		%pl.placeFaceDown = !%pl.placeFaceDown;
	// 		bottomprintCardInfo(%pl);
	// 		return;
	// 	}
	// 	parent::serverCmdUseSprayCan(%cl, %colorID);
	// }

	// function serverCmdUseFxCan(%cl, %colorID) {
	// 	if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
	// 		%pl.placeFaceDown = !%pl.placeFaceDown;
	// 		bottomprintCardInfo(%pl);
	// 		return;
	// 	}
	// 	parent::serverCmdUseFxCan(%cl, %colorID);
	// }

	// function serverCmdUseTool(%cl, %slot) {
	// 	%cl.player.unUsedTool = 0;
	// 	parent::serverCmdUseTool(%cl, %slot);
	// }

	// function serverCmdUnuseTool(%cl) {
	// 	// if (isObject(%pl = %cl.player) && %pl.isSelectingCard) {
	// 	// 	%pl.unUsedTool = 1;
	// 	// 	return;
	// 	// }
	// 	parent::serverCmdUnuseTool(%cl);
	// }
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