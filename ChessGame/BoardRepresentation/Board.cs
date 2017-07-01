using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Enums;
using ChessGame.Exceptions;
using ChessGame.MoveSearching;
using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.BoardSearching;
using log4net;

namespace ChessGame.BoardRepresentation
{
    /// <summary>
    /// Represents a game board state needed for a full game
    /// including all 12 bitboards who is to move and flags for moves like en-passant and castling
    /// </summary>
    [Serializable]
    public class Board
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region private properties

        private List<BoardState> boardStates;        //Keeps track of the move history
        private int moveCount = 0;

        private bool whiteToMove = true;
        
        private int halfMoveClock = 0;      //To track captures or pawn advance - for 50 move rule
        private int fullMoveClock = 1;

        private ulong zobrist;

        private bool whiteInCheck, blackInCheck;    //To keep track of whether players are in check - easier than checking lots in the evaluation
        
        #region bitboard piece positions

        #region Piece bitboard fields

        private ulong whitePawns;
        private ulong whiteKnights;
        private ulong whiteBishops;
        private ulong whiteRooks;
        private ulong whiteQueen;
        private ulong whiteKing;

        private ulong blackPawns;
        private ulong blackKnights;
        private ulong blackBishops;
        private ulong blackRooks;
        private ulong blackQueen;
        private ulong blackKing;

        private string pgnMove = String.Empty;

        #endregion Piece bitboard fields

        #region useful bitboards
        
        private ulong allWhiteOccupiedSquares;
        private ulong allBlackOccupiedSquares;
        private ulong allOccupiedSquares;
        private ulong emptySquares;
        private ulong whiteOrEmpty;
        private ulong blackOrEmpty;

        private ulong fullBoard = ulong.MaxValue;
        //private ulong emptyBoard = ulong.MinValue;
        
        #endregion useful bitboards

        #endregion bitboard piece positions

        ulong enPassantPosition = 0;       //The attack target behind any move that an enemy pawn makes on its first position (this is reset to 0 after every move if a en passant move isn't possible)
        //ulong possiblePawnCaptures;

        #region castling check fields

        private bool whiteCanCastleQueenside = true;
        private bool whiteCanCastleKingside = true;
        private bool blackCanCastleQueenside = true;
        private bool blackCanCastleKingside = true;

        #endregion castling check fields

        #endregion private properties

        #region public properties

        #region piece bitboard properties

        #region White piece properties

        public ulong WhitePawns
        {
            get { return whitePawns; }
            set { whitePawns = value; }
        }

        public ulong WhiteKnights
        {
            get { return whiteKnights; }
            set { whiteKnights = value; }
        }

        public ulong WhiteBishops
        {
            get { return whiteBishops; }
            set { whiteBishops = value; }
        }

        public ulong WhiteRooks
        {
            get { return whiteRooks; }
            set { whiteRooks = value; }
        }
        public ulong WhiteQueen
        {
            get { return whiteQueen; }
            set { whiteQueen = value; }
        }

        public ulong WhiteKing
        {
            get { return whiteKing; }
            set { whiteKing = value; }
        }

        #endregion White piece properties

        #region Black piece properties

        public ulong BlackPawns
        {
            get { return blackPawns; }
            set { blackPawns = value; }
        }

        public ulong BlackKnights
        {
            get { return blackKnights; }
            set { blackKnights = value; }
        }

        public ulong BlackBishops
        {
            get { return blackBishops; }
            set { blackBishops = value; }
        }

        public ulong BlackRooks
        {
            get { return blackRooks; }
            set { blackRooks = value; }
        }

        public ulong BlackQueen
        {
            get { return blackQueen; }
            set { blackQueen = value; }
        }

        public ulong BlackKing
        {
            get { return blackKing; }
            set { blackKing = value; }
        }

        #endregion Black piece properties
        
        #endregion piece bitboard properties

        #region useful bitboard properties

        public ulong AllWhiteOccupiedSquares
        {
            get { return allWhiteOccupiedSquares; }
            set { allWhiteOccupiedSquares = value; }
        }

        public ulong AllBlackOccupiedSquares
        {
            get { return allBlackOccupiedSquares; }
            set { allBlackOccupiedSquares = value; }
        }

        public ulong AllOccupiedSquares
        {
            get { return allOccupiedSquares; }
            set { allOccupiedSquares = value; }
        }

        public ulong EmptySquares
        {
            get { return emptySquares; }
            set { emptySquares = value; }
        }

        public ulong WhiteOrEmpty
        {
            get { return whiteOrEmpty; }
            set { whiteOrEmpty = value; }
        }

        public ulong BlackOrEmpty
        {
            get { return blackOrEmpty; }
            set { blackOrEmpty = value; }
        }

        #endregion useful bitboard properties

        #region Castling check properties

        public bool WhiteCanCastleQueenside
        {
            get { return whiteCanCastleQueenside; }
            set { whiteCanCastleQueenside = value; }
        }

        public bool WhiteCanCastleKingside
        {
            get { return whiteCanCastleKingside; }
            set { whiteCanCastleKingside = value; }
        }

        public bool BlackCanCastleQueenside
        {
            get { return blackCanCastleQueenside; }
            set { blackCanCastleQueenside = value; }
        }

        public bool BlackCanCastleKingside
        {
            get { return blackCanCastleKingside; }
            set { blackCanCastleKingside = value; }
        } 

        #endregion Castling check properties

        //public ulong PossiblePawnCaptures
        //{
        //    get
        //    {
        //        return possiblePawnCaptures;
        //    }
        //    set
        //    {
        //        possiblePawnCaptures = value;
        //    }
        //}

        public ulong EnPassantPosition
        {
            get
            {
                return enPassantPosition;
            }
        }

        public bool WhiteToMove
        {
            get
            {
                return whiteToMove;
            }
        }

        //public bool WhiteIsInCheck
        //{
        //    get { return whiteInCheck; }
        //}

        //public bool BlackIsInCheck
        //{
        //    get { return blackInCheck; }
        //}

        public PieceColour MoveColour
        {
            get
            {
                if (whiteToMove)
                    return PieceColour.White;
                else
                    return PieceColour.Black;
            }
        }

        public PieceColour NotMoveColour
        {
            get
            {
                if (whiteToMove)
                    return PieceColour.Black;
                else
                    return PieceColour.White;
            }
        }

        public int HalfMoveClock
        {
            get { return halfMoveClock; }
            set { halfMoveClock = value; }
        }

        public int FullMoveClock
        {
            get { return fullMoveClock; }
            set { fullMoveClock = value; }
        }

        public List<BoardState> History
        {
            get
            {
                return boardStates;
            }
        }

        public ulong Zobrist
        {
            get { return zobrist; }
            set { zobrist = value; }
        }

        #endregion public properties

        #region constructor

        public Board()
        {
            LookupTables.InitialiseAllTables();            
            PieceValidMoves.GenerateMoveArrays();
            TranspositionTable.InitialiseTable();
            ZobristHash.Initialise();

            boardStates = new List<BoardState>();
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
            whiteToMove = true;

            whitePawns = LookupTables.A2 | LookupTables.B2 | LookupTables.C2 | LookupTables.D2 | LookupTables.E2 | LookupTables.F2 | LookupTables.G2 | LookupTables.H2;
            whiteKnights = LookupTables.B1 | LookupTables.G1;
            whiteBishops = LookupTables.C1 | LookupTables.F1;
            whiteRooks = LookupTables.A1 | LookupTables.H1;
            whiteQueen = LookupTables.D1;
            whiteKing = LookupTables.E1;

            blackPawns = LookupTables.A7 | LookupTables.B7 | LookupTables.C7 | LookupTables.D7 | LookupTables.E7 | LookupTables.F7 | LookupTables.G7 | LookupTables.H7;
            blackKnights = LookupTables.B8 | LookupTables.G8;
            blackBishops = LookupTables.C8 | LookupTables.F8;
            blackRooks = LookupTables.A8 | LookupTables.H8;
            blackQueen = LookupTables.D8;
            blackKing = LookupTables.E8;

            moveCount = 0;

            halfMoveClock = 0;
            fullMoveClock = 1;

            boardStates.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }

        /// <summary>
        /// Removes all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            whitePawns = 0;
            whiteKnights = 0;
            whiteBishops = 0;
            whiteRooks = 0;
            whiteQueen = 0;
            whiteKing = 0;

            blackPawns = 0;
            blackKnights = 0;
            blackBishops = 0;
            blackRooks = 0;
            blackQueen = 0;
            blackKing = 0;

            moveCount = 0;

            halfMoveClock = 0;
            fullMoveClock = 1;

            boardStates.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }

        private void SetPosition(BoardState state)
        {
            whiteToMove = state.WhiteToMove;

            whitePawns = state.WhitePawns;
            whiteKnights = state.WhiteKnights;
            whiteBishops = state.WhiteBishops;
            whiteRooks = state.WhiteRooks;
            whiteQueen = state.WhiteQueen;
            whiteKing = state.WhiteKing;

            blackPawns = state.BlackPawns;
            blackKnights = state.BlackKnights;
            blackBishops = state.BlackBishops;
            blackRooks = state.BlackRooks;
            blackQueen = state.BlackQueen;
            blackKing = state.BlackKing;

            enPassantPosition = state.EnPassantPosition;

            whiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            whiteCanCastleKingside = state.WhiteCanCastleKingside;
            blackCanCastleQueenside = state.BlackCanCastleQueenside;
            blackCanCastleKingside = state.BlackCanCastleKingside;

            halfMoveClock = state.HalfMoveClock;
            fullMoveClock = state.FullMoveClock;

            moveCount = 0;

            CalculateZobristKey();
        }

        internal void PlacePiece(PieceType typeToPlace, PieceColour colour, int positionToPlace)
        {
            ulong squareToPlace = LookupTables.SquareValuesFromIndex[positionToPlace];

            PlacePiece(typeToPlace, colour, squareToPlace);
        }

        internal void PlacePiece(PieceType typeToPlace, PieceColour colour, int file, int rank)
        {
            ulong squareToPlace = LookupTables.SquareValuesFromPosition[file, rank];

            PlacePiece(typeToPlace, colour, squareToPlace);
        }

        /// <summary>
        /// Adds the given bitboard value to the given piece type and colour bitboard
        /// Note: No checks are done on squareToPlace. An actual value must be given or multiple pieces may be placed
        /// </summary>
        /// <param name="typeToPlace"></param>
        /// <param name="squareToPlace"></param>
        internal void PlacePiece(PieceType typeToPlace, PieceColour colour, ulong squareToPlace)
        {
            switch (typeToPlace)
            {
                case (PieceType.Pawn):
                    if (colour == PieceColour.White)
                        whitePawns = whitePawns | squareToPlace;
                    else
                        blackPawns = blackPawns | squareToPlace;

                    break;
                case (PieceType.Knight):
                    if (colour == PieceColour.White)
                        whiteKnights = whiteKnights | squareToPlace;
                    else
                        blackKnights = blackKnights | squareToPlace;

                    break;
                case (PieceType.Bishop):
                    if (colour == PieceColour.White)
                        whiteBishops = whiteBishops | squareToPlace;
                    else
                        blackBishops = blackBishops | squareToPlace;

                    break;
                case (PieceType.Rook):
                    if (colour == PieceColour.White)
                        whiteRooks = whiteRooks | squareToPlace;
                    else
                        blackRooks = blackRooks | squareToPlace;

                    break;
                case (PieceType.Queen):
                    if (colour == PieceColour.White)
                        whiteQueen = whiteQueen | squareToPlace;
                    else
                        blackQueen = blackQueen | squareToPlace;

                    break;
                case (PieceType.King):
                    if (colour == PieceColour.White)
                        whiteKing = whiteKing | squareToPlace;
                    else
                        blackKing = blackKing | squareToPlace;
                    break;
                case (PieceType.None):                   
                        RemovePiece(squareToPlace); 
                    break;
            }

            CalculateUsefulBitboards();
        }
        
        /// <summary>
        ///Removes the piece at the given index - 0-63 
        /// </summary>
        internal void RemovePiece(int positionToClear)
        {
            ulong squareToClear = LookupTables.SquareValuesFromIndex[positionToClear];

            RemovePiece(squareToClear);

        }

        /// <summary>
        ///Removes the piece at the given file and rank
        /// </summary>
        internal void RemovePiece(int file, int rank)
        {
            ulong squareToClear = LookupTables.SquareValuesFromPosition[file, rank];

            RemovePiece(squareToClear);
        }

        /// <summary>
        ///Removes the piece at the given square value
        ///Note: No checks are done on squareToRemove. An actual value must be given or multiple pieces will be removed
        /// </summary>
        internal void RemovePiece(ulong squareToClear)
        {
            ulong notSquareToClear = ~squareToClear;

            ulong pieceBefore = 0;

            pieceBefore = whitePawns;
            whitePawns = whitePawns & notSquareToClear;

            if (pieceBefore != whitePawns)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = blackPawns;            
            blackPawns = blackPawns & notSquareToClear;

            if (pieceBefore != blackPawns)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = whiteKnights;
            whiteKnights = whiteKnights & notSquareToClear;

            if (pieceBefore != whiteKnights)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = blackKnights;
            blackKnights = blackKnights & notSquareToClear;

            if (pieceBefore != blackKnights)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = whiteBishops;            
            whiteBishops = whiteBishops & notSquareToClear;

            if (pieceBefore != whiteBishops)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = blackBishops;
            blackBishops = blackBishops & notSquareToClear;

            if (pieceBefore != blackBishops)
            {
                CalculateUsefulBitboards();
                return;
            }


            pieceBefore = whiteRooks;
            whiteRooks = whiteRooks & notSquareToClear;

            if (pieceBefore != whiteRooks)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = blackRooks;
            blackRooks = blackRooks & notSquareToClear;

            if (pieceBefore != blackRooks)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = whiteQueen;
            whiteQueen = whiteQueen & notSquareToClear;

            if (pieceBefore != whiteQueen)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = blackQueen;
            blackQueen = blackQueen & notSquareToClear;

            if (pieceBefore != blackQueen)
            {
                CalculateUsefulBitboards();
                return;
            }

            pieceBefore = whiteKing;
            whiteKing = whiteKing & notSquareToClear;

            if (pieceBefore != whiteKing)
            {
                CalculateUsefulBitboards();
                return;
            }
            
            blackKing = blackKing & notSquareToClear;

            //ClearFromPieceBitboard(notSquareToClear, ref whitePawns);
            //ClearFromPieceBitboard(notSquareToClear, ref whiteKnights);
            //ClearFromPieceBitboard(notSquareToClear, ref whiteBishops);
            //ClearFromPieceBitboard(notSquareToClear, ref whiteRooks);
            //ClearFromPieceBitboard(notSquareToClear, ref whiteQueen);
            //ClearFromPieceBitboard(notSquareToClear, ref whiteKing);

            //ClearFromPieceBitboard(notSquareToClear, ref blackPawns);
            //ClearFromPieceBitboard(notSquareToClear, ref blackKnights);
            //ClearFromPieceBitboard(notSquareToClear, ref blackBishops);
            //ClearFromPieceBitboard(notSquareToClear, ref blackRooks);
            //ClearFromPieceBitboard(notSquareToClear, ref blackQueen);
            //ClearFromPieceBitboard(notSquareToClear, ref blackKing);

            CalculateUsefulBitboards();
        }

        /// <summary>
        /// Removes the given bitboard square for the given bitboard if it is set
        /// </summary>
        /// <param name="whitePawns"></param>
        private void ClearFromPieceBitboard(ulong squareToClear, ref ulong piecesToClearFrom)
        {
            piecesToClearFrom = piecesToClearFrom & squareToClear; 

            //ulong piecesBefore = piecesToClearFrom;
            //ulong notPiecesToClearFrom = ~piecesToClearFrom;
            //notPiecesToClearFrom ^= squareToClear;
            //piecesToClearFrom = ~notPiecesToClearFrom & piecesBefore;
        }

        public void AllowAllCastling(bool value)
        {
            whiteCanCastleQueenside = value;
            whiteCanCastleKingside = value;
            blackCanCastleQueenside = value;
            blackCanCastleKingside = value;
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
        /// <param name="confirmedMove">If the move is confirmed extra processes are carried out (i.e. PGN calculations. During move generation these are not necessary and slow things down</param>
        public void MakeMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove, bool confirmedMove)
        {
            SaveBoardState();            

            //if(confirmedMove)
            //    AddPgnValue(moveFromBoard, moveToBoard, pieceToMove);   //Do before everything else so we can check for capture etc //We can't trust pieceToMove since promotions ar already dealt with

            UpdateZobrist(moveFromBoard, moveToBoard, pieceToMove, specialMove);    //Do before everything else so we can check for capture etc 

            RemovePiece(moveFromBoard);

            PieceColour colourOfPiece;

            if (whiteToMove)
                colourOfPiece = PieceColour.White;
            else
                colourOfPiece = PieceColour.Black;

            RemovePiece(moveToBoard);       //Clear the square we're moving the piece to first

            if (IsMovePromotion(specialMove))
            {
                switch (specialMove)
                {
                    case SpecialMoveType.KnightPromotion:
                        PlacePiece(PieceType.Knight, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.KnightPromotionCapture:
                        PlacePiece(PieceType.Knight, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.BishopPromotion:
                        PlacePiece(PieceType.Bishop, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.BishopPromotionCapture:
                        PlacePiece(PieceType.Bishop, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.RookPromotion:
                        PlacePiece(PieceType.Rook, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.RookPromotionCapture:
                        PlacePiece(PieceType.Rook, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.QueenPromotion:
                        PlacePiece(PieceType.Queen, colourOfPiece, moveToBoard);
                        break;
                    case SpecialMoveType.QueenPromotionCapture:
                        PlacePiece(PieceType.Queen, colourOfPiece, moveToBoard);
                        break;
                }                
            }
            else
            {
                PlacePiece(pieceToMove, colourOfPiece, moveToBoard);
            }
            
            CheckCastlingStatus(moveFromBoard, moveToBoard, pieceToMove);
            CheckEnPassantStatus(moveFromBoard, moveToBoard, pieceToMove);
            //CheckForPawnPromotion(moveFromBoard, moveToBoard, pieceToMove);  
                        
            moveCount++;

            halfMoveClock ++;

            if (!whiteToMove)
                fullMoveClock++;

            //if (confirmedMove)
                CalculateUsefulBitboards();

            SwitchSides();

            //UpdateIsKingInCheck(); 
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
            if (whiteToMove)
            {
                if (whiteCanCastleKingside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.G1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.H1);
                            PlacePiece(PieceType.Rook, PieceColour.White, LookupTables.F1);

                            //whiteCanCastleKingside = false;
                        }

                        zobrist ^= ZobristKey.WhiteCastleKingside;
                        //zobrist ^= ZobristKey.WhiteCastleQueenside;
                        whiteCanCastleKingside = false;     //Any king move means we can no longer castle
                        //whiteCanCastleQueenside = false;
                        //else if (moveFromBoard == BitboardSquare.H1)     //Moved Rook
                        //    whiteCanCastleKingside = false;
                    }

                    if (moveFromBoard == LookupTables.H1)     //Moved Rook
                    {
                        zobrist ^= ZobristKey.WhiteCastleKingside;
                        whiteCanCastleKingside = false;
                    }
                }

                if (whiteCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C1)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.A1);
                            PlacePiece(PieceType.Rook, PieceColour.White, LookupTables.D1);

                            whiteCanCastleQueenside = false;
                            //whiteCanCastleKingside = false;  
                        }

                        zobrist ^= ZobristKey.WhiteCastleQueenside;
                        whiteCanCastleQueenside = false;

                        //else if (moveFromBoard == BitboardSquare.A1)     //Moved Rook
                        //    whiteCanCastleQueenside = false;
                    }

                    if (moveFromBoard == LookupTables.A1)     //Moved Rook
                    {
                        zobrist ^= ZobristKey.WhiteCastleQueenside;                        
                        whiteCanCastleQueenside = false;
                    }
                }
                
                if (blackCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H8)
                    {
                        zobrist ^= ZobristKey.BlackCastleKingside;                        
                        blackCanCastleKingside = false;
                    }
                }

                if (blackCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A8)
                    {
                        zobrist ^= ZobristKey.BlackCastleQueenside;
                        blackCanCastleQueenside = false;
                    }
                }
            }
            else
            {
                if (blackCanCastleKingside)
                {
                   if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.G8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.H8);
                            PlacePiece(PieceType.Rook, PieceColour.Black, LookupTables.F8);

                            //blackCanCastleQueenside = false;
                            //blackCanCastleKingside = false;
                        }

                        blackCanCastleKingside = false; //Any king move means we can no longer castle
                        //blackCanCastleQueenside = false;

                        zobrist ^= ZobristKey.BlackCastleKingside;
                       
                        //else if (moveFromBoard == BitboardSquare.H8)     //Moved Rook
                        //    blackCanCastleKingside = false;
                    }

                   if (moveFromBoard == LookupTables.H8)     //Moved Rook
                   {
                       blackCanCastleKingside = false;
                       zobrist ^= ZobristKey.BlackCastleKingside;
                   }
                }

                if (blackCanCastleQueenside)
                {
                    if (pieceToMove == PieceType.King)
                    {
                        if (moveToBoard == LookupTables.C8)     //No need to check origin square because we know from the whiteCanCastleKingside that this is the kings first move
                        {
                            //Move rook too
                            RemovePiece(LookupTables.A8);
                            PlacePiece(PieceType.Rook, PieceColour.Black, LookupTables.D8);

                            //blackCanCastleQueenside = false;
                        }

                        blackCanCastleQueenside = false;
                        //blackCanCastleKingside = false;
                        zobrist ^= ZobristKey.BlackCastleQueenside;
                        //else if (moveFromBoard == BitboardSquare.A8)     //Moved Rook
                        //    blackCanCastleQueenside = false;
                    }

                    if (moveFromBoard == LookupTables.A8)     //Moved Rook
                    {
                        blackCanCastleQueenside = false;
                        zobrist ^= ZobristKey.BlackCastleQueenside;
                    }
                }

                //Check if black has captured a white rook
                if (whiteCanCastleKingside)
                {
                    if (moveToBoard == LookupTables.H1)
                    {
                        whiteCanCastleKingside = false;
                        zobrist ^= ZobristKey.WhiteCastleKingside;
                    }
                }

                if (whiteCanCastleQueenside)
                {
                    if (moveToBoard == LookupTables.A1)
                    {
                        whiteCanCastleQueenside = false;
                        zobrist ^= ZobristKey.WhiteCastleQueenside;
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
                if (whiteToMove)
                {
                    int differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard) - BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard);
                    if ((moveToBoard & enPassantPosition) != 0)      //Move is an en-passant capture
                    {
                        //Remove captured piece
                        RemovePiece(enPassantPosition >> 8);
                        enPassantPosition = 0;
                    }
                    else if(differenceInMoveIndex == 16)
                    {
                        enPassantPosition = moveToBoard >> 8;
                    }
                    else
                        enPassantPosition = 0;
                }
                else
                {
                    int differenceInMoveIndex = BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard) - BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard);
                    if ((moveToBoard & enPassantPosition) != 0)      //Move is an en-passant capture
                    {
                        //Remove captured piece
                        RemovePiece(enPassantPosition << 8);
                        enPassantPosition = 0;
                    }
                    else if (differenceInMoveIndex == 16)
                    {
                        enPassantPosition = moveToBoard << 8;
                    }
                    else
                        enPassantPosition = 0;
                }
            }
            else
                enPassantPosition = 0;
        }

        //private void AddPgnValue(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove)
        //{
        //    string move = PgnTranslator.ToPgnMove(this, moveFromBoard, moveToBoard, pieceToMove);

        //    pgnMove = String.Copy(move);
        //}
            

        public void SwitchSides()
        {
            zobrist ^= ZobristKey.BlackToMove;

            if (whiteToMove)
                whiteToMove = false;
            else
                whiteToMove = true;
        }

        public void SetPlayerColour(PieceColour colour)
        {
            if (colour == PieceColour.White)
                whiteToMove = true;
            else
                whiteToMove = false;
        }

        private void SaveBoardState()
        {
             BoardState state = new BoardState();

             state.WhiteToMove = WhiteToMove;

             state.WhitePawns = whitePawns;
             state.WhiteKnights = whiteKnights;
             state.WhiteBishops = whiteBishops;
             state.WhiteRooks = whiteRooks;
             state.WhiteQueen = whiteQueen;
             state.WhiteKing = whiteKing;

             state.BlackPawns = blackPawns;
             state.BlackKnights = blackKnights;
             state.BlackBishops = blackBishops;
             state.BlackRooks = blackRooks;
             state.BlackQueen = blackQueen;
             state.BlackKing = blackKing;

             state.EnPassantPosition = enPassantPosition;
             
             state.WhiteCanCastleQueenside = whiteCanCastleQueenside;
             state.WhiteCanCastleKingside = whiteCanCastleKingside;
             state.BlackCanCastleQueenside = blackCanCastleQueenside;
             state.BlackCanCastleKingside = blackCanCastleKingside;

             state.WhiteInCheck = whiteInCheck;
             state.BlackInCheck = blackInCheck;

             state.HalfMoveClock = halfMoveClock;
             state.FullMoveClock = fullMoveClock;

             state.PgnMove = pgnMove;

             state.ZobristKey = zobrist;

             boardStates.Add(state);
        }

        public void UnMakeLastMove()
        {
            UnMakeLastMove(true);
        }

        public void UnMakeLastMove(bool verbose)
        {
            moveCount--;

            BoardState state = boardStates[moveCount];
            
            whiteToMove = state.WhiteToMove;

            whitePawns = state.WhitePawns;
            whiteKnights = state.WhiteKnights;
            whiteBishops = state.WhiteBishops;
            whiteRooks = state.WhiteRooks;
            whiteQueen = state.WhiteQueen;
            whiteKing = state.WhiteKing;

            blackPawns = state.BlackPawns;
            blackKnights = state.BlackKnights;
            blackBishops = state.BlackBishops;
            blackRooks = state.BlackRooks;
            blackQueen = state.BlackQueen;
            blackKing = state.BlackKing;

            enPassantPosition = state.EnPassantPosition;

            whiteCanCastleQueenside = state.WhiteCanCastleQueenside;
            whiteCanCastleKingside = state.WhiteCanCastleKingside;
            blackCanCastleQueenside = state.BlackCanCastleQueenside;
            blackCanCastleKingside = state.BlackCanCastleKingside;

            halfMoveClock = state.HalfMoveClock;
            fullMoveClock = state.FullMoveClock;

            whiteInCheck = state.WhiteInCheck;
            blackInCheck = state.BlackInCheck;

            pgnMove = state.PgnMove;

            zobrist = state.ZobristKey;

            boardStates.RemoveAt(boardStates.Count-1);

            //if(verbose)
                CalculateUsefulBitboards();
        } 
        
        #endregion Make move methods

        //Gets called every time a move is made to update all useful boards
        public void CalculateUsefulBitboards()
        {
            allWhiteOccupiedSquares = whitePawns | whiteKnights | whiteBishops | whiteRooks | whiteQueen | whiteKing;
            allBlackOccupiedSquares = blackPawns | blackKnights | blackBishops | blackRooks | blackQueen | blackKing;
            allOccupiedSquares = allWhiteOccupiedSquares | allBlackOccupiedSquares;
            emptySquares = allOccupiedSquares ^ fullBoard;
            whiteOrEmpty = allWhiteOccupiedSquares | emptySquares;
            blackOrEmpty = allBlackOccupiedSquares | emptySquares;
        }
        
        /// <summary>
        /// Sets a board with a fen position
        /// </summary>
        /// <param name="fenNotation"></param>
        public void SetFENPosition(string fenNotation)
        {
            BoardState state = FenTranslator.ToBoardState(fenNotation);

            SetPosition(state);

            CalculateUsefulBitboards();

            log.Info(string.Format("Set board position to: {0}", fenNotation));
        }

        public void SetPGNPosition(string pgnNotation)
        {
            PgnTranslator.ToBoard(this, pgnNotation);
        }
        
        #region slower debug get methods

        #region WriteWriteBoardToConsole methods

        public void WriteBoardToConsole()
        {
            string piece = " ";

            char[] squares = new char[64];

            AddPieceLetterToSquares(squares, whitePawns, 'p');
            AddPieceLetterToSquares(squares, blackPawns, 'P');

            AddPieceLetterToSquares(squares, whiteKnights, 'n');
            AddPieceLetterToSquares(squares, blackKnights, 'N');

            AddPieceLetterToSquares(squares, whiteBishops, 'b');
            AddPieceLetterToSquares(squares, blackBishops, 'B');

            AddPieceLetterToSquares(squares, whiteRooks, 'r');
            AddPieceLetterToSquares(squares, blackRooks, 'R');

            AddPieceLetterToSquares(squares, whiteQueen, 'q');
            AddPieceLetterToSquares(squares, blackQueen, 'Q');

            AddPieceLetterToSquares(squares, whiteKing, 'k');
            AddPieceLetterToSquares(squares, blackKing, 'K');

            for (int rank = 7; rank >= 0; rank--)
            {
                Console.WriteLine("");
                Console.WriteLine("-------------------------");
                Console.Write("|");

                for (int file = 0; file < 8; file++)
                {
                    int index = rank * 8 + file;

                    if (char.IsLetter(squares[index]))
                        Console.Write(squares[index]);
                    else
                        Console.Write(" ");
                    Console.Write(piece + "|");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("-------------------------");
        }

        public string BoardToString()
        {
            string boardPos = string.Empty;
            string piece = " ";

            char[] squares = new char[64];

            AddPieceLetterToSquares(squares, whitePawns, 'p');
            AddPieceLetterToSquares(squares, blackPawns, 'P');

            AddPieceLetterToSquares(squares, whiteKnights, 'n');
            AddPieceLetterToSquares(squares, blackKnights, 'N');

            AddPieceLetterToSquares(squares, whiteBishops, 'b');
            AddPieceLetterToSquares(squares, blackBishops, 'B');

            AddPieceLetterToSquares(squares, whiteRooks, 'r');
            AddPieceLetterToSquares(squares, blackRooks, 'R');

            AddPieceLetterToSquares(squares, whiteQueen, 'q');
            AddPieceLetterToSquares(squares, blackQueen, 'Q');

            AddPieceLetterToSquares(squares, whiteKing, 'k');
            AddPieceLetterToSquares(squares, blackKing, 'K');

            for (int rank = 7; rank >= 0; rank--)
            {
                boardPos += System.Environment.NewLine;
                boardPos += "-------------------------" + System.Environment.NewLine;
                boardPos += "|";

                for (int file = 0; file < 8; file++)
                {
                    int index = rank * 8 + file;

                    if (char.IsLetter(squares[index]))
                        boardPos += squares[index];
                    else
                        boardPos += " ";
                    boardPos += piece + "|";
                }
            }

            boardPos += System.Environment.NewLine;
            boardPos += "-------------------------" + System.Environment.NewLine;

            return boardPos;
        }

        private void AddPieceLetterToSquares(char[] squares, ulong piecePosition, char letterToAdd)
        {
            List<byte> pieceSquares = BitboardOperations.GetSquareIndexesFromBoardValue(piecePosition);

            for (int i = 0; i < pieceSquares.Count; i++)
            {
                squares[pieceSquares[i]] = letterToAdd;
            }
        }

        #endregion WriteWriteBoardToConsole methods

        /// <summary>
        /// Checks that the board satisies it end move criteria.
        /// Used only for debugging as will slow the algorithm down too much.
        /// </summary>
        public void FullAssert()
        {
            if (blackKing == 0)
            {
                string moveList = GetMovesList();

                throw new ChessBoardException(string.Format("There is no black king: {0}", moveList));
            }

            if (blackKing == 0)
            {
                throw new ChessBoardException("There is no black king.");
            }

            //Check all occupied squares
            if (allWhiteOccupiedSquares != (whitePawns | whiteKnights | whiteBishops | whiteRooks | whiteQueen | whiteKing))
                throw new ChessBoardException("The white pieces don't match the allwhiteOccupiedSquares board.");

            if (allBlackOccupiedSquares != (blackPawns | blackKnights | blackBishops | blackRooks | blackQueen | blackKing))
                throw new ChessBoardException("The black pieces don't match the allBlackOccupiedSquares board.");

            if (allOccupiedSquares != (allWhiteOccupiedSquares | allBlackOccupiedSquares))
                throw new ChessBoardException("The occupied squares board does not match the allWhiteOccupiedSquares | allBlackOccupiedSquares board.");
        }

        /// <summary>
        /// Gets the moveslist from the boardstate history
        /// </summary>
        /// <returns></returns>
        private string GetMovesList()
        {
            string movesList = string.Empty;

            foreach (BoardState state in boardStates)
            {
                movesList += FenTranslator.ToFENString(state) + " || ";
            }

            return movesList;
        }

        #endregion slower debug methods

        public string GetFENNotation()
        {
            return FenTranslator.ToFENString(GetCurrentBoardState());
        }

        public string GetPGNNotation()
        {
            return PgnTranslator.ToPgnMovesList(this);
        }

        public BoardState GetCurrentBoardState()
        {
            BoardState state = new BoardState();

            state.WhiteToMove = WhiteToMove;

            state.WhitePawns = whitePawns;
            state.WhiteKnights = whiteKnights;
            state.WhiteBishops = whiteBishops;
            state.WhiteRooks = whiteRooks;
            state.WhiteQueen = whiteQueen;
            state.WhiteKing = whiteKing;

            state.BlackPawns = blackPawns;
            state.BlackKnights = blackKnights;
            state.BlackBishops = blackBishops;
            state.BlackRooks = blackRooks;
            state.BlackQueen = blackQueen;
            state.BlackKing = blackKing;

            state.EnPassantPosition = enPassantPosition;

            state.WhiteCanCastleQueenside = whiteCanCastleQueenside;
            state.WhiteCanCastleKingside = whiteCanCastleKingside;
            state.BlackCanCastleQueenside = blackCanCastleQueenside;
            state.BlackCanCastleKingside = blackCanCastleKingside;

            state.HalfMoveClock = halfMoveClock;
            state.FullMoveClock = fullMoveClock;

            state.ZobristKey = zobrist; 

            return state;
        }

        #region Zobrist functions

        private void UpdateZobrist(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            UpdateEnPassantZobrist(specialMove, moveToBoard);
            
            UpdateZobristForMove(moveFromBoard, moveToBoard, pieceToMove, specialMove);
            
            //Update for side
            //zobrist ^= ZobristKey.BlackToMove;
        }

        private void UpdateZobristForMove(ulong moveFromBoard, ulong moveToBoard, PieceType pieceToMove, SpecialMoveType specialMove)
        {
            //Remove old piece
            zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, MoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard)];
            
            if (IsMovePromotionCapture(specialMove))
            {
                //remove captured piece
                PieceType capturedPiece = BoardChecking.GetPieceTypeOnSquare(this, moveToBoard);
                zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, NotMoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add promoted piece
                PieceType promotedPiece = PromotedPiece(specialMove);
                zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, MoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if(IsMovePromotionNonCapture(specialMove))
            {
                //Add promoted piece
                PieceType promotedPiece = PromotedPiece(specialMove);
                zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(promotedPiece, MoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            }
            else if (specialMove == SpecialMoveType.Capture)
            {
                //remove captured piece
                PieceType capturedPiece = BoardChecking.GetPieceTypeOnSquare(this, moveToBoard);
                zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(capturedPiece, NotMoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];     
                
                //Add moved piece
                zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(pieceToMove, MoveColour), (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                       
            }
            else if (IsMoveCastling(specialMove))
            {
                int movedPieceNumber;

                if(whiteToMove)
                    movedPieceNumber = ZobristHash.WHITE_KING;
                else
                    movedPieceNumber = ZobristHash.BLACK_KING;

                //Add moved piece
                zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];           
            
                //Add rook
                if ((moveToBoard & LookupTables.C1) > 0)
                {
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.A1)];
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.D1)];
                }
                else if ((moveToBoard & LookupTables.G1) > 0)
                {
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.H1)];
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.WHITE_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.F1)];
                }
                else if ((moveToBoard & LookupTables.C8) > 0)
                {
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.A8)];
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.D8)];
                }
                else if ((moveToBoard & LookupTables.G8) > 0)
                {
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.H8)];
                    zobrist ^= ZobristKey.PiecePositions[ZobristHash.BLACK_ROOK, (int)BitboardOperations.GetSquareIndexFromBoardValue(LookupTables.F8)];
                }
            }
            else if(specialMove == SpecialMoveType.ENPassantCapture)
            {
                int capturedPieceNumber;
                int movedPieceNumber;

                if (whiteToMove)
                {
                    movedPieceNumber = ZobristHash.WHITE_PAWN;
                    capturedPieceNumber = ZobristHash.BLACK_PAWN;
                }
                else
                {
                    movedPieceNumber = ZobristHash.BLACK_PAWN;                    
                    capturedPieceNumber = ZobristHash.WHITE_PAWN;                    
                }

                //Remove captured pawn
                zobrist ^= ZobristKey.PiecePositions[capturedPieceNumber, (int)BitboardOperations.GetSquareIndexFromBoardValue(enPassantPosition)];
                
                //Add moved pawn
                zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];                
            }
            else
            {
                int movedPieceNumber = ZobristHash.GetPieceValue(pieceToMove, MoveColour);

                //Just a normal move
                //zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, (int)BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard)];
                zobrist ^= ZobristKey.PiecePositions[movedPieceNumber, (int)BitboardOperations.GetSquareIndexFromBoardValue(moveToBoard)];              
            }
        }

        private void UpdateEnPassantZobrist(SpecialMoveType specialMove, ulong moveToBoard)
        {
            // Previous position
            if (boardStates[boardStates.Count - 1].EnPassantPosition > (ulong)0)
                zobrist ^= ZobristHash.HashEnPassantSquare(boardStates[boardStates.Count - 1].EnPassantPosition);
            //Current Postion
            if (specialMove == SpecialMoveType.DoublePawnPush)
            {
                zobrist ^= ZobristHash.HashEnPassantSquare(moveToBoard);
#warning May have to change this to only add enpassant hash if there is a pawn which can capture
            }            
        }

        /// <summary>
        /// Calculates the Zobrist key from the board
        /// </summary>
        private void CalculateZobristKey()
        {
            zobrist = ZobristHash.HashBoard(this);
        }

        #endregion Zobrist functions

        public void ResetFlags()
        {
            boardStates = new List<BoardState>();

            AllowAllCastling(true);
            pgnMove = String.Empty;
            
            moveCount = 0;

            whiteToMove = true;
        
            halfMoveClock = 0;      //To track captures or pawn advance - for 50 move rule
            fullMoveClock = 1;
            enPassantPosition = 0;
        }
    }
}
