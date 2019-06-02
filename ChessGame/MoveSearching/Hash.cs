using ChessGame.PossibleMoves;

namespace ChessGame.MoveSearching
{
    public enum HashNodeType
    {
        Exact,
        UpperBound,
        LowerBound
    }

    public struct Hash
    {
        internal ulong Key;
        internal int Depth;
        internal int Flags;
        internal decimal Score;
        internal HashNodeType NodeType;
        internal PieceMoves BestMove;
        internal int Ancient;

        //internal string fenPosition;  //For debugging only. remove once fixed

    }
}
