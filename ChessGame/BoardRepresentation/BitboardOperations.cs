using ChessGame.BoardSearching;
using System;
using System.Collections.Generic;

namespace ChessGame.BoardRepresentation
{
    public static class BitboardOperations
    {
        private static readonly byte[] Index64 = {
                63,  0, 58,  1, 59, 47, 53,  2,
                60, 39, 48, 27, 54, 33, 42,  3,
                61, 51, 37, 40, 49, 18, 28, 20,
                55, 30, 34, 11, 43, 14, 22,  4,
                62, 57, 46, 52, 38, 26, 32, 41,
                50, 36, 17, 19, 29, 10, 13, 21,
                56, 45, 25, 31, 35, 16,  9, 12,
                44, 24, 15,  8, 23,  7,  6,  5  };

        private const ulong Debruijn64 = 0x07EDD5E59A4E28C2;

        public static byte GetSquareIndexFromBoardValue(ulong boardValue)
        {
            return Index64[((boardValue & (~boardValue + 1)) * Debruijn64) >> 58];
        }

        public static byte GetSquareIndexFromBoardValueOld(ulong boardValue)
        {
            LookupTables.InitialiseAllTables();

            if (boardValue == 0)
                return 0;

            for (byte i = 0; i < 64; i++)
            {
                if (LookupTables.SquareValuesFromIndex[i] == boardValue)
                    return i;
            }

            throw new Exception($"Bitboard value {boardValue} does not represent a value");
        }

        /// <summary>
        /// Returns the indexes of the bitboard value as a byte array
        /// </summary>
        /// <param name="boardValue"></param>
        /// <returns></returns>
        public static List<byte> GetSquareIndexesFromBoardValue(ulong boardValue)
        {
            var values = new List<byte>();
            
            while (boardValue != 0)
            {
                var first = GetSquareIndexFromBoardValue(boardValue);

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

        //public static byte GetPopCountOld(ulong boardValue)
        //{
        //    LookupTables.InitialiseAllTables();

        //    byte popCount = 0;

        //    var tempBoard = boardValue;

        //    for (var i = 63; i >= 0; i--)
        //    {
        //        var currentSquareValue = LookupTables.SquareValuesFromIndex[i];

        //        if (tempBoard >= currentSquareValue)
        //        {
        //            popCount++;
        //            tempBoard -= currentSquareValue;
        //        }
        //    }

        //    return popCount;
        //}

        public static ulong[] SplitBoardToArray(ulong boardToSplit)
        {
            var size = GetPopCount(boardToSplit);
            var boards = new ulong[size];

            var initial = boardToSplit;
            var reducing = initial;

            reducing &= reducing - 1;

            var count = 0;

            while (initial != 0)
            {
                boards[count] = initial & ~reducing;

                initial &= initial - 1;

                reducing &= reducing - 1;

                count++;
            }

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
            var afterBoard = beforeBoard;

            const ulong k1 = (0x00FF00FF00FF00FF);

            const ulong k2 = (0x0000FFFF0000FFFF);

            afterBoard = ((afterBoard >> 8) & k1) | ((afterBoard & k1) << 8);

            afterBoard = ((afterBoard >> 16) & k2) | ((afterBoard & k2) << 16);

            afterBoard = (afterBoard >> 32) | (afterBoard << 32);
            
            return afterBoard;
        }
    }
}
