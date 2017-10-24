
function getEmptyDeck() {
	return new ScriptObject(Decks) {
		class = DeckSO;
		numCards = 0;
		cards = "";
	};
}

function getNewDeck(%num) {
	%deck = getEmptyDeck();
	if (%num > 0) {
		%deck.addDeck(%num);
	} else {
		%deck.addDeck(%num);
	}
	return %deck;
}

function getShuffledDeck(%num) {
	if (%num > 0) {
		%deck = getEmptyDeck();
		%deck.addDeck(%num);
	} else {
		%deck = getNewDeck();
	}
	%deck.shuffle();
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

function DeckSO::shuffle(%deck) {
	while (%deck.numCards > 0) {
		%id = getRandom(%deck.numCards - 1);
		%card = %deck.removeCard(%id);
		%shuffled = %shuffled SPC %card;
		%count++;
	}
	// talk(%shuffled);
	%deck.cards = trim(%shuffled);
	%deck.numCards = %count;
}

function DeckSO::addDeck(%deck, %numDecks) {
	for (%i = 0; %i < %numDecks; %i++) {
		%deck.cards = trim(%deck.cards SPC getIntList(0, 52));
	}
	%deck.numCards += %numDecks * 52;
}


////////////////////


function DeckSO::sortDeck(%deck) {
	%list = %deck.cards;
	%count = getWordCount(%list);

	%ctrl = new GUITextListCtrl(SortCtrl) {};
	for (%i = 0; %i < %count; %i++) {
		%val = getWord(%list, %i);
		%ctrl.addRow(%i, %val);
	}

	%ctrl.sortNumerical(0, 1);

	for (%i = 0; %i < %count; %i++) {
		%new = trim(%new SPC %ctrl.getRowText(%i));
	}

	%deck.cards = %new;
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


function getLongCardName(%num) {
	switch (mFloor(%num / 13)) {
		case 0: %suit = "Spades"; %color = "\c7";
		case 1: %suit = "Hearts"; %color = "\c0";
		case 2: %suit = "Clubs"; %color = "\c7";
		case 3: %suit = "Diamonds"; %color = "\c0";
	}

	switch (%num % 13 + 1) {
		case 11: %type = "Jack of";
		case 12: %type = "Queen of";
		case 13: %type = "King of";
		case 1: %type = "Ace of";
		case 2: %type = "Two of";
		case 3: %type = "Three of";
		case 4: %type = "Four of";
		case 5: %type = "Five of";
		case 6: %type = "Six of";
		case 7: %type = "Seven of";
		case 8: %type = "Eight of";
		case 9: %type = "Nine of";
		case 10: %type = "Ten of";
		default: %type = (%num % 13 + 1) @ " of";
	}
	
	return %color @ %type SPC %suit;
}