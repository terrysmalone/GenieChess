using ChessEngine.PossibleMoves;

namespace ChessEngine.MoveSearching
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
        internal int Score;
        internal HashNodeType NodeType;
        internal PieceMove BestMove;
        internal bool Ancient;

        //internal string fenPosition;  //For debugging only. remove once fixed

    }
}
