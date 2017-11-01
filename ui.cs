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
			%line0 = "<just:right>\c3Right Click\c6: Cancel <br>";
			%line0 = "<just:left>\c3Left Click\c6: " @ %pl.addToChips @ " to a bet" @ %line0;
		} else if (%pl.mergeChips) {
			%line0 = "<just:right>\c3Right Click\c6: Cancel <br>";
			%radius = %pl.mergeRadius > 0 ? "(Radius: " @ %pl.mergeRadius @ ")" : "";
			%line0 = "<just:left>\c3Left Click\c6: Merge overlapping chips " @ %radius @ %line0;
		}
		%line1 = "<just:center>\c3/bet #\c6 to prepare a bet <br>";
		%line2 = "<just:center>\c6Total Points: \c2" @ %cl.score;
	}

	%cl.bottomprint(%line0 @ %line1 @ %line2, -1, 1);
}

function serverCmdCardsHelp(%cl) {
	messageClient(%cl, '', "\c3/clearAllCards \c6- Removes all placed cards");
	messageClient(%cl, '', "\c3/clearAllCardData \c6- Removes all placed cards and any held cards");
	messageClient(%cl, '', "\c3/clearAllPlacedChips \c6- Removes all placed chips");
	messageClient(%cl, '', "\c3/bet # \c6- Prepares a bet, rounded down");
	messageClient(%cl, '', "\c3/addToChips # \c6- Prepares a payout, rounded down, adds to the chip set you click");
	messageClient(%cl, '', "\c7            Automatically returns it to the owner's pile");
	messageClient(%cl, '', "\c3/multiplyChips # \c6- Prepares a multiplier payout, adds to the chip set you click");
	messageClient(%cl, '', "\c7            Automatically returns it to the owner's pile, 0 is a valid number");
	messageClient(%cl, '', "\c3/toggleChipPickup [name] \c6- Toggles if chips can be picked up");
	messageClient(%cl, '', "\c7            Include a name to only toggle for one person, none to toggle everyone");
	messageClient(%cl, '', "\c3/mergeChips # \c6- Turns on chip merging using the chip item, default radius is 1 stud");
	messageClient(%cl, '', "\c7            Include a number to set a radius (ie, merge all chips on a table)");
}

function serverCmdCardHelp(%cl) {
	serverCmdCardsHelp(%cl);
}

function serverCmdQueueHelp(%cl) {
	messageClient(%cl, '', "\c3/JoinBlackjackQueue \c6- Joins the Blackjack queue");
	messageClient(%cl, '', "\c3/LeaveBlackjackQueue \c6- Leaves the Blackjack queue");
	messageClient(%cl, '', "\c3/ListBlackjackQueue \c6- Lists everyone in the Blackjack Queue");
	messageClient(%cl, '', "\c3/LeaveGame \c6- Respawns you (aka exit the current game, if you are in one)");
	messageClient(%cl, '', "\c3/PopBlackjackQueue \c6- Spawns the next person in the queue at the blackjack spawn");
	messageClient(%cl, '', "\c7            If it cannot find the blackjack spawn, it uses your player.");
	messageClient(%cl, '', "\c7            Spawned players have no card item, but have the chip item.");
}