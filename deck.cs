
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

function getShuffledDeck() {
	%deck = getNewDeck().shuffle();
	return %deck;
}


////////////////////


function DeckSO::addCard(%this, %card) {
	if (%card $= "") {
		error("Cannot add card: no card value!");
		return;
	}

	%this.cards = trim(%this.cards SPC %card);
	%this.numCards++;
}

function DeckSO::removeCard(%this, %idx) {
	if (%this.numCards <= 0 || !getWordCount(%this.cards)) {
		// error("Cannot remove card: No cards to remove!");
		return -1;
	} else if (%idx < 0 ||  %idx >= %this.numCards) {
		// error("Cannot remove card: Index out of range!");
		return -1;
	}

	%ret = getWord(%this.cards, %idx);
	%this.cards = removeWord(%this.cards, %idx);
	%this.numCards--;
	return %ret;
}

function DeckSO::getCard(%this, %idx) {
	return getWord(%this.cards, %idx);
}

function DeckSO::clearCards(%this)  {
	%this.cards = "";
	%this.numCards = 0;
}

function DeckSO::shuffleDeck(%deck) {
	for (%i = 0; %i < %deck.numCards; %i = 0) {
		%id = getRandom(%deck.numCards - 1);
		%card = %deck.removeCard(%id);
		%shuffled = %shuffled SPC %card;
		%count++;
	}
	talk(%shuffled);
	%deck.cards = trim(%shuffled);
	%deck.numCards = %count;
}


////////////////////


function getCardName(%num) {
	switch (mFloor(%num / 13)) {
		case 0: %suit = "s_";
		case 1: %suit = "h_";
		case 2: %suit = "c_";
		case 3: %suit = "d_";
	}
	
	return %suit @ (%num % 13 + 1);
}