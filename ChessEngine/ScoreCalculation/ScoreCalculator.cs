using System;
using System.Linq;
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
        private Board _CurrentBoard;

        private readonly byte _endGameCount = 3;

        private bool _isEndGame = false;

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

            _CurrentBoard = currentBoard;

            //StaticBoardChecks.Calculate(currentBoard);

            DetectEndGame();

            var score = 0;

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
            
            if(BitboardOperations.GetPopCount(_CurrentBoard.WhiteNonEndGamePieces) <= _endGameCount
               &&
               BitboardOperations.GetPopCount(_CurrentBoard.BlackNonEndGamePieces) <= _endGameCount)
            {
                _isEndGame = true;
            }
            else
            {
                _isEndGame = false;
            }

        }

        /// <summary>
        /// Calculates points for the pieces each player has on the board
        /// </summary>
        /// <returns></returns>
        private int CalculatePieceValues()
        {
            var kingScore = 357913941;  // int.MaxValue / 6

            var pieceScore = 0;

            //Calculate white piece values
            pieceScore += BitboardOperations.GetPopCount(_CurrentBoard.WhitePawns) * PawnPieceValue;
            pieceScore += BitboardOperations.GetPopCount(_CurrentBoard.WhiteKnights) * KnightPieceValue;

            var whiteBishopCount = BitboardOperations.GetPopCount(_CurrentBoard.WhiteBishops);
            pieceScore += whiteBishopCount * BishopPieceValue;

            if (whiteBishopCount == 2)
            {
                pieceScore += DoubleBishopScore;
            }

            pieceScore += BitboardOperations.GetPopCount(_CurrentBoard.WhiteRooks) * RookPieceValue;

            var whiteQueenCount = BitboardOperations.GetPopCount(_CurrentBoard.WhiteQueen);

            pieceScore += whiteQueenCount * QueenPieceValue;
            pieceScore += whiteQueenCount * SoloQueenScore;

            pieceScore += BitboardOperations.GetPopCount(_CurrentBoard.WhiteKing) * kingScore;

            //Calculate black piece values
            pieceScore -= BitboardOperations.GetPopCount(_CurrentBoard.BlackPawns) * PawnPieceValue;
            pieceScore -= BitboardOperations.GetPopCount(_CurrentBoard.BlackKnights) * KnightPieceValue;

            var blackBishopCount = BitboardOperations.GetPopCount(_CurrentBoard.BlackBishops);
            pieceScore -= blackBishopCount * BishopPieceValue;

            if (blackBishopCount == 2)
            {
                pieceScore -= DoubleBishopScore;
            }

            pieceScore -= BitboardOperations.GetPopCount(_CurrentBoard.BlackRooks) * RookPieceValue;

            var blackQueenCount = BitboardOperations.GetPopCount(_CurrentBoard.BlackQueen);

            pieceScore -= blackQueenCount * QueenPieceValue;
            pieceScore -= blackQueenCount * SoloQueenScore;

            pieceScore -= BitboardOperations.GetPopCount(_CurrentBoard.BlackKing) * kingScore;
            
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
            var positionScores = 0;

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
            var pawnStructureScore = 0;
            
            //Doubled pawns
            var whiteDoubleCount = 0;
            var blackDoubleCount = 0;
            
            for (var i = 0; i < 8; i++)
			{
                var mask = LookupTables.ColumnMaskByColumn[i];

                if (BitboardOperations.GetPopCount(mask & _CurrentBoard.WhitePawns) > 1)
                {
                    whiteDoubleCount++;
                }

                if (BitboardOperations.GetPopCount(mask & _CurrentBoard.BlackPawns) > 1)
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
            var wPawnAttackSquares = ((_CurrentBoard.WhitePawns << 9) & notA) | (_CurrentBoard.WhitePawns << 7) & notH;
            var wProtectedPawns = wPawnAttackSquares & _CurrentBoard.WhitePawns;

            pawnStructureScore += BitboardOperations.GetPopCount(wProtectedPawns) * PawnChainScore;

            //Black pawns
            var bPawnAttackSquares = ((_CurrentBoard.BlackPawns >> 9) & notH) | (_CurrentBoard.BlackPawns >> 7) & notA;
            var bProtectedPawns = bPawnAttackSquares & _CurrentBoard.BlackPawns;

            pawnStructureScore -= BitboardOperations.GetPopCount(bProtectedPawns) * PawnChainScore;

            // Passed pawn bonus

            foreach (var whitePawnBoard in BitboardOperations.SplitBoardToArray(_CurrentBoard.WhitePawns))
            {
                //Pawn is on 7th rank so it can promote
                if ((whitePawnBoard & LookupTables.RowMask7) != 0)
                {
                    pawnStructureScore += PassedPawnBonus;
                    pawnStructureScore += PassedPawnAdvancementBonus * 5;

                    continue;
                }

                var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(whitePawnBoard);

                var pawnFrontSpan = LookupTables.WhitePawnFrontSpan[pawnIndex];

                if ((pawnFrontSpan & _CurrentBoard.BlackPawns) == 0)
                {
                    pawnStructureScore += PassedPawnBonus;
                    pawnStructureScore += PassedPawnAdvancementBonus * ((pawnIndex / 8) - 1);
                }
            }

            foreach (var blackPawnBoard in BitboardOperations.SplitBoardToArray(_CurrentBoard.BlackPawns))
            {
                //Pawn is on 2nd rank so it can promote
                if ((blackPawnBoard & LookupTables.RowMask2) != 0)
                {
                    pawnStructureScore -= PassedPawnBonus;
                    pawnStructureScore -= PassedPawnAdvancementBonus * 5;
                    continue;
                }

                var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(blackPawnBoard);

                var pawnFrontSpan = LookupTables.BlackPawnFrontSpan[pawnIndex];

                if ((pawnFrontSpan & _CurrentBoard.WhitePawns) == 0)
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
            var innerCentralSquares = (ulong)103481868288;
            var outerCentralSquares = (ulong)66125924401152;
            
            var piecePositionScore = 0;

            //Pawns
            var whitePawnBoard = _CurrentBoard.WhitePawns;

            piecePositionScore += CalculatePositionScores(whitePawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore += CalculatePositionScores(whitePawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            var blackPawnBoard = _CurrentBoard.BlackPawns;

            piecePositionScore -= CalculatePositionScores(blackPawnBoard, innerCentralSquares) * InnerCentralPawnScore;
            piecePositionScore -= CalculatePositionScores(blackPawnBoard, outerCentralSquares) * OuterCentralPawnScore;

            //Knights
            var whiteKnightBoard = _CurrentBoard.WhiteKnights;

            piecePositionScore += CalculatePositionScores(whiteKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore += CalculatePositionScores(whiteKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            var blackKnightBoard = _CurrentBoard.BlackKnights;

            piecePositionScore -= CalculatePositionScores(blackKnightBoard, innerCentralSquares) * InnerCentralKnightScore;
            piecePositionScore -= CalculatePositionScores(blackKnightBoard, outerCentralSquares) * OuterCentralKnightScore;

            //Bishops
            var whiteBishopBoard = _CurrentBoard.WhiteBishops;

            piecePositionScore += CalculatePositionScores(whiteBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore += CalculatePositionScores(whiteBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            var blackBishopBoard = _CurrentBoard.BlackBishops;

            piecePositionScore -= CalculatePositionScores(blackBishopBoard, innerCentralSquares) * InnerCentralBishopScore;
            piecePositionScore -= CalculatePositionScores(blackBishopBoard, outerCentralSquares) * OuterCentralBishopScore;

            //Rooks
            var whiteRookBoard = _CurrentBoard.WhiteRooks;

            piecePositionScore += CalculatePositionScores(whiteRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore += CalculatePositionScores(whiteRookBoard, outerCentralSquares) * OuterCentralRookScore;

            var blackRookBoard = _CurrentBoard.BlackRooks;

            piecePositionScore -= CalculatePositionScores(blackRookBoard, innerCentralSquares) * InnerCentralRookScore;
            piecePositionScore -= CalculatePositionScores(blackRookBoard, outerCentralSquares) * OuterCentralRookScore;

            //Queens
            var whiteQueenBoard = _CurrentBoard.WhiteQueen;

            piecePositionScore += CalculatePositionScores(whiteQueenBoard, innerCentralSquares) * InnerCentralQueenScore;
            piecePositionScore += CalculatePositionScores(whiteQueenBoard, outerCentralSquares) * OuterCentralQueenScore;

            var blackQueenBoard = _CurrentBoard.BlackQueen;

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
            var kingSafetyScore = 0;

            kingSafetyScore += CalculateKingCastlingScores();
            kingSafetyScore += CalculateCanCastleScores();

            //Add king pawn protection scores
            //Add king position scores (stay away from the middle early/mid game)

            return kingSafetyScore;
        }

        private int CalculateKingCastlingScores()
        {   
            if (_isEndGame)
            {
                return 0;
            }
                   
            var castlingScore = 0;

            if (_CurrentBoard.WhiteKing == LookupTables.G1 && (_CurrentBoard.WhiteRooks & LookupTables.F1) != 0)
            {
                castlingScore += CastlingKingSideScore;
            }
            else if (_CurrentBoard.WhiteKing == LookupTables.C1 && (_CurrentBoard.WhiteRooks & LookupTables.D1) != 0)
            {
                castlingScore += CastlingQueenSideScore;
            }

            if (_CurrentBoard.BlackKing == LookupTables.G8 && (_CurrentBoard.BlackRooks & LookupTables.F8) != 0)
            {
                castlingScore -= CastlingKingSideScore;
            }
            else if (_CurrentBoard.BlackKing == LookupTables.C8 && (_CurrentBoard.BlackRooks & LookupTables.D8) != 0)
            {
                castlingScore -= CastlingQueenSideScore;
            }
            
            return castlingScore;
        }

        private int CalculateCanCastleScores()
        {
            var canCastleScore = 0;

            var state = _CurrentBoard.GetCurrentBoardState();

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
            var squareTableScores = 0;

            squareTableScores += CalculatePawnSquareTableScores();
            squareTableScores += CalculateKnightSquareTableScores();
            squareTableScores += CalculateBishopSquareTableScores();
            
            if(!_isEndGame)
                squareTableScores += CalculateKingSquareTableScores();
            else
                squareTableScores += CalculateKingEndGameSquareTableScores();

            return squareTableScores;
        }
        
        private int CalculatePawnSquareTableScores()
        {
            var pawnSquareTableScore = 0;

            pawnSquareTableScore += CalculateTableScores(_CurrentBoard.WhitePawns, PawnSquareTable, true);

            pawnSquareTableScore -= CalculateTableScores(_CurrentBoard.BlackPawns, PawnSquareTable, false);
            
            return pawnSquareTableScore;
        }

        private int CalculateKnightSquareTableScores()
        {
            var knightSquareTableScore = 0;

            knightSquareTableScore += CalculateTableScores(_CurrentBoard.WhiteKnights, KnightSquareTable, true);

            knightSquareTableScore -= CalculateTableScores(_CurrentBoard.BlackKnights, KnightSquareTable, false);

            return knightSquareTableScore;
        }

        private int CalculateBishopSquareTableScores()
        {
            var bishopSquareTableScore = 0;

            bishopSquareTableScore += CalculateTableScores(_CurrentBoard.WhiteBishops, BishopSquareTable, true);

            bishopSquareTableScore -= CalculateTableScores(_CurrentBoard.BlackBishops, BishopSquareTable, false);

            return bishopSquareTableScore;
        }

        private int CalculateKingSquareTableScores()
        {
            var kingSquareTableScore = 0;

            kingSquareTableScore += CalculateTableScores(_CurrentBoard.WhiteKing, KingSquareTable, true);

            kingSquareTableScore -= CalculateTableScores(_CurrentBoard.BlackKing, KingSquareTable, false);

            return kingSquareTableScore;
        }

        private int CalculateKingEndGameSquareTableScores()
        {
            var kingEndGameSquareTableScore = 0;

            kingEndGameSquareTableScore += CalculateTableScores(_CurrentBoard.WhiteKing, KingEndGameSquareTable, true);

            kingEndGameSquareTableScore -= CalculateTableScores(_CurrentBoard.BlackKing, KingEndGameSquareTable, false);

            return kingEndGameSquareTableScore;
        }

        private static int CalculateTableScores(ulong board, int[] squareTableValues, bool isWhite)
        {
            var positions = 
                BitboardOperations.GetSquareIndexesFromBoardValue(isWhite ? board : BitboardOperations.FlipVertical(board));

            return positions.Sum(squareIndex => squareTableValues[squareIndex]);
        }

        #endregion square table scores

        #region development score
         
        private int CalculateDevelopmentBonus()
        {
            var developedPieceBonus = 0;

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

            var whiteDevelopedPieces = (_CurrentBoard.WhiteBishops ^ _CurrentBoard.WhiteKnights ^ _CurrentBoard.WhiteQueen) & ~whiteBack;
            var developedWhitePieceCount = BitboardOperations.GetPopCount(whiteDevelopedPieces);
            developedPiecesScore += developedWhitePieceCount * DevelopedPieceScore;
            
            var blackDevelopedPieces = (_CurrentBoard.BlackBishops ^ _CurrentBoard.BlackKnights ^ _CurrentBoard.BlackQueen) & ~blackBack;
            var developedBlackPieceCount = BitboardOperations.GetPopCount(blackDevelopedPieces);
            developedPiecesScore -= developedBlackPieceCount * DevelopedPieceScore;

            return developedPiecesScore;
        }

        private int CalculateConnectedRookBonus()
        {
            var connectedRookScore = 0;

            if (BitboardOperations.GetPopCount(_CurrentBoard.WhiteRooks) > 1)
            {
                var rooks = BitboardOperations.SplitBoardToArray(_CurrentBoard.WhiteRooks);

                var firstRook = rooks[0];
                var secondRook = rooks[1];

                if ((BoardChecking.FindRightBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindLeftBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindUpBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
                else if ((BoardChecking.FindDownBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                {
                    connectedRookScore += ConnectedRookBonus;
                }
            }

            if (BitboardOperations.GetPopCount(_CurrentBoard.BlackRooks) > 1)
            {
                var rooks = BitboardOperations.SplitBoardToArray(_CurrentBoard.BlackRooks);

                var firstRook = rooks[0];
                var secondRook = rooks[1];

                if ((BoardChecking.FindRightBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindLeftBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindUpBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
                else if ((BoardChecking.FindDownBlockingPosition(_CurrentBoard, firstRook) & secondRook) > 0)
                    connectedRookScore -= ConnectedRookBonus;
            }

            return connectedRookScore;
        }

        private int CalculateEarlyQueenPenalty()
        {
            var earlyQueenPenaltyScore = 0; 

            ulong whiteUndevelopedPieceBoard = 0;

            whiteUndevelopedPieceBoard |= _CurrentBoard.WhiteBishops & 36;  //Any white bishops on C1 or F1
            whiteUndevelopedPieceBoard |= _CurrentBoard.WhiteKnights & 66;  //Any white knights on B1 orG1

            var whiteUndevelopedPieceCount = BitboardOperations.GetPopCount(whiteUndevelopedPieceBoard);

            // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
            if (whiteUndevelopedPieceCount >= 2 && (_CurrentBoard.WhiteQueen & ~8UL) != 0)
            {
                earlyQueenPenaltyScore -= EarlyQueenMovePenalty;
            }

            ulong blackUndevelopedPieceBoard = 0;

            //Any black bishops on C8 or F8
            blackUndevelopedPieceBoard |= _CurrentBoard.BlackBishops & 2594073385365405696;
            //Any black knights on B8 or G8
            blackUndevelopedPieceBoard |= _CurrentBoard.BlackKnights & 66;  

            int blackUndevelopedPieceCount = BitboardOperations.GetPopCount(blackUndevelopedPieceBoard);

            // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
            if (blackUndevelopedPieceCount >= 2 && (_CurrentBoard.BlackQueen & ~576460752303423488UL) != 0)
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
            var attackScore = 0;

            ulong whiteCoverageBoard = 0;   // All the empty squares white can move to next turn

            ulong whiteAttackBoard = 0;     // All squares with black pieces white can move to next turn

            // White
            // Bonus for bishop 
            var whiteBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(_CurrentBoard, _CurrentBoard.WhiteBishops, true);                                       ;
            
            if (whiteBishopPossibleMoves > 0)
            {
                var whiteBishopCoverageBoard = whiteBishopPossibleMoves & ~_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(whiteBishopCoverageBoard) 
                               * BishopCoverageBonus; // Add points for white bishop coverage
                
                whiteCoverageBoard |= whiteBishopCoverageBoard;

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whiteBishopPossibleMoves 
                    & (_CurrentBoard.BlackQueen | _CurrentBoard.BlackRooks)) 
                    * MoreValuablePieceAttackBonus;
                
                whiteAttackBoard |= whiteBishopPossibleMoves & _CurrentBoard.AllBlackOccupiedSquares;             
            }

            // Bonus for rook coverage
            var whiteRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(_CurrentBoard, 
                                                                                 _CurrentBoard.WhiteRooks,
                                                                                 true);

            if (whiteRookPossibleMoves > 0)
            {
                var whiteRookCoverageBoard = 
                    whiteRookPossibleMoves 
                    & ~_CurrentBoard.AllBlackOccupiedSquares;

                attackScore += BitboardOperations.GetPopCount(whiteRookCoverageBoard) * RookCoverageBonus;

                whiteCoverageBoard |= whiteRookCoverageBoard;

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whiteRookPossibleMoves & _CurrentBoard.BlackQueen) * MoreValuablePieceAttackBonus;

                whiteAttackBoard |= whiteRookPossibleMoves & _CurrentBoard.AllBlackOccupiedSquares;               
            }

            // Bonus for queen coverage
            var whiteQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(_CurrentBoard, 
                                                                                   _CurrentBoard.WhiteQueen,
                                                                                   true);
            if (whiteQueenPossibleMoves > 0)
            {
                var whiteQueenCoverageBoard = whiteQueenPossibleMoves & ~_CurrentBoard.AllBlackOccupiedSquares;
                attackScore += BitboardOperations.GetPopCount(whiteQueenCoverageBoard) * QueenCoverageBonus;

                whiteCoverageBoard |= whiteQueenCoverageBoard;

                whiteAttackBoard |= whiteQueenPossibleMoves & _CurrentBoard.AllBlackOccupiedSquares;
            }

            // Bonus for knight coverage
            ulong whiteKnightPossibleMoves = 0;

            var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(_CurrentBoard.WhiteKnights);

            foreach (var knightPos in whiteKnightPositions)
	        {
                var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                possibleMoves = possibleMoves & ~_CurrentBoard.AllWhiteOccupiedSquares;
                whiteKnightPossibleMoves |= possibleMoves;
	        }

            whiteCoverageBoard |= whiteKnightPossibleMoves & ~_CurrentBoard.AllBlackOccupiedSquares;

            whiteAttackBoard |= whiteKnightPossibleMoves & _CurrentBoard.AllBlackOccupiedSquares;

            // Points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteKnightPossibleMoves & (_CurrentBoard.BlackQueen | _CurrentBoard.BlackRooks)) * MoreValuablePieceAttackBonus;

            //Pawns
            const ulong notA = 18374403900871474942;
            const ulong notH = 9187201950435737471;

            var whitePawnPossibleAttackMoves = ((_CurrentBoard.WhitePawns << 9) & notA) 
                                               | ((_CurrentBoard.WhitePawns << 7) & notH);

            if (whitePawnPossibleAttackMoves > 0)
            {
                // There is no bonus for pawn coverage, just pawn attacks

                // Add points for every attack on a more valuable piece
                attackScore += BitboardOperations.GetPopCount(
                    whitePawnPossibleAttackMoves 
                        & (_CurrentBoard.BlackQueen 
                           | _CurrentBoard.BlackRooks 
                           | _CurrentBoard.BlackBishops 
                           | _CurrentBoard.BlackKnights)) 
                * MoreValuablePieceAttackBonus;

                whiteAttackBoard |= whitePawnPossibleAttackMoves & _CurrentBoard.AllBlackOccupiedSquares;
            }

            // Points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(
                whiteKnightPossibleMoves 
                & (_CurrentBoard.BlackQueen | _CurrentBoard.BlackRooks)) 
                * MoreValuablePieceAttackBonus;

            // Points for overall board coverage
            attackScore += BitboardOperations.GetPopCount(whiteCoverageBoard) * BoardCoverageBonus;

            // Points for board attacks
            attackScore += BitboardOperations.GetPopCount(whiteAttackBoard) * AttackBonus;

            //Black
            ulong blackCoverageBoard = 0;   // All the empty squares black can move to next turn
            ulong blackAttackBoard = 0;    // All white squares black can move to next turn

            // Bonus for bishop coverage
            var blackBishopPossibleMoves = BoardChecking.CalculateAllowedBishopMoves(_CurrentBoard, 
                                                                                     _CurrentBoard.BlackBishops,
                                                                                     false);

            if (blackBishopPossibleMoves > 0)
            {
                var blackBishopCoverageBoard = blackBishopPossibleMoves & ~_CurrentBoard.AllWhiteOccupiedSquares;

                // Add points for black bishop coverage
                attackScore -= BitboardOperations.GetPopCount(blackBishopCoverageBoard) 
                               * BishopCoverageBonus; 

                blackCoverageBoard |= blackBishopCoverageBoard;

                // Points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackBishopPossibleMoves & (_CurrentBoard.WhiteQueen | _CurrentBoard.WhiteRooks))
                    * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackBishopPossibleMoves & _CurrentBoard.AllWhiteOccupiedSquares;
            }

            // Bonus for rook coverage
            var blackRookPossibleMoves = BoardChecking.CalculateAllowedRookMoves(_CurrentBoard, 
                                                                                 _CurrentBoard.BlackRooks, 
                                                                                 false);

            if (blackRookPossibleMoves > 0)
            { 
                var blackRookCoverageBoard = blackRookPossibleMoves & ~_CurrentBoard.AllWhiteOccupiedSquares;

                // Add points for black rook coverage
                attackScore -= BitboardOperations.GetPopCount(blackRookCoverageBoard) * RookCoverageBonus; 

                blackCoverageBoard |= blackRookCoverageBoard;

                // Points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackRookPossibleMoves & _CurrentBoard.WhiteQueen) * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackRookPossibleMoves & _CurrentBoard.AllWhiteOccupiedSquares;
            }

            // Bonus for queen coverage
            var blackQueenPossibleMoves = BoardChecking.CalculateAllowedQueenMoves(_CurrentBoard, 
                                                                                   _CurrentBoard.BlackQueen,
                                                                                   false);

            if (blackQueenPossibleMoves > 0)
            { 
                var blackQueenCoverageBoard = blackQueenPossibleMoves & ~_CurrentBoard.AllWhiteOccupiedSquares;
                attackScore -= BitboardOperations.GetPopCount(blackQueenCoverageBoard) * QueenCoverageBonus;

                blackCoverageBoard |= blackQueenCoverageBoard;

                blackAttackBoard |= blackQueenPossibleMoves & _CurrentBoard.AllWhiteOccupiedSquares;
            }

            //// Bonus for knight coverage
            ulong blackKnightPossibleMoves = 0;

            var blackKnightPositions = 
                BitboardOperations.GetSquareIndexesFromBoardValue(_CurrentBoard.BlackKnights);

            foreach (var knightPos in blackKnightPositions)
            {
                var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

                possibleMoves = possibleMoves & ~_CurrentBoard.AllBlackOccupiedSquares;
                blackKnightPossibleMoves |= possibleMoves;
            }

            blackCoverageBoard |= blackKnightPossibleMoves & ~_CurrentBoard.AllWhiteOccupiedSquares;

            blackAttackBoard |= blackKnightPossibleMoves & _CurrentBoard.AllWhiteOccupiedSquares;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(
                blackKnightPossibleMoves & (_CurrentBoard.WhiteQueen | _CurrentBoard.WhiteRooks)) 
                * MoreValuablePieceAttackBonus;

            //Pawns
            var blackPawnPossibleAttackMoves = ((_CurrentBoard.BlackPawns >> 7) & notA) 
                                               | ((_CurrentBoard.BlackPawns >> 7) & notH);

            if (blackPawnPossibleAttackMoves > 0)
            {
                // There is no bonus for pawn coverage, just pawn attacks
                // Add points for every attack on a more valuable piece
                attackScore -= BitboardOperations.GetPopCount(
                    blackPawnPossibleAttackMoves & (_CurrentBoard.WhiteQueen 
                                                    | _CurrentBoard.WhiteRooks 
                                                    | _CurrentBoard.WhiteBishops 
                                                    | _CurrentBoard.WhiteKnights))
                    * MoreValuablePieceAttackBonus;

                blackAttackBoard |= blackPawnPossibleAttackMoves & _CurrentBoard.AllWhiteOccupiedSquares;
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
