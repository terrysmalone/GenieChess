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

## Working Notes ##

2021/12/17

Making everything static speeds things up nut it makes testing difficult. I'm going to try to make some things static. I'll do a before benchmark to make sure things don't get too bad.

### Settings ###

var engineEvaluation = 
    performanceEvaluatorFactory.CreatePerformanceEvaluator(evaluatePerfTPositions:     true,
                                                           evaluateTestSuitePositions: true,
                                                           runFullTestSuiteEvaluation: false);
            
            //engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 6, maxThinkingSeconds: 10);
            engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 5);


====================================================================
PerfTEvaluator
====================================================================
Test positions evaluator
--------------------------------------------------------------
PerftInitial - rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 - Depth 5
PASSED - Total time: 00:00:03.4386235
--------------------------------------------------------------
Perft2 - 8/p7/8/1P6/K1k3p1/6P1/7P/8 w - - - Depth 5
PASSED - Total time: 00:00:00.0105980
--------------------------------------------------------------
Perft3 - r3k2r/p6p/8/B7/1pp1p3/3b4/P6P/R3K2R w KQkq - - Depth 5
PASSED - Total time: 00:00:01.9447173
--------------------------------------------------------------
Perft4 - 8/5p2/8/2k3P1/p3K3/8/1P6/8 b - - - Depth 5
PASSED - Total time: 00:00:00.0317347
--------------------------------------------------------------
Perft5 - r3k2r/pb3pp1/5n1p/n2p4/1p1PPB2/6P1/P2N1PBP/R3K2R b KQkq - - Depth 5
PASSED - Total time: 00:00:12.4720128
--------------------------------------------------------------
Perft6 - r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - - Depth 5
PASSED - Total time: 00:01:08.1161094
--------------------------------------------------------------
Perft7 - n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1 - Depth 5
PASSED - Total time: 00:00:01.7736479
--------------------------------------------------------------
Perft8 - 8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.3473655
--------------------------------------------------------------
Perft9 - q7/2pp3b/8/3n1k1r/R1K1N3/8/B4PP1/7Q b - - 0 1 - Depth 5
PASSED - Total time: 00:00:15.6957042
--------------------------------------------------------------
Perft10 - 3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.0535703
--------------------------------------------------------------
Perft11 - 8/8/4k3/8/2p5/8/B2P2K1/8 w - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.0428903
--------------------------------------------------------------
Perft12 - 8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1 - Depth 5
PASSED - Total time: 00:00:00.0775173
--------------------------------------------------------------
Perft13 - 5k2/8/8/8/8/8/8/4K2R w K - 0 1 - Depth 5
PASSED - Total time: 00:00:00.0369173
--------------------------------------------------------------
Perft14 - 3k4/8/8/8/8/8/8/R3K3 w Q - 0 1 - Depth 5
PASSED - Total time: 00:00:00.0355439
--------------------------------------------------------------
Perft15 - r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1 - Depth 5
PASSED - Total time: 00:00:14.0680281
--------------------------------------------------------------
Perft16 - r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1 - Depth 5
PASSED - Total time: 00:00:21.8902237
--------------------------------------------------------------
Perft17 - 2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.1433577
--------------------------------------------------------------
Perft18 - 8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.2899721
--------------------------------------------------------------
Perft19 - 4k3/1P6/8/8/8/8/K7/8 w - - 0 1 - Depth 5
PASSED - Total time: 00:00:00.0157559
--------------------------------------------------------------
Perft20 - 8/P1k5/K7/8/8/8/8/8 w - - 0 - Depth 5
PASSED - Total time: 00:00:00.0099498
--------------------------------------------------------------
Test set: BratkoKopecTestSuite
FAILED - 1/6 - Total time: 00:00:34.2597036 - Total node visited: 1,672,282
--------------------------------------------------------------
Test set: KaufmanTestSuite
FAILED - 2/6 - Total time: 00:00:40.0612468 - Total node visited: 2,936,965
--------------------------------------------------------------
Test set: LctIiTestSuite
FAILED - 1/6 - Total time: 00:00:35.7883215 - Total node visited: 2,296,790
--------------------------------------------------------------
Test set: NolotTestSuite.epd
FAILED - 0/6 - Total time: 00:00:52.0587016 - Total node visited: 4,539,605
--------------------------------------------------------------
Test set: NullMoveTestSuite.epd
FAILED - 1/5 - Total time: 00:00:14.9250732 - Total node visited: 83,544
--------------------------------------------------------------
Test set: SilentButDeadlyTestSuite
FAILED - 2/6 - Total time: 00:00:30.7247159 - Total node visited: 2,159,927
--------------------------------------------------------------
Test set: STS1.epd
FAILED - 2/6 - Total time: 00:00:36.9919573 - Total node visited: 2,300,504
--------------------------------------------------------------
Test set: STS2.epd
FAILED - 0/6 - Total time: 00:00:25.8350581 - Total node visited: 1,827,661
--------------------------------------------------------------
Test set: STS3.epd
FAILED - 2/6 - Total time: 00:01:01.3549578 - Total node visited: 4,130,832
--------------------------------------------------------------
Test set: STS4.epd
FAILED - 2/6 - Total time: 00:00:33.0831932 - Total node visited: 2,067,315
--------------------------------------------------------------
Test set: STS5.epd
FAILED - 2/6 - Total time: 00:00:45.9343113 - Total node visited: 3,423,804
--------------------------------------------------------------
Test set: STS6.epd
FAILED - 1/6 - Total time: 00:00:41.8288283 - Total node visited: 2,748,029
--------------------------------------------------------------
Test set: STS7.epd
FAILED - 4/6 - Total time: 00:00:42.4708532 - Total node visited: 2,882,127
--------------------------------------------------------------
Test set: STS8.epd
FAILED - 1/6 - Total time: 00:00:54.3953863 - Total node visited: 3,815,458
--------------------------------------------------------------


