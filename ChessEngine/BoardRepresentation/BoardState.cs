namespace ChessEngine.BoardRepresentation;

// Keeps track of the state of a board before any moves are made. Allows for the easy undoing of moves
[Serializable]
public struct BoardState
{
    public bool WhiteToMove;

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

    public ulong EnPassantPosition;

    public bool WhiteCanCastleQueenside;
    public bool WhiteCanCastleKingside;
    public bool BlackCanCastleQueenside;
    public bool BlackCanCastleKingside;

    public int HalfMoveClock;
    public int FullMoveClock;

    public ulong ZobristKey;

    public override bool Equals(object obj)
    {
        if (!(obj is BoardState))
            return false;

        var mys = (BoardState)obj;

        if (mys.ZobristKey != ZobristKey)
            return false;

        if (mys.WhiteToMove != WhiteToMove)
            return false;

        if(mys.WhitePawns != WhitePawns)
            return false;

        if(mys.WhiteKnights != WhiteKnights)
            return false;

        if(mys.WhiteBishops != WhiteBishops)
            return false;

        if(mys.WhiteRooks != WhiteRooks)
            return false;

        if(mys.WhiteQueen != WhiteQueen)
            return false;

        if(mys.WhiteKing != WhiteKing)
            return false;

        if(mys.BlackPawns != BlackPawns)
            return false;

        if(mys.BlackKnights != BlackKnights)
            return false;

        if(mys.BlackBishops != BlackBishops)
            return false;

        if(mys.BlackRooks != BlackRooks)
            return false;

        if(mys.BlackQueen != BlackQueen)
            return false;

        if(mys.BlackKing != BlackKing)
            return false;

        if(mys.EnPassantPosition != EnPassantPosition)
            return false;

        if(mys.WhiteCanCastleQueenside != WhiteCanCastleQueenside)
            return false;

        if(mys.WhiteCanCastleKingside != WhiteCanCastleKingside)
            return false;

        if(mys.BlackCanCastleQueenside != BlackCanCastleQueenside)
            return false;

        if(mys.BlackCanCastleKingside != BlackCanCastleKingside)
            return false;

        if(mys.HalfMoveClock != HalfMoveClock)
            return false;

        if(mys.FullMoveClock != FullMoveClock)
            return false;

        return true;
    }

    public static bool operator ==(BoardState left, BoardState right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoardState left, BoardState right)
    {
        return !(left == right);
    }
}

