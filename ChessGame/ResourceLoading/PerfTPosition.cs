using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ResourceLoading
{
    /// <summary>
    /// Stores Perft positions and their expected results
    /// </summary>
    public struct PerfTPosition
    {
        public string Name;
        public string FenPosition;

        public List<ulong> Results;
    }
}
