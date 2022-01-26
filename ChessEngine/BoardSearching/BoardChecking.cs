using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.Exceptions;
using ChessEngine.PossibleMoves;

namespace ChessEngine.BoardSearching
{
    // Checks carried out on the chess board regarding various moves and attacks
    internal static class BoardChecking
    {
        internal static bool IsPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                if ((board.AllOccupiedSquares & square) != 0) return true; 
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }
        
        internal static bool IsFriendlyPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var friendlySquares = 
                    board.WhiteToMove ? board.AllWhiteOccupiedSquares : board.AllBlackOccupiedSquares;

                if ((friendlySquares & square) != 0)
                {
                    return true;
                }
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }
        
        internal static bool IsEnemyPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var enemySquares = 
                    board.WhiteToMove ? board.AllBlackOccupiedSquares : board.AllWhiteOccupiedSquares;

                if ((enemySquares & square) != 0)
                {
                    return true;
                }
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }

        internal static PieceType GetPieceTypeOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) != 1)
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            if (((board.WhitePawns | board.BlackPawns) & square) != 0) return PieceType.Pawn;

            if (((board.WhiteKnights | board.BlackKnights) & square) != 0) return PieceType.Knight;
            
            if (((board.WhiteBishops | board.BlackBishops) & square) != 0) return PieceType.Bishop;

            if (((board.WhiteRooks | board.BlackRooks) & square) != 0) return PieceType.Rook;

            if (((board.WhiteQueen | board.BlackQueen) & square) != 0) return PieceType.Queen;

            if (((board.WhiteKing | board.BlackKing) & square) != 0) return PieceType.King;

            return PieceType.None;
        }

        internal static SpecialMoveType GetSpecialMoveType(Board board, 
                                                           ulong moveFrom,
                                                           ulong moveTo, 
                                                           string uciMove)
        {
            var captureFlag = false;
            var promotionFlag = false;
            
            if (IsEnemyPieceOnSquare(board, moveTo))
            {
                captureFlag = true;
            }

            if (uciMove.Length > 4)
            {
                promotionFlag = true;
            }

            if (captureFlag)
            {
                return promotionFlag ? GetPromotionCaptureForType(uciMove[4]) : SpecialMoveType.Capture;
            }
            else
            {
                if (promotionFlag)
                {
                    return GetPromotionForType(uciMove[4]);
                }
            }

            var moveFromPiece = GetPieceTypeOnSquare(board, moveFrom);

            if (moveFromPiece == PieceType.King)
            { 
                if (moveFrom == LookupTables.E1)
                {
                    if (moveTo == LookupTables.G1)
                    {
                        return SpecialMoveType.KingCastle;
                    }
                    
                    if (moveTo == LookupTables.C1)
                    {
                        return SpecialMoveType.QueenCastle;
                    }
                }
                else if (moveFrom == LookupTables.E8)
                {
                    if (moveTo == LookupTables.G8)
                    {
                        return SpecialMoveType.KingCastle;
                    }
                    
                    if (moveTo == LookupTables.C8)
                    {
                        return SpecialMoveType.QueenCastle;
                    }
                }                
            }
            
            if (moveFromPiece == PieceType.Pawn)
            {
                if (moveFrom << 16 == moveTo)
                {
                    return SpecialMoveType.DoublePawnPush;
                }

                if (board.EnPassantPosition != 0)
                {
                    if(board.EnPassantPosition == moveTo)   //pawn is moving to en passant position
                    {
                        //pawn did a capture move and we already know there was no standard capture
                        if (moveFrom << 7 == moveTo || moveFrom << 9 == moveTo) 
                        {
                            return SpecialMoveType.ENPassantCapture;
                        }
                    }
                }
            }

            return SpecialMoveType.Normal;
        }

        private static SpecialMoveType GetPromotionCaptureForType(char pieceType)
        {
            switch (pieceType)
            {
                case 'r':
                case 'R':
                    return SpecialMoveType.RookPromotionCapture;
                case 'n':
                case 'N':
                    return SpecialMoveType.KnightPromotionCapture;
                case 'b':
                case 'B':
                    return SpecialMoveType.BishopPromotionCapture;
                case 'q':
                case 'Q':
                    return SpecialMoveType.QueenPromotionCapture;
                default:
                    return SpecialMoveType.Normal;

            }
        }

        private static SpecialMoveType GetPromotionForType(char pieceType)
        {
            switch (pieceType)
            {
                case 'r':
                case 'R':
                    return SpecialMoveType.RookPromotion;
                case 'n':
                case 'N':
                    return SpecialMoveType.KnightPromotion;
                case 'b':
                case 'B':
                    return SpecialMoveType.BishopPromotion;
                case 'q':
                case 'Q':
                    return SpecialMoveType.QueenPromotion;
                default:
                    return SpecialMoveType.Normal;

            }
        }
        
        // Checks if the square is attacked from a ray attack above.
        // Used to find if white king will be in check when castling
        // Ray attacks are from bishops, rooks and queens
        //
        internal static bool IsSquareRayAttackedFromAbove(Board board, ulong square)
        {
            // Up
            var nearestUpPiece = FindUpBlockingPosition(board, square);

            if ((nearestUpPiece & board.BlackRooks) > 0 || (nearestUpPiece & board.BlackQueen) > 0)
                return true;

            // Up-right
            var nearestUpRightPiece = FindUpRightBlockingPosition(board, square);

            if ((nearestUpRightPiece & board.BlackBishops) > 0 || (nearestUpRightPiece & board.BlackQueen) > 0)
                return true;

            // Up Left
            var nearestUpLeftPiece = FindUpLeftBlockingPosition(board, square);

            return (nearestUpLeftPiece & board.BlackBishops) > 0
                   || (nearestUpLeftPiece & board.BlackQueen) > 0;
        }
        
        // Checks if the square is attacked from a ray attack below. Used to find if black king will be in check when castling
        // Ray attacks are from bishops, rooks and queens
        //
        internal static bool IsSquareRayAttackedFromBelow(Board board, ulong square)
        {
            //Down
            var nearestDownPiece = FindDownBlockingPosition(board, square);

            if ((nearestDownPiece & board.WhiteRooks) > 0 || (nearestDownPiece & board.WhiteQueen) > 0)
                return true;

            //Down-right
            var nearestDownRightPiece = FindDownRightBlockingPosition(board, square);

            if ((nearestDownRightPiece & board.WhiteBishops) > 0 || (nearestDownRightPiece & board.WhiteQueen) > 0)
            {
                return true;
            }

            //Up Left
            var nearestDownLeftPiece = FindDownLeftBlockingPosition(board, square);

            if ((nearestDownLeftPiece & board.WhiteBishops) > 0 || (nearestDownLeftPiece & board.WhiteQueen) > 0)
            {
                return true;
            }
            
            return false;
        }

        internal static bool IsSquareRayAttackedFromTheSide(Board board, ulong square)
        {
            if (board.WhiteToMove)
            {
                var nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.BlackRooks) > 0 || (nearestLeftPiece & board.BlackQueen) > 0)
                {
                    return true;
                }

                var nearestRightPiece = FindRightBlockingPosition(board, square);

                if ((nearestRightPiece & board.BlackRooks) > 0 || (nearestRightPiece & board.BlackQueen) > 0)
                {
                    return true;
                }
            }
            else
            {
                var nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.WhiteRooks) > 0 || (nearestLeftPiece & board.WhiteQueen) > 0)
                {
                    return true;
                }

                var nearestRightPiece = FindRightBlockingPosition(board, square);

                if ((nearestRightPiece & board.WhiteRooks) > 0 || (nearestRightPiece & board.WhiteQueen) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // Returns true or false whether king is in check. If we do not need to know the number
        // of checks or who/where is checking king use this over IsKingInCheck since it returns
        // true as soon as it knows
        //
        internal static bool IsKingInCheck(Board board, bool whitePieces)
        {
            ulong friendlyKing;

            if (whitePieces)
            {
                friendlyKing = board.WhiteKing;
            }
            else
            {
                friendlyKing = board.BlackKing;
            }
            
            return IsSquareAttacked(board, friendlyKing, whitePieces);
        }

        // Checks if the king has any flight squares
        internal static bool CanKingMove(Board boardPosition, bool whitePieces)
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.WhiteKing);
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.BlackKing);

            if(whitePieces)
            {
                var possibleMoves = 
                    ValidMoveArrays.KingMoves[whiteKingPosition] & ~ValidMoveArrays.KingMoves[blackKingPosition];

                // Even if a black piece is on the square the king can go there if the square is not
                // under attack (i.e. the piece is not protected)
                var freeSquares = possibleMoves & ~boardPosition.AllWhiteOccupiedSquares; 

                var storedAllOccupiedSquares = boardPosition.AllOccupiedSquares;
                var storedAllOccupiedWhiteSquares = boardPosition.AllWhiteOccupiedSquares;

                if (freeSquares > 0)
                {
                    // We need to temporarily remove the king to make sure his current place isn't blocking an attack
                    boardPosition.AllOccupiedSquares &= ~boardPosition.WhiteKing;
                    boardPosition.AllWhiteOccupiedSquares &= ~boardPosition.WhiteKing;

                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    foreach (var possMove in possMoves)
                    {
                        if (!IsSquareAttacked(boardPosition, possMove, whitePieces))
                        {
                            boardPosition.AllOccupiedSquares = storedAllOccupiedSquares;
                            boardPosition.AllWhiteOccupiedSquares = storedAllOccupiedWhiteSquares;
                            return true;
                        }
                    }
                }

                boardPosition.AllOccupiedSquares = storedAllOccupiedSquares;
                boardPosition.AllWhiteOccupiedSquares = storedAllOccupiedWhiteSquares;
                return false;
            }
            else
            {
                var possibleMoves = 
                    ValidMoveArrays.KingMoves[blackKingPosition] & ~ValidMoveArrays.KingMoves[whiteKingPosition];
                
                // Even if a white piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)
                //
                var freeSquares = 
                    possibleMoves & ~boardPosition.AllBlackOccupiedSquares; 

                var storedAllOccupiedSquares = boardPosition.AllOccupiedSquares;
                var storedAllOccupiedBlackSquares = boardPosition.AllBlackOccupiedSquares;

                if (freeSquares > 0)
                {
                    boardPosition.AllOccupiedSquares &= ~boardPosition.BlackKing;
                    boardPosition.AllBlackOccupiedSquares &= ~boardPosition.BlackKing;

                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    foreach (var t in possMoves)
                    {
                        if (!IsSquareAttacked(boardPosition, t, whitePieces))
                        {
                            boardPosition.AllOccupiedSquares = storedAllOccupiedSquares;
                            boardPosition.AllBlackOccupiedSquares = storedAllOccupiedBlackSquares;
                            return true;
                        }
                    }
                }

                boardPosition.AllOccupiedSquares = storedAllOccupiedSquares;
                boardPosition.AllBlackOccupiedSquares = storedAllOccupiedBlackSquares;
                return false;
            }
        }
        
        // Checks all points from this piece to see if it is being attacked
        // If it is it returns true straight away (i.e. We don't know how many pieces it is being attacked by)
        // 
        // NOTE: To save time it is assumed that the pieceBoard has exactly 1 piece on it. If not, it may not behave as expecte
        //
        private static bool IsSquareAttacked(Board board, ulong pieceBoard, bool whitePieces)
        {
             if (IsKnightAttackingSquare(board, pieceBoard, whitePieces))
             {
                return true;
             }

             // Check if piece is surrounded by friends. If so, we only need to worry about 
             // knights so we can assume false
             var surroundingSpace = GetSurroundingSpace(pieceBoard);

             ulong emptyOrEnemyNeighbours;

             if (whitePieces)
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllWhiteOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                 {
                     return false;
                 }
             }
             else
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllBlackOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                 {
                     return false;
                 }
             }

             if (IsPawnAttackingSquareFast(board, pieceBoard, whitePieces))
             {
                 return true;
             }
            
             if (IsSquareAttackedByKing(board, pieceBoard, whitePieces))
             {
                 return true;
             }             
            
             if (IsSquareUnderRayAttackSuperFast(board, pieceBoard, emptyOrEnemyNeighbours, whitePieces))
             {
                 return true;
             }


             return false;
        }
        
        // Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        // since only one pawn can be attacking the king at once
        internal static bool IsPawnAttackingSquareFast(Board board, ulong squarePosition, bool whitePieces)
        {
            var squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            
            // Allows the quick masking of wrapping checks
            //
            var proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      

            if (whitePieces)
            {
                if(board.BlackPawns == 0)
                {
                    return false;
                }
                
                // heck up-right    
                var upRight = squarePosition << 9;

                if ((upRight & board.BlackPawns & proximityBoard) != 0)
                {
                    return true;
                }

                //Check up-left
                var upLeft = squarePosition << 7;

                if ((upLeft & board.BlackPawns & proximityBoard) != 0)
                {
                    return true;
                }
            }
            else
            {
                if (board.WhitePawns == 0)
                {
                    return false;
                }

                //Check down-right
                var downRight = squarePosition >> 7;

                if ((downRight & board.WhitePawns & proximityBoard) != 0)
                {
                    return true;
                }

                //Check down-left
                var upLeft = squarePosition >> 9;

                if ((upLeft & board.WhitePawns & proximityBoard) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a knight is attacking square. There is no need to check all knights for double-check 
        /// since only one knight can be attacking the king at once
        /// </summary>
        internal static bool IsKnightAttackingSquare(Board board, ulong squarePosition, bool whitePieces)
        {
            ulong knights;

            knights = whitePieces ? board.BlackKnights : board.WhiteKnights;
              
            // If there are no knights we do not have to check  
            //
            if (knights == 0)   
            {
                return false;
            }

            var currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

            var possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

            var knightAttacks = possibleKnightMoves & knights;
            
            if (knightAttacks != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsSquareAttackedByKing(Board board, ulong squarePosition, bool whitePieces)
        {
            ulong enemyKing;

            if (whitePieces)
            {
                enemyKing = board.BlackKing;
            }
            else
            {
                enemyKing = board.WhiteKing;
            }

            var checkSquare = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            var surroundBoard = ValidMoveArrays.KingMoves[checkSquare];

            if ((enemyKing & surroundBoard) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsSquareUnderRayAttackSuperFast(Board board, ulong squarePositionBoard, ulong emptyOrEnemySpaces, bool whitePieces)
        {
            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (whitePieces)
            {
                enemyQueenSquares = board.BlackQueen;
                enemyBishopSquares = board.BlackBishops;
                enemyRookSquares = board.BlackRooks;
            }
            else
            {
                enemyQueenSquares = board.WhiteQueen;
                enemyBishopSquares = board.WhiteBishops;
                enemyRookSquares = board.WhiteRooks;
            }
                       
            if (enemyQueenSquares != 0 || enemyRookSquares != 0)
            {
                var leftBoard = squarePositionBoard >> 1;
                var checkLeft = (leftBoard & emptyOrEnemySpaces) > 0;

                if(checkLeft)
                {
                    var nearestLeftPiece = FindLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var rightBoard = squarePositionBoard << 1;
                var checkRight = (rightBoard & emptyOrEnemySpaces) > 0;
                
                if(checkRight)
                {
                    var nearestRightPiece = FindRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var upBoard = squarePositionBoard << 8;
                var checkUp = (upBoard & emptyOrEnemySpaces) > 0;

                if(checkUp)
                {
                    var nearestUpPiece = FindUpBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downBoard = squarePositionBoard >> 8;
                var checkDown = (downBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDown)
                {
                    var nearestDownPiece = FindDownBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }
            }            

            if (enemyQueenSquares != 0 || enemyBishopSquares != 0)
            {
                var upRightBoard = squarePositionBoard << 9;
                var checkUpRight = (upRightBoard & emptyOrEnemySpaces) > 0;

                if (checkUpRight)
                {
                    var nearestUpRightPiece = FindUpRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var upLeftBoard = squarePositionBoard << 7;
                var checkUpLeft = (upLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkUpLeft)
                {
                    var nearestUpLeftPiece = FindUpLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downRightBoard = squarePositionBoard >> 7;
                var checkDownRight = (downRightBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownRight)
                {
                    var nearestDownRightPiece = FindDownRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downLeftBoard = squarePositionBoard >> 9;
                var checkDownLeft = (downLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownLeft)
                {
                    var nearestDownLeftPiece = FindDownLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static ulong CalculateAllowedBishopMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(board, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedBishopMoves(Board board, ulong square, bool whiteToMove)
        {
            return (CalculateAllowedUpRightMoves(board, square, whiteToMove) |
                    CalculateAllowedDownRightMoves(board, square, whiteToMove) |
                    CalculateAllowedDownLeftMoves(board, square, whiteToMove) |
                    CalculateAllowedUpLeftMoves(board, square, whiteToMove));
        }

        internal static ulong CalculateAllowedRookMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(board, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedRookMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(board, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedQueenMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedUpRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(board, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedQueenMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedUpRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(board, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(board, pieceIndex, whiteToMove));
        }

        private static ulong CalculateAllowedUpMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var upBoard = LookupTables.UpBoard[pieceIndex];

            return CalculateAllowedUpMovesFromBoard(board,
                                                    upBoard,
                                                    whiteToMove);
        }

        private static ulong CalculateAllowedUpMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            var upBoard = GetUpBoard(pieceIndex);

            return CalculateAllowedUpMovesFromBoard(board,
                                                    upBoard,
                                                    whiteToMove);
        }

        private static ulong CalculateAllowedUpMovesFromBoard(Board board, 
                                                              ulong upBoard, 
                                                              bool whiteToMove)
        {
            var upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square

            upMoves = (upMoves << 8) 
                      | (upMoves << 16) 
                      | (upMoves << 24) 
                      | (upMoves << 32) 
                      | (upMoves << 40) 
                      | (upMoves << 48);  //Fill all squares above by performing left shifts
            
            upMoves ^= upBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                upMoves &= board.BlackOrEmpty;
            }
            else
            {
                upMoves &= board.WhiteOrEmpty;
            }

            return upMoves;
        }

        private static ulong CalculateAllowedRightMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var rightBoard = LookupTables.RightBoard[pieceIndex];

            return CalculateAllowedRightMovesFromBoard(board,
                                                       rightBoard,
                                                       whiteToMove);
        }

        private static ulong CalculateAllowedRightMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            var rightBoard = GetRightBoard(pieceIndex);

            return CalculateAllowedRightMovesFromBoard(board,
                                                       rightBoard,
                                                       whiteToMove);
        }

        private static ulong CalculateAllowedRightMovesFromBoard(Board board, 
                                                                      ulong rightBoard, 
                                                                      bool whiteToMove)
        {
            var rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square

            rightMoves = (rightMoves << 1) 
                         | (rightMoves << 2) 
                         | (rightMoves << 3) 
                         | (rightMoves << 4) 
                         | (rightMoves << 5) 
                         | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves &= rightBoard;       //Remove overflow

            rightMoves ^= rightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
                rightMoves &= board.BlackOrEmpty;
            else
                rightMoves &= board.WhiteOrEmpty;

            return rightMoves;
        }

        private static ulong CalculateAllowedDownMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var downBoard = LookupTables.DownBoard[pieceIndex];

            return CalculateAllowedDownMovesFromBoard(board,
                                                      downBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedDownMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            var downBoard = GetDownBoard(pieceIndex);

            return CalculateAllowedDownMovesFromBoard(board,
                                                      downBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedDownMovesFromBoard(Board board,
                                                                ulong downBoard,
                                                                bool whiteToMove)
        {
            var downMoves = downBoard & board.AllOccupiedSquares; //Find first hit square

            downMoves = (downMoves >> 8) 
                        | (downMoves >> 16) 
                        | (downMoves >> 24) 
                        | (downMoves >> 32) 
                        | (downMoves >> 40) 
                        | (downMoves >> 48); //Fill all squares below by performing right shifts

            downMoves ^= downBoard; //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                downMoves &= board.BlackOrEmpty;
            }
            else
            {
                downMoves &= board.WhiteOrEmpty;
            }

            return downMoves;
        }

        private static ulong CalculateAllowedLeftMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var leftBoard = LookupTables.LeftBoard[pieceIndex];

            return CalculateAllowedLeftMovesFromBoard(board,
                                                          leftBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedLeftMoves(Board board, ulong pieceIndex, bool whiteToMove)
        {
            var leftBoard = GetLeftBoard(pieceIndex);

            return CalculateAllowedLeftMovesFromBoard(board,
                                                          leftBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedLeftMovesFromBoard(Board board,
                                                                    ulong leftBoard,
                                                                    bool whiteToMove)
        {
            var leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square

            leftMoves = (leftMoves >> 1) 
                        | (leftMoves >> 2) 
                        | (leftMoves >> 3) 
                        | (leftMoves >> 4) 
                        | (leftMoves >> 5) 
                        | (leftMoves >> 6);  //Fill all squares to the left by performing right shifts

            leftMoves &= leftBoard;       //Remove overflow

            leftMoves ^= leftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                leftMoves &= board.BlackOrEmpty;
            }
            else
            {
                leftMoves &= board.WhiteOrEmpty;
            }

            return leftMoves;
        }

        private static ulong CalculateAllowedUpRightMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var upRightBoard = LookupTables.UpRightBoard[pieceIndex];

            return CalculateAllowedUpRightMovesFromBoard(board,
                                                          upRightBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedUpRightMoves(Board board, ulong piecePosition, bool whiteToMove)
        {
            var upRightBoard = GetUpRightBoard(piecePosition);

            return CalculateAllowedUpRightMovesFromBoard(board,
                                                         upRightBoard,
                                                         whiteToMove);
        }

        private static ulong CalculateAllowedUpRightMovesFromBoard(Board board,
                                                                    ulong upRightBoard,
                                                                    bool whiteToMove)
        {
            var upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square

            upRightMoves = (upRightMoves << 9) 
                           | (upRightMoves << 18) 
                           | (upRightMoves << 27) 
                           | (upRightMoves << 36) 
                           | (upRightMoves << 45) 
                           | (upRightMoves << 54);  //Fill all squares up and right by performing left shifts

            upRightMoves &= upRightBoard;       //Remove overflow

            upRightMoves ^= upRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                upRightMoves &= board.BlackOrEmpty;
            }
            else
            {
                upRightMoves &= board.WhiteOrEmpty;
            }

            return upRightMoves;
        }

        private static ulong CalculateAllowedDownRightMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var downRightBoard = LookupTables.DownRightBoard[pieceIndex];

            return CalculateAllowedDownRightMovesFromBoard(board,
                                                           downRightBoard,
                                                           whiteToMove);
        }

        private static ulong CalculateAllowedDownRightMoves(Board board, ulong piecePosition, bool whiteToMove)
        {
            var downRightBoard = GetDownRightBoard(piecePosition);

            return CalculateAllowedDownRightMovesFromBoard(board,
                                                           downRightBoard,
                                                           whiteToMove);
        }

        private static ulong CalculateAllowedDownRightMovesFromBoard(Board board,
                                                                   ulong downRightBoard,
                                                                   bool whiteToMove)
        {
            var downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square

            downRightMoves = (downRightMoves >> 7) 
                             | (downRightMoves >> 14) 
                             | (downRightMoves >> 21) 
                             | (downRightMoves >> 28) 
                             | (downRightMoves >> 35) 
                             | (downRightMoves >> 42);  //Fill all squares down and right by performing left shifts

            downRightMoves &= downRightBoard;       //Remove overflow

            downRightMoves ^= downRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                downRightMoves &= board.BlackOrEmpty;
            }
            else
            {
                downRightMoves &= board.WhiteOrEmpty;
            }

            return downRightMoves;
        }

        private static ulong CalculateAllowedDownLeftMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var downLeftBoard = LookupTables.DownLeftBoard[pieceIndex];

            return CalculateAllowedDownLeftMovesFromBoard(board,
                                                           downLeftBoard,
                                                           whiteToMove);
        }

        private static ulong CalculateAllowedDownLeftMoves(Board board, ulong piecePosition, bool whiteToMove)
        {
            //ulong downLeftBoard = LookupTables.DownLeftBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            var downLeftBoard = GetDownLeftBoard(piecePosition);

            return CalculateAllowedDownLeftMovesFromBoard(board,
                                                          downLeftBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedDownLeftMovesFromBoard(Board board,
                                                                    ulong downLeftBoard,
                                                                    bool whiteToMove)
        {
            var downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) 
                            | (downLeftMoves >> 18) 
                            | (downLeftMoves >> 27) 
                            | (downLeftMoves >> 36) 
                            | (downLeftMoves >> 45)     // Fill all squares down and
                            | (downLeftMoves >> 54);    // left by performing right shifts

            downLeftMoves &= downLeftBoard;       //Remove overflow

            downLeftMoves ^= downLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                downLeftMoves &= board.BlackOrEmpty;
            }
            else
            {
                downLeftMoves &= board.WhiteOrEmpty;
            }

            return downLeftMoves;
        }

        private static ulong CalculateAllowedUpLeftMoves(Board board, byte pieceIndex, bool whiteToMove)
        {
            var upLeftBoard = LookupTables.UpLeftBoard[pieceIndex];

            return CalculateAllowedUpLeftMovesFromBoard(board,
                                                        upLeftBoard,
                                                        whiteToMove);
        }

        private static ulong CalculateAllowedUpLeftMoves(Board board, ulong piecePosition, bool whiteToMove)
        {
            var upLeftBoard = GetUpLeftBoard(piecePosition);

            return CalculateAllowedUpLeftMovesFromBoard(board,
                                                        upLeftBoard,
                                                        whiteToMove);
        }

        private static ulong CalculateAllowedUpLeftMovesFromBoard(Board board,
                                                                  ulong upLeftBoard,
                                                                  bool whiteToMove)
        {
            var upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square

            upLeftMoves = (upLeftMoves << 7) 
                          | (upLeftMoves << 14) 
                          | (upLeftMoves << 21) 
                          | (upLeftMoves << 28) 
                          | (upLeftMoves << 35)   //Fill all squares up and 
                          | (upLeftMoves << 42);  // left by performing right shifts

            upLeftMoves &= upLeftBoard;       //Remove overflow

            upLeftMoves ^= upLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (whiteToMove)
            {
                upLeftMoves &= board.BlackOrEmpty;
            }
            else
            {
                upLeftMoves &= board.WhiteOrEmpty;
            }

            return upLeftMoves;
        }

        // Returns the bitboard value of the first piece, of any colour, up from the given squarePosition
        public static ulong FindUpBlockingPosition(Board board, ulong square)
        {
            var upBoard = GetUpBoard(square);

            var upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square

            upMoves = (upMoves << 8) 
                      | (upMoves << 16) 
                      | (upMoves << 24) 
                      | (upMoves << 32) 
                      | (upMoves << 40) 
                      | (upMoves << 48);  //Fill all squares above by performing left shifts
            
            upMoves ^= upBoard;        //Just allowed squares

            return upMoves & board.AllOccupiedSquares;
        }

        public static ulong FindUpRightBlockingPosition(Board board, ulong square)
        {
            var upRightBoard = GetUpRightBoard(square);

            var upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square

            upRightMoves = (upRightMoves << 9) 
                           | (upRightMoves << 18) 
                           | (upRightMoves << 27) 
                           | (upRightMoves << 36) 
                           | (upRightMoves << 45)       //Fill all squares above and
                           | (upRightMoves << 54);      // right by performing left shifts

            upRightMoves &= upRightBoard;       //Remove overflow

            var notAboveRight = upRightMoves ^ upRightBoard;

            return notAboveRight & board.AllOccupiedSquares;
        }

        public static ulong FindRightBlockingPosition(Board board, ulong square)
        {
            var rightBoard = GetRightBoard(square);
            
            var rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square

            rightMoves = (rightMoves << 1) 
                         | (rightMoves << 2) 
                         | (rightMoves << 3) 
                         | (rightMoves << 4) 
                         | (rightMoves << 5) 
                         | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves &= rightBoard;       //Remove overflow

            var notRight = rightMoves ^ rightBoard;

            return notRight & board.AllOccupiedSquares;
        }

        public static ulong FindDownRightBlockingPosition(Board board, ulong square)
        {
            var downRightBoard = GetDownRightBoard(square);
            
            var downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square

            downRightMoves = (downRightMoves >> 7) 
                             | (downRightMoves >> 14) 
                             | (downRightMoves >> 21) 
                             | (downRightMoves >> 28) 
                             | (downRightMoves >> 35)   //Fill all squares below-right
                             | (downRightMoves >> 42);  // by performing right shifts

            downRightMoves &= downRightBoard;       //Remove overflow

            var notBelowRight = downRightMoves ^ downRightBoard;

            return notBelowRight & board.AllOccupiedSquares;
        }

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, down from the given square
        /// </summary>
        public static ulong FindDownBlockingPosition(Board board, ulong square)
        {
            var downBoard = GetDownBoard(square);

            var downMoves = downBoard & board.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts

            //downMoves = downMoves & downBoard;       //Remove overflow

            var notBelow = downMoves ^ downBoard;
            return notBelow & board.AllOccupiedSquares;
        }

        public static ulong FindDownLeftBlockingPosition(Board board, ulong square)
        {
            var downLeftBoard = GetDownLeftBoard(square);

            var downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below-left by performing right shifts

            downLeftMoves &= downLeftBoard;       //Remove overflow

            var notBelowRight = downLeftMoves ^ downLeftBoard;
            return notBelowRight & board.AllOccupiedSquares;
        }

        internal static ulong FindLeftBlockingPosition(Board board, ulong square)
        {
            var leftBoard = GetLeftBoard(square);

            var leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the right by performing left shifts

            leftMoves &= leftBoard;       //Remove overflow

            var notLeft = leftMoves ^ leftBoard;

            return notLeft & board.AllOccupiedSquares;
        }

        internal static ulong FindUpLeftBlockingPosition(Board board, ulong square)
        {
            var upLeftBoard = GetUpLeftBoard(square);

            var upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square

            upLeftMoves = (upLeftMoves << 7) 
                          | (upLeftMoves << 14) 
                          | (upLeftMoves << 21) 
                          | (upLeftMoves << 28) 
                          | (upLeftMoves << 35) 
                          | (upLeftMoves << 42);  //Fill all squares up-left by performing left shifts

            upLeftMoves &= upLeftBoard;       //Remove overflow

            var notAboveLeft = upLeftMoves ^ upLeftBoard;

            return notAboveLeft & board.AllOccupiedSquares;
        }

        private static ulong GetSurroundingSpace(ulong pieceBoard)
        {
            const ulong notA = 18374403900871474942; //All squares except A column
            const ulong notH = 9187201950435737471; //All squares except H column
            
            return ((pieceBoard << 7) & notH) | (pieceBoard << 8) | ((pieceBoard << 9) & notA) |
                   ((pieceBoard >> 1) & notH) |                     ((pieceBoard << 1) & notA) |
                   ((pieceBoard >> 9) & notH) | (pieceBoard >> 8) | ((pieceBoard >> 7) & notA);
        }

        private static ulong GetUpRightBoard(ulong square)
        {
            const ulong notA = 18374403900871474942;

            var shift1 = (square << 9) & notA;
            var shift2 = shift1 | ((shift1 << 9) & notA);
            var shift3 = shift2 | ((shift2 << 9) & notA);
            var shift4 = shift3 | ((shift3 << 9) & notA);
            var shift5 = shift4 | ((shift4 << 9) & notA);
            var shift6 = shift5 | ((shift5 << 9) & notA);
            var shift7 = shift6 | ((shift6 << 9) & notA);

            return shift7;            
        }

        private static ulong GetUpBoard(ulong square)
        {
            return (square << 8) 
                    | (square << 16) 
                    | (square << 24) 
                    | (square << 32) 
                    | (square << 40) 
                    | (square << 48) 
                    | (square << 56);
        }

        private static ulong GetRightBoard(ulong square)
        {
            const ulong notA = 18374403900871474942;

            var shift1 = (square << 1) & notA;
            var shift2 = shift1 | ((shift1 << 1) & notA);
            var shift3 = shift2 | ((shift2 << 1) & notA);
            var shift4 = shift3 | ((shift3 << 1) & notA);
            var shift5 = shift4 | ((shift4 << 1) & notA);
            var shift6 = shift5 | ((shift5 << 1) & notA);
            var shift7 = shift6 | ((shift6 << 1) & notA);

            return shift7; 
        }

        private static ulong GetDownRightBoard(ulong square)
        {
            const ulong notA = 18374403900871474942;

            var shift1 = (square >> 7) & notA;
            var shift2 = shift1 | ((shift1 >> 7) & notA);
            var shift3 = shift2 | ((shift2 >> 7) & notA);
            var shift4 = shift3 | ((shift3 >> 7) & notA);
            var shift5 = shift4 | ((shift4 >> 7) & notA);
            var shift6 = shift5 | ((shift5 >> 7) & notA);
            var shift7 = shift6 | ((shift6 >> 7) & notA);

            return shift7; 
        }

        private static ulong GetDownBoard(ulong square)
        {
            return (square >> 8) 
                    | (square >> 16) 
                    | (square >> 24) 
                    | (square >> 32) 
                    | (square >> 40) 
                    | (square >> 48) 
                    | (square >> 56);
        }

         private static ulong GetDownLeftBoard(ulong square)
        {
            const ulong notH = 9187201950435737471;

            var shift1 = (square >> 9) & notH;
            var shift2 = shift1 | ((shift1 >> 9) & notH);
            var shift3 = shift2 | ((shift2 >> 9) & notH);
            var shift4 = shift3 | ((shift3 >> 9) & notH);
            var shift5 = shift4 | ((shift4 >> 9) & notH);
            var shift6 = shift5 | ((shift5 >> 9) & notH);
            var shift7 = shift6 | ((shift6 >> 9) & notH);

            return shift7; 
        }

        private static ulong GetLeftBoard(ulong square)
        {
            const ulong notH = 9187201950435737471;

        var shift1 = (square >> 1) & notH;
        var shift2 = shift1 | ((shift1 >> 1) & notH);
        var shift3 = shift2 | ((shift2 >> 1) & notH);
        var shift4 = shift3 | ((shift3 >> 1) & notH);
        var shift5 = shift4 | ((shift4 >> 1) & notH);
        var shift6 = shift5 | ((shift5 >> 1) & notH);
        var shift7 = shift6 | ((shift6 >> 1) & notH);

        return shift7; 
        }

        private static ulong GetUpLeftBoard(ulong square)
        {
            const ulong notH = 9187201950435737471;

            var shift1 = (square << 7) & notH;
            var shift2 = shift1 | ((shift1 << 7) & notH);
            var shift3 = shift2 | ((shift2 << 7) & notH);
            var shift4 = shift3 | ((shift3 << 7) & notH);
            var shift5 = shift4 | ((shift4 << 7) & notH);
            var shift6 = shift5 | ((shift5 << 7) & notH);
            var shift7 = shift6 | ((shift6 << 7) & notH);

            return shift7; 
        }
    }
}
