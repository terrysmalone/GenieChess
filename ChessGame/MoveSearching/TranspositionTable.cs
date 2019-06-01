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



        private static ulong transpositionTableSize = 20940347; //2000003;// 1048583; //Must be prime 
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

                table = new Hash[transpositionTableSize];
                initialised = true;
            }
        }

        internal static void Add(Hash hash)
        {
            ulong index = hash.Key % transpositionTableSize;

            //if(table.ContainsKey(index))  

            //Hash currentTable = table[index];
            //if (currentTable.Key != 0)
            //{
                //VERIFY

                //Decide if we should replace
                //Replace if new depth is more than or equal to the stored depth
                //if (hash.Depth >= currentTable.Depth)
            //        table[index] = hash;

                    //Replace always
                    //table[index] = hash;
            //}
            //else
           // {
                table[index] = hash;
           // }
        }

        internal static Hash ProbeTable(ulong zobristKey, int depth, decimal alpha, decimal beta)
        {
            Hash hash = Search(zobristKey);

            CountDebugger.Transposition_Searches++; 

            if (hash.Key != 0)
            {
                //Verify
                if (hash.Key == zobristKey)
                {
                    CountDebugger.Transposition_MatchCount++;

                    if (hash.Depth >= depth)
                    {
                        CountDebugger.Transposition_MatchAndUsed++;

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
                    CountDebugger.Transposition_CollisionCount++;
                    //Collision
                    //if (((Hash)hash).Depth <= depth)
                    //    Add((Hash)hash);

                    return new Hash();
                    //return false;
                }
            }

            return new Hash(); 
            //return false;
        }

        private static Hash Search(ulong zobristKey)
        {
            ulong index = zobristKey % transpositionTableSize;
                        
            Hash hash = table[index];
            //if (table.ContainsKey(index))
            if (hash.Key != 0)
            {
                return hash;
            }
            else
                return new Hash();
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
