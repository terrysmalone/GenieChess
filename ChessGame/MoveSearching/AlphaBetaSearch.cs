using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;
using ChessGame.Debugging;
using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using log4net;

namespace ChessGame.MoveSearching
{
    // The basic algorithm performs a negamax alpha-beta pruning
    // using iterative deepening
    public sealed class AlphaBetaSearch
    {
        private static readonly ILog m_Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
 
        private List<MoveValueInfo> m_InitialMoves;
        private List<Tuple<decimal, PieceMoves>> m_InitialMovesIterativeDeepeningShuffleOrder;

        public AlphaBetaSearch(IBoard boardPosition, IScoreCalculator scoreCalculator)
        {
            m_BoardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        public PieceMoves CalculateBestMove(int maxDepth)
        {
            var toMove = m_BoardPosition.WhiteToMove ? "white" : "black";
            m_Log.Info($"Calculating move for {toMove}");

            m_InitialMoves = new List<MoveValueInfo>();
            m_InitialMovesIterativeDeepeningShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            var bestMove = new PieceMoves();

            CountDebugger.ClearTranspositionValues();

            // Calculate scores for each move, starting at a
            // depth of one and working to max
            for (var depth = 1; depth <= maxDepth; depth++)
            {
#if UCI
                Console.WriteLine($"info depth {depth}");
#endif
                m_Log.Info("----------------------------------------------------------------------------------");
                m_Log.Info($"Depth {depth}");

                CountDebugger.ClearNodesAndEvaluations();

                var timer = new Stopwatch();

                timer.Start();

                // Calculate the best move at the current depth
                bestMove = CalculateBestMove(depth, out var bestScore);

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

                //LogTranspositionCounts();

                m_Log.Info($"Move info: {UCIMoveTranslator.ToUCIMove(moveValueInfo.Move)} - " +
                           $"score: {moveValueInfo.Score} - " +
                           $"nodes: {moveValueInfo.NodesVisited} - " +
                           $"time at depth: {moveValueInfo.DepthTime.TotalMilliseconds} - " +
                           $"Accumulated move time: {moveValueInfo.AccumulatedTime.TotalMilliseconds}");

                

//#if UCI
//                var bestMove = UCIMoveTranslator.ToUCIMove();
//                //Console.WriteLine(string.Format("Best move at depth {0}: {1}", i, bestMove));
//                //Console.WriteLine(String.Format("info currmove {0} depth {1} nodes {2} ", bestMove, i, pvInfo.NodesVisited));
//                //Console.WriteLine(String.Format("info score cp 0 {0} depth {1} nodes {2} time {3} ", bestMove, i, pvInfo.NodesVisited, pvInfo.DepthTime));
//                Console.WriteLine($"info score cp {pvInfo.Score} depth {i} nodes {pvInfo.NodesVisited} pv {bestMove} ");

                //                //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UCIMoveTranslator.ToUCIMove(bestIDMove)));
                //#endif
            }

            m_Log.Info($"Found move: {UCIMoveTranslator.ToUCIMove(bestMove)}");

            return bestMove;
        }

        private PieceMoves CalculateBestMove(int depth, out decimal bestScore)
        {
            var alpha = decimal.MinValue / 2 - 1;
            var beta = decimal.MaxValue / 2 + 1;

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
                OrderMovesInPlace(moveList);
            }

#if Debug
            m_Log.Info("---------------------------------------------------------");
            m_Log.Info($"Moves to Check: {moveList.Count}");
#endif

            foreach (var move in moveList)
            {
#if UCI
                Console.WriteLine($"info currmove {UCIMoveTranslator.ToUCIMove(moveList[i])} currmovenumber {i + 1}");
#endif

                m_BoardPosition.MakeMove(move, false);

                // Since we're swapping colours at the next depth invert alpha and beta
                var score = AlphaBeta(-beta, -alpha, depth - 1);

                if (score > bestScore)
                {
                    bestMove = move;
                    bestScore = score;
                }

#if Debug
                m_Log.Info($"Move: {UCIMoveTranslator.ToUCIMove(move)} - Score: {score}");
#endif

                m_InitialMovesIterativeDeepeningShuffleOrder.Add(new Tuple<decimal, PieceMoves>(score, move));

                m_BoardPosition.UnMakeLastMove();
            }

            return bestMove;
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

        //        public PieceMoves CalculateBestMove(int depth)
        //        {
        //            var alpha = decimal.MinValue / 2 - 1;
        //            var beta = decimal.MaxValue / 2 + 1;

        //            var bestScore = alpha;
        //            var bestMove = new PieceMoves();

        //            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));

        //#if Debug
        //            m_Log.Info("======================================================");
        //            m_Log.Info($"Moves to Check: {moveList.Count}");
        //#endif

        //            for (var i = 0; i < moveList.Count; i++)
        //            {
        //#if UCI
        //                Console.WriteLine($"info currmove {UCIMoveTranslator.ToUCIMove(moveList[i])} currmovenumber {i + 1}");
        //#endif

        //                m_BoardPosition.MakeMove(moveList[i], false);

        //                var score = AlphaBeta(-beta, -alpha, depth - 1);

        //                if (score > bestScore)
        //                {
        //                    bestMove = moveList[i];
        //                    bestScore = score;
        //                }

        //#if Debug
        //                m_Log.Info($"Move: {UCIMoveTranslator.ToUCIMove(moveList[i])}  - Score: {score}");
        //#endif

        //                m_BoardPosition.UnMakeLastMove();
        //            }

        //            return bestMove;
        //

        private decimal AlphaBeta(decimal alpha, decimal beta, int depthLeft)
        {
            var bestScore = decimal.MinValue / 2 + 1;

            if (depthLeft == 0)
            {
                //TODO: Perform quiescience search

                return Evaluate(m_BoardPosition);
            }

            // Check transposition table
            var hash = TranspositionTable.ProbeTable(m_BoardPosition.Zobrist, depthLeft, alpha, beta);

            if (hash.Key != 0 && hash.Depth == depthLeft)
            {
                var povScore = hash.Score;

                if (!m_BoardPosition.WhiteToMove)
                {
                    povScore = -hash.Score;
                }

                switch (hash.NodeType)
                {
                    case HashNodeType.Exact:
                        return povScore;
                    case HashNodeType.LowerBound:
                        alpha = Math.Max(alpha, povScore);
                        break;
                    case HashNodeType.UpperBound:
                        beta = Math.Min(beta, povScore);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (alpha >= beta)
                {
                    return povScore;
                }
            }

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));

            foreach (var move in moveList)
            {
                m_BoardPosition.MakeMove(move, false);

                var score = -AlphaBeta(-beta, -alpha, depthLeft - 1);

                if (score >= beta)
                {
                    m_BoardPosition.UnMakeLastMove();

                    return score;
                }

                if (score > bestScore)
                {
                    bestScore = score;

                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }

                m_BoardPosition.UnMakeLastMove();
            }

            // transposition table store
            HashNodeType hashNodeType;

            if (bestScore <= alpha)
            {
                hashNodeType = HashNodeType.UpperBound;
            }
            //else if (bestScore >= beta)
            //{
            //    hashNodeType = HashNodeType.LowerBound;
            //}
            else
            {
                hashNodeType = HashNodeType.Exact;
            }

            if (m_BoardPosition.WhiteToMove)
            {
                RecordHash(depthLeft, bestScore, hashNodeType);
            }
            else
            {
                RecordHash(depthLeft, -bestScore, hashNodeType);
            }

            //OrderMovesInPlaceByEvaluation(moveList);
            OrderMovesInPlace(moveList);

            return bestScore;
        }

        private void RecordHash(int depth, decimal score, HashNodeType hashNodeType)
        {
            var hash = new Hash
            {
                Key = m_BoardPosition.Zobrist,
                Depth = depth,
                NodeType = hashNodeType,
                Score = score,
                //BestMove = bestMove
            };


            TranspositionTable.Add(hash);
        }

        /// <summary>
        /// Evaluates the score relative to the current player
        /// i.e. A high score means the position is better for the current player 
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <returns></returns>
        private decimal Evaluate(IBoard boardPosition)
        {
            if (m_BoardPosition.WhiteToMove)
            {
                return -m_ScoreCalculator.CalculateScore(boardPosition);
            }
            else
            {
                return m_ScoreCalculator.CalculateScore(boardPosition);
            }
        }
        
        private void OrderMovesInPlaceByEvaluation(IList<PieceMoves> moveList)
        {
            var scores = new List<Tuple<int, decimal>>();

            for (var i = 0; i < moveList.Count; i++)
            {
                m_BoardPosition.MakeMove(moveList[i], false);

                scores.Add(new Tuple<int, decimal>(i, Evaluate(m_BoardPosition)));

                m_BoardPosition.UnMakeLastMove();
            }

            scores = scores.OrderBy(s => s.Item2).ToList();

            for (var i = 0; i < scores.Count; i++)
            {
                var toMove = moveList[scores[i].Item1];

                moveList.RemoveAt(scores[i].Item1);
                moveList.Insert(0, toMove);
            }
        }

        // Order all moves by MVV/LVA
            private void OrderMovesInPlace(IList<PieceMoves> moveList)
        {
            // move list position, victim score, attacker score
            var ordering = new List<Tuple<int, int, int>>();

            //Move capture
            for (var moveNum = 0; moveNum < moveList.Count; moveNum++)
            {
                if (   moveList[moveNum].SpecialMove == SpecialMoveType.Capture 
                    || moveList[moveNum].SpecialMove == SpecialMoveType.ENPassantCapture 
                    || IsPromotionCapture(moveList[moveNum].SpecialMove))
                {
                    var victimType = BoardChecking.GetPieceTypeOnSquare(m_BoardPosition, moveList[moveNum].Moves);

                    ordering.Add(new Tuple<int, int, int>(moveNum, 
                                                          GetPieceScore(victimType), 
                                                          GetPieceScore(moveList[moveNum].Type)));
                }
            }

            //Order by victim and then attacker. We do it in reverse
            ordering = ordering.OrderBy(o => o.Item3).ThenBy(o => o.Item2).ToList();

            for (var orderPosition = 0; orderPosition < ordering.Count; orderPosition++)
            {
                var toMove = moveList[ordering[orderPosition].Item1];

                moveList.RemoveAt(ordering[orderPosition].Item1);
                moveList.Insert(orderPosition, toMove);
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

        private void LogTranspositionCounts()
        {
            m_Log.Info("----------------------------------------------------------------------------------");
            m_Log.Info("TRANSPOSITION DEBUGGING");
            m_Log.Info($"Times hash added:              {CountDebugger.Transposition_HashAdded}");
            m_Log.Info($"Times hash replaced:           {CountDebugger.Transposition_HashReplaced}");
            m_Log.Info($"Times probed:                  {CountDebugger.Transposition_Searches}");
            m_Log.Info($"Times hash returned:           {CountDebugger.Transposition_HashFound}");
            m_Log.Info($"Times keys matched:            {CountDebugger.Transposition_MatchCount}");
            m_Log.Info($"Times keys and depths matched: {CountDebugger.Transposition_MatchAndUsed}");
            m_Log.Info($"Times not matched:             {CountDebugger.Transposition_Searches - CountDebugger.Transposition_MatchAndUsed}");
            m_Log.Info("----------------------------------------------------------------------------------");
        }
    }
}