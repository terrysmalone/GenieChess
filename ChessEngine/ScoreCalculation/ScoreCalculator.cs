using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
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
        private IBoard m_CurrentBoard;

        private readonly byte endGameCount = 3;

        #region Properties

        public bool IsEndGame { get; private set; }

        #region Piece values

        public decimal PawnPieceValue { get; set; }

        public decimal KnightPieceValue { get; set; }

        public decimal BishopPieceValue { get; set; }

        public decimal RookPieceValue { get; set; }

        public decimal QueenPieceValue { get; set; }

        #endregion Piece values

        #region pawn position score properties


        public decimal InnerCentralPawnScore { get; set; }
        public decimal OuterCentralPawnScore { get; set; }


        public decimal InnerCentralKnightScore { get; set; }
        public decimal OuterCentralKnightScore { get; set; }

        public decimal InnerCentralBishopScore { get; set; }
        public decimal OuterCentralBishopScore { get; set; }
        
        public decimal InnerCentralRookScore { get; set; }
        public decimal OuterCentralRookScore { get; set; }
        
        public decimal InnerCentralQueenScore { get; set; }
        public decimal OuterCentralQueenScore { get; set; }

        #endregion pawn position score properties
        
        #region king safety bonuses

        public decimal CastlingKingSideScore { get; set; }
        public decimal CastlingQueenSideScore { get; set; }
        public decimal CanCastleKingsideScore { get; set; }
        public decimal CanCastleQueensideScore { get; set; }

        #endregion king safety bonuses

        #region square table values

        public decimal[] PawnSquareTable { get; } = new decimal[64];

        public decimal[] KnightSquareTable { get; } = new decimal[64];

        public decimal[] BishopSquareTable { get; } = new decimal[64];

        public decimal[] KingSquareTable { get; } = new decimal[64];

        public decimal[] KingEndGameSquareTable { get; } = new decimal[64];

        #endregion square table values

        public decimal DevelopedPieceScore { get; set; }

        public decimal DoubleBishopScore { get; set; }

        public decimal DoubledPawnPenalty { get; set; }
        public decimal PawnChainScore { get; set; }

        public decimal ConnectedRookBonus { get; set; }

        public decimal BoardCoverageBonus { get; set; }
        public decimal QueenCoverageBonus { get; set; }
        public decimal RookCoverageBonus { get; set; }
        public decimal BishopCoverageBonus { get; set; }
        public decimal AttackBonus { get; set; }
        public decimal MoreValuablePieceAttackBonus { get; set; }

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
        public decimal CalculateScore(IBoard currentBoard)
        {
#if Debug
            CountDebugger.Evaluations++;
#endif
            m_CurrentBoard = currentBoard;

            //StaticBoardChecks.Calculate(currentBoard);

            DetectEndGame();

            decimal score = 0;

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
        private decimal CalculatePieceValues()
        {
            var kingScore = (decimal.MaxValue/6);

            decimal pieceScore = 0;

            //Calculate white piece values
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhitePawns) * PawnPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKnights) * KnightPieceValue;

            var whiteBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.WhiteBishops);

            pieceScore += whiteBishopCount * BishopPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) * RookPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteQueen) * QueenPieceValue;

            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKing) * kingScore;

            //Calculate black piece values
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackPawns) * PawnPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKnights) * KnightPieceValue;

            var blackBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.BlackBishops);

            pieceScore -= blackBishopCount * BishopPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackRooks) * RookPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackQueen) * QueenPieceValue;

            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKing) * kingScore;

            //Calculate double bishop bonus
            if (whiteBishopCount == 2)
            {
                pieceScore += 1;
            }

            if (blackBishopCount == 2)
            {
                pieceScore -= 1;
            }

            return pieceScore;
        }

        #region position values methods

        /// <summary>
        /// Points for having well positioned pieces 
        /// Note: Counts only position. No other factor
        /// </summary>
        /// <returns></returns>
        private decimal CalculatePositionValues()
        {
            decimal positionScores = 0;

            positionScores += CalculatePawnStructureScores();
            positionScores += CalculateCentralPieceScores();
                       
            return positionScores;
        }

        /// <summary>
        /// DoubledPawnPenalty
        /// PawnChainBonus
        /// </summary>
        /// <returns></returns>
        private decimal CalculatePawnStructureScores()
        {
            decimal pawnStructureScore = 0;
            
            //Doubled pawns
            var whiteDoubleCount = 0;
            var blackDoubleCount = 0;
            
            for (var i = 0; i < 8; i++)
			{
                var mask = LookupTables.FileMaskByColumn[i];
                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.WhitePawns) > 1)
                    whiteDoubleCount++;

                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.BlackPawns) > 1)
                    blackDoubleCount++;
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
            var hg = (m_CurrentBoard.BlackPawns >> 9) & notH;
            var bPawnAttackSquares = ((m_CurrentBoard.BlackPawns >> 9) & notH) | (m_CurrentBoard.BlackPawns >> 7) & notA;
            var bProtectedPawns = bPawnAttackSquares & m_CurrentBoard.BlackPawns;

            pawnStructureScore -= BitboardOperations.GetPopCount(bProtectedPawns) * PawnChainScore;

            return pawnStructureScore;
        }

        /// <summary>
        /// Points for placing pieces near to the centre
        /// </summary>
        /// <returns></returns>
        private decimal CalculateCentralPieceScores()
        {
            var centralSquares = UsefulBitboards.CentralSquares;
            var innerCentralSquares = UsefulBitboards.InnerCentralSquares;
            var outerCentralSquares = UsefulBitboards.OuterCentralSquares;
            
            decimal piecePositionScore = 0;

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
        
        private decimal CalculatePositionScores(ulong pieces, ulong positions)
        {
            var inPosition = pieces & positions;

            if (inPosition > 0)
                return BitboardOperations.GetPopCount(inPosition);

            return 0;
        }

        #endregion position values methods

        #region King safety scores

        private decimal CalculateKingSafetyScores()
        {
            decimal kingSafetyScore = 0;

            kingSafetyScore += CalculateKingCastlingScores();
            kingSafetyScore += CalculateCanCastleScores();

            //Add king pawn protection scores
            //Add king position scores (stay away from the middle early/mid game)

            return kingSafetyScore;
        }

        private decimal CalculateKingCastlingScores()
        {            
            decimal castlingScore = 0;

            if (!IsEndGame)
            {
                if (m_CurrentBoard.WhiteKing == LookupTables.G1 && (m_CurrentBoard.WhiteRooks & LookupTables.F1) != (ulong)0)
                    castlingScore += CastlingKingSideScore;
                else if (m_CurrentBoard.WhiteKing == LookupTables.C1 && (m_CurrentBoard.WhiteRooks & LookupTables.D1) != (ulong)0)
                    castlingScore += CastlingQueenSideScore;

                if (m_CurrentBoard.BlackKing == LookupTables.G8 && (m_CurrentBoard.BlackRooks & LookupTables.F8) != (ulong)0)
                    castlingScore -= CastlingKingSideScore;
                else if (m_CurrentBoard.BlackKing == LookupTables.C8 && (m_CurrentBoard.BlackRooks & LookupTables.D8) != (ulong)0)
                    castlingScore -= CastlingQueenSideScore;

            }
            return castlingScore;
        }

        private decimal CalculateCanCastleScores()
        {
            decimal canCastleScore = 0;

            var state = m_CurrentBoard.GetCurrentBoardState();

            canCastleScore += Convert.ToDecimal(state.WhiteCanCastleKingside) * CanCastleKingsideScore;
            canCastleScore += Convert.ToDecimal(state.WhiteCanCastleQueenside) * CanCastleQueensideScore;

            canCastleScore -= Convert.ToDecimal(state.BlackCanCastleKingside) * CanCastleKingsideScore;
            canCastleScore -= Convert.ToDecimal(state.BlackCanCastleQueenside) * CanCastleQueensideScore;

            return canCastleScore;
        }

        #endregion King safety scores

        #region square table scores

        private decimal CalculateSquareTableScores()
        {
            decimal squareTableScores = 0;

            squareTableScores += CalculatePawnSquareTableScores();
            squareTableScores += CalculateKnightSquareTableScores();
            squareTableScores += CalculateBishopSquareTableScores();
            
            if(!IsEndGame)
                squareTableScores += CalculateKingSquareTableScores();
            else
                squareTableScores += CalculateKingEndGameSquareTableScores();

            return squareTableScores;
        }
        
        private decimal CalculatePawnSquareTableScores()
        {
            decimal pawnSquareTableScore = 0;

            pawnSquareTableScore += CalculateTableScores(m_CurrentBoard.WhitePawns, PawnSquareTable, PieceColour.White);

            pawnSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackPawns, PawnSquareTable, PieceColour.Black);
            
            return pawnSquareTableScore;
        }

        private decimal CalculateKnightSquareTableScores()
        {
            decimal knightSquareTableScore = 0;

            knightSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKnights, KnightSquareTable, PieceColour.White);

            knightSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKnights, KnightSquareTable, PieceColour.Black);

            return knightSquareTableScore;
        }

        private decimal CalculateBishopSquareTableScores()
        {
            decimal bishopSquareTableScore = 0;

            bishopSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteBishops, BishopSquareTable, PieceColour.White);

            bishopSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackBishops, BishopSquareTable, PieceColour.Black);

            return bishopSquareTableScore;
        }

        private decimal CalculateKingSquareTableScores()
        {
            decimal kingSquareTableScore = 0;

            kingSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKing, KingSquareTable, PieceColour.White);

            kingSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKing, KingSquareTable, PieceColour.Black);

            return kingSquareTableScore;
        }

        private decimal CalculateKingEndGameSquareTableScores()
        {
            decimal kingEndGameSquareTableScore = 0;

            kingEndGameSquareTableScore += CalculateTableScores(m_CurrentBoard.WhiteKing, KingEndGameSquareTable, PieceColour.White);

            kingEndGameSquareTableScore -= CalculateTableScores(m_CurrentBoard.BlackKing, KingEndGameSquareTable, PieceColour.Black);

            return kingEndGameSquareTableScore;
        }

        private decimal CalculateTableScores(ulong board, decimal[] squareTableValues, PieceColour pieceColour)
        {
            decimal pieceScore = 0;

            List<byte> positions;
            
            if(pieceColour == PieceColour.White)
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
         
        private decimal CalculateDevelopmentBonus()
        {
            decimal developedPieceBonus = 0;

            developedPieceBonus += CalculateDevelopedPieceBonus();
            developedPieceBonus += CalculateConnectedRookBonus();

            return developedPieceBonus;
        }

        /// <summary>
        /// Points for all pieces not on the back rank (not including pawns)
        /// </summary>
        /// <returns></returns>
        private decimal CalculateDevelopedPieceBonus()
        {
            decimal developedPiecesScore = 0;

            ulong whiteBack = 255;
            var blackBack = 18374686479671623680;

            var whiteDevelopedPieces = (m_CurrentBoard.WhiteBishops ^ m_CurrentBoard.WhiteKnights ^ m_CurrentBoard.WhiteQueen) & ~whiteBack;
            developedPiecesScore += BitboardOperations.GetPopCount(whiteDevelopedPieces) * DevelopedPieceScore;

            var blackDevelopedPieces = (m_CurrentBoard.BlackBishops ^ m_CurrentBoard.BlackKnights ^ m_CurrentBoard.BlackQueen) & ~blackBack;
            developedPiecesScore -= BitboardOperations.GetPopCount(blackDevelopedPieces) * DevelopedPieceScore;

            return developedPiecesScore;
        }

        private decimal CalculateConnectedRookBonus()
        {
            decimal connectedRookScore = 0;

            if (BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) > 1)
            {
                var rooks = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhiteRooks);

                var firstRook = rooks[0];
                var secondRook = rooks[1];
                
                if((BoardChecking.FindRightBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore += ConnectedRookBonus;
                else if((BoardChecking.FindLeftBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore += ConnectedRookBonus;
                 else if((BoardChecking.FindUpBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore += ConnectedRookBonus;
                 else if((BoardChecking.FindDownBlockingPosition(m_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore += ConnectedRookBonus;
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
        private decimal CalculateCoverageAndAttackScores()
        {
            decimal attackScore = 0;

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
