﻿using System.Collections.Generic;

namespace ResourceLoading
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