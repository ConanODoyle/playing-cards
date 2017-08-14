function shuffleDeck(%deck) {
  for (%i = 0; %i < %deck.numCards;) {
    %id = getRandom(%deck.numCards - 1);
    %card = %deck.removeCard(%deck.cards, %id);
    %shuffled = %shuffled SPC %card;
    %count++;
  }
  %deck.cards = trim(%shuffled);
  %deck.numCards = %count;
}