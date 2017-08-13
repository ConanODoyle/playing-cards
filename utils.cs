function getIntList(%start, %end) {
  while (%start < %end) {
    %ret = %ret SPC mFloor(%start);
    %start++;
  }
  return trim(%ret);
}