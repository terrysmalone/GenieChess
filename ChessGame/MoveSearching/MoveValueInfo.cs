using ChessGame.PossibleMoves;
using System;

namespace ChessGame.MoveSearching
{
    /// <summary>
    /// Stores info relating to a move. Used for storing iterative deepening lines
    /// </summary>
    public struct MoveValueInfo
    {
        public PieceMoves Move;
        public decimal Score;
        public TimeSpan DepthTime;
        public TimeSpan AccumulatedTime;

        public ulong NodesVisited;        
    }
}
