using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.Exceptions;
using ChessEngine.PossibleMoves;

namespace ChessEngine.BoardSearching
{
    // Checks carried out on the chess board regarding various moves and attacks
    internal static class BoardChecking
    {
        internal static bool IsPieceOnSquare(UsefulBitboards usefulBitboards, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                if ((usefulBitboards.AllOccupiedSquares & square) != 0) return true;
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }
        
        internal static bool IsFriendlyPieceOnSquare(BoardState boardState, UsefulBitboards usefulBitboards, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var friendlySquares = 
                    boardState.WhiteToMove ? usefulBitboards.AllWhiteOccupiedSquares : usefulBitboards.AllBlackOccupiedSquares;

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
        
        internal static bool IsEnemyPieceOnSquare(BoardState boardState, UsefulBitboards usefulBitboards, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var enemySquares = 
                    boardState.WhiteToMove ? usefulBitboards.AllBlackOccupiedSquares : usefulBitboards.AllWhiteOccupiedSquares;

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

        internal static PieceType GetPieceTypeOnSquare(BoardState boardState, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) != 1)
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            if (((boardState.WhitePawns | boardState.BlackPawns) & square) != 0) return PieceType.Pawn;

            if (((boardState.WhiteKnights | boardState.BlackKnights) & square) != 0) return PieceType.Knight;
            
            if (((boardState.WhiteBishops | boardState.BlackBishops) & square) != 0) return PieceType.Bishop;

            if (((boardState.WhiteRooks | boardState.BlackRooks) & square) != 0) return PieceType.Rook;

            if (((boardState.WhiteQueen | boardState.BlackQueen) & square) != 0) return PieceType.Queen;

            if (((boardState.WhiteKing | boardState.BlackKing) & square) != 0) return PieceType.King;

            return PieceType.None;
        }

        internal static SpecialMoveType GetSpecialMoveType(Board board, 
                                                           ulong moveFrom,
                                                           ulong moveTo, 
                                                           string uciMove)
        {
            var captureFlag = false;
            var promotionFlag = false;

            var currentBoardState = board.GetCurrentBoardState();
            var usefulBitBoards = board.GetUsefulBitBoards();

            if (IsEnemyPieceOnSquare(currentBoardState, usefulBitBoards, moveTo))
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

            var moveFromPiece = GetPieceTypeOnSquare(currentBoardState, moveFrom);

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
        internal static bool IsSquareRayAttackedFromAbove(BoardState boardState, UsefulBitboards usefulBitboards, ulong square)
        {
            // Up
            var nearestUpPiece = FindUpBlockingPosition(usefulBitboards, square);

            if ((nearestUpPiece & boardState.BlackRooks) > 0 || (nearestUpPiece & boardState.BlackQueen) > 0)
                return true;

            // Up-right
            var nearestUpRightPiece = FindUpRightBlockingPosition(usefulBitboards, square);

            if ((nearestUpRightPiece & boardState.BlackBishops) > 0 || (nearestUpRightPiece & boardState.BlackQueen) > 0)
                return true;

            // Up Left
            var nearestUpLeftPiece = FindUpLeftBlockingPosition(usefulBitboards, square);

            return (nearestUpLeftPiece & boardState.BlackBishops) > 0
                   || (nearestUpLeftPiece & boardState.BlackQueen) > 0;
        }
        
        // Checks if the square is attacked from a ray attack below. Used to find if black king will be in check when castling
        // Ray attacks are from bishops, rooks and queens
        //
        internal static bool IsSquareRayAttackedFromBelow(BoardState boardState, UsefulBitboards usefulBitboards, ulong square)
        {
            //Down
            var nearestDownPiece = FindDownBlockingPosition(usefulBitboards, square);

            if ((nearestDownPiece & boardState.WhiteRooks) > 0 || (nearestDownPiece & boardState.WhiteQueen) > 0)
                return true;

            //Down-right
            var nearestDownRightPiece = FindDownRightBlockingPosition(usefulBitboards, square);

            if ((nearestDownRightPiece & boardState.WhiteBishops) > 0 || (nearestDownRightPiece & boardState.WhiteQueen) > 0)
            {
                return true;
            }

            //Up Left
            var nearestDownLeftPiece = FindDownLeftBlockingPosition(usefulBitboards, square);

            if ((nearestDownLeftPiece & boardState.WhiteBishops) > 0 || (nearestDownLeftPiece & boardState.WhiteQueen) > 0)
            {
                return true;
            }
            
            return false;
        }

        internal static bool IsSquareRayAttackedFromTheSide(BoardState boardState, UsefulBitboards usefulBitboards, ulong square)
        {
            if (boardState.WhiteToMove)
            {
                var nearestLeftPiece = FindLeftBlockingPosition(usefulBitboards, square);

                if ((nearestLeftPiece & boardState.BlackRooks) > 0 || (nearestLeftPiece & boardState.BlackQueen) > 0)
                {
                    return true;
                }

                var nearestRightPiece = FindRightBlockingPosition(usefulBitboards, square);

                if ((nearestRightPiece & boardState.BlackRooks) > 0 || (nearestRightPiece & boardState.BlackQueen) > 0)
                {
                    return true;
                }
            }
            else
            {
                var nearestLeftPiece = FindLeftBlockingPosition(usefulBitboards, square);

                if ((nearestLeftPiece & boardState.WhiteRooks) > 0 || (nearestLeftPiece & boardState.WhiteQueen) > 0)
                {
                    return true;
                }

                var nearestRightPiece = FindRightBlockingPosition(usefulBitboards, square);

                if ((nearestRightPiece & boardState.WhiteRooks) > 0 || (nearestRightPiece & boardState.WhiteQueen) > 0)
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
        internal static bool IsKingInCheck(BoardState boardState, UsefulBitboards usefulBitboards, bool whitePieces)
        {
            ulong friendlyKing;

            if (whitePieces)
            {
                friendlyKing = boardState.WhiteKing;
            }
            else
            {
                friendlyKing = boardState.BlackKing;
            }
            
            return IsSquareAttacked(boardState, usefulBitboards, friendlyKing, whitePieces);
        }

        // Checks if the king has any flight squares
        internal static bool CanKingMove(BoardState boardState, UsefulBitboards usefulBitboards, bool whitePieces)
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardState.WhiteKing);
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardState.BlackKing);

            if(whitePieces)
            {
                var possibleMoves = 
                    ValidMoveArrays.KingMoves[whiteKingPosition] & ~ValidMoveArrays.KingMoves[blackKingPosition];

                // Even if a black piece is on the square the king can go there if the square is not
                // under attack (i.e. the piece is not protected)
                var freeSquares = possibleMoves & ~usefulBitboards.AllWhiteOccupiedSquares;

                var storedAllOccupiedSquares = usefulBitboards.AllOccupiedSquares;
                var storedAllOccupiedWhiteSquares = usefulBitboards.AllWhiteOccupiedSquares;

                if (freeSquares > 0)
                {
                    // We need to temporarily remove the king to make sure his current place isn't blocking an attack
                    usefulBitboards.AllOccupiedSquares &= ~boardState.WhiteKing;
                    usefulBitboards.AllWhiteOccupiedSquares &= ~boardState.WhiteKing;

                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    foreach (var possMove in possMoves)
                    {
                        if (!IsSquareAttacked(boardState, usefulBitboards, possMove, true))
                        {
                            usefulBitboards.AllOccupiedSquares = storedAllOccupiedSquares;
                            usefulBitboards.AllWhiteOccupiedSquares = storedAllOccupiedWhiteSquares;
                            return true;
                        }
                    }
                }

                usefulBitboards.AllOccupiedSquares = storedAllOccupiedSquares;
                usefulBitboards.AllWhiteOccupiedSquares = storedAllOccupiedWhiteSquares;
                return false;
            }
            else
            {
                var possibleMoves = 
                    ValidMoveArrays.KingMoves[blackKingPosition] & ~ValidMoveArrays.KingMoves[whiteKingPosition];
                
                // Even if a white piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)
                //
                var freeSquares = 
                    possibleMoves & ~usefulBitboards.AllBlackOccupiedSquares;

                var storedAllOccupiedSquares = usefulBitboards.AllOccupiedSquares;
                var storedAllOccupiedBlackSquares = usefulBitboards.AllBlackOccupiedSquares;

                if (freeSquares > 0)
                {
                    usefulBitboards.AllOccupiedSquares &= ~boardState.BlackKing;
                    usefulBitboards.AllBlackOccupiedSquares &= ~boardState.BlackKing;

                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    foreach (var possMove in possMoves)
                    {
                        if (!IsSquareAttacked(boardState, usefulBitboards, possMove, false))
                        {
                            usefulBitboards.AllOccupiedSquares = storedAllOccupiedSquares;
                            usefulBitboards.AllBlackOccupiedSquares = storedAllOccupiedBlackSquares;
                            return true;
                        }
                    }
                }

                usefulBitboards.AllOccupiedSquares = storedAllOccupiedSquares;
                usefulBitboards.AllBlackOccupiedSquares = storedAllOccupiedBlackSquares;

                return false;
            }
        }
        
        // Checks all points from this piece to see if it is being attacked
        // If it is it returns true straight away (i.e. We don't know how many pieces it is being attacked by)
        // 
        // NOTE: To save time it is assumed that the pieceBoard has exactly 1 piece on it. If not, it may not behave as expecte
        //
        private static bool IsSquareAttacked(BoardState boardState, UsefulBitboards usefulBitboards, ulong pieceBoard, bool whitePieces)
        {
             if (IsKnightAttackingSquare(boardState, pieceBoard, whitePieces))
             {
                return true;
             }

             // Check if piece is surrounded by friends. If so, we only need to worry about 
             // knights so we can assume false
             var surroundingSpace = GetSurroundingSpace(pieceBoard);

             ulong emptyOrEnemyNeighbours;

             if (whitePieces)
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~usefulBitboards.AllWhiteOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                 {
                     return false;
                 }
             }
             else
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~usefulBitboards.AllBlackOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                 {
                     return false;
                 }
             }

             if (IsPawnAttackingSquareFast(boardState, pieceBoard, whitePieces))
             {
                 return true;
             }
            
             if (IsSquareAttackedByKing(boardState, pieceBoard, whitePieces))
             {
                 return true;
             }             
            
             if (IsSquareUnderRayAttackSuperFast(boardState, usefulBitboards, pieceBoard, emptyOrEnemyNeighbours, whitePieces))
             {
                 return true;
             }


             return false;
        }
        
        // Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        // since only one pawn can be attacking the king at once
        internal static bool IsPawnAttackingSquareFast(BoardState boardState, ulong squarePosition, bool whitePieces)
        {
            var squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            
            // Allows the quick masking of wrapping checks
            //
            var proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      

            if (whitePieces)
            {
                if(boardState.BlackPawns == 0)
                {
                    return false;
                }
                
                // heck up-right    
                var upRight = squarePosition << 9;

                if ((upRight & boardState.BlackPawns & proximityBoard) != 0)
                {
                    return true;
                }

                //Check up-left
                var upLeft = squarePosition << 7;

                if ((upLeft & boardState.BlackPawns & proximityBoard) != 0)
                {
                    return true;
                }
            }
            else
            {
                if (boardState.WhitePawns == 0)
                {
                    return false;
                }

                //Check down-right
                var downRight = squarePosition >> 7;

                if ((downRight & boardState.WhitePawns & proximityBoard) != 0)
                {
                    return true;
                }

                //Check down-left
                var upLeft = squarePosition >> 9;

                if ((upLeft & boardState.WhitePawns & proximityBoard) != 0)
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
        internal static bool IsKnightAttackingSquare(BoardState boardState, ulong squarePosition, bool whitePieces)
        {
            ulong knights;

            knights = whitePieces ? boardState.BlackKnights : boardState.WhiteKnights;
              
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

        internal static bool IsSquareAttackedByKing(BoardState boardState, ulong squarePosition, bool whitePieces)
        {
            ulong enemyKing;

            if (whitePieces)
            {
                enemyKing = boardState.BlackKing;
            }
            else
            {
                enemyKing = boardState.WhiteKing;
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

        private static bool IsSquareUnderRayAttackSuperFast(BoardState boardState, UsefulBitboards usefulBitboards, ulong squarePositionBoard, ulong emptyOrEnemySpaces, bool whitePieces)
        {
            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (whitePieces)
            {
                enemyQueenSquares = boardState.BlackQueen;
                enemyBishopSquares = boardState.BlackBishops;
                enemyRookSquares = boardState.BlackRooks;
            }
            else
            {
                enemyQueenSquares = boardState.WhiteQueen;
                enemyBishopSquares = boardState.WhiteBishops;
                enemyRookSquares = boardState.WhiteRooks;
            }
                       
            if (enemyQueenSquares != 0 || enemyRookSquares != 0)
            {
                var leftBoard = squarePositionBoard >> 1;
                var checkLeft = (leftBoard & emptyOrEnemySpaces) > 0;

                if(checkLeft)
                {
                    var nearestLeftPiece = FindLeftBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var rightBoard = squarePositionBoard << 1;
                var checkRight = (rightBoard & emptyOrEnemySpaces) > 0;
                
                if(checkRight)
                {
                    var nearestRightPiece = FindRightBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var upBoard = squarePositionBoard << 8;
                var checkUp = (upBoard & emptyOrEnemySpaces) > 0;

                if(checkUp)
                {
                    var nearestUpPiece = FindUpBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downBoard = squarePositionBoard >> 8;
                var checkDown = (downBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDown)
                {
                    var nearestDownPiece = FindDownBlockingPosition(usefulBitboards, squarePositionBoard);

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
                    var nearestUpRightPiece = FindUpRightBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var upLeftBoard = squarePositionBoard << 7;
                var checkUpLeft = (upLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkUpLeft)
                {
                    var nearestUpLeftPiece = FindUpLeftBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downRightBoard = squarePositionBoard >> 7;
                var checkDownRight = (downRightBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownRight)
                {
                    var nearestDownRightPiece = FindDownRightBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }

                var downLeftBoard = squarePositionBoard >> 9;
                var checkDownLeft = (downLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownLeft)
                {
                    var nearestDownLeftPiece = FindDownLeftBlockingPosition(usefulBitboards, squarePositionBoard);

                    if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static ulong CalculateAllowedBishopMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(usefulBitboards, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedBishopMoves(UsefulBitboards usefulBitboards, ulong square, bool whiteToMove)
        {
            return (CalculateAllowedUpRightMoves(usefulBitboards, square, whiteToMove) |
                    CalculateAllowedDownRightMoves(usefulBitboards, square, whiteToMove) |
                    CalculateAllowedDownLeftMoves(usefulBitboards, square, whiteToMove) |
                    CalculateAllowedUpLeftMoves(usefulBitboards, square, whiteToMove));
        }

        internal static ulong CalculateAllowedRookMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(usefulBitboards, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedRookMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(usefulBitboards, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedQueenMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedUpRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(usefulBitboards, pieceIndex, whiteToMove));
        }

        internal static ulong CalculateAllowedQueenMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            return (CalculateAllowedUpMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedLeftMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedUpRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownRightMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedDownLeftMoves(usefulBitboards, pieceIndex, whiteToMove) |
                    CalculateAllowedUpLeftMoves(usefulBitboards, pieceIndex, whiteToMove));
        }

        private static ulong CalculateAllowedUpMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var upBoard = LookupTables.UpBoard[pieceIndex];

            return CalculateAllowedUpMovesFromBoard(usefulBitboards,
                                                    upBoard,
                                                    whiteToMove);
        }

        private static ulong CalculateAllowedUpMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            var upBoard = GetUpBoard(pieceIndex);

            return CalculateAllowedUpMovesFromBoard(usefulBitboards,
                                                    upBoard,
                                                    whiteToMove);
        }

        private static ulong CalculateAllowedUpMovesFromBoard(UsefulBitboards usefulBitboards,
                                                              ulong upBoard, 
                                                              bool whiteToMove)
        {
            var upMoves = upBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                upMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                upMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return upMoves;
        }

        private static ulong CalculateAllowedRightMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var rightBoard = LookupTables.RightBoard[pieceIndex];

            return CalculateAllowedRightMovesFromBoard(usefulBitboards,
                                                       rightBoard,
                                                       whiteToMove);
        }

        private static ulong CalculateAllowedRightMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            var rightBoard = GetRightBoard(pieceIndex);

            return CalculateAllowedRightMovesFromBoard(usefulBitboards,
                                                       rightBoard,
                                                       whiteToMove);
        }

        private static ulong CalculateAllowedRightMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                 ulong rightBoard,
                                                                 bool whiteToMove)
        {
            var rightMoves = rightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                rightMoves &= usefulBitboards.BlackOrEmpty;
            else
                rightMoves &= usefulBitboards.WhiteOrEmpty;

            return rightMoves;
        }

        private static ulong CalculateAllowedDownMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var downBoard = LookupTables.DownBoard[pieceIndex];

            return CalculateAllowedDownMovesFromBoard(usefulBitboards,
                                                      downBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedDownMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            var downBoard = GetDownBoard(pieceIndex);

            return CalculateAllowedDownMovesFromBoard(usefulBitboards,
                                                      downBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedDownMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                ulong downBoard,
                                                                bool whiteToMove)
        {
            var downMoves = downBoard & usefulBitboards.AllOccupiedSquares; //Find first hit square

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
                downMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                downMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return downMoves;
        }

        private static ulong CalculateAllowedLeftMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var leftBoard = LookupTables.LeftBoard[pieceIndex];

            return CalculateAllowedLeftMovesFromBoard(usefulBitboards,
                                                      leftBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedLeftMoves(UsefulBitboards usefulBitboards, ulong pieceIndex, bool whiteToMove)
        {
            var leftBoard = GetLeftBoard(pieceIndex);

            return CalculateAllowedLeftMovesFromBoard(usefulBitboards,
                                                      leftBoard,
                                                      whiteToMove);
        }

        private static ulong CalculateAllowedLeftMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                ulong leftBoard,
                                                                bool whiteToMove)
        {
            var leftMoves = leftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                leftMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                leftMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return leftMoves;
        }

        private static ulong CalculateAllowedUpRightMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var upRightBoard = LookupTables.UpRightBoard[pieceIndex];

            return CalculateAllowedUpRightMovesFromBoard(usefulBitboards,
                                                          upRightBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedUpRightMoves(UsefulBitboards usefulBitboards, ulong piecePosition, bool whiteToMove)
        {
            var upRightBoard = GetUpRightBoard(piecePosition);

            return CalculateAllowedUpRightMovesFromBoard(usefulBitboards,
                                                         upRightBoard,
                                                         whiteToMove);
        }

        private static ulong CalculateAllowedUpRightMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                   ulong upRightBoard,
                                                                   bool whiteToMove)
        {
            var upRightMoves = upRightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                upRightMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                upRightMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return upRightMoves;
        }

        private static ulong CalculateAllowedDownRightMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var downRightBoard = LookupTables.DownRightBoard[pieceIndex];

            return CalculateAllowedDownRightMovesFromBoard(usefulBitboards,
                                                           downRightBoard,
                                                           whiteToMove);
        }

        private static ulong CalculateAllowedDownRightMoves(UsefulBitboards usefulBitboards, ulong piecePosition, bool whiteToMove)
        {
            var downRightBoard = GetDownRightBoard(piecePosition);

            return CalculateAllowedDownRightMovesFromBoard(usefulBitboards,
                                                           downRightBoard,
                                                           whiteToMove);
        }

        private static ulong CalculateAllowedDownRightMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                     ulong downRightBoard,
                                                                     bool whiteToMove)
        {
            var downRightMoves = downRightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                downRightMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                downRightMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return downRightMoves;
        }

        private static ulong CalculateAllowedDownLeftMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var downLeftBoard = LookupTables.DownLeftBoard[pieceIndex];

            return CalculateAllowedDownLeftMovesFromBoard(usefulBitboards,
                                                          downLeftBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedDownLeftMoves(UsefulBitboards usefulBitboards, ulong piecePosition, bool whiteToMove)
        {
            //ulong downLeftBoard = LookupTables.DownLeftBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            var downLeftBoard = GetDownLeftBoard(piecePosition);

            return CalculateAllowedDownLeftMovesFromBoard(usefulBitboards,
                                                          downLeftBoard,
                                                          whiteToMove);
        }

        private static ulong CalculateAllowedDownLeftMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                    ulong downLeftBoard,
                                                                    bool whiteToMove)
        {
            var downLeftMoves = downLeftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square
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
                downLeftMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                downLeftMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return downLeftMoves;
        }

        private static ulong CalculateAllowedUpLeftMoves(UsefulBitboards usefulBitboards, byte pieceIndex, bool whiteToMove)
        {
            var upLeftBoard = LookupTables.UpLeftBoard[pieceIndex];

            return CalculateAllowedUpLeftMovesFromBoard(usefulBitboards,
                                                        upLeftBoard,
                                                        whiteToMove);
        }

        private static ulong CalculateAllowedUpLeftMoves(UsefulBitboards usefulBitboards, ulong piecePosition, bool whiteToMove)
        {
            var upLeftBoard = GetUpLeftBoard(piecePosition);

            return CalculateAllowedUpLeftMovesFromBoard(usefulBitboards,
                                                        upLeftBoard,
                                                        whiteToMove);
        }

        private static ulong CalculateAllowedUpLeftMovesFromBoard(UsefulBitboards usefulBitboards,
                                                                  ulong upLeftBoard,
                                                                  bool whiteToMove)
        {
            var upLeftMoves = upLeftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

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
                upLeftMoves &= usefulBitboards.BlackOrEmpty;
            }
            else
            {
                upLeftMoves &= usefulBitboards.WhiteOrEmpty;
            }

            return upLeftMoves;
        }

        // Returns the bitboard value of the first piece, of any colour, up from the given squarePosition
        public static ulong FindUpBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var upBoard = GetUpBoard(square);

            var upMoves = upBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

            upMoves = (upMoves << 8) 
                      | (upMoves << 16) 
                      | (upMoves << 24) 
                      | (upMoves << 32) 
                      | (upMoves << 40) 
                      | (upMoves << 48);  //Fill all squares above by performing left shifts
            
            upMoves ^= upBoard;        //Just allowed squares

            return upMoves & usefulBitboards.AllOccupiedSquares;
        }

        public static ulong FindUpRightBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var upRightBoard = GetUpRightBoard(square);

            var upRightMoves = upRightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

            upRightMoves = (upRightMoves << 9) 
                           | (upRightMoves << 18) 
                           | (upRightMoves << 27) 
                           | (upRightMoves << 36) 
                           | (upRightMoves << 45)       //Fill all squares above and
                           | (upRightMoves << 54);      // right by performing left shifts

            upRightMoves &= upRightBoard;       //Remove overflow

            var notAboveRight = upRightMoves ^ upRightBoard;

            return notAboveRight & usefulBitboards.AllOccupiedSquares;
        }

        public static ulong FindRightBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var rightBoard = GetRightBoard(square);
            
            var rightMoves = rightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

            rightMoves = (rightMoves << 1) 
                         | (rightMoves << 2) 
                         | (rightMoves << 3) 
                         | (rightMoves << 4) 
                         | (rightMoves << 5) 
                         | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves &= rightBoard;       //Remove overflow

            var notRight = rightMoves ^ rightBoard;

            return notRight & usefulBitboards.AllOccupiedSquares;
        }

        public static ulong FindDownRightBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var downRightBoard = GetDownRightBoard(square);
            
            var downRightMoves = downRightBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

            downRightMoves = (downRightMoves >> 7) 
                             | (downRightMoves >> 14) 
                             | (downRightMoves >> 21) 
                             | (downRightMoves >> 28) 
                             | (downRightMoves >> 35)   //Fill all squares below-right
                             | (downRightMoves >> 42);  // by performing right shifts

            downRightMoves &= downRightBoard;       //Remove overflow

            var notBelowRight = downRightMoves ^ downRightBoard;

            return notBelowRight & usefulBitboards.AllOccupiedSquares;
        }

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, down from the given square
        /// </summary>
        public static ulong FindDownBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var downBoard = GetDownBoard(square);

            var downMoves = downBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts

            //downMoves = downMoves & downBoard;       //Remove overflow

            var notBelow = downMoves ^ downBoard;
            return notBelow & usefulBitboards.AllOccupiedSquares;
        }

        public static ulong FindDownLeftBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var downLeftBoard = GetDownLeftBoard(square);

            var downLeftMoves = downLeftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below-left by performing right shifts

            downLeftMoves &= downLeftBoard;       //Remove overflow

            var notBelowRight = downLeftMoves ^ downLeftBoard;
            return notBelowRight & usefulBitboards.AllOccupiedSquares;
        }

        internal static ulong FindLeftBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var leftBoard = GetLeftBoard(square);

            var leftMoves = leftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the right by performing left shifts

            leftMoves &= leftBoard;       //Remove overflow

            var notLeft = leftMoves ^ leftBoard;

            return notLeft & usefulBitboards.AllOccupiedSquares;
        }

        internal static ulong FindUpLeftBlockingPosition(UsefulBitboards usefulBitboards, ulong square)
        {
            var upLeftBoard = GetUpLeftBoard(square);

            var upLeftMoves = upLeftBoard & usefulBitboards.AllOccupiedSquares;   //Find first hit square

            upLeftMoves = (upLeftMoves << 7) 
                          | (upLeftMoves << 14) 
                          | (upLeftMoves << 21) 
                          | (upLeftMoves << 28) 
                          | (upLeftMoves << 35) 
                          | (upLeftMoves << 42);  //Fill all squares up-left by performing left shifts

            upLeftMoves &= upLeftBoard;       //Remove overflow

            var notAboveLeft = upLeftMoves ^ upLeftBoard;

            return notAboveLeft & usefulBitboards.AllOccupiedSquares;
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
