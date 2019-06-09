namespace ChessEngine.MoveSearching
{  
    public static class TranspositionTable
    {
        private static Hash[] s_Table;

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



        private static ulong transpositionTableSize = 1048583; //20940347; //Must be prime 
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
            if (!initialised){
                ZobristHash.Initialise();

                s_Table = new Hash[transpositionTableSize];
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
                if (hash.Depth >= currentHash.Depth || currentHash.Ancient);    
                {
                    s_Table[index] = hash;

#if FullNodeCountDebug
                    CountDebugger.Transposition_HashReplaced++;
#endif
                }
            }
            else
            {
                if (currentHash.Ancient)
                {
                    s_Table[index] = hash;

#if FullNodeCountDebug
                CountDebugger.Transposition_HashAdded++;
#endif
                }
            }
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

        internal static void ClearAll()
        {
            s_Table = new Hash[transpositionTableSize];
        }

        internal static void ClearAncients()
        {
            for (var i = 0; i < s_Table.Length; i++)
            {
                s_Table[i].Ancient = true;
            }
        }
    }
}
