using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngine
{
    internal sealed class PieceMover
    {
        private readonly Board _board;
        
        private const ulong FullBoard = ulong.MaxValue;

        internal PieceMover(Board board)
        {
            _board = board;
        }
        
        // If the move is confirmed extra processes are carried out (i.e. PGN calculations. During move generation these are not necessary and slow things down
        internal void MakeMove(PieceMove move, bool confirmedMove)
        {
            //MakeMove(move.Position, move.Moves, move.Type, false);
            MakeMove(move.Position, move.Moves, move.Type, move.SpecialMove, confirmedMove);
        }
        
        // If the move is confirmed extra processes are carried out (i.e. PGN calculations. During move generation these are not necessary and slow things down</param>
        internal void MakeMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove, bool confirmedMove)
        {
            SaveBoardState();            

            UpdateZobrist(moveFromBoard, moveToBoard, pieceToMove, specialMove);    //Do before everything else so we can check for capture etc 

            RemovePiece(moveFromBoard);
            
            var whiteToMove =_board.WhiteToMove; //This is mostly because I worry that constantly hitting a property will be slow
            
            RemovePiece(moveToBoard);       //Clear the square we're moving the piece to first

            //UpdateUsefulBitboardsForRemoval(moveFromBoard | moveToBoard);

            if (IsMovePromotion(specialMove))
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (specialMove)
                {
                    case SpecialMoveType.KnightPromotion:
                        PlacePiece(PieceType.Knight, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.KnightPromotionCapture:
                        PlacePiece(PieceType.Knight, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.BishopPromotion:
                        PlacePiece(PieceType.Bishop, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.BishopPromotionCapture:
                        PlacePiece(PieceType.Bishop, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.RookPromotion:
                        PlacePiece(PieceType.Rook, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.RookPromotionCapture:
                        PlacePiece(PieceType.Rook, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.QueenPromotion:
                        PlacePiece(PieceType.Queen, whiteToMove, moveToBoard);
                        break;
                    case SpecialMoveType.QueenPromotionCapture:
                        PlacePiece(PieceType.Queen, whiteToMove, moveToBoard);
                        break;
                }                
            }
            else
            {
                PlacePiece(pieceToMove, whiteToMove, moveToBoard);
            }
            
            CheckCastlingStatus(moveFromBoard, moveToBoard, pieceToMove);
            CheckEnPassantStatus(moveFromBoard, moveToBoard, pieceToMove);
                        
            _board.HalfMoveClock ++;

            if (!_board.WhiteToMove)
            {
                _board.FullMoveClock++;
            }
            
            _board.SwitchSides();

            CalculateUsefulBitboards();
        }
        
        public void UnMakeLastMove()
        {
            UnMakeLastMove(true);
        }

        public void UnMakeLastMove(bool verbose)
        {
            var state = _board.PopHistory(); 
            
            _board.WhiteToMove = state.WhiteToMove;

            _board.WhitePawns = state.WhitePawns;
            _board.WhiteKnights = state.WhiteKnights;
            _board.WhiteBishops = state.WhiteBishops;
            _board.WhiteRooks = state.WhiteRooks;
            _board.WhiteQueen = state.WhiteQueen;
            _board.WhiteKing = state.WhiteKing;

            _board.BlackPawns = state.BlackPawns;
            _board.BlackKnights = state.BlackKnights;
            _board.BlackBishops = state.BlackBishops;
            _board.BlackRooks = state.BlackRooks;
            _board.BlackQueen = state.BlackQueen;
            _board.BlackKing = state.BlackKing;

            _board.EnPassantPosition = state.EnPassantPosition;

            _board.WhiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            _board.WhiteCanCastleKingside = state.WhiteCanCastleKingside;
            _board.BlackCanCastleQueenside = state.BlackCanCastleQueenside;
            _board.BlackCanCastleKingside = state.BlackCanCastleKingside;

            _board.HalfMoveClock = state.HalfMoveClock;
            _board.FullMoveClock = state.FullMoveClock;

            //whiteInCheck = state.WhiteInCheck;
            //blackInCheck = state.BlackInCheck;

            //m_PgnMove = state.PgnMove;

            _board.Zobrist = state.ZobristKey;
            
            CalculateUsefulBitboards();
        } 
        
        // Adds the given bitboard value to the given piece type and colour bitboard
        // Note: No checks are done on squareToPlace. A correct value must be given or multiple 
        // pieces may be placed
        private void PlacePiece(PieceType typeToPlace, bool pieceIsWhite, ulong squareToPlace)
        {
            switch (typeToPlace)
            {
                case (PieceType.Pawn):
                    if (pieceIsWhite)
                    {
                        _board.WhitePawns |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackPawns |= squareToPlace;
                    }

                    break;
                case (PieceType.Knight):
                    if (pieceIsWhite)
                    {
                        _board.WhiteKnights |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackKnights |= squareToPlace;
                    }

                    break;
                case (PieceType.Bishop):
                    if (pieceIsWhite)
                    {
                        _board.WhiteBishops |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackBishops |= squareToPlace;
                    }

                    break;
                case (PieceType.Rook):
                    if (pieceIsWhite)
                    {
                        _board.WhiteRooks |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackRooks |= squareToPlace;
                    }

                    break;
                case (PieceType.Queen):
                    if (pieceIsWhite)
                    {
                        _board.WhiteQueen |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackQueen |= squareToPlace;
                    }

                    break;
                case (PieceType.King):
                    if (pieceIsWhite)
                    {
                        _board.WhiteKing |= squareToPlace;
                    }
                    else
                    {
                        _board.BlackKing |= squareToPlace;
                    }

                    break;
            }
        }
        
        // Removes the piece at the given square value
        // Note: No checks are done on squareToRemove. An actual value must be given or multiple pieces will be removed
        private void RemovePiece(ulong squareToClear)
        {
            var notSquareToClear = ~squareToClear;

            _board.WhitePawns &= notSquareToClear;
            _board.BlackPawns &= notSquareToClear;
            _board.WhiteKnights &= notSquareToClear;
            _board.BlackKnights &= notSquareToClear;
            _board.WhiteBishops &= notSquareToClear;
            _board.BlackBishops &= notSquareToClear;
            _board.WhiteRooks &= notSquareToClear;
            _board.BlackRooks &= notSquareToClear;
            _board.WhiteQueen &= notSquareToClear;
            _board.BlackQueen &= notSquareToClear;
            _board.WhiteKing &= notSquareToClear;
            _board.BlackKing &= notSquareToClear;
        }
        
        private void SaveBoardState()
        {
            var state = new BoardState
            {
                WhiteToMove = _board.WhiteToMove,
                WhitePawns = _board.WhitePawns,
                WhiteKnights = _board.WhiteKnights,
                WhiteBishops = _board.WhiteBishops,
                WhiteRooks = _board.WhiteRooks,
                WhiteQueen = _board.WhiteQueen,
                WhiteKing = _board.WhiteKing,
                BlackPawns = _board.BlackPawns,
                BlackKnights = _board.BlackKnights,
                BlackBishops = _board.BlackBishops,
                BlackRooks = _board.BlackRooks,
                BlackQueen = _board.BlackQueen,
                BlackKing = _board.BlackKing,
                EnPassantPosition = _board.EnPassantPosition,
                WhiteCanCastleQueenside = _board.WhiteCanCastleQueenside,
                WhiteCanCastleKingside = _board.WhiteCanCastleKingside,
                BlackCanCastleQueenside = _board.BlackCanCastleQueenside,
                BlackCanCastleKingside = _board.BlackCanCastleKingside,
                HalfMoveClock = _board.HalfMoveClock,
                FullMoveClock = _board.FullMoveClock,
                ZobristKey = _board.Zobrist
            };

            _board.AddToHistory(state);
        }
        
        private void UpdateZobrist(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            UpdateEnPassantZobrist(specialMove, moveToBoard);
            
            UpdateZobristForMove(moveFromBoard, moveToBoard, pieceToMove, specialMove);
        }
        
        private void UpdateEnPassantZobrist(SpecialMoveType specialMove, ulong moveToBoard)
        {
            var enPassantPosition = _board.PeekHistory();

            // Previous position
            if (enPassantPosition > 0)
            {
                _board.Zobrist ^= ZobristHash.HashEnPassantColumn(enPassantPosition);
            }

            //Current Position
            if (specialMove == SpecialMoveType.DoublePawnPush)
            {
                _board.Zobrist ^= ZobristHash.HashEnPassantColumn(moveToBoard);
#warning May have to change this to only add enpassant hash if there is a pawn which can capture
            }            
        }
        
        private void UpdateZobristForMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            //Remove old piece
            _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, _board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard)];
            
            if (IsMovePromotionCapture(specialMove))
            {
                //remove captured piece
                var capturedPiece = BoardChecking.GetPieceTypeOnSquare(_board, moveToBoard);
                _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, !_board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add promoted piece
                var promotedPiece = PromotedPiece(specialMove);
                _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, _board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if(IsMovePromotionNonCapture(specialMove))
            {
                //Add promoted piece
                var promotedPiece = PromotedPiece(specialMove);
                _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, _board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if (specialMove == SpecialMoveType.Capture)
            {
                //remove captured piece
                var capturedPiece = BoardChecking.GetPieceTypeOnSquare(_board, moveToBoard);
                _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, !_board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add moved piece
                _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, _board.WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                       
            }
            else if (IsMoveCastling(specialMove))
            {
                int movedPieceNumber;

                if(_board.WhiteToMove)
                    movedPieceNumber = ZobristHash.WHITE_KING;
                else
                    movedPieceNumber = ZobristHash.BLACK_KING;

                //Add moved piece
                _board.Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            
                //Add rook
                if ((moveToBoard & LookupTables.C1) > 0)
                {
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 0];
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 3];
                }
                else if ((moveToBoard & LookupTables.G1) > 0)
                {
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 7];
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 5];
                }
                else if ((moveToBoard & LookupTables.C8) > 0)
                {
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 56];
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 59];
                }
                else if ((moveToBoard & LookupTables.G8) > 0)
                {
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 63];
                    _board.Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 61];
                }
            }
            else if(specialMove == SpecialMoveType.ENPassantCapture)
            {
                int capturedPieceNumber;
                int movedPieceNumber;

                ulong capturedPawnBitboard;

                if (_board.WhiteToMove)
                {
                    movedPieceNumber = ZobristHash.WHITE_PAWN;
                    capturedPieceNumber = ZobristHash.BLACK_PAWN;
                    capturedPawnBitboard = _board.EnPassantPosition << 8;
                }
                else
                {
                    movedPieceNumber = ZobristHash.BLACK_PAWN;                    
                    capturedPieceNumber = ZobristHash.WHITE_PAWN;
                    capturedPawnBitboard = _board.EnPassantPosition >> 8;
                }

                //Remove captured pawn
                _board.Zobrist ^= ZobristKey.PiecePositions[capturedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(capturedPawnBitboard)];
                
                //Add moved pawn
                _board.Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                
            }
            else
            {
                var movedPieceNumber = ZobristHash.GetPieceValue(pieceToMove, _board.WhiteToMove);

                _board.Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];              
            }
        }
        
        private static bool IsMovePromotion(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotion || specialMove == SpecialMoveType.KnightPromotionCapture ||
                    specialMove == SpecialMoveType.BishopPromotion || specialMove == SpecialMoveType.BishopPromotionCapture ||
                    specialMove == SpecialMoveType.RookPromotion || specialMove == SpecialMoveType.RookPromotionCapture ||
                    specialMove == SpecialMoveType.QueenPromotion || specialMove == SpecialMoveType.QueenPromotionCapture);
        }
        
        private static bool IsMovePromotionCapture(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotionCapture ||
                    specialMove == SpecialMoveType.BishopPromotionCapture ||
                    specialMove == SpecialMoveType.RookPromotionCapture ||
                    specialMove == SpecialMoveType.QueenPromotionCapture);
        }
        
        private static bool IsMovePromotionNonCapture(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotion ||
                    specialMove == SpecialMoveType.BishopPromotion ||
                    specialMove == SpecialMoveType.RookPromotion ||
                    specialMove == SpecialMoveType.QueenPromotion);
        }
        
        private static bool IsMoveCastling(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KingCastle ||
                    specialMove == SpecialMoveType.QueenCastle);
        }
        
        private static PieceType PromotedPiece(SpecialMoveType specialMove)
        {
            switch (specialMove)
            {
                case SpecialMoveType.KnightPromotion:
                case SpecialMoveType.KnightPromotionCapture:
                    return PieceType.Knight;
                case SpecialMoveType.BishopPromotion:
                case SpecialMoveType.BishopPromotionCapture:
                    return PieceType.Bishop;
                case SpecialMoveType.RookPromotion:
                case SpecialMoveType.RookPromotionCapture:
                    return PieceType.Rook;
                case SpecialMoveType.QueenPromotion:
                case SpecialMoveType.QueenPromotionCapture:
                    return PieceType.Queen;
                default:
                    return PieceType.None;
            }
        }
        
        // Check if move has castled or altered the state of the castling flags
        private void CheckCastlingStatus(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            //Castling flag checks
            if (_board.WhiteToMove)
            {
                if (_board.WhiteCanCastleKingside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.G1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            var notSquareToClear = ~(ulong)128;   // h1
                            _board.WhiteRooks &= notSquareToClear;

                            PlacePiece(PieceType.Rook, true, LookupTables.F1);
                        }

                        _board.Zobrist ^= ZobristKey.WhiteCastleKingside;
                        _board.WhiteCanCastleKingside = false;     //Any king move means we can no longer castle
                    }

                    if (moveFromBoard == LookupTables.H1)     //Moved Rook
                    {
                        _board.Zobrist ^= ZobristKey.WhiteCastleKingside;
                        _board.WhiteCanCastleKingside = false;
                    }
                }

                if (_board.WhiteCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            var notSquareToClear = ~(ulong)1;   // a1
                            _board.WhiteRooks &= notSquareToClear;

                            PlacePiece(PieceType.Rook, true, LookupTables.D1);

                            _board.WhiteCanCastleQueenside = false;
                        }

                        _board.Zobrist ^= ZobristKey.WhiteCastleQueenside;
                        _board.WhiteCanCastleQueenside = false;
                    }

                    if (moveFromBoard == LookupTables.A1)     //Moved Rook
                    {
                        _board.Zobrist ^= ZobristKey.WhiteCastleQueenside;                        
                        _board.WhiteCanCastleQueenside = false;
                    }
                }
                
                if (_board.BlackCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H8)
                    {
                        _board.Zobrist ^= ZobristKey.BlackCastleKingside;                        
                        _board.BlackCanCastleKingside = false;
                    }
                }

                if (_board.BlackCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A8)
                    {
                        _board.Zobrist ^= ZobristKey.BlackCastleQueenside;
                        _board.BlackCanCastleQueenside = false;
                    }
                }
            }
            else
            {
                if (_board.BlackCanCastleKingside)
                {
                   if (pieceToMove == PieceType.King)
                   {
                        if (moveToBoard == LookupTables.G8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            var notSquareToClear = ~9223372036854775808;   // h8
                            _board.BlackRooks &= notSquareToClear;

                            PlacePiece(PieceType.Rook, false, LookupTables.F8);
                        }

                        _board.BlackCanCastleKingside = false; //Any king move means we can no longer castle

                        _board.Zobrist ^= ZobristKey.BlackCastleKingside;
                   }

                   if (moveFromBoard == LookupTables.H8)     //Moved Rook
                   {
                       _board.BlackCanCastleKingside = false;
                       _board.Zobrist ^= ZobristKey.BlackCastleKingside;
                   }
                }

                if (_board.BlackCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            var notSquareToClear = ~72057594037927936u;   // a8
                            _board.BlackRooks &= notSquareToClear;

                            PlacePiece(PieceType.Rook, false, LookupTables.D8);
                        }

                        _board.BlackCanCastleQueenside = false;
                        _board.Zobrist ^= ZobristKey.BlackCastleQueenside;
                    }

                    if (moveFromBoard == LookupTables.A8)     //Moved Rook
                    {
                        _board.BlackCanCastleQueenside = false;
                        _board.Zobrist ^= ZobristKey.BlackCastleQueenside;
                    }
                }

                //Check if black has captured a white rook
                if (_board.WhiteCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H1)
                    {
                        _board.WhiteCanCastleKingside = false;
                        _board.Zobrist ^= ZobristKey.WhiteCastleKingside;
                    }
                }

                if (_board.WhiteCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A1)
                    {
                        _board.WhiteCanCastleQueenside = false;
                        _board.Zobrist ^= ZobristKey.WhiteCastleQueenside;
                    }
                }
            }  
        }
        
        // Checks if an en passant capture was made or if the flags have to be updated
        private void CheckEnPassantStatus(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            if (pieceToMove == PieceType.Pawn)
            {
                if (_board.WhiteToMove)
                {

                    if ((moveToBoard & _board.EnPassantPosition) != 0) //Move is an en-passant capture  
                    {
                        //Remove captured piece
                        var notSquareToClear = ~(_board.EnPassantPosition >> 8);
                        _board.BlackPawns &= notSquareToClear;
                        
                        _board.EnPassantPosition = 0;
                        return;
                    }

                    var differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)
                                                   - BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard);

                    if(differenceInMoveIndex == 16)
                    {
                        _board.EnPassantPosition = moveToBoard >> 8;
                    }
                    else
                    {
                        _board.EnPassantPosition = 0;
                    }
                }
                else
                {
                    if ((moveToBoard & _board.EnPassantPosition) != 0) //Move is an en-passant capture
                    {
                        //Remove captured piece
                        var notSquareToClear = ~(_board.EnPassantPosition << 8);
                        _board.WhitePawns &= notSquareToClear;

                        _board.EnPassantPosition = 0;

                        return;
                    }

                    var differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard)
                                                - BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard);

                    if (differenceInMoveIndex == 16)
                    {
                        _board.EnPassantPosition = moveToBoard << 8;
                    }
                    else
                    {
                        _board.EnPassantPosition = 0;
                    }
                }
            }
            else
                _board.EnPassantPosition = 0;
        }
        
        //Gets called every time a move is made to update all useful boards
        private void CalculateUsefulBitboards()
        {
            _board.WhiteNonEndGamePieces = _board.WhiteKnights | _board.WhiteBishops | _board.WhiteRooks | _board.WhiteQueen;
            _board.BlackNonEndGamePieces = _board.BlackKnights | _board.BlackBishops | _board.BlackRooks | _board.BlackQueen;

            _board.AllWhiteOccupiedSquares = _board.WhitePawns | _board.WhiteNonEndGamePieces | _board.WhiteKing;
            _board.AllBlackOccupiedSquares = _board.BlackPawns | _board.BlackNonEndGamePieces | _board.BlackKing;
            _board.AllOccupiedSquares      = _board.AllWhiteOccupiedSquares | _board.AllBlackOccupiedSquares;
            _board.EmptySquares            = _board.AllOccupiedSquares ^ FullBoard;
            _board.WhiteOrEmpty            = _board.AllWhiteOccupiedSquares | _board.EmptySquares;
            _board.BlackOrEmpty            = _board.AllBlackOccupiedSquares | _board.EmptySquares;
        }
    }
}
