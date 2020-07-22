# Architecture

THIS DRAFT VERSION IS A WORK IN PROGRESS AND NOT YET AGREED UPON.

The goal of this document is to present an agreed-upon development trajectory
that will enable us to deliver the best possible plateform in the shortest possible timeframe.

## Technical Stack

Two technical stacks are considered:

1. Using [Vue.js](https://vuejs.org), [PixiJS](https://www.pixijs.com) and [Socket.io](https://socket.io) for an HTML5 in-browser platform
    - Pro: Easier to provide all systems and mobile devices compatibility
    - Con: Harder to develop, will require developers to learn JS

2. Using [Unity](https://unity.com) or [Unreal engine](https://www.unrealengine.com) and an existing framework or code base:
    - Pro: Easier and faster to develop
    - Con: More work for packaging and multi-systems adaptation

## Common principles

Whatever stack is selected, the following points stand.

### Game state and journal

The game state and journal is kept on the server side, and synchronised with all clients.
The journal lists all games changes, including players chat messages.
The game state can always be reverted to a previous state by using the journal.
The game state and journal survives a server reboot, so that players can pick up where they left.

### Shared state

The full game state is not shared with all players, only revealed cards are shared with the necessary players.
The journal is shared with all players, but the exact content of some journal entries is not shared:
for example, all players know a card has been drawned, but only the affected player know which card is drawn.

### Card images, texts and rulings

Card images, text and rulings are fetched from the [KRCG API](https://api.krcg.org).
This ensure up-to-date cards texts, images and rulings while avoiding the need to keep a dedicated database up-to-date.
Ideally, either KRCG or the platform itself produce the card images from templates and official card text CSV,
to make sure keeping the cards base up-to-date is easily done.

### Deck lists

Deck lists are stored on the server side, but no interface to modify a previous deck is required.
A deck list in text format can be imported and used.
Deck lists can be fetched from [Amaranth](https://amaranth.vtes.co.nz), either from a link or in text form.
Ideally, [FELDB](https://app.assembla.com/spaces/korni/documents) and [ARDB](http://www.nongnu.org/anarchdb/)
text export of deck lists can be used too.

## User stories and development trajectory

This is the current proposed development trajectory to achieve our goal.
Each step defines a core set of features to develop.
Each subsequent step providex additional features until a full working version can be reached.

### Proof of concept

The first step is very basic and can be used to demonstrate the rendering of all considered technical stacks.
This should enable us to choose a technical stack for further development.

Features:

- Display a hand and a crypt
- Play a card
- Zoom on a card
- Add and remove blood
- Add and remove pool
- Lock and unlock

Test:

- Card display and manipulation
- Packaging and execution on target systems

### Alpha

Features:

- Play a basic 2 players game completely (niche mechanics not required, see Beta)
- Load a deck
- Draw and Discard
- Handle life counters
- Edge management 
- Turn management
- Game log (chat not required)
- Rollback game state

Test:

- deck load
- multi-player system

### Beta

Features:

- Player account (nickname, deck lists, current games)
- Game matching interface ("Looking for game")
- Log & chat interface
- 4 and 5 players game, all card 
- Game can continue if a player disconnects and reconnects
- Game can continue if the server is rebooted
- All counters (Anarch, Corruption, Disease, Pathos, ...)
- Notes (Rack target, Toreador Grand Ball, Call the Great Beast disciplines)
- Remove a card from play
- Random discard 
- Put a card on the bottom of Crypt or Library (Heart of Nizchetus, Chain of Command)
- Reveal hand, library or crypt to specific or all players, all or top X
- Play hidden card (Concealed weapon, Shattering Crescendo)
- Random reveal (Fortune Teller)
- Change play order (Reversal of Fortunes)
- Secret bet (Malkavian Prank, Cracking the wall)
- Seating order change (Dramatic Upheaval)

Test:

- Full working version, 4/5 players games
- Open test to all players for bug reports, until V1 is achieved

### V1

V1 has the same scenario as beta, with bugs fixed.

## Backlog

These additional features are out of scope and may or may not be developed for V1:

- Easy declaration of discipline choice when playing a card
- Game matching API/hook for easy interface with bots, websites, etc.
- Deck builder
- Collection index
- Seat randomiser at game start
- Game clock
- Cards scripting (eg. replace, cost, starting life, requirements check, ...)
- Cards rulings 
- Search for first X (Vast Wealth)
