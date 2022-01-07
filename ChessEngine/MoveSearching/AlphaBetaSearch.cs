using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
using ChessEngine.Exceptions;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using log4net;

namespace ChessEngine.MoveSearching
{
    // The basic algorithm performs a negamax alpha-beta pruning
    // using iterative deepening
    public sealed class AlphaBetaSearch
    {
        private static readonly ILog s_Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Board _boardPosition;
        private readonly IScoreCalculator _scoreCalculator;

        private PieceMover _pieceMover;
 
        private List<MoveValueInfo> _initialMoves;
        private List<Tuple<int, PieceMoves>> _initialMovesIterativeDeepeningShuffleOrder;

        private int m_KillerMovesToStore = 2;
        private PieceMoves[,] _killerMoves;

        private int _evaluationDepth;

        private PieceMoves? _bestMoveSoFar;

        private int _nullMoveR = 1;

        private int _maxCheckExtension = 10;

        public AlphaBetaSearch(Board boardPosition, IScoreCalculator scoreCalculator)
        {
            _boardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));

            _pieceMover = new PieceMover(_boardPosition);
        }

        private static PieceMoves s_CompletedSearchBestMove;
        private static PieceMoves s_LatestDepthBestMove;
        private static PieceMoves s_InDepthBestMove;
        public ulong TotalSearchNodes;

        // See: https://stackoverflow.com/questions/2265412/set-timeout-to-an-operation
        // Task might be better
        public PieceMoves CalculateBestMove(int maxDepth, int maxThinkingSeconds)
        {
            s_CompletedSearchBestMove = new PieceMoves();
            s_LatestDepthBestMove = new PieceMoves();
            s_InDepthBestMove = new PieceMoves();

            TotalSearchNodes = 0;

            var threadTimeout = new TimeSpan(0, 0, maxThinkingSeconds);

            var moveTimer = new Stopwatch();
            moveTimer.Start();

            var moveThread = new Thread(() => CalculateBestMove(maxDepth, moveTimer));  

            s_Log.Info($"Thread Starting: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
            moveThread.Start();
            s_Log.Info($"Thread Started: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

            s_Log.Info($"Thread joining: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
            var finished = moveThread.Join(threadTimeout);
            s_Log.Info($"Thread joined: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");


            if (!finished)
            {
                s_Log.Info($"Thread aborting: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
                moveThread.Abort();
                s_Log.Info($"Thread aborted: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

                s_Log.Info("Maximum time limit reached");

                TotalSearchNodes += CountDebugger.Evaluations;
            }

            moveTimer.Stop();

            s_Log.Info($"Move Time: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");

            // Temporary
            s_Log.Info(s_CompletedSearchBestMove.Type == PieceType.None
                           ? "Completed search best move: -"
                           : $"Completed search best move: {UciMoveTranslator.ToUciMove(s_CompletedSearchBestMove)}");

            s_Log.Info(s_LatestDepthBestMove.Type == PieceType.None
                           ? "Highest depth best move: -"
                            :$"Highest depth best move: {UciMoveTranslator.ToUciMove(s_LatestDepthBestMove)}");

            s_Log.Info(s_InDepthBestMove.Type == PieceType.None
                           ? "In latest depth best move: -"
                           : $"In latest depth best move: {UciMoveTranslator.ToUciMove(s_InDepthBestMove)}");

            // Temporary end

            if (s_CompletedSearchBestMove.Type != PieceType.None)
            {
                s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(s_CompletedSearchBestMove)}");

                return s_CompletedSearchBestMove;
            }

            if (s_LatestDepthBestMove.Type != PieceType.None)
            {
                s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(s_LatestDepthBestMove)}");

                return s_LatestDepthBestMove;
            }

            if (s_InDepthBestMove.Type != PieceType.None)
            {
                s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(s_InDepthBestMove)}");

                return s_InDepthBestMove;
            }

            throw new ChessBoardException("No move was found in given time");
        }

        public PieceMoves CalculateBestMove(int maxDepth, Stopwatch moveTimer = null)
        {
            var toMove = _boardPosition.WhiteToMove ? "white" : "black";
            s_Log.Info($"Calculating move for {toMove}");
            s_Log.Info(FenTranslator.ToFENString(_boardPosition.GetCurrentBoardState()));

            _killerMoves = new PieceMoves[maxDepth, m_KillerMovesToStore]; // Try to a depth of maxDepth with 5 saved each round

            _initialMoves = new List<MoveValueInfo>();
            _initialMovesIterativeDeepeningShuffleOrder = new List<Tuple<int, PieceMoves>>();

            TranspositionTable.ResetAncients();

            var bestMove = new PieceMoves();

            CountDebugger.ClearTranspositionValues();
            
            // Calculate scores for each move, starting at a
            // depth of one and working to max
            for (var depth = 1; depth <= maxDepth; depth++)
            {
#if UCI
                Console.WriteLine($"info depth {depth}");
#endif
                s_Log.Info("=========================================================================================");
                s_Log.Info($"Depth {depth}");

                CountDebugger.ClearNodesAndEvaluations();

                var timer = new Stopwatch();

                timer.Start();

                _evaluationDepth = depth;

                // Calculate the best move at the current depth
                bestMove = CalculateBestMove(depth, out var bestScore);

                s_LatestDepthBestMove = bestMove;

                // Reset this after each depth to make sure any move is from the current depth
                s_InDepthBestMove = new PieceMoves();

                timer.Stop();

                var speed = new TimeSpan(timer.Elapsed.Ticks);

                var moveValueInfo = new MoveValueInfo
                {
                    Move = bestMove,
                    Score = bestScore,
                    DepthTime = speed,
                    AccumulatedTime = _initialMoves.Count > 0
                        ? _initialMoves[_initialMoves.Count - 1].AccumulatedTime.Add(speed)
                        : speed,
                    NodesVisited = CountDebugger.Evaluations
                };

                if (moveTimer != null)
                {
                    s_Log.Info($"Depth {depth} found: {moveTimer.Elapsed:mm\':\'ss\':\'ffff}");
                }


                _initialMoves.Add(moveValueInfo);

#if FullNodeCountDebug
                LogTranspositionCounts();
#endif

#if Debug
                //LogKillerMoves(m_KillerMoves);
#endif
                s_Log.Info($"Move info: {UciMoveTranslator.ToUciMove(moveValueInfo.Move)} - " +
                           $"score: {GetScoreString(moveValueInfo.Score, _boardPosition.WhiteToMove)} - " +
                           $"nodes: {moveValueInfo.NodesVisited} - " +
                           $"time at depth: {moveValueInfo.DepthTime:mm\':\'ss\':\'ffff} - " +
                           $"Accumulated move time: {moveValueInfo.AccumulatedTime:mm\':\'ss\':\'ffff}");

#if UCI
                var foundMove = UciMoveTranslator.ToUciMove(bestMove);
                Console.WriteLine($"Best move at depth {depth}: {foundMove}");

                Console.WriteLine($"info currmove {bestMove} " +
                                  $"depth {depth} " +
                                  $"nodes {moveValueInfo.NodesVisited} ");

                Console.WriteLine($"info score cp 0 {bestMove} " +
                                  $"depth {depth} " +
                                  $"nodes {moveValueInfo.NodesVisited} " +
                                  $"time {moveValueInfo.DepthTime:mm\':\'ss\':\'ffff}");

                Console.WriteLine($"info score cp {moveValueInfo.Score} " +
                                  $"depth {depth} " +
                                  $"nodes {moveValueInfo.NodesVisited} pv {bestMove} ");
#endif

                TotalSearchNodes += CountDebugger.Evaluations;
            }

            s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(bestMove)}");

            s_CompletedSearchBestMove = bestMove;

            return bestMove;
        }

        private PieceMoves CalculateBestMove(int depth, out int bestScore)
        {
            var alpha = int.MinValue / 2 - 1;
            var beta = int.MaxValue / 2 + 1;

            bestScore = alpha;

            var bestMove = new PieceMoves();

            List<PieceMoves> moveList;

            // Order the initial moves by their scores from the last depth, if any.
            // Otherwise order them based on....
            if (_initialMovesIterativeDeepeningShuffleOrder.Count > 0)
            {
                moveList = OrderFromIterativeDeepeningMoves();
            }
            else
            {
                moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(_boardPosition));
                
                //OrderMovesInPlaceByEvaluation(moveList);
                OrderMovesInPlace(moveList, depth);
            }

            s_Log.Info("---------------------------------------------------------");
            s_Log.Info($"Moves to Check: {moveList.Count}");
            s_Log.Info("Shown in order checked");

            for (var i = 0; i < moveList.Count; i++)
            {
                var move = moveList[i];
#if UCI
                Console.WriteLine($"info currmove {UciMoveTranslator.ToUciMove(move)} currmovenumber {i + 1}");
#endif

                var movePath = new List<PieceMoves>();

                _pieceMover.MakeMove(move, false);

                // Since we're swapping colours at the next depth invert alpha and beta
                var score = -AlphaBeta(-beta, -alpha, depth - 1, true, false, movePath, 0);

                if (score > bestScore)
                {
                    bestMove = move;
                    bestScore = score;

                    s_InDepthBestMove = bestMove;
                }

                _initialMovesIterativeDeepeningShuffleOrder.Add(new Tuple<int, PieceMoves>(score, move));

                _pieceMover.UnMakeLastMove();
                
                //Print move path
                s_Log.Info($"Move: {UciMoveTranslator.ToUciMove(move)} - " +
                           $"Score: {GetScoreString(score, _boardPosition.WhiteToMove)} - " +
                           $"Best path: { string.Join(", ", movePath.Select(UciMoveTranslator.ToUciMove)) }");
            }

            return bestMove;
        }

        // Breakdown of steps
        // 1. Check the transposition table for the same position
        // 2. If we are at depth 0 move to a quiescence search, or if in check, extend the depth
        // 3. Decide whether to do a null move check or not
        // 4. Generate moves
        // 5. If we think it's suitable, do an internal iterative deepening search for a best move, 
        //    otherwise order moves by best hash move, killer moves and MVV/LVA
        // 6. Loop through the moves recursively calling AlphaBeta
        // 7. Record alpha, beta or evaluation to the transposition table
        private int AlphaBeta(int alpha, int beta, int depthLeft, bool allowNull, bool isNullSearch, List<PieceMoves> pvPath, int extensionDepth)
        {
            var pvPosition = pvPath.Count;
            
            var positionValue = int.MinValue / 2 + 1;

            PieceMoves? bestHashMove = null;

            // Check transposition table
            var hash = TranspositionTable.ProbeTable(_boardPosition.Zobrist, depthLeft, alpha, beta);
            
            if (hash.Key !=0)
            {
                var transpositionScore = hash.Score;

                if (hash.BestMove.Type != PieceType.None)
                {
                    bestHashMove = hash.BestMove;
                }

                // We use the best move even if the depth is shallower.
                // We only want the 
                if (hash.Depth >= depthLeft)
                {
                    switch (hash.NodeType)
                    {
                        case HashNodeType.Exact:
                            return transpositionScore;
                        case HashNodeType.LowerBound:
                            alpha = Math.Max(alpha, transpositionScore);
                            break;
                        case HashNodeType.UpperBound:
                            beta = Math.Min(beta, transpositionScore);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (alpha >= beta)
                    {
                        return transpositionScore;
                    }
                }
            }

            // If in check do a check extension otherwise perform quiescence search
            if (depthLeft == 0)
            {
                if (extensionDepth < _maxCheckExtension && BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove))
                {
                    depthLeft++;
                    extensionDepth++;
                }
                else
                {
                    return QuiescenceEvaluate(alpha, beta, pvPath);
                }
            }

            // Null move check 
            if (allowNull && depthLeft > _nullMoveR && !BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove))
            {
                _boardPosition.SwitchSides();
                //We don't care about the PV path. Maybe we should implement this differently
                // Set extensionDepth to m_MaxCheckExtension because we don't want it to get
                // extended after the null move check
                var eval = -AlphaBeta(-beta, 
                                      -beta + 1, 
                                      depthLeft - _nullMoveR - 1, 
                                      false, 
                                      true, 
                                      new List<PieceMoves>(), 
                                      _maxCheckExtension); 

                _boardPosition.SwitchSides();

                if (eval >= beta)
                {
                    return eval; // Cutoff
                }
            }

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));
            
            // There are no possible moves. It's either check mate or stale mate
            if (moveList.Count == 0)
            {
                return EvaluateEndGame(depthLeft);
            }

            // Internal iterative deepening
            // If we're near the last node of this search and we have no best move from
            // the transposition table perform a limited iterative deepening search
            if (!isNullSearch && bestHashMove == null && depthLeft > 3)
            {
                _bestMoveSoFar = null;

                // Call this just to get the best move
                // We don't care about the PV path. Maybe we should implement this differently
                // Set extensionDepth to m_MaxCheckExtension because we don't want any further deepening
                AlphaBeta(alpha, 
                          beta, 
                          depthLeft - 1, 
                          false, 
                          false, 
                          new List<PieceMoves>(), 
                          _maxCheckExtension); 

                if (_bestMoveSoFar != null)
                {
                    OrderMovesInPlace(moveList, depthLeft, _bestMoveSoFar);
                }
                else
                {
                    //This never seems to be hit. I'm not sure if it's needed
                    OrderMovesInPlace(moveList, depthLeft, null);
                }
            }
            else
            {
                OrderMovesInPlace(moveList, depthLeft, bestHashMove);
            }

            PieceMoves? bestMoveSoFar = null;
            var bestScoreSoFar = positionValue;

            // Check the colour before moving since it's this colour king we have to check for 
            // legal move generation.
            // Plus, we only have to do it once for all moves.
            var colourToMove = _boardPosition.WhiteToMove;

            var noMovesAnalysed = true;

            var pvMove = true;
            
            foreach (var move in moveList)
            {
                // Since we do pseudo legal move generation we need to validate 
                // any castling moves, otherwise skip to the next iteration of the loop
                if (move.SpecialMove == SpecialMoveType.KingCastle || move.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    // If king is in check he can't move
                    if (BoardChecking.IsKingInCheck(_boardPosition, colourToMove))
                    {
                        continue;
                    }

                    if (!MoveGeneration.ValidateCastlingMove(_boardPosition, move))
                    {
                        continue;
                    }
                }

                _pieceMover.MakeMove(move, false);

                // Futility pruning
                //if (depthLeft == 1 &&
                //    move.SpecialMove != SpecialMoveType.Capture &&
                //    move.SpecialMove != SpecialMoveType.ENPassantCapture &&
                //    !IsPromotionCapture(move.SpecialMove))
                //{
                //    if (Evaluate(m_BoardPosition) + 1.25m < alpha)
                //    {
                //        continue;
                //    }
                //}

                // Since we do pseudo legal move generation we need to check if this move is legal
                // otherwise skip to the next iteration of the loop
                if (BoardChecking.IsKingInCheck(_boardPosition, colourToMove))
                {
                    _pieceMover.UnMakeLastMove();
                    continue;
                }
                
                var bestPath = new List<PieceMoves>();

                int score;

                // if this is the first (pv move) then do a full search
                if (pvMove)
                {
                    score = -AlphaBeta(-beta, 
                                       -alpha, 
                                       depthLeft - 1, 
                                       true, 
                                       false, 
                                       bestPath, 
                                       extensionDepth);
                    pvMove = false;
                }
                else
                {
                    // Principal variation search - Do a search with a narrow aspiration window
                    score = -AlphaBeta(-alpha - 1, 
                                       -alpha, 
                                       depthLeft - 1, 
                                       true, 
                                       false, 
                                       bestPath, 
                                       extensionDepth);

                    // If it fails high do a full search
                    if (score > alpha && score < beta)
                    {
                        score = -AlphaBeta(-beta, 
                                           -alpha, 
                                           depthLeft - 1, 
                                           true, 
                                           false, 
                                           new List<PieceMoves>(), 
                                           extensionDepth);
                    }
                }

                noMovesAnalysed = false;

                _pieceMover.UnMakeLastMove();

                // Save the best move so far for the transposition table
                if (score > bestScoreSoFar)
                {
                    bestMoveSoFar = move;
                    bestScoreSoFar = score;

                    _bestMoveSoFar = move;

                    pvPath.RemoveRange(pvPosition, pvPath.Count - pvPosition);
                    pvPath.Add(move);
                    pvPath.AddRange(bestPath);
                    
                }

                positionValue = Math.Max(alpha, score);

                alpha = Math.Max(alpha, positionValue);

                if (alpha >= beta)
                {
                    RecordHash(depthLeft, score, HashNodeType.LowerBound, bestMoveSoFar);

                    // Check if the current move is already in the killer move list
                    var duplicate = false;

                    for (var i = 0; i < m_KillerMovesToStore; i++)
                    {
                        if (move == _killerMoves[depthLeft-1,i])
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (duplicate == false)
                    {
                        //Shift killer moves to the right
                        for (var i = m_KillerMovesToStore - 2; i >= 0; i--)
                        {
                            _killerMoves[depthLeft - 1, i + 1] = _killerMoves[depthLeft - 1, i];
                        }

                        //Insert killer move at index 0
                        _killerMoves[depthLeft - 1, 0] = move;
                    }

                    return score;
                }
            }

            if (noMovesAnalysed)
            {
                return EvaluateEndGame(depthLeft);
            }

            // transposition table store
            var hashNodeType = positionValue <= alpha ? HashNodeType.UpperBound 
                                                      : HashNodeType.Exact;

            RecordHash(depthLeft, positionValue, hashNodeType, bestMoveSoFar);

            return positionValue;
        }

        private void RecordHash(int depth, int score, HashNodeType hashNodeType, PieceMoves? bestMove = null)
        {
            var hash = new Hash
                       {
                           Key      = _boardPosition.Zobrist,
                           Depth    = depth,
                           NodeType = hashNodeType,
                           Score    = score
                       };

            if (bestMove != null)
            {
                hash.BestMove = (PieceMoves)bestMove;
            }

            TranspositionTable.Add(hash);
        }

        /// Evaluates the score relative to the current player
        /// i.e. A high score means the position is better for the current player
        private int Evaluate(Board boardPosition)
        {
            int score;
            
            if (_boardPosition.WhiteToMove)
            {
                score = _scoreCalculator.CalculateScore(boardPosition);
            }
            else
            {
                score = -_scoreCalculator.CalculateScore(boardPosition);
            }

            return score;
        }

        // Keep examining down the tree until all moves are quiet.
        // Quiet moves are ones without captures
        private int QuiescenceEvaluate(int alpha, int beta, List<PieceMoves> pvPath)
        {
            var pvPosition = pvPath.Count;

            // Check transposition table
            var hash = TranspositionTable.ProbeQuiescenceTable(_boardPosition.Zobrist, alpha, beta);

            PieceMoves? bestHashMove = null;

            if (hash.Key != 0)
            {
                var transpositionScore = hash.Score;

                if (hash.BestMove.Type != PieceType.None)
                {
                    bestHashMove = hash.BestMove;
                }

                switch (hash.NodeType)
                {
                    case HashNodeType.Exact:
                        return transpositionScore;
                    case HashNodeType.LowerBound:
                        alpha = Math.Max(alpha, transpositionScore);
                        break;
                    case HashNodeType.UpperBound:
                        beta = Math.Min(beta, transpositionScore);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (alpha >= beta)
                {
                    return transpositionScore;
                }
            }

            var evaluationScore = Evaluate(_boardPosition);

            if (evaluationScore >= beta)
            {
                RecordQuiescenceHash(beta, HashNodeType.LowerBound);

                return beta;
            }

            if (evaluationScore >= alpha)
            {
                alpha = evaluationScore;
            }

            var moves = MoveGeneration.CalculateAllCapturingMoves(_boardPosition);

            if (moves.Count == 0)
            {
                // We are effectively returning the max of evaluationScore and alpha
                return alpha;
            }
            
            OrderMovesByMvvVla(moves);

            if (bestHashMove != null)
            {
                BringBestHashMoveToTheFront(moves, (PieceMoves)bestHashMove);
            }

            PieceMoves? bestMoveSoFar = null;

            foreach (var move in moves)
            {
                var currentPath = new List<PieceMoves> { move };

                _pieceMover.MakeMove(move, false);
                
                evaluationScore = -QuiescenceEvaluate(-beta, -alpha, currentPath);

                _pieceMover.UnMakeLastMove();

                if (evaluationScore >= beta)
                {
                    RecordQuiescenceHash(beta, HashNodeType.LowerBound);

                    return beta;
                }

                if (evaluationScore > alpha)
                {
                    alpha = evaluationScore;

                    bestMoveSoFar = move;

                    pvPath.RemoveRange(pvPosition, pvPath.Count - pvPosition);
                    pvPath.AddRange(currentPath);
                }
            }

            // transposition table store
            var hashNodeType = evaluationScore <= alpha ? HashNodeType.UpperBound
                                                        : HashNodeType.Exact;

            RecordQuiescenceHash(alpha, hashNodeType, bestMoveSoFar);

            return alpha;
        }

        private void RecordQuiescenceHash(int evaluationScore, HashNodeType hashNodeType, PieceMoves? bestMove = null)
        {
            var hash = new Hash
            {
                Key      = _boardPosition.Zobrist,
                NodeType = hashNodeType,
                Score    = evaluationScore
            };

            if (bestMove != null)
            {
                hash.BestMove = (PieceMoves)bestMove;
            }

            TranspositionTable.AddQuiescenceHash(hash);
        }

        private List<PieceMoves> OrderFromIterativeDeepeningMoves()
        {
            var moveList = new List<PieceMoves>();

            _initialMovesIterativeDeepeningShuffleOrder =
                _initialMovesIterativeDeepeningShuffleOrder.OrderByDescending(i => i.Item1).ToList();

            foreach (var move in _initialMovesIterativeDeepeningShuffleOrder)
            {
                moveList.Add(move.Item2);
            }

            _initialMovesIterativeDeepeningShuffleOrder.Clear();

            return moveList;
        }

        // Order all moves by MVV/LVA
        private void OrderMovesInPlace(IList<PieceMoves> moveList, int depth)
        {
            OrderMovesByMvvVla(moveList);

            BringKillerMovesToTheFront(moveList, depth);
        }

        private void OrderMovesInPlace(IList<PieceMoves> moveList, int depth, PieceMoves? bestHashMove)
        {
            OrderMovesByMvvVla(moveList);

            BringKillerMovesToTheFront(moveList, depth);

            if (bestHashMove != null)
            {
                BringBestHashMoveToTheFront(moveList, (PieceMoves)bestHashMove);
            }
        }

        private void OrderMovesByMvvVla(IList<PieceMoves> moveList)
        {
            // move list position, victim score, attacker score
            var ordering = new List<Tuple<PieceMoves, int, int>>();

            var toRemove = new List<int>();

            //Move capture
            for (var moveNum = 0; moveNum < moveList.Count; moveNum++)
            {
                if (moveList[moveNum].SpecialMove == SpecialMoveType.Capture
                    || moveList[moveNum].SpecialMove == SpecialMoveType.ENPassantCapture
                    || IsPromotionCapture(moveList[moveNum].SpecialMove))
                {
                    var victimType = BoardChecking.GetPieceTypeOnSquare(_boardPosition, moveList[moveNum].Moves);

                    ordering.Add(new Tuple<PieceMoves, int, int>(
                        moveList[moveNum],
                        GetPieceScore(victimType),
                        GetPieceScore(moveList[moveNum].Type)));

                    toRemove.Add(moveNum);
                }
            }

            //We need to remove them in reverse so we don't change the numbers
            // of the others to remove
            toRemove = toRemove.OrderByDescending(t => t).ToList();

            foreach (var remove in toRemove)
            {
                moveList.RemoveAt(remove);
            }

            //Order by victim and then attacker. We do it in reverse
            ordering = ordering.OrderByDescending(o => o.Item2).ThenBy(o => o.Item3).ToList();
            
            for (var orderPosition = 0; orderPosition < ordering.Count; orderPosition++)
            {
                moveList.Insert(orderPosition, ordering[orderPosition].Item1);
            }
        }

        private void BringKillerMovesToTheFront(IList<PieceMoves> moveList, int depth)
        {
            for (var slot = 0; slot < m_KillerMovesToStore; slot++)
            {
                var killerMove = _killerMoves[depth-1, slot];

                // There are no more killer moves at this depth
                if (killerMove.Type == PieceType.None)
                {
                    break;
                }

                for (var i = 0; i < moveList.Count; i++)
                {
                    var move = moveList[i];

                    if (move == killerMove)
                    {
                        var toMove = moveList[i];
                        moveList.RemoveAt(i);
                        moveList.Insert(0, toMove);

                        break;
                    }
                }
            }
        }

        private static void BringBestHashMoveToTheFront(IList<PieceMoves> moveList, PieceMoves bestHashMove)
        {
            if (moveList.Remove(bestHashMove))
            {
                moveList.Insert(0, bestHashMove);
            }
        }

        private static bool IsPromotionCapture(SpecialMoveType specialMoveType)
        {
            return    specialMoveType == SpecialMoveType.BishopPromotionCapture
                   || specialMoveType == SpecialMoveType.KnightPromotionCapture
                   || specialMoveType == SpecialMoveType.RookPromotionCapture
                   || specialMoveType == SpecialMoveType.QueenPromotionCapture;
        }

        private int GetPieceScore(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return 1;
                case PieceType.Knight:
                    return 2;
                case PieceType.Bishop:
                    return 3;
                case PieceType.Rook:
                    return 4;
                case PieceType.Queen:
                    return 5;
                case PieceType.King:
                    return 6;
                case PieceType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null);
            }

            return 0;
        }

        // Evaluates the end game relative to the current player
        // (i.e. A low score if the current player loses)
        private int EvaluateEndGame(int depth)
        {
            var movesToEnd = (_evaluationDepth - depth) + 1;  //Since we want lower depth mates to score lower

            var isInCheck = BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove);

            if (isInCheck)
            {
                return int.MinValue / 2 + movesToEnd; // Player is in checkmate
            }
            else
            {
                return 0; //stalemate
            }
        }

        private void LogTranspositionCounts()
        {
            s_Log.Info("----------------------------------------------------------------------------------");
            s_Log.Info("TRANSPOSITION DEBUGGING");
            s_Log.Info($"Times hash added:              {CountDebugger.Transposition_HashAdded}");
            s_Log.Info($"Times hash replaced:           {CountDebugger.Transposition_HashReplaced}");
            s_Log.Info($"Times probed:                  {CountDebugger.Transposition_Searches}");
            s_Log.Info($"Times hash returned:           {CountDebugger.Transposition_HashFound}");
            s_Log.Info($"Times keys matched:            {CountDebugger.Transposition_MatchCount}");
            s_Log.Info($"Times keys and depths matched: {CountDebugger.Transposition_MatchAndUsed}");
            s_Log.Info($"Times not matched:             {CountDebugger.Transposition_Searches - CountDebugger.Transposition_MatchAndUsed}");
            s_Log.Info("----------------------------------------------------------------------------------");
        }

        private static void LogKillerMoves(PieceMoves[,] killerMoves)
        {
            s_Log.Info("----------------------------------------------------------------------------------");
            s_Log.Info("KILLER MOVES");

            for (var depth = 0; depth < killerMoves.GetLength(0); depth++)
            {
                var killerMovesList = new List<string>();

                for (var move = 0; move < killerMoves.GetLength(1); move++)
                {
                    var killerMove = killerMoves[depth, move];
                    
                    if (killerMove.Type != PieceType.None)
                    {
                        killerMovesList.Add(UciMoveTranslator.ToUciMove(killerMoves[depth, move]));
                    }
                }

                if (killerMovesList.Any())
                {
                    s_Log.Info($"Depth: {depth + 1} - {string.Join(",", killerMovesList)}");
                }
            }

            s_Log.Info("----------------------------------------------------------------------------------");
        }

        private static string GetScoreString(int score, bool whiteToMove)
        {
            //We try to maximise the scores so for display purposes negate them for black
            if (!whiteToMove)
            {
                score = -score;
            }

            var scoreString = score.ToString(CultureInfo.InvariantCulture);

            if (score > int.MaxValue / 3)
            {
                var movesToMate = ((int.MaxValue / 2) - score) / 2;

                scoreString = $"+M{movesToMate}";
            }
            else if (score < int.MinValue / 3)
            {
                var movesToMate = Math.Abs((int.MinValue / 2 - score) / 2);

                scoreString = $"-M{movesToMate}";
            }

            return scoreString;
        }

        public List<MoveValueInfo> GetMoveValueInfo()
        {
            return _initialMoves;
        }
    }
}