
function getEmptyDeck() {
  return new ScriptObject(Decks) {
    class = DeckSO;
    numCards = 0;
    cards = "";
  };
}

function getNewDeck() {
  return new ScriptObject(Decks) {
    class = DeckSO;
    numCards = 52;
    cards = getIntList(0, 52);
  };
}

function DeckSO::addCard(%this, %card) {
  %this.cards = trim(%this.cards SPC %card);
  %this.numCards++;
}

function DeckSO::removeCard(%this, %idx) {
  %ret = getWord(%this.cards, %idx);
  %this.cards = removeWord(%this, %idx);
  %this.numCards--;
  return %ret;
}

function DeckSO::getCard(%this, %idx) {
  return getWord(%this.cards, %idx);
}

function getCardName(%num) {
  switch (%num / 13) {
    case 0: %suit = "s_";
    case 1: %suit = "h_";
    case 2: %suit = "c_";
    case 3: %suit = "d_";
  }
  
  return %suit @ (%num % 13 + 1);
}