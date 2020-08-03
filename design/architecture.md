# Architecture

THIS DRAFT VERSION IS A WORK IN PROGRESS AND NOT YET AGREED UPON.

The goal of this document is to present an agreed-upon development trajectory
that will enable us to deliver the best possible plateform in the shortest possible timeframe.

## Technical Stack

On the client side, we will use [Vue.js](https://vuejs.org), [PixiJS](https://www.pixijs.com)
and [Socket.io](https://socket.io) for an HTML5 in-browser platform,
as it will help us provide compatibility for  all systems and mobile devices.
All interactions should be compatible with mobile devices to make the mobile version easier to craft.

On the server side, [Python](https://www.python.org) or [NodeJS](https://nodejs.org) could be used.

## Common principles

The following points are the major design decisions.

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

### Game creation and tournaments

Creating a game should give access to an invite link. There will be no game matching server.
This will allow us to run multiple game servers in parallel, for an overall stability guarantee.
In the future, the platform could help organise a tournament or event with a separate link for event invite,
then auto-generation of subsequent invite links for individual games.
Ideally, the tournament organisation page could be used for offline tournaments too.
Moreover, a tournament structure could enable a judge role who could see all hidden cards.

### Players accounts and deck lists

Players need to have an account to play on the platform. Deck lists are not stored on the server.
A link to an online version of a deck on [Amaranth](https://amaranth.vtes.co.nz) can be used to load a deck.
Links to [Amaranth](https://amaranth.vtes.co.nz) deck lists can be saved in the player profile for future uses.
The current version of the deck is loaded from [Amaranth](https://amaranth.vtes.co.nz) as the game begins.
A deck list in text format can be imported and used, but it will not be stored for future uses.
[Amaranth](https://amaranth.vtes.co.nz),
[FELDB](https://app.assembla.com/spaces/korni/documents) and
[ARDB](http://www.nongnu.org/anarchdb/)
text export of deck lists can be used too.

Ideally, an OAuth process should enable a player to link his [Amaranth](https://amaranth.vtes.co.nz) account
in order to be able to access his decks and choose from the whole list.

## User stories and development trajectory

This is the current proposed development trajectory to achieve our goal.
Each step defines a core set of features to develop.
Each subsequent step lists additional features until a full working version can be reached.

### Proof of concept

The first step is very basic and can be used to demonstrate the rendering, packaging and execution.
The POC will be hosted and accessible for use online ()

Features:

- Display a hand and a crypt
- Move a card from the hand to the table
- Zoom on a card
- Add and remove blood
- Add and remove pool
- Get a vampire out from the crypt
- Lock and unlock
- Face up and face down

Test:

- Card display and manipulation
- Packaging and execution on target systems

### Alpha

Features:

- Play a basic 2 players game completely (niche mechanics not required, see Beta)
- Load a deck from an Amaranth "share" link
- Draw and Discard
- Handle life counters
- Edge management
- Take control of a card (should be the same as the edge)
- Turn management
- Game log (chat not required)
- Rollback to the previous game state

Test:

- deck load
- multi-player system

### Beta

Features:

- Player account (nickname, decks, current games)
- Log & chat interface
- 4 and 5 players game, all card
- Check decks legality (grouping, cards count, and banned list)
- Choose seat at game start
- Spectator mode (silent in tournament game)
- Game can continue if a player disconnects and reconnects
- Game can continue if the server is rebooted
- All counters (Anarch, Corruption, Disease, Pathos, ...)
- Notes (Rack target, Toreador Grand Ball, Call the Great Beast disciplines)
- Unlock all (with possibility to prevent unlock for infernal minions)
- Remove a card from play
- Random discard
- Random reveal (Fortune Teller)
- Put a card on the bottom of Crypt or Library (Heart of Nizchetus, Chain of Command)
- Reveal hand, library or crypt to specific or all players, all or top X
- Play hidden card (Concealed weapon, Shattering Crescendo)
- Secret bet (Malkavian Prank, Cracking the wall)
- Seating order change (Dramatic Upheaval)
- Flip a coin (Gemini's Mirror, Walk Through Arcardia)
- 6-sided die roll (J. Oswald ""Ozzy"" Hyde-White)
- ⍰ Rock / Paper / Scissors (nice to have, can be achieved with secret bet)
- ⍰ Play card as if they're in hand (Agaitas, possibly nothing specific required)

Test:

- Full working version, 4/5 players games
- Open test to all players for bug reports, until V1 is achieved

### V1

V1 has the same scenario as beta, with bugs fixed.

## Backlog

These additional features are out of scope and may or may not be developed for V1:

- Seat randomiser at game start
- Rulings for the card when zoom-displaying it
- Easy declaration of discipline choice when playing a card
- Card images generation (Bitmap image, vector text)
- Discipline adjustment on a vampire (if we handle the card image generation)
- Game matching API/hook for easy interface with bots, websites, etc.
- Deck builder
- Collection index
- Game clock
- Cards scripting (eg. replace, cost, starting life, requirements check, ...)
- Cards rulings
- Search for first X (Vast Wealth)
- Rollback to a known previous state (ie. click the log, rollback the whole action, the whole turn)
- Auto shuffle deck on draw rollbacks when in tournament mode
- Change play order (Reversal of Fortunes - banned)
