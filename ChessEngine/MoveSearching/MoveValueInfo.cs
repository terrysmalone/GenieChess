using ChessEngine.PossibleMoves;

namespace ChessEngine.MoveSearching;

// Stores info relating to a move. Used for storing iterative deepening lines
public struct MoveValueInfo
{
    public PieceMove Move;
    public int Score;
    public TimeSpan DepthTime;
    public TimeSpan AccumulatedTime;

    public ulong NodesVisited;
}

