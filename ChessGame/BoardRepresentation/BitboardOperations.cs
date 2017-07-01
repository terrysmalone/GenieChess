using ChessGame.BoardSearching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.BoardRepresentation
{
    public static class BitboardOperations
    {
        private static byte[] index64 = {
                63,  0, 58,  1, 59, 47, 53,  2,
                60, 39, 48, 27, 54, 33, 42,  3,
                61, 51, 37, 40, 49, 18, 28, 20,
                55, 30, 34, 11, 43, 14, 22,  4,
                62, 57, 46, 52, 38, 26, 32, 41,
                50, 36, 17, 19, 29, 10, 13, 21,
                56, 45, 25, 31, 35, 16,  9, 12,
                44, 24, 15,  8, 23,  7,  6,  5  };

        private static ulong debruijn64 = 0x07EDD5E59A4E28C2;


        //public BitboardOperations()
        //{
        //   LookupTables.InitialiseAllTables();
        //}

        public static byte GetSquareIndexFromBoardValue(ulong boardValue)
        {
            return index64[((boardValue & (~boardValue + 1)) * debruijn64) >> 58];
        }

        public static byte GetSquareIndexFromBoardValueOld(ulong boardValue)
        {
            LookupTables.InitialiseAllTables();
#warning make faster

            if (boardValue == 0)
                return 0;

            for (byte i = 0; i < 64; i++)
            {
                if (LookupTables.SquareValuesFromIndex[i] == boardValue)
                    return i;
            }

            throw new Exception(string.Format("Bitboard value {0} does not represent a value", boardValue));
        }

        /// <summary>
        /// Returns the indexes of the bitboard value as a byte array
        /// </summary>
        /// <param name="boardValue"></param>
        /// <returns></returns>
        public static List<byte> GetSquareIndexesFromBoardValue(ulong boardValue)
        {
            List<byte> values = new List<byte>();

            ulong tempBoard = boardValue;

            while (boardValue != 0)
            {
                byte first = GetSquareIndexFromBoardValue(boardValue);
                values.Add(first);

                boardValue &= ~((ulong)1 << first);
            }

            return values;
        }

        public static byte GetPopCount(ulong boardValue)
        {
            byte count = 0;
            while (boardValue != 0)
            {
                count++;
                boardValue &= (boardValue - 1);
            }

            return count;
        }

        public static byte GetPopCountOld(ulong boardValue)
        {
            LookupTables.InitialiseAllTables();
#warning make faster really soon
            byte popCount = 0;
            ulong tempBoard = boardValue;

            for (int i = 63; i >= 0; i--)
            {
                ulong currentSquareValue = LookupTables.SquareValuesFromIndex[i];

                if (tempBoard >= currentSquareValue)
                {
                    popCount++;
                    tempBoard -= currentSquareValue;
                }

            }

            return popCount;
        }

        /// <summary>
        /// Splits a bit board with multiple squares set to a List of bitboards with one square set each
        /// </summary>
        /// <param name="boardToSplit"></param>
        /// <returns></returns>
        //public static List<ulong> SplitBoard(ulong boardToSplit)
        //{
        //    //LookupTables.InitialiseAllTables();

        //    List<byte> indexes = GetSquareIndexesFromBoardValue(boardToSplit);
        //    List<ulong> boards = new List<ulong>();

        //    for (int index = 0; index < indexes.Count; index++)
        //    {
        //        boards.Add(LookupTables.SquareValuesFromIndex[indexes[index]]);
        //    }

        //    return boards;
        //}

        //public static List<ulong> SplitBoard(ulong boardToSplit)
        //{
        //    List<ulong> boards = new List<ulong>();

        //    ulong initial = boardToSplit;
        //    ulong reducing = initial;

        //    reducing &= reducing - 1;

        //    while (initial != 0)
        //    {
        //        boards.Add(initial & ~reducing);

        //        initial &= initial - 1;
        //        reducing &= reducing - 1;
        //    }

        //    //List<byte> indexes = GetSquareIndexesFromBoardValue(boardToSplit);
        //    //List<ulong> boards = new List<ulong>();

        //    //for (int index = 0; index < indexes.Count; index++)
        //    //{
        //    //    boards.Add(LookupTables.SquareValuesFromIndex[indexes[index]]);
        //    //}

        //    return boards;
        //}

        public static ulong[] SplitBoardToArray(ulong boardToSplit)
        {
            byte size = GetPopCount(boardToSplit);
            ulong[] boards = new ulong[size];

            ulong initial = boardToSplit;
            ulong reducing = initial;

            reducing &= reducing - 1;

            int count = 0;

            while (initial != 0)
            {
                boards[count] = initial & ~reducing;

                initial &= initial - 1;
                reducing &= reducing - 1;

                count++;
            }

            //List<byte> indexes = GetSquareIndexesFromBoardValue(boardToSplit);
            //List<ulong> boards = new List<ulong>();

            //for (int index = 0; index < indexes.Count; index++)
            //{
            //    boards.Add(LookupTables.SquareValuesFromIndex[indexes[index]]);
            //}

            return boards;
        }

        /// <summary>
        /// Flips the bitboard vertically so that a1 becomes a8 etc
        /// from https://chessprogramming.wikispaces.com/Flipping+Mirroring+and+Rotating#FlipVertically
        /// </summary>
        /// <param name="beforeBoard"></param>
        /// <returns></returns>
        internal static ulong FlipVertical(ulong beforeBoard)
        {
            ulong afterBoard = beforeBoard;

            const ulong k1 = (0x00FF00FF00FF00FF);
            const ulong k2 = (0x0000FFFF0000FFFF);
            afterBoard = ((afterBoard >> 8) & k1) | ((afterBoard & k1) << 8);
            afterBoard = ((afterBoard >> 16) & k2) | ((afterBoard & k2) << 16);
            afterBoard = (afterBoard >> 32) | (afterBoard << 32);
            return afterBoard;

           // ulong afterBoard = ((beforeBoard << 56)) |
           //((beforeBoard << 40) & (0x00ff000000000000)) |
           //((beforeBoard << 24) & (0x0000ff0000000000)) |
           //((beforeBoard << 8) & (0x000000ff00000000)) |
           //((beforeBoard >> 8) & (0x00000000ff000000)) |
           //((beforeBoard >> 24) & (0x0000000000ff0000)) |
           //((beforeBoard >> 40) & (0x000000000000ff00)) |
           //((beforeBoard >> 56));

           // return afterBoard;
        }
    }
}
