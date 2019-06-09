using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private readonly IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
 
        private List<MoveValueInfo> m_InitialMoves;
        private List<Tuple<decimal, PieceMoves>> m_InitialMovesIterativeDeepeningShuffleOrder;

        private int m_KillerMovesToStore = 2;
        private PieceMoves[,] m_KillerMoves;

        private int m_EvaluationDepth;

        public AlphaBetaSearch(IBoard boardPosition, IScoreCalculator scoreCalculator)
        {
            m_BoardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        public PieceMoves CalculateBestMove(int maxDepth)
        {
            var toMove = m_BoardPosition.WhiteToMove ? "white" : "black";
            s_Log.Info($"Calculating move for {toMove}");
            s_Log.Info(FenTranslator.ToFENString(m_BoardPosition.GetCurrentBoardState()));

            m_KillerMoves = new PieceMoves[maxDepth, m_KillerMovesToStore]; // Try to a depth of maxDepth with 5 saved each round

            m_InitialMoves = new List<MoveValueInfo>();
            m_InitialMovesIterativeDeepeningShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            TranspositionTable.ClearAncients();

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
                LogKillerMoves(m_KillerMoves);
#endif

                s_Log.Info($"Move info: {UciMoveTranslator.ToUciMove(moveValueInfo.Move)} - " +
                           $"score: {moveValueInfo.Score} - " +
                           $"nodes: {moveValueInfo.NodesVisited} - " +
                           $"time at depth: {moveValueInfo.DepthTime:mm\':\'ss\':\'ffff} - " +
                           $"Accumulated move time: {moveValueInfo.AccumulatedTime:mm\':\'ss\':\'ffff}");



//#if UCI
//                var bestMove = UciMoveTranslator.ToUciMove();
//                //Console.WriteLine(string.Format("Best move at depth {0}: {1}", i, bestMove));
//                //Console.WriteLine(String.Format("info currmove {0} depth {1} nodes {2} ", bestMove, i, pvInfo.NodesVisited));
//                //Console.WriteLine(String.Format("info score cp 0 {0} depth {1} nodes {2} time {3} ", bestMove, i, pvInfo.NodesVisited, pvInfo.DepthTime));
//                Console.WriteLine($"info score cp {pvInfo.Score} depth {i} nodes {pvInfo.NodesVisited} pv {bestMove} ");

                // //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UciMoveTranslator.ToUciMove(bestIDMove)));
                //#endif
            }

            s_Log.Info($"Found move: {UciMoveTranslator.ToUciMove(bestMove)}");

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
                OrderMovesInPlace(moveList, depth);
            }

#if Debug
            s_Log.Info("---------------------------------------------------------");
            s_Log.Info($"Moves to Check: {moveList.Count}");
#endif

            foreach (var move in moveList)
            {
#if UCI
                Console.WriteLine($"info currmove {UciMoveTranslator.ToUciMove(moveList[i])} currmovenumber {i + 1}");
#endif

                m_BoardPosition.MakeMove(move, false);

                // Since we're swapping colours at the next depth invert alpha and beta
                var score = -AlphaBeta(-beta, -alpha, depth - 1);

                if (score > bestScore)
                {
                    bestMove = move;
                    bestScore = score;
                }

#if Debug
                s_Log.Info($"Move: {UciMoveTranslator.ToUciMove(move)} - Score: {score}");
#endif

                m_InitialMovesIterativeDeepeningShuffleOrder.Add(new Tuple<decimal, PieceMoves>(score, move));

                m_BoardPosition.UnMakeLastMove();
            }

            return bestMove;
        }

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

            if (hash.Key !=0)
            {
                var transpositionScore = hash.Score;
                
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

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));

            if (moveList.Count == 0)
            {
                return EvaluateEndGame(depthLeft);
            }

            OrderMovesInPlace(moveList, depthLeft);

            foreach (var move in moveList)
            {
                m_BoardPosition.MakeMove(move, false);

                var score = -AlphaBeta(-beta, -alpha, depthLeft - 1);

                m_BoardPosition.UnMakeLastMove();

                if (score >= beta)
                {
                    RecordHash(depthLeft, score, HashNodeType.LowerBound);

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

                if (score > bestScore)
                {
                    bestScore = score;

                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }

            // transposition table store
            HashNodeType hashNodeType;

            hashNodeType = bestScore <= alpha ? HashNodeType.UpperBound 
                                              : HashNodeType.Exact;

            RecordHash(depthLeft, bestScore, hashNodeType);
           
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

        /// Evaluates the score relative to the current player
        /// i.e. A high score means the position is better for the current player
        private decimal Evaluate(IBoard boardPosition)
        {
            if (m_BoardPosition.WhiteToMove)
            {
                return m_ScoreCalculator.CalculateScore(boardPosition);
            }
            else
            {
                return -m_ScoreCalculator.CalculateScore(boardPosition);
            }
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

        private void OrderMovesByMvvVla(IList<PieceMoves> moveList)
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

                    ordering.Add(new Tuple<int, int, int>(
                        moveNum,
                        GetPieceScore(victimType),
                        GetPieceScore(moveList[moveNum].Type)));
                }
            }

            //Order by victim and then attacker. We do it in reverse
            ordering = ordering.OrderBy(o => o.Item3).ThenBy(o => o.Item2).ToList();
            //ordering = ordering.OrderBy(o => o.Item2).ThenByDescending(o => o.Item3).ToList();

            for (var orderPosition = 0; orderPosition < ordering.Count; orderPosition++)
            {
                var toMove = moveList[ordering[orderPosition].Item1];

                moveList.RemoveAt(ordering[orderPosition].Item1);
                moveList.Insert(orderPosition, toMove);
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
        private decimal EvaluateEndGame(int depth)
        {
            var movesToend = (m_EvaluationDepth - depth) + 1;  //Since we want lower depth mates to score lower

            bool isInCheck;

            isInCheck = BoardChecking.IsKingInCheckFast(m_BoardPosition, m_BoardPosition.WhiteToMove ? PieceColour.White 
                                                                                                     : PieceColour.Black);

            if (isInCheck)
            {
                return decimal.MinValue / 2 + (100000 * movesToend); // Player is in checkmate
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

        public List<MoveValueInfo> GetMoveValueInfo()
        {
            return m_InitialMoves;
        }
    }
}