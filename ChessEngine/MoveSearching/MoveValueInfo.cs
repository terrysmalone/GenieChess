using System;
using ChessEngine.PossibleMoves;

namespace ChessEngine.MoveSearching
{
    /// <summary>
    /// Stores info relating to a move. Used for storing iterative deepening lines
    /// </summary>
    public struct MoveValueInfo
    {
        public PieceMoves Move;
        public int Score;
        public TimeSpan DepthTime;
        public TimeSpan AccumulatedTime;

        public ulong NodesVisited;        
    }
}
