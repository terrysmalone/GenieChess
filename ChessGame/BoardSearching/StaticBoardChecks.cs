using ChessGame.BoardRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.BoardSearching
{
    /// <summary>
    /// Performs all checks on static board positions required for Score calculations.
    /// </summary>
    public static class StaticBoardChecks
    {
        private static ulong whiteRangeBoard;     //All non-white squares withing range of white pieces        
        private static ulong blackRangeBoard;     //All non-black squares withing range of black pieces
        private static ulong whiteAttackBoard;    //All black pieces within range of white pieces
        private static ulong blackAttackBoard;    //All white pieces within range of black pieces

        public static ulong WhiteRangeBoard
        {
            get { return StaticBoardChecks.whiteRangeBoard; }
        }

        public static ulong BlackRangeBoard
        {
            get { return StaticBoardChecks.blackRangeBoard; }
        }

        public static ulong WhiteAttackBoard
        {
            get { return StaticBoardChecks.whiteAttackBoard; }
        }

        public static ulong BlackAttackBoard
        {
            get { return StaticBoardChecks.blackAttackBoard; }
        }

        public static void Calculate(Board board)
        {
            CalculateRangedBoard(board);
            CalculateAttackBoard(board);
        }


        private static void CalculateRangedBoard(Board board)
        {
            CalculateWhiteRangedBoard(board);
            CalculateBlackRangedBoard(board);
        }

        private static void CalculateWhiteRangedBoard(Board board)
        {
            whiteRangeBoard = 0;

            whiteRangeBoard = whiteRangeBoard  | board.AllWhiteOccupiedSquares;

            throw new NotImplementedException();
        }

        private static void CalculateBlackRangedBoard(Board board)
        {
            throw new NotImplementedException();
        }

        private static void CalculateAttackBoard(Board board)
        {
            CalculateWhiteAttackBoard(board);
            CalculateBlackAttackBoard(board);
        }

        private static void CalculateWhiteAttackBoard(Board board)
        {
            throw new NotImplementedException();
        }

        private static void CalculateBlackAttackBoard(Board board)
        {
            throw new NotImplementedException();
        }


    }
}
