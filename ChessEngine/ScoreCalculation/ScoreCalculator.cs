using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.Debugging;
using ChessEngine.PossibleMoves;

namespace ChessEngine.ScoreCalculation
{
    /// <summary>
    /// Calculates the score of a particular board position
    /// </summary>
    public class ScoreCalculator : IScoreCalculator
    {
        private Board m_CurrentBoard;

        private readonly byte endGameCount = 3;

        #region Properties

        public bool IsEndGame { get; private set; }

        #region Piece values

        public int PawnPieceValue { get; set; }

        public int KnightPieceValue { get; set; }

        public int BishopPieceValue { get; set; }

        public int RookPieceValue { get; set; }

        public int QueenPieceValue { get; set; }

        #endregion Piece values

        #region pawn position score properties


        public int InnerCentralPawnScore { get; set; }
        public int OuterCentralPawnScore { get; set; }


        public int InnerCentralKnightScore { get; set; }
        public int OuterCentralKnightScore { get; set; }

        public int InnerCentralBishopScore { get; set; }
        public int OuterCentralBishopScore { get; set; }
        
        public int InnerCentralRookScore { get; set; }
        public int OuterCentralRookScore { get; set; }
        
        public int InnerCentralQueenScore { get; set; }
        public int OuterCentralQueenScore { get; set; }

        #endregion pawn position score properties
        
        #region king safety bonuses

        public int CastlingKingSideScore { get; set; }
        public int CastlingQueenSideScore { get; set; }
        public int CanCastleKingsideScore { get; set; }
        public int CanCastleQueensideScore { get; set; }

        #endregion king safety bonuses

        #region square table values

        public int[] PawnSquareTable { get; } = new int[64];

        public int[] KnightSquareTable { get; } = new int[64];

        public int[] BishopSquareTable { get; } = new int[64];

        public int[] KingSquareTable { get; } = new int[64];

        public int[] KingEndGameSquareTable { get; } = new int[64];

        #endregion square table values

        public int DevelopedPieceScore { get; set; }

        public int EarlyQueenMovePenalty { get; set; }

        public int DoubleBishopScore { get; set; }

        public int  SoloQueenScore { get; set; }

        public int DoubledPawnPenalty { get; set; }
        public int PawnChainScore { get; set; }

        public int PassedPawnBonus { get; set; }

        public int PassedPawnAdvancementBonus { get; set; }

        public int ConnectedRookBonus { get; set; }

        public int BoardCoverageBonus { get; set; }
        public int QueenCoverageBonus { get; set; }
        public int RookCoverageBonus { get; set; }
        public int BishopCoverageBonus { get; set; }
        public int AttackBonus { get; set; }
        public int MoreValuablePieceAttackBonus { get; set; }

        #endregion Properties

        #region Initialisation methods

        public ScoreCalculator(string xmlFileName)
        {
            ScoreValueXmlReader.ReadScores(this, xmlFileName);
        }

        #endregion

        /// <summary>
        /// Calculates the score with black advantage as negative and white as positive
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <returns></returns>
        public int CalculateScore(Board currentBoard)
        {

            CountDebugger.Evaluations++;

            m_CurrentBoard = currentBoard;

            //StaticBoardChecks.Calculate(currentBoard);

            DetectEndGame();

            int score = 0;

            score += CalculatePieceValues();
                       
            score += CalculatePositionValues();
            score += CalculateKingSafetyScores();

            score += CalculateDevelopmentBonus();

            score += CalculateSquareTableScores();

            score += CalculateCoverageAndAttackScores();

            return score;
        }

        private void DetectEndGame()
        {
            if((BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKnights) 
                + BitboardOperations.GetPopCount(m_CurrentBoard.WhiteBishops)
                + BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks)
                + BitboardOperations.GetPopCount(m_CurrentBoard.WhiteQueen)) <= endGameCount
                &&
            (BitboardOperations.GetPopCount(m_CurrentBoard.BlackKnights)
                + BitboardOperations.GetPopCount(m_CurrentBoard.BlackBishops)
                + BitboardOperations.GetPopCount(m_CurrentBoard.BlackRooks)
                + BitboardOperations.GetPopCount(m_CurrentBoard.BlackQueen)) <= endGameCount)
                IsEndGame = true;
            else
                IsEndGame = false;

        }

        /// <summary>
        /// Calculates points for the pieces each player has on the board
        /// </summary>
        /// <returns></returns>
        private int CalculatePieceValues()
        {
            var kingScore = (int.MaxValue/6);

            int pieceScore = 0;

            //Calculate white piece values
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhitePawns) * PawnPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKnights) * KnightPieceValue;

            var whiteBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.WhiteBishops);
            pieceScore += whiteBishopCount * BishopPieceValue;

            if (whiteBishopCount == 2)
            {
                pieceScore += DoubleBishopScore;
            }

            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) * RookPieceValue;

            var whiteQueenCount = BitboardOperations.GetPopCount(m_CurrentBoard.WhiteQueen);

            pieceScore += whiteQueenCount * QueenPieceValue;
            pieceScore += whiteQueenCount * SoloQueenScore;

            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKing) * kingScore;

            //Calculate black piece values
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackPawns) * PawnPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKnights) * KnightPieceValue;

            var blackBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.BlackBishops);
            pieceScore -= blackBishopCount * BishopPieceValue;

            if (blackBishopCount == 2)
            {
                pieceScore -= DoubleBishopScore;
            }

            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackRooks) * RookPieceValue;

            var blackQueenCount = BitboardOperations.GetPopCount(m_CurrentBoard.BlackQueen);

            pieceScore -= blackQueenCount * QueenPieceValue;
            pieceScore -= blackQueenCount * SoloQueenScore;

            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKing) * kingScore;
            
            return pieceScore;
        }

        #region position values methods

        /// <summary>
        /// Points for having well positioned pieces 
        /// Note: Counts only position. No other factor
        /// </summary>
        /// <returns></returns>
        private int CalculatePositionValues()
        {
            int positionScores = 0;

            positionScores += CalculatePawnStructureScores();
            positionScores += CalculateCentralPieceScores();
                       
            return positionScores;
        }

        /// <summary>
        /// DoubledPawnPenalty
        /// PawnChainBonus
        /// </summary>
        /// <returns></returns>
        private int CalculatePawnStructureScores()
        {
            int pawnStructureScore = 0;
            
            //Doubled pawns
            var whiteDoubleCount = 0;
            var blackDoubleCount = 0;
            
            for (var i = 0; i < 8; i++)
			{
                var mask = LookupTables.FileMaskByColumn[i];

                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.WhitePawns) > 1)
                {
                    whiteDoubleCount++;
                }

                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.BlackPawns) > 1)
                {
                    blackDoubleCount++;
                }
            }

            pawnStructureScore += whiteDoubleCount * DoubledPawnPenalty;
            pawnStructureScore -= blackDoubleCount * DoubledPawnPenalty;
            
            //Pawn chain 
            var notA = 18374403900871474942;
            ulong notH = 9187201950435737471;

            //White pawns 
            var wPawnAttackSquares = ((m_CurrentBoard.WhitePawns << 9) & notA) | (m_CurrentBoard.WhitePawns << 7) & notH;
            var wProtectedPawns = wPawnAttackSquares & m_CurrentBoard.WhitePawns;

            pawnStructureScore += BitboardOperations.GetPopCount(wProtectedPawns) * PawnChainScore;

            //Black pawns
            var bPawnAttackSquares = ((m_CurrentBoard.BlackPawns >> 9) & notH) | (m_CurrentBoard.BlackPawns >> 7) & notA;
            var bProtectedPawns = bPawnAttackSquares & m_CurrentBoard.BlackPawns;

            pawnStructureScore -= BitboardOperations.GetPopCount(bProtectedPawns) * PawnChainScore;

            // Passed pawn bonus

            foreach (var whitePawnBoard in BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhitePawns))
            {
                //Pawn is on 7th rank so it can promote
                if ((whitePawnBoard & LookupTables.RankMask7) != 0)
                {
                    pawnStructureScore += PassedPawnBonus;
                    pawnStructureScore += PassedPawnAdvancementBonus * 5;

                    continue;
                }

                var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(whitePawnBoard);

                var pawnFrontSpan = LookupTables.WhitePawnFrontSpan[pawnIndex];

                if ((pawnFrontSpan & m_CurrentBoard.BlackPawns) == 0)
                {
                    pawnStructureScore += PassedPawnBonus;
                    pawnStructureScore += PassedPawnAdvancementBonus * ((pawnIndex / 8) - 1);
                }
            }

            foreach (var blackPawnBoard in BitboardOperations.SplitBoardToArray(m_CurrentBoard.BlackPawns))
            {
                //Pawn is on 2nd rank so it can promote
                if ((blackPawnBoard & LookupTables.RankMask2) != 0)
                {
                    pawnStructureScore -= PassedPawnBonus;
                    pawnStructureScore -= PassedPawnAdvancementBonus * 5;
                    continue;
                }

                var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(blackPawnBoard);

                var pawnFrontSpan = LookupTables.BlackPawnFrontSpan[pawnIndex];

                if ((pawnFrontSpan & m_CurrentBoard.WhitePawns) == 0)
                {
                    pawnStructureScore -= PassedPawnBonus;
                    pawnStructureScore -= PassedPawnAdvancementBonus * (8 - (pawnIndex / 8) - 2);
                }
            }

            return pawnStructureScore;
        }

        /// <summary>
        /// Points for placing pieces near to the centre
        /// </summary>
        /// <returns></returns>
        private int CalculateCentralPieceScores()
        {
            var innerCentralSquares = UsefulBitboards.InnerCentralSquares;
            var outerCentralSquares = UsefulBitboards.OuterCentralSquares;
            
            int piecePositionScore = 0;

            //Pawns
            var whitePawnBoard = m_CurrentBoard.WhitePawns;

            piecePositionScore += CalculatePositionScores(whitePawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore += CalculatePositionScores(whitePawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            var blackPawnBoard = m_CurrentBoard.BlackPawns;

            piecePositionScore -= CalculatePositionScores(blackPawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore -= CalculatePositionScores(blackPawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            //Knights
            var whiteKnightBoard = m_CurrentBoard.WhiteKnights;

            piecePositionScore += CalculatePositionScores(whiteKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore += CalculatePositionScores(whiteKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            var blackKnightBoard = m_CurrentBoard.BlackKnights;

            piecePositionScore -= CalculatePositionScores(blackKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore -= CalculatePositionScores(blackKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            //Bishops
            var whiteBishopBoard = m_CurrentBoard.WhiteBishops;

            piecePositionScore += CalculatePositionScores(whiteBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore += CalculatePositionScores(whiteBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            var blackBishopBoard = m_CurrentBoard.BlackBishops;

            piecePositionScore -= CalculatePositionScores(blackBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore -= CalculatePositionScores(blackBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            //Rooks
            var whiteRookBoard = m_CurrentBoard.WhiteRooks;

            piecePositionScore += CalculatePositionScores(whiteRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore += CalculatePositionScores(whiteRookBoard, outerCentralSquares) * OuterCentralRookScore;

            var blackRookBoard = m_CurrentBoard.BlackRooks;

            piecePositionScore -= CalculatePositionScores(blackRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore -= CalculatePositionScores(blackRookBoard, outerCentralSquares) * OuterCentralRookScore;

            //Queens
            var whiteQueenBoard = m_CurrentBoard.WhiteQueen;

            piecePositionScore += CalculatePositionScores(whiteQueenBoard, innerCentralSquares) * InnerCentralQueenScore;
            piecePositionScore += CalculatePositionScores(whiteQueenBoard, outerCentralSquares) * OuterCentralQueenScore;

            var blackQueenBoard = m_CurrentBoard.BlackQueen;

            piecePositionScore -= CalculatePositionScores(blackQueenBoard, innerCentralSquares) * InnerCentralQueenScore;
            piecePositionScore -= CalculatePositionScores(blackQueenBoard, outerCentralSquares) * OuterCentralQueenScore;
            
            return piecePositionScore;
        }
        
        private int CalculatePositionScores(ulong pieces, ulong positions)
        {
            var inPosition = pieces & positions;

            if (inPosition > 0)
                return BitboardOperations.GetPopCount(inPosition);

            return 0;
        }

        #endregion position values methods

        #region King safety scores

        private int CalculateKingSafetyScores()
        {
            int kingSafetyScore = 0;

            kingSafetyScore += CalculateKingCastlingScores();
            kingSafetyScore += CalculateCanCastleScores();

            //Add king pawn protection scores
            //Add king position scores (stay away from the middle early/mid game)

            return kingSafetyScore;
        }

        private int CalculateKingCastlingScores()
        {            
            int castlingScore = 0;

            if (!IsEndGame)
            {
                if (m_CurrentBoard.WhiteKing == LookupTables.G1 && (m_CurrentBoard.WhiteRooks & LookupTables.F1) != 0)
                    castlingScore += CastlingKingSideScore;
                else if (m_CurrentBoard.WhiteKing == LookupTables.C1 && (m_CurrentBoard.WhiteRooks & LookupTables.D1) != 0)
                    castlingScore += CastlingQueenSideScore;

                if (m_CurrentBoard.BlackKing == LookupTables.G8 && (m_CurrentBoard.BlackRooks & LookupTables.F8) != 0)
                    castlingScore -= CastlingKingSideScore;
                else if (m_CurrentBoard.BlackKing == LookupTables.C8 && (m_CurrentBoard.BlackRooks & LookupTables.D8) != 0)
                    castlingScore -= CastlingQueenSideScore;

            }
            return castlingScore;
        }

        private int CalculateCanCastleScores()
        {
            int canCastleScore = 0;

            var state = m_CurrentBoard.GetCurrentBoardState();

            canCastleScore += Convert.ToInt32(state.WhiteCanCastleKingside) * CanCastleKingsideScore;
            canCastleScore += Convert.ToInt32(state.WhiteCanCastleQueenside) * CanCastleQueensideScore;

            canCastleScore -= Convert.ToInt32(state.BlackCanCastleKingside) * CanCastleKingsideScore;
            canCastleScore -= Convert.ToInt32(state.BlackCanCastleQueenside) * CanCastleQueensideScore;

            return canCastleScore;
        }

        #endregion King safety scores

        #region square table scores

        private int CalculateSquareTableScores()
        {
            int squareTableScores = 0;

            squareTableScores += CalculatePawnSquareTableScores();
            squareTableScores += CalculateKnightSquareTableScores();
            squareTableScores += CalculateBishopSquareTableScores();
            
            if(!IsEndGame)
                squareTableScores += CalculateKingSquareTableScores();
            else
                squareTableScores += CalculateKingEndGameSquareTableScores();

            return squareTableScores;
        }
        
        private int CalculatePawnSquareTableScores()
        {
            int pawnSquareTableScore = 0;

            pawnSquareTableScore += CalculateTableScores(m_CurrentBoard.WhitePawns, PawnSquareTable, true);

            pawnSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackPawns, PawnSquareTable, false);
            
            return pawnSquareTableScore;
        }

        private int CalculateKnightSquareTableScores()
        {
            int knightSquareTableScore = 0;

            knightSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKnights, KnightSquareTable, true);

            knightSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKnights, KnightSquareTable, false);

            return knightSquareTableScore;
        }

        private int CalculateBishopSquareTableScores()
        {
            int bishopSquareTableScore = 0;

            bishopSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteBishops, BishopSquareTable, true);

            bishopSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackBishops, BishopSquareTable, false);

            return bishopSquareTableScore;
        }

        private int CalculateKingSquareTableScores()
        {
            int kingSquareTableScore = 0;

            kingSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKing, KingSquareTable, true);

            kingSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKing, KingSquareTable, false);

            return kingSquareTableScore;
        }

        private int CalculateKingEndGameSquareTableScores()
        {
            int kingEndGameSquareTableScore = 0;

            kingEndGameSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKing, KingEndGameSquareTable, true);

            kingEndGameSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKing, KingEndGameSquareTable, false);

            return kingEndGameSquareTableScore;
        }

        private int CalculateTableScores(ulong board, int[] squareTableValues, bool isWhite)
        {
            int pieceScore = 0;

            List<byte> positions;
            
            if(isWhite)
                positions = BitboardOperations.GetSquareIndexesFromBoardValue(board);
            else
                positions = BitboardOperations.GetSquareIndexesFromBoardValue(BitboardOperations.FlipVertical(board));


            foreach (var squareIndex in positions)
            {
                //Debug.Assert(squareIndex >= 0);
                //Debug.Assert(squareIndex < 64);

                pieceScore += squareTableValues[squareIndex];
            }

            return pieceScore;
        }

        #endregion square table scores

        #region development score
         
        private int CalculateDevelopmentBonus()
        {
            int developedPieceBonus = 0;

            developedPieceBonus += CalculateDevelopedPieceBonus();
            developedPieceBonus += CalculateConnectedRookBonus();

            developedPieceBonus += CalculateEarlyQueenPenalty();

            return developedPieceBonus;
        }

        /// <summary>
        /// Points for pieces not on the back rank (bishops & knights)
        /// </summary>
        /// <returns></returns>
        private int CalculateDevelopedPieceBonus()
        {
            var developedPiecesScore = 0;

            ulong whiteBack = 255;
            var blackBack = 18374686479671623680;

            var whiteDevelopedPieces = (m_CurrentBoard.WhiteBishops ^ m_CurrentBoard.WhiteKnights ^ m_CurrentBoard.WhiteQueen) & ~whiteBack;
            var developedWhitePieceCount = BitboardOperations.GetPopCount(whiteDevelopedPieces);
            developedPiecesScore += developedWhitePieceCount * DevelopedPieceScore;
            
            var blackDevelopedPieces = (m_CurrentBoard.BlackBishops ^ m_CurrentBoard.BlackKnights ^ m_CurrentBoard.BlackQueen) & ~blackBack;
            var developedBlackPieceCount = BitboardOperations.GetPopCount(blackDevelopedPieces);
            developedPiecesScore -= developedBlackPieceCount * DevelopedPieceScore;

            return developedPiecesScore;
        }

        private int CalculateConnectedRookBonus()
        {
            int connectedRookScore = 0;

            if (BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) > 1)
            {
                var rooks = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhiteRooks);

                var firstRook = rooks[0];
                var secondRook = rooks[1];

                if ((BoardChecking.FindRightBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindLeftBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindUpBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindDownBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
            }

            if (BitboardOperations.GetPopCount(m_CurrentBoard.BlackRooks) > 1)
            {
                var rooks = BitboardOperations.SplitBoardToArray(m_CurrentBoard.BlackRooks);

                var firstRook = rooks[0];
                var secondRook = rooks[1];

                if ((BoardChecking.FindRightBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindLeftBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindUpBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindDownBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
            }

            return connectedRookScore;
        }

        private int CalculateEarlyQueenPenalty()
        {
            var earlyQueenPenaltyScore = 0; 

            ulong whiteUndevelopedPieceBoard = 0;

            whiteUndevelopedPieceBoard |= m_CurrentBoard.WhiteBishops & 36;  //Any white bishops on C1 or F1
            whiteUndevelopedPieceBoard |= m_CurrentBoard.WhiteKnights & 66;  //Any white knights on B1 orG1

            var whiteUndevelopedPieceCount = BitboardOperations.GetPopCount(whiteUndevelopedPieceBoard);

            // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
            if (whiteUndevelopedPieceCount >= 2 && (m_CurrentBoard.WhiteQueen & ~8UL) != 0)
            {
                earlyQueenPenaltyScore -= EarlyQueenMovePenalty;
            }

            ulong blackUndevelopedPieceBoard = 0;

            //Any black bishops on C8 or F8
            blackUndevelopedPieceBoard |= m_CurrentBoard.BlackBishops & 2594073385365405696;
            //Any black knights on B8 or G8
            blackUndevelopedPieceBoard |= m_CurrentBoard.BlackKnights & 66;  

            int blackUndevelopedPieceCount = BitboardOperations.GetPopCount(blackUndevelopedPieceBoard);

            // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
            if (blackUndevelopedPieceCount <= 3 && (m_CurrentBoard.BlackQueen & ~576460752303423488UL) != 0)
            {
                earlyQueenPenaltyScore += EarlyQueenMovePenalty;
            }

            return earlyQueenPenaltyScore;
        }

        #endregion development score

        #region coverage scores

        /// <summary>
        /// BoardCoverageBonus
        /// 
        /// QueenCoverageBonus
        /// RookCoverageBonus
        /// BishopCoverageBonus
        /// 
        /// AttackBonus
        /// </summary>
        /// <returns></returns>
        private int CalculateCoverageAndAttackScores()
        {
            int attackScore = 0;

            ulong whiteCoverageBoard = 0;   // All the empty squares white can move to next turn

            ulong whiteAttackBoard = 0;     // All squares with black pieces white can move to next turn

            // White
            // Bonus for bishop 
            var whiteBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, m_CurrentBoard.WhiteBishops, whiteToMove: true);                                       ;
            
            if (whiteBishopPossibleMoves > 0)
            {
                var whiteBishopCoverageBoard = whiteBishopPossibleMoves & ~m_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(whiteBishopCoverageBoard) 
                               * BishopCoverageBonus; // Add points for white bishop coverage
                
                whiteCoverageBoard |= whiteBishopCoverageBoard;

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whiteBishopPossibleMoves 
                    & (m_CurrentBoard.BlackQueen | m_CurrentBoard.BlackRooks)) 
                    * MoreValuablePieceAttackBonus;
                
                whiteAttackBoard |= whiteBishopPossibleMoves & m_CurrentBoard.AllBlackOccupiedSquares;             
            }

            // Bonus for rook coverage
            var whiteRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, 
                                                                                 m_CurrentBoard.WhiteRooks,
                                                                                 whiteToMove: true);

            if (whiteRookPossibleMoves > 0)
            {
                var whiteRookCoverageBoard = 
                    whiteRookPossibleMoves 
                    & ~m_CurrentBoard.AllBlackOccupiedSquares;

                attackScore += BitboardOperations.GetPopCount(whiteRookCoverageBoard) * RookCoverageBonus;

                whiteCoverageBoard |= whiteRookCoverageBoard;

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whiteRookPossibleMoves & m_CurrentBoard.BlackQueen) * MoreValuablePieceAttackBonus;

                whiteAttackBoard |= whiteRookPossibleMoves & m_CurrentBoard.AllBlackOccupiedSquares;               
            }

            // Bonus for queen coverage
            var whiteQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, 
                                                                                   m_CurrentBoard.WhiteQueen,
                                                                                   whiteToMove: true);
            if (whiteQueenPossibleMoves > 0)
            {
                var whiteQueenCoverageBoard = whiteQueenPossibleMoves & ~m_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(whiteQueenCoverageBoard) * QueenCoverageBonus;

                whiteCoverageBoard |= whiteQueenCoverageBoard;

                whiteAttackBoard |= whiteQueenPossibleMoves & m_CurrentBoard.AllBlackOccupiedSquares;
            }

            // Bonus for knight coverage
            ulong whiteKnightPossibleMoves = 0;

            var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteKnights);

            foreach (var knightPos in whiteKnightPositions)
	        {
                var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                possibleMoves = possibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;
                whiteKnightPossibleMoves |= possibleMoves;
	        }

            whiteCoverageBoard |= whiteKnightPossibleMoves & ~m_CurrentBoard.AllBlackOccupiedSquares;

            whiteAttackBoard |= whiteKnightPossibleMoves & m_CurrentBoard.AllBlackOccupiedSquares;

            // Points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteKnightPossibleMoves & (m_CurrentBoard.BlackQueen | m_CurrentBoard.BlackRooks)) * MoreValuablePieceAttackBonus;

            //Pawns
            const ulong notA = 18374403900871474942;
            const ulong notH = 9187201950435737471;

            var whitePawnPossibleAttackMoves = ((m_CurrentBoard.WhitePawns << 9) & notA) 
                                               | ((m_CurrentBoard.WhitePawns << 7) & notH);

            if (whitePawnPossibleAttackMoves > 0)
            {
                // There is no bonus for pawn coverage, just pawn attacks

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whitePawnPossibleAttackMoves 
                        & (m_CurrentBoard.BlackQueen 
                           | m_CurrentBoard.BlackRooks 
                           | m_CurrentBoard.BlackBishops 
                           | m_CurrentBoard.BlackKnights)) 
                * MoreValuablePieceAttackBonus;

                whiteAttackBoard |= whitePawnPossibleAttackMoves & m_CurrentBoard.AllBlackOccupiedSquares;
            }

            // Points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteKnightPossibleMoves 
                & (m_CurrentBoard.BlackQueen | m_CurrentBoard.BlackRooks)) 
                * MoreValuablePieceAttackBonus;

            // Points for overall board coverage
            attackScore += BitboardOperations.GetPopCount(whiteCoverageBoard) * BoardCoverageBonus;

            // Points for board attacks
            attackScore += BitboardOperations.GetPopCount(whiteAttackBoard) * AttackBonus;

            //Black
            ulong blackCoverageBoard = 0;   // All the empty squares black can move to next turn
            ulong blackAttackBoard = 0;    // All white squares black can move to next turn

            // Bonus for bishop coverage
            var blackBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, 
                                                                                     m_CurrentBoard.BlackBishops,
                                                                                     whiteToMove: false);

            if (blackBishopPossibleMoves > 0)
            {
                var blackBishopCoverageBoard = blackBishopPossibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;

                // Add points for black bishop coverage
                attackScore -= BitboardOperations.GetPopCount(blackBishopCoverageBoard) 
                               * BishopCoverageBonus; 

                blackCoverageBoard |= blackBishopCoverageBoard;

                // Points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackBishopPossibleMoves & (m_CurrentBoard.WhiteQueen | m_CurrentBoard.WhiteRooks))
                    * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackBishopPossibleMoves & m_CurrentBoard.AllWhiteOccupiedSquares;
            }

            // Bonus for rook coverage
            var blackRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, 
                                                                                 m_CurrentBoard.BlackRooks, 
                                                                                 whiteToMove: false);

            if (blackRookPossibleMoves > 0)
            { 
                var blackRookCoverageBoard = blackRookPossibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;

                // Add points for black rook coverage
                attackScore -= BitboardOperations.GetPopCount(blackRookCoverageBoard) * RookCoverageBonus; 

                blackCoverageBoard |= blackRookCoverageBoard;

                // Points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackRookPossibleMoves & m_CurrentBoard.WhiteQueen) * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackRookPossibleMoves & m_CurrentBoard.AllWhiteOccupiedSquares;
            }

            // Bonus for queen coverage
            var blackQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, 
                                                                                   m_CurrentBoard.BlackQueen,
                                                                                   whiteToMove: false);

            if (blackQueenPossibleMoves > 0)
            { 
                var blackQueenCoverageBoard = blackQueenPossibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;
                attackScore -= BitboardOperations.GetPopCount(blackQueenCoverageBoard) * QueenCoverageBonus;

                blackCoverageBoard |= blackQueenCoverageBoard;

                blackAttackBoard |= blackQueenPossibleMoves & m_CurrentBoard.AllWhiteOccupiedSquares;
            }

            //// Bonus for knight coverage
            ulong blackKnightPossibleMoves = 0;

            var blackKnightPositions = 
                BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackKnights);

            foreach (var knightPos in blackKnightPositions)
            {
                var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                possibleMoves = possibleMoves & ~m_CurrentBoard.AllBlackOccupiedSquares;
                blackKnightPossibleMoves |= possibleMoves;
            }

            blackCoverageBoard |= blackKnightPossibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;

            blackAttackBoard |= blackKnightPossibleMoves & m_CurrentBoard.AllWhiteOccupiedSquares;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(
                blackKnightPossibleMoves & (m_CurrentBoard.WhiteQueen | m_CurrentBoard.WhiteRooks)) 
                * MoreValuablePieceAttackBonus;

            //Pawns
            var blackPawnPossibleAttackMoves = ((m_CurrentBoard.BlackPawns >> 7) & notA) 
                                               | ((m_CurrentBoard.BlackPawns >> 7) & notH);

            if (blackPawnPossibleAttackMoves > 0)
            {
                // There is no bonus for pawn coverage, just pawn attacks
                // Add points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackPawnPossibleAttackMoves & (m_CurrentBoard.WhiteQueen 
                                                    | m_CurrentBoard.WhiteRooks 
                                                    | m_CurrentBoard.WhiteBishops 
                                                    | m_CurrentBoard.WhiteKnights))
                    * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackPawnPossibleAttackMoves & m_CurrentBoard.AllWhiteOccupiedSquares;
            }

            // Points for overall board coverage
                attackScore -= BitboardOperations.GetPopCount(blackCoverageBoard) * BoardCoverageBonus;

            // Points for board attacks
            attackScore -= BitboardOperations.GetPopCount(blackAttackBoard) * AttackBonus;

            return attackScore;
        }

        #endregion coverage scores
    }
}
