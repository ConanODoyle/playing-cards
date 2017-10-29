package Chips {
	function fxDTSBrick::onRemove(%b) {
		%b.clearChips();
		parent::onRemove(%b);
	}
};
activatePackage(Chips);

$ChipTypeCount = 5;
$ChipType0 = "0.8 0.8 0.8 1";
$ChipType0Cost = "1";

$ChipType1 = "1 0 0 1";
$ChipType1Cost = "5";

$ChipType2 = "0 0 1 1";
$ChipType2Cost = "20";

$ChipType3 = "0 1 0 1";
$ChipType3Cost = "100";

$ChipType4 = "0.02 0.02 0.02 1";
$ChipType4Cost = "500";

$offset0 = "0 0 0";
$offset1 = "0.15 -0.15 0";
$offset2 = "0.15 0.15 0";
$offset3 = "-0.15 0.15 0";
$offset4 = "-0.15 -0.15 0";

function getChipCounts(%value) {
	if (%value <= 0) {
		return "0 0 0 0 0";
	}

	for (%i = $ChipTypeCount - 1; %i >= 0; %i--) {
		%ret = mFloor(%value / $ChipType[%i @ "Cost"]) SPC %ret;
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

registerInputEvent("fxDTSBrick", "onCreateChips", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onClearChips", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
// registerOutputEvent("fxDTSBrick", "createChips", "", 1);
// registerOutputEvent("fxDTSBrick", "clearChips", "", 1);
registerOutputEvent("fxDTSBrick", "toggleChipDisplay", "", 1);

function fxDTSBrick::onCreateChips(%b, %pl, %cl) {
	$InputTarget_["Self"] = %b;
	$InputTarget_["Player"] = %pl;
	$InputTarget_["Client"] = %cl;
	$InputTarget_["MiniGame"] = getMiniGameFromObject(%cl);

	%b.processInputEvent("onCreateChips", %cl);
}

function fxDTSBrick::onClearChips(%b, %pl, %cl) {
	$InputTarget_["Self"] = %b;
	$InputTarget_["Player"] = %pl;
	$InputTarget_["Client"] = %cl;
	$InputTarget_["MiniGame"] = getMiniGameFromObject(%cl);

	%b.processInputEvent("onClearChips", %cl);
}

function fxDTSBrick::createChips(%b, %cl) {
	if (!isObject(%cl.player)) {
		return;
	}

	%value = %cl.score;
	if (%value <= 0) {
		%b.clearChips();
		return;
	}
	%loc = vectorAdd(%b.getPosition(), "0 0 " @ %b.getDatablock().brickSizeZ * 0.1);

	%chipVector = getChipCounts(%value);
	%count = 0;
	%largestChipCount = 0;
	for (%i = 0; %i < getWordCount(%chipVector); %i++) {
		%chipCount = getWord(%chipVector, %i);
		if (!isObject(%b.chip[%count]) && %chipCount != 0) {
			%b.chip[%count] = new StaticShape(ChipDisplayShapes) {
				datablock = ChipShape;
			};
		} else if (isObject(%b.chip[%count]) && %chipCount == 0) {
			%b.chip[%count].delete();
		}

		if (!isObject(%b.chip[%count])) {
			continue;
		}
		%b.chip[%count].setNodeColor("ALL", $ChipType[%i]);
		%b.chip[%count].setScale("1 1 " @ %chipCount);
		// %b.chip[%count].chipValue = %chipCount * $ChipType[%i @ "Cost"];

		// %b.chip[%count].setShapeNameColor(getWords($ChipType[%i], 0, 2));
		// %b.chip[%count].setShapeName(%chipCount @ " x " @ $ChipType[%i @ "Cost"]);

		if (%chipCount > %largestChipCount) {
			%largestChipCount = %chipCount;
			%largest = %count;
		}
		%b.chip[%count].setShapeName("");
		%b.chip[%count].setShapeNameColor("1 1 1");

		%b.chip[%count].setTransform(vectorAdd($offset[%count], %loc));
		%count++;
	}

	for (%i = %count; %i < getWordCount(%chipVector); %i++) {
		if (isObject(%b.chip[%i])) {
			%b.chip[%i].delete();
		}
	}

	if (isObject(%b.chip[%largest])) {
		%b.chip[%largest].setShapeName(%value @ " - " @ %cl.name);
	}

	%b.onCreateChips(%cl.player, %cl);
	%cl.player.chipDisplayBrick = %b;
	%b.currentDisplayClient = %cl;
	%b.isDisplayingChips = 1;
}

function fxDTSBrick::clearChips(%b, %cl) {
	for (%i = 0; %i < 10; %i++) {
		if (isObject(%b.chip[%i])) {
			%b.chip[%i].delete();
		}
	}

	%b.isDisplayingChips = 0;
	%b.currentDisplayClient.player.chipDisplayBrick = "";
	%b.currentDisplayClient = "";

	%b.onClearChips(%cl.player, %cl);
}

function fxDTSBrick::toggleChipDisplay(%b, %cl) {
	if (%b.currentDisplayClient == %cl) {
		%b.clearChips(%cl);
	} else if (%b.isDisplayingChips) {
		messageClient(%cl, '', "This chip display is in use!");
		return;
	} else if (%cl.score <= 0) {
		messageClient(%cl, '', "You don't have any points to display!");
		return;
	} else if (isObject(%cl.player.chipDisplayBrick)) {
		%cl.player.chipDisplayBrick.clearChips(%cl);
		%b.createChips(%cl);
	} else {
		%b.createChips(%cl);
	}
}

function Player::placeChips(%pl, %value, %loc) {
	%chipVector = getChipCounts(%value);
	%count = 0;
	%largestChipCount = 0;
	for (%i = 0; %i < getWordCount(%chipVector); %i++) {
		%chipCount = getWord(%chipVector, %i);

		if (%chipCount > 0) {
			%chip[%count] = new StaticShape(ChipShapes) {
				datablock = ChipShape;
			};
		}

		if (!isObject(%chip[%count])) {
			continue;
		}
		%chip[%count].setNodeColor("ALL", $ChipType[%i]);
		%chip[%count].setScale("1 1 " @ %chipCount);
		// %chip[%count].chipValue = %chipCount * $ChipType[%i @ "Cost"];
		// talk(%chipValue);

		// %chip[%count].setShapeNameColor(getWords($ChipType[%i], 0, 2));
		// %chip[%count].setShapeName(%chipCount @ " x " @ $ChipType[%i @ "Cost"]);

		if (%chipCount > %largestChipCount) {
			%largestChipCount = %chipCount;
			%largest = %count;
		}
		%chip[%count].setShapeName("");
		%chip[%count].setShapeNameColor("1 1 1");

		%chip[%count].setTransform(vectorAdd($offset[%count], %loc));
		%count++;
	}

	if (%count > 0) {
		%group = new ScriptGroup(ChipShapes) { 
			center = %loc;
			value = %value;
			sourceObject = %pl;
			sourceClient = %pl.client;
		};

		for (%i = 0; %i < %count; %i++) {
			%group.add(%chip[%i]);
		}
	}

	if (isObject(%chip[%largest])) {
		%chip[%largest].setShapeName(%value);
	}

	if (isObject(%pl.chipDisplayBrick)) {
		%pl.chipDisplayBrick.createChips(%pl.client);
	}
}


////////////////////


function ChipImage::onUnmount(%this, %obj, %slot) {
	%obj.canPickupChips = 0;
	%obj.isChipsVisible = 0;
	bottomprintChipInfo(%obj);
}

function ChipImage::onMount(%this, %obj, %slot) {
	%obj.canPickupChips = $canPickUpChips || %obj.permToPickUpChips;
	%obj.isChipsVisible = 1;
	bottomprintChipInfo(%obj);
}


////////////////////



function pickUpChips(%chip, %cl) {
	%g = %chip.getGroup();
	if (!isObject(%g) || %g.value <= 0) {
		return 0;
	}

	%value = %g.value;

	%cl.score += %value;

	if (isObject(%cl.player.chipDisplayBrick)) {
		%cl.player.chipDisplayBrick.createChips(%cl);
	}
	%cl.player.emote(winStarProjectile, 1);

	%g.chainDeleteAll();
	%g.delete();
	return 1;
}

function addToChips(%chip, %value, %cl) {
	%g = %chip.getGroup();
	if (!isObject(%g) || %g.value $= "" || %g.value < 0) {
		return 0;
	}

	%origValue = %g.value;
	%type = getWord(%value, 0);
	%value = getWord(%value, 1);
	%so = %g.sourceClient;

	if (%type $= "ADD") {
		%add = " + " @ %value;
		%value = %origValue + %value;
	} else if (%type $= "MULTIPLY") {
		%add = " x " @ %value;
		%value = mFloor(%origValue * %value);
	}


	%loc = %g.center;
	%g.chainDeleteAll();
	%g.delete();

	if (%value <= 0) {
		return;
	}

	if (!isObject(%so)) {
		%cl.player.placeChips(%value, %loc);
	} else {
		%so.score += %value;
		messageClient(%cl, '', "\c3" @ %so.name @ "\c6 received \c2" @ %value @ " \c6points (" @ %origValue @ %add @ ")");
		messageClient(%so, '', "\c6You received \c2" @ %value @ " \c6points (" @ %origValue @ %add @ ")");

		if (isObject(%so.player.chipDisplayBrick)) {
			%so.player.chipDisplayBrick.createChips(%so);
		}
	}
}

function mergeNearbyChips(%startChip, %mergeMultiple, %loc, %radius) {
	// talk(" ");
	// talk("StartChip: " @ %startChip);
	%og = %startChip.getGroup();
	%so = %og.sourceClient;
	// %value = 0;

	// %og.chainDeleteAll();
	// %og.delete();

	if (%radius > 0) {
		%bounds = %radius SPC %radius SPC %radius;
	} else {
		%bounds = "1 1 1";
	}
	initContainerBoxSearch(%loc, %bounds, $TypeMasks::StaticObjectType);
	%next = containerSearchNext();

	// talk("Search Loc: " @ %loc);
	%count = 0;
	while (isObject(%next) && %count < 100) {
		// talk("obj" @ %count @ ": " @ %next SPC %next.getClassName());
		
		if (!isObject(%next.getGroup()) || %next.getGroup().value <= 0) {
			// talk("    No value, skipping");
			%next = containerSearchNext();
			continue;
		}
		// talk("    Client: " @ %next.getGroup().sourceClient);

		if (%next.getGroup().sourceClient == %so && %next.getGroup() != %og) {
			%singleSourceFound++;
		}

		%found[%count] = %next.getGroup();
		%next = containerSearchNext();
		%count++;
	}

	%addedCount = 1;
	if (!%mergeMultiple) { //(%singleSourceFound > 1) {
		for (%i = 0; %i < %count; %i++) {
			if (isObject(%found[%i]) && %found[%i].sourceClient == %so) {
				%found[%i].chainDeleteAll();
				%value += %found[%i].value;
				%found[%i].delete();
			}
		}
	} else {
		for (%i = 0; %i < %count; %i++) {
			if (isObject(%found[%i])) {
				%found[%i].chainDeleteAll();
				%value += %found[%i].value;
				%found[%i].delete();
			}
		}
	}

	if (!%mergeMultiple) { //(%singleSourceFound > 1) {
		if (!isObject(%so.player)) {
			%temp = new ScriptObject() { client = %so; };
		} else {
			%temp = %so.player;
		}
		Player::placeChips(%temp, %value, %loc);
		if (!isObject(%so.player)) {
			%temp.delete();
		}
	} else {
		Player::placeChips("", %value, %loc);
	}
}


////////////////////


function serverCmdBet(%cl, %val) {
	%val = mFloor(%val);
	%pl = %cl.player;
	if (!isObject(%pl)) {
		messageClient(%cl, '', "You cannot bet while dead!");
		return;
	} else if (%val < 0) {
		messageClient(%cl, '', "You cannot bet a value lower than 0!");
		return;
	} else if (%val > %cl.score) {
		messageClient(%cl, '', "You cannot bet more points than you have!");
		return;
	}

	%cl.player.bet = %val;
	if (!%pl.isChipsVisible) {
		messageClient(%cl, '', "\c6Bet set at " @ %val @ "!");
	}
	bottomprintChipInfo(%pl);
}

function serverCmdAddToChips(%cl, %val) {
	if (!%cl.isAdmin) {
		return;
	}

	%val = mFloor(%val);
	%pl = %cl.player;
	if (!isObject(%pl)) {
		messageClient(%cl, '', "You cannot add to chips while dead!");
		return;
	}

	%cl.player.addToChips = "ADD" SPC %val;

	bottomprintChipInfo(%pl);
}

function serverCmdMultiplyChips(%cl, %val) {
	if (!%cl.isAdmin) {
		return;
	}

	%pl = %cl.player;
	if (!isObject(%pl)) {
		messageClient(%cl, '', "You cannot multiply chips while dead!");
		return;
	}

	%cl.player.addToChips = "MULTIPLY" SPC %val;

	bottomprintChipInfo(%pl);
}

function serverCmdToggleChipPickup(%cl, %target1, %target2, %target3) {
	if (!%cl.isAdmin) {
		return;
	}

	%target = trim(%target1 SPC %target2 SPC %target3);

	if (%target $= "") {
		$canPickUpChips = !$canPickUpChips;

		for (%i = 0; %i < ClientGroup.getCount(); %i++) {
			%pl = ClientGroup.getObject(%i).player;
			if (%pl.isChipsVisible) {
				%pl.canPickupChips = $canPickUpChips || %pl.permToPickUpChips;
				bottomprintChipInfo(%pl);
			}	
		}

		messageAll('', "\c6Chip pickup has been turned " @ ($canPickUpChips ? "\c2ON" : "\c0OFF"));
	} else {
		%t = findClientByName(%target);
		if (!isObject(%t)) {
			messageClient(%cl, '', "Cannot find client with name " @ %target @ "!");
			return;
		} else if (!isObject(%t.player)) {
			messageClient(%cl, '', %t.name @ " does not have a player!");
			return;
		} else {
			%pl = %t.player;
			%pl.permToPickUpChips = !%pl.permToPickUpChips;

			messageClient(%cl, '', "\c6Chip pickup for \c3" @ %t.name @ "\c6 has been turned " @ (%pl.permToPickUpChips ? "\c2ON" : "\c0OFF"));
			messageClient(%t, '', "\c6You now " @ (%pl.permToPickUpChips ? "\c2can\c6" : "\c0cannot\c6") @ " pick up chips");

			if (%pl.isChipsVisible) {
				%pl.canPickupChips = %pl.permToPickUpChips;
				bottomprintChipInfo(%pl);
			}
		}
	}
}

function serverCmdMergeChips(%cl, %radius) {
	if (!%cl.isSuperAdmin || !isObject(%cl.player)) {
		return;
	}

	%cl.player.mergeChips = 1;
	%cl.player.mergeMultiple = 1;

	if (%radius <= 0) {
		%radius = 0;
		%cl.player.mergeMultiple = 0;
	} else if (%radius > 15) {
		%radius = 15;
	}

	%cl.player.mergeRadius = %radius * 2;

	messageClient(%cl, '', "\c6Chip merging radius set to " @ %radius @ " (" @ %radius / 0.5 @ " studs)");

	if (%cl.player.isChipsVisible) {
		bottomprintChipInfo(%cl.player);
	}
}
