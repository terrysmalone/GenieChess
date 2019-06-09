﻿using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.Exceptions;
using ChessEngine.PossibleMoves;

namespace ChessEngine.BoardSearching
{
    /// <summary>
    /// Checks carried out on the chess board regarding various moves and attacks
    /// </summary>
    internal static class BoardChecking
    {
        #region piece on square checks

        /// <summary>
        /// Checks if there is a piece on the given square
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="square">The bitboard to check</param>
        /// <returns></returns>
        internal static bool IsPieceOnSquare(IBoard board, ulong square)
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

        /// <summary>
        /// Checks if there is an enemy piece on the given square
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="square">The bitboard to check</param>
        /// <returns></returns>
        internal static bool IsEnemyPieceOnSquare(IBoard board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var enemySquares = board.WhiteToMove ? board.AllBlackOccupiedSquares : board.AllWhiteOccupiedSquares;

                if ((enemySquares & square) != 0) return true;
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a friendly piece on the given square
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="square">The bitboard to check</param>
        /// <returns></returns>
        internal static bool IsFriendlyPieceOnSquare(IBoard board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                var friendlySquares = board.WhiteToMove ? board.AllWhiteOccupiedSquares : board.AllBlackOccupiedSquares;

                //ulong fullBoard = ulong.MaxValue;
                //ulong emptySquares = friendlySquares ^ fullBoard;

                if ((friendlySquares & square) != 0) return true;
            }
            else
            {
                throw new BitboardException("Bitboard is not equal to one square");
            }

            return false;
        }

        internal static PieceType GetPieceTypeOnSquare(IBoard board, ulong square)
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

        #endregion Piece on square methods

        /// <summary>
        /// Gets the Special move type of a move
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moveFrom"></param>
        /// <param name="moveTo"></param>
        /// <param name="pieceType"></param>
        /// <param name="uciMove"></param>
        /// <returns></returns>
        internal static SpecialMoveType GetSpecialMoveType(IBoard board, ulong moveFrom, ulong moveTo, PieceType pieceType, string uciMove)
        {
            var captureFlag = false;
            var promotionFlag = false;
            
            if (IsEnemyPieceOnSquare(board, moveTo))
                captureFlag = true;

            if (uciMove.Length > 4)
                promotionFlag = true;

            if (captureFlag)
            {
                if (promotionFlag)
                {
                    return GetPromotionCaptureForType(uciMove[4]);
                }
                else
                {
                    //A standard capture
                    return SpecialMoveType.Capture;
                }
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
                        return SpecialMoveType.KingCastle;
                    else if (moveTo == LookupTables.C1)
                        return SpecialMoveType.QueenCastle;
                }
                else if (moveFrom == LookupTables.E8)
                {
                    if (moveTo == LookupTables.G8)
                        return SpecialMoveType.KingCastle;
                    else if (moveTo == LookupTables.C8)
                        return SpecialMoveType.QueenCastle;
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

        /// <summary>
        /// Returns the given char letter as a promotion type
        /// </summary>
        /// <param name="pieceType"></param>
        /// <returns></returns>
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

        #region is square attacked methods

        /// <summary>
        /// Checks if the square is attacked from a ray attack above. Used to find if white king will be in check when castling
        /// Ray attacks are from bishops, rooks and queens
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        internal static bool IsSquareRayAttackedFromAbove(IBoard board, ulong square)
        {
            //Up
            var nearestUpPiece = FindUpBlockingPosition(board, square);

            if ((nearestUpPiece & board.BlackRooks) > 0 || (nearestUpPiece & board.BlackQueen) > 0)
                return true;

            //Up-right
            var nearestUpRightPiece = FindUpRightBlockingPosition(board, square);

            if ((nearestUpRightPiece & board.BlackBishops) > 0 || (nearestUpRightPiece & board.BlackQueen) > 0)
                return true;

            //Up Left
            var nearestUpLeftPiece = FindUpLeftBlockingPosition(board, square);

            return (nearestUpLeftPiece & board.BlackBishops) > 0
                   || (nearestUpLeftPiece & board.BlackQueen) > 0;
        }

        /// <summary>
        /// Checks if the square is attacked from a ray attack below. Used to find if black king will be in check when castling
        /// Ray attacks are from bishops, rooks and queens
        /// </summary>
        /// <param name="board"></param>
        /// <param name="square"></param>
        /// <returns></returns>
        internal static bool IsSquareRayAttackedFromBelow(IBoard board, ulong square)
        {
            //Down
            var nearestDownPiece = FindDownBlockingPosition(board, square);

            if ((nearestDownPiece & board.WhiteRooks) > 0 || (nearestDownPiece & board.WhiteQueen) > 0)
                return true;

            //Down-right
            var nearestDownRightPiece = FindDownRightBlockingPosition(board, square);

            if ((nearestDownRightPiece & board.WhiteBishops) > 0 || (nearestDownRightPiece & board.WhiteQueen) > 0)
                return true;

            //Up Left
            var nearestDownLeftPiece = FindDownLeftBlockingPosition(board, square);

            if ((nearestDownLeftPiece & board.WhiteBishops) > 0 || (nearestDownLeftPiece & board.WhiteQueen) > 0)
                return true;

            return false;
        }

        internal static bool IsSquareRayAttackedFromTheSide(IBoard board, ulong square)
        {
            if (board.WhiteToMove)
            {
                var nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.BlackRooks) > 0 || (nearestLeftPiece & board.BlackQueen) > 0)
                    return true;

                var nearestRightPiece = FindRightBlockingPosition(board, square);

                if ((nearestRightPiece & board.BlackRooks) > 0 || (nearestRightPiece & board.BlackQueen) > 0)
                    return true;
            }
            else
            {
                var nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.WhiteRooks) > 0 || (nearestLeftPiece & board.WhiteQueen) > 0)
                    return true;

                var nearestRightPiece = FindRightBlockingPosition(board, square);

                if ((nearestRightPiece & board.WhiteRooks) > 0 || (nearestRightPiece & board.WhiteQueen) > 0)
                    return true;
            }

            return false;
        }

        #region fast attack methods

        /// <summary>
        /// Returns true or false whether king is in check. If we do not need to know the number of checks or who/where is
        /// checking king use this over IsKingInCheck since it returns true as soon as it knows
        /// </summary>
        /// <returns></returns>
        internal static bool IsKingInCheckFast(IBoard board, PieceColour friendlyColour)
        {
            ulong friendlyKing;

            if (friendlyColour == PieceColour.White)
                friendlyKing = board.WhiteKing;
            else
                friendlyKing = board.BlackKing;

            //if (IsSquareAttackedFast(board, friendlyKing, friendlyColour))  
            if (IsSquareAttackedSuperFast(board, friendlyKing, friendlyColour))            
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the king has any flight squares
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <param name="pieceColour"></param>
        /// <returns></returns>
        internal static bool CanKingMove(IBoard boardPosition, PieceColour pieceColour)
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.WhiteKing);
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.BlackKing);

            if(pieceColour == PieceColour.White)
            {
                var possibleMoves = ValidMoveArrays.KingMoves[whiteKingPosition] & ~ValidMoveArrays.KingMoves[blackKingPosition];

                var freeSquares = possibleMoves & ~boardPosition.AllWhiteOccupiedSquares; //Even if a black piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)

                if (freeSquares > 0)
                {
                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    for (var i = 0; i < possMoves.Length; i++)
			        {
			            if(!IsSquareAttackedSuperFast(boardPosition, possMoves[i], pieceColour))
                            return true;
			        }
                }
                
                return false;
            }
            else
            {
                var possibleMoves = ValidMoveArrays.KingMoves[blackKingPosition] & ~ValidMoveArrays.KingMoves[whiteKingPosition];

                var freeSquares = possibleMoves & ~boardPosition.AllBlackOccupiedSquares; //Even if a white piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)

                if (freeSquares > 0)
                {
                    var possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    for (var i = 0; i < possMoves.Length; i++)
                    {
                        if (!IsSquareAttackedSuperFast(boardPosition, possMoves[i], pieceColour))
                            return true;
                    }
                }

                return false;
            }
        }


        /// <summary>
        /// Checks all points from this piece to see if it is being attacked
        /// If it is it returns true straight away (i.e. We don't know how many pieces it is being attacked by)
        /// 
        /// NOTE: To save time it is assumed that the pieceBoard has exactly 1 piece on it. If not, it may not behave as expecte
        /// </summary>
        /// <param name="board"></param>
        /// <param name="pieceBoard">The occupying square we want to check</param>
        /// <param name="friendlyColour"></param>
        /// /// <param name="checkKing">If we are checking if the king is being attacked we do not need to worry about the enemy king</param>
        /// <returns></returns>
        private static bool IsSquareAttackedSuperFast(IBoard board, ulong pieceBoard, PieceColour friendlyColour)
        {
             if (IsKnightAttackingSquare(board, pieceBoard, friendlyColour))
                return true;

            //Check if piece is surrounded by friends. If so, we only need to worry about 
            // knights so we can assume false
             var surroundingSpace = GetSurroundingSpace(pieceBoard);

             ulong emptyOrEnemyNeighbours;

             if (friendlyColour == PieceColour.White)
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllWhiteOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0) return false;
             }
             else
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllBlackOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0) return false;
             }

             if (IsPawnAttackingSquareFast(board, pieceBoard, friendlyColour))
                 return true;
            
            if (IsSquareAttackedByKing(board, pieceBoard, friendlyColour))
                return true;             
            
            if (IsSquareUnderRayAttackSuperFast(board, pieceBoard, emptyOrEnemyNeighbours, friendlyColour))
                return true;


             return false;
        }

        // This method is used to check if a square is under attack or to check if the king is undert attack.
        // Use this method if knowing the attacking piece/position or attack count is not necessary as it returns as soon as it knows       
        //internal static bool IsSquareAttackedFast(IBoard board, ulong squarePositionBoard, PieceColour friendlyColour)
        //{
        //    if (IsPawnAttackingSquareFast(board, squarePositionBoard, friendlyColour))
        //        return true;

        //    if (IsKnightAttackingSquare(board, squarePositionBoard, friendlyColour))
        //        return true;

        //    if (IsSquareAttackedByKing(board, squarePositionBoard, friendlyColour))
        //        return true;

        //    if (IsSquareUnderRayAttackFast(board, squarePositionBoard, friendlyColour))
        //        return true;

        //    return false;
        //}

        /// <summary>
        /// Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        /// since only one pawn can be attacking the king at once
        /// </summary>
        internal static bool IsPawnAttackingSquareFast(IBoard board, ulong squarePosition, PieceColour friendlyColour)
        {
            var squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            var proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      //Allows the quick masking of wrapping checks

            if (friendlyColour == PieceColour.White)
            {
                if(board.BlackPawns == 0)
                    return false;
                
                //Check up-right    
                var upRight = squarePosition << 9;

                if ((upRight & board.BlackPawns & proximityBoard) != 0)
                    return true;

                //Check up-left
                var upLeft = squarePosition << 7;

                if ((upLeft & board.BlackPawns & proximityBoard) != 0)                
                    return true;
                        }
            else
            {
                if (board.WhitePawns == 0)
                    return false;

                //Check down-right
                var downRight = squarePosition >> 7;

                if ((downRight & board.WhitePawns & proximityBoard) != 0)
                    return true;

                //Check down-left
                var upLeft = squarePosition >> 9;

                if ((upLeft & board.WhitePawns & proximityBoard) != 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a knight is attacking square. There is no need to check all knights for double-check 
        /// since only one knight can be attacking the king at once
        /// </summary>
        internal static bool IsKnightAttackingSquare(IBoard board, ulong squarePosition, PieceColour friendlyColour)
        {
            ulong knights;

            if (friendlyColour == PieceColour.White)
                knights = board.BlackKnights;
            else
                knights = board.WhiteKnights;
                
            if (knights == 0)   //If there are no kinghts we do not have to check
                return false;

            var currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

            var possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

            var knightAttacks = possibleKnightMoves & knights;
            if (knightAttacks != 0)
                return true;
            else
                return false;

        }

        internal static bool IsSquareAttackedByKing(IBoard board, ulong squarePosition, PieceColour friendlyColour)
        {
            ulong enemyKing;

            if (friendlyColour == PieceColour.White)
                enemyKing = board.BlackKing;
            else
                enemyKing = board.WhiteKing;

            var checkSquare = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            var surroundBoard = ValidMoveArrays.KingMoves[checkSquare];

            if ((enemyKing & surroundBoard) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the square is under ray attack. If position of attack or number of attacks is required this method will not work
        /// Use this method if knowing the attacking piece/position or attack count is not necessary as it returns as soon as it knows
        /// </summary>
        /// <param name="squarePositionBoard"></param>
        /// <param name="friendlyColour"></param>
        /// <returns></returns>
        internal static bool IsSquareUnderRayAttackFast(IBoard board, ulong squarePositionBoard, PieceColour friendlyColour)
        {
            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (friendlyColour == PieceColour.White)
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
                //Up
                var nearestUpPiece = FindUpBlockingPosition(board, squarePositionBoard);

                if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                    return true;

                //Left 
                var nearestLeftPiece = FindLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
                    return true;

                //Right
                var nearestRightPiece = FindRightBlockingPosition(board, squarePositionBoard);

                if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Down
                var nearestDownPiece = FindDownBlockingPosition(board, squarePositionBoard);

                if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
                    return true;
            }

            if (enemyQueenSquares != 0 || enemyBishopSquares != 0)
            {
                //Up-right
                var nearestUpRightPiece = FindUpRightBlockingPosition(board, squarePositionBoard);

                if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Up Left
                var nearestUpLeftPiece = FindUpLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                    return true;

                //Down-right
                var nearestDownRightPiece = FindDownRightBlockingPosition(board, squarePositionBoard);

                if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Up Left
                var nearestDownLeftPiece = FindDownLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                    return true;
            }

            return false;
        }

        internal static bool IsSquareUnderRayAttackSuperFast(IBoard board, ulong squarePositionBoard, ulong emptyOrEnemySpaces, PieceColour friendlyColour)
        {
            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (friendlyColour == PieceColour.White)
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
                        return true;
                }

                var rightBoard = squarePositionBoard << 1;
                var checkRight = (rightBoard & emptyOrEnemySpaces) > 0;
                
                if(checkRight)
                {
                    var nearestRightPiece = FindRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                        return true;
                }

                var upBoard = squarePositionBoard << 8;
                var checkUp = (upBoard & emptyOrEnemySpaces) > 0;

                if(checkUp)
                {
                    var nearestUpPiece = FindUpBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                        return true;
                }

                var downBoard = squarePositionBoard >> 8;
                var checkDown = (downBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDown)
                {
                    var nearestDownPiece = FindDownBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
                        return true;
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
                        return true;
                }

                var upLeftBoard = squarePositionBoard << 7;
                var checkUpLeft = (upLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkUpLeft)
                {
                    var nearestUpLeftPiece = FindUpLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                        return true;
                }

                var downRightBoard = squarePositionBoard >> 7;
                var checkDownRight = (downRightBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownRight)
                {
                    var nearestDownRightPiece = FindDownRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                        return true;
                }

                var downLeftBoard = squarePositionBoard >> 9;
                var checkDownLeft = (downLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownLeft)
                {
                    var nearestDownLeftPiece = FindDownLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                        return true;
                }
            }

            return false;
        }

        #endregion fast attack methods

        #endregion is square attacked methods

        #region Calculate allowed moves methods

        internal static ulong CalculateAllowedBishopMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            return (CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                    CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedBishopMoves(IBoard board, ulong square, PieceColour colour)
        {
            return (CalculateAllowedUpRightMoves(board, square, colour) |
                    CalculateAllowedDownRightMoves(board, square, colour) |
                    CalculateAllowedDownLeftMoves(board, square, colour) |
                    CalculateAllowedUpLeftMoves(board, square, colour));
        }

        internal static ulong CalculateAllowedRookMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, colour) |
                    CalculateAllowedRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownMoves(board, pieceIndex, colour) |
                    CalculateAllowedLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedRookMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, colour) |
                    CalculateAllowedRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownMoves(board, pieceIndex, colour) |
                    CalculateAllowedLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedQueenMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, colour) |
                    CalculateAllowedRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownMoves(board, pieceIndex, colour) |
                    CalculateAllowedLeftMoves(board, pieceIndex, colour) |
                    CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                    CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                    CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedQueenMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            return (CalculateAllowedUpMoves(board, pieceIndex, colour) |
                     CalculateAllowedRightMoves(board, pieceIndex, colour) |
                     CalculateAllowedDownMoves(board, pieceIndex, colour) |
                     CalculateAllowedLeftMoves(board, pieceIndex, colour) |
                     CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                     CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                     CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                     CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        #region Calculate up moves

        internal static ulong CalculateAllowedUpMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var upBoard = LookupTables.UpBoard[pieceIndex];

            return CalculateAllowedUpMovesFromBoard(board,
                                                      upBoard,
                                                      colour);
        }

        internal static ulong CalculateAllowedUpMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            var upBoard = GetUpBoard(pieceIndex);

            return CalculateAllowedUpMovesFromBoard(board,
                                                      upBoard,
                                                      colour);
        }

        private static ulong CalculateAllowedUpMovesFromBoard(IBoard board, 
                                                                ulong upBoard, 
                                                                PieceColour colour)
        {
            var upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square

            upMoves = (upMoves << 8) 
                      | (upMoves << 16) 
                      | (upMoves << 24) 
                      | (upMoves << 32) 
                      | (upMoves << 40) 
                      | (upMoves << 48);  //Fill all squares above by performing left shifts
            
            upMoves = upMoves ^ upBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                upMoves = upMoves & board.BlackOrEmpty;
            }
            else
            {
                upMoves = upMoves & board.WhiteOrEmpty;
            }

            return upMoves;
        }

        #endregion Calculate up moves

        #region calculate right moves

        internal static ulong CalculateAllowedRightMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var rightBoard = LookupTables.RightBoard[pieceIndex];

            return CalculateAllowedRightMovesFromBoard(board,
                                                            rightBoard, 
                                                            colour);
        }

        internal static ulong CalculateAllowedRightMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            var rightBoard = GetRightBoard(pieceIndex);

            return CalculateAllowedRightMovesFromBoard(board,
                                                            rightBoard, 
                                                            colour);
        }

        private static ulong CalculateAllowedRightMovesFromBoard(IBoard board, 
                                                                      ulong rightBoard, 
                                                                      PieceColour colour)
        {
            var rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square

            rightMoves = (rightMoves << 1) 
                         | (rightMoves << 2) 
                         | (rightMoves << 3) 
                         | (rightMoves << 4) 
                         | (rightMoves << 5) 
                         | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves = rightMoves & rightBoard;       //Remove overflow

            rightMoves = rightMoves ^ rightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                rightMoves = rightMoves & board.BlackOrEmpty;
            else
                rightMoves = rightMoves & board.WhiteOrEmpty;

            return rightMoves;
        }

        #endregion calculate right moves

        #region Calculate down moves

        internal static ulong CalculateAllowedDownMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var downBoard = LookupTables.DownBoard[pieceIndex];

            return CalculateAllowedDownMovesFromBoard(board,
                                                          downBoard,
                                                          colour);
        }

        internal static ulong CalculateAllowedDownMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            var downBoard = GetDownBoard(pieceIndex);

            return CalculateAllowedDownMovesFromBoard(board,
                                                          downBoard,
                                                          colour);
        }

        private static ulong CalculateAllowedDownMovesFromBoard(IBoard board,
                                                                    ulong downBoard,
                                                                    PieceColour colour)
        {
            var downMoves = downBoard & board.AllOccupiedSquares; //Find first hit square

            downMoves = (downMoves >> 8) 
                        | (downMoves >> 16) 
                        | (downMoves >> 24) 
                        | (downMoves >> 32) 
                        | (downMoves >> 40) 
                        | (downMoves >> 48); //Fill all squares below by performing right shifts

            downMoves = downMoves ^ downBoard; //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                downMoves = downMoves & board.BlackOrEmpty;
            }
            else
            {
                downMoves = downMoves & board.WhiteOrEmpty;
            }

            return downMoves;
        }

        #endregion Calculate down moves

        #region Calculate left moves

        internal static ulong CalculateAllowedLeftMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var leftBoard = LookupTables.LeftBoard[pieceIndex];

            return CalculateAllowedLeftMovesFromBoard(board,
                                                          leftBoard,
                                                          colour);
        }

        private static ulong CalculateAllowedLeftMoves(IBoard board, ulong pieceIndex, PieceColour colour)
        {
            var leftBoard = GetLeftBoard(pieceIndex);

            return CalculateAllowedLeftMovesFromBoard(board,
                                                          leftBoard,
                                                          colour);
        }

        private static ulong CalculateAllowedLeftMovesFromBoard(IBoard board,
                                                                    ulong leftBoard,
                                                                    PieceColour colour)
        {
            var leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square

            leftMoves = (leftMoves >> 1) 
                        | (leftMoves >> 2) 
                        | (leftMoves >> 3) 
                        | (leftMoves >> 4) 
                        | (leftMoves >> 5) 
                        | (leftMoves >> 6);  //Fill all squares to the left by performing right shifts

            leftMoves = leftMoves & leftBoard;       //Remove overflow

            leftMoves = leftMoves ^ leftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                leftMoves = leftMoves & board.BlackOrEmpty;
            }
            else
            {
                leftMoves = leftMoves & board.WhiteOrEmpty;
            }

            return leftMoves;
        }

        #endregion Calculate left moves

        #region Calculate up right moves

        internal static ulong CalculateAllowedUpRightMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var upRightBoard = LookupTables.UpRightBoard[pieceIndex];

            return CalculateAllowedUpRightMovesFromBoard(board,
                                                          upRightBoard,
                                                          colour);
        }

        internal static ulong CalculateAllowedUpRightMoves(IBoard board, ulong piecePosition, PieceColour colour)
        {
            var upRightBoard = GetUpRightBoard(piecePosition);

            return CalculateAllowedUpRightMovesFromBoard(board,
                                                         upRightBoard,
                                                         colour);
        }

        private static ulong CalculateAllowedUpRightMovesFromBoard(IBoard board,
                                                                    ulong upRightBoard,
                                                                    PieceColour colour)
        {
            var upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square

            upRightMoves = (upRightMoves << 9) 
                           | (upRightMoves << 18) 
                           | (upRightMoves << 27) 
                           | (upRightMoves << 36) 
                           | (upRightMoves << 45) 
                           | (upRightMoves << 54);  //Fill all squares up and right by performing left shifts

            upRightMoves = upRightMoves & upRightBoard;       //Remove overflow

            upRightMoves = upRightMoves ^ upRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                upRightMoves = upRightMoves & board.BlackOrEmpty;
            }
            else
            {
                upRightMoves = upRightMoves & board.WhiteOrEmpty;
            }

            return upRightMoves;
        }

        #endregion Calculate up right moves
        
        #region Calculate down right moves

        internal static ulong CalculateAllowedDownRightMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var downRightBoard = LookupTables.DownRightBoard[pieceIndex];

            return CalculateAllowedDownRightMovesFromBoard(board,
                                                           downRightBoard,
                                                           colour);
        }

        internal static ulong CalculateAllowedDownRightMoves(IBoard board, ulong piecePosition, PieceColour colour)
        {
            var downRightBoard = GetDownRightBoard(piecePosition);

            return CalculateAllowedDownRightMovesFromBoard(board,
                                                           downRightBoard,
                                                         colour);
        }

        private static ulong CalculateAllowedDownRightMovesFromBoard(IBoard board,
                                                                   ulong downRightBoard,
                                                                   PieceColour colour)
        {
            var downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square

            downRightMoves = (downRightMoves >> 7) 
                             | (downRightMoves >> 14) 
                             | (downRightMoves >> 21) 
                             | (downRightMoves >> 28) 
                             | (downRightMoves >> 35) 
                             | (downRightMoves >> 42);  //Fill all squares down and right by performing left shifts

            downRightMoves = downRightMoves & downRightBoard;       //Remove overflow

            downRightMoves = downRightMoves ^ downRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                downRightMoves = downRightMoves & board.BlackOrEmpty;
            }
            else
            {
                downRightMoves = downRightMoves & board.WhiteOrEmpty;
            }

            return downRightMoves;
        }

        #endregion Calculate down right moves

        #region Calculate down left moves
        
        internal static ulong CalculateAllowedDownLeftMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var downLeftBoard = LookupTables.DownLeftBoard[pieceIndex];

            return CalculateAllowedDownLeftMovesFromBoard(board,
                                                           downLeftBoard,
                                                           colour);
        }

        internal static ulong CalculateAllowedDownLeftMoves(IBoard board, ulong piecePosition, PieceColour colour)
        {
            //ulong downLeftBoard = LookupTables.DownLeftBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            var downLeftBoard = GetDownLeftBoard(piecePosition);

            return CalculateAllowedDownLeftMovesFromBoard(board,
                                                          downLeftBoard,
                                                          colour);
        }

        private static ulong CalculateAllowedDownLeftMovesFromBoard(IBoard board,
                                                                    ulong downLeftBoard,
                                                                    PieceColour colour)
        {
            var downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) 
                            | (downLeftMoves >> 18) 
                            | (downLeftMoves >> 27) 
                            | (downLeftMoves >> 36) 
                            | (downLeftMoves >> 45)     // Fill all squares down and
                            | (downLeftMoves >> 54);    // left by performing right shifts

            downLeftMoves = downLeftMoves & downLeftBoard;       //Remove overflow

            downLeftMoves = downLeftMoves ^ downLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                downLeftMoves = downLeftMoves & board.BlackOrEmpty;
            }
            else
            {
                downLeftMoves = downLeftMoves & board.WhiteOrEmpty;
            }

            return downLeftMoves;
        }

        #endregion Calculate down left moves

        #region Calculate up left moves

        internal static ulong CalculateAllowedUpLeftMoves(IBoard board, byte pieceIndex, PieceColour colour)
        {
            var upLeftBoard = LookupTables.UpLeftBoard[pieceIndex];

            return CalculateAllowedUpLeftMovesFromBoard(board,
                                                          upLeftBoard,
                                                          colour);
        }

        internal static ulong CalculateAllowedUpLeftMoves(IBoard board, ulong piecePosition, PieceColour colour)
        {
            var upLeftBoard = GetUpLeftBoard(piecePosition);

            return CalculateAllowedUpLeftMovesFromBoard(board,
                                                          upLeftBoard,
                                                          colour);
        }

        private static ulong CalculateAllowedUpLeftMovesFromBoard(IBoard board,
                                                                    ulong upLeftBoard,
                                                                    PieceColour colour)
        {
            var upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square

            upLeftMoves = (upLeftMoves << 7) 
                          | (upLeftMoves << 14) 
                          | (upLeftMoves << 21) 
                          | (upLeftMoves << 28) 
                          | (upLeftMoves << 35)   //Fill all squares up and 
                          | (upLeftMoves << 42);  // left by performing right shifts

            upLeftMoves = upLeftMoves & upLeftBoard;       //Remove overflow

            upLeftMoves = upLeftMoves ^ upLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
            {
                upLeftMoves = upLeftMoves & board.BlackOrEmpty;
            }
            else
            {
                upLeftMoves = upLeftMoves & board.WhiteOrEmpty;
            }

            return upLeftMoves;
        }

        #endregion Calculate up left moves

        #endregion Calculate allowed moves methods

        #region find nearest piece searches

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, up from the given squarePosition
        /// </summary>
        internal static ulong FindUpBlockingPosition(IBoard board, ulong square)
        {
            var upBoard = GetUpBoard(square);

            var upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square

            upMoves = (upMoves << 8) 
                      | (upMoves << 16) 
                      | (upMoves << 24) 
                      | (upMoves << 32) 
                      | (upMoves << 40) 
                      | (upMoves << 48);  //Fill all squares above by performing left shifts
            
            upMoves = upMoves ^ upBoard;        //Just allowed squares

            return upMoves & board.AllOccupiedSquares;
        }

        internal static ulong FindUpRightBlockingPosition(IBoard board, ulong square)
        {
            var upRightBoard = GetUpRightBoard(square);

            var upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square

            upRightMoves = (upRightMoves << 9) 
                           | (upRightMoves << 18) 
                           | (upRightMoves << 27) 
                           | (upRightMoves << 36) 
                           | (upRightMoves << 45)       //Fill all squares above and
                           | (upRightMoves << 54);      // right by performing left shifts

            upRightMoves = upRightMoves & upRightBoard;       //Remove overflow

            var notAboveRight = upRightMoves ^ upRightBoard;

            return notAboveRight & board.AllOccupiedSquares;
        }

        internal static ulong FindRightBlockingPosition(IBoard board, ulong square)
        {
            var rightBoard = GetRightBoard(square);
            
            var rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square

            rightMoves = (rightMoves << 1) 
                         | (rightMoves << 2) 
                         | (rightMoves << 3) 
                         | (rightMoves << 4) 
                         | (rightMoves << 5) 
                         | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves = rightMoves & rightBoard;       //Remove overflow

            var notRight = rightMoves ^ rightBoard;

            return notRight & board.AllOccupiedSquares;
        }

        internal static ulong FindDownRightBlockingPosition(IBoard board, ulong square)
        {
            var downRightBoard = GetDownRightBoard(square);
            
            var downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square

            downRightMoves = (downRightMoves >> 7) 
                             | (downRightMoves >> 14) 
                             | (downRightMoves >> 21) 
                             | (downRightMoves >> 28) 
                             | (downRightMoves >> 35)   //Fill all squares below-right
                             | (downRightMoves >> 42);  // by performing right shifts

            downRightMoves = downRightMoves & downRightBoard;       //Remove overflow

            var notBelowRight = downRightMoves ^ downRightBoard;

            return notBelowRight & board.AllOccupiedSquares;
        }

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, down from the given square
        /// </summary>
        internal static ulong FindDownBlockingPosition(IBoard board, ulong square)
        {
            var downBoard = GetDownBoard(square);

            var downMoves = downBoard & board.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts

            //downMoves = downMoves & downBoard;       //Remove overflow

            var notBelow = downMoves ^ downBoard;
            return notBelow & board.AllOccupiedSquares;
        }

        internal static ulong FindDownLeftBlockingPosition(IBoard board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong downLeftBoard = LookupTables.DownLeftBoard[squareIndex];

            var downLeftBoard = GetDownLeftBoard(square);

            var downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below-left by performing right shifts

            downLeftMoves = downLeftMoves & downLeftBoard;       //Remove overflow

            var notBelowRight = downLeftMoves ^ downLeftBoard;
            return notBelowRight & board.AllOccupiedSquares;
        }

        internal static ulong FindLeftBlockingPosition(IBoard board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong leftBoard = LookupTables.LeftBoard[squareIndex];

            var leftBoard = GetLeftBoard(square);

            var leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the right by performing left shifts

            leftMoves = leftMoves & leftBoard;       //Remove overflow

            var notLeft = leftMoves ^ leftBoard;

            return notLeft & board.AllOccupiedSquares;
        }

        internal static ulong FindUpLeftBlockingPosition(IBoard board, ulong square)
        {
            var upLeftBoard = GetUpLeftBoard(square);

            var upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square

            upLeftMoves = (upLeftMoves << 7) 
                          | (upLeftMoves << 14) 
                          | (upLeftMoves << 21) 
                          | (upLeftMoves << 28) 
                          | (upLeftMoves << 35) 
                          | (upLeftMoves << 42);  //Fill all squares up-left by performing left shifts

            upLeftMoves = upLeftMoves & upLeftBoard;       //Remove overflow

            var notAboveLeft = upLeftMoves ^ upLeftBoard;

            return notAboveLeft & board.AllOccupiedSquares;
        }

        #endregion find nearest piece searches

        #region shifts

         
        private static ulong GetSurroundingSpace(ulong pieceBoard)
        {
            const ulong notA = 18374403900871474942; //All squares except A column
            const ulong notH = 9187201950435737471; //All squares except H column
            
            return ((pieceBoard << 7) & notH) | (pieceBoard << 8) | ((pieceBoard << 9) & notA) |
                   ((pieceBoard >> 1) & notH) |                     ((pieceBoard << 1) & notA) |
                   ((pieceBoard >> 9) & notH) | (pieceBoard >> 8) | ((pieceBoard >> 7) & notA);
        }
         

        #endregion shifts

        #region ray boards

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

        #endregion ray boards
    }
}