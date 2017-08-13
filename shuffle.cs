function shuffleDeck(%this) {
  %deck = %this.deck;
  
  %newDeck = getEmptyDeck();
  for (%i = 0; %i < %deck.numCards;) {
    %id = getRandom(%deck.numCards - 1);
    %card = %deck.removeCard(%deck.cards, %id);
    %newDeck.addCard(%card);
  }
  %deck.delete();
  %this.deck = %newDeck;
}
