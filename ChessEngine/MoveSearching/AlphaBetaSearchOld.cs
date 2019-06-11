using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using System.Threading;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;

namespace ChessEngine.MoveSearching
{
    /// <summary>
    /// Similar http://www.chessbin.com/post/Move-Searching-and-Alpha-Beta
    /// 
    /// https://en.wikipedia.org/wiki/Negamax#Negamax_with_alpha_beta_pruning
    /// http://cis-linux1.temple.edu/~giorgio/cis587/readings/alpha-beta.html
    /// </summary>
    public sealed class AlphaBetaSearchOld
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalc;

        private int startDepth; 

        private Tuple<PieceMoves, decimal>[] primaryKillerMoves;
        private Tuple<PieceMoves, decimal>[] secondaryKillerMoves; 
        
        #region iterative deepening variables

        private bool useIterativeDeepening;

        private PieceMoves bestIDMove;
        private decimal bestIDScore;
        private List<MoveValueInfo> idMoves;

        private List<Tuple<decimal, PieceMoves>> idShuffleOrder;
        
        #endregion iterative deepening variables
               
        private int nullMoveR = 3;

        #region properties

        int EXTENSION_LIMIT = 10;

        public List<MoveValueInfo> IdMoves
        {
            get { return idMoves; }
        }

        #endregion properties

        public AlphaBetaSearchOld(IBoard boardPosition, IScoreCalculator scoreCalc)
        {
            m_BoardPosition = boardPosition;
            m_ScoreCalc = scoreCalc;
        }

        /// <summary>
        /// Begins an iterative deepening search
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public PieceMoves StartSearch(int maxDepth)
        {
            var toMove = "black";
            if (m_BoardPosition.WhiteToMove)
                toMove = "white";

            log.Info($"Calculating move for {toMove}");
            
            //Reset iterative deepening variables
            useIterativeDeepening = true;

            idMoves = new List<MoveValueInfo>();
            bestIDMove = new PieceMoves { Moves = 0, Position = 0, SpecialMove = SpecialMoveType.Normal, Type = PieceType.None };
            bestIDScore = 0;

            //Reset killer moves
             primaryKillerMoves = new Tuple<PieceMoves, decimal>[10];
             secondaryKillerMoves = new Tuple<PieceMoves, decimal>[10];
            
            idShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            for (var i = 1; i <= maxDepth; i++)
            {

#if UCI
                    Console.WriteLine($"info depth {i}");               
#endif

#if Debug
                CountDebugger.Evaluations = 0;
#endif

                var timer = new Stopwatch();

                timer.Start();

                bestIDMove = MoveCalculate(i, out bestIDScore);

                timer.Stop();

                var speed = new TimeSpan(timer.Elapsed.Ticks);

                var pvInfo = new MoveValueInfo
                {
                    Move = bestIDMove,

                    Score = bestIDScore,

                    DepthTime = speed,

                    AccumulatedTime = idMoves.Count > 0
                        ? idMoves[idMoves.Count - 1].AccumulatedTime.Add(speed)
                        : speed,

                    NodesVisited = CountDebugger.Evaluations
                };
#if Debug
#endif
                idMoves.Add(pvInfo);

                log.Info($"Depth {i}: {UciMoveTranslator.ToUciMove(pvInfo.Move)} - " +
                         $"score:{pvInfo.Score} - " +
                         $"nodes:{pvInfo.NodesVisited} - " +
                         $"time at depth:{pvInfo.DepthTime:ss'.'fff}s - " +
                         $"time for move:{pvInfo.AccumulatedTime:ss'.'fff}s");

                 //LogPrincipalVariation(i);               

#if UCI
                var bestMove = UciMoveTranslator.ToUciMove(bestIDMove);
                //Console.WriteLine(string.Format("Best move at depth {0}: {1}", i, bestMove));
                //Console.WriteLine(String.Format("info currmove {0} depth {1} nodes {2} ", bestMove, i, pvInfo.NodesVisited));
                //Console.WriteLine(String.Format("info score cp 0 {0} depth {1} nodes {2} time {3} ", bestMove, i, pvInfo.NodesVisited, pvInfo.DepthTime));
                Console.WriteLine($"info score cp {pvInfo.Score} depth {i} nodes {pvInfo.NodesVisited} pv {bestMove} ");
                
                //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UciMoveTranslator.ToUciMove(bestIDMove)));
#endif
            }

            log.Info($"Found move {UciMoveTranslator.ToUciMove(bestIDMove)}");

            LogKillerMoves();
            return bestIDMove;
        }

        /// <summary>
        /// 1000 = 1 s
        /// </summary>
        /// <param name="timeInMilliseconds"></param>
        /// <returns></returns>
        public PieceMoves StartTimedSearch(int timeInMilliseconds)
        {            
            var toMove = "black";
            if (m_BoardPosition.WhiteToMove)
                toMove = "white";

            log.Info($"Calculating move for {toMove}");

            
            //Reset iterative deepening variables
            useIterativeDeepening = true;

            idMoves = new List<MoveValueInfo>();
            bestIDMove = new PieceMoves() { Moves = 0, Position = 0, SpecialMove = SpecialMoveType.Normal, Type = PieceType.None };
            bestIDScore = 0;

            idShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            var moveTimer = new Stopwatch();
            moveTimer.Start();

            var timedMove = new Thread(new ThreadStart(MakeTimedMove));
            timedMove.Start();
            
            while(moveTimer.Elapsed.Ticks <= timeInMilliseconds)
            {
                
            }

            

            return bestIDMove;
        }

        private void MakeTimedMove()
        {
            var maxDepth = 30;

            for (var i = 1; i <= maxDepth; i++)
            {
#if Debug
                CountDebugger.Evaluations = 0;
#endif
                var timer = new Stopwatch();
                timer.Start();

                bestIDMove = MoveCalculate(i, out bestIDScore);

                timer.Stop();
                var speed = new TimeSpan(timer.Elapsed.Ticks);

                var pvInfo = new MoveValueInfo();
                pvInfo.Move = bestIDMove;
                pvInfo.Score = bestIDScore;
                pvInfo.DepthTime = speed;

                if (idMoves.Count > 0)
                    pvInfo.AccumulatedTime = idMoves[idMoves.Count - 1].AccumulatedTime.Add(speed);
                else
                    pvInfo.AccumulatedTime = speed;

#if Debug
                pvInfo.NodesVisited = CountDebugger.Evaluations;
#endif
                idMoves.Add(pvInfo);

                log.Info(
                    $"{i}: {UciMoveTranslator.ToUciMove(pvInfo.Move)} - score:{pvInfo.Score} - nodes:{pvInfo.NodesVisited} - time at depth:{pvInfo.DepthTime.ToString("ss'.'fff")}s - time for move:{pvInfo.AccumulatedTime.ToString("ss'.'fff")}s");
                //LogPrincipalVariation(i);               

#if UCI
                Console.WriteLine($"Best move at depth {i}: {UciMoveTranslator.ToUciMove(bestIDMove)}");
                //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UciMoveTranslator.ToUciMove(bestIDMove))); 
#endif
            }

            log.Info($"Found move {UciMoveTranslator.ToUciMove(bestIDMove)}");
        }

        public PieceMoves MoveCalculate(int depth)
        {
            useIterativeDeepening = false;

            decimal empty;
            return MoveCalculate(depth, out empty);
        }

        public PieceMoves MoveCalculate(int depth, out decimal bestScore)
        {
            startDepth = depth;

            var alpha = decimal.MinValue/2-1;
            var beta = decimal.MaxValue/2+1;

            Debug.Assert(depth >= 1);

            decimal score = 0;

            var bestMove = new PieceMoves();
            
            List<PieceMoves> moveList;

            if (useIterativeDeepening && idShuffleOrder.Count > 0)
                moveList = OrderFromIDMoves();
            else
            {
                moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));
                OrderMoveInPlace(moveList);
            }

            //Aspiration search
            //decimal window = 0.2m;
            //decimal previousScore = 0;
            //decimal wAlpha = 0;
            //decimal wBeta = 0;

            //if (depth > 2)
            //{
            //    previousScore = idMoves[(idMoves.Count - 2)].Score;
            //    wAlpha = previousScore - window;
            //    wBeta = previousScore + window;
            //}

            for (var i = 0; i < moveList.Count; i++)
            {
#if UCI
                Console.WriteLine($"info currmove {UciMoveTranslator.ToUciMove(moveList[i])} currmovenumber {i + 1}");
#endif

                m_BoardPosition.MakeMove(moveList[i], false);
                
                score = -AlphaBeta(-beta, -alpha, depth - 1, true);
                
                if (score > alpha)
                {
                    alpha = score;

                    bestMove = moveList[i];
                }

                if (useIterativeDeepening)
                {
                    idShuffleOrder.Add(new Tuple<decimal, PieceMoves>(score, moveList[i]));
                }

                //RecordHash(depth, score, HashNodeType.Exact);

                m_BoardPosition.UnMakeLastMove();
            }

            bestScore = alpha;
            return bestMove;
        }


        private decimal AlphaBeta(decimal alpha, decimal beta, int depth, bool allowNullMove)
        {
            return AlphaBeta(alpha, beta, depth, allowNullMove, isNullMoveSearch: false, extensionDepth: 0);
        }

        private decimal AlphaBeta(decimal alpha, decimal beta, int depth, bool allowNullMove, bool isNullMoveSearch, int extensionDepth)
        {
            var searchDepth = startDepth - depth - 1;

            var alphaOrig = alpha;

            var score = decimal.MinValue / 2 - 1;

           #region transposition table lookup

           var bestMoveSoFar = new PieceMoves();

           var hash = TranspositionTable.ProbeTable(m_BoardPosition.Zobrist, depth, alpha, beta);

           if (hash.Key != 0)
           {
               if (hash.Depth >= depth)
               {
                   var povScore = hash.Score;

                   if (!m_BoardPosition.WhiteToMove)
                   {
                       povScore = -hash.Score;
                   }

                   if (hash.NodeType == HashNodeType.Exact)
                   {
                       return povScore;
                   }
                   else if (hash.NodeType == HashNodeType.LowerBound)
                   {
                       alpha = Math.Max(alpha, povScore);
                   }
                   else if (hash.NodeType == HashNodeType.UpperBound)
                   {
                       beta = Math.Min(beta, povScore);
                   }

                   if (alpha >= beta)
                   {
                       return povScore;
                   }

                   //return alpha;
               }

               if (hash.BestMove.Type != PieceType.None)
               {
                   bestMoveSoFar = hash.BestMove; //move this to the front
               }
           }
           
           #endregion transposition table lookup
                                    
            var isPositionInCheck = false;
            var canKingMove = false;
            GetKingState(out isPositionInCheck, out canKingMove);
            
            #region Check extensions

            var extensions = 0;

            if (depth == 0)
            {
                if (isPositionInCheck && extensionDepth < EXTENSION_LIMIT)
                {
                    extensions = DetermineExtensions(depth);

                    extensionDepth++;
                }
                else
                {
                    score = Evaluate(m_BoardPosition);

                    if (m_BoardPosition.WhiteToMove)
                        RecordHash(depth, score, HashNodeType.Exact);
                    else
                        RecordHash(depth, -score, HashNodeType.Exact);


                    return score;
                }
            }

            #endregion Check extensions
                        
            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));

            if (moveList.Count == 0)
                return EvaluateEndGame(depth, isPositionInCheck);

            #region null move pruning

#warning - Now that I do pseudo legal move generation this may be called if the player is in stalemate. I should sort this
#warning - This should go before calculating moves but until I move the check for endgame before this it can't
            if (!isPositionInCheck && allowNullMove && depth > nullMoveR)
            {
                m_BoardPosition.SwitchSides();
                var eval = -AlphaBeta(-beta, -beta + 1, depth - 1 - nullMoveR, false, true, extensionDepth);

                //if (eval >= beta)
                //{
                //    RecordHash(depth - 1 - nullMoveR, score, HashNodeType.LowerBound);
                //}

                m_BoardPosition.SwitchSides();

                if (eval >= beta)
                {
                    CountDebugger.NullMovesPruned++;
                    
                    return eval; // Cutoff
                }
            }

            #endregion null move pruning

            #region move ordering

            //Put killer moves to the front
            if (searchDepth > 0)
            {
                //if (secondaryKillerMoves[searchDepth] != null)
                //{
                //    PieceMoves killerMove = secondaryKillerMoves[searchDepth].Item1;

                //    if (moveList.Contains(killerMove))
                //    {
                //        MoveToFront(moveList, killerMove);
                //    }
                //}

                if (primaryKillerMoves[searchDepth] != null)
                {
                    var killerMove = primaryKillerMoves[searchDepth].Item1;

                    if (moveList.Contains(killerMove))
                    {
                        MoveToFront(moveList, killerMove);
                    }
                }
            }

            OrderMoveInPlace(moveList);

            //Put best move from transposition table in front
            if (bestMoveSoFar.Type != PieceType.None && moveList.Contains(bestMoveSoFar))
            {
                MoveToFront(moveList, bestMoveSoFar);
            }
            
            #endregion move ordering
            
            var calculationDepth = depth + extensions;
            var areAnyMovesLegal = false;

            var bestMove = new PieceMoves();

            for (var i = 0; i < moveList.Count; i++)
            {
                var currentMove = moveList[i];

                var skipCastlingMove = false;
                if (IsCastlingMove(currentMove))
                {
                    if (isPositionInCheck || !MoveGeneration.ValidateCastlingMove(m_BoardPosition, currentMove))                   
                    {
                        moveList.RemoveAt(i);
                        i--;
                        skipCastlingMove = true;
                    }
                }

                if (!skipCastlingMove)
                {
                    m_BoardPosition.MakeMove(currentMove, false);

                    if (MoveGeneration.ValidateMove(m_BoardPosition))
                    {
                        areAnyMovesLegal = true;

                        var val = -AlphaBeta(-beta, -alpha, calculationDepth - 1, true, isNullMoveSearch, extensionDepth);    //Flip and negate bounds

                        if (val > score)
                        {
                            bestMove = currentMove;
                        }

                        score = Math.Max(score, val);
                        alpha = Math.Max(alpha, val);

                        if (alpha >= beta)
                        {

                            //Save killer move
                            if (!IsCapture(currentMove.SpecialMove))
                            {
                                if (primaryKillerMoves[searchDepth] != null)
                                {
                                    var primary = primaryKillerMoves[searchDepth];

                                    if (alpha > primary.Item2)
                                    {
                                        primaryKillerMoves[searchDepth] = new Tuple<PieceMoves, decimal>(currentMove, alpha);   //Just update the score
                                        //if (primary.Item1 != currentMove)
                                        //{
                                        //    secondaryKillerMoves[searchDepth] = primaryKillerMoves[searchDepth];
                                        //    primaryKillerMoves[searchDepth] = new Tuple<PieceMoves, decimal>(currentMove, alpha);


                                        //}
                                        //else
                                        //    primaryKillerMoves[searchDepth] = new Tuple<PieceMoves, decimal>(currentMove, alpha);   //Just update the score
                                    }
                                }
                                else
                                {
                                    primaryKillerMoves[searchDepth] = new Tuple<PieceMoves, decimal>(currentMove, alpha);
                                }
                            }

                            m_BoardPosition.UnMakeLastMove();
                            break;
                        }
                    }
                    //else
                    //{
                        //moveList.RemoveAt(i);
                        //i--;
                    //}

                    m_BoardPosition.UnMakeLastMove();
                }
            }

            if (areAnyMovesLegal == false)
            {
                return EvaluateEndGame(depth, isPositionInCheck);
            }

            // transposition table store
            HashNodeType hashNodeType;

            if (score <= alphaOrig)
                hashNodeType = HashNodeType.UpperBound;
            else if (score >= beta)
                hashNodeType = HashNodeType.LowerBound;
            else
                hashNodeType = HashNodeType.Exact;

            if (m_BoardPosition.WhiteToMove)
                RecordHash(calculationDepth, score, hashNodeType, bestMove);
            else
                RecordHash(calculationDepth, -score, hashNodeType, bestMove);
            
                       
            return score;
        }

        #region move ordering

        private void OrderMoveInPlace(List<PieceMoves> moveList)
        {
            //OrderSimple(moveList);   
            
            //OrderByScore(moveList);

            OrderByMVVLVA(moveList);
         
        }

        private void OrderSimple(List<PieceMoves> moveList)
        {
            //Move all captures
            for (var i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                    var moveToMove = moveList[i];
                    moveList.RemoveAt(i);
                    moveList.Insert(0, moveToMove);
                }
            }

            //Move all promotions 
            for (var i = 0; i < moveList.Count; i++)
            {
                if (IsPromotion(moveList[i].SpecialMove))
                {
                    var moveToMove = moveList[i];
                    moveList.RemoveAt(i);
                    moveList.Insert(0, moveToMove);
                }
            }
        }

        private void OrderByMVVLVA(List<PieceMoves> moveList)
        {
            //position, victim attacker
            var ordering = new List<Tuple<int, int, int>>();

            //Move capture
            for (var i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                    var victimType = BoardChecking.GetPieceTypeOnSquare(m_BoardPosition, moveList[i].Moves);
                    ordering.Add(new Tuple<int, int, int>(i, GetPieceScore(victimType), GetPieceScore(moveList[i].Type)));
                }
            }

            //Order by victim and then attacker
            ordering = ordering.OrderByDescending(o => o.Item2).ThenBy(o => o.Item3).ToList();

            for (var i = 0; i < ordering.Count; i++)
            {
                MoveTo(moveList, ordering[i].Item1, i);
            }
        }

        /// <summary>
        /// Orders based on victim-attacker score
        /// </summary>
        /// <param name="moveList"></param>
        private void OrderByScore(List<PieceMoves> moveList)
        {
            //position, victim attacker
            //List<Tuple<int, int, int>> ordering = new List<Tuple<int, int, int>>();

            //position, victim-attacker
            var ordering = new List<Tuple<int, int>>();

            //Move capture
            for (var i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                   var victimType = BoardChecking.GetPieceTypeOnSquare(m_BoardPosition, moveList[i].Moves);
                   ordering.Add(new Tuple<int, int>(i, GetPieceScore(victimType)-GetPieceScore(moveList[i].Type)));
                }
            }

            //Order by victim and then attacker
            ordering = ordering.OrderByDescending(o => o.Item2).ToList();

            var cutoff = 0;

            for (var i = 0; i < ordering.Count; i++)
            {
                if (ordering[i].Item2 >= 0)
                    MoveTo(moveList, ordering[i].Item1, i);
                else
                {
                    cutoff = i;
                    break;
                }
            }

            var end = moveList.Count - 1;

            for (var i = cutoff; i < ordering.Count; i++)
            {
                MoveTo(moveList, ordering[i].Item1, end);
            }
        }

        /// <summary>
        /// Calculates the piece score for move ordering
        /// 1=pawn
        /// 2=knight
        /// 3=bishop
        /// 4=rook
        /// 5=queen
        /// 6=king
        /// </summary>
        /// <param name="victimType"></param>
        /// <returns></returns>
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
            }

            return 0;
        }

        private List<PieceMoves> OrderFromIDMoves()
        {
            var moveList = new List<PieceMoves>();
            idShuffleOrder = idShuffleOrder.OrderByDescending(i => i.Item1).ToList();
            
            foreach (var move in idShuffleOrder)
            {
                moveList.Add(move.Item2);
            }

            idShuffleOrder.Clear();

            return moveList;
        }

        private void MoveTo(List<PieceMoves> moveList, int positionFrom, int positionTo)
        {
            var toMove = moveList[positionFrom];
            moveList.RemoveAt(positionFrom);
            moveList.Insert(positionTo, toMove);
        }

        private void MoveToFront(List<PieceMoves> moveList, int position)
        {
            var toMove = moveList[position];
            moveList.RemoveAt(position);
            moveList.Insert(0, toMove);
        }

        private void MoveToFront(List<PieceMoves> moveList, PieceMoves move)
        {
            if (moveList.Contains(move))
            {
                moveList.Remove(move);
                moveList.Insert(0, move);   
            }
        }

        #endregion move ordering

        #region Move type

        private bool IsPromotion(SpecialMoveType specialMoveType)
        {
            if (specialMoveType == SpecialMoveType.BishopPromotion || specialMoveType == SpecialMoveType.BishopPromotionCapture
                || specialMoveType == SpecialMoveType.KnightPromotion || specialMoveType == SpecialMoveType.KnightPromotionCapture
                || specialMoveType == SpecialMoveType.RookPromotion || specialMoveType == SpecialMoveType.RookPromotionCapture
                || specialMoveType == SpecialMoveType.QueenPromotion || specialMoveType == SpecialMoveType.QueenPromotionCapture)
                return true;

            return false;

        }

        private bool IsPromotionCapture(SpecialMoveType specialMoveType)
        {
            if (specialMoveType == SpecialMoveType.BishopPromotionCapture
                || specialMoveType == SpecialMoveType.KnightPromotionCapture
                || specialMoveType == SpecialMoveType.RookPromotionCapture
                || specialMoveType == SpecialMoveType.QueenPromotionCapture)
                return true;

            return false;

        }

        private bool IsCapture(SpecialMoveType specialMoveType)
        {
            if (specialMoveType != SpecialMoveType.Normal)
            {
                if (specialMoveType == SpecialMoveType.Capture)
                    return true;

                if (IsPromotionCapture(specialMoveType))
                    return true;
            }

            return false;
        }

        private bool IsCastlingMove(PieceMoves currentMove)
        {
            if (currentMove.SpecialMove == SpecialMoveType.KingCastle || currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                return true;

            return false;

        }

        #endregion Move type


        private void GetKingState(out bool isInCheck, out bool canMove)
        {
            canMove = false;

            if(m_BoardPosition.WhiteToMove)
            {
                isInCheck = BoardChecking.IsKingInCheck(m_BoardPosition, PieceColour.White);
                //canMove = BoardChecking.CanKingMove(boardPosition, PieceColour.White); 
            }
            else
            {
                isInCheck = BoardChecking.IsKingInCheck(m_BoardPosition, PieceColour.Black);
                //canMove = BoardChecking.CanKingMove(boardPosition, PieceColour.Black); 
            }                        
        }

        #region extension methods

        /// <summary>
        /// Decides if the depth search needs to be extended
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private int DetermineExtensions(int depth)
        {
            var extension = 0;

            //if (boardPosition.WhiteToMove)
            //{
                //if (boardPosition.WhiteIsInCheck)
                    extension++;
            //}
            //else
            //{
           //     if (boardPosition.BlackIsInCheck)
            //        extension++;
           // }

            return extension;
        }

        #endregion extension methods

        /// <summary>
        /// Evaluates the score relative to the current player
        /// i.e. A high score means the position is better for the current player 
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <returns></returns>
        private decimal Evaluate(IBoard boardPosition)
        {
            if (boardPosition.WhiteToMove)
                return m_ScoreCalc.CalculateScore(boardPosition);
            else
                return -m_ScoreCalc.CalculateScore(boardPosition);
        }

        /// <summary>
        /// Evaluates the end game relative to the current player
        /// (i.e. A low score if the current player loses
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private decimal EvaluateEndGame(int depth, bool isInCheck)
        {
            var movesToend = (startDepth - depth) + 1;  //Since we want lower depth mates to score lower
            //Check for draw 

            //else
            //if (boardPosition.WhiteToMove)
            //{
                //if (BoardChecking.IsKingInCheck(boardPosition, PieceColour.White))
                if(isInCheck)
                    return decimal.MinValue/2 + (100000 * movesToend);  //White is in checkmate
                else
                    return 0;   //stalemate
            //}
            //else
            //{
            //    //if (BoardChecking.IsKingInCheck(boardPosition, PieceColour.Black))
            //    if (isInCheck)
            //        return decimal.MinValue/2;
            //    else
            //        return 0;   //stalemate
            //}
        }

        /// <summary>
        /// Records the given hash in the transposition table 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="score"></param>
        /// <param name="hashNodeType"></param>
        private void RecordHash(int depth, decimal score, HashNodeType hashNodeType, PieceMoves bestMove)
        {
            var hash = new Hash
            {
                Key = m_BoardPosition.Zobrist,
                Depth = depth,
                NodeType = hashNodeType,
                Score = score,
                BestMove = bestMove
            };


            TranspositionTable.Add(hash);
        }

        private void RecordHash(int depth, decimal score, HashNodeType hashNodeType)
        {
            var hash = new Hash
            {
                Key = m_BoardPosition.Zobrist,
                Depth = depth,
                NodeType = hashNodeType,
                Score = score
            };

        TranspositionTable.Add(hash);
        }

        #region Logging

        //private void LogPrincipalVariation(int depth)
        //{
        //    string pvList = string.Empty;

        //    foreach (PieceMoves move in principalVariation)
        //    {
        //        pvList += UciMoveTranslator.ToUciMove(move);
        //        pvList += ", ";
        //    }

        //    log.Info(string.Format("Principal variation at depth {0}: {1}", depth, pvList));
        //}

        private void LogKillerMoves()
        {
            log.Info("Killer moves");

            for (var i = 0; i < primaryKillerMoves.Length; i++)
			{
			    var killerMove = primaryKillerMoves[i];
                var killerMove2 = secondaryKillerMoves[i];

                var p1 = "null";
                var p2 = "null";

                var score1 = string.Empty;
                var score2 = string.Empty;

                if (killerMove != null)
                {
                    p1 = UciMoveTranslator.ToUciMove(killerMove.Item1);
                    score1 = "-" + killerMove.Item2;
                }

                if (killerMove2 != null)
                {
                    p2 = UciMoveTranslator.ToUciMove(killerMove2.Item1);
                    score2 = "-" + killerMove2.Item2;
                }


                log.Info($"{i}: {p1}{score1} || {p2}{score2}");
                
                //else
                //    break;

			}           
        }

        #endregion Logging
    }
}
