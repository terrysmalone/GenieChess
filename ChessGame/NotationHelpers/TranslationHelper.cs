using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardSearching;
using log4net;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;

namespace ChessGame.NotationHelpers
{
    internal static class TranslationHelper
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="moveFromBoard">The square of the piece. This is ignored (and can be any value) unless the piece is a pawn</param>
        /// <returns></returns>
        internal static string GetPieceLetter(PieceType piece, ulong moveFromBoard)
        {
            switch (piece)
            {
                case PieceType.None:
                    return string.Empty;
                case PieceType.Pawn:
                    byte index = BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard);

                    int col = index % 8 + 1;

                    return ColumnToString(col);
                case PieceType.Knight:
                    return "N";
                case PieceType.Bishop:
                    return "B";
                case PieceType.Rook:
                    return "R";
                case PieceType.Queen:
                    return "Q";
                case PieceType.King:
                    return "K";
                default:
                    return string.Empty;
            }
        }

        internal static string SquareBitboardToSquareString(ulong bitBoard)
        {
            byte index = BitboardOperations.GetSquareIndexFromBoardValue(bitBoard);

            int row = index / 8;
            int col = (index % 8)+1;

            string square = ColumnToString(col) + RowToString(row);

            return square;
        }

        /// <summary>
        /// Returns the colum 0-7 as a letter a-h
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string ColumnToString(int number)
        {
            Char c = (Char)(97 + (number - 1));
            return c.ToString();
        }

        private static string RowToString(int number)
        {
            return  (number + 1).ToString();             
        } 

        //internal static PieceType GetPieceType(char letter)
        //{
        //    switch (letter)
        //    {
               
        //    }
        //}

        internal static ulong BitboardFromSquareString(string position)
        {
            ulong bitboard = 0;
            try
            {
                int file = TextToNumber(position.Substring(0, 1).ToUpperInvariant()) - 1;
                int rank = Convert.ToInt32(position.Substring(1, 1)) - 1;
                

                bitboard = LookupTables.SquareValuesFromPosition[file, rank];
            }
            catch(Exception exc)
            {
                log.Error(string.Format("Error converting string {0} to bitboard.", position), exc);
            }

            return bitboard;
        }

        private static int TextToNumber(string text)
        {
            return text
                .Select(c => c - 'A' + 1)
                .Aggregate((sum, next) => sum * 26 + next);
        }
    }
}
