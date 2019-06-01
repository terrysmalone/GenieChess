﻿using System;
using System.Collections.Generic;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.PossibleMoves;
using ChessGame.BoardSearching;
using ChessGame.Debugging;

namespace ChessGame.ScoreCalculation
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
        public decimal CalculateScore(Board currentBoard)
        {
            CountDebugger.Evaluations++;

            this.m_CurrentBoard = currentBoard;

            //StaticBoardChecks.Calculate(currentBoard);

            DetectEndGame();

            decimal score = 0;

            score += CalculatePieceValues();
                       
            score += CalculatePositionValues();
            score += CalculateKingSafetyScores();

            score += CalculateDevelopmentBonus();

            score += CalculateSquareTableScores();

            score += CalculateAttackScores();

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
            decimal kingScore = (decimal.MaxValue/6);

            decimal pieceScore = 0;

            //Calculate white piece values
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhitePawns) * PawnPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKnights) * KnightPieceValue;

            byte whiteBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.WhiteBishops);

            pieceScore += whiteBishopCount * BishopPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) * RookPieceValue;
            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteQueen) * QueenPieceValue;

            pieceScore += BitboardOperations.GetPopCount(m_CurrentBoard.WhiteKing) * kingScore;

            //Calculate black piece values
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackPawns) * PawnPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKnights) * KnightPieceValue;

            byte blackBishopCount = BitboardOperations.GetPopCount(m_CurrentBoard.BlackBishops);

            pieceScore -= blackBishopCount * BishopPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackRooks) * RookPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackQueen) * QueenPieceValue;

            pieceScore -= BitboardOperations.GetPopCount(m_CurrentBoard.BlackKing) * kingScore;

            //Calculate double bishop bonus
            pieceScore += whiteBishopCount * DoubleBishopScore;
            pieceScore -= blackBishopCount * DoubleBishopScore;


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
            int whiteDoubleCount = 0;
            int blackDoubleCount = 0;
            
            for (int i = 0; i < 8; i++)
			{
                ulong mask = LookupTables.FileMaskByColumn[i];
                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.WhitePawns) > 1)
                    whiteDoubleCount++;

                if (BitboardOperations.GetPopCount(mask & m_CurrentBoard.BlackPawns) > 1)
                    blackDoubleCount++;
			}

            pawnStructureScore += whiteDoubleCount * DoubledPawnPenalty;
            pawnStructureScore -= blackDoubleCount * DoubledPawnPenalty;
            
            //Pawn chain 
            ulong notA = 18374403900871474942;
            ulong notH = 9187201950435737471;

            //White pawns 
            ulong wPawnAttackSquares = ((m_CurrentBoard.WhitePawns << 9) & notA) | (m_CurrentBoard.WhitePawns << 7) & notH;
            ulong wProtectedPawns = wPawnAttackSquares & m_CurrentBoard.WhitePawns;

            pawnStructureScore += BitboardOperations.GetPopCount(wProtectedPawns) * PawnChainScore;

            //Black pawns 
            ulong hg = (m_CurrentBoard.BlackPawns >> 9) & notH;
            ulong bPawnAttackSquares = ((m_CurrentBoard.BlackPawns >> 9) & notH) | (m_CurrentBoard.BlackPawns >> 7) & notA;
            ulong bProtectedPawns = bPawnAttackSquares & m_CurrentBoard.BlackPawns;

            pawnStructureScore -= BitboardOperations.GetPopCount(bProtectedPawns) * PawnChainScore;

            return pawnStructureScore;
        }

        /// <summary>
        /// Points for placing pieces near to the centre
        /// </summary>
        /// <returns></returns>
        private decimal CalculateCentralPieceScores()
        {
            ulong centralSquares = UsefulBitboards.CentralSquares;
            ulong innerCentralSquares = UsefulBitboards.InnerCentralSquares;
            ulong outerCentralSquares = UsefulBitboards.OuterCentralSquares;
            
            decimal piecePositionScore = 0;

            //Pawns
            ulong whitePawnBoard = m_CurrentBoard.WhitePawns;

            piecePositionScore += CalculatePositionScores(whitePawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore += CalculatePositionScores(whitePawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            ulong blackPawnBoard = m_CurrentBoard.BlackPawns;

            piecePositionScore -= CalculatePositionScores(blackPawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore -= CalculatePositionScores(blackPawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            //Knights
            ulong whiteKnightBoard = m_CurrentBoard.WhiteKnights;

            piecePositionScore += CalculatePositionScores(whiteKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore += CalculatePositionScores(whiteKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            ulong blackKnightBoard = m_CurrentBoard.BlackKnights;

            piecePositionScore -= CalculatePositionScores(blackKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore -= CalculatePositionScores(blackKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            //Bishops
            ulong whiteBishopBoard = m_CurrentBoard.WhiteBishops;

            piecePositionScore += CalculatePositionScores(whiteBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore += CalculatePositionScores(whiteBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            ulong blackBishopBoard = m_CurrentBoard.BlackBishops;

            piecePositionScore -= CalculatePositionScores(blackBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore -= CalculatePositionScores(blackBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            //Rooks
            ulong whiteRookBoard = m_CurrentBoard.WhiteRooks;

            piecePositionScore += CalculatePositionScores(whiteRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore += CalculatePositionScores(whiteRookBoard, outerCentralSquares) * OuterCentralRookScore;

            ulong blackRookBoard = m_CurrentBoard.BlackRooks;

            piecePositionScore -= CalculatePositionScores(blackRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore -= CalculatePositionScores(blackRookBoard, outerCentralSquares) * OuterCentralRookScore;

            //Queens
            ulong whiteQueenBoard = m_CurrentBoard.WhiteQueen;

            piecePositionScore += CalculatePositionScores(whiteQueenBoard, innerCentralSquares) * InnerCentralQueenScore;
            piecePositionScore += CalculatePositionScores(whiteQueenBoard, outerCentralSquares) * OuterCentralQueenScore;

            ulong blackQueenBoard = m_CurrentBoard.BlackQueen;

            piecePositionScore -= CalculatePositionScores(blackQueenBoard, innerCentralSquares) * InnerCentralQueenScore;
            piecePositionScore -= CalculatePositionScores(blackQueenBoard, outerCentralSquares) * OuterCentralQueenScore;
            
            return piecePositionScore;
        }
        
        private decimal CalculatePositionScores(ulong pieces, ulong positions)
        {
            ulong inPosition = pieces & positions;

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

            BoardState state = m_CurrentBoard.GetCurrentBoardState();

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


            foreach (byte squareIndex in positions)
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
            ulong blackBack = 18374686479671623680;

            ulong whiteDevelopedPieces = (m_CurrentBoard.WhiteBishops ^ m_CurrentBoard.WhiteKnights ^ m_CurrentBoard.WhiteQueen) & ~whiteBack;
            developedPiecesScore += BitboardOperations.GetPopCount(whiteDevelopedPieces) * DevelopedPieceScore;

            ulong blackDevelopedPieces = (m_CurrentBoard.BlackBishops ^ m_CurrentBoard.BlackKnights ^ m_CurrentBoard.BlackQueen) & ~blackBack;
            developedPiecesScore -= BitboardOperations.GetPopCount(blackDevelopedPieces) * DevelopedPieceScore;

            return developedPiecesScore;
        }

        private decimal CalculateConnectedRookBonus()
        {
            decimal connectedRookScore = 0;

            if (BitboardOperations.GetPopCount(m_CurrentBoard.WhiteRooks) > 1)
            {
                ulong[] rooks = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhiteRooks);

                ulong firstRook = rooks[0];
                ulong secondRook = rooks[1];
                
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
                ulong[] rooks = BitboardOperations.SplitBoardToArray(m_CurrentBoard.BlackRooks);

                ulong firstRook = rooks[0];
                ulong secondRook = rooks[1];

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

        #region Attack scores

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
        private decimal CalculateAttackScores()
        {
            decimal attackScore = 0;

            
            //White
            ulong whiteBishopCoverage = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, m_CurrentBoard.WhiteBishops, PieceColour.White);

            if (whiteBishopCoverage > 0)
            {
                attackScore += BitboardOperations.GetPopCount(whiteBishopCoverage) * BishopCoverageBonus;

                ulong attackBoard = whiteBishopCoverage & m_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
            }

            ulong whiteRookCoverage = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, m_CurrentBoard.WhiteRooks, PieceColour.White);

            if (whiteRookCoverage > 0)
            {
                attackScore += BitboardOperations.GetPopCount(whiteRookCoverage) * RookCoverageBonus;

                ulong attackBoard = whiteRookCoverage & m_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
            }

            ulong whiteQueenCoverage = BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, m_CurrentBoard.WhiteQueen, PieceColour.White);
            if (whiteQueenCoverage > 0)
            {
                attackScore += BitboardOperations.GetPopCount(whiteQueenCoverage) * QueenCoverageBonus;

                ulong attackBoard = whiteQueenCoverage & m_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
            }

            //Knights
            ulong whiteKnightCoverage = 0;
             List<byte> whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteKnights);

             foreach (byte knightPos in whiteKnightPositions)
	         {
                 ulong possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                 possibleMoves = possibleMoves & ~m_CurrentBoard.AllWhiteOccupiedSquares;
                 whiteKnightCoverage |= possibleMoves;
	         }

             ulong wKnightAttackBoard = whiteKnightCoverage & m_CurrentBoard.AllBlackOccupiedSquares;
             attackScore += BitboardOperations.GetPopCount(wKnightAttackBoard) * AttackBonus;

             ulong boardCoverage = whiteBishopCoverage | whiteRookCoverage | whiteQueenCoverage | whiteKnightCoverage;

             attackScore += BitboardOperations.GetPopCount(boardCoverage) * BoardCoverageBonus;

             //Black
             ulong blackBishopCoverage = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, m_CurrentBoard.BlackBishops, PieceColour.Black);
             if (blackBishopCoverage > 0)
             {
                 attackScore -= BitboardOperations.GetPopCount(blackBishopCoverage) * BishopCoverageBonus;

                 ulong attackBoard = blackBishopCoverage & m_CurrentBoard.AllWhiteOccupiedSquares;
                 attackScore -= BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
             }

             ulong blackRookCoverage = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, m_CurrentBoard.BlackRooks, PieceColour.Black);
             if (blackRookCoverage > 0)
             { 
                 attackScore -= BitboardOperations.GetPopCount(blackRookCoverage) * RookCoverageBonus;

                 ulong attackBoard = blackRookCoverage & m_CurrentBoard.AllWhiteOccupiedSquares;
                 attackScore -= BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
             }

             ulong blackQueenCoverage = BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, m_CurrentBoard.BlackQueen, PieceColour.Black);
             if (blackQueenCoverage > 0)
             { 
                 attackScore -= BitboardOperations.GetPopCount(blackQueenCoverage) * QueenCoverageBonus;

                 ulong attackBoard = blackQueenCoverage & m_CurrentBoard.AllWhiteOccupiedSquares;
                 attackScore -= BitboardOperations.GetPopCount(attackBoard) * AttackBonus;
             }

             //Knights
             ulong blackKnightCoverage = 0;
             List<byte> blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackKnights);

             foreach (byte knightPos in blackKnightPositions)
             {
                 ulong possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                 possibleMoves = possibleMoves & ~m_CurrentBoard.AllBlackOccupiedSquares;
                 blackKnightCoverage |= possibleMoves;
             }

             ulong bKnightAttackBoard = blackKnightCoverage & m_CurrentBoard.AllWhiteOccupiedSquares;
             attackScore -= BitboardOperations.GetPopCount(bKnightAttackBoard) * AttackBonus;

             ulong blackBoardCoverage = blackBishopCoverage | blackRookCoverage | blackQueenCoverage | blackKnightCoverage;

             attackScore -= BitboardOperations.GetPopCount(blackBoardCoverage) * BoardCoverageBonus;
            
#warning need to add pawn attacks  to attack scores
             return attackScore;
        }

        #endregion Attack scores
    }
}
