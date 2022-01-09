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
        private static Board m_CurrentBoard;

        private static List<PieceMoves> m_AllMoves;

        private static int _checkCount; //Used to find if king is in double check

        /// <summary>
        /// Returns all truly legal moves
        /// </summary>
        /// <returns></returns>
        public static List<PieceMoves> CalculateAllMoves(Board board)
        {
            m_AllMoves = new List<PieceMoves>();

            _checkCount = 0;

            CalculateAllPseudoLegalMoves(board);

            RemoveSelfCheckingMoves();

            RemoveSelfCheckingCastlingMoves();

            return m_AllMoves;
        }

        /// <summary>
        /// Returns all pseudo legal moves (i.e. all possible moves including 
        /// those that put the king in check)
        /// </summary>
        /// <param name="board"></param>
        public static List<PieceMoves> CalculateAllPseudoLegalMoves(Board board)
        {
            m_AllMoves = new List<PieceMoves>();

            _checkCount = 0;

            m_CurrentBoard = board;
            m_CurrentBoard.CalculateUsefulBitboards();

            if (m_CurrentBoard.WhiteToMove)
            {
                CalculateWhiteKnightMoves(false);

                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteBishops),
                                     false);

                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteRooks),
                                   false);

                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteQueen),
                                    false);

                CalculateWhitePawnMoves(false);

                CalculateWhiteKingMoves(false);

                CheckForWhiteCastlingMoves();
            }
            else
            {
                CalculateBlackKnightMoves(false);

                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackBishops),
                                     false);

                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackRooks),
                                   false);

                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackQueen),
                                    false);

                CalculateBlackPawnMoves(false);

                CalculateBlackKingMoves(false);

                CheckForBlackCastlingMoves();
            }

            return m_AllMoves;
        }

        // Calculates all capturing moves
        public static List<PieceMoves> CalculateAllCapturingMoves(Board board)
        {
            m_AllMoves = new List<PieceMoves>();

            _checkCount = 0;

            m_CurrentBoard = board;
            m_CurrentBoard.CalculateUsefulBitboards();

            if (m_CurrentBoard.WhiteToMove)
            {
                CalculateWhiteKnightMoves(true);

                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteBishops),
                                     true);

                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteRooks),
                                   true);

                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteQueen),
                                    true);

                CalculateWhitePawnMoves(true);

                CalculateWhiteKingMoves(true);
            }
            else
            {
                CalculateBlackKnightMoves(true);

                CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackBishops),
                                     true);

                CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackRooks),
                                   true);

                CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackQueen),
                                    true);

                CalculateBlackPawnMoves(true);

                CalculateBlackKingMoves(true);
            }

            RemoveSelfCheckingMoves();

            return m_AllMoves;
        }

        /// <summary>
        /// Checks all moves list and removes any that would put king in check
        /// </summary>
        private static void RemoveSelfCheckingMoves()
        {
            var pieceMover = new PieceMover(m_CurrentBoard);
            
            var whiteToCheck = m_CurrentBoard.WhiteToMove;

            //Search backwards so we can remove moves and carry on
            for (var i = m_AllMoves.Count - 1; i >= 0; i--)
            {
                var currentMove = m_AllMoves[i];

                pieceMover.MakeMove(currentMove.Position, 
                                        currentMove.Moves, 
                                        currentMove.Type,
                                        currentMove.SpecialMove, 
                                        false);

                if (BoardChecking.IsKingInCheck(m_CurrentBoard, whiteToCheck))
                {
                    m_AllMoves.RemoveAt(i);
                }

                pieceMover.UnMakeLastMove();
            }
        }

        private static void RemoveSelfCheckingCastlingMoves()
        {
            //Search backwards so we can remove moves and carry on
            for (var i = m_AllMoves.Count - 1; i >= 0; i--)
            {
                var currentMove = m_AllMoves[i];

                if (currentMove.SpecialMove == SpecialMoveType.KingCastle)
                {
                    if (BoardChecking.IsKingInCheck(m_CurrentBoard, m_CurrentBoard.WhiteToMove))
                    {
                        m_AllMoves.RemoveAt(i);
                    }
                    else if (!ValidateKingsideCastlingMove(m_CurrentBoard, currentMove))
                    {
                        m_AllMoves.RemoveAt(i);
                    }
                }
                else if (currentMove.SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (BoardChecking.IsKingInCheck(m_CurrentBoard, m_CurrentBoard.WhiteToMove))
                    {
                        m_AllMoves.RemoveAt(i);
                    }
                    else if (!ValidateQueensideCastlingMove(m_CurrentBoard, currentMove))
                    {
                        m_AllMoves.RemoveAt(i);
                    }
                }
            }
        }

        // Checks that the last move was legal by ensuring that the player who has just moved is not in check
        public static bool ValidateMove(Board board)
        {
            bool valid;
            
            if (BoardChecking.IsKingInCheck(board, !board.WhiteToMove))
                valid = false;
            else
                valid = true;

            return valid;
        }
        
        internal static bool ValidateKingsideCastlingMove(Board boardPosition, PieceMoves currentMove)
        {
            if (boardPosition.WhiteToMove)
            {
                if (IsCastlingPathAttacked(LookupTables.WhiteCastlingKingsideAttackPath, true))
                {
                    return false;
                }
            }
            else
            {

                if (IsCastlingPathAttacked(LookupTables.BlackCastlingKingsideAttackPath, false))
                {
                    return false;
                }
            }

            return true;
        }
        
        internal static bool ValidateQueensideCastlingMove(Board boardPosition, PieceMoves currentMove)
        {
            if (boardPosition.WhiteToMove)
            {
                if (IsCastlingPathAttacked(LookupTables.WhiteCastlingQueensideAttackPath, true))
                {
                    return false;
                }
            }
            else
            {
                if (IsCastlingPathAttacked(LookupTables.BlackCastlingQueensideAttackPath, false))
                    return false;
            }

            return true;
        }

        private static void CalculateWhitePawnMoves(bool capturesOnly)
        {
            var whitePawnPositions = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhitePawns);

            for (var i = 0; i < whitePawnPositions.Length; i++)
            {
                var currentPosition = whitePawnPositions[i];

                var pawnSingleMove = currentPosition << 8;

                if (!capturesOnly)
                {
                    //Check for promotions
                    var promotionsBoard =
                        (pawnSingleMove & LookupTables.RankMask8) & ~m_CurrentBoard.AllOccupiedSquares;

                    if (promotionsBoard > 0) //There are promotions. Split moves
                    {
                        //Add promotions to a new move
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.KnightPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.BishopPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.RookPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.QueenPromotion
                                          });
                    }
                    else
                    {
                        pawnSingleMove &= m_CurrentBoard.EmptySquares;

                        if (pawnSingleMove > 0)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = pawnSingleMove,
                                                  SpecialMove = SpecialMoveType.Normal
                                              });

                            //A pawn that can be promoted can't double move so we're safe to check here
                            //Check for double moves

                            var pawnDoubleMove = (currentPosition << 16) & m_CurrentBoard.EmptySquares;

                            if ((currentPosition & LookupTables.RankMask2) != 0 && pawnDoubleMove > 0
                            ) //If on start rank and first rank is not blocked
                            {
                                m_AllMoves.Add(new PieceMoves
                                                  {
                                                      Type        = PieceType.Pawn,
                                                      Position    = currentPosition,
                                                      Moves       = pawnDoubleMove,
                                                      SpecialMove = SpecialMoveType.DoublePawnPush
                                                  });
                            }
                        }
                    }
                }

                var possiblePawnCaptures =
                    ValidMoveArrays.WhitePawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];

                var pawnEnPassantCaptures = possiblePawnCaptures & (m_CurrentBoard.EnPassantPosition & LookupTables.RankMask6);

                if (pawnEnPassantCaptures > 0)
                {
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.Pawn,
                                          Position    = currentPosition,
                                          Moves       = pawnEnPassantCaptures,
                                          SpecialMove = SpecialMoveType.ENPassantCapture
                                      });
                }

                var pawnCaptures = possiblePawnCaptures & m_CurrentBoard.AllBlackOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    var capturePromotionsBoard = pawnCaptures & LookupTables.RankMask8;

                    if (capturePromotionsBoard > 0)
                    {
                        var pawnPromotionCapturesSingleBoard =
                            BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (var move in pawnPromotionCapturesSingleBoard)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.KnightPromotionCapture
                                              });

                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.BishopPromotionCapture
                                              });
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.RookPromotionCapture
                                              });
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.QueenPromotionCapture
                                              });
                        }
                    }
                    else
                    {
                        var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (var move in pawnCapturesSingleBoard)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn, Position = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.Capture
                                              });
                        }
                    }
                }
            }
        }

        private static void CalculateBlackPawnMoves(bool capturesOnly)
        {
            var blackPawnPositions = BitboardOperations.SplitBoardToArray(m_CurrentBoard.BlackPawns);

            foreach (var currentPosition in blackPawnPositions)
            {
                var pawnSingleMove = currentPosition >> 8;

                if (!capturesOnly)
                {
                    // Check for promotions
                    var promotionsBoard =
                        (pawnSingleMove & LookupTables.RankMask1) & ~m_CurrentBoard.AllOccupiedSquares;

                    if (promotionsBoard > 0) //There are promotions. Split moves
                    {
                        // Add promotions to a new move
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.KnightPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.BishopPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.RookPromotion
                                          });
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = promotionsBoard,
                                              SpecialMove = SpecialMoveType.QueenPromotion
                                          });
                    }
                    else
                    {
                        pawnSingleMove = pawnSingleMove & m_CurrentBoard.EmptySquares;

                        if (pawnSingleMove > 0)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = pawnSingleMove,
                                                  SpecialMove = SpecialMoveType.Normal
                                              });

                            // A pawn that can be promoted can't double move so we're safe to check here
                            // Check for double moves
                            var pawnDoubleMove = (currentPosition >> 16) & m_CurrentBoard.EmptySquares;

                            // If on start rank and first rank is not blocked
                            if ((currentPosition & LookupTables.RankMask7) != 0 && pawnDoubleMove != 0) 
                            {
                                m_AllMoves.Add(new PieceMoves
                                                  {
                                                      Type        = PieceType.Pawn,
                                                      Position    = currentPosition,
                                                      Moves       = pawnDoubleMove,
                                                      SpecialMove = SpecialMoveType.DoublePawnPush
                                                  });
                            }
                        }
                    }
                }

                var possiblePawnCaptures =
                    ValidMoveArrays.BlackPawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];

                var pawnEnPassantCaptures = possiblePawnCaptures & (m_CurrentBoard.EnPassantPosition & LookupTables.RankMask3);

                if (pawnEnPassantCaptures > 0)
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.Pawn,
                                          Position    = currentPosition,
                                          Moves       = pawnEnPassantCaptures,
                                          SpecialMove = SpecialMoveType.ENPassantCapture
                                      });

                var pawnCaptures = possiblePawnCaptures & m_CurrentBoard.AllWhiteOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    var capturePromotionsBoard = pawnCaptures & LookupTables.RankMask1;

                    if (capturePromotionsBoard > 0)
                    {
                        //pawnCaptures = pawnCaptures & ~capturePromotionsBoard;

                        var pawnPromotionCapturesSingleBoard =
                            BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (var move in pawnPromotionCapturesSingleBoard)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.KnightPromotionCapture
                                              });
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.BishopPromotionCapture
                                              });
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.RookPromotionCapture
                                              });
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.QueenPromotionCapture
                                              });
                        }
                    }
                    else
                    {
                        var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (var move in pawnCapturesSingleBoard)
                        {
                            m_AllMoves.Add(new PieceMoves
                                              {
                                                  Type        = PieceType.Pawn,
                                                  Position    = currentPosition,
                                                  Moves       = move,
                                                  SpecialMove = SpecialMoveType.Capture
                                              });
                        }
                    }
                }
            }
        }

        private static void CalculateWhiteKnightMoves(bool capturesOnly)
        {
            var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteKnights);

            var index = whiteKnightPositions.Count - 1;

            while (index >= 0)
            {
                ulong currentPosition = whiteKnightPositions[index];

                var piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                var pieceType     = PieceType.Knight;

                var possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                var splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (var move in splitMoves)
                {
                    if ((move & m_CurrentBoard.AllBlackOccupiedSquares) > 0)
                    {
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = pieceType,
                                              Position    = piecePosition,
                                              Moves       = move,
                                              SpecialMove = SpecialMoveType.Capture
                                          });
                    }

                    if ((move & m_CurrentBoard.EmptySquares) > 0 && !capturesOnly)
                    {
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = pieceType,
                                              Position    = piecePosition,
                                              Moves       = move,
                                              SpecialMove = SpecialMoveType.Normal
                                          });
                    }
                }

                index--;
            }
        }

        private static void CalculateBlackKnightMoves(bool capturesOnly)
        {
            var blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackKnights);

            var index = blackKnightPositions.Count - 1;

            while (index >= 0)
            {
                ulong currentPosition = blackKnightPositions[index];

                var piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                var pieceType     = PieceType.Knight;

                var possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                var splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (var move in splitMoves)
                {
                    if ((move & m_CurrentBoard.AllWhiteOccupiedSquares) > 0)
                    {
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = pieceType,
                                              Position    = piecePosition,
                                              Moves       = move,
                                              SpecialMove = SpecialMoveType.Capture
                                          });
                    }

                    if ((move & m_CurrentBoard.EmptySquares) > 0 && !capturesOnly)
                    {
                        m_AllMoves.Add(new PieceMoves
                                          {
                                              Type        = pieceType,
                                              Position    = piecePosition,
                                              Moves       = move,
                                              SpecialMove = SpecialMoveType.Normal
                                          });
                    }
                }

                index--;
            }
        }

        private static void CalculateBishopMoves(IReadOnlyList<byte> bishopPositions, bool capturesOnly)
        {
            var index = bishopPositions.Count - 1;

            while (index >= 0)
            {
                var bishopIndex    = bishopPositions[index];
                var bishopPosition = LookupTables.SquareValuesFromIndex[bishopIndex];

                var allAllowedMoves =
                    BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, bishopIndex, m_CurrentBoard.WhiteToMove);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Capture);

                if (!capturesOnly)
                {
                    var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                    SplitAndAddMoves(normalMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Normal);
                }

                index--;
            }
        }

        private static void SplitAndAddMoves(ulong           moves, ulong position, PieceType type,
                                             SpecialMoveType specialMoveType)
        {
            var splitMoves = BitboardOperations.SplitBoardToArray(moves);

            foreach (var move in splitMoves)
            {
                if (move > 0)
                    m_AllMoves.Add(new PieceMoves
                                      {Type = type, Position = position, Moves = move, SpecialMove = specialMoveType});
            }
        }

        private static void CalculateRookMoves(IReadOnlyList<byte> rookPositions, bool capturesOnly)
        {
            var index = rookPositions.Count - 1;

            while (index >= 0)
            {
                var rookIndex    = rookPositions[index];
                var rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];

                var allAllowedMoves =
                    BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, rookIndex, m_CurrentBoard.WhiteToMove);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, rookPosition, PieceType.Rook, SpecialMoveType.Capture);

                if (!capturesOnly)
                {
                    var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                    SplitAndAddMoves(normalMoves, rookPosition, PieceType.Rook, SpecialMoveType.Normal);
                }

                index--;
            }
        }

        private static void CalculateQueenMoves(IReadOnlyList<byte> queenPositions, bool capturesOnly)
        {
            var index = queenPositions.Count - 1;

            while (index >= 0)
            {
                var pieceType = PieceType.Queen;

                var pieceIndex    = queenPositions[index];
                var piecePosition = LookupTables.SquareValuesFromIndex[pieceIndex];

                var allAllowedMoves =
                    BoardChecking.CalculateAllowedQueenMoves(m_CurrentBoard, pieceIndex, m_CurrentBoard.WhiteToMove);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, piecePosition, pieceType, SpecialMoveType.Capture);

                if (!capturesOnly)
                {
                    var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                    SplitAndAddMoves(normalMoves, piecePosition, pieceType, SpecialMoveType.Normal);
                }

                index--;
            }
        }

        private static void CalculateWhiteKingMoves(bool capturesOnly)
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.WhiteKing);

            var whiteKingMoves = ValidMoveArrays.KingMoves[whiteKingPosition];

            var splitMoves = BitboardOperations.SplitBoardToArray(whiteKingMoves);

            foreach (var moveBoard in splitMoves)
            {
                if ((moveBoard & m_CurrentBoard.AllBlackOccupiedSquares) > 0)
                {
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = moveBoard,
                                          SpecialMove = SpecialMoveType.Capture
                                      });
                }

                if ((moveBoard & m_CurrentBoard.EmptySquares) > 0 && !capturesOnly)
                {
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.WhiteKing,
                                          Moves       = moveBoard,
                                          SpecialMove = SpecialMoveType.Normal
                                      });
                }
            }
        }

        private static void CalculateBlackKingMoves(bool capturesOnly)
        {
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.BlackKing);

            var blackKingMoves = ValidMoveArrays.KingMoves[blackKingPosition];

            var splitMoves = BitboardOperations.SplitBoardToArray(blackKingMoves);

            foreach (var moveBoard in splitMoves)
            {
                if ((moveBoard & m_CurrentBoard.AllWhiteOccupiedSquares) > 0)
                {
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type  = PieceType.King, Position = m_CurrentBoard.BlackKing,
                                          Moves = moveBoard, SpecialMove   = SpecialMoveType.Capture
                                      });
                }

                if ((moveBoard & m_CurrentBoard.EmptySquares) > 0 && !capturesOnly)
                {
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.BlackKing,
                                          Moves       = moveBoard,
                                          SpecialMove = SpecialMoveType.Normal
                                      });
                }
            }
        }

        /// <summary>
        /// Adds an en passant capture to the valid movelist
        /// </summary>
        /// <param name="attackingPiece"></param>
        /// <param name="capturePosition"></param>
        private static void AddEnPassantCapture(ulong attackingPiece, ulong capturePosition)
        {
            var enPassantCapture = new PieceMoves();
            enPassantCapture.Type     = PieceType.Pawn;
            enPassantCapture.Position = attackingPiece;
            enPassantCapture.Moves    = capturePosition;

            m_AllMoves.Add(enPassantCapture);
        }

        private static void CheckForWhiteCastlingMoves()
        {
            if (m_CurrentBoard.WhiteCanCastleQueenside)
            {
                // Check the path is clear
                var castlingPath = LookupTables.WhiteCastlingQueensideObstructionPath;

                if ((castlingPath & m_CurrentBoard.EmptySquares) == castlingPath)
                {
                    m_AllMoves.Add(new PieceMoves
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
                    m_AllMoves.Add(new PieceMoves
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
                    m_AllMoves.Add(new PieceMoves
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
                    m_AllMoves.Add(new PieceMoves
                                      {
                                          Type        = PieceType.King,
                                          Position    = m_CurrentBoard.BlackKing,
                                          Moves       = LookupTables.G8,
                                          SpecialMove = SpecialMoveType.KingCastle
                                      });
                }
            }
        }

        /// <summary>
        /// This method is used to check if a square is under attack or to check if the king is under attack.
        /// With the latter checkCount is used in order to check for double check.
        /// </summary>
        /// <param name="squarePositionBoard"></param>
        /// <param name="friendlyColour"></param>
        /// <returns></returns>
        private static bool IsSquareAttacked(ulong squarePositionBoard, bool whiteToMove)
        {
            bool squareAttacked = IsPawnAttackingSquare(squarePositionBoard, whiteToMove);

            if (IsKnightAttackingSquare(squarePositionBoard, whiteToMove))
            {
                squareAttacked = true;
                //knightCheck = true;
            }

            if (IsSquareUnderRayAttack(squarePositionBoard, whiteToMove))
                squareAttacked = true;

            if (BoardChecking.IsSquareAttackedByKing(m_CurrentBoard, squarePositionBoard, m_CurrentBoard.WhiteToMove))
                squareAttacked = true;

            if (squareAttacked)
                return true;
            else
                return false;
        }
        
        // Checks if the square is under ray attack. Must check every square for double-checks
        private static bool IsSquareUnderRayAttack(ulong squarePositionBoard, bool whiteToMove)
        {
            var underRayAttack = false;

            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (whiteToMove)
            {
                enemyQueenSquares  = m_CurrentBoard.BlackQueen;
                enemyBishopSquares = m_CurrentBoard.BlackBishops;
                enemyRookSquares   = m_CurrentBoard.BlackRooks;
            }
            else
            {
                enemyQueenSquares  = m_CurrentBoard.WhiteQueen;
                enemyBishopSquares = m_CurrentBoard.WhiteBishops;
                enemyRookSquares   = m_CurrentBoard.WhiteRooks;
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
            var nearestDownRightPiece =
                BoardChecking.FindDownRightBlockingPosition(m_CurrentBoard, squarePositionBoard);

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
        private static bool IsPawnAttackingSquare(ulong squarePosition, bool whiteToMove)
        {
            var squareIndex    = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            var proximityBoard = ValidMoveArrays.KingMoves[squareIndex]; //Allows the quick masking of wrapping checks

            if (whiteToMove)
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
        private static bool IsKnightAttackingSquare(ulong squarePosition, bool whiteToMove)
        {
            ulong knights;

            if (whiteToMove)
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

        private static bool IsCastlingPathAttacked(ulong path, bool whiteToMove)
        {
            //Calculate path positions
            var pathPositions = BitboardOperations.SplitBoardToArray(path);

            foreach (var position in pathPositions)
            {
                if (IsCastlingSquareAttacked(position, whiteToMove))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the given square is under attack on the current bitboard
        /// </summary>
        /// <param name="positionBitboard"></param>
        private static bool IsCastlingSquareAttacked(ulong squarePosition, bool whiteToMove)
        {
            if (IsKnightAttackingSquare(squarePosition, whiteToMove))
                return true;

            if (IsPawnAttackingSquare(squarePosition, whiteToMove))
                return true;

            if (BoardChecking.IsSquareAttackedByKing(m_CurrentBoard, squarePosition, whiteToMove))
                return true;

            if (whiteToMove)
            {
                if (BoardChecking.IsSquareRayAttackedFromAbove(m_CurrentBoard, squarePosition) ||
                    BoardChecking.IsSquareRayAttackedFromTheSide(m_CurrentBoard, squarePosition)
                ) //White castling king willnot be attacked from below so no need to check all squares
                    return true;
            }
            else
            {
                if (BoardChecking.IsSquareRayAttackedFromBelow(m_CurrentBoard, squarePosition) ||
                    BoardChecking.IsSquareRayAttackedFromTheSide(m_CurrentBoard, squarePosition)
                ) //Black castling king will not be attacked from above so no need to check all squares
                    return true;
            }

            return false;
        }
    }
}