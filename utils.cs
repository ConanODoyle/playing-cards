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