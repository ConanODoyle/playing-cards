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