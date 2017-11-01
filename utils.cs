$MinimumScore = 50;
$FreeScoreTimeout = 120000;

package MinScore {
	function GameConnection::spawnPlayer(%cl) {
		%ret = parent::spawnPlayer(%cl);

		if (%cl.score <= 0 && getSimTime() - %cl.lastReceivedPoints > $FreeScoreTimeout) {
			%cl.score = $MinimumScore;
			messageClient(%cl, '', "\c2You received " @ $MinimumScore @ " free points for being broke (2 minute cooldown)");
			%cl.lastReceivedPoints = getSimTime();
		} else if (%cl.score <= 0) {
			messageClient(%cl, '', "\c2You need to wait " @ ($freescoretimeout - getSimTime() - %cl.lastReceivedPoints) @ " for your next set of free points!");
		}

		return %ret;
	}
};
activatePackage(MinScore);

function getIntList(%start, %end) {
	while (%start < %end) {
		%ret = %ret SPC mFloor(%start);
		%start++;
	}
	return trim(%ret);
}

function getWordIndex(%str, %word) {
	%ret = -1;
	for (%i = 0; %i < getWordCount(%str); %i++) {
		if (getWord(%str, %i) $= %word) {
			%ret = %i;
			break;
		}
	}
	return %ret;
}

function addWord(%str, %word, %index) {
	%low = getWords(%str, 0, %index - 1);
	%high = getWords(%str, %index, strLen(%str) - 1);
	return trim(trim(%low) SPC %word SPC trim(%high));
}

function getCompassVec(%cl) {
	%fv = vectorScale(%cl.getControlObject().getForwardVector(), 1 / mSqrt(2));
	%v = vectorAdd(%fv, "0.5 0.5 0");
	return mFloor(getWord(%v, 0)) SPC mFloor(getWord(%v, 1)) SPC "0";
}

function getBrickShiftDirection(%cl, %x, %y, %z) {
	// talk(%x SPC %y SPC %z);
	if (%z != 0) {
		return %z > 0 ? "UP" : "DOWN";
	}

	if (%x != 0) {
		return %x > 0 ? "FORWARD" : "BACKWARD";
	} else if (%y != 0) {
		return %y > 0 ? "LEFT" : "RIGHT";
	} else {
		return "NONE";
	}

	// %compassVec = getCompassVec(%cl);
	// switch$ (%compassVec) {
	// 	case "1 0 0":   %dir = "EAST";
	// 		if (%x != 0) {
	// 			return %x > 0 ? "FORWARD" : "BACKWARD";
	// 		} else if (%y != 0) {
	// 			return %y > 0 ? "LEFT" : "RIGHT";
	// 		} else {
	// 			return "NONE";
	// 		}
	// 	case "0 1 0":   %dir = "NORTH";
	// 		if (%x != 0) {
	// 			return %x > 0 ? "LEFT" : "RIGHT";
	// 		} else if (%y != 0) {
	// 			return %y > 0 ? "FORWARD" : "BACKWARD";
	// 		} else {
	// 			return "NONE";
	// 		}
	// 	case "-1 0 0":  %dir = "WEST";
	// 		if (%x != 0) {
	// 			return %x > 0 ? "BACKWARD" : "FORWARD";
	// 		} else if (%y != 0) {
	// 			return %y > 0 ? "RIGHT" : "LEFT";
	// 		} else {
	// 			return "NONE";
	// 		}
	// 	case "0 -1 0":  %dir = "SOUTH";
	// 		if (%x != 0) {
	// 			return %x > 0 ? "RIGHT" : "LEFT";
	// 		} else if (%y != 0) {
	// 			return %y > 0 ? "BACKWARD" : "FORWARD";
	// 		} else {
	// 			return "NONE";
	// 		}
	// 	default: 		error("getBrickShiftDirection - unable to find shift dir from object " @ %cl.getControlObject() @ " with shift params" @ %x SPC %y SPC %z @ "!");
	// 					return "ERROR";
	// }
}


//first in first out queue
function queuePush(%queueID, %val) {
	%idx = $Queue[%queueID @ "_pushIDX"] + 0;

	$Queue[%queueID @ "_val" @ %idx] = %val;

	$Queue[%queueID @ "_pushIDX"]++;
}

function queuePop(%queueID) {
	%idx = $Queue[%queueID @ "_popIDX"] + 0;
	$Queue[%queueID @ "_popIDX"]++;

	if ($Queue[%queueID @ "_val" @ %idx] $= "") {
		deleteVariables("$Queue" @ %queueID @ "*");
	}

	return $Queue[%queueID @ "_val" @ %idx];
}

function queueCount(%queueID) {
	return $Queue[%queueID @ "_pushIDX"] - $Queue[%queueID @ "_popIDX"];
}

function queueContains(%queueID, %val) {
	for (%i = $Queue[%queueID @ "_popIDX"] + 0; %i < $Queue[%queueID @ "_pushIDX"]; %i++) {
		if ($Queue[%queueID @ "_val" @ %i] $= %val) {
			return 1;
		}
	}
	return 0;
}

function queueRemoveID(%queueId, %idToRemove) {
	%start = %idToRemove + 1;

	if (%idToRemove >= $Queue[%queueID @ "_pushIDX"]) {
		return;
	} else if (%idToRemove < $Queue[%queueID @ "_popIDX"]) {
		return;
	}

	for (%i = $Queue[%queueID @ "_popIDX"] + %idToRemove; %i < $Queue[%queueID @ "_pushIDX"] - 1; %i++) {
		$Queue[%queueID @ "_val" @ %i] = $Queue[%queueID @ "_val" @ (%i + 1)];
	}
	$Queue[%queueID @ "_val" @ %i] = "";
	$Queue[%queueID @ "_pushIDX"]--;
}

function queueGetID(%queueID, %val) {
	%count = 0;
	for (%i = $Queue[%queueID @ "_popIDX"] + 0; %i < $Queue[%queueID @ "_pushIDX"]; %i++) {
		if ($Queue[%queueID @ "_val" @ %i] $= %val) {
			return %count;
		}
		%count++;
	}
	return -1;
}

function serverCmdJoinBlackjackQueue(%cl) {
	if (queueContains("BlackjackQueue", %cl.bl_id TAB %cl.name)) {
		messageClient(%cl, '', "You already are in the queue!");
		return;
	}

	queuePush("BlackjackQueue", %cl.bl_id TAB %cl.name);
	messageClient(%cl, '', "\c2You joined the Blackjack queue. Use /listBlackjackQueue to view the queue.");
}

function serverCmdListBlackjackQueue(%cl) {
	%queueID = "BlackjackQueue";
	messageClient(%cl, '', "\c7- Blackjack Queue -");
	for (%i = $Queue[%queueID @ "_popIDX"] + 0; %i < $Queue[%queueID @ "_pushIDX"]; %i++) {
		%data = $Queue[%queueID @ "_val" @ %i];
		%client = findClientByBL_ID(getWord(%data, 0));
		%name = getField(%data, 1);

		if (!isObject(%client)) {
			%name = %name SPC "(LEFT SERVER)";
		}
		messageClient(%cl, '', "\c2" @ (%i + 1 - $Queue[%queueID @ "_popIDX"]) @ ". " @ %name);
	}
}

function serverCmdLeaveBlackjackQueue(%cl) {
	if (!queueContains("BlackjackQueue", %cl.bl_id TAB %cl.name)) {
		messageClient(%cl, '', "You are not in the queue!");
		return;
	}

	queueRemoveID("BlackjackQueue", queueGetID("BlackjackQueue", %cl.bl_id TAB %cl.name));
	messageClient(%cl, '', "You left the Blackjack queue.");
}

function serverCmdPopBlackjackQueue(%cl) {
	if (!%cl.isAdmin) {
		return;
	}

	%next = queuePop("BlackjackQueue");
	%targ = findClientByBL_ID(getWord(%next, 0));
	while (!isObject(%targ) && %next !$= "") {
		%next = queuePop("BlackjackQueue");
		%targ = findClientByBL_ID(getWord(%next, 0));
	}

	if (isObject(%targ)) {
		messageClient(%cl, '', "\c3" @ %targ.name @ " has been added to the blackjack table!");

		if (isObject(%targ.player)) {
			%targ.player.delete();
		}

		if (isObject(_blackjack_spawn)) {
			%targ.createPlayer(_blackjack_spawn.getSpawnPoint());
			%targ.player.removeItemDB(CardsOutItem);
			%targ.player.addItem(ChipItem);
		} else {
			%targ.createPlayer(%cl.getControlObject().getTransform());
		}
	} else {
		messageClient(%cl, '', "Nobody found in queue...");
	}
}

function serverCmdLeaveGame(%cl) {
	if (getSimTime() - %cl.lastLeftGame > 1000 && isObject(%cl.player)) {
		%cl.instantRespawn();
		%cl.lastLeftGame = getSimTime();
	}
}

function Player::removeItemDB(%pl, %db) {
	%cl = %pl.client;

	for (%i = 0; %i < %pl.getDatablock().maxTools; %i++) {
		if (%pl.tool[%i].getID() == %db.getID()) {
			%pl.tool[%i] = 0;
			if (isObject(%cl)) {
				messageClient(%cl, 'MsgItemPickup', "", %i, 0, 1);

				if (%pl.currTool == %i) {
					serverCmdUnuseTool(%cl);
				}
			}
		}
	}
}

function Player::addItem(%this, %item) {
	%item = %item.getID();
	%cl = %this.client;
	for(%i = 0; %i < %this.getDatablock().maxTools; %i++) {
		%tool = %this.tool[%i];
		if (%tool == 0) {
			%this.tool[%i] = %item.getID();
			%this.weaponCount++;
			messageClient(%cl, 'MsgItemPickup', '', %i, %item.getID());
			break;
		}
	}
}
