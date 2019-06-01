# Genie #

## Setup ##

There are two ways to run the engine:

### UCI Communication Protocol ###

The main way, is using the UCI communication protocol (http://wbec-ridderkerk.nl/html/UCIProtocol.html). This allows various methods of input and output, all of which are implemented in various external UI programs. 

First, compile and build using the UCIDebug or UCIRelease configuration.

Currently, CuteChess is the best one I've found.

### Command Line ###

It can also be run from the command line (usually for debugging).

#### Commands ####

* `-help` for info on basic commands
* `print` displays the current board
*



#### Defaults ####

* **Opening book** - as default an opening book is used
* **Thinking depth** - the default thinking depth is 7 
* **Search strategy** - the default strategy is alphabeta


## Engine ##

### Features ###

#### Opening book ####

The opening book is loaded by giving the address of the txt file containing the opening book at......

The format of the opening book is..........

If no location is given the default will be......

#### Scoring ####

The scoring criteria is taken from an external xml file, giving the scores ..............

The location should be given at........

If no location is given the default will be......

* TODO list implemented and to implement features

#### Move searching ####

##### AlphaBeta #####

The search strategy used is alpha beta [Add description]


* Iterative deepening
* Transposition table
* Check extensions
* Null move pruning
* Move ordering - OrderSimple - OrderByMVVLVA - OrderByScore
* Killer moves
* Zobrist hashing




