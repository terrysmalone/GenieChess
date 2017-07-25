using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using ChessGame.BoardSearching;
using ChessGame.Debugging;
using ChessGame.NotationHelpers;
using log4net;
using System.Threading;
using ChessGame.BoardRepresentation.Enums;

namespace ChessGame.MoveSearching
{
    /// <summary>
    /// Similar http://www.chessbin.com/post/Move-Searching-and-Alpha-Beta
    /// 
    /// https://en.wikipedia.org/wiki/Negamax#Negamax_with_alpha_beta_pruning
    /// http://cis-linux1.temple.edu/~giorgio/cis587/readings/alpha-beta.html
    /// </summary>
    public sealed class AlphaBetaSearch
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Board boardPosition;
        private ScoreCalculator scoreCalc;

        private int startDepth; 

        private Tuple<PieceMoves, decimal>[] primaryKillerMoves;
        private Tuple<PieceMoves, decimal>[] secondaryKillerMoves; 
        
        #region iterative deepening variables

        private bool useIterativeDeepening;

        private PieceMoves bestIDMove;
        private decimal bestIDScore;
        private List<PVInfo> idMoves;

        private List<Tuple<decimal, PieceMoves>> idShuffleOrder;
        
        #endregion iterative deepening variables
               
        private int nullMoveR = 3;

        #region properties

        int EXTENSION_LIMIT = 10;

        public List<PVInfo> IdMoves
        {
            get { return idMoves; }
        }

        #endregion properties

        public AlphaBetaSearch(Board boardPosition, ScoreCalculator scoreCalc)
        {
            this.boardPosition = boardPosition;
            this.scoreCalc = scoreCalc;
        }

        /// <summary>
        /// Begins an iterative deepening search
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public PieceMoves StartSearch(int maxDepth)
        {
            string toMove = "black";
            if (boardPosition.WhiteToMove)
                toMove = "white";

            log.Info(string.Format("Calculating move for {0}", toMove));
            
            //Reset iterative deepening variables
            useIterativeDeepening = true;

            idMoves = new List<PVInfo>();
            bestIDMove = new PieceMoves() { Moves = 0, Position = 0, SpecialMove = SpecialMoveType.Normal, Type = PieceType.None };
            bestIDScore = 0;

            //Reset killer moves
             primaryKillerMoves = new Tuple<PieceMoves, decimal>[10];
             secondaryKillerMoves = new Tuple<PieceMoves, decimal>[10];
            
            idShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            for (int i = 1; i <= maxDepth; i++)
            {

                #if UCI
                    Console.WriteLine(String.Format("info depth {0}", i));               
                #endif
#if Debug
                CountDebugger.Evaluations = 0;
#endif

                Stopwatch timer = new Stopwatch();
                timer.Start();

                bestIDMove = MoveCalculate(i, out bestIDScore);

                timer.Stop();
                TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

                PVInfo pvInfo = new PVInfo();
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

                log.Info(string.Format("{0}: {1} - score:{2} - nodes:{3} - time at depth:{4}s - time for move:{5}s", i, UCIMoveTranslator.ToUCIMove(pvInfo.Move), pvInfo.Score, pvInfo.NodesVisited, pvInfo.DepthTime.ToString("ss'.'fff"), pvInfo.AccumulatedTime.ToString("ss'.'fff")));
                 //LogPrincipalVariation(i);               

#if UCI
                string bestMove = UCIMoveTranslator.ToUCIMove(bestIDMove);
                //Console.WriteLine(string.Format("Best move at depth {0}: {1}", i, bestMove));
                //Console.WriteLine(String.Format("info currmove {0} depth {1} nodes {2} ", bestMove, i, pvInfo.NodesVisited));
                //Console.WriteLine(String.Format("info score cp 0 {0} depth {1} nodes {2} time {3} ", bestMove, i, pvInfo.NodesVisited, pvInfo.DepthTime));
                Console.WriteLine(String.Format("info score cp 0 depth {0} nodes {1} pv {2} ", i, pvInfo.NodesVisited, bestMove));
                
                //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UCIMoveTranslator.ToUCIMove(bestIDMove)));
#endif
            }

            log.Info(string.Format("Found move {0}", UCIMoveTranslator.ToUCIMove(bestIDMove)));

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
            string toMove = "black";
            if (boardPosition.WhiteToMove)
                toMove = "white";

            log.Info(string.Format("Calculating move for {0}", toMove));

            
            //Reset iterative deepening variables
            useIterativeDeepening = true;

            idMoves = new List<PVInfo>();
            bestIDMove = new PieceMoves() { Moves = 0, Position = 0, SpecialMove = SpecialMoveType.Normal, Type = PieceType.None };
            bestIDScore = 0;

            idShuffleOrder = new List<Tuple<decimal, PieceMoves>>();

            Stopwatch moveTimer = new Stopwatch();
            moveTimer.Start();

            Thread timedMove = new Thread(new ThreadStart(MakeTimedMove));
            timedMove.Start();
            
            while(moveTimer.Elapsed.Ticks <= timeInMilliseconds)
            {
                
            }

            

            return bestIDMove;
        }

        private void MakeTimedMove()
        {
            int maxDepth = 30;

            for (int i = 1; i <= maxDepth; i++)
            {
#if Debug
                CountDebugger.Evaluations = 0;
#endif
                Stopwatch timer = new Stopwatch();
                timer.Start();

                bestIDMove = MoveCalculate(i, out bestIDScore);

                timer.Stop();
                TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

                PVInfo pvInfo = new PVInfo();
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

                log.Info(string.Format("{0}: {1} - score:{2} - nodes:{3} - time at depth:{4}s - time for move:{5}s", i, UCIMoveTranslator.ToUCIMove(pvInfo.Move), pvInfo.Score, pvInfo.NodesVisited, pvInfo.DepthTime.ToString("ss'.'fff"), pvInfo.AccumulatedTime.ToString("ss'.'fff")));
                //LogPrincipalVariation(i);               

#if UCI
                Console.WriteLine(string.Format("Best move at depth {0}: {1}", i, UCIMoveTranslator.ToUCIMove(bestIDMove)));
                //Console.WriteLine(string.Format("info Best move at depth {0}: {1}", i, UCIMoveTranslator.ToUCIMove(bestIDMove))); 
#endif
            }

            log.Info(string.Format("Found move {0}", UCIMoveTranslator.ToUCIMove(bestIDMove)));
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

            decimal alpha = Decimal.MinValue/2-1;
            decimal beta = Decimal.MaxValue/2+1;

            Debug.Assert(depth >= 1);

            decimal score = 0;

            PieceMoves bestMove = new PieceMoves();
            
            List<PieceMoves> moveList;

            if (useIterativeDeepening && idShuffleOrder.Count > 0)
                moveList = OrderFromIDMoves();
            else
            {
                moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(boardPosition));
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

            for (int i = 0; i < moveList.Count; i++)
            {
#if UCI
                Console.WriteLine(String.Format("info currmove {0} currmovenumber {1}",UCIMoveTranslator.ToUCIMove(moveList[i]),  i+1));
#endif

                boardPosition.MakeMove(moveList[i], false);

                //if (depth > 2)
                //{
                //    score = -AlphaBeta(-wBeta, -wAlpha, depth - 1, true);

                //    if (score <= wAlpha)
                //    {
                //        score = -AlphaBeta(-wBeta, -alpha, depth - 1, true);

                //        if (score >= wBeta)
                //            score = -AlphaBeta(-beta, -alpha, depth - 1, true);
                //    }
                //    else if (score >= wBeta)
                //    {
                //        score = -AlphaBeta(-beta, -wAlpha, depth - 1, true);

                //        if (score <= wAlpha)
                //            score = -AlphaBeta(-beta, -alpha, depth - 1, true);
                //    }
                //}
                //else
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

                boardPosition.UnMakeLastMove();
            }

            bestScore = alpha;
            return bestMove;
        }


        private decimal AlphaBeta(decimal alpha, decimal beta, int depth, bool allowNullMove)
        {
            return AlphaBeta(alpha, beta, depth, allowNullMove, false, 0);
        }

        private decimal AlphaBeta(decimal alpha, decimal beta, int depth, bool allowNullMove, bool isNullMoveSearch, int extensionDepth)
        {
            int searchDepth = startDepth - depth - 1;

            decimal alphaOrig = alpha;

            decimal score = Decimal.MinValue / 2 - 1;

            #region transposition table lookup

            PieceMoves bestMoveSoFar = new PieceMoves();
            Hash hash = TranspositionTable.ProbeTable(boardPosition.Zobrist, depth, alpha, beta);
            if (hash.Key != 0)
            {
                if (hash.Depth >= depth)
                {
                    decimal povScore = hash.Score;

                    if (!boardPosition.WhiteToMove)
                        povScore = -hash.Score;

                    if (hash.NodeType == HashNodeType.Exact)
                        return povScore;
                    else if (hash.NodeType == HashNodeType.LowerBound)
                        alpha = Math.Max(alpha, povScore);
                    else if (hash.NodeType == HashNodeType.UpperBound)
                        beta = Math.Min(beta, povScore);

                    if (alpha >= beta)
                        return povScore;
                    //return alpha;
                }

                if(hash.BestMove.Type != PieceType.None)
                    bestMoveSoFar = hash.BestMove; //move this to the front
            }

            #endregion transposition table lookup
                                    
            bool isPositionInCheck = false;
            bool canKingMove = false;
            GetKingState(out isPositionInCheck, out canKingMove);
            
            #region Check extensions

            int extensions = 0;

            if (depth == 0)
            {
                if (isPositionInCheck && extensionDepth < EXTENSION_LIMIT)
                {
                    extensions = DetermineExtensions(depth);

                    extensionDepth++;
                }
                else
                {
                    score = Evaluate(boardPosition);

                    if (boardPosition.WhiteToMove)
                        RecordHash(depth, score, HashNodeType.Exact);
                    else
                        RecordHash(depth, -score, HashNodeType.Exact);


                    return score;
                }
            }

            #endregion Check extensions
                        
            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            if (moveList.Count == 0)
                return EvaluateEndGame(depth, isPositionInCheck);

            #region null move pruning

#warning - Now that I do pseudo legal move generation this may be called if the player is in stalemate. I should sort this
#warning - This should go before calculating moves but until I move the check for endgame before this it can't
            if (!isPositionInCheck && allowNullMove && depth > nullMoveR)
            {
                boardPosition.SwitchSides();
                decimal eval = -AlphaBeta(-beta, -beta + 1, depth - 1 - nullMoveR, false, true, extensionDepth);

                //if (eval >= beta)
                //{
                //    RecordHash(depth - 1 - nullMoveR, score, HashNodeType.LowerBound);
                //}

                boardPosition.SwitchSides();

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
                    PieceMoves killerMove = primaryKillerMoves[searchDepth].Item1;

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
            
            int calculationDepth = depth + extensions;
            bool areAnyMovesLegal = false;

            PieceMoves bestMove = new PieceMoves();

            for (int i = 0; i < moveList.Count; i++)
            {
                PieceMoves currentMove = moveList[i];

                bool skipCastlingMove = false;
                if (IsCastlingMove(currentMove))
                {
                    if (isPositionInCheck || !MoveGeneration.ValidateCastlingMove(boardPosition, currentMove))                   
                    {
                        moveList.RemoveAt(i);
                        i--;
                        skipCastlingMove = true;
                    }
                }

                if (!skipCastlingMove)
                {
                    boardPosition.MakeMove(currentMove, false);

                    if (MoveGeneration.ValidateMove(boardPosition))
                    {
                        areAnyMovesLegal = true;

                        decimal val = -AlphaBeta(-beta, -alpha, calculationDepth - 1, true, isNullMoveSearch, extensionDepth);    //Flip and negate bounds

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
                                    Tuple<PieceMoves, decimal> primary = primaryKillerMoves[searchDepth];

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

                            boardPosition.UnMakeLastMove();
                            break;
                        }
                    }
                    //else
                    //{
                        //moveList.RemoveAt(i);
                        //i--;
                    //}

                    boardPosition.UnMakeLastMove();
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

            if (boardPosition.WhiteToMove)
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
            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                    PieceMoves moveToMove = moveList[i];
                    moveList.RemoveAt(i);
                    moveList.Insert(0, moveToMove);
                }
            }

            //Move all promotions 
            for (int i = 0; i < moveList.Count; i++)
            {
                if (IsPromotion(moveList[i].SpecialMove))
                {
                    PieceMoves moveToMove = moveList[i];
                    moveList.RemoveAt(i);
                    moveList.Insert(0, moveToMove);
                }
            }
        }

        private void OrderByMVVLVA(List<PieceMoves> moveList)
        {
            //position, victim attacker
            List<Tuple<int, int, int>> ordering = new List<Tuple<int, int, int>>();

            //Move capture
            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                    PieceType victimType = BoardChecking.GetPieceTypeOnSquare(boardPosition, moveList[i].Moves);
                    ordering.Add(new Tuple<int, int, int>(i, GetPieceScore(victimType), GetPieceScore(moveList[i].Type)));
                }
            }

            //Order by victim and then attacker
            ordering = ordering.OrderByDescending(o => o.Item2).ThenBy(o => o.Item3).ToList();

            for (int i = 0; i < ordering.Count; i++)
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
            List<Tuple<int, int>> ordering = new List<Tuple<int, int>>();

            //Move capture
            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i].SpecialMove == SpecialMoveType.Capture || moveList[i].SpecialMove == SpecialMoveType.ENPassantCapture || IsPromotionCapture(moveList[i].SpecialMove))
                {
                   PieceType victimType = BoardChecking.GetPieceTypeOnSquare(boardPosition, moveList[i].Moves);
                   ordering.Add(new Tuple<int, int>(i, GetPieceScore(victimType)-GetPieceScore(moveList[i].Type)));
                }
            }

            //Order by victim and then attacker
            ordering = ordering.OrderByDescending(o => o.Item2).ToList();

            int cutoff = 0;

            for (int i = 0; i < ordering.Count; i++)
            {
                if (ordering[i].Item2 >= 0)
                    MoveTo(moveList, ordering[i].Item1, i);
                else
                {
                    cutoff = i;
                    break;
                }
            }

            int end = moveList.Count - 1;

            for (int i = cutoff; i < ordering.Count; i++)
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
            List<PieceMoves> moveList = new List<PieceMoves>();
            idShuffleOrder = idShuffleOrder.OrderByDescending(i => i.Item1).ToList();
            
            foreach (Tuple<decimal, PieceMoves> move in idShuffleOrder)
            {
                moveList.Add(move.Item2);
            }

            idShuffleOrder.Clear();

            return moveList;
        }

        private void MoveTo(List<PieceMoves> moveList, int positionFrom, int positionTo)
        {
            PieceMoves toMove = moveList[positionFrom];
            moveList.RemoveAt(positionFrom);
            moveList.Insert(positionTo, toMove);
        }

        private void MoveToFront(List<PieceMoves> moveList, int position)
        {
            PieceMoves toMove = moveList[position];
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

            if(boardPosition.WhiteToMove)
            {
                isInCheck = BoardChecking.IsKingInCheckFast(boardPosition, PieceColour.White);
                //canMove = BoardChecking.CanKingMove(boardPosition, PieceColour.White); 
            }
            else
            {
                isInCheck = BoardChecking.IsKingInCheckFast(boardPosition, PieceColour.Black);
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
            int extension = 0;

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
        private decimal Evaluate(Board boardPosition)
        {
            if (boardPosition.WhiteToMove)
                return scoreCalc.CalculateScore(boardPosition);
            else
                return -scoreCalc.CalculateScore(boardPosition);
        }

        /// <summary>
        /// Evaluates the end game relative to the current player
        /// (i.e. A low score if the current player loses
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private decimal EvaluateEndGame(int depth, bool isInCheck)
        {
            int movesToend = (startDepth - depth) + 1;  //Since we want lower depth mates to score lower
            //Check for draw 

            //else
            //if (boardPosition.WhiteToMove)
            //{
                //if (BoardChecking.IsKingInCheckFast(boardPosition, PieceColour.White))
                if(isInCheck)
                    return decimal.MinValue/2 + (100000 * movesToend);  //White is in checkmate
                else
                    return 0;   //stalemate
            //}
            //else
            //{
            //    //if (BoardChecking.IsKingInCheckFast(boardPosition, PieceColour.Black))
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
            Hash hash = new Hash();

            hash.Key = boardPosition.Zobrist;
            hash.Depth = depth;
            hash.NodeType = hashNodeType;
            hash.Score = score;
            hash.BestMove = bestMove;

            TranspositionTable.Add(hash);
        }

        private void RecordHash(int depth, decimal score, HashNodeType hashNodeType)
        {
            Hash hash = new Hash();

            hash.Key = boardPosition.Zobrist;
            hash.Depth = depth;
            hash.NodeType = hashNodeType;
            hash.Score = score;

            TranspositionTable.Add(hash);
        }

        #region Logging

        //private void LogPrincipalVariation(int depth)
        //{
        //    string pvList = string.Empty;

        //    foreach (PieceMoves move in principalVariation)
        //    {
        //        pvList += UCIMoveTranslator.ToUCIMove(move);
        //        pvList += ", ";
        //    }

        //    log.Info(string.Format("Principal variation at depth {0}: {1}", depth, pvList));
        //}

        private void LogKillerMoves()
        {
            log.Info("Killer moves");

            for (int i = 0; i < primaryKillerMoves.Length; i++)
			{
			    Tuple<PieceMoves, decimal> killerMove = primaryKillerMoves[i];
                Tuple<PieceMoves, decimal> killerMove2 = secondaryKillerMoves[i];

                string p1 = "null";
                string p2 = "null";

                string score1 = string.Empty;
                string score2 = string.Empty;

                if (killerMove != null)
                {
                    p1 = UCIMoveTranslator.ToUCIMove(killerMove.Item1);
                    score1 = "-" + killerMove.Item2;
                }

                if (killerMove2 != null)
                {
                    p2 = UCIMoveTranslator.ToUCIMove(killerMove2.Item1);
                    score2 = "-" + killerMove2.Item2;
                }


                log.Info(string.Format("{0}: {1}{2} || {3}{4}", i, p1, score1, p2, score2));
                
                //else
                //    break;

			}           
        }

        #endregion Logging
    }
}
