using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;

namespace ChessEngine.PossibleMoves;

// Calculates all possible moves from a given position
public class MoveGeneration
{
    private Board _currentBoard;

    private List<PieceMove> _allMoves;

    private int _checkCount; //Used to find if king is in double check

    // Returns all truly legal moves
    public List<PieceMove> CalculateAllMoves(Board board)
    {
        _allMoves = new List<PieceMove>();

        _checkCount = 0;

        CalculateAllPseudoLegalMoves(board);

        RemoveSelfCheckingMoves();

        RemoveSelfCheckingCastlingMoves();

        return _allMoves;
    }

    // Returns all pseudo legal moves (i.e. all possible moves including
    // those that put the king in check)
    public List<PieceMove> CalculateAllPseudoLegalMoves(Board board)
    {
        _allMoves = new List<PieceMove>();

        _checkCount = 0;

        _currentBoard = board;
        _currentBoard.CalculateUsefulBitboards();

        if (_currentBoard.WhiteToMove)
        {
            CalculateWhiteKnightMoves(false);

            CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteBishops), false);

            CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteRooks), false);

            CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteQueen), false);

            CalculateWhitePawnMoves(false);

            CalculateWhiteKingMoves(false);

            CheckForWhiteCastlingMoves();
        }
        else
        {
            CalculateBlackKnightMoves(false);

            CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackBishops), false);

            CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackRooks), false);

            CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackQueen), false);

            CalculateBlackPawnMoves(false);

            CalculateBlackKingMoves(false);

            CheckForBlackCastlingMoves();
        }

        return _allMoves;
    }

    // Calculates all capturing moves
    public List<PieceMove> CalculateAllCapturingMoves(Board board)
    {
        _allMoves = new List<PieceMove>();

        _checkCount = 0;

        _currentBoard = board;
        _currentBoard.CalculateUsefulBitboards();

        if (_currentBoard.WhiteToMove)
        {
            CalculateWhiteKnightMoves(true);

            CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteBishops), true);

            CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteRooks), true);

            CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteQueen), true);

            CalculateWhitePawnMoves(true);

            CalculateWhiteKingMoves(true);
        }
        else
        {
            CalculateBlackKnightMoves(true);

            CalculateBishopMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackBishops), true);

            CalculateRookMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackRooks), true);

            CalculateQueenMoves(BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackQueen), true);

            CalculateBlackPawnMoves(true);

            CalculateBlackKingMoves(true);
        }

        RemoveSelfCheckingMoves();

        return _allMoves;
    }

    // Checks all moves list and removes any that would put king in check
    private void RemoveSelfCheckingMoves()
    {
        var pieceMover = new PieceMover(_currentBoard);

        var whiteToCheck = _currentBoard.WhiteToMove;

        //Search backwards so we can remove moves and carry on
        for (var i = _allMoves.Count - 1; i >= 0; i--)
        {
            var currentMove = _allMoves[i];

            pieceMover.MakeMove(currentMove.Position,
                                currentMove.Moves,
                                currentMove.Type,
                                currentMove.SpecialMove);

            if (PieceChecking.IsKingInCheck(_currentBoard, whiteToCheck))
            {
                _allMoves.RemoveAt(i);
            }

            pieceMover.UnMakeLastMove();
        }
    }

    private void RemoveSelfCheckingCastlingMoves()
    {
        //Search backwards so we can remove moves and carry on
        for (var i = _allMoves.Count - 1; i >= 0; i--)
        {
            var currentMove = _allMoves[i];

            if (currentMove.SpecialMove == SpecialMoveType.KingCastle)
            {
                if (PieceChecking.IsKingInCheck(_currentBoard, _currentBoard.WhiteToMove))
                {
                    _allMoves.RemoveAt(i);
                }
                else if (!ValidateKingsideCastlingMove(_currentBoard))
                {
                    _allMoves.RemoveAt(i);
                }
            }
            else if (currentMove.SpecialMove == SpecialMoveType.QueenCastle)
            {
                if (PieceChecking.IsKingInCheck(_currentBoard, _currentBoard.WhiteToMove))
                {
                    _allMoves.RemoveAt(i);
                }
                else if (!ValidateQueensideCastlingMove(_currentBoard))
                {
                    _allMoves.RemoveAt(i);
                }
            }
        }
    }

    // Checks that the last move was legal by ensuring that the player who has just moved is not in check
    public static bool WasLastMoveAllowed(Board board)
    {
        return !PieceChecking.IsKingInCheck(board, !board.WhiteToMove);
    }

    internal bool ValidateKingsideCastlingMove(Board boardPosition)
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

    internal bool ValidateQueensideCastlingMove(Board boardPosition)
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
            {
                return false;
            }
        }

        return true;
    }

    private void CalculateWhitePawnMoves(bool capturesOnly)
    {
        var whitePawnPositions = BitboardOperations.SplitBoardToArray(_currentBoard.WhitePawns);

        foreach (var currentPosition in whitePawnPositions)
        {
            var pawnSingleMove = currentPosition << 8;

            if (!capturesOnly)
            {
                //Check for promotions
                var promotionsBoard =
                    (pawnSingleMove & LookupTables.RowMask8) & ~_currentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0) //There are promotions. Split moves
                {
                    _allMoves.AddRange(GetPromotionMoves(currentPosition, promotionsBoard, false));
                }
                else
                {
                    pawnSingleMove &= _currentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        _allMoves.Add(new PieceMove
                        {
                            Type        = PieceType.Pawn,
                            Position    = currentPosition,
                            Moves       = pawnSingleMove,
                            SpecialMove = SpecialMoveType.Normal
                        });

                        //A pawn that can be promoted can't double move so we're safe to check here
                        //Check for double moves

                        var pawnDoubleMove = (currentPosition << 16) & _currentBoard.EmptySquares;

                        if ((currentPosition & LookupTables.RowMask2) != 0 && pawnDoubleMove > 0) //If on start rank and first rank is not blocked
                        {
                            _allMoves.Add(new PieceMove
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

            var pawnEnPassantCaptures = possiblePawnCaptures & (_currentBoard.EnPassantPosition & LookupTables.RowMask6);

            if (pawnEnPassantCaptures > 0)
            {
                _allMoves.Add(new PieceMove
                {
                    Type        = PieceType.Pawn,
                    Position    = currentPosition,
                    Moves       = pawnEnPassantCaptures,
                    SpecialMove = SpecialMoveType.ENPassantCapture
                });
            }

            var pawnCaptures = possiblePawnCaptures & _currentBoard.AllBlackOccupiedSquares;

            if (pawnCaptures > 0)
            {
                var capturePromotionsBoard = pawnCaptures & LookupTables.RowMask8;

                if (capturePromotionsBoard > 0)
                {
                    var pawnPromotionCapturesSingleBoard =
                        BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                    foreach (var move in pawnPromotionCapturesSingleBoard)
                    {
                        _allMoves.AddRange(GetPromotionMoves(currentPosition, move, true));
                    }
                }
                else
                {
                    var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                    foreach (var move in pawnCapturesSingleBoard)
                    {
                        _allMoves.Add(new PieceMove
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

    private IEnumerable<PieceMove> GetPromotionMoves(ulong currentPosition, ulong promotionsBoard, bool areCaptures)
    {
        var moves = new List<PieceMove>
        {
            new()
            {
                Type = PieceType.Pawn,
                Position = currentPosition,
                Moves = promotionsBoard,
                SpecialMove = areCaptures ? SpecialMoveType.KnightPromotionCapture : SpecialMoveType.KnightPromotion
            },
            new()
            {
                Type = PieceType.Pawn,
                Position = currentPosition,
                Moves = promotionsBoard,
                SpecialMove = areCaptures ? SpecialMoveType.BishopPromotionCapture : SpecialMoveType.BishopPromotion
            },
            new()
            {
                Type = PieceType.Pawn,
                Position = currentPosition,
                Moves = promotionsBoard,
                SpecialMove = areCaptures ? SpecialMoveType.RookPromotionCapture : SpecialMoveType.RookPromotion
            },
            new()
            {
                Type = PieceType.Pawn,
                Position = currentPosition,
                Moves = promotionsBoard,
                SpecialMove = areCaptures ? SpecialMoveType.QueenPromotionCapture : SpecialMoveType.QueenPromotion
            }
        };

        return moves;
    }

    private void CalculateBlackPawnMoves(bool capturesOnly)
    {
        var blackPawnPositions = BitboardOperations.SplitBoardToArray(_currentBoard.BlackPawns);

        foreach (var currentPosition in blackPawnPositions)
        {
            var pawnSingleMove = currentPosition >> 8;

            if (!capturesOnly)
            {
                // Check for promotions
                var promotionsBoard =
                    (pawnSingleMove & LookupTables.RowMask1) & ~_currentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0) //There are promotions. Split moves
                {
                    _allMoves.AddRange(GetPromotionMoves(currentPosition, promotionsBoard, false));
                }
                else
                {
                    pawnSingleMove = pawnSingleMove & _currentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        _allMoves.Add(new PieceMove
                                          {
                                              Type        = PieceType.Pawn,
                                              Position    = currentPosition,
                                              Moves       = pawnSingleMove,
                                              SpecialMove = SpecialMoveType.Normal
                                          });

                        // A pawn that can be promoted can't double move so we're safe to check here
                        // Check for double moves
                        var pawnDoubleMove = (currentPosition >> 16) & _currentBoard.EmptySquares;

                        // If on start rank and first rank is not blocked
                        if ((currentPosition & LookupTables.RowMask7) != 0 && pawnDoubleMove != 0)
                        {
                            _allMoves.Add(new PieceMove
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

            var pawnEnPassantCaptures = possiblePawnCaptures & (_currentBoard.EnPassantPosition & LookupTables.RowMask3);

            if (pawnEnPassantCaptures > 0)
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.Pawn,
                                      Position    = currentPosition,
                                      Moves       = pawnEnPassantCaptures,
                                      SpecialMove = SpecialMoveType.ENPassantCapture
                                  });

            var pawnCaptures = possiblePawnCaptures & _currentBoard.AllWhiteOccupiedSquares;

            if (pawnCaptures > 0)
            {
                var capturePromotionsBoard = pawnCaptures & LookupTables.RowMask1;

                if (capturePromotionsBoard > 0)
                {
                    var pawnPromotionCapturesSingleBoard = BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                    foreach (var move in pawnPromotionCapturesSingleBoard)
                    {
                        _allMoves.AddRange(GetPromotionMoves(currentPosition, move, true));
                    }
                }
                else
                {
                    var pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                    foreach (var move in pawnCapturesSingleBoard)
                    {
                        _allMoves.Add(new PieceMove
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

    private void CalculateWhiteKnightMoves(bool capturesOnly)
    {
        var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.WhiteKnights);

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
                if ((move & _currentBoard.AllBlackOccupiedSquares) > 0)
                {
                    _allMoves.Add(new PieceMove
                                      {
                                          Type        = pieceType,
                                          Position    = piecePosition,
                                          Moves       = move,
                                          SpecialMove = SpecialMoveType.Capture
                                      });
                }

                if ((move & _currentBoard.EmptySquares) > 0 && !capturesOnly)
                {
                    _allMoves.Add(new PieceMove
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

    private void CalculateBlackKnightMoves(bool capturesOnly)
    {
        var blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(_currentBoard.BlackKnights);

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
                if ((move & _currentBoard.AllWhiteOccupiedSquares) > 0)
                {
                    _allMoves.Add(new PieceMove
                                      {
                                          Type        = pieceType,
                                          Position    = piecePosition,
                                          Moves       = move,
                                          SpecialMove = SpecialMoveType.Capture
                                      });
                }

                if ((move & _currentBoard.EmptySquares) > 0 && !capturesOnly)
                {
                    _allMoves.Add(new PieceMove
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

    private void CalculateBishopMoves(IReadOnlyList<byte> bishopPositions, bool capturesOnly)
    {
        var index = bishopPositions.Count - 1;

        while (index >= 0)
        {
            var bishopIndex    = bishopPositions[index];
            var bishopPosition = LookupTables.SquareValuesFromIndex[bishopIndex];

            var allAllowedMoves =
                PieceChecking.CalculateAllowedBishopMoves(_currentBoard, bishopIndex, _currentBoard.WhiteToMove);

            var captureMoves = allAllowedMoves & ~_currentBoard.EmptySquares;
            SplitAndAddMoves(captureMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Capture);

            if (!capturesOnly)
            {
                var normalMoves = allAllowedMoves & _currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, bishopPosition, PieceType.Bishop, SpecialMoveType.Normal);
            }

            index--;
        }
    }

    private void SplitAndAddMoves(ulong           moves, ulong position, PieceType type,
                                         SpecialMoveType specialMoveType)
    {
        var splitMoves = BitboardOperations.SplitBoardToArray(moves);

        foreach (var move in splitMoves)
        {
            if (move > 0)
                _allMoves.Add(new PieceMove
                                  {Type = type, Position = position, Moves = move, SpecialMove = specialMoveType});
        }
    }

    private void CalculateRookMoves(IReadOnlyList<byte> rookPositions, bool capturesOnly)
    {
        var index = rookPositions.Count - 1;

        while (index >= 0)
        {
            var rookIndex    = rookPositions[index];
            var rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];

            var allAllowedMoves =
                PieceChecking.CalculateAllowedRookMoves(_currentBoard, rookIndex, _currentBoard.WhiteToMove);

            var captureMoves = allAllowedMoves & ~_currentBoard.EmptySquares;
            SplitAndAddMoves(captureMoves, rookPosition, PieceType.Rook, SpecialMoveType.Capture);

            if (!capturesOnly)
            {
                var normalMoves = allAllowedMoves & _currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, rookPosition, PieceType.Rook, SpecialMoveType.Normal);
            }

            index--;
        }
    }

    private void CalculateQueenMoves(IReadOnlyList<byte> queenPositions, bool capturesOnly)
    {
        var index = queenPositions.Count - 1;

        while (index >= 0)
        {
            var pieceType = PieceType.Queen;

            var pieceIndex    = queenPositions[index];
            var piecePosition = LookupTables.SquareValuesFromIndex[pieceIndex];

            var allAllowedMoves =
                PieceChecking.CalculateAllowedQueenMoves(_currentBoard, pieceIndex, _currentBoard.WhiteToMove);

            var captureMoves = allAllowedMoves & ~_currentBoard.EmptySquares;
            SplitAndAddMoves(captureMoves, piecePosition, pieceType, SpecialMoveType.Capture);

            if (!capturesOnly)
            {
                var normalMoves = allAllowedMoves & _currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, piecePosition, pieceType, SpecialMoveType.Normal);
            }

            index--;
        }
    }

    private void CalculateWhiteKingMoves(bool capturesOnly)
    {
        var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(_currentBoard.WhiteKing);

        var whiteKingMoves = ValidMoveArrays.KingMoves[whiteKingPosition];

        var splitMoves = BitboardOperations.SplitBoardToArray(whiteKingMoves);

        foreach (var moveBoard in splitMoves)
        {
            if ((moveBoard & _currentBoard.AllBlackOccupiedSquares) > 0)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.WhiteKing,
                                      Moves       = moveBoard,
                                      SpecialMove = SpecialMoveType.Capture
                                  });
            }

            if ((moveBoard & _currentBoard.EmptySquares) > 0 && !capturesOnly)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.WhiteKing,
                                      Moves       = moveBoard,
                                      SpecialMove = SpecialMoveType.Normal
                                  });
            }
        }
    }

    private void CalculateBlackKingMoves(bool capturesOnly)
    {
        var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(_currentBoard.BlackKing);

        var blackKingMoves = ValidMoveArrays.KingMoves[blackKingPosition];

        var splitMoves = BitboardOperations.SplitBoardToArray(blackKingMoves);

        foreach (var moveBoard in splitMoves)
        {
            if ((moveBoard & _currentBoard.AllWhiteOccupiedSquares) > 0)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type  = PieceType.King, Position = _currentBoard.BlackKing,
                                      Moves = moveBoard, SpecialMove   = SpecialMoveType.Capture
                                  });
            }

            if ((moveBoard & _currentBoard.EmptySquares) > 0 && !capturesOnly)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.BlackKing,
                                      Moves       = moveBoard,
                                      SpecialMove = SpecialMoveType.Normal
                                  });
            }
        }
    }

    // Adds an en passant capture to the valid movelist
    private void AddEnPassantCapture(ulong attackingPiece, ulong capturePosition)
    {
        var enPassantCapture = new PieceMove
        {
            Type = PieceType.Pawn,
            Position = attackingPiece,
            Moves = capturePosition
        };

        _allMoves.Add(enPassantCapture);
    }

    private void CheckForWhiteCastlingMoves()
    {
        if (_currentBoard.WhiteCanCastleQueenside)
        {
            // Check the path is clear
            var castlingPath = LookupTables.WhiteCastlingQueensideObstructionPath;

            if ((castlingPath & _currentBoard.EmptySquares) == castlingPath)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.WhiteKing,
                                      Moves       = LookupTables.C1,
                                      SpecialMove = SpecialMoveType.QueenCastle
                                  });
            }
        }

        if (_currentBoard.WhiteCanCastleKingside)
        {
            // Check the path is clear
            var castlingPath = LookupTables.WhiteCastlingKingsideObstructionPath;

            if ((castlingPath & _currentBoard.EmptySquares) == castlingPath)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.WhiteKing,
                                      Moves       = LookupTables.G1,
                                      SpecialMove = SpecialMoveType.KingCastle
                                  });
            }
        }
    }

    private void CheckForBlackCastlingMoves()
    {
        if (_currentBoard.BlackCanCastleQueenside)
        {
            // Check the path is clear
            var castlingPath = LookupTables.BlackCastlingQueensideObstructionPath;

            if ((castlingPath & _currentBoard.EmptySquares) == castlingPath)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.BlackKing,
                                      Moves       = LookupTables.C8,
                                      SpecialMove = SpecialMoveType.QueenCastle
                                  });
            }
        }

        if (_currentBoard.BlackCanCastleKingside)
        {
            // Check the path is clear
            var castlingPath = LookupTables.BlackCastlingKingsideObstructionPath;

            if ((castlingPath & _currentBoard.EmptySquares) == castlingPath)
            {
                _allMoves.Add(new PieceMove
                                  {
                                      Type        = PieceType.King,
                                      Position    = _currentBoard.BlackKing,
                                      Moves       = LookupTables.G8,
                                      SpecialMove = SpecialMoveType.KingCastle
                                  });
            }
        }
    }


    // This method is used to check if a square is under attack or to check if the king is under attack.
    // With the latter checkCount is used in order to check for double check.
    private bool IsSquareAttacked(ulong squarePositionBoard, bool whiteToMove)
    {
        var squareAttacked = IsPawnAttackingSquare(squarePositionBoard, whiteToMove);

        if (IsKnightAttackingSquare(squarePositionBoard, whiteToMove))
        {
            squareAttacked = true;
        }

        if (IsSquareUnderRayAttack(squarePositionBoard, whiteToMove))
        {
            squareAttacked = true;
        }

        if (PieceChecking.IsSquareAttackedByKing(_currentBoard, squarePositionBoard, _currentBoard.WhiteToMove))
        {
            squareAttacked = true;
        }

        if (squareAttacked)
        {
            return true;
        }

        return false;
    }

    // Checks if the square is under ray attack. Must check every square for double-checks
    private bool IsSquareUnderRayAttack(ulong squarePositionBoard, bool whiteToMove)
    {
        var underRayAttack = false;

        ulong enemyQueenSquares;
        ulong enemyBishopSquares;
        ulong enemyRookSquares;

        if (whiteToMove)
        {
            enemyQueenSquares  = _currentBoard.BlackQueen;
            enemyBishopSquares = _currentBoard.BlackBishops;
            enemyRookSquares   = _currentBoard.BlackRooks;
        }
        else
        {
            enemyQueenSquares  = _currentBoard.WhiteQueen;
            enemyBishopSquares = _currentBoard.WhiteBishops;
            enemyRookSquares   = _currentBoard.WhiteRooks;
        }

        //Up
        var nearestUpPiece = PieceChecking.FindUpBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Left
        var nearestLeftPiece = PieceChecking.FindLeftBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Right
        var nearestRightPiece = PieceChecking.FindRightBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;

            _checkCount++;
        }

        //Down
        var nearestDownPiece = PieceChecking.FindDownBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Up-right
        var nearestUpRightPiece = PieceChecking.FindUpRightBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Up Left
        var nearestUpLeftPiece = PieceChecking.FindUpLeftBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Down-right
        var nearestDownRightPiece =
            PieceChecking.FindDownRightBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        //Up Left
        var nearestDownLeftPiece = PieceChecking.FindDownLeftBlockingPosition(_currentBoard, squarePositionBoard);

        if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
        {
            underRayAttack = true;
            _checkCount++;
        }

        return underRayAttack;
    }

    // Checks if pawn is attacking square. There is no need to check all pawns for double-check
    // since only one pawn can be attacking the king at once
    private bool IsPawnAttackingSquare(ulong squarePosition, bool whiteToMove)
    {
        var squareIndex    = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
        var proximityBoard = ValidMoveArrays.KingMoves[squareIndex]; //Allows the quick masking of wrapping checks

        if (whiteToMove)
        {
            //Check up-right
            var upRight = squarePosition << 9;

            if ((upRight & _currentBoard.BlackPawns & proximityBoard) != 0)
            {
                _checkCount++;
                return true;
            }

            //Check up-left
            var upLeft = squarePosition << 7;

            if ((upLeft & _currentBoard.BlackPawns & proximityBoard) != 0)
            {
                _checkCount++;
                return true;
            }
        }
        else
        {
            //Check down-right
            var downRight = squarePosition >> 7;

            if ((downRight & _currentBoard.WhitePawns & proximityBoard) != 0)
            {
                _checkCount++;
                return true;
            }

            //Check down-left
            var upLeft = squarePosition >> 9;

            if ((upLeft & _currentBoard.WhitePawns & proximityBoard) != 0)
            {
                _checkCount++;
                return true;
            }
        }

        return false;
    }

    // Checks if a knight is attacking square. There is no need to check all knights for double-check
    // since only one knight can be attacking the king at once
    private bool IsKnightAttackingSquare(ulong squarePosition, bool whiteToMove)
    {
        ulong knights;

        if (whiteToMove)
            knights = _currentBoard.BlackKnights;
        else
            knights = _currentBoard.WhiteKnights;

        var currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

        var possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

        var knightAttacks = possibleKnightMoves & knights;
        if (knightAttacks != 0)
        {
            _checkCount++;
            return true;
        }

        return false;
    }

    private bool IsCastlingPathAttacked(ulong path, bool whiteToMove)
    {
        //Calculate path positions
        var pathPositions = BitboardOperations.SplitBoardToArray(path);

        foreach (var position in pathPositions)
        {
            if (IsCastlingSquareAttacked(position, whiteToMove))
            {
                return true;
            }
        }

        return false;
    }

    // Checks if the given square is under attack on the current bitboard
    private bool IsCastlingSquareAttacked(ulong squarePosition, bool whiteToMove)
    {
        if (IsKnightAttackingSquare(squarePosition, whiteToMove))
        {
            return true;
        }

        if (IsPawnAttackingSquare(squarePosition, whiteToMove))
        {
            return true;
        }

        if (PieceChecking.IsSquareAttackedByKing(_currentBoard, squarePosition, whiteToMove))
        {
            return true;
        }

        if (whiteToMove)
        {
            if (PieceChecking.IsSquareRayAttackedFromAbove(_currentBoard, squarePosition) ||
                PieceChecking.IsSquareRayAttackedFromTheSide(_currentBoard, squarePosition)) //White castling king willnot be attacked from below so no need to check all squares
            {
                return true;
            }
        }
        else
        {
            if (PieceChecking.IsSquareRayAttackedFromBelow(_currentBoard, squarePosition) ||
                PieceChecking.IsSquareRayAttackedFromTheSide(_currentBoard, squarePosition)) //Black castling king will not be attacked from above so no need to check all squares
            {
                return true;
            }
        }

        return false;
    }
}

