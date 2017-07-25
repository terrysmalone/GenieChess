using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Exceptions;
using ChessGame.PossibleMoves;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;

namespace ChessGame.BoardSearching
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
        internal static bool IsPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                ulong blackPieces = board.BlackPawns | board.BlackKnights | board.BlackBishops | board.BlackRooks | board.BlackQueen | board.BlackKing;                
                ulong whitePieces = board.WhitePawns | board.WhiteKnights | board.WhiteBishops | board.WhiteRooks | board.WhiteQueen | board.WhiteKing;

                ulong allOccupiedSquares = whitePieces | blackPieces;

               // ulong fullBoard = ulong.MaxValue;
                //ulong emptySquares = allOccupiedSquares ^ fullBoard;

                if ((allOccupiedSquares & square) != (ulong)0)
                    return true;
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
        internal static bool IsEnemyPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                ulong enemySquares;

                if (board.WhiteToMove)
                {
                    enemySquares = board.BlackPawns | board.BlackKnights | board.BlackBishops | board.BlackRooks | board.BlackQueen | board.BlackKing;
                }
                else
                {
                    enemySquares = board.WhitePawns | board.WhiteKnights | board.WhiteBishops | board.WhiteRooks | board.WhiteQueen | board.WhiteKing;
                
                }

                //ulong fullBoard = ulong.MaxValue;
                //ulong emptySquares = enemySquares ^ fullBoard;

                if ((enemySquares & square) != (ulong)0)
                    return true;
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
        internal static bool IsFriendlyPieceOnSquare(Board board, ulong square)
        {
            if (BitboardOperations.GetPopCount(square) == 1)
            {
                ulong friendlySquares;

                if (board.WhiteToMove)
                {

                    friendlySquares = board.WhitePawns | board.WhiteKnights | board.WhiteBishops | board.WhiteRooks | board.WhiteQueen | board.WhiteKing;
                }
                else
                {
                    friendlySquares = board.BlackPawns | board.BlackKnights | board.BlackBishops | board.BlackRooks | board.BlackQueen | board.BlackKing;

                }

                //ulong fullBoard = ulong.MaxValue;
                //ulong emptySquares = friendlySquares ^ fullBoard;

                if ((friendlySquares & square) != (ulong)0)
                    return true;
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

            if (((board.WhitePawns | board.BlackPawns) & square) != 0)
                return PieceType.Pawn;

            if (((board.WhiteKnights | board.BlackKnights) & square) != 0)
                return PieceType.Knight;

            if (((board.WhiteBishops | board.BlackBishops) & square) != 0)
                return PieceType.Bishop;

            if (((board.WhiteRooks | board.BlackRooks) & square) != 0)
                return PieceType.Rook;

            if (((board.WhiteQueen | board.BlackQueen) & square) != 0)
                return PieceType.Queen;

            if (((board.WhiteKing | board.BlackKing) & square) != 0)
                return PieceType.King;

            return PieceType.None;
        }

        #endregion Piece on square methods

        /// <summary>
        /// Gets the Special move type of a move
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moveFrom"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        internal static SpecialMoveType GetSpecialMoveType(Board board, ulong moveFrom, ulong moveTo, PieceType pieceType, string uciMove)
        {
            bool captureFlag = false;
            bool promotionFlag = false;
            
            if (BoardChecking.IsEnemyPieceOnSquare(board, moveTo))
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

            PieceType moveFromPiece = BoardChecking.GetPieceTypeOnSquare(board, moveFrom);

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
                    return SpecialMoveType.DoublePawnPush;

                if (board.EnPassantPosition != (ulong)0)
                {
                    if(board.EnPassantPosition == moveTo)   //pawn is moving to en passant position
                    {
                        if (moveFrom << 7 == moveTo || moveFrom << 9 == moveTo) //pawn did a capture move and we already know there was no standard capture
                            return SpecialMoveType.ENPassantCapture;
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
        internal static bool IsSquareRayAttackedFromAbove(Board board, ulong square)
        {
            //Up
            ulong nearestUpPiece = FindUpBlockingPosition(board, square);

            if ((nearestUpPiece & board.BlackRooks) > 0 || (nearestUpPiece & board.BlackQueen) > 0)
                return true;

            //Up-right
            ulong nearestUpRightPiece = FindUpRightBlockingPosition(board, square);

            if ((nearestUpRightPiece & board.BlackBishops) > 0 || (nearestUpRightPiece & board.BlackQueen) > 0)
                return true;

            //Up Left
            ulong nearestUpLeftPiece = FindUpLeftBlockingPosition(board, square);

            if ((nearestUpLeftPiece & board.BlackBishops) > 0 || (nearestUpLeftPiece & board.BlackQueen) > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the square is attacked from a ray attack below. Used to find if black king will be in check when castling
        /// Ray attacks are from bishops, rooks and queens
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        internal static bool isSquareRayAttackedFromBelow(Board board, ulong square)
        {
            //Down
            ulong nearestDownPiece = FindDownBlockingPosition(board, square);

            if ((nearestDownPiece & board.WhiteRooks) > 0 || (nearestDownPiece & board.WhiteQueen) > 0)
                return true;

            //Down-right
            ulong nearestDownRightPiece = FindDownRightBlockingPosition(board, square);

            if ((nearestDownRightPiece & board.WhiteBishops) > 0 || (nearestDownRightPiece & board.WhiteQueen) > 0)
                return true;

            //Up Left
            ulong nearestDownLeftPiece = FindDownLeftBlockingPosition(board, square);

            if ((nearestDownLeftPiece & board.WhiteBishops) > 0 || (nearestDownLeftPiece & board.WhiteQueen) > 0)
                return true;

            return false;
        }

        internal static bool IsSquareRayAttackedFromTheSide(Board board, ulong square)
        {
            if (board.WhiteToMove)
            {
                ulong nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.BlackRooks) > 0 || (nearestLeftPiece & board.BlackQueen) > 0)
                    return true;

                ulong nearestRightPiece = FindRightBlockingPosition(board, square);

                if ((nearestRightPiece & board.BlackRooks) > 0 || (nearestRightPiece & board.BlackQueen) > 0)
                    return true;
            }
            else
            {
                ulong nearestLeftPiece = FindLeftBlockingPosition(board, square);

                if ((nearestLeftPiece & board.WhiteRooks) > 0 || (nearestLeftPiece & board.WhiteQueen) > 0)
                    return true;

                ulong nearestRightPiece = FindRightBlockingPosition(board, square);

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
        internal static bool IsKingInCheckFast(Board board, PieceColour friendlyColour)
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
        internal static bool CanKingMove(Board boardPosition, PieceColour pieceColour)
        {
            byte whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.WhiteKing);
            byte blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(boardPosition.BlackKing);

            if(pieceColour == PieceColour.White)
            {
                ulong possibleMoves = ValidMoveArrays.KingMoves[whiteKingPosition] & ~ValidMoveArrays.KingMoves[blackKingPosition];

                ulong freeSquares = possibleMoves & ~boardPosition.AllWhiteOccupiedSquares; //Even if a black piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)

                if (freeSquares > 0)
                {
                    ulong[] possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    for (int i = 0; i < possMoves.Length; i++)
			        {
			            if(!IsSquareAttackedSuperFast(boardPosition, possMoves[i], pieceColour))
                            return true;
			        }
                }
                
                return false;
            }
            else
            {
                ulong possibleMoves = ValidMoveArrays.KingMoves[blackKingPosition] & ~ValidMoveArrays.KingMoves[whiteKingPosition];

                ulong freeSquares = possibleMoves & ~boardPosition.AllBlackOccupiedSquares; //Even if a white piece is on the square the king can go there if the square is not under attack (i.e. the piece is not protected)

                if (freeSquares > 0)
                {
                    ulong[] possMoves = BitboardOperations.SplitBoardToArray(freeSquares);

                    for (int i = 0; i < possMoves.Length; i++)
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
        /// <param name="pieceBoard"></param>
        /// <param name="friendlyColour"></param>
        /// /// <param name="checkKing">If we are checking if the king is being attacked we do not need to worry about the enemy king</param>
        /// <returns></returns>
        private static bool IsSquareAttackedSuperFast(Board board, ulong pieceBoard, PieceColour friendlyColour)
        {
            //ulong notA = 18374403900871474942;
            //ulong notH = 9187201950435737471;

             if (IsKnightAttackingSquareFast(board, pieceBoard, friendlyColour))
                return true;

            //Check if piece is surrounded by friends. If so, we only need to worry about knights 
             ulong surroundingSpace = GetSurroundingSpace(pieceBoard);
             ulong emptyOrEnemyNeighbours = 0;

             if (friendlyColour == PieceColour.White)
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllWhiteOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                     return false;

                 //Carry on with checks here
             }
             else
             {
                 emptyOrEnemyNeighbours = ((pieceBoard | surroundingSpace) & ~board.AllBlackOccupiedSquares);

                 if (emptyOrEnemyNeighbours == 0)
                     return false;

                 //Carry on with checks here
             }

             if (IsPawnAttackingSquareFast(board, pieceBoard, friendlyColour))
                 return true;
            
            if (IsSquareAttackedByKing(board, pieceBoard, friendlyColour))
                return true;             

            //if (IsSquareUnderRayAttackFast(board, pieceBoard, friendlyColour))
            //    return true;

            if (IsSquareUnderRayAttackSuperFast(board, pieceBoard, emptyOrEnemyNeighbours, friendlyColour))
                return true;


             return false;
        }

        /// <summary>
        /// This method is used to check if a square is under attack or to check if the king is undert attack.
        /// Use this method if knowing the attacking piece/position or attack count is not necessary as it returns as soon as it knows
        /// </summary>
        /// <param name="squarePositionBoard"></param>
        /// <param name="friendlyColour"></param>
        /// <returns></returns>
        internal static bool IsSquareAttackedFast(Board board, ulong squarePositionBoard, PieceColour friendlyColour)
        {
            if (IsPawnAttackingSquareFast(board, squarePositionBoard, friendlyColour))
                return true;

            if (IsKnightAttackingSquareFast(board, squarePositionBoard, friendlyColour))
                return true;

            if (IsSquareAttackedByKing(board, squarePositionBoard, friendlyColour))
                return true;

            if (IsSquareUnderRayAttackFast(board, squarePositionBoard, friendlyColour))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        /// since only one pawn can be attacking the king at once
        /// </summary>
        internal static bool IsPawnAttackingSquareFast(Board board, ulong squarePosition, PieceColour friendlyColour)
        {
            byte squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            ulong proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      //Allows the quick masking of wrapping checks

            if (friendlyColour == PieceColour.White)
            {
                if(board.BlackPawns == 0)
                    return false;
                
                //Check up-right    
                ulong upRight = squarePosition << 9;

                if ((upRight & board.BlackPawns & proximityBoard) != 0)
                    return true;

                //Check up-left
                ulong upLeft = squarePosition << 7;

                if ((upLeft & board.BlackPawns & proximityBoard) != 0)                
                    return true;
                        }
            else
            {
                if (board.WhitePawns == 0)
                    return false;

                //Check down-right
                ulong downRight = squarePosition >> 7;

                if ((downRight & board.WhitePawns & proximityBoard) != 0)
                    return true;

                //Check down-left
                ulong upLeft = squarePosition >> 9;

                if ((upLeft & board.WhitePawns & proximityBoard) != 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a knight is attacking square. There is no need to check all knights for double-check 
        /// since only one knight can be attacking the king at once
        /// </summary>
        internal static bool IsKnightAttackingSquareFast(Board board, ulong squarePosition, PieceColour friendlyColour)
        {
            ulong knights;

            if (friendlyColour == PieceColour.White)
                knights = board.BlackKnights;
            else
                knights = board.WhiteKnights;
                
            if (knights == 0)   //If there are no kinghts we do not have to check
                return false;

            byte currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

            ulong possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

            ulong knightAttacks = possibleKnightMoves & knights;
            if (knightAttacks != 0)
                return true;
            else
                return false;

        }

        internal static bool IsSquareAttackedByKing(Board board, ulong squarePosition, PieceColour friendlyColour)
        {
            ulong enemyKing;

            if (friendlyColour == PieceColour.White)
                enemyKing = board.BlackKing;
            else
                enemyKing = board.WhiteKing;

            byte checkSquare = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            ulong surroundBoard = ValidMoveArrays.KingMoves[checkSquare];

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
        internal static bool IsSquareUnderRayAttackFast(Board board, ulong squarePositionBoard, PieceColour friendlyColour)
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
                ulong nearestUpPiece = FindUpBlockingPosition(board, squarePositionBoard);

                if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                    return true;

                //Left 
                ulong nearestLeftPiece = FindLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
                    return true;

                //Right
                ulong nearestRightPiece = FindRightBlockingPosition(board, squarePositionBoard);

                if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Down
                ulong nearestDownPiece = FindDownBlockingPosition(board, squarePositionBoard);

                if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
                    return true;
            }

            if (enemyQueenSquares != 0 || enemyBishopSquares != 0)
            {
                //Up-right
                ulong nearestUpRightPiece = FindUpRightBlockingPosition(board, squarePositionBoard);

                if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Up Left
                ulong nearestUpLeftPiece = FindUpLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                    return true;

                //Down-right
                ulong nearestDownRightPiece = FindDownRightBlockingPosition(board, squarePositionBoard);

                if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                    return true;

                //Up Left
                ulong nearestDownLeftPiece = FindDownLeftBlockingPosition(board, squarePositionBoard);

                if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                    return true;
            }

            return false;
        }

        internal static bool IsSquareUnderRayAttackSuperFast(Board board, ulong squarePositionBoard, ulong emptyOrEnemySpaces, PieceColour friendlyColour)
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
                ulong leftBoard = squarePositionBoard >> 1;
                bool checkLeft = (leftBoard & emptyOrEnemySpaces) > 0;

                if(checkLeft)
                {
                    ulong nearestLeftPiece = FindLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong rightBoard = squarePositionBoard << 1;
                bool checkRight = (rightBoard & emptyOrEnemySpaces) > 0;
                
                if(checkRight)
                {
                    ulong nearestRightPiece = FindRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong upBoard = squarePositionBoard << 8;
                bool checkUp = (upBoard & emptyOrEnemySpaces) > 0;

                if(checkUp)
                {
                    ulong nearestUpPiece = FindUpBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong downBoard = squarePositionBoard >> 8;
                bool checkDown = (downBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDown)
                {
                    ulong nearestDownPiece = FindDownBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
                        return true;
                }
            }            

            if (enemyQueenSquares != 0 || enemyBishopSquares != 0)
            {
                ulong upRightBoard = squarePositionBoard << 9;
                bool checkUpRight = (upRightBoard & emptyOrEnemySpaces) > 0;

                if (checkUpRight)
                {
                    ulong nearestUpRightPiece = FindUpRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong upLeftBoard = squarePositionBoard << 7;
                bool checkUpLeft = (upLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkUpLeft)
                {
                    ulong nearestUpLeftPiece = FindUpLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpLeftPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong downRightBoard = squarePositionBoard >> 7;
                bool checkDownRight = (downRightBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownRight)
                {
                    ulong nearestDownRightPiece = FindDownRightBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
                        return true;
                }

                ulong downLeftBoard = squarePositionBoard >> 9;
                bool checkDownLeft = (downLeftBoard & emptyOrEnemySpaces) > 0;
                
                if (checkDownLeft)
                {
                    ulong nearestDownLeftPiece = FindDownLeftBlockingPosition(board, squarePositionBoard);

                    if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
                        return true;
                }
            }

            return false;
        }

        #endregion fast attack methods

        #endregion is square attacked methods

        #region Calculate allowed moves methods

        internal static ulong CalculateAllowedBishopMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedBishopMoves(Board board, ulong square, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpRightMoves(board, square, colour) |
                    BoardChecking.CalculateAllowedDownRightMoves(board, square, colour) |
                    BoardChecking.CalculateAllowedDownLeftMoves(board, square, colour) |
                    BoardChecking.CalculateAllowedUpLeftMoves(board, square, colour));
        }

        internal static ulong CalculateAllowedRookMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedRightMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedDownMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedRookMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedRightMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedDownMoves(board, pieceIndex, colour) |
                    BoardChecking.CalculateAllowedLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedQueenMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedLeftMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedQueenMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            return (BoardChecking.CalculateAllowedUpMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedLeftMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedUpRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownRightMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedDownLeftMoves(board, pieceIndex, colour) |
                     BoardChecking.CalculateAllowedUpLeftMoves(board, pieceIndex, colour));
        }

        internal static ulong CalculateAllowedUpMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong upBoard = LookupTables.UpBoard[pieceIndex];

            ulong upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square
            upMoves = (upMoves << 8) | (upMoves << 16) | (upMoves << 24) | (upMoves << 32) | (upMoves << 40) | (upMoves << 48);  //Fill all squares above by performing left shifts
            //upMoves = upMoves & upBoard;       //Remove overflow
            upMoves = upMoves ^ upBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upMoves = upMoves & board.BlackOrEmpty;
            else
                upMoves = upMoves & board.WhiteOrEmpty;

            return upMoves;
        }

        internal static ulong CalculateAllowedUpMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            ulong upBoard = GetUpBoard(pieceIndex);

            ulong upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square
            upMoves = (upMoves << 8) | (upMoves << 16) | (upMoves << 24) | (upMoves << 32) | (upMoves << 40) | (upMoves << 48);  //Fill all squares above by performing left shifts
            //upMoves = upMoves & upBoard;       //Remove overflow
            upMoves = upMoves ^ upBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upMoves = upMoves & board.BlackOrEmpty;
            else
                upMoves = upMoves & board.WhiteOrEmpty;

            return upMoves;
        }

        internal static ulong CalculateAllowedRightMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong rightBoard = LookupTables.RightBoard[pieceIndex];

            ulong rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square
            rightMoves = (rightMoves << 1) | (rightMoves << 2) | (rightMoves << 3) | (rightMoves << 4) | (rightMoves << 5) | (rightMoves << 6);  //Fill all squares to the right by performing left shifts
            rightMoves = rightMoves & rightBoard;       //Remove overflow
            rightMoves = rightMoves ^ rightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                rightMoves = rightMoves & board.BlackOrEmpty;
            else
                rightMoves = rightMoves & board.WhiteOrEmpty;

            return rightMoves;
        }

        internal static ulong CalculateAllowedRightMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            ulong rightBoard = GetRightBoard(pieceIndex);

            ulong rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square
            rightMoves = (rightMoves << 1) | (rightMoves << 2) | (rightMoves << 3) | (rightMoves << 4) | (rightMoves << 5) | (rightMoves << 6);  //Fill all squares to the right by performing left shifts
            rightMoves = rightMoves & rightBoard;       //Remove overflow
            rightMoves = rightMoves ^ rightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                rightMoves = rightMoves & board.BlackOrEmpty;
            else
                rightMoves = rightMoves & board.WhiteOrEmpty;

            return rightMoves;
        }

        internal static ulong CalculateAllowedDownMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong downBoard = LookupTables.DownBoard[pieceIndex];

            ulong downMoves = downBoard & board.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts
            //downMoves = downMoves & downBoard;       //Remove overflow
            downMoves = downMoves ^ downBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downMoves = downMoves & board.BlackOrEmpty;
            else
                downMoves = downMoves & board.WhiteOrEmpty;

            return downMoves;
        }

        internal static ulong CalculateAllowedDownMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            ulong downBoard = GetDownBoard(pieceIndex);

            ulong downMoves = downBoard & board.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts
            //downMoves = downMoves & downBoard;       //Remove overflow
            downMoves = downMoves ^ downBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downMoves = downMoves & board.BlackOrEmpty;
            else
                downMoves = downMoves & board.WhiteOrEmpty;

            return downMoves;
        }

        internal static ulong CalculateAllowedLeftMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong leftBoard = LookupTables.LeftBoard[pieceIndex];

            ulong leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the left by performing right shifts
            leftMoves = leftMoves & leftBoard;       //Remove overflow
            leftMoves = leftMoves ^ leftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                leftMoves = leftMoves & board.BlackOrEmpty;
            else
                leftMoves = leftMoves & board.WhiteOrEmpty;

            return leftMoves;
        }

        private static ulong CalculateAllowedLeftMoves(Board board, ulong pieceIndex, PieceColour colour)
        {
            ulong leftBoard = GetLeftBoard(pieceIndex);

            ulong leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the left by performing right shifts
            leftMoves = leftMoves & leftBoard;       //Remove overflow
            leftMoves = leftMoves ^ leftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                leftMoves = leftMoves & board.BlackOrEmpty;
            else
                leftMoves = leftMoves & board.WhiteOrEmpty;

            return leftMoves;
        }

        internal static ulong CalculateAllowedUpRightMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong upRightBoard = LookupTables.UpRightBoard[pieceIndex];

            ulong upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square
            upRightMoves = (upRightMoves << 9) | (upRightMoves << 18) | (upRightMoves << 27) | (upRightMoves << 36) | (upRightMoves << 45) | (upRightMoves << 54);  //Fill all squares above and right by performing left shifts
            upRightMoves = upRightMoves & upRightBoard;       //Remove overflow
            upRightMoves = upRightMoves ^ upRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upRightMoves = upRightMoves & board.BlackOrEmpty;
            else
                upRightMoves = upRightMoves & board.WhiteOrEmpty;

            return upRightMoves;
        }

        internal static ulong CalculateAllowedUpRightMoves(Board board, ulong piecePosition, PieceColour colour)
        {
            //ulong upRightBoard = LookupTables.UpRightBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            ulong upRightBoard = GetUpRightBoard(piecePosition);

            ulong upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square
            upRightMoves = (upRightMoves << 9) | (upRightMoves << 18) | (upRightMoves << 27) | (upRightMoves << 36) | (upRightMoves << 45) | (upRightMoves << 54);  //Fill all squares above and right by performing left shifts
            upRightMoves = upRightMoves & upRightBoard;       //Remove overflow
            upRightMoves = upRightMoves ^ upRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upRightMoves = upRightMoves & board.BlackOrEmpty;
            else
                upRightMoves = upRightMoves & board.WhiteOrEmpty;

            return upRightMoves;
        }

        internal static ulong CalculateAllowedDownRightMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong downRightBoard = LookupTables.DownRightBoard[pieceIndex];

            ulong downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square
            downRightMoves = (downRightMoves >> 7) | (downRightMoves >> 14) | (downRightMoves >> 21) | (downRightMoves >> 28) | (downRightMoves >> 35) | (downRightMoves >> 42);  //Fill all squares below and right by performing left shifts
            downRightMoves = downRightMoves & downRightBoard;       //Remove overflow
            downRightMoves = downRightMoves ^ downRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downRightMoves = downRightMoves & board.BlackOrEmpty;
            else
                downRightMoves = downRightMoves & board.WhiteOrEmpty;

            return downRightMoves;
        }

        internal static ulong CalculateAllowedDownRightMoves(Board board, ulong piecePosition, PieceColour colour)
        {
            //ulong downRightBoard = LookupTables.DownRightBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            ulong downRightBoard = GetDownRightBoard(piecePosition);

            ulong downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square
            downRightMoves = (downRightMoves >> 7) | (downRightMoves >> 14) | (downRightMoves >> 21) | (downRightMoves >> 28) | (downRightMoves >> 35) | (downRightMoves >> 42);  //Fill all squares below and right by performing left shifts
            downRightMoves = downRightMoves & downRightBoard;       //Remove overflow
            downRightMoves = downRightMoves ^ downRightBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downRightMoves = downRightMoves & board.BlackOrEmpty;
            else
                downRightMoves = downRightMoves & board.WhiteOrEmpty;

            return downRightMoves;
        }

        internal static ulong CalculateAllowedDownLeftMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong downLeftBoard = LookupTables.DownLeftBoard[pieceIndex];

            ulong downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below and left by performing right shifts
            downLeftMoves = downLeftMoves & downLeftBoard;       //Remove overflow
            downLeftMoves = downLeftMoves ^ downLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downLeftMoves = downLeftMoves & board.BlackOrEmpty;
            else
                downLeftMoves = downLeftMoves & board.WhiteOrEmpty;

            return downLeftMoves;
        }

        internal static ulong CalculateAllowedDownLeftMoves(Board board, ulong piecePosition, PieceColour colour)
        {
            //ulong downLeftBoard = LookupTables.DownLeftBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            ulong downLeftBoard = GetDownLeftBoard(piecePosition);

            ulong downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below and left by performing right shifts
            downLeftMoves = downLeftMoves & downLeftBoard;       //Remove overflow
            downLeftMoves = downLeftMoves ^ downLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                downLeftMoves = downLeftMoves & board.BlackOrEmpty;
            else
                downLeftMoves = downLeftMoves & board.WhiteOrEmpty;

            return downLeftMoves;
        }

        internal static ulong CalculateAllowedUpLeftMoves(Board board, byte pieceIndex, PieceColour colour)
        {
            ulong upLeftBoard = LookupTables.UpLeftBoard[pieceIndex];

            ulong upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            upLeftMoves = (upLeftMoves << 7) | (upLeftMoves << 14) | (upLeftMoves << 21) | (upLeftMoves << 28) | (upLeftMoves << 35) | (upLeftMoves << 42);  //Fill all squares up and left by performing right shifts
            upLeftMoves = upLeftMoves & upLeftBoard;       //Remove overflow
            upLeftMoves = upLeftMoves ^ upLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upLeftMoves = upLeftMoves & board.BlackOrEmpty;
            else
                upLeftMoves = upLeftMoves & board.WhiteOrEmpty;

            return upLeftMoves;
        }

        internal static ulong CalculateAllowedUpLeftMoves(Board board, ulong piecePosition, PieceColour colour)
        {
            //ulong upLeftBoard = LookupTables.UpLeftBoard[BitboardOperations.GetSquareIndexFromBoardValue(piecePosition)];
            ulong upLeftBoard = GetUpLeftBoard(piecePosition);

            ulong upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            upLeftMoves = (upLeftMoves << 7) | (upLeftMoves << 14) | (upLeftMoves << 21) | (upLeftMoves << 28) | (upLeftMoves << 35) | (upLeftMoves << 42);  //Fill all squares up and left by performing right shifts
            upLeftMoves = upLeftMoves & upLeftBoard;       //Remove overflow
            upLeftMoves = upLeftMoves ^ upLeftBoard;       //Get just the allowed squares using XOR

            //Remove the blocking piece if it can't be captured (i.e. It is a friendly piece)
            if (colour == PieceColour.White)
                upLeftMoves = upLeftMoves & board.BlackOrEmpty;
            else
                upLeftMoves = upLeftMoves & board.WhiteOrEmpty;

            return upLeftMoves;
        }

        #endregion Calculate allowed moves methods


        #region find nearest piece searches

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, up from the given squarePosition
        /// </summary>
        internal static ulong FindUpBlockingPosition(Board board, ulong square)
        {
            ulong upBoard = GetUpBoard(square);

            ulong upMoves = upBoard & board.AllOccupiedSquares;   //Find first hit square
            upMoves = (upMoves << 8) | (upMoves << 16) | (upMoves << 24) | (upMoves << 32) | (upMoves << 40) | (upMoves << 48);  //Fill all squares above by performing left shifts

            //upMoves = upMoves & upBoard;       //Remove overflow

            upMoves = upMoves ^ upBoard;        //Just allowed squares

            return upMoves & board.AllOccupiedSquares;
        }

        internal static ulong FindUpRightBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong upRightBoard = LookupTables.UpRightBoard[squareIndex];

            ulong upRightBoard = GetUpRightBoard(square);

            ulong upRightMoves = upRightBoard & board.AllOccupiedSquares;   //Find first hit square
            upRightMoves = (upRightMoves << 9) | (upRightMoves << 18) | (upRightMoves << 27) | (upRightMoves << 36) | (upRightMoves << 45) | (upRightMoves << 54);  //Fill all squares above and right by performing left shifts

            upRightMoves = upRightMoves & upRightBoard;       //Remove overflow
            ulong notAboveRight = upRightMoves ^ upRightBoard;

            return notAboveRight & board.AllOccupiedSquares;
        }

        internal static ulong FindRightBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong rightBoard = LookupTables.RightBoard[squareIndex];
            ulong rightBoard = GetRightBoard(square);
            
            ulong rightMoves = rightBoard & board.AllOccupiedSquares;   //Find first hit square
            rightMoves = (rightMoves << 1) | (rightMoves << 2) | (rightMoves << 3) | (rightMoves << 4) | (rightMoves << 5) | (rightMoves << 6);  //Fill all squares to the right by performing left shifts

            rightMoves = rightMoves & rightBoard;       //Remove overflow

            ulong notRight = rightMoves ^ rightBoard;

            return notRight & board.AllOccupiedSquares;
        }

        internal static ulong FindDownRightBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong downRightBoard = LookupTables.DownRightBoard[squareIndex];

            ulong downRightBoard = GetDownRightBoard(square);
            
            ulong downRightMoves = downRightBoard & board.AllOccupiedSquares;   //Find first hit square
            downRightMoves = (downRightMoves >> 7) | (downRightMoves >> 14) | (downRightMoves >> 21) | (downRightMoves >> 28) | (downRightMoves >> 35) | (downRightMoves >> 42);  //Fill all squares below-right by performing right shifts

            downRightMoves = downRightMoves & downRightBoard;       //Remove overflow

            ulong notBelowRight = downRightMoves ^ downRightBoard;

            return notBelowRight & board.AllOccupiedSquares;
        }

        /// <summary>
        /// Returns the bitboard value of the first piece, of any colour, down from the given square
        /// </summary>
        internal static ulong FindDownBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong downBoard = LookupTables.DownBoard[squareIndex];

            ulong downBoard = GetDownBoard(square);

            ulong downMoves = downBoard & board.AllOccupiedSquares;   //Find first hit square
            downMoves = (downMoves >> 8) | (downMoves >> 16) | (downMoves >> 24) | (downMoves >> 32) | (downMoves >> 40) | (downMoves >> 48);  //Fill all squares below by performing right shifts

            //downMoves = downMoves & downBoard;       //Remove overflow

            ulong notBelow = downMoves ^ downBoard;
            return notBelow & board.AllOccupiedSquares;
        }

        internal static ulong FindDownLeftBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong downLeftBoard = LookupTables.DownLeftBoard[squareIndex];

            ulong downLeftBoard = GetDownLeftBoard(square);

            ulong downLeftMoves = downLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            downLeftMoves = (downLeftMoves >> 9) | (downLeftMoves >> 18) | (downLeftMoves >> 27) | (downLeftMoves >> 36) | (downLeftMoves >> 45) | (downLeftMoves >> 54);  //Fill all squares below-left by performing right shifts

            downLeftMoves = downLeftMoves & downLeftBoard;       //Remove overflow

            ulong notBelowRight = downLeftMoves ^ downLeftBoard;
            return notBelowRight & board.AllOccupiedSquares;
        }

        internal static ulong FindLeftBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong leftBoard = LookupTables.LeftBoard[squareIndex];

            ulong leftBoard = GetLeftBoard(square);

            ulong leftMoves = leftBoard & board.AllOccupiedSquares;   //Find first hit square
            leftMoves = (leftMoves >> 1) | (leftMoves >> 2) | (leftMoves >> 3) | (leftMoves >> 4) | (leftMoves >> 5) | (leftMoves >> 6);  //Fill all squares to the right by performing left shifts

            leftMoves = leftMoves & leftBoard;       //Remove overflow

            ulong notLeft = leftMoves ^ leftBoard;

            return notLeft & board.AllOccupiedSquares;
        }

        internal static ulong FindUpLeftBlockingPosition(Board board, ulong square)
        {
            //int squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(square);
            //ulong upLeftBoard = LookupTables.UpLeftBoard[squareIndex];
            ulong upLeftBoard = GetUpLeftBoard(square);

            ulong upLeftMoves = upLeftBoard & board.AllOccupiedSquares;   //Find first hit square
            upLeftMoves = (upLeftMoves << 7) | (upLeftMoves << 14) | (upLeftMoves << 21) | (upLeftMoves << 28) | (upLeftMoves << 35) | (upLeftMoves << 42);  //Fill all squares above and right by performing left shifts

            upLeftMoves = upLeftMoves & upLeftBoard;       //Remove overflow

            ulong notAboveLeft = upLeftMoves ^ upLeftBoard;

            return notAboveLeft & board.AllOccupiedSquares;
        }

        #endregion find nearest piece searches

        #region shifts

          private static ulong GetSurroundingSpace(ulong pieceBoard)
          {
              ulong notA = (ulong)18374403900871474942;    //All squares except A column
              ulong notH = (ulong)9187201950435737471;     //All squares except H column

            return ((pieceBoard << 7) & notH) | (pieceBoard << 8) | ((pieceBoard << 9) & notA) |
               ((pieceBoard >> 1) & notH) | ((pieceBoard << 1) & notA) |
               ((pieceBoard >> 9) & notH) | (pieceBoard >> 8) | ((pieceBoard >> 7) & notA);
         }
         

        #endregion shifts

        #region ray boards

        private static ulong GetUpRightBoard(ulong square)
        {
            ulong notA = 18374403900871474942;

            ulong shift1 = (square << 9) & notA;
            ulong shift2 = shift1 | ((shift1 << 9) & notA);
            ulong shift3 = shift2 | ((shift2 << 9) & notA);
            ulong shift4 = shift3 | ((shift3 << 9) & notA);
            ulong shift5 = shift4 | ((shift4 << 9) & notA);
            ulong shift6 = shift5 | ((shift5 << 9) & notA);
            ulong shift7 = shift6 | ((shift6 << 9) & notA);

            return shift7;            
        }

        private static ulong GetUpBoard(ulong square)
        {
            return (square << 8) | (square << 16) | (square << 24) | (square << 32) | (square << 40) | (square << 48) | (square << 56);
        }

        private static ulong GetRightBoard(ulong square)
        {
            ulong notA = 18374403900871474942;

            ulong shift1 = (square << 1) & notA;
            ulong shift2 = shift1 | ((shift1 << 1) & notA);
            ulong shift3 = shift2 | ((shift2 << 1) & notA);
            ulong shift4 = shift3 | ((shift3 << 1) & notA);
            ulong shift5 = shift4 | ((shift4 << 1) & notA);
            ulong shift6 = shift5 | ((shift5 << 1) & notA);
            ulong shift7 = shift6 | ((shift6 << 1) & notA);

            return shift7; 
        }

        private static ulong GetDownRightBoard(ulong square)
        {
            ulong notA = 18374403900871474942;

            ulong shift1 = (square >> 7) & notA;
            ulong shift2 = shift1 | ((shift1 >> 7) & notA);
            ulong shift3 = shift2 | ((shift2 >> 7) & notA);
            ulong shift4 = shift3 | ((shift3 >> 7) & notA);
            ulong shift5 = shift4 | ((shift4 >> 7) & notA);
            ulong shift6 = shift5 | ((shift5 >> 7) & notA);
            ulong shift7 = shift6 | ((shift6 >> 7) & notA);

            return shift7; 
        }

        private static ulong GetDownBoard(ulong square)
        {
            return (square >> 8) | (square >> 16) | (square >> 24) | (square >> 32) | (square >> 40) | (square >> 48) | (square >> 56);
        }

         private static ulong GetDownLeftBoard(ulong square)
        {
            ulong notH = 9187201950435737471;

            ulong shift1 = (square >> 9) & notH;
            ulong shift2 = shift1 | ((shift1 >> 9) & notH);
            ulong shift3 = shift2 | ((shift2 >> 9) & notH);
            ulong shift4 = shift3 | ((shift3 >> 9) & notH);
            ulong shift5 = shift4 | ((shift4 >> 9) & notH);
            ulong shift6 = shift5 | ((shift5 >> 9) & notH);
            ulong shift7 = shift6 | ((shift6 >> 9) & notH);

            return shift7; 
        }

         private static ulong GetLeftBoard(ulong square)
         {
             ulong notH = 9187201950435737471;

            ulong shift1 = (square >> 1) & notH;
            ulong shift2 = shift1 | ((shift1 >> 1) & notH);
            ulong shift3 = shift2 | ((shift2 >> 1) & notH);
            ulong shift4 = shift3 | ((shift3 >> 1) & notH);
            ulong shift5 = shift4 | ((shift4 >> 1) & notH);
            ulong shift6 = shift5 | ((shift5 >> 1) & notH);
            ulong shift7 = shift6 | ((shift6 >> 1) & notH);

            return shift7; 
         }

         private static ulong GetUpLeftBoard(ulong square)
        {
            ulong notH = 9187201950435737471;

            ulong shift1 = (square << 7) & notH;
            ulong shift2 = shift1 | ((shift1 << 7) & notH);
            ulong shift3 = shift2 | ((shift2 << 7) & notH);
            ulong shift4 = shift3 | ((shift3 << 7) & notH);
            ulong shift5 = shift4 | ((shift4 << 7) & notH);
            ulong shift6 = shift5 | ((shift5 << 7) & notH);
            ulong shift7 = shift6 | ((shift6 << 7) & notH);

            return shift7; 
        }

        #endregion ray boards
    }
}
