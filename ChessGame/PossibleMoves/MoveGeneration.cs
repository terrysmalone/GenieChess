using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;

namespace ChessGame.PossibleMoves
{
    /// <summary>
    /// Calculates all possible moves from a given position
    /// </summary>
    public static class MoveGeneration
    {
        #region private properties

        private static Board currentBoard;

        static List<PieceMoves> allMovesList = new List<PieceMoves>();

        private static PieceColour friendlyColour = PieceColour.White;     //These are swapped at the end of every...
       
        private static int checkCount = 0;     //Used to find if king is in double check
        
        #endregion Private properties
        
        #region Calculate moves methods
        
        /// <summary>
        /// Returns all legal moves
        /// </summary>
        /// <returns></returns>
        public static List<PieceMoves> CalculateAllMoves(Board board)
        {
            allMovesList.Clear();
            checkCount = 0;

            PieceValidMoves.GenerateMoveArrays();

            CalculateAllPseudoLegalMoves(board);
           
            RemoveCheckingMoves();

            return allMovesList;
        }

        /// <summary>
        /// Returns all pseudo legal moves (i.e. all possible moves including those that put the king in check)
        /// </summary>
        /// <param name="board"></param>
        public static List<PieceMoves> CalculateAllPseudoLegalMoves(Board board)
        {
            allMovesList.Clear();
            checkCount = 0;

            PieceValidMoves.GenerateMoveArrays();

            currentBoard = board;
            currentBoard.CalculateUsefulBitboards();

            SetFriendlyPlayerColour();

            allMovesList.Clear();

            CalculateKnightMoves();
            CalculateBishopMoves(friendlyColour);
            CalculateRookMoves(friendlyColour);
            CalculateQueenMoves(friendlyColour);
            CalculatePawnMoves();
            CalculateKingMoves();

            CheckForCastlingMoves();

            return allMovesList;
        } 

        /// <summary>
        /// Sets friendly colour based on whose turn it is to move
        /// </summary>
        private static void SetFriendlyPlayerColour()
        {
            friendlyColour = currentBoard.WhiteToMove ? PieceColour.White : PieceColour.Black;
        }
        

        /// <summary>
        /// Checks all moves list and removes any that would put king in check
        /// </summary>
        private static void RemoveCheckingMoves()
        {
            //Search backwards so we can remove moves and carry on
            for (int i = allMovesList.Count - 1; i >= 0; i--)
            {
                PieceMoves currentMove = allMovesList[i];
                
                currentBoard.MakeMove(currentMove.Position, currentMove.Moves, currentMove.Type, currentMove.SpecialMove, false);

                if (BoardChecking.IsKingInCheckFast(currentBoard, friendlyColour))
                {
                    allMovesList.RemoveAt(i);
                }

                currentBoard.UnMakeLastMove();
            }
        }

        /// <summary>
        /// Checks that the last move was legal by ensuring that the player who has just moved is not in check
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool ValidateMove(Board board)
        {
            bool valid;
            
            PieceColour colourToCheck;

            if (board.WhiteToMove)
            {
                colourToCheck = PieceColour.Black;
            }
            else
                colourToCheck = PieceColour.White; 

            if (BoardChecking.IsKingInCheckFast(board, colourToCheck))
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
        internal static bool ValidateCastlingMove(Board boardPosition, PieceMoves currentMove)
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
            checkCount = 0;

            ulong friendlyKing;

            if (friendlyColour == PieceColour.White)
                friendlyKing = currentBoard.WhiteKing;
            else
                friendlyKing = currentBoard.BlackKing;

            IsSquareAttacked(friendlyKing, friendlyColour);
                   
            if(checkCount > 0)
                return true;
            else 
                return false;          
        }

        #region Pawn moves

        private static void CalculatePawnMoves()
        {
            if (friendlyColour == PieceColour.White)
                CalculateWhitePawnMoves();

            else
                CalculateBlackPawnMoves();
        }

        private static void CalculateWhitePawnMoves()
        {
            ulong[] whitePawnPositions = BitboardOperations.SplitBoardToArray(currentBoard.WhitePawns);

            for (int i = 0; i < whitePawnPositions.Length; i++)
            {
                ulong currentPosition = whitePawnPositions[i];
            
                ulong pawnSingleMove = currentPosition << 8;

                //Check for promotions
                ulong promotionsBoard = (pawnSingleMove & LookupTables.RankMask8) & ~currentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0)    //There are promortions. Split moves
                {
                    //Remove promotions from pawn moves
                    //pawnSingleMove = pawnSingleMove & ~promotionsBoard;

                    //Add promotions to a new move
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.KnightPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.BishopPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.RookPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.QueenPromotion });
                }
                else
                {
                    pawnSingleMove = pawnSingleMove & currentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnSingleMove, SpecialMove = SpecialMoveType.Normal });

                        //A pawn that can be promoted can't double move so we're safe to check here
                        //Check for double moves

                        ulong pawnDoubleMove = (currentPosition << 16) & currentBoard.EmptySquares;

                        if ((currentPosition & LookupTables.RankMask2) != 0 && pawnDoubleMove > 0)    //If on start rank and first rank is not blocked
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnDoubleMove, SpecialMove = SpecialMoveType.DoublePawnPush });
                        }
                    }
                }

                ulong possiblePawnCaptures = ValidMoveArrays.WhitePawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];

                ulong pawnEnPassantCaptures = possiblePawnCaptures & currentBoard.EnPassantPosition;

                if (pawnEnPassantCaptures > 0)
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnEnPassantCaptures, SpecialMove = SpecialMoveType.ENPassantCapture });

                ulong pawnCaptures = possiblePawnCaptures & currentBoard.AllBlackOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    ulong capturePromotionsBoard = pawnCaptures & LookupTables.RankMask8;

                    if (capturePromotionsBoard > 0)
                    {
                        //pawnCaptures = pawnCaptures & ~capturePromotionsBoard;

                        ulong[] pawnPromotionCapturesSingleBoard = BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (ulong move in pawnPromotionCapturesSingleBoard)
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.KnightPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.BishopPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.RookPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.QueenPromotionCapture });
                        }
                    }
                    else
                    {
                        ulong[] pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (ulong move in pawnCapturesSingleBoard)
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.Capture });
                        }
                    }
                }
            }
        }

        //private static void CalculateWhitePawnMoves()
        //{
        //    //see https://chessprogramming.wikispaces.com/Pawn+Pushes+(Bitboards) 
        //    //Change it so we calc moves as one and only split later

        //    ulong rank8 = 18374686479671623680;
        //    ulong rank3 = 16711680;
        //    ulong fileA = 72340172838076673;
        //    ulong fileH = 9259542123273814144;

        //    ulong singlePushPawns = (currentBoard.WhitePawns << 8) & currentBoard.EmptySquares;
        //    ulong doublePushPawns = ((singlePushPawns & rank3) << 8) & currentBoard.EmptySquares;

        //    ulong singlePushPromotions = singlePushPawns & rank8;

        //    //Move promotions
        //    if (singlePushPromotions != 0)
        //    {
        //        List<ulong> singlePromotions = BitboardOperations.SplitBoard(singlePushPromotions);

        //        foreach (ulong moveTo in singlePromotions)
        //        {
        //            ulong moveFrom = moveTo >> 8;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.KnightPromotion });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.BishopPromotion });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.RookPromotion });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.QueenPromotion });
        //        }
        //    }

        //    //Moves
        //    singlePushPawns &= ~rank8;

        //    if (singlePushPawns != 0)
        //    {
        //        List<ulong> singlePushes = BitboardOperations.SplitBoard(singlePushPawns);

        //        foreach (ulong moveTo in singlePushes)
        //        {
        //            ulong moveFrom = moveTo >> 8;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.Normal });
        //        }
        //    }

        //    if (doublePushPawns != 0)
        //    {
        //        List<ulong> doublePushes = BitboardOperations.SplitBoard(doublePushPawns);

        //        foreach (ulong moveTo in doublePushes)
        //        {
        //            ulong moveFrom = moveTo >> 16;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.DoublePawnPush });
        //        }
        //    }

        //    // Attacks
        //    ulong leftAttacks = (currentBoard.WhitePawns << 7) & (currentBoard.AllBlackOccupiedSquares & ~rank8);

        //    leftAttacks &= ~fileH;

        //    if (leftAttacks != 0)
        //    {
        //        List<ulong> lefties = BitboardOperations.SplitBoard(leftAttacks);

        //        foreach (ulong moveTo in lefties)
        //        {
        //            ulong moveFrom = moveTo >> 7;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.Capture });
        //        }
        //    }

        //    ulong rightAttacks = (currentBoard.WhitePawns << 9) & (currentBoard.AllBlackOccupiedSquares & ~rank8);

        //    rightAttacks &= ~fileA;

        //    if (rightAttacks != 0)
        //    {
        //        List<ulong> righties = BitboardOperations.SplitBoard(rightAttacks);

        //        foreach (ulong moveTo in righties)
        //        {
        //            ulong moveFrom = moveTo >> 9;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.Capture });
        //        }
        //    }

        //    ulong leftAttackEP = (currentBoard.WhitePawns << 7) & currentBoard.EnPassantPosition;
        //    leftAttackEP &= ~fileH;

        //    if (leftAttackEP != 0)
        //    {
        //        ulong moveFrom = leftAttackEP >> 7;
        //        allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = leftAttackEP, SpecialMove = SpecialMoveType.ENPassantCapture });
        //    }

        //    ulong rightAttackEP = (currentBoard.WhitePawns << 9) & currentBoard.EnPassantPosition;
        //    rightAttackEP &= ~fileA;

        //    if (rightAttackEP != 0)
        //    {
        //        ulong moveFrom = rightAttackEP >> 9;
        //        allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = rightAttackEP, SpecialMove = SpecialMoveType.ENPassantCapture });
        //    }


        //    //Promotion captures
        //    ulong leftAttackPromotions = (currentBoard.WhitePawns << 7) & (currentBoard.AllBlackOccupiedSquares & rank8);
        //    leftAttackPromotions &= ~fileH;

        //    if (leftAttackPromotions != 0)
        //    {
        //        List<ulong> leftAP = BitboardOperations.SplitBoard(leftAttackPromotions);

        //        foreach (ulong moveTo in leftAP)
        //        {
        //            ulong moveFrom = moveTo >> 7;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.KnightPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.BishopPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.RookPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.QueenPromotionCapture });
        //        }
        //    }

        //    ulong rightAttackPromotions = (currentBoard.WhitePawns << 9) & (currentBoard.AllBlackOccupiedSquares & rank8);
        //    rightAttackPromotions &= ~fileA;

        //    if (rightAttackPromotions != 0)
        //    {
        //        List<ulong> rightAP = BitboardOperations.SplitBoard(rightAttackPromotions);

        //        foreach (ulong moveTo in rightAP)
        //        {
        //            ulong moveFrom = moveTo >> 9;

        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.KnightPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.BishopPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.RookPromotionCapture });
        //            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = moveFrom, Moves = moveTo, SpecialMove = SpecialMoveType.QueenPromotionCapture });
        //        }
        //    }
        //}

        private static void CalculateBlackPawnMoves()
        {
            ulong[] blackPawnPositions = BitboardOperations.SplitBoardToArray(currentBoard.BlackPawns);

            foreach (ulong currentPosition in blackPawnPositions)
            {
                ulong pawnSingleMove = currentPosition >> 8;

                //Check for promotions
                ulong promotionsBoard = (pawnSingleMove & LookupTables.RankMask1) & ~currentBoard.AllOccupiedSquares;

                if (promotionsBoard > 0)    //There are promortions. Split moves
                {
                    //Remove promotions from pawn moves
                    //pawnSingleMove = pawnSingleMove & ~promotionsBoard;

                    //Add promotions to a new move
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.KnightPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.BishopPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.RookPromotion });
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = promotionsBoard, SpecialMove = SpecialMoveType.QueenPromotion });
                }
                else
                {
                    pawnSingleMove = pawnSingleMove & currentBoard.EmptySquares;

                    if (pawnSingleMove > 0)
                    {
                        allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnSingleMove, SpecialMove = SpecialMoveType.Normal });

                        //A pawn that can be promoted can't double move so we're safe to check here
                        //Check for double moves
                        ulong pawnDoubleMove = (currentPosition >> 16) & currentBoard.EmptySquares;

                        if ((currentPosition & LookupTables.RankMask7) != 0 && pawnDoubleMove != 0)   //If on start rank and first rank is not blocked
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnDoubleMove, SpecialMove = SpecialMoveType.DoublePawnPush });
                        }
                    }
                }

                ulong possiblePawnCaptures = ValidMoveArrays.BlackPawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];
                
                ulong pawnEnPassantCaptures = possiblePawnCaptures & currentBoard.EnPassantPosition;

                if(pawnEnPassantCaptures > 0)
                    allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = pawnEnPassantCaptures, SpecialMove = SpecialMoveType.ENPassantCapture });

                ulong pawnCaptures = possiblePawnCaptures & currentBoard.AllWhiteOccupiedSquares;

                if (pawnCaptures > 0)
                {
                    ulong capturePromotionsBoard = pawnCaptures & LookupTables.RankMask1;

                    if(capturePromotionsBoard > 0)
                    {
                        //pawnCaptures = pawnCaptures & ~capturePromotionsBoard;

                        ulong[] pawnPromotionCapturesSingleBoard = BitboardOperations.SplitBoardToArray(capturePromotionsBoard);

                        foreach (ulong move in pawnPromotionCapturesSingleBoard)
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.KnightPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.BishopPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.RookPromotionCapture });
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.QueenPromotionCapture });
                        }
                    }
                    else
                    {
                        ulong[] pawnCapturesSingleBoard = BitboardOperations.SplitBoardToArray(pawnCaptures);

                        foreach (ulong move in pawnCapturesSingleBoard)
                        {
                            allMovesList.Add(new PieceMoves { Type = PieceType.Pawn, Position = currentPosition, Moves = move, SpecialMove = SpecialMoveType.Capture });
                        }                    
                    }
                }

            //List<ulong> blackPawnPositions = BitboardOperations.SplitBoard(currentBoard.BlackPawns);

            //foreach (ulong currentPosition in blackPawnPositions)
            //{
            //    PieceType pieceType = PieceType.Pawn;
                
            //    ulong pawnMoves = currentPosition >> 8;

            //    pawnMoves = pawnMoves & currentBoard.EmptySquares;

            //    if ((currentPosition & LookupTables.RankMask7) != 0 && pawnMoves != 0)    //If on start rank
            //    {
            //        //Move forward again (if there was a piece one ahead pawn moves will be zero
            //        pawnMoves |= pawnMoves >> 8;

            //    }

            //    pawnMoves = pawnMoves & currentBoard.EmptySquares;

            //    currentBoard.PossiblePawnCaptures = ValidMoveArrays.BlackPawnCaptures[BitboardOperations.GetSquareIndexFromBoardValue(currentPosition)];
            //    pawnMoves = pawnMoves | (currentBoard.PossiblePawnCaptures & currentBoard.AllWhiteOccupiedSquares) | currentBoard.PossiblePawnCaptures & currentBoard.EnPassantPosition;

            //    //Check for promotions
            //    ulong promotionsBoard = pawnMoves & LookupTables.RankMask1;

            //    if (promotionsBoard > 0)    //There are promotions. Split moves
            //    {
            //        //Remove promotions from pawn moves
            //        pawnMoves = pawnMoves & ~promotionsBoard;

            //        List<ulong> promotionsBoards = BitboardOperations.SplitBoard(promotionsBoard);

            //        foreach (ulong move in promotionsBoards)
            //        {
            //            //Add promotions to a new move
            //            allMovesList.Add(new PieceMoves { Type = PieceType.Knight, Position = currentPosition, Moves = move });
            //            allMovesList.Add(new PieceMoves { Type = PieceType.Bishop, Position = currentPosition, Moves = move });
            //            allMovesList.Add(new PieceMoves { Type = PieceType.Rook, Position = currentPosition, Moves = move });
            //            allMovesList.Add(new PieceMoves { Type = PieceType.Queen, Position = currentPosition, Moves = move });
            //        }
            //    }

            //    SplitAndAddMoves(pawnMoves, currentPosition, pieceType);
            }
        }

        #endregion Pawn moves

        #region Knight moves

        private static void CalculateKnightMoves()
        {
            if (friendlyColour == PieceColour.White)
                CalculateWhiteKnightMoves();
            else
                CalculateBlackKnightMoves();
        }

        private static void CalculateWhiteKnightMoves()
        {
            List<byte> whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteKnights);

            int index = whiteKnightPositions.Count - 1;

            while (index >= 0)
            {                
                ulong currentPosition = whiteKnightPositions[index];

                ulong piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                PieceType pieceType = PieceType.Knight;

                ulong possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                ulong[] splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (ulong move in splitMoves)
                {
                    if ((move & currentBoard.AllBlackOccupiedSquares) > 0)
                        allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Capture });

                    if ((move & currentBoard.EmptySquares) > 0)
                        allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Normal });     
                }

                index--;
            }
        }

        private static void CalculateBlackKnightMoves()
        {
            List<byte> blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackKnights);

            int index = blackKnightPositions.Count - 1;

            while (index >= 0)
            {
                ulong currentPosition = blackKnightPositions[index];

                ulong piecePosition = LookupTables.SquareValuesFromIndex[currentPosition];
                PieceType pieceType = PieceType.Knight;

                ulong possibleMoves = ValidMoveArrays.KnightMoves[currentPosition];

                ulong[] splitMoves = BitboardOperations.SplitBoardToArray(possibleMoves);

                foreach (ulong move in splitMoves)
                {
                    if ((move & currentBoard.AllWhiteOccupiedSquares) > 0)
                        allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Capture });

                    if ((move & currentBoard.EmptySquares) > 0)
                        allMovesList.Add(new PieceMoves { Type = pieceType, Position = piecePosition, Moves = move, SpecialMove = SpecialMoveType.Normal });        
                }

                index--;
            }
        }

        #endregion Knight moves

        #region Bishop moves

        /// <summary>
        /// Ray version
        /// </summary>
        /// <param name="friendlyColour"></param>
        //private void CalculateBishopMoves(PieceColour friendlyColour)
        //{
        //    List<ulong> bishopPositions;

        //    if (friendlyColour == PieceColour.White)
        //    {
        //        bishopPositions = BitboardOperations.SplitBoard(currentBoard.WhiteBishops);
        //    }
        //    else
        //    {
        //        bishopPositions = bishopPositions = BitboardOperations.SplitBoard(currentBoard.BlackBishops);
        //    }

        //    int index = bishopPositions.Count - 1;

        //    while (index >= 0)
        //    {
        //        PieceType pieceType = PieceType.Bishop;

        //        //byte bishopIndex = bishopPositions[index];
        //        ulong bishopPosition = bishopPositions[index];

        //        ulong allAllowedMoves = BoardChecking.CalculateAllowedBishopMoves(currentBoard, bishopPosition, friendlyColour);

        //        ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
        //        SplitAndAddMoves(normalMoves, bishopPosition, pieceType, SpecialMoveType.Normal);

        //        ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
        //        SplitAndAddMoves(captureMoves, bishopPosition, pieceType, SpecialMoveType.Capture);

        //        index--;
        //    }
        //}

        private static void CalculateBishopMoves(PieceColour friendlyColour)
        {
            List<byte> bishopPositions = new List<byte>();

            if (friendlyColour == PieceColour.White)
                bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteBishops);
            else
                bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackBishops);

            int index = bishopPositions.Count - 1;

            while (index >= 0)
            {
                PieceType pieceType = PieceType.Bishop;

                byte bishopIndex = bishopPositions[index];
                ulong bishopPosition = LookupTables.SquareValuesFromIndex[bishopIndex];

                ulong allAllowedMoves = BoardChecking.CalculateAllowedBishopMoves(currentBoard, bishopIndex, friendlyColour);

                ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, bishopPosition, pieceType, SpecialMoveType.Normal);

                ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, bishopPosition, pieceType, SpecialMoveType.Capture);

                index--;
            }
        }

        private static void SplitAndAddMoves(ulong moves, ulong position, PieceType type, SpecialMoveType specialMoveType)
        {
            ulong[] splitMoves = BitboardOperations.SplitBoardToArray(moves);

            foreach (ulong move in splitMoves)
            {
                if (move > 0)
                    allMovesList.Add(new PieceMoves { Type = type, Position = position, Moves = move, SpecialMove = specialMoveType});
            }
        }

        #endregion Bishop moves

        #region Rook moves
        
        /// <summary>
        /// Ray version
        /// </summary>
        /// <param name="friendlyColour"></param>
        //private void CalculateRookMoves(PieceColour friendlyColour)
        //{
        //    List<ulong> rookPositions = new List<ulong>();
            
        //    if (friendlyColour == PieceColour.White)
        //        rookPositions = BitboardOperations.SplitBoard(currentBoard.WhiteRooks);
        //    else
        //        rookPositions = BitboardOperations.SplitBoard(currentBoard.BlackRooks);

        //    int index = rookPositions.Count - 1;

        //    while (index >= 0)
        //    {
        //        PieceType pieceType = PieceType.Rook;

        //        //byte rookIndex = rookPositions[index];
        //        //ulong rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];
        //        ulong rookPosition = rookPositions[index];

        //        ulong allAllowedMoves = BoardChecking.CalculateAllowedRookMoves(currentBoard, rookPosition, friendlyColour);

        //        ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
        //        SplitAndAddMoves(normalMoves, rookPosition, pieceType, SpecialMoveType.Normal);

        //        ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
        //        SplitAndAddMoves(captureMoves, rookPosition, pieceType, SpecialMoveType.Capture); 

        //        index--;
        //    }
        //}

        private static void CalculateRookMoves(PieceColour friendlyColour)
        {
            List<byte> rookPositions = new List<byte>();

            if (friendlyColour == PieceColour.White)
                rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteRooks);
            else
                rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackRooks);

            int index = rookPositions.Count - 1;

            while (index >= 0)
            {
                PieceType pieceType = PieceType.Rook;

                byte rookIndex = rookPositions[index];
                ulong rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];

                ulong allAllowedMoves = BoardChecking.CalculateAllowedRookMoves(currentBoard, rookIndex, friendlyColour);

                ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, rookPosition, pieceType, SpecialMoveType.Normal);

                ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, rookPosition, pieceType, SpecialMoveType.Capture);

                index--;
            }
        }

        #endregion Rook moves

        #region Queen moves

        /// <summary>
        /// Ray version
        /// </summary>
        /// <param name="friendlyColour"></param>
        //private void CalculateQueenMoves(PieceColour friendlyColour)
        //{
        //    List<ulong> queenPositions = new List<ulong>();

        //    if (friendlyColour == PieceColour.White)
        //        queenPositions = BitboardOperations.SplitBoard(currentBoard.WhiteQueen);
        //    else
        //        queenPositions = BitboardOperations.SplitBoard(currentBoard.BlackQueen);

        //    int index = queenPositions.Count - 1;

        //    while (index >= 0)
        //    {
        //        PieceType pieceType = PieceType.Queen;

        //        //byte pieceIndex = queenPositions[index];
        //        ulong piecePosition = queenPositions[index];

        //        ulong allAllowedMoves = BoardChecking.CalculateAllowedQueenMoves(currentBoard, piecePosition, friendlyColour);

        //        ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
        //        SplitAndAddMoves(normalMoves, piecePosition, pieceType, SpecialMoveType.Normal);

        //        ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
        //        SplitAndAddMoves(captureMoves, piecePosition, pieceType, SpecialMoveType.Capture); 

        //        index--;
        //    }
        //}

        private static void CalculateQueenMoves(PieceColour friendlyColour)
        {
            List<byte> queenPositions = new List<byte>();

            if (friendlyColour == PieceColour.White)
                queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteQueen);
            else
                queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackQueen);

            int index = queenPositions.Count - 1;

            while (index >= 0)
            {
                PieceType pieceType = PieceType.Queen;

                byte pieceIndex = queenPositions[index];
                ulong piecePosition = LookupTables.SquareValuesFromIndex[pieceIndex];

                ulong allAllowedMoves = BoardChecking.CalculateAllowedQueenMoves(currentBoard, pieceIndex, friendlyColour);

                ulong normalMoves = allAllowedMoves & currentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, piecePosition, pieceType, SpecialMoveType.Normal);

                ulong captureMoves = allAllowedMoves & ~currentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, piecePosition, pieceType, SpecialMoveType.Capture);

                index--;
            }
        }

        #endregion Queen moves

        #region King moves

        private static void CalculateKingMoves()
        {
            byte whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(currentBoard.WhiteKing);
            byte blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(currentBoard.BlackKing);

            ulong whiteMoves = ValidMoveArrays.KingMoves[whiteKingPosition];
            ulong blackMoves = ValidMoveArrays.KingMoves[blackKingPosition];
            
            if (friendlyColour == PieceColour.White)
            {                
                if (currentBoard.WhiteKing > 0)
                {
                    ulong possibleWhiteMoves = whiteMoves & ~blackMoves;

                    ulong[] splitMoves = BitboardOperations.SplitBoardToArray(possibleWhiteMoves);

                    foreach (ulong moveBoard in splitMoves)
                    {
                        if ((moveBoard & currentBoard.EmptySquares) > 0)
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.WhiteKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Normal });
                        if ((moveBoard & currentBoard.AllBlackOccupiedSquares) > 0)
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.WhiteKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Capture });

                    }
                }
            }
            else
            {
                if (currentBoard.BlackKing > 0)
                {
                    ulong possibleBlackMoves = blackMoves & ~whiteMoves;

                    ulong[] splitMoves = BitboardOperations.SplitBoardToArray(possibleBlackMoves);

                    foreach (ulong moveBoard in splitMoves)
                    {
                        if ((moveBoard & currentBoard.EmptySquares) > 0)
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.BlackKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Normal });
                        if ((moveBoard & currentBoard.AllWhiteOccupiedSquares) > 0)
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.BlackKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Capture });
                    }
                }
            }
        }

        #endregion King moves
        
        #region Special move methods

        /// <summary>
        /// Checks if there are any enPassant moves available
        /// </summary>
        //private void CheckForEnPassantMoves()
        //{
        //    if (currentBoard.EnPassantPosition != 0)
        //    {
        //        //Check if pawns can attack enPassantSquare
        //        if ((currentBoard.PossiblePawnCaptures & currentBoard.EnPassantPosition) > 0)
        //        {
        //            ulong leftOfCapture;
        //            ulong rightOfCapture;

        //            ulong attackingPiece;

        //            //get capturing piece position
        //            if (friendlyColour == PieceColour.White)
        //            {
        //                //Check left of capture
        //                leftOfCapture = (currentBoard.EnPassantPosition >> 9) & LookupTables.RankMask5;      //Caluculate down left from capture using right shift whie checking result is on rank 5

        //                attackingPiece = currentBoard.WhitePawns & leftOfCapture;

        //                if (attackingPiece > 0)
        //                {
        //                    AddEnPassantCapture(attackingPiece, currentBoard.EnPassantPosition);
        //                }

        //                //Check right of capture
        //                rightOfCapture = (currentBoard.EnPassantPosition >> 7) & LookupTables.RankMask5;  //Caluculate down right from capture using right shift whie checking result is on rank 5

        //                attackingPiece = currentBoard.WhitePawns & rightOfCapture;

        //                if (attackingPiece > 0)
        //                {
        //                    AddEnPassantCapture(attackingPiece, currentBoard.EnPassantPosition);
        //                }

        //            }
        //            else
        //            {
        //                //Check left of capture
        //                leftOfCapture = (currentBoard.EnPassantPosition << 9) & LookupTables.RankMask4;      //Caluculate up left from capture using left shift whie checking result is on rank 4

        //                attackingPiece = currentBoard.BlackPawns & leftOfCapture;

        //                if (attackingPiece > 0)
        //                {
        //                    AddEnPassantCapture(attackingPiece, currentBoard.EnPassantPosition);
        //                }

        //                //Check right of capture
        //                rightOfCapture = (currentBoard.EnPassantPosition << 7) & LookupTables.RankMask4;  //Caluculate up right from capture using left shift whie checking result is on rank 4

        //                attackingPiece = currentBoard.BlackPawns & rightOfCapture;

        //                if (attackingPiece > 0)
        //                {
        //                    AddEnPassantCapture(attackingPiece, currentBoard.EnPassantPosition);
        //                }
        //            }

        //            PieceMoves pawnCapture = new PieceMoves();
        //            pawnCapture.Type = PieceType.Pawn;
        //        }
        //    }
        //}

        /// <summary>
        /// Adds an en passant capture to the valid movelist
        /// </summary>
        /// <param name="attackingPiece"></param>
        /// <param name="capturePosition"></param>
        private static void AddEnPassantCapture(ulong attackingPiece, ulong capturePosition)
        {
            PieceMoves enPassantCapture = new PieceMoves();
            enPassantCapture.Type = PieceType.Pawn;
            enPassantCapture.Position = attackingPiece;
            enPassantCapture.Moves = capturePosition;

            allMovesList.Add(enPassantCapture);
        }

        private static void CheckForCastlingMoves()
        {
            if (friendlyColour == PieceColour.White)
            {
                if (!BoardChecking.IsKingInCheckFast(currentBoard, PieceColour.White))
                {
                    if (currentBoard.WhiteCanCastleQueenside)
                    {
                        if (CheckIfPathIsClear(LookupTables.WhiteCastlingQueensideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingQueensideAttackPath, PieceColour.White))
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.WhiteKing, Moves = LookupTables.C1, SpecialMove = SpecialMoveType.QueenCastle });
                        }
                    }

                    if (currentBoard.WhiteCanCastleKingside)
                    {

                        if (CheckIfPathIsClear(LookupTables.WhiteCastlingKingsideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingKingsideAttackPath, PieceColour.White))
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.WhiteKing, Moves = LookupTables.G1, SpecialMove = SpecialMoveType.KingCastle });
                        }
                    }
                }
            }
            else
            {
                if (!BoardChecking.IsKingInCheckFast(currentBoard, PieceColour.White))
                {
                    if (currentBoard.BlackCanCastleQueenside)
                    {
                        if (CheckIfPathIsClear(LookupTables.BlackCastlingQueensideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingQueensideAttackPath, PieceColour.Black))
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.BlackKing, Moves = LookupTables.C8, SpecialMove = SpecialMoveType.QueenCastle });
                        }
                    }
                    if (currentBoard.BlackCanCastleKingside)
                    {
                        if (CheckIfPathIsClear(LookupTables.BlackCastlingKingsideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingKingsideAttackPath, PieceColour.Black))
                            allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = currentBoard.BlackKing, Moves = LookupTables.G8, SpecialMove = SpecialMoveType.KingCastle });
                        }
                    }
                }
            }
        }

        private static bool CheckIfPathIsClear(ulong path)
        {
            if ((path & currentBoard.EmptySquares) == path)
                return true;
            else
                return false;

        }

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
            bool squareAttacked = false;

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

            if (BoardChecking.IsSquareAttackedByKing(currentBoard, squarePositionBoard, friendlyColour))
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
            bool underRayAttack = false;

            ulong enemyQueenSquares;
            ulong enemyBishopSquares;
            ulong enemyRookSquares;

            if (friendlyColour == PieceColour.White)
            {
                enemyQueenSquares = currentBoard.BlackQueen;
                enemyBishopSquares = currentBoard.BlackBishops;
                enemyRookSquares = currentBoard.BlackRooks;
            }
            else
            {
                enemyQueenSquares = currentBoard.WhiteQueen;
                enemyBishopSquares = currentBoard.WhiteBishops;
                enemyRookSquares = currentBoard.WhiteRooks;
            }

            //Up
            ulong nearestUpPiece = BoardChecking.FindUpBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestUpPiece & enemyRookSquares) > 0 || (nearestUpPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                checkCount++;
            }

            //Left 
            ulong nearestLeftPiece = BoardChecking.FindLeftBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestLeftPiece & enemyRookSquares) > 0 || (nearestLeftPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                checkCount++;
            }

            //Right
            ulong nearestRightPiece = BoardChecking.FindRightBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestRightPiece & enemyRookSquares) > 0 || (nearestRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                
                checkCount++;
            }

            //Down
            ulong nearestDownPiece = BoardChecking.FindDownBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestDownPiece & enemyRookSquares) > 0 || (nearestDownPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;                 
                checkCount++;
            }

            //Up-right
            ulong nearestUpRightPiece = BoardChecking.FindUpRightBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestUpRightPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                checkCount++;
            }

            //Up Left
            ulong nearestUpLeftPiece = BoardChecking.FindUpLeftBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestUpLeftPiece & enemyBishopSquares) > 0 || (nearestUpRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                checkCount++;
            }

            //Down-right
            ulong nearestDownRightPiece = BoardChecking.FindDownRightBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestDownRightPiece & enemyBishopSquares) > 0 || (nearestDownRightPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;              
                checkCount++;
            }

            //Up Left
            ulong nearestDownLeftPiece = BoardChecking.FindDownLeftBlockingPosition(currentBoard, squarePositionBoard);

            if ((nearestDownLeftPiece & enemyBishopSquares) > 0 || (nearestDownLeftPiece & enemyQueenSquares) > 0)
            {
                underRayAttack = true;
                checkCount++;
            }

            return underRayAttack;
        }

        /// <summary>
        /// Checks if pawn is attacking square. There is no need to check all pawns for double-check 
        /// since only one pawn can be attacking the king at once
        /// </summary>
        private static bool IsPawnAttackingSquare(ulong squarePosition, PieceColour friendlyColour)
        {
            byte squareIndex = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);
            ulong proximityBoard = ValidMoveArrays.KingMoves[squareIndex];      //Allows the quick masking of wrapping checks

            if (friendlyColour == PieceColour.White)
            {                
                //Check up-right
                ulong upRight = squarePosition << 9;

                if ((upRight & currentBoard.BlackPawns & proximityBoard) != 0)
                {
                    checkCount++;
                    return true;
                }

                //Check up-left
                ulong upLeft = squarePosition << 7;

                if ((upLeft & currentBoard.BlackPawns & proximityBoard) != 0)
                {
                    checkCount++;
                    return true;
                }
            }
            else
            {               
                //Check down-right
                ulong downRight = squarePosition >> 7;

                if ((downRight & currentBoard.WhitePawns & proximityBoard) != 0)
                {
                    checkCount++;
                    return true;
                }

                //Check down-left
                ulong upLeft = squarePosition >> 9;

                if ((upLeft & currentBoard.WhitePawns & proximityBoard) != 0)
                {
                    checkCount++;
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
                knights = currentBoard.BlackKnights;
            else
                knights = currentBoard.WhiteKnights;

            byte currentPosition = BitboardOperations.GetSquareIndexFromBoardValue(squarePosition);

            ulong possibleKnightMoves = ValidMoveArrays.KnightMoves[currentPosition];

            ulong knightAttacks = possibleKnightMoves & knights;
            if (knightAttacks != 0)
            {
                checkCount++;
                return true;
            }
            else
                return false;

        }

        #region Castling attacks

        private static bool IsCastlingPathAttacked(ulong path, PieceColour friendlyColour)
        {
            //Calculate path positions
            ulong[] pathPositions = BitboardOperations.SplitBoardToArray(path);

            foreach (ulong position in pathPositions)
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

            if (BoardChecking.IsSquareAttackedByKing(currentBoard, squarePosition, friendlyColour))
                return true;

            if (friendlyColour == PieceColour.White)
            {
                if (BoardChecking.IsSquareRayAttackedFromAbove(currentBoard, squarePosition) || BoardChecking.IsSquareRayAttackedFromTheSide(currentBoard, squarePosition))      //White castling king willnot be attacked from below so no need to check all squares
                    return true;
            }
            else
            {
                if (BoardChecking.IsSquareRayAttackedFromBelow(currentBoard, squarePosition) || BoardChecking.IsSquareRayAttackedFromTheSide(currentBoard, squarePosition))      //Black castling king will not be attacked from above so no need to check all squares
                    return true;
            }

            return false;
        }

        #endregion castling attacks
        
        #endregion attack calculations

        #endregion Calculate moves methods

    }
}
