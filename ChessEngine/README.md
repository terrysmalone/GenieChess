# Genie #

# Setup #

There are two ways to run the engine:

## UCI Communication Protocol ##

The main way, is using the UCI communication protocol (http://wbec-ridderkerk.nl/html/UCIProtocol.html). This allows various methods of input and output, all of which are implemented in various external UI programs. 

First, compile and build using the UCIDebug or UCIRelease configuration.

Currently, CuteChess is the best one I've found.

## Command Line ##

It can also be run from the command line (usually for debugging).

### Commands ###

* `-help` for info on basic commands
* `print` displays the current board

### Defaults ###

* **Opening book** - as default an opening book is used
* **Thinking depth** - the default thinking depth is 7 
* **Search strategy** - the default strategy is alphabeta

# Engine #

## Features ##

### Opening book ###

The opening book is loaded by passing the path of the txt file containing the opening book

### Scoring ###

The scoring criteria is taken from an external xml file, attributing values to certain characteristics.
These range from piece values, to positional advantages, to strategical factors

#### Score Values ####

##### PieceValuesScoreCalculation #####

* Individual piece value scores
* DoubleBishopScore - Score for having both bishops
* SoloQueenScore - Score if you have a queen and the opponent doesn't

##### PawnStructureScoreCalculation #####

* DoubledPawnScore
* ProtectedPawnScore
* PassedPawnScore - Score for every passed pawn
* PassedPawnAdvancementScore - Score for every square past it's home that a passed pawn has moved

##### CentralPieceScoreCalculation #####

Individual scores for every piece (including pawns, excluding king) at the centre of the board

* InnerCentralPawnScore - Score for pieces in the inner 4 central squares
* OuterCentralPawnScore - Score for pieces in the inner 12 central squares around the inner central square

##### CastlingScoreCalculation #####

* CastlingKingSideScore - Score for being castled kingside
* CastlingQueenSideScore - Score for being castled queenside
* CanCastleKingsideScore - Score for still being able to castle kingside
* CanCastleQueensideScore - Score for still being able to castle queenside

##### SquareTableScoreCalculation #####

Square tables for all squares of all pieces

##### CoverageAndAttackScoreCalculation #####

* knightCoverageScore
* BishopCoverageScore
* RookCoverageScore
* QueenCoverageScore
* AttackScore - Score for every attack on an enemy piece
* MoreValuablePieceAttackScore - Score for any piece (including pawns) attacking a more valuable piece

##### PieceDevelopmentScoreCalculation #####

* DevelopedPieceScore - Score for pieces not on the back rank
* ConnectedRookScore
* EarlyQueenMoveScore - Score for moving the queen early

##### KingPositionScoreCalculator #####

* EarlyMoveKingScore - Score for moving the king early
* KingProtectionScore - Score for every piece protecting the king












### Move searching ###

The search strategy used is a negamax implementation alpha beta pruning with iterative deepening.

#### Iterative deepening ####

Iterative deepening starts by searching for the best move at a depth of one, while increasing the depth up to the max search depth (or allowed time). At the beginning of every new depth search the moves are ordered by their scores in the previous depth search. The idea behind this is to increase the number of pruned branches by starting with the highest scoring moves first. This should negate the extra searches that would normally be required from running the search multiple times.

* Transposition table
* Check extensions
* Null move pruning
* Move ordering - Most Valuable Victim - Least Valuable Aggressor
* Killer moves
* Zobrist hashing

# Code #

## Evaluating the Engine ##

To test the speed of the engine, go to EngineEvaluation > Program. Set the things to be tested

* evaluatePerfTPositions
* evaluateTestSuitePositions
* runFullTestSuiteEvaluation

and the max thinking depth (maxDepth)

# TODO #

Migration

* Remove reference to System.Windows.Interactivity
* Migrate all Projects to PackageReference
* Remove log4Net



* Make it all .net 6



Chess Engine

* Change all instances of == null and != null to is null and is not null

WPF

* BUG: Deal with selecting promotion piece

* Highlight available moves when a piece is selected

* Allow the board to be flipped

* Add options to change colours

* Let a FULL GAME VS ai TO BE PLAYED

* Make Chess board a separate control


Large Tasks

* Pull out BoardState from Board. Pass PoardState into things rather than Board
	1. Roll history and current board state into one thing. 

* Include useful bitboards in BoardState

* Implement Magic Bitboards (using https://github.com/Tearth/Fast-Magic-Bitboards)


# Working Notes #

2021/12/17

Making everything static speeds things up nut it makes testing difficult. I'm going to try to make some things static. I'll do a before benchmark to make sure things don't get too bad.
