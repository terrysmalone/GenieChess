using ChessGame.PossibleMoves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.MoveSearching
{
    /// <summary>
    /// Stores info relating to a move. Used for storing iterative deepening lines
    /// </summary>
    public struct PVInfo
    {
        public PieceMoves Move;
        public decimal Score;
        public TimeSpan DepthTime;
        public TimeSpan AccumulatedTime;

        public ulong NodesVisited;        
    }
}
