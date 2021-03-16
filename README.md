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

The scoring criteria is taken from an external xml file, attributing values to certain characteristics.
These range from piece values, to positional advantages, to strategical factors

#### Move searching ####

The search strategy used is a negamax implementation alpha beta pruning with iterative deepening.

##### Iterative deepening #####

Iterative deepening starts by searching for the best move at a depth of one, while increasing the depth up to the max search depth (or allowed time). At the beginning of every new depth search the moves are ordered by their scores in the previous depth search. The idea behind this is to increase the number of pruned branches by starting with the highest scoring moves first. This should negate the extra searches that would normally be required from running the search multiple times.

* Transposition table
* Check extensions
* Null move pruning
* Move ordering - Most Valuable Victim - Least Valuable Aggressor
* Killer moves
* Zobrist hashing

## Code ##

### Evaluating the Engine ###

To test the speed of the engine, go to EngineEvaluation > Program. Set the things to be tested

* evaluatePerfTPositions
* evaluateTestSuitePositions
* runFullTestSuiteEvaluation

and the max thinking depth (maxDepth)

