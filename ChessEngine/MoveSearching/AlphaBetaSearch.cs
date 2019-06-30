using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
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

        private readonly Board m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
 
        private List<MoveValueInfo> m_InitialMoves;
        private List<Tuple<int, PieceMoves>> m_InitialMovesIterativeDeepeningShuffleOrder;

        private int m_KillerMovesToStore = 2;
        private PieceMoves[,] m_KillerMoves;

        private int m_EvaluationDepth;

        private PieceMoves? m_BestMoveSoFar;

        private int m_NullMoveR = 1;

        private int m_MaxCheckExtension = 10;

        public AlphaBetaSearch(Board boardPosition, IScoreCalculator scoreCalculator)
        {
            m_BoardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        static bool m_SearchCompleted = false;
        private static PieceMoves m_ReturnedMove;

        public PieceMoves CalculateBestMoveTimed(int maxDepth, int maxThinkingSeconds)
        {

            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.RunWorkerAsync(maxDepth);
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();

            //while (m_SearchCompleted == false)
            //{
            //    Thread.Sleep(10);
            //}

            return m_ReturnedMove;
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            CalculateBestMove(Convert.ToInt32(e.Argument));
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_SearchCompleted = true;
        }

        public PieceMoves CalculateBestMove(int maxDepth)
        {
            var toMove = m_BoardPosition.WhiteToMove ? "white" : "black";
            s_Log.Info($"Calculating move for {toMove}");
            s_Log.Info(FenTranslator.ToFENString(m_BoardPosition.GetCurrentBoardState()));

            m_KillerMoves = new PieceMoves[maxDepth, m_KillerMovesToStore]; // Try to a depth of maxDepth with 5 saved each round

            m_InitialMoves = new List<MoveValueInfo>();
            m_InitialMovesIterativeDeepeningShuffleOrder = new List<Tuple<int, PieceMoves>>();

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

                m_EvaluationDepth = depth;

                // Calculate the best move at the current depth
                bestMove = CalculateBestMove(depth, out var bestScore);

                m_ReturnedMove = bestMove;

                timer.Stop();

                var speed = new TimeSpan(timer.Elapsed.Ticks);

                var moveValueInfo = new MoveValueInfo
                {
                    Move = bestMove,
                    Score = bestScore,
                    DepthTime = speed,
                    AccumulatedTime = m_InitialMoves.Count > 0
                        ? m_InitialMoves[m_InitialMoves.Count - 1].AccumulatedTime.Add(speed)
                        : speed,
                    NodesVisited = CountDebugger.Evaluations
                };

                m_InitialMoves.Add(moveValueInfo);

#if FullNodeCountDebug
                LogTranspositionCounts();
#endif

#if Debug
                //LogKillerMoves(m_KillerMoves);
#endif
                s_Log.Info($"Move info: {UciMoveTranslator.ToUciMove(moveValueInfo.Move)} - " +
                           $"score: {GetScoreString(moveValueInfo.Score, m_BoardPosition.WhiteToMove)} - " +
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
            }

            s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(bestMove)}");

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
            if (m_InitialMovesIterativeDeepeningShuffleOrder.Count > 0)
            {
                moveList = OrderFromIterativeDeepeningMoves();
            }
            else
            {
                moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));
                
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

                m_BoardPosition.MakeMove(move, false);

                // Since we're swapping colours at the next depth invert alpha and beta
                var score = -AlphaBeta(-beta, -alpha, depth - 1, allowNull: true, isNullSearch: false, movePath, extensionDepth: 0);

                if (score > bestScore)
                {
                    bestMove = move;
                    bestScore = score;
                }

                m_InitialMovesIterativeDeepeningShuffleOrder.Add(new Tuple<int, PieceMoves>(score, move));

                m_BoardPosition.UnMakeLastMove();
                
                //Print move path
                s_Log.Info($"Move: {UciMoveTranslator.ToUciMove(move)} - " +
                           $"Score: {GetScoreString(score, m_BoardPosition.WhiteToMove)} - " +
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
            var hash = TranspositionTable.ProbeTable(m_BoardPosition.Zobrist, depthLeft, alpha, beta);
            
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
                if (extensionDepth < m_MaxCheckExtension && BoardChecking.IsKingInCheck(m_BoardPosition, m_BoardPosition.WhiteToMove))
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
            if (allowNull && depthLeft > m_NullMoveR && !BoardChecking.IsKingInCheck(m_BoardPosition, m_BoardPosition.WhiteToMove))
            {
                m_BoardPosition.SwitchSides();
                //We don't care about the PV path. Maybe we should implement this differently
                // Set extensionDepth to m_MaxCheckExtension because we don't want it to get
                // extended after the null move check
                var eval = -AlphaBeta(-beta, 
                                      -beta + 1, 
                                      depthLeft - m_NullMoveR - 1, 
                                      allowNull: false, 
                                      isNullSearch: true, 
                                      new List<PieceMoves>(), 
                                      extensionDepth: m_MaxCheckExtension); 

                m_BoardPosition.SwitchSides();

                if (eval >= beta)
                {
                    return eval; // Cutoff
                }
            }

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));
            
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
                m_BestMoveSoFar = null;

                // Call this just to get the best move
                // We don't care about the PV path. Maybe we should implement this differently
                // Set extensionDepth to m_MaxCheckExtension because we don't want any further deepening
                AlphaBeta(alpha, 
                          beta, 
                          depthLeft - 1, 
                          allowNull: false, 
                          isNullSearch: false, 
                          new List<PieceMoves>(), 
                          extensionDepth: m_MaxCheckExtension); 

                if (m_BestMoveSoFar != null)
                {
                    OrderMovesInPlace(moveList, depthLeft, m_BestMoveSoFar);
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
            var colourToMove = m_BoardPosition.WhiteToMove;

            var noMovesAnalysed = true;

            var pvMove = true;
            
            foreach (var move in moveList)
            {
                // Since we do pseudo legal move generation we need to validate 
                // any castling moves, otherwise skip to the next iteration of the loop
                if (move.SpecialMove == SpecialMoveType.KingCastle || move.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    // If king is in check he can't move
                    if (BoardChecking.IsKingInCheck(m_BoardPosition, colourToMove))
                    {
                        continue;
                    }

                    if (!MoveGeneration.ValidateCastlingMove(m_BoardPosition, move))
                    {
                        continue;
                    }
                }

                m_BoardPosition.MakeMove(move, false);

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
                if (BoardChecking.IsKingInCheck(m_BoardPosition, colourToMove))
                {
                    m_BoardPosition.UnMakeLastMove();
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
                                       allowNull: true, 
                                       isNullSearch: false, 
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
                                       allowNull: true, 
                                       isNullSearch: false, 
                                       bestPath, 
                                       extensionDepth);

                    // If it fails high do a full search
                    if (score > alpha && score < beta)
                    {
                        score = -AlphaBeta(-beta, 
                                           -alpha, 
                                           depthLeft - 1, 
                                           allowNull: true, 
                                           isNullSearch: false, 
                                           new List<PieceMoves>(), 
                                           extensionDepth);
                    }
                }

                noMovesAnalysed = false;

                m_BoardPosition.UnMakeLastMove();

                // Save the best move so far for the transposition table
                if (score > bestScoreSoFar)
                {
                    bestMoveSoFar = move;
                    bestScoreSoFar = score;

                    m_BestMoveSoFar = move;

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
                        if (move == m_KillerMoves[depthLeft-1,i])
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
                            m_KillerMoves[depthLeft - 1, i + 1] = m_KillerMoves[depthLeft - 1, i];
                        }

                        //Insert killer move at index 0
                        m_KillerMoves[depthLeft - 1, 0] = move;
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
                           Key      = m_BoardPosition.Zobrist,
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
            
            if (m_BoardPosition.WhiteToMove)
            {
                score = m_ScoreCalculator.CalculateScore(boardPosition);
            }
            else
            {
                score = -m_ScoreCalculator.CalculateScore(boardPosition);
            }

            return score;
        }

        // Keep examining down the tree until all moves are quiet.
        // Quiet moves are ones without captures
        private int QuiescenceEvaluate(int alpha, int beta, List<PieceMoves> pvPath)
        {
            var pvPosition = pvPath.Count;

            // Check transposition table
            var hash = TranspositionTable.ProbeQuiescenceTable(m_BoardPosition.Zobrist, alpha, beta);

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

            var evaluationScore = Evaluate(m_BoardPosition);

            if (evaluationScore >= beta)
            {
                RecordQuiescenceHash(beta, HashNodeType.LowerBound);

                return beta;
            }

            if (evaluationScore >= alpha)
            {
                alpha = evaluationScore;
            }

            var moves = MoveGeneration.CalculateAllCapturingMoves(m_BoardPosition);

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

                m_BoardPosition.MakeMove(move, false);
                
                evaluationScore = -QuiescenceEvaluate(-beta, -alpha, currentPath);

                m_BoardPosition.UnMakeLastMove();

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
                Key      = m_BoardPosition.Zobrist,
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

            m_InitialMovesIterativeDeepeningShuffleOrder =
                m_InitialMovesIterativeDeepeningShuffleOrder.OrderByDescending(i => i.Item1).ToList();

            foreach (var move in m_InitialMovesIterativeDeepeningShuffleOrder)
            {
                moveList.Add(move.Item2);
            }

            m_InitialMovesIterativeDeepeningShuffleOrder.Clear();

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

        private void OrderMovesByMvvVlaNew(IList<PieceMoves> moveList)
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
                    var victimType = BoardChecking.GetPieceTypeOnSquare(m_BoardPosition, moveList[moveNum].Moves);

                    ordering.Add(new Tuple<PieceMoves, int, int>(
                                     moveList[moveNum],
                                     GetPieceScore(victimType),
                                     GetPieceScore(moveList[moveNum].Type)));

                    //toRemove.Add(moveNum);
                }
            }

            //Order by victim and then attacker. We do it in reverse
            ordering = ordering.OrderByDescending(o => o.Item2).ThenBy(o => o.Item3).ToList();

            foreach (var order in ordering)
            {
                if (moveList.Remove(order.Item1))
                {

                    moveList.Add(order.Item1);
                }
                else
                {
                    Console.WriteLine("This shouldn't happen");
                }
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
                    var victimType = BoardChecking.GetPieceTypeOnSquare(m_BoardPosition, moveList[moveNum].Moves);

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
                var killerMove = m_KillerMoves[depth-1, slot];

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
            var movesToEnd = (m_EvaluationDepth - depth) + 1;  //Since we want lower depth mates to score lower

            var isInCheck = BoardChecking.IsKingInCheck(m_BoardPosition, m_BoardPosition.WhiteToMove);

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
            return m_InitialMoves;
        }
    }
}