using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.Exceptions;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using log4net;

namespace ChessEngine.BoardRepresentation
{
    /// <summary>
    /// Represents a game board state needed for a full game
    /// including all 12 bitboards who is to move and flags for moves like en-passant and castling
    /// </summary>
    [Serializable]
    public class Board 
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection
                                                                      .MethodBase
                                                                      .GetCurrentMethod()
                                                                      .DeclaringType);

        #region piece bitboard properties

        private int m_MoveCount;
        
        private const ulong FullBoard = ulong.MaxValue;
        
        public ulong WhitePawns;
        public ulong WhiteKnights;
        public ulong WhiteBishops;
        public ulong WhiteRooks;
        public ulong WhiteQueen;
        public ulong WhiteKing;

        public ulong BlackPawns;
        public ulong BlackKnights;
        public ulong BlackBishops;
        public ulong BlackRooks;
        public ulong BlackQueen;
        public ulong BlackKing;
        
        #endregion piece bitboard properties

        #region useful bitboard properties

        public ulong AllWhiteOccupiedSquares;
        public ulong AllBlackOccupiedSquares;

        public ulong AllOccupiedSquares;

        public ulong EmptySquares;

        public ulong WhiteOrEmpty;
        public ulong BlackOrEmpty;

        #endregion useful bitboard properties

        #region Castling check properties

        public bool WhiteCanCastleQueenside = true;
        public bool WhiteCanCastleKingside = true;

        public bool BlackCanCastleQueenside = true;
        public bool BlackCanCastleKingside = true;

        #endregion Castling check properties

        public ulong EnPassantPosition;

        public bool WhiteToMove = true;

        public int HalfMoveClock;

        public int FullMoveClock = 1;

        private Stack<BoardState> m_History;

        public ulong Zobrist;
        
        #region constructor

        public Board()
        {
            LookupTables.InitialiseAllTables();            
            PieceValidMoves.GenerateMoveArrays();
            TranspositionTable.InitialiseTable();
            ZobristHash.Initialise();

            m_History = new Stack<BoardState>();
        }

        #endregion constructor

        #region place piece methods

        /// <summary>
        /// Initialises the pieces to a games starting position
        /// Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        /// therefore the far left digit is a1 and the leftmeos digit is h8
        /// </summary>
        public void InitaliseStartingPosition()
        {
            WhiteToMove = true;

            WhitePawns = LookupTables.A2 | LookupTables.B2 | LookupTables.C2 | LookupTables.D2 | LookupTables.E2 | LookupTables.F2 | LookupTables.G2 | LookupTables.H2;
            WhiteKnights = LookupTables.B1 | LookupTables.G1;
            WhiteBishops = LookupTables.C1 | LookupTables.F1;
            WhiteRooks = LookupTables.A1 | LookupTables.H1;
            WhiteQueen = LookupTables.D1;
            WhiteKing = LookupTables.E1;

            BlackPawns = LookupTables.A7 | LookupTables.B7 | LookupTables.C7 | LookupTables.D7 | LookupTables.E7 | LookupTables.F7 | LookupTables.G7 | LookupTables.H7;
            BlackKnights = LookupTables.B8 | LookupTables.G8;
            BlackBishops = LookupTables.C8 | LookupTables.F8;
            BlackRooks = LookupTables.A8 | LookupTables.H8;
            BlackQueen = LookupTables.D8;
            BlackKing = LookupTables.E8;

            m_MoveCount = 0;

            HalfMoveClock = 0;
            FullMoveClock = 1;

            m_History.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }

        /// <summary>
        /// Removes all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            WhitePawns = 0;
            WhiteKnights = 0;
            WhiteBishops = 0;
            WhiteRooks = 0;
            WhiteQueen = 0;
            WhiteKing = 0;

            BlackPawns = 0;
            BlackKnights = 0;
            BlackBishops = 0;
            BlackRooks = 0;
            BlackQueen = 0;
            BlackKing = 0;

            m_MoveCount = 0;

            HalfMoveClock = 0;
            FullMoveClock = 1;

            m_History.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }

        public void SetPosition(BoardState state)
        {
            WhiteToMove = state.WhiteToMove;

            WhitePawns = state.WhitePawns;
            WhiteKnights = state.WhiteKnights;
            WhiteBishops = state.WhiteBishops;
            WhiteRooks = state.WhiteRooks;
            WhiteQueen = state.WhiteQueen;
            WhiteKing = state.WhiteKing;

            BlackPawns = state.BlackPawns;
            BlackKnights = state.BlackKnights;
            BlackBishops = state.BlackBishops;
            BlackRooks = state.BlackRooks;
            BlackQueen = state.BlackQueen;
            BlackKing = state.BlackKing;

            EnPassantPosition = state.EnPassantPosition;

            WhiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            WhiteCanCastleKingside = state.WhiteCanCastleKingside;
            BlackCanCastleQueenside = state.BlackCanCastleQueenside;
            BlackCanCastleKingside = state.BlackCanCastleKingside;

            HalfMoveClock = state.HalfMoveClock;
            FullMoveClock = state.FullMoveClock;

            m_MoveCount = 0;

            CalculateZobristKey();

            CalculateUsefulBitboards();
        }

        internal void PlacePiece(PieceType typeToPlace, bool pieceIsWhite, int positionToPlace)
        {
            var squareToPlace = LookupTables.SquareValuesFromIndex[positionToPlace];

            PlacePiece(typeToPlace, pieceIsWhite, squareToPlace);
        }

        public void PlacePiece(PieceType typeToPlace, bool pieceIsWhite, int file, int rank)
        {
            var squareToPlace = LookupTables.SquareValuesFromPosition[file, rank];

            PlacePiece(typeToPlace, pieceIsWhite, squareToPlace);
        }

        /// <summary>
        /// Adds the given bitboard value to the given piece type and colour bitboard
        /// Note: No checks are done on squareToPlace. A correct value must be given or multiple 
        /// pieces may be placed
        /// </summary>
        /// <param name="typeToPlace"></param>
        /// <param name="colour"></param>
        /// <param name="squareToPlace"></param>
        public void PlacePiece(PieceType typeToPlace, bool pieceIsWhite, ulong squareToPlace)
        {
            switch (typeToPlace)
            {
                case (PieceType.Pawn):
                    if (pieceIsWhite)
                    {
                        WhitePawns |= squareToPlace;
                    }
                    else
                    {
                        BlackPawns |= squareToPlace;
                    }

                    break;
                case (PieceType.Knight):
                    if (pieceIsWhite)
                    {
                        WhiteKnights |= squareToPlace;
                    }
                    else
                    {
                        BlackKnights |= squareToPlace;
                    }

                    break;
                case (PieceType.Bishop):
                    if (pieceIsWhite)
                    {
                        WhiteBishops |= squareToPlace;
                    }
                    else
                    {
                        BlackBishops |= squareToPlace;
                    }

                    break;
                case (PieceType.Rook):
                    if (pieceIsWhite)
                    {
                        WhiteRooks |= squareToPlace;
                    }
                    else
                    {
                        BlackRooks |= squareToPlace;
                    }

                    break;
                case (PieceType.Queen):
                    if (pieceIsWhite)
                    {
                        WhiteQueen |= squareToPlace;
                    }
                    else
                    {
                        BlackQueen |= squareToPlace;
                    }

                    break;
                case (PieceType.King):
                    if (pieceIsWhite)
                    {
                        WhiteKing |= squareToPlace;
                    }
                    else
                    {
                        BlackKing |= squareToPlace;
                    }

                    break;
            }

            //UpdateUsefulBitboardsForAddition(squareToPlace, pieceIsWhite);
        }

        /// <summary>
        ///Removes the piece at the given index - 0-63 
        /// </summary>
        public void RemovePiece(int positionToClear)
        {
            var squareToClear = LookupTables.SquareValuesFromIndex[positionToClear];

            RemovePiece(squareToClear);

        }

        /// <summary>
        ///Removes the piece at the given file and rank
        /// </summary>
        internal void RemovePiece(int file, int rank)
        {
            var squareToClear = LookupTables.SquareValuesFromPosition[file, rank];

            RemovePiece(squareToClear);
        }

        /// <summary>
        ///Removes the piece at the given square value
        ///Note: No checks are done on squareToRemove. An actual value must be given or multiple pieces will be removed
        /// </summary>
        internal void RemovePiece(ulong squareToClear)
        {
            var notSquareToClear = ~squareToClear;

            var pieceBefore = WhitePawns;

            WhitePawns &= notSquareToClear;

            if (pieceBefore != WhitePawns)
            {
                return;
            }

            pieceBefore = BlackPawns;            
            BlackPawns &= notSquareToClear;

            if (pieceBefore != BlackPawns)
            {
                return;
            }

            pieceBefore = WhiteKnights;
            WhiteKnights &= notSquareToClear;

            if (pieceBefore != WhiteKnights)
            {
                return;
            }

            pieceBefore = BlackKnights;
            BlackKnights &= notSquareToClear;

            if (pieceBefore != BlackKnights)
            {
                return;
            }

            pieceBefore = WhiteBishops;            
            WhiteBishops &= notSquareToClear;

            if (pieceBefore != WhiteBishops)
            {
                return;
            }

            pieceBefore = BlackBishops;
            BlackBishops &= notSquareToClear;

            if (pieceBefore != BlackBishops)
            {
                return;
            }

            pieceBefore = WhiteRooks;
            WhiteRooks &= notSquareToClear;

            if (pieceBefore != WhiteRooks)
            {
                return;
            }

            pieceBefore = BlackRooks;
            BlackRooks &= notSquareToClear;

            if (pieceBefore != BlackRooks)
            {
                return;
            }

            pieceBefore = WhiteQueen;
            WhiteQueen &= notSquareToClear;

            if (pieceBefore != WhiteQueen)
            {
                return;
            }

            pieceBefore = BlackQueen;
            BlackQueen &= notSquareToClear;

            if (pieceBefore != BlackQueen)
            {
                return;
            }

            pieceBefore = WhiteKing;
            WhiteKing &= notSquareToClear;

            if (pieceBefore != WhiteKing)
            {
                return;
            }
            
            BlackKing &= notSquareToClear;
        }

        public void AllowAllCastling(bool value)
        {
            WhiteCanCastleQueenside = value;
            WhiteCanCastleKingside = value;
            BlackCanCastleQueenside = value;
            BlackCanCastleKingside = value;
        }

        #endregion place piece methods

        #region Make move methods

        /// <summary>
        /// Make a move on the board
        /// </summary>
        /// <param name="move"></param>
        /// <param name="confirmedMove">If the move is confirmed extra processes are carried out (i.e. PGN calculations. During move generation these are not necessary and slow things down</param>
        public void MakeMove(PieceMoves move, bool confirmedMove)
        {
            //MakeMove(move.Position, move.Moves, move.Type, false);
            MakeMove(move.Position, move.Moves, move.Type, move.SpecialMove, confirmedMove);
        }

        //public void MakeMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, bool confirmedMove)
        //{
        //    MakeMove(moveFromBoard, moveToBoard, pieceToMove, SpecialMoveType.Normal, confirmedMove);
        //}

        /// <summary>
        /// Make a move on the board
        /// </summary>
        /// <param name="moveFromBoard"></param>
        /// <param name="moveToBoard"></param>
        /// <param name="pieceToMove"></param>
        /// <param name="specialMove"></param>
        /// <param name="confirmedMove">If the move is confirmed extra processes are carried out (i.e. PGN calculations. During move generation these are not necessary and slow things down</param>
        public void MakeMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove, bool confirmedMove)
        {
            SaveBoardState();            

            UpdateZobrist(moveFromBoard, moveToBoard, pieceToMove, specialMove);    //Do before everything else so we can check for capture etc 

            RemovePiece(moveFromBoard);
            
            var whiteToMove = WhiteToMove; //This is mostly because I worry that constantly hitting a property will be slow
            
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
                        
            m_MoveCount++;

            HalfMoveClock ++;

            if (!WhiteToMove)
            {
                FullMoveClock++;
            }
            
            SwitchSides();

            CalculateUsefulBitboards();
        }

        #region special move type methods

        private bool IsMovePromotion(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotion || specialMove == SpecialMoveType.KnightPromotionCapture ||
                specialMove == SpecialMoveType.BishopPromotion || specialMove == SpecialMoveType.BishopPromotionCapture ||
                specialMove == SpecialMoveType.RookPromotion || specialMove == SpecialMoveType.RookPromotionCapture ||
                specialMove == SpecialMoveType.QueenPromotion || specialMove == SpecialMoveType.QueenPromotionCapture);
        }

        private bool IsMovePromotionNonCapture(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotion ||
                specialMove == SpecialMoveType.BishopPromotion ||
                specialMove == SpecialMoveType.RookPromotion ||
                specialMove == SpecialMoveType.QueenPromotion);
        }

        private bool IsMovePromotionCapture(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KnightPromotionCapture ||
                specialMove == SpecialMoveType.BishopPromotionCapture ||
                specialMove == SpecialMoveType.RookPromotionCapture ||
                specialMove == SpecialMoveType.QueenPromotionCapture);
        }

        private PieceType PromotedPiece(SpecialMoveType specialMove)
        {
            if (specialMove == SpecialMoveType.KnightPromotion || specialMove == SpecialMoveType.KnightPromotionCapture)
                return PieceType.Knight;
            if (specialMove == SpecialMoveType.BishopPromotion || specialMove == SpecialMoveType.BishopPromotionCapture)
                return PieceType.Bishop;
            if (specialMove == SpecialMoveType.RookPromotion || specialMove == SpecialMoveType.RookPromotionCapture)
                return PieceType.Rook;
            if (specialMove == SpecialMoveType.QueenPromotion || specialMove == SpecialMoveType.QueenPromotionCapture)
                return PieceType.Queen;

            return PieceType.None;
        }

        private static bool IsMoveCastling(SpecialMoveType specialMove)
        {
            return (specialMove == SpecialMoveType.KingCastle ||
                specialMove == SpecialMoveType.QueenCastle);
        }
        

        #endregion special move type methods


        /// <summary>
        /// Check if move has castled or altered the state of the castling flags
        /// </summary>
        /// <param name="moveFromBoard"></param>
        /// <param name="moveToBoard"></param>
        /// <param name="pieceToMove"></param>
        private void CheckCastlingStatus(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            //Castling flag checks
            if (WhiteToMove)
            {
                if (WhiteCanCastleKingside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.G1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.H1);
                            PlacePiece(PieceType.Rook, pieceIsWhite: true, LookupTables.F1);
                        }

                        Zobrist ^= ZobristKey.WhiteCastleKingside;
                        WhiteCanCastleKingside = false;     //Any king move means we can no longer castle
                    }

                    if (moveFromBoard == LookupTables.H1)     //Moved Rook
                    {
                        Zobrist ^= ZobristKey.WhiteCastleKingside;
                        WhiteCanCastleKingside = false;
                    }
                }

                if (WhiteCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.A1);
                            PlacePiece(PieceType.Rook, pieceIsWhite: true, LookupTables.D1);

                            WhiteCanCastleQueenside = false;
                            //whiteCanCastleKingside = false;  
                        }

                        Zobrist ^= ZobristKey.WhiteCastleQueenside;
                        WhiteCanCastleQueenside = false;

                        //else if (moveFromBoard == BitboardSquare.A1)     //Moved Rook
                        //    whiteCanCastleQueenside = false;
                    }

                    if (moveFromBoard == LookupTables.A1)     //Moved Rook
                    {
                        Zobrist ^= ZobristKey.WhiteCastleQueenside;                        
                        WhiteCanCastleQueenside = false;
                    }
                }
                
                if (BlackCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H8)
                    {
                        Zobrist ^= ZobristKey.BlackCastleKingside;                        
                        BlackCanCastleKingside = false;
                    }
                }

                if (BlackCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A8)
                    {
                        Zobrist ^= ZobristKey.BlackCastleQueenside;
                        BlackCanCastleQueenside = false;
                    }
                }
            }
            else
            {
                if (BlackCanCastleKingside)
                {
                   if (pieceToMove == PieceType.King)
                   {
                        if (moveToBoard == LookupTables.G8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.H8);
                            PlacePiece(PieceType.Rook, pieceIsWhite: false, LookupTables.F8);
                        }

                        BlackCanCastleKingside = false; //Any king move means we can no longer castle

                        Zobrist ^= ZobristKey.BlackCastleKingside;
                   }

                   if (moveFromBoard == LookupTables.H8)     //Moved Rook
                   {
                       BlackCanCastleKingside = false;
                       Zobrist ^= ZobristKey.BlackCastleKingside;
                   }
                }

                if (BlackCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.A8);
                            PlacePiece(PieceType.Rook, pieceIsWhite: false, LookupTables.D8);
                        }

                        BlackCanCastleQueenside = false;
                        Zobrist ^= ZobristKey.BlackCastleQueenside;
                    }

                    if (moveFromBoard == LookupTables.A8)     //Moved Rook
                    {
                        BlackCanCastleQueenside = false;
                        Zobrist ^= ZobristKey.BlackCastleQueenside;
                    }
                }

                //Check if black has captured a white rook
                if (WhiteCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H1)
                    {
                        WhiteCanCastleKingside = false;
                        Zobrist ^= ZobristKey.WhiteCastleKingside;
                    }
                }

                if (WhiteCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A1)
                    {
                        WhiteCanCastleQueenside = false;
                        Zobrist ^= ZobristKey.WhiteCastleQueenside;
                    }
                }
            }  
        }

        /// <summary>
        /// Checks if an en passant capture was made or if the flags have to be updated
        /// </summary>
        /// <param name="moveFromBoard"></param>
        /// <param name="moveToBoard"></param>
        /// <param name="pieceToMove"></param>
        private void CheckEnPassantStatus(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        {
            if (pieceToMove == PieceType.Pawn)
            {
                if (WhiteToMove)
                {
                    var differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard) 
                                              - BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard);
                    
                    if ((moveToBoard & EnPassantPosition) != 0) //Move is an en-passant capture  
                    {
                        //Remove captured piece
                        RemovePiece(EnPassantPosition >> 8);
                        //UpdateUsefulBitboardsForRemoval(EnPassantPosition >> 8);
                        EnPassantPosition = 0;
                    }
                    else if(differenceInMoveIndex == 16)
                    {
                        EnPassantPosition = moveToBoard >> 8;
                    }
                    else
                        EnPassantPosition = 0;
                }
                else
                {
                    var differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard) - BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard);
                    if ((moveToBoard & EnPassantPosition) != 0)      //Move is an en-passant capture
                    {
                        //Remove captured piece
                        RemovePiece(EnPassantPosition << 8);
                        //UpdateUsefulBitboardsForRemoval(EnPassantPosition << 8);
                        EnPassantPosition = 0;
                    }
                    else if (differenceInMoveIndex == 16)
                    {
                        EnPassantPosition = moveToBoard << 8;
                    }
                    else
                        EnPassantPosition = 0;
                }
            }
            else
                EnPassantPosition = 0;
        }

        public void SwitchSides()
        {
            Zobrist ^= ZobristKey.BlackToMove;

            if (WhiteToMove)
            {
                WhiteToMove = false;
            }
            else
            {
                WhiteToMove = true;
            }
        }

        private void SaveBoardState()
        {
            var state = new BoardState
            {
                WhiteToMove = WhiteToMove,
                WhitePawns = WhitePawns,
                WhiteKnights = WhiteKnights,
                WhiteBishops = WhiteBishops,
                WhiteRooks = WhiteRooks,
                WhiteQueen = WhiteQueen,
                WhiteKing = WhiteKing,
                BlackPawns = BlackPawns,
                BlackKnights = BlackKnights,
                BlackBishops = BlackBishops,
                BlackRooks = BlackRooks,
                BlackQueen = BlackQueen,
                BlackKing = BlackKing,
                EnPassantPosition = EnPassantPosition,
                WhiteCanCastleQueenside = WhiteCanCastleQueenside,
                WhiteCanCastleKingside = WhiteCanCastleKingside,
                BlackCanCastleQueenside = BlackCanCastleQueenside,
                BlackCanCastleKingside = BlackCanCastleKingside,
                //WhiteInCheck = whiteInCheck,
                //BlackInCheck = blackInCheck,
                HalfMoveClock = HalfMoveClock,
                FullMoveClock = FullMoveClock,
                //PgnMove = m_PgnMove,
                ZobristKey = Zobrist
            };

            m_History.Push(state);
        }

        public void UnMakeLastMove()
        {
            UnMakeLastMove(true);
        }

        public void UnMakeLastMove(bool verbose)
        {
            var state = m_History.Pop();
            
            WhiteToMove = state.WhiteToMove;

            WhitePawns = state.WhitePawns;
            WhiteKnights = state.WhiteKnights;
            WhiteBishops = state.WhiteBishops;
            WhiteRooks = state.WhiteRooks;
            WhiteQueen = state.WhiteQueen;
            WhiteKing = state.WhiteKing;

            BlackPawns = state.BlackPawns;
            BlackKnights = state.BlackKnights;
            BlackBishops = state.BlackBishops;
            BlackRooks = state.BlackRooks;
            BlackQueen = state.BlackQueen;
            BlackKing = state.BlackKing;

            EnPassantPosition = state.EnPassantPosition;

            WhiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            WhiteCanCastleKingside = state.WhiteCanCastleKingside;
            BlackCanCastleQueenside = state.BlackCanCastleQueenside;
            BlackCanCastleKingside = state.BlackCanCastleKingside;

            HalfMoveClock = state.HalfMoveClock;
            FullMoveClock = state.FullMoveClock;

            //whiteInCheck = state.WhiteInCheck;
            //blackInCheck = state.BlackInCheck;

            //m_PgnMove = state.PgnMove;

            Zobrist = state.ZobristKey;
            
            CalculateUsefulBitboards();
        } 
        
        #endregion Make move methods

        //Gets called every time a move is made to update all useful boards
        public void CalculateUsefulBitboards()
        {
            AllWhiteOccupiedSquares = WhitePawns | WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueen | WhiteKing;
            AllBlackOccupiedSquares = BlackPawns | BlackKnights | BlackBishops | BlackRooks | BlackQueen | BlackKing;
            AllOccupiedSquares      = AllWhiteOccupiedSquares | AllBlackOccupiedSquares;
            EmptySquares            = AllOccupiedSquares ^ FullBoard;
            WhiteOrEmpty            = AllWhiteOccupiedSquares | EmptySquares;
            BlackOrEmpty            = AllBlackOccupiedSquares | EmptySquares;
        }

        #region slower debug get methods

        #region WriteWriteBoardToConsole methods

        public void WriteBoardToConsole()
        {
            const string piece = " ";

            var squares = new char[64];

            AddPieceLetterToSquares(squares, WhitePawns, 'p');
            AddPieceLetterToSquares(squares, BlackPawns, 'P');

            AddPieceLetterToSquares(squares, WhiteKnights, 'n');
            AddPieceLetterToSquares(squares, BlackKnights, 'N');

            AddPieceLetterToSquares(squares, WhiteBishops, 'b');
            AddPieceLetterToSquares(squares, BlackBishops, 'B');

            AddPieceLetterToSquares(squares, WhiteRooks, 'r');
            AddPieceLetterToSquares(squares, BlackRooks, 'R');

            AddPieceLetterToSquares(squares, WhiteQueen, 'q');
            AddPieceLetterToSquares(squares, BlackQueen, 'Q');

            AddPieceLetterToSquares(squares, WhiteKing, 'k');
            AddPieceLetterToSquares(squares, BlackKing, 'K');

            for (var rank = 7; rank >= 0; rank--)
            {
                Console.WriteLine("");
                Console.WriteLine(@"-------------------------");
                Console.Write(@"|");

                for (var file = 0; file < 8; file++)
                {
                    var index = rank * 8 + file;

                    if (char.IsLetter(squares[index]))
                    {
                        Console.Write(squares[index]);
                    }
                    else
                    {
                        Console.Write(@" ");
                    }

                    Console.Write(piece + @"|");
                }
            }

            Console.WriteLine("");
            Console.WriteLine(@"-------------------------");
        }

        public string BoardToString()
        {
            var boardPos = string.Empty;
            const string piece = " ";

            var squares = new char[64];

            AddPieceLetterToSquares(squares, WhitePawns, 'p');
            AddPieceLetterToSquares(squares, BlackPawns, 'P');

            AddPieceLetterToSquares(squares, WhiteKnights, 'n');
            AddPieceLetterToSquares(squares, BlackKnights, 'N');

            AddPieceLetterToSquares(squares, WhiteBishops, 'b');
            AddPieceLetterToSquares(squares, BlackBishops, 'B');

            AddPieceLetterToSquares(squares, WhiteRooks, 'r');
            AddPieceLetterToSquares(squares, BlackRooks, 'R');

            AddPieceLetterToSquares(squares, WhiteQueen, 'q');
            AddPieceLetterToSquares(squares, BlackQueen, 'Q');

            AddPieceLetterToSquares(squares, WhiteKing, 'k');
            AddPieceLetterToSquares(squares, BlackKing, 'K');

            for (var rank = 7; rank >= 0; rank--)
            {
                boardPos += Environment.NewLine;
                boardPos += "-------------------------" + Environment.NewLine;
                boardPos += "|";

                for (var file = 0; file < 8; file++)
                {
                    var index = rank * 8 + file;

                    if (char.IsLetter(squares[index]))
                        boardPos += squares[index];
                    else
                        boardPos += " ";
                    boardPos += piece + "|";
                }
            }

            boardPos += Environment.NewLine;
            boardPos += "-------------------------" + Environment.NewLine;

            return boardPos;
        }

        private static void AddPieceLetterToSquares(IList<char> squares, ulong piecePosition, char letterToAdd)
        {
            var pieceSquares = BitboardOperations.GetSquareIndexesFromBoardValue(piecePosition);

            foreach (var pieceSquare in pieceSquares)
            {
                squares[pieceSquare] = letterToAdd;
            }
        }

        #endregion WriteWriteBoardToConsole methods

        #endregion slower debug methods
        
        public BoardState GetCurrentBoardState()
        {
            var state = new BoardState
            {
                WhiteToMove = WhiteToMove,
                WhitePawns = WhitePawns,
                WhiteKnights = WhiteKnights,
                WhiteBishops = WhiteBishops,
                WhiteRooks = WhiteRooks,
                WhiteQueen = WhiteQueen,
                WhiteKing = WhiteKing,

                BlackPawns = BlackPawns,
                BlackKnights = BlackKnights,
                BlackBishops = BlackBishops,
                BlackRooks = BlackRooks,
                BlackQueen = BlackQueen,
                BlackKing = BlackKing,
                EnPassantPosition = EnPassantPosition,
                WhiteCanCastleQueenside = WhiteCanCastleQueenside,
                WhiteCanCastleKingside = WhiteCanCastleKingside,
                BlackCanCastleQueenside = BlackCanCastleQueenside,
                BlackCanCastleKingside = BlackCanCastleKingside,
                HalfMoveClock = HalfMoveClock,
                FullMoveClock = FullMoveClock,
                ZobristKey = Zobrist
            };

            return state;
        }

        #region Zobrist functions

        private void UpdateZobrist(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            UpdateEnPassantZobrist(specialMove, moveToBoard);
            
            UpdateZobristForMove(moveFromBoard, moveToBoard, pieceToMove, specialMove);

            //Update for side
            //Zobrist ^= ZobristKey.BlackToMove;
        }

        private void UpdateZobristForMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            //Remove old piece
            Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard)];
            
            if (IsMovePromotionCapture(specialMove))
            {
                //remove captured piece
                var capturedPiece = BoardChecking.GetPieceTypeOnSquare(this, moveToBoard);
                Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, !WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add promoted piece
                var promotedPiece = PromotedPiece(specialMove);
                Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if(IsMovePromotionNonCapture(specialMove))
            {
                //Add promoted piece
                var promotedPiece = PromotedPiece(specialMove);
                Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if (specialMove == SpecialMoveType.Capture)
            {
                //remove captured piece
                var capturedPiece = BoardChecking.GetPieceTypeOnSquare(this, moveToBoard);
                Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, !WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add moved piece
                Zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, WhiteToMove), BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                       
            }
            else if (IsMoveCastling(specialMove))
            {
                int movedPieceNumber;

                if(WhiteToMove)
                    movedPieceNumber = ZobristHash.WHITE_KING;
                else
                    movedPieceNumber = ZobristHash.BLACK_KING;

                //Add moved piece
                Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            
                //Add rook
                if ((moveToBoard & LookupTables.C1) > 0)
                {
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 0];
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 3];
                }
                else if ((moveToBoard & LookupTables.G1) > 0)
                {
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 7];
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, 5];
                }
                else if ((moveToBoard & LookupTables.C8) > 0)
                {
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 56];
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 59];
                }
                else if ((moveToBoard & LookupTables.G8) > 0)
                {
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 63];
                    Zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, 61];
                }
            }
            else if(specialMove == SpecialMoveType.ENPassantCapture)
            {
                int capturedPieceNumber;
                int movedPieceNumber;

                ulong capturedPawnBitboard;

                if (WhiteToMove)
                {
                    movedPieceNumber = ZobristHash.WHITE_PAWN;
                    capturedPieceNumber = ZobristHash.BLACK_PAWN;
                    capturedPawnBitboard = EnPassantPosition << 8;
                }
                else
                {
                    movedPieceNumber = ZobristHash.BLACK_PAWN;                    
                    capturedPieceNumber = ZobristHash.WHITE_PAWN;
                    capturedPawnBitboard = EnPassantPosition >> 8;
                }

                //Remove captured pawn
                Zobrist ^= ZobristKey.PiecePositions[capturedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(capturedPawnBitboard)];
                
                //Add moved pawn
                Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                
            }
            else
            {
                var movedPieceNumber = ZobristHash.GetPieceValue(pieceToMove, WhiteToMove);

                Zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];              
            }
        }

        private void UpdateEnPassantZobrist(SpecialMoveType specialMove, ulong moveToBoard)
        {
            var enPassantPosition = m_History.Peek().EnPassantPosition;

            // Previous position
            if (enPassantPosition > 0)
            {
                Zobrist ^= ZobristHash.HashEnPassantColumn(enPassantPosition);
            }

            //Current Position
            if (specialMove == SpecialMoveType.DoublePawnPush)
            {
                Zobrist ^= ZobristHash.HashEnPassantColumn(moveToBoard);
#warning May have to change this to only add enpassant hash if there is a pawn which can capture
            }            
        }

        /// <summary>
        /// Calculates the Zobrist key from the board
        /// </summary>
        private void CalculateZobristKey()
        {
            Zobrist = ZobristHash.HashBoard(GetCurrentBoardState());
        }

        #endregion Zobrist functions

        public void ResetFlags()
        {
            m_History = new Stack<BoardState>();

            AllowAllCastling(true);
            //m_PgnMove = string.Empty;
            
            m_MoveCount = 0;

            WhiteToMove = true;
        
            HalfMoveClock = 0;      //To track captures or pawn advance - for 50 move rule
            FullMoveClock = 1;
            EnPassantPosition = 0;
        }
    }
}
