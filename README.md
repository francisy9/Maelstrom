
# Maelstrom
To test the game
1. Open 3 clients through ParrellSync 
2. 1 client as server only, other 2 as client

## Francis
Current:
- Continuing mirror logic in play, refactoring code to better suit request response structure
- Complete opponent play card function and initialize enemy board

TODO:
- Convert end turn button to authority of person being in turn
- Card in hand display (tap to zoom, by default shrunk fanned out visual)
- Card positions (ensure boards are mirrored + preview of card placement by player)


## Max
Current:

TODO:
- Look to store user card decks (slack me when you get a chance, but would look at cardStatsSO, deck field in Player.cs to see how cards are brought in)
- Deck building/card collection scene (separate scene to make parallel work easier), since syncing would just be how the card are initialized