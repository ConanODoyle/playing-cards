# playing-cards

a playing card/casino chips add-on made for the game Blockland

## general instructions:
1) use an event to create a deck on a brick `onActivate > Self > setDeckCount [# of decks of cards in the stack]`
2) use an event to link that deck brick to you `onActivate > Self > toggleDeckBrick` - can be done in the same event sequence as setting the deck count
3) use an event to shuffle the deck on a brick `onActivate/onRelay > Self > shuffleDeck`
4) use the deck item to place cards down, UI will show the controls
5) use the card item to pick up and look at cards (and place them down again if necessary) (only needed if you are playing a game like texas, blackjack can be done with only the dealer having a card item/deck item)
6) use the chip item to place down bets - admins have extra commands that let them add to or multiply a stack of chips and return the value to the person who put the chips down. if the placer of some chips place some more close to the original stack, they'll automatically merge
7) use an event to display the number of chips you have on top of a 1x1 stud onActivate > Self > toggleChipDisplay

deck items pick up cards into the bottom of the deck

## chip slash commands:
- /bet [amount]

### admin only:
- /addToChips [amount] - sets chip item mode to add chips to a stack, then return to the chip stack owner
- /multiplyChips [amount] - sets chip item mode to multiply the amount of chips on the next stack you click, then return it to the chip stack owner 
- /toggleChipPickup [optional name] useful for blackjack/etc when you want only one person able to control who gets their chips back or not. globally turns it on/off if no name is specified
- /mergeChips [radius] - changes the radius used to search for nearby chips when placing additional bets
- /clearAllPlacedChips 
