using ChessEngine.BoardSearching;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;

namespace ChessEngine.BoardRepresentation;

// Represents a game board state needed for a full game
// including all 12 bitboards who is to move and flags for moves like en-passant and castling
[Serializable]
public class Board
{
    private const ulong _fullBoard = ulong.MaxValue;

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

    public ulong AllWhiteOccupiedSquares;
    public ulong AllBlackOccupiedSquares;

    public ulong WhiteNonEndGamePieces;
    public ulong BlackNonEndGamePieces;

    public ulong AllOccupiedSquares;

    public ulong EmptySquares;

    public ulong WhiteOrEmpty;
    public ulong BlackOrEmpty;

    public bool WhiteCanCastleQueenside = true;
    public bool WhiteCanCastleKingside = true;

    public bool BlackCanCastleQueenside = true;
    public bool BlackCanCastleKingside = true;

    public ulong EnPassantPosition;

    public bool WhiteToMove = true;

    public int HalfMoveClock;

    public int FullMoveClock = 1;

    private Stack<BoardState> _history;

    public ulong Zobrist;

    public Board()
    {
        LookupTables.InitialiseAllTables();
        PieceValidMoves.GenerateMoveArrays();
        TranspositionTable.InitialiseTable();
        ZobristHash.Initialise();

        _history = new Stack<BoardState>();
    }

    // Initialises the pieces to a games starting position
    // Note: bitboards go right and up from a1-h8. Bitboards run from right to left,
    // therefore the far left digit is a1 and the leftmeos digit is h8
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

        HalfMoveClock = 0;
        FullMoveClock = 1;

        _history.Clear();

        CalculateUsefulBitboards();

        CalculateZobristKey();
    }

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

        HalfMoveClock = 0;
        FullMoveClock = 1;

        _history.Clear();

        CalculateUsefulBitboards();

        CalculateZobristKey();
    }

    public void SetPosition(string fenPosition)
    {
        SetPosition(FenTranslator.ToBoardState(fenPosition));
    }

    private void SetPosition(in BoardState state)
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

        WhiteToMove = !WhiteToMove;
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
        WhiteNonEndGamePieces = WhiteKnights | WhiteBishops | WhiteRooks | WhiteQueen;
        BlackNonEndGamePieces = BlackKnights | BlackBishops | BlackRooks | BlackQueen;

        AllWhiteOccupiedSquares = WhitePawns | WhiteNonEndGamePieces | WhiteKing;
        AllBlackOccupiedSquares = BlackPawns | BlackNonEndGamePieces | BlackKing;
        AllOccupiedSquares      = AllWhiteOccupiedSquares | AllBlackOccupiedSquares;
        EmptySquares            = AllOccupiedSquares ^ _fullBoard;
        WhiteOrEmpty            = AllWhiteOccupiedSquares | EmptySquares;
        BlackOrEmpty            = AllBlackOccupiedSquares | EmptySquares;
    }

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

    /// <summary>
    /// Calculates the Zobrist key from the board
    /// </summary>
    private void CalculateZobristKey()
    {
        Zobrist = ZobristHash.HashBoard(GetCurrentBoardState());
    }

    public void ResetFlags()
    {
        _history = new Stack<BoardState>();

        AllowAllCastling(true);
        //m_PgnMove = string.Empty;

        WhiteToMove = true;

        HalfMoveClock = 0;      //To track captures or pawn advance - for 50 move rule
        FullMoveClock = 1;
        EnPassantPosition = 0;
    }

    public void AddToHistory(in BoardState state)
    {
        _history.Push(state);
    }

    public ulong PeekHistory()
    {
        return _history.Peek().EnPassantPosition;
    }

    public BoardState PopHistory()
    {
        return _history.Pop();
    }
}

