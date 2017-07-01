using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Debugging
{
    public static class CountDebugger
    {
        public static ulong Transposition_Searches = 0;
        public static ulong Transposition_CollisionCount = 0;
        public static ulong Transposition_MatchCount = 0;
        public static ulong Transposition_MatchAndUsed = 0;

        public static ulong NullMovesPruned = 0;

        public static ulong Evaluations = 0;
        public static ulong Nodes = 0;

        public static void ClearAll()
        {
            Transposition_CollisionCount = 0;
            Transposition_MatchCount = 0;
            Transposition_MatchAndUsed = 0;
            Transposition_Searches = 0;

            NullMovesPruned = 0;

            Evaluations = 0;
            Nodes = 0;
        }

    }
}
