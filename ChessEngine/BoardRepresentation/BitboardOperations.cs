using System.Collections.Generic;

namespace ChessEngine.BoardRepresentation
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
        
        // Returns the indexes of the bitboard value as a byte array
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

        

        public static ulong[] SplitBoardToArray(ulong boardToSplit)
        {
            var size = GetPopCount(boardToSplit);
            var boards = new ulong[size];

            var reducing = boardToSplit;

            reducing &= reducing - 1;
            
            for (var i = 0; i < size; i++)
            {
                boards[i] = boardToSplit & ~reducing;

                boardToSplit &= boardToSplit - 1;

                reducing &= reducing - 1;
            }

            return boards;
        }

        // Flips the bitboard vertically so that a1 becomes a8 etc
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
