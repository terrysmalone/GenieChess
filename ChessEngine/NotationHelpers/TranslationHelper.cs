﻿using System;
using System.Linq;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using log4net;

namespace ChessEngine.NotationHelpers
{
    internal static class TranslationHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static string GetPieceLetter(PieceType piece, ulong moveFromBoard)
        {
            switch (piece)
            {
                case PieceType.None:
                    return string.Empty;
                case PieceType.Pawn:
                    var index = BitboardOperations.GetSquareIndexFromBoardValue(moveFromBoard);

                    var col = index % 8 + 1;

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

        internal static string GetSquareNotation(ulong bitBoard)
        {
            var index = BitboardOperations.GetSquareIndexFromBoardValue(bitBoard);

            var row = index / 8;
            var col = (index % 8)+1;

            var square = ColumnToString(col) + RowToString(row);

            return square;
        }

        // Returns the colum 0-7 as a letter a-h
        private static string ColumnToString(int number)
        {
            var c = (Char)(97 + (number - 1));
            return c.ToString();
        }

        private static string RowToString(int number)
        {
            return  (number + 1).ToString();             
        }

        internal static ulong GetBitboard(string position)
        {
            ulong bitboard = 0;
            try
            {
                var file = TextToNumber(position.Substring(0, 1).ToUpperInvariant()) - 1;
                var rank = Convert.ToInt32(position.Substring(1, 1)) - 1;
                

                bitboard = LookupTables.SquareValuesFromPosition[file, rank];
            }
            catch(Exception exc)
            {
                Log.Error(string.Format("Error converting string {0} to bitboard.", position), exc);
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
