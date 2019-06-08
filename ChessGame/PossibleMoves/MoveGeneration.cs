using System.Collections.Generic;
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

            SetFriendlyPlayerColour();
            
            CalculateKnightMoves();
            CalculateBishopMoves();
            CalculateRookMoves();
            CalculateQueenMoves();
            CalculatePawnMoves();
            CalculateKingMoves();

            CheckForCastlingMoves();

            return _allMovesList;
        } 

        /// <summary>
        /// Sets friendly colour based on whose turn it is to move
        /// </summary>
        private static void SetFriendlyPlayerColour()
        {
            m_FriendlyColour = m_CurrentBoard.WhiteToMove ? PieceColour.White : PieceColour.Black;
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

                if (BoardChecking.IsKingInCheckFast(m_CurrentBoard, m_FriendlyColour))
                {
                    _allMovesList.RemoveAt(i);
                }

                m_CurrentBoard.UnMakeLastMove();
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

        private static void CalculatePawnMoves()
        {
            if (m_FriendlyColour == PieceColour.White)
                CalculateWhitePawnMoves();

            else
                CalculateBlackPawnMoves();
        }

        private static void CalculateWhitePawnMoves()
        {
            var whitePawnPositions = BitboardOperations.SplitBoardToArray(m_CurrentBoard.WhitePawns);

            for (var i = 0; i < whitePawnPositions.Length; i++)
            {
                var currentPosition = whitePawnPositions[i];
            
                var pawnSingleMove = currentPosition << 8;

                //Check for promotions
                var promotionsBoard = (pawnSingleMove & LookupTables.RankMask8) & ~m_CurrentBoard.AllOccupiedSquares;

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
            if (m_FriendlyColour == PieceColour.White)
                CalculateWhiteKnightMoves();
            else
                CalculateBlackKnightMoves();
        }

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

        private static void CalculateBishopMoves()
        {
            var bishopPositions = new List<byte>();

            if (m_FriendlyColour == PieceColour.White)
                bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteBishops);
            else
                bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackBishops);

            var index = bishopPositions.Count - 1;

            while (index >= 0)
            {
                var pieceType = PieceType.Bishop;

                var bishopIndex = bishopPositions[index];
                var bishopPosition = LookupTables.SquareValuesFromIndex[bishopIndex];

                var allAllowedMoves = BoardChecking.CalculateAllowedBishopMoves(m_CurrentBoard, bishopIndex, m_FriendlyColour);

                var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, bishopPosition, pieceType, SpecialMoveType.Normal);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(captureMoves, bishopPosition, pieceType, SpecialMoveType.Capture);

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

        private static void CalculateRookMoves()
        {
            List<byte> rookPositions;

            if (m_FriendlyColour == PieceColour.White)
                rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteRooks);
            else
                rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackRooks);

            var index = rookPositions.Count - 1;

            while (index >= 0)
            {
                var pieceType = PieceType.Rook;

                var rookIndex = rookPositions[index];
                var rookPosition = LookupTables.SquareValuesFromIndex[rookIndex];

                var allAllowedMoves = BoardChecking.CalculateAllowedRookMoves(m_CurrentBoard, rookIndex, m_FriendlyColour);

                var normalMoves = allAllowedMoves & m_CurrentBoard.EmptySquares;
                SplitAndAddMoves(normalMoves, rookPosition, pieceType, SpecialMoveType.Normal);

                var captureMoves = allAllowedMoves & ~m_CurrentBoard.EmptySquares;
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

        private static void CalculateQueenMoves()
        {
            var queenPositions = new List<byte>();

            if (m_FriendlyColour == PieceColour.White)
                queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.WhiteQueen);
            else
                queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(m_CurrentBoard.BlackQueen);

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

        private static void CalculateKingMoves()
        {
            var whiteKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.WhiteKing);
            var blackKingPosition = BitboardOperations.GetSquareIndexFromBoardValue(m_CurrentBoard.BlackKing);

            var whiteMoves = ValidMoveArrays.KingMoves[whiteKingPosition];
            var blackMoves = ValidMoveArrays.KingMoves[blackKingPosition];
            
            if (m_FriendlyColour == PieceColour.White)
            {                
                if (m_CurrentBoard.WhiteKing > 0)
                {
                    var possibleWhiteMoves = whiteMoves & ~blackMoves;

                    var splitMoves = BitboardOperations.SplitBoardToArray(possibleWhiteMoves);

                    foreach (var moveBoard in splitMoves)
                    {
                        if ((moveBoard & m_CurrentBoard.EmptySquares) > 0)
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Normal });
                        if ((moveBoard & m_CurrentBoard.AllBlackOccupiedSquares) > 0)
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Capture });

                    }
                }
            }
            else
            {
                if (m_CurrentBoard.BlackKing > 0)
                {
                    var possibleBlackMoves = blackMoves & ~whiteMoves;

                    var splitMoves = BitboardOperations.SplitBoardToArray(possibleBlackMoves);

                    foreach (var moveBoard in splitMoves)
                    {
                        if ((moveBoard & m_CurrentBoard.EmptySquares) > 0)
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Normal });
                        if ((moveBoard & m_CurrentBoard.AllWhiteOccupiedSquares) > 0)
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = moveBoard, SpecialMove = SpecialMoveType.Capture });
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
            var enPassantCapture = new PieceMoves();
            enPassantCapture.Type = PieceType.Pawn;
            enPassantCapture.Position = attackingPiece;
            enPassantCapture.Moves = capturePosition;

            _allMovesList.Add(enPassantCapture);
        }

        private static void CheckForCastlingMoves()
        {
            if (m_FriendlyColour == PieceColour.White)
            {
                if (!BoardChecking.IsKingInCheckFast(m_CurrentBoard, PieceColour.White))
                {
                    if (m_CurrentBoard.WhiteCanCastleQueenside)
                    {
                        if (CheckIfPathIsClear(LookupTables.WhiteCastlingQueensideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingQueensideAttackPath, PieceColour.White))
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = LookupTables.C1, SpecialMove = SpecialMoveType.QueenCastle });
                        }
                    }

                    if (m_CurrentBoard.WhiteCanCastleKingside)
                    {

                        if (CheckIfPathIsClear(LookupTables.WhiteCastlingKingsideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.WhiteCastlingKingsideAttackPath, PieceColour.White))
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.WhiteKing, Moves = LookupTables.G1, SpecialMove = SpecialMoveType.KingCastle });
                        }
                    }
                }
            }
            else
            {
                if (!BoardChecking.IsKingInCheckFast(m_CurrentBoard, PieceColour.White))
                {
                    if (m_CurrentBoard.BlackCanCastleQueenside)
                    {
                        if (CheckIfPathIsClear(LookupTables.BlackCastlingQueensideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingQueensideAttackPath, PieceColour.Black))
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = LookupTables.C8, SpecialMove = SpecialMoveType.QueenCastle });
                        }
                    }
                    if (m_CurrentBoard.BlackCanCastleKingside)
                    {
                        if (CheckIfPathIsClear(LookupTables.BlackCastlingKingsideObstructionPath))
                        {
                            //if (!IsCastlingPathAttacked(LookupTables.BlackCastlingKingsideAttackPath, PieceColour.Black))
                            _allMovesList.Add(new PieceMoves { Type = PieceType.King, Position = m_CurrentBoard.BlackKing, Moves = LookupTables.G8, SpecialMove = SpecialMoveType.KingCastle });
                        }
                    }
                }
            }
        }

        private static bool CheckIfPathIsClear(ulong path)
        {
            if ((path & m_CurrentBoard.EmptySquares) == path)
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
