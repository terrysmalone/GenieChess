namespace ChessEngine.MoveSearching
{  
    public static class TranspositionTable
    {
        private static Hash[] s_Table;

        private static Hash[] s_QuiescenceTable;

        /*****************************
         * Table size should be a prime number 
         * 2^x 
         * 
         * 2^20 + 7 - 1048583
         *
         *
         * 
         * 
         * 
         *********************************/



        private static ulong transpositionTableSize = 20940347; //1048583; 20940347; //Must be prime 
        private static bool initialised = false;


        /// <summary>
        /// Clears and reinitialises the transposition table
        /// </summary>
        public static void Restart()
        {
            initialised = false;
            InitialiseTable();
        }

        internal static void InitialiseTable()
        {
            if (!initialised)
            {
                ZobristHash.Initialise();

                s_Table = new Hash[transpositionTableSize];

                s_QuiescenceTable = new Hash[transpositionTableSize];

                initialised = true;
            }
        }

        internal static void Add(Hash hash)
        {
            var index = hash.Key % transpositionTableSize;

            var currentHash = s_Table[index];

            if (currentHash.Key != 0)  //There's already one there
            {
                // If the new one searches deeper replace it
                // Or if we've marked this as ancient and it's a different hash
                if (hash.Depth >= currentHash.Depth || (hash.Key != currentHash.Key && currentHash.Ancient))    
                {
                    s_Table[index] = hash;

#if FullNodeCountDebug
                    CountDebugger.Transposition_HashReplaced++;
#endif
                }
            }
            else
            {
                s_Table[index] = hash;

#if FullNodeCountDebug
                CountDebugger.Transposition_HashAdded++;
#endif
            }
        }

        internal static void AddQuiescenceHash(Hash hash)
        {
            var index = hash.Key % transpositionTableSize;

            var currentHash = s_QuiescenceTable[index];

            if (currentHash.Key != 0) //There's already one there
            {
                // If the new one searches deeper replace it
                // Or if we've marked this as ancient and it's a different hash
                if (hash.Depth >= currentHash.Depth || (hash.Key != currentHash.Key && currentHash.Ancient))
                {
                    s_QuiescenceTable[index] = hash;
                }
            }
            else
            {
                    s_QuiescenceTable[index] = hash;
            }
        }

        internal static void ClearAll()
        {
            s_Table           = new Hash[transpositionTableSize];
            s_QuiescenceTable = new Hash[transpositionTableSize];
        }

        internal static Hash ProbeTable(ulong zobristKey, int depth, decimal alpha, decimal beta)
        {
            var hash = Search(zobristKey);

#if FullNodeCountDebug
            CountDebugger.Transposition_Searches++; 
#endif

            if (hash.Key != 0)
            {
#if FullNodeCountDebug
                CountDebugger.Transposition_HashFound++;
#endif
                //Verify
                if (hash.Key == zobristKey)     
                {
#if FullNodeCountDebug
                    CountDebugger.Transposition_MatchCount++;
#endif

                    if (hash.Depth >= depth)
                    {
#if FullNodeCountDebug
                        CountDebugger.Transposition_MatchAndUsed++;
#endif

                        // If we probed it then it's proven useful. Set it to not be ancient
                        hash.Ancient = false;

                        return hash;                         
                    }
                }
                else
                {
                    return new Hash();
                }
            }

            return new Hash(); 
        }

        private static Hash Search(ulong zobristKey)
        {
            var index = zobristKey % transpositionTableSize;
                        
            var hash = s_Table[index];

            return hash.Key != 0 ? hash : new Hash();
        }

        internal static void ResetAncients()
        {
            for (var i = 0; i < s_Table.Length; i++)
            {
                s_Table[i].Ancient = true;
                s_QuiescenceTable[i].Ancient = true;
            }
        }

        internal static Hash ProbeQuiescenceTable(ulong zobristKey, decimal alpha, decimal beta)
        {
            var hash = SearchQuiescenceTable(zobristKey);

            if (hash.Key != 0)
            {
                //Verify
                if (hash.Key == zobristKey)
                {
                    // If we probed it then it's proven useful. Set it to not be ancient
                    hash.Ancient = false;
                    return hash;
                }

                return new Hash();
            }

            return new Hash();
        }

        private static Hash SearchQuiescenceTable(ulong zobristKey)
        {
            var index = zobristKey % transpositionTableSize;

            var hash = s_QuiescenceTable[index];

            return hash.Key != 0 ? hash : new Hash();
        }

    }
}
