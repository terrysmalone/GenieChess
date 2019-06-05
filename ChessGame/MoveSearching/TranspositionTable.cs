using System;
using ChessGame.Debugging;

namespace ChessGame.MoveSearching
{  
    public static class TranspositionTable
    {
        private static Hash[] table;

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



        private static ulong transpositionTableSize = 20940347; //218591;//548591;//20940347; //2000003;// 1048583; //Must be prime 
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

                table = new Hash[transpositionTableSize];
                initialised = true;
            }
        }

        internal static void Add(Hash hash)
        {
            var index = hash.Key % transpositionTableSize;

            var currentHash = table[index];

            if (currentHash.Key != 0)  //There's already one there
            {
                // If the new one searches deeper replace it
                if (hash.Depth >= currentHash.Depth)    
                {
                    table[index] = hash;

#if FullNodeCountDebug
                    CountDebugger.Transposition_HashReplaced++;
#endif
                }
            }
            else
            {
                table[index] = hash;

#if FullNodeCountDebug
                CountDebugger.Transposition_HashAdded++;
#endif
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

                        return hash;

                        //if (((Hash)hash).NodeType == HashNodeType.Exact)
                        //{
                        //    value = ((Hash)hash).Score;
                        //    return true;
                        //}

                        //if (((Hash)hash).NodeType == HashNodeType.Alpha &&
                        //    ((Hash)hash).Score <= alpha)
                        //{
                        //    value =  alpha;
                        //    return true;
                        //}

                        //if (((Hash)hash).NodeType == HashNodeType.Beta &&
                        //    ((Hash)hash).Score >= beta)
                        //{
                        //    value = beta;
                        //    return true;
                        //}                            
                    }

                    //RememberBestMove();
                    
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
                        
            var hash = table[index];

            return hash.Key != 0 ? hash : new Hash();
        }

        internal static void ClearAncients()
        {
            throw new NotImplementedException();
        }

        internal static void ClearAll()
        {
            table = new Hash[transpositionTableSize];
        }
    }
}
