function getBottomprintUI(%pl) {
	if (%pl.isSelectingCard) {
		%line1 = "<just:left>\c3Left Click";
		%line1 = %line1 @ "<just:center>\c6[\c3" @ getLongCardName(%pl.deck.getCard(%pl.currentCard)) @ "\c6]";
		%line1 = %line1 @ "                <just:Right>\c3Right Click <br><color:ffffff>";
		%line2 = "<just:left>Place " @ (%pl.placeFaceDown ? "Face Down" : "Face Up");
		%line2 = %line2 @ "<just:center>\c4Light - Toggle Place Mode               <just:right>Cycle Right ";
		return %line1 @ %line2;
	} else {
		if (%pl.deck.numCards == 0) {
			%line0 = "<just:center>You have no cards!";
			%line0 = %line0 @ " <br><just:center>\c3Right Click\c6 - Pick up cards";
		} else {
			%line0 = "<just:left>\c3Left Click\c6 - Enter Select Mode <just:right>\c3Right Click\c6 - Pick up cards";
			%line0 = %line0 @ " <br><just:center>\c6Placing " @ (%pl.placeFaceDown ? "Face Down" : "Face Up");
		}
		return %line0;
	}
}

function bottomprintCardInfo(%pl) {
	%cl = %pl.client;
	if (!isObject(%cl)) {
		return;
	}

	if (%pl.isCardsVisible) {
		%cl.bottomprint(getBottomprintUI(%pl), -1, 1);
	} else if (%pl.isDealingCards) {
		%cl.bottomprint(getBottomprintDeckUI(%pl), -1, 1);
	} else {
		%cl.bottomprint("", 0, 1);
	}
}

function getBottomprintDeckUI(%pl) {
	%cl = %pl.client;
	%deck = %pl.deckBrick.deck;

	if (!isObject(%deck)) {
		return "<just:center>You don't have a deck brick!";
	}

	%count = %deck.numCards;
	%faceUp = %pl.dealFaceUp + 0;
	%add = %pl.recentlyAddedCardCount;
	%line1 = "<just:center>\c3Deck Count: \c6" @ %count;
	if (%add > 0) {
		%line1 = %line1 @ "\c3 +" @ %add;
	} else if (%add < 0) {
		%line1 = %line1 @ "\c3 " @ %add;
	}
	%line1 = %line1 @ " <br>";

	%line2 = "<just:left>\c3Left Click<just:center>\c3Right Click               <just:right>\c3Light <br>";
	%line3 = "<just:left>\c6Place " @ (%pl.placeFaceDown ? "Face Down" : "Face Up     ");
	%line3 = %line3 @ "<just:center>\c6Pick Up                            <just:right>\c6Toggle Mode";

	return %line1 @ %line2 @ %line3;
}

function bottomprintChipInfo(%pl) {
	%cl = %pl.client;
	if (!isObject(%cl)) {
		return;
	}

	if (%pl.isChipsVisible && %pl.bet> 0) {
		%line0 = "<just:left>\c3Left Click\c6: Place down bet<just:right>\c3Right Click\c6: Cancel <br>";
		%line1 = "<just:center>\c6Current bet: \c3" @ %pl.bet @ " <br>";
		%line2 = "<just:center>\c6Total Points: \c2" @ %cl.score;
	} else if (%pl.isChipsVisible) {
		if (%pl.canPickupChips) {
			%line0 = "<just:right>\c3Right Click\c6: Pick up chips <br>";
		}

		if (%pl.addToChips !$= "") {
			%line0 = "<just:left>\c3Left Click\c6: " @ %pl.addToChips @ " to a bet" @ %line0;
		}
		%line1 = "<just:center>\c3/bet #\c6 to prepare a bet <br>";
		%line2 = "<just:center>\c6Total Points: \c2" @ %cl.score;
	}

	%cl.bottomprint(%line0 @ %line1 @ %line2, -1, 1);
}
