using System;
using ChessEngine.BoardRepresentation;

namespace ChessEngine.BoardSearching
{
    /// <summary>
    /// Performs all checks on static board positions required for Score calculations.
    /// </summary>
    public static class StaticBoardChecks
    {
        public static ulong WhiteRangeBoard { get; private set; } // All non-white squares withing range of white pieces        

        public static ulong BlackRangeBoard { get; private set; } // All non-black squares withing range of black pieces      

        public static ulong WhiteAttackBoard{ get; private set; } // All black pieces within range of white pieces

        public static ulong BlackAttackBoard{ get; private set; } // All white pieces within range of black pieces

        public static void Calculate(UsefulBitboards usefulBitboards)
        {
            CalculateRangedBoard(usefulBitboards);
            CalculateAttackBoard(usefulBitboards);
        }


        private static void CalculateRangedBoard(UsefulBitboards usefulBitboards)
        {
            CalculateWhiteRangedBoard(usefulBitboards);
            CalculateBlackRangedBoard(usefulBitboards);
        }

        private static void CalculateWhiteRangedBoard(UsefulBitboards usefulBitboards)
        {
            WhiteRangeBoard = 0;

            WhiteRangeBoard |= usefulBitboards.AllWhiteOccupiedSquares;

            throw new NotImplementedException();
        }

        private static void CalculateBlackRangedBoard(UsefulBitboards usefulBitboards)
        {
            throw new NotImplementedException();
        }

        private static void CalculateAttackBoard(UsefulBitboards usefulBitboards)
        {
            CalculateWhiteAttackBoard(usefulBitboards);
            CalculateBlackAttackBoard(usefulBitboards);
        }

        private static void CalculateWhiteAttackBoard(UsefulBitboards usefulBitboards)
        {
            throw new NotImplementedException();
        }

        private static void CalculateBlackAttackBoard(UsefulBitboards usefulBitboards)
        {
            throw new NotImplementedException();
        }
    }
}
