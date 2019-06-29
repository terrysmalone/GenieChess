using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private readonly Board m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
 
        private List<MoveValueInfo> m_InitialMoves;
        private List<Tuple<int, PieceMoves>> m_InitialMovesIterativeDeepeningShuffleOrder;

        // Static exchange Evaluation values
        const int m_PawnScore   = 1;
        const int m_KnightScore = 3;
        const int m_BishopScore = 3;
        const int m_RookScore   = 5;
        const int m_QueenScore  = 9;
        const int m_KingScore   = 10; // We want the king to attack last so that it can't be taken

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

            foreach (var move in moveList)
            {
#if UCI
                Console.WriteLine($"info currmove {UciMoveTranslator.ToUciMove(moveList[i])} currmovenumber {i + 1}");
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
            
            OrderMovesByStaticExchangeEvaluation(moves);
            
            //OrderMovesByMvvVla(moves);

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

            OrderMovesByStaticExchangeEvaluation(moveList);

            BringKillerMovesToTheFront(moveList, depth);

            if (bestHashMove != null)
            {
                BringBestHashMoveToTheFront(moveList, (PieceMoves)bestHashMove);
            }
        }

        private void OrderMovesByStaticExchangeEvaluation(IList<PieceMoves> moveList)
        {
            // swap score, move
            var ordering = new List<Tuple<int, PieceMoves>>();

            for (var i = 0; i < moveList.Count; i++)
            {
                var move = moveList[i];

                if (move.SpecialMove == SpecialMoveType.Capture 
                    || move.SpecialMove == SpecialMoveType.ENPassantCapture 
                    || IsPromotionCapture(move.SpecialMove))
                {
                    var friendlyPieces = GetAttackingPieceScores(move.Moves, m_BoardPosition.WhiteToMove);
                    var enemyPieces = GetAttackingPieceScores(move.Moves, !m_BoardPosition.WhiteToMove);

                    var swapScore = CalculateSwapScore(friendlyPieces, enemyPieces, move);

                    ordering.Add(new Tuple<int, PieceMoves>(swapScore, move));
                }
            }

            ordering.OrderBy(o => o.Item1);

            foreach (var order in ordering)
            {
                if (order.Item1 >= 0)
                {
                    if (moveList.Remove(order.Item2))
                    {

                        moveList.Insert(0, order.Item2);
                    }
                    else
                    {
                        throw new ChessBoardException("Tried to remove a non-existent move in SEE ordering");
                    }
                }
                else
                {
                    // Move them to the back. This moves them out of order but we don't really care for now
                    if (moveList.Remove(order.Item2))
                    {
                        moveList.Insert(moveList.Count, order.Item2);
                    }
                    else
                    {
                        throw new ChessBoardException("Tried to remove a non-existent move in SEE ordering");
                    }
                }
            }


            // for each move
            // If it's a capture
            // do SEE

            // Order moves
        }

        private int StaticExchangeEvaluation(ulong attackPositionBoard, bool whiteToPlay)
        {
            var enemyPieces = GetAttackingPieceScores(attackPositionBoard, whiteToPlay);
            var friendlyPieces = GetAttackingPieceScores(attackPositionBoard, !whiteToPlay);

            // Swap off from least to most valuable
            // Beware king captures
            // Beware checks

            return 0;
        }

        private List<int> GetAttackingPieceScores(ulong attackPositionBoard, bool whiteToMove)
        {
            const int pawnScore   = 1;
            const int knightScore = 3;
            const int bishopScore = 3;
            const int rookScore   = 5;
            const int queenScore  = 9;
            const int kingScore   = 10; // We want the king to attack last so that it can't be taken

            var attackingPieceScores = new List<int>();

            // Get all pieces attacking attackPositionBoard 
            // Pawn attacks
            var enemyPawnAttacks = BoardChecking.PawnCountAttackingSquare(m_BoardPosition,
                                                                          attackPositionBoard,
                                                                          whiteToMove);

            for (var i = 0; i < enemyPawnAttacks; i++)
            {
                attackingPieceScores.Add(pawnScore);
            }

            // Knight attacks
            var knightAttacks = BoardChecking.KnightCountAttackingSquare(m_BoardPosition,
                                                                         attackPositionBoard,
                                                                         m_BoardPosition.WhiteToMove);

            for (var i = 0; i < knightAttacks; i++)
            {
                attackingPieceScores.Add(knightScore);
            }

            ulong friendlyBishops;
            ulong friendlyRooks;
            ulong friendlyQueen;


            if (whiteToMove)
            {
                friendlyBishops = m_BoardPosition.WhiteBishops;
                friendlyRooks = m_BoardPosition.WhiteRooks;
                friendlyQueen = m_BoardPosition.WhiteQueen;
            }
            else
            {
                friendlyBishops = m_BoardPosition.BlackBishops;
                friendlyRooks = m_BoardPosition.BlackRooks;
                friendlyQueen = m_BoardPosition.BlackQueen;
            }

            // Bishop attacks
            var diagonalBoard = BoardChecking.CalculateAllowedBishopMoves(m_BoardPosition, 
                                                                          attackPositionBoard, 
                                                                          whiteToMove,
                                                                          removeFriendlyBlocker: false);

            var attackingBishops = diagonalBoard & friendlyBishops;

            if (attackingBishops != 0)
            {
                 var bishopCount = BitboardOperations.GetPopCount(attackingBishops);

                 for (var i = 0; i < bishopCount; i++)
                 {
                    attackingPieceScores.Add(bishopScore);
                 }
            }

            // Rook attacks
            var straightBoard = BoardChecking.CalculateAllowedRookMoves(m_BoardPosition, 
                                                                        attackPositionBoard, 
                                                                        whiteToMove,
                                                                        removeFriendlyBlocker: false);

            var attackingRooks = straightBoard & friendlyRooks;

            if (attackingRooks != 0)
            {
                var rookCount = BitboardOperations.GetPopCount(attackingRooks);

                for (var i = 0; i < rookCount; i++)
                {
                    attackingPieceScores.Add(rookScore);
                }
            }

            // Queen attacks
            var attackingQueens = (straightBoard | diagonalBoard) & friendlyQueen;

            if (attackingQueens != 0)
            {
                var queenCount = BitboardOperations.GetPopCount(attackingQueens);

                for (var i = 0; i < queenCount; i++)
                {
                    attackingPieceScores.Add(queenScore);
                }
            }

            // King attacks
            if (BoardChecking.IsSquareAttackedByKing(m_BoardPosition, attackPositionBoard, !whiteToMove))
            {
                attackingPieceScores.Add(kingScore);
            }

            // Enemy x-ray attacks

            attackingPieceScores.Sort();

            return attackingPieceScores;
        }

        private int CalculateSwapScore(IList<int> friendlyPieces, IList<int> enemyPieces, PieceMoves move)
        {
            var pieceScore = 
                move.SpecialMove == SpecialMoveType.ENPassantCapture ? 1 
                                                                     : GetValueOfPieceOnBoard(move.Moves);

            var swapValue = 0;
            
            swapValue += pieceScore;
            
            //Then work up, swapping sides
            var friendlyPiece = true;

            var friendlyPiecesLeft = true;
            var enemyPiecesLeft = true;
            var piecesLeft = true;

            while (friendlyPieces.Count > 0 && enemyPieces.Count > 0)
            {
                if(friendlyPiece)
                {
                    if (friendlyPieces.Count > 0)
                    {
                        var lowestLeft = friendlyPieces.First();

                        if (lowestLeft == m_KingScore)
                        {
                            // Only do a king capture if nothing else can attack it
                            if(enemyPieces.Count == 0)
                            {
                                swapValue -= m_KingScore;

                            }

                            friendlyPiecesLeft = false;
                            enemyPiecesLeft    = false;
                            piecesLeft         = false;
                        }
                        else
                        {
                            swapValue -= lowestLeft;
                        }

                        friendlyPieces.Remove(lowestLeft);
                    }
                    else
                    {
                        friendlyPiecesLeft = false;
                    }
                }
                else
                {
                    if (enemyPieces.Count > 0)
                    {
                        var lowestLeft = enemyPieces.First();

                        if (lowestLeft == m_KingScore)
                        {
                            // Only do a king capture if nothing else can attack it
                            if (friendlyPieces.Count == 0)
                            {
                                swapValue += m_KingScore;
                            }

                            friendlyPiecesLeft = false;
                            enemyPiecesLeft    = false;
                            piecesLeft         = false;
                        }
                        else
                        {
                            swapValue += lowestLeft;
                        }

                        enemyPieces.Remove(lowestLeft);
                    }
                    else
                    {
                        enemyPiecesLeft = false;
                    }
                }

                if (!friendlyPiecesLeft && !enemyPiecesLeft)
                {
                    piecesLeft = false;
                }

                friendlyPiece = !friendlyPiece;
            }

            return swapValue;

        }

        private int GetValueOfPieceOnBoard(ulong squareToCheck)
        {
            if ((m_BoardPosition.WhitePawns & squareToCheck) > 0
                ||(m_BoardPosition.BlackPawns & squareToCheck) > 0)
            {
                return m_PawnScore;
            }

            if ((m_BoardPosition.WhiteKnights & squareToCheck) > 0
                || (m_BoardPosition.BlackKnights & squareToCheck) > 0)
            {
                return m_KnightScore;
            }

            if ((m_BoardPosition.WhiteBishops & squareToCheck) > 0
                || (m_BoardPosition.BlackBishops & squareToCheck) > 0)
            {
                return m_BishopScore;
            }

            if ((m_BoardPosition.WhiteRooks & squareToCheck) > 0
                || (m_BoardPosition.BlackRooks & squareToCheck) > 0)
            {
                return m_RookScore;
            }

            if ((m_BoardPosition.WhiteQueen & squareToCheck) > 0
                || (m_BoardPosition.BlackQueen & squareToCheck) > 0)
            {
                return m_QueenScore;
            }

            if ((m_BoardPosition.WhiteKing & squareToCheck) > 0
                || (m_BoardPosition.BlackKing & squareToCheck) > 0)
            {
                return m_KingScore;
            }

            throw new ChessBoardException($"Board {squareToCheck} should have 1 piece on it");
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