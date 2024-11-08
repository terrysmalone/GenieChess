﻿using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using Logging;

namespace ChessEngine.NotationHelpers;

public static class TranslationHelper
{
    private static readonly ILog Log;

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

    public static string GetSquareNotation(ulong bitBoard)
    {
        var index = BitboardOperations.GetSquareIndexFromBoardValue(bitBoard);

        var row = index / 8;
        var col = (index % 8)+1;

        var square = ColumnToString(col) + RowToString(row);

        return square;
    }

    public static (int, int) GetPosition(ulong bitBoard)
    {
        var index = BitboardOperations.GetSquareIndexFromBoardValue(bitBoard);

        var row = index / 8;
        var col = (index % 8);

        return (col, row);
    }

    public static (string, int) GetColumnAndRow(ulong bitBoard)
    {
        var index = BitboardOperations.GetSquareIndexFromBoardValue(bitBoard);

        var row = index / 8 + 1;
        var col = (index % 8);

        var column = col switch
        {
            0 => "a",
            1 => "b",
            2 => "c",
            3 => "d",
            4 => "e",
            5 => "f",
            6 => "g",
            7 => "h",
            _ => string.Empty
        };

        return (column, row);
    }

    // Returns the column 0-7 as a letter a-h
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
            var file = TextToNumber(position[..1].ToUpperInvariant()) - 1;
            var rank = Convert.ToInt32(position.Substring(1, 1)) - 1;


            bitboard = LookupTables.SquareValuesFromPosition[file, rank];
        }
        catch(Exception exc)
        {
            Log.Error($"Error converting string {position} to bitboard.", exc);
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

