using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using log4net;

namespace ChessEngine.BoardRepresentation
{
    // Represents a game board state needed for a full game
    // including all 12 bitboards who is to move and flags for moves like en-passant and castling
    [Serializable]
    public class Board 
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection
                                                                      .MethodBase
                                                                      .GetCurrentMethod()
                                                                      .DeclaringType);

        private const ulong FullBoard = ulong.MaxValue;

        private BoardState _currentBoardState;
        private UsefulBitboards _usefulBitboards;

        public bool WhiteCanCastleQueenside = true;
        public bool WhiteCanCastleKingside = true;

        public bool BlackCanCastleQueenside = true;
        public bool BlackCanCastleKingside = true;

        public ulong EnPassantPosition;

        private Stack<BoardState> m_History;

        public ulong Zobrist;
        
        public Board()
        {
            LookupTables.InitialiseAllTables();            
            PieceValidMoves.GenerateMoveArrays();
            TranspositionTable.InitialiseTable();
            ZobristHash.Initialise();

            m_History = new Stack<BoardState>();
        }
        
        // Initialises the pieces to a games starting position
        // Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
        // therefore the far left digit is a1 and the leftmeos digit is h8
        public void InitaliseStartingPosition()
        {
            _currentBoardState = new BoardState
            {
                WhiteToMove = true,
                WhitePawns = LookupTables.A2 | LookupTables.B2 | LookupTables.C2 | LookupTables.D2 | LookupTables.E2 | LookupTables.F2 | LookupTables.G2 | LookupTables.H2,
                WhiteKnights = LookupTables.B1 | LookupTables.G1,
                WhiteBishops = LookupTables.C1 | LookupTables.F1,
                WhiteRooks = LookupTables.A1 | LookupTables.H1,
                WhiteQueen = LookupTables.D1,
                WhiteKing = LookupTables.E1,

                BlackPawns = LookupTables.A7 | LookupTables.B7 | LookupTables.C7 | LookupTables.D7 | LookupTables.E7 | LookupTables.F7 | LookupTables.G7 | LookupTables.H7,
                BlackKnights = LookupTables.B8 | LookupTables.G8,
                BlackBishops = LookupTables.C8 | LookupTables.F8,
                BlackRooks = LookupTables.A8 | LookupTables.H8,
                BlackQueen = LookupTables.D8,
                BlackKing = LookupTables.E8,

                HalfMoveClock = 0,
                FullMoveClock = 1
            };

            m_History.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }
        
        public void ClearBoard()
        {
            _currentBoardState = new BoardState();

            m_History.Clear();

            CalculateUsefulBitboards();

            CalculateZobristKey();
        }

        public void SetPosition(string fenPosition)
        {
            SetPosition(FenTranslator.ToBoardState(fenPosition));
        }

        private void SetPosition(BoardState state)
        {
            _currentBoardState = new BoardState
            {
                WhiteToMove = state.WhiteToMove,
                WhitePawns = state.WhitePawns,
                WhiteKnights = state.WhiteKnights,
                WhiteBishops = state.WhiteBishops,
                WhiteRooks = state.WhiteRooks,
                WhiteQueen = state.WhiteQueen,
                WhiteKing = state.WhiteKing,
                BlackPawns = state.BlackPawns,
                BlackKnights = state.BlackKnights,
                BlackBishops = state.BlackBishops,
                BlackRooks = state.BlackRooks,
                BlackQueen = state.BlackQueen,
                BlackKing = state.BlackKing,
                EnPassantPosition = state.EnPassantPosition,
                WhiteCanCastleQueenside = state.WhiteCanCastleQueenside,
                WhiteCanCastleKingside = state.WhiteCanCastleKingside,
                BlackCanCastleQueenside = state.BlackCanCastleQueenside,
                BlackCanCastleKingside = state.BlackCanCastleKingside,
                HalfMoveClock = state.HalfMoveClock,
                FullMoveClock = state.FullMoveClock
            };

            CalculateZobristKey();

            CalculateUsefulBitboards();
        }

        public string GetPosition()
        {
            return FenTranslator.ToFenString(GetCurrentBoardState());
        }
        
        public void SwitchSides()
        {
            Zobrist ^= ZobristKey.BlackToMove;

            _currentBoardState.WhiteToMove = !_currentBoardState.WhiteToMove;
        }

        public void AllowAllCastling(bool value)
        {
            WhiteCanCastleQueenside = value;
            WhiteCanCastleKingside = value;
            BlackCanCastleQueenside = value;
            BlackCanCastleKingside = value;
        }

        //Gets called every time a move is made to update all useful boards
        public void CalculateUsefulBitboards()
        {
            var whiteNonEndGamePieces = _currentBoardState.WhiteKnights | _currentBoardState.WhiteBishops | _currentBoardState.WhiteRooks | _currentBoardState.WhiteQueen;
            var blackNonEndGamePieces = _currentBoardState.BlackKnights | _currentBoardState.BlackBishops | _currentBoardState.BlackRooks | _currentBoardState.BlackQueen;

            var allWhiteOccupiedSquares = _currentBoardState.WhitePawns | whiteNonEndGamePieces | _currentBoardState.WhiteKing;
            var allBlackOccupiedSquares = _currentBoardState.BlackPawns | blackNonEndGamePieces | _currentBoardState.BlackKing;
            var allOccupiedSquares      = allWhiteOccupiedSquares | allBlackOccupiedSquares;
            var emptySquares            = allOccupiedSquares ^ FullBoard;
            var whiteOrEmpty            = allWhiteOccupiedSquares | emptySquares;
            var blackOrEmpty            = allBlackOccupiedSquares | emptySquares;

            _usefulBitboards = new UsefulBitboards
            {
                WhiteNonEndGamePieces =  whiteNonEndGamePieces,
                BlackNonEndGamePieces = blackNonEndGamePieces,
                AllWhiteOccupiedSquares = allWhiteOccupiedSquares,
                AllBlackOccupiedSquares = allBlackOccupiedSquares,
                AllOccupiedSquares = allOccupiedSquares,
                EmptySquares = emptySquares,
                WhiteOrEmpty = whiteOrEmpty,
                BlackOrEmpty = blackOrEmpty
            };
        }

        public BoardState GetCurrentBoardState()
        {
            return _currentBoardState;
        }

        public UsefulBitboards GetUsefulBitBoards()
        {
            return _usefulBitboards;
        }

        /// <summary>
        /// Calculates the Zobrist key from the board
        /// </summary>
        private void CalculateZobristKey()
        {
            Zobrist = ZobristHash.HashBoard(GetCurrentBoardState());
        }

        public void ResetFlags()
        {
            m_History = new Stack<BoardState>();

            AllowAllCastling(true);
            //m_PgnMove = string.Empty;

            _currentBoardState.WhiteToMove = true;
        
            _currentBoardState.HalfMoveClock = 0;      //To track captures or pawn advance - for 50 move rule
            _currentBoardState.FullMoveClock = 1;
            EnPassantPosition = 0;
        }

        public void AddToHistory(BoardState state)
        {
            m_History.Push(state);
        }

        public ulong PeekHistory()
        {
            return m_History.Peek().EnPassantPosition;
        }

        public BoardState PopHistory()
        {
            return m_History.Pop();
        }
    }
}
