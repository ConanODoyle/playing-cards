
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