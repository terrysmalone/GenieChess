using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;

namespace ChessEngine.PossibleMoves
{
    /// <summary>
    /// Calculates all possible moves from a given position
    /// </summary>
    public static class MoveGeneration
    {
        private static IBoard m_CurrentBoard;

        private static List<PieceMoves> _allMovesList;

        private static PieceColour m_FriendlyColour = PieceColour.White; //These are swapped at the end of every turn
       
        private static int _checkCount;     //Used to find if king is in double check
        
        #region Calculate moves methods
        
        /// <summary>
        /// Returns all truly legal moves
        /// </summary>
        /// <returns></returns>
        public static List<PieceMoves> CalculateAllMoves(IBoard board)
        {
            _allMovesList = new List<PieceMoves>();

            _checkCount = 0;

            PieceValidMoves.GenerateMoveArrays();

            CalculateAllPseudoLegalMoves(board);
           
            RemoveCheckingMoves();

            RemoveCastlingCheckMoves();

            return _allMovesList;
        }

        /// <summary>
        /// Returns all pseudo legal moves (i.e. all possible moves including 
        /// those that put the king in check)
        /// </summary>
        /// <param name="board"></param>
        public static List<PieceMoves> CalculateAllPseudoLegalMoves(IBoard board)
        {
            _allMovesList = new List<PieceMoves>();
            
            _checkCount = 0;

            PieceValidMoves.GenerateMoveArrays();

            m_CurrentBoard = board;
            m_CurrentBoard.CalculateUsefulBitboards();

            m_FriendlyColour = m_CurrentBoard.WhiteToMove ? PieceColour.White : PieceColour.Black;

            if (m_FriendlyColour == PieceColour.White)
            {
                CalculateWhiteKnightMoves();
                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteBishops));
                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteRooks));
                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteQueen));
                CalculateWhitePawnMoves();
                CalculateWhiteKingMoves();
                CheckForWhiteCastlingMoves();
            }
            else
            {
                CalculateBlackKnightMoves();
                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackBishops));
                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackRooks));
                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackQueen));
                CalculateBlackPawnMoves();
                CalculateBlackKingMoves();
                CheckForBlackCastlingMoves();
            }

            return _allMovesList;
        } 

        /// <summary>
        /// Checks all moves list and removes any that would put king in check
        /// </summary>
        private static void RemoveCheckingMoves()
        {
            //Search backwards so we can remove moves and carry on
            for (var i = _allMovesList.Count - 1; i >= 0; i--)
            {
                var currentMove = _allMovesList[i];
                
                m_CurrentBoard.MakeMove(currentMove.Position, currentMove.Moves, currentMove.Type, currentMove.SpecialMove, false);

                if (BoardChecking.IsKingInCheck(m_CurrentBoard, m_FriendlyColour))
                {
                    _allMovesList.RemoveAt(i);
                }

                m_CurrentBoard.UnMakeLastMove();
            }
        }

        private static void RemoveCastlingCheckMoves()
        {
            //Search backwards so we can remove moves and carry on
            for (var i = _allMovesList.Count - 1; i >= 0; i--)
            {
                var currentMove = _allMovesList[i];

                if (   currentMove.SpecialMove == SpecialMoveType.KingCastle
                    || currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (BoardChecking.IsKingInCheck(m_CurrentBoard, m_FriendlyColour))
                    {
                        _allMovesList.RemoveAt(i);
                    }
                    else if (!ValidateCastlingMove(m_CurrentBoard, currentMove))
                    {
                        _allMovesList.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Checks that the last move was legal by ensuring that the player who has just moved is not in check
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool ValidateMove(IBoard board)
        {
            bool valid;
            
            PieceColour colourToCheck;

            if (board.WhiteToMove)
            {
                colourToCheck = PieceColour.Black;
            }
            else
                colourToCheck = PieceColour.White; 

            if (BoardChecking.IsKingInCheck(board, colourToCheck))
                valid = false;
            else
                valid =  true;
            
            return valid;
        }

        /// <summary>
        /// Checks that making a castling move would not violate any castling rules
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <param name="currentMove"></param>
        internal static bool ValidateCastlingMove(IBoard boardPosition, PieceMoves currentMove)
        {
            if(boardPosition.WhiteToMove)
            {
                if(currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (IsCastlingPathAttacked(LookupTables.WhiteCastlingQueensideAttackPath, PieceColour.White))
                        return false;
                }
                else if (currentMove.SpecialMove == SpecialMoveType.KingCastle)
                {
                    if (IsCastlingPathAttacked(LookupTables.WhiteCastlingKingsideAttackPath, PieceColour.White))
                        return false;
                }
            }
            else
            {
                if (currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (IsCastlingPathAttacked(LookupTables.BlackCastlingQueensideAttackPath, PieceColour.Black))
                        return false;
                }
                else if (currentMove.SpecialMove == SpecialMoveType.KingCastle)
                {
                    if (IsCastlingPathAttacked(LookupTables.BlackCastlingKingsideAttackPath, PieceColour.Black))
                        return false;
                }
            }

            return true;

        }

        /// <summary>
        /// Checks if the king is in check
        /// </summary>
        /// <returns></returns>
        private static bool IsKingInCheck()
        {
            _checkCount = 0;

            ulong friendlyKing;

            if (m_FriendlyColour == PieceColour.White)
                friendlyKing = m_CurrentBoard.WhiteKing;
            else
                friendlyKing = m_CurrentBoard.BlackKing;

            IsSquareAttacked(friendlyKing, m_FriendlyColour);
                   
            if(_checkCount > 0)
                return true;
            else 
                return false;          
        }

        #region Pawn moves
        
        private static void CalculateWhitePawnMoves()
        {
            var whitePawnPositions = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhitePawns);

            for (var i = 0; i < whitePawnPositions.Length; i++)
            {
                var currentPosition = whitePawnPositions[i];
            
                var pawnSingleMove = currentPosition << 8;

                //Check for promotions
                var promotionsBoard = (pawnSingleMove & LookupTables.RankMask8) & ~m_CurrentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0)    //There are promotions. Split moves
                {
                    //Add promotions to a new move
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.KnightPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.BishopPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.RookPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.QueenPromotion });
                }
                else
                {
                    pawnSingleMove &= m_CurrentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnSingleMove, SpecialMove = SpecialMoveType.Normal });

                        //A pawn that can be promoted can't double move so we're safe to check here
                        //Check for double moves

                        var pawnDoubleMove = (currentPosition << 16) & m_CurrentBoard.EmptySquares;

                        if ((currentPosition & LookupTables.RankMask2) != 0 && pawnDoubleMove > 0)    //If on start rank and first rank is not blocked
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnDoubleMove, SpecialMove = SpecialMoveType.DoublePawnPush });
                        }
                    }
                }

                var possiblePawnCaptures = ValidMoveArrays.WhitePawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];

                var pawnEnPassantCaptures = possiblePawnCaptures & m_CurrentBoard.EnPassantPosition;

                if (pawnEnPassantCaptures > 0)
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnEnPassantCaptures, SpecialMove = SpecialMoveType.ENPassantCapture });

                var pawnCaptures = possiblePawnCaptures & m_CurrentBoard.AllBlackOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    var capturePromotionsBoard = pawnCaptures & LookupTables.RankMask8;

                    if (capturePromotionsBoard > 0)
                    {
                        //pawnCaptures = pawnCaptures & ~capturePromotionsBoard;

                        var pawnPromotionCapturesSingleBoard = BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (var move in pawnPromotionCapturesSingleBoard)
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.KnightPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.BishopPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.RookPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.QueenPromotionCapture });
                        }
                    }
                    else
                    {
                        var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (var move in pawnCapturesSingleBoard)
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.Capture });
                        }
                    }
                }
            }
        }

        private static void CalculateBlackPawnMoves()
        {
            var blackPawnPositions = BitboardOperations.SplitBoardToArray(m_CurrentBoard.BlackPawns);

            foreach (var currentPosition in blackPawnPositions)
            {
                var pawnSingleMove = currentPosition >> 8;

                //Check for promotions
                var promotionsBoard = (pawnSingleMove & LookupTables.RankMask1) & ~m_CurrentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0)    //There are promortions. Split moves
                {
                    //Remove promotions from pawn moves
                    //pawnSingleMove = pawnSingleMove & ~promotionsBoard;

                    //Add promotions to a new move
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.KnightPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.BishopPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.RookPromotion });
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.QueenPromotion });
                }
                else
                {
                    pawnSingleMove = pawnSingleMove & m_CurrentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnSingleMove, SpecialMove = SpecialMoveType.Normal });

                        //A pawn that can be promoted can't double move so we're safe to check here
                        //Check for double moves
                        var pawnDoubleMove = (currentPosition >> 16) & m_CurrentBoard.EmptySquares;

                        if ((currentPosition & LookupTables.RankMask7) != 0 && pawnDoubleMove != 0)   //If on start rank and first rank is not blocked
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnDoubleMove, SpecialMove = SpecialMoveType.DoublePawnPush });
                        }
                    }
                }

                var possiblePawnCaptures = ValidMoveArrays.BlackPawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];
                
                var pawnEnPassantCaptures = possiblePawnCaptures & m_CurrentBoard.EnPassantPosition;

                if(pawnEnPassantCaptures > 0)
                    _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnEnPassantCaptures, SpecialMove = SpecialMoveType.ENPassantCapture });

                var pawnCaptures = possiblePawnCaptures & m_CurrentBoard.AllWhiteOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    var capturePromotionsBoard = pawnCaptures & LookupTables.RankMask1;

                    if(capturePromotionsBoard > 0)
                    {
                        //pawnCaptures = pawnCaptures & ~capturePromotionsBoard;

                        var pawnPromotionCapturesSingleBoard = BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (var move in pawnPromotionCapturesSingleBoard)
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.KnightPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.BishopPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.RookPromotionCapture });
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.QueenPromotionCapture });
                        }
                    }
                    else
                    {
                        var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (var move in pawnCapturesSingleBoard)
                        {
                            _allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.Capture });
                        }                    
                    }
                }
            }
        }

        #endregion Pawn moves

        #region Knight moves
        
        private static void CalculateWhiteKnightMoves()
        {
            var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteKnights);

            var index = whiteKnightPositions.Count - 1;

            while (index >= 0)
            {                
                ulong currentPosition = whiteKnightPositions[index];

                var piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                var pieceType = PieceType.Knight;

                var possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                var splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (var move in splitMoves)
                {
                    if ((move & m_CurrentBoard.AllBlackOccupiedSquares) > 0)
                        _allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Capture });

                    if ((move & m_CurrentBoard.EmptySquares) > 0)
                        _allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Normal });     
                }

                index--;
            }
        }

        private static void CalculateBlackKnightMoves()
        {
            var blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackKnights);

            var index = blackKnightPositions.Count - 1;

            while (index >= 0)
            {
                ulong currentPosition = blackKnightPositions[index];

                var piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                var pieceType = PieceType.Knight;

                var possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                var splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (var move in splitMoves)
                {
                    if ((move & m_CurrentBoard.AllWhiteOccupiedSquares) > 0)
                        _allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Capture });

                    if ((move & m_CurrentBoard.EmptySquares) > 0)
                        _allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Normal });        
                }

                index--;
            }
        }

        #endregion Knight moves

        #region Bishop moves

        private static void CalculateBishopMoves(IReadOnlyList<byte> bishopPositions)
        {
            var index = bishopPositions.Count - 1;

            while (index >= 0)
            {
                var bishopIndex = bishopPositions[index];
                var bishopPosition = LookupTables.SquareValuesFromIndex[bishopIndex];

                var allAllowedMoves = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, bishopIndex, m_FriendlyColour);

                var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Normal);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Capture);

                index--;
            }
        }

        private static void SplitAndAddMoves(ulong moves, ulong position, PieceType type, SpecialMoveType specialMoveType)
        {
            var splitMoves = BitboardOperations.SplitBoardToArray(moves);

            foreach (var move in splitMoves)
            {
                if (move > 0)
                    _allMovesList.Add(new PieceMoves { Type = type, Position = position, Moves = move, SpecialMove = specialMoveType});
            }
        }

        #endregion Bishop moves

        #region Rook moves
        
        private static void CalculateRookMoves(IReadOnlyList<byte> rookPositions)
        {
            var index = rookPositions.Count - 1;

            while (index >= 0)
            {
                var rookIndex = rookPositions[index];
                var rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];

                var allAllowedMoves = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, rookIndex, m_FriendlyColour);

                var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, rookPosition, PieceType.Rook, SpecialMoveType.Normal);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, rookPosition, PieceType.Rook, SpecialMoveType.Capture);

                index--;
            }
        }

        #endregion Rook moves

        #region Queen moves

        private static void CalculateQueenMoves(IReadOnlyList<byte> queenPositions)
        {
            var index = queenPositions.Count - 1;

            while (index >= 0)
            {
                var pieceType = PieceType.Queen;

                var pieceIndex = queenPositions[index];
                var piecePosition = LookupTables.SquareValuesFromIndex[pieceIndex];

                var allAllowedMoves = BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, pieceIndex, m_FriendlyColour);

                var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, piecePosition, pieceType, SpecialMoveType.Normal);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, piecePosition, pieceType, SpecialMoveType.Capture);

                index--;
            }
        }

        #endregion Queen moves

        #region King moves
        private static void CalculateWhiteKingMoves()
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.WhiteKing);

            var whiteKingMoves = ValidMoveArrays.KingMoves[whiteKingPosition];

            var splitMoves = BitboardOperations.SplitBoardToArray(whiteKingMoves);

            foreach (var moveBoard in splitMoves)
            {
                if ((moveBoard & m_CurrentBoard.EmptySquares) > 0)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = moveBoard,
                                          SpecialMove = SpecialMoveType.Normal
                                      });
                }

                if ((moveBoard & m_CurrentBoard.AllBlackOccupiedSquares) > 0)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = moveBoard,
                                          SpecialMove = SpecialMoveType.Capture
                                      });
                }
            }
        }

        private static void CalculateBlackKingMoves()
        {
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.BlackKing);

            var blackKingMoves = ValidMoveArrays.KingMoves[blackKingPosition];
            
            var splitMoves = BitboardOperations.SplitBoardToArray(blackKingMoves);

            foreach (var moveBoard in splitMoves)
            {
                if ((moveBoard & m_CurrentBoard.EmptySquares) > 0)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type  = PieceType.King, Position = m_CurrentBoard.BlackKing,
                                          Moves = moveBoard, SpecialMove   = SpecialMoveType.Normal
                                      });
                }

                if ((moveBoard & m_CurrentBoard.AllWhiteOccupiedSquares) > 0)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type  = PieceType.King, Position = m_CurrentBoard.BlackKing,
                                          Moves = moveBoard, SpecialMove   = SpecialMoveType.Capture
                                      });
                }
            }
        }

        #endregion King moves
        
        #region Special move methods
        
        /// <summary>
        /// Adds an en passant capture to the valid movelist
        /// </summary>
        /// <param name="attackingPiece"></param>
        /// <param name="capturePosition"></param>
        private static void AddEnPassantCapture(ulong attackingPiece, ulong capturePosition)
        {
            var enPassantCapture = new PieceMoves();
            enPassantCapture.Type = PieceType.Pawn;
            enPassantCapture.Position = attackingPiece;
            enPassantCapture.Moves = capturePosition;

            _allMovesList.Add(enPassantCapture);
        }

        private static void CheckForWhiteCastlingMoves()
        {
            if (m_CurrentBoard.WhiteCanCastleQueenside)
            {
                // Check the path is clear
                var castlingPath = LookupTables.WhiteCastlingQueensideObstructionPath;
                
                if ((castlingPath & m_CurrentBoard.EmptySquares) == castlingPath)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = LookupTables.C1,
                                          SpecialMove = SpecialMoveType.QueenCastle
                                      });
                }
            }

            if (m_CurrentBoard.WhiteCanCastleKingside)
            {
                // Check the path is clear
                var castlingPath = LookupTables.WhiteCastlingKingsideObstructionPath;

                if ((castlingPath & m_CurrentBoard.EmptySquares) == castlingPath)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = LookupTables.G1,
                                          SpecialMove = SpecialMoveType.KingCastle
                                      });
                }
            }
        }

        private static void CheckForBlackCastlingMoves()
        {
            if (m_CurrentBoard.BlackCanCastleQueenside)
            {
                // Check the path is clear
                var castlingPath = LookupTables.BlackCastlingQueensideObstructionPath;

                if ((castlingPath & m_CurrentBoard.EmptySquares) == castlingPath)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.BlackKing,
                                          Moves       = LookupTables.C8,
                                          SpecialMove = SpecialMoveType.QueenCastle
                                      });
                }
            }

            if (m_CurrentBoard.BlackCanCastleKingside)
            {
                // Check the path is clear
                var castlingPath = LookupTables.BlackCastlingKingsideObstructionPath;

                if ((castlingPath & m_CurrentBoard.EmptySquares) == castlingPath)
                {
                    _allMovesList.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.BlackKing,
                                          Moves       = LookupTables.G8,
                                          SpecialMove = SpecialMoveType.KingCastle
                                      });
                }
            }
        }

        //private static void CheckForCastlingMoves()
        //{
        //    if (m_FriendlyColour == PieceColour.White)
        //    {
        //        if (!BoardChecking.IsKingInCheck(m_CurrentBoard, PieceColour.White))
        //        {
        //            if (m_CurrentBoard.WhiteCanCastleQueenside)
        //            {
        //                if (CheckIfPathIsClear(LookupTables.WhiteCastlingQueensideObstructionPath))
        //                {
        //                    //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingQueensideAttackPath, PieceColour.White))
        //                    _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = LookupTables.C1, SpecialMove = SpecialMoveType.QueenCastle });
        //                }
        //            }

        //            if (m_CurrentBoard.WhiteCanCastleKingside)
        //            {

        //                if (CheckIfPathIsClear(LookupTables.WhiteCastlingKingsideObstructionPath))
        //                {
        //                    //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingKingsideAttackPath, PieceColour.White))
        //                    _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = LookupTables.G1, SpecialMove = SpecialMoveType.KingCastle });
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (!BoardChecking.IsKingInCheck(m_CurrentBoard, PieceColour.White))
        //        {
        //            if (m_CurrentBoard.BlackCanCastleQueenside)
        //            {
        //                if (CheckIfPathIsClear(LookupTables.BlackCastlingQueensideObstructionPath))
        //                {
        //                    //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingQueensideAttackPath, PieceColour.Black))
        //                    _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = LookupTables.C8, SpecialMove = SpecialMoveType.QueenCastle });
        //                }
        //            }
        //            if (m_CurrentBoard.BlackCanCastleKingside)
        //            {
        //                if (CheckIfPathIsClear(LookupTables.BlackCastlingKingsideObstructionPath))
        //                {
        //                    //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingKingsideAttackPath, PieceColour.Black))
        //                    _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = LookupTables.G8, SpecialMove = SpecialMoveType.KingCastle });
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion Special move methods

        #region Attack calculations

        /// <summary>
        /// This method is used to check if a square is under attack or to check if the king is under attack.
        /// With the latter checkCount is used in order to check for double check.
        /// </summary>
        /// <param name="squarePositionBoard"></param>
        /// <param name="friendlyColour"></param>
        /// <returns></returns>
        private static bool IsSquareAttacked(ulong squarePositionBoard, PieceColour friendlyColour)
        {
            var squareAttacked = false;

            if (IsPawnAttackingSquare(squarePositionBoard, friendlyColour))
            {
                squareAttacked = true;
                //pawnCheck = true;
            }

            if (IsKnightAttackingSquare(squarePositionBoard, friendlyColour))
            {
                squareAttacked = true;
                //knightCheck = true;
            }

            if (IsSquareUnderRayAttack(squarePositionBoard, friendlyColour))
                squareAttacked = true;

            if (BoardChecking.IsSquareAttackedByKing(m_CurrentBoard, squarePositionBoard, friendlyColour))
                squareAttacked = true;

            if (squareAttacked)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the square is under ray attack. Must check every square for double-checks
        /// </summary>
        /// <param name="squarePositionBoard"></param>
        /// <param name="friendlyColour"></param>
        /// <returns></returns>
        private static bool IsSquareUnderRayAttack(ulong squarePositionBoard, PieceColour friendlyColour)
        {
            var underRayAttack = false;

            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (friendlyColour == PieceColour.White)
            {
                enemyQueenSquares = m_CurrentBoard.BlackQueen;
                enemyBishopSquares = m_CurrentBoard.BlackBishops;
                enemyRookSquares = m_CurrentBoard.BlackRooks;
            }
            else
            {
                enemyQueenSquares = m_CurrentBoard.WhiteQueen;
                enemyBishopSquares = m_CurrentBoard.WhiteBishops;
                enemyRookSquares = m_CurrentBoard.WhiteRooks;
            }

            //Up
            var nearestUpPiece = BoardChecking.FindUpBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                _checkCount++;
            }

            //Left 
            var nearestLeftPiece = BoardChecking.FindLeftBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                _checkCount++;
            }

            //Right
            var nearestRightPiece = BoardChecking.FindRightBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                
                _checkCount++;
            }

            //Down
            var nearestDownPiece = BoardChecking.FindDownBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;                 
                _checkCount++;
            }

            //Up-right
            var nearestUpRightPiece = BoardChecking.FindUpRightBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                _checkCount++;
            }

            //Up Left
            var nearestUpLeftPiece = BoardChecking.FindUpLeftBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                _checkCount++;
            }

            //Down-right
            var nearestDownRightPiece = BoardChecking.FindDownRightBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;              
                _checkCount++;
            }

            //Up Left
            var nearestDownLeftPiece = BoardChecking.FindDownLeftBlockingPosition(m_CurrentBoard, squarePositionBoard);

            if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                _checkCount++;
            }

            return underRayAttack;
        }

        /// <summary>
        /// Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        /// since only one pawn can be attacking the king at once
        /// </summary>
        private static bool IsPawnAttackingSquare(ulong squarePosition, PieceColour friendlyColour)
        {
            var squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            var proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      //Allows the quick masking of wrapping checks

            if (friendlyColour == PieceColour.White)
            {                
                //Check up-right
                var upRight = squarePosition << 9;

                if ((upRight & m_CurrentBoard.BlackPawns & proximityBoard) != 0)
                {
                    _checkCount++;
                    return true;
                }

                //Check up-left
                var upLeft = squarePosition << 7;

                if ((upLeft & m_CurrentBoard.BlackPawns & proximityBoard) != 0)
                {
                    _checkCount++;
                    return true;
                }
            }
            else
            {               
                //Check down-right
                var downRight = squarePosition >> 7;

                if ((downRight & m_CurrentBoard.WhitePawns & proximityBoard) != 0)
                {
                    _checkCount++;
                    return true;
                }

                //Check down-left
                var upLeft = squarePosition >> 9;

                if ((upLeft & m_CurrentBoard.WhitePawns & proximityBoard) != 0)
                {
                    _checkCount++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a knight is attacking square. There is no need to check all knights for double-check 
        /// since only one knight can be attacking the king at once
        /// </summary>
        private static bool IsKnightAttackingSquare(ulong squarePosition, PieceColour friendlyColour)
        {
            ulong knights;

            if (friendlyColour == PieceColour.White)
                knights = m_CurrentBoard.BlackKnights;
            else
                knights = m_CurrentBoard.WhiteKnights;

            var currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

            var possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

            var knightAttacks = possibleKnightMoves & knights;
            if (knightAttacks != 0)
            {
                _checkCount++;
                return true;
            }
            else
                return false;

        }

        #region Castling attacks

        private static bool IsCastlingPathAttacked(ulong path, PieceColour friendlyColour)
        {
            //Calculate path positions
            var pathPositions = BitboardOperations.SplitBoardToArray(path);

            foreach (var position in pathPositions)
            {
                if (IsCastlingSquareAttacked(position, friendlyColour))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the given square is under attack on the current bitboard
        /// </summary>
        /// <param name="positionBitboard"></param>
        private static bool IsCastlingSquareAttacked(ulong squarePosition, PieceColour friendlyColour)
        {
            if (IsKnightAttackingSquare(squarePosition, friendlyColour))
                return true;

            if (IsPawnAttackingSquare(squarePosition, friendlyColour))
                return true;

            if (BoardChecking.IsSquareAttackedByKing(m_CurrentBoard, squarePosition, friendlyColour))
                return true;

            if (friendlyColour == PieceColour.White)
            {
                if (BoardChecking.IsSquareRayAttackedFromAbove(m_CurrentBoard, squarePosition) || BoardChecking.IsSquareRayAttackedFromTheSide(m_CurrentBoard, squarePosition))      //White castling king willnot be attacked from below so no need to check all squares
                    return true;
            }
            else
            {
                if (BoardChecking.IsSquareRayAttackedFromBelow(m_CurrentBoard, squarePosition) || BoardChecking.IsSquareRayAttackedFromTheSide(m_CurrentBoard, squarePosition))      //Black castling king will not be attacked from above so no need to check all squares
                    return true;
            }

            return false;
        }

        #endregion castling attacks
        
        #endregion attack calculations

        #endregion Calculate moves methods

    }
}
