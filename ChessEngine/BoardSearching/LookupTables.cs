using System;
namespace ChessEngine.BoardSearching;

// Initialises and generates lookup tables for speeding up bitboard translations

[Serializable]
public static class LookupTables
{
    private static bool _initialised;

    public static ulong[] SquareValuesFromIndex = new ulong[64];
    public static ulong[,] SquareValuesFromPosition = new ulong[8, 8];

    public static ulong A1;
    public static ulong B1;
    public static ulong C1;
    public static ulong D1;
    public static ulong E1;
    public static ulong F1;
    public static ulong G1;
    public static ulong H1;

    public static ulong A2;
    public static ulong B2;
    public static ulong C2;
    public static ulong D2;
    public static ulong E2;
    public static ulong F2;
    public static ulong G2;
    public static ulong H2;

    public static ulong A3;
    public static ulong B3;
    public static ulong C3;
    public static ulong D3;
    public static ulong E3;
    public static ulong F3;
    public static ulong G3;
    public static ulong H3;

    public static ulong A4;
    public static ulong B4;
    public static ulong C4;
    public static ulong D4;
    public static ulong E4;
    public static ulong F4;
    public static ulong G4;
    public static ulong H4;

    public static ulong A5;
    public static ulong B5;
    public static ulong C5;
    public static ulong D5;
    public static ulong E5;
    public static ulong F5;
    public static ulong G5;
    public static ulong H5;

    public static ulong A6;
    public static ulong B6;
    public static ulong C6;
    public static ulong D6;
    public static ulong E6;
    public static ulong F6;
    public static ulong G6;
    public static ulong H6;

    public static ulong A7;
    public static ulong B7;
    public static ulong C7;
    public static ulong D7;
    public static ulong E7;
    public static ulong F7;
    public static ulong G7;
    public static ulong H7;

    public static ulong A8;
    public static ulong B8;
    public static ulong C8;
    public static ulong D8;
    public static ulong E8;
    public static ulong F8;
    public static ulong G8;
    public static ulong H8;

    public static ulong[] UpBoard = new ulong[64];
    public static ulong[] RightBoard = new ulong[64];
    public static ulong[] DownBoard = new ulong[64];
    public static ulong[] LeftBoard = new ulong[64];

    public static ulong[] UpRightBoard = new ulong[64];
    public static ulong[] DownRightBoard = new ulong[64];
    public static ulong[] DownLeftBoard = new ulong[64];
    public static ulong[] UpLeftBoard = new ulong[64];

    public static ulong RowMask1;
    public static ulong RowMask2;
    public static ulong RowMask3;
    public static ulong RowMask4;
    public static ulong RowMask5;
    public static ulong RowMask6;
    public static ulong RowMask7;
    public static ulong RowMask8;

    public static ulong ColumnMaskA;
    public static ulong ColumnMaskB;
    public static ulong ColumnMaskC;
    public static ulong ColumnMaskD;
    public static ulong ColumnMaskE;
    public static ulong ColumnMaskF;
    public static ulong ColumnMaskG;
    public static ulong ColumnMaskH;

    public static ulong[] ColumnMaskByColumn = new ulong[8];
    public static ulong[] ColumnMaskByIndex = new ulong[64];

    public static ulong WhiteCastlingQueensideObstructionPath;
    public static ulong WhiteCastlingQueensideAttackPath;
    public static ulong WhiteCastlingKingsideObstructionPath;
    public static ulong WhiteCastlingKingsideAttackPath;

    public static ulong BlackCastlingQueensideObstructionPath;
    public static ulong BlackCastlingQueensideAttackPath;
    public static ulong BlackCastlingKingsideObstructionPath;
    public static ulong BlackCastlingKingsideAttackPath;

    public static ulong[] WhitePawnFrontSpan = new ulong[64];
    public static ulong[] BlackPawnFrontSpan = new ulong[64];

    public static void InitialiseAllTables()
    {
        if (_initialised)
            return;

        GenerateSquareValueTable();

        CalculateDirectionBoards();

        CalculateRowsAndColumnMasks();

        CalculatePawnFrontSpans();

        InitialiseSquareValues();

        InitialiseCastlingPathBoards();

        _initialised = true;
    }

    private static void InitialiseSquareValues()
    {
        A1 = 1;
        B1 = 2 * A1;
        C1 = 2 * B1;
        D1 = 2 * C1;
        E1 = 2 * D1;
        F1 = 2 * E1;
        G1 = 2 * F1;
        H1 = 2 * G1;

        A2 = 2 * H1;
        B2 = 2 * A2;
        C2 = 2 * B2;
        D2 = 2 * C2;
        E2 = 2 * D2;
        F2 = 2 * E2;
        G2 = 2 * F2;
        H2 = 2 * G2;

        A3 = 2 * H2;
        B3 = 2 * A3;
        C3 = 2 * B3;
        D3 = 2 * C3;
        E3 = 2 * D3;
        F3 = 2 * E3;
        G3 = 2 * F3;
        H3 = 2 * G3;

        A4 = 2 * H3;
        B4 = 2 * A4;
        C4 = 2 * B4;
        D4 = 2 * C4;
        E4 = 2 * D4;
        F4 = 2 * E4;
        G4 = 2 * F4;
        H4 = 2 * G4;

        A5 = 2 * H4;
        B5 = 2 * A5;
        C5 = 2 * B5;
        D5 = 2 * C5;
        E5 = 2 * D5;
        F5 = 2 * E5;
        G5 = 2 * F5;
        H5 = 2 * G5;

        A6 = 2 * H5;
        B6 = 2 * A6;
        C6 = 2 * B6;
        D6 = 2 * C6;
        E6 = 2 * D6;
        F6 = 2 * E6;
        G6 = 2 * F6;
        H6 = 2 * G6;

        A7 = 2 * H6;
        B7 = 2 * A7;
        C7 = 2 * B7;
        D7 = 2 * C7;
        E7 = 2 * D7;
        F7 = 2 * E7;
        G7 = 2 * F7;
        H7 = 2 * G7;

        A8 = 2 * H7;
        B8 = 2 * A8;
        C8 = 2 * B8;
        D8 = 2 * C8;
        E8 = 2 * D8;
        F8 = 2 * E8;
        G8 = 2 * F8;
        H8 = 2 * G8;
    }

    /// <summary>
    /// Generates a table which gives the bitboard value of all 64 squares
    /// </summary>
    private static void GenerateSquareValueTable()
    {
        GenerateSquareValuesFromFromIndex();
        GenerateSquareValuesFromFromPosition();
    }

    private static void GenerateSquareValuesFromFromIndex()
    {
        SquareValuesFromIndex[0] = 1;

        for (var i = 1; i < SquareValuesFromIndex.Length; i++)
        {
            //Left shift gives the same result as multiplying by two but is faster
            SquareValuesFromIndex[i] = SquareValuesFromIndex[i - 1] << 1;
        }
    }

    private static void GenerateSquareValuesFromFromPosition()
    {
        ulong squareValue = 1;

        for (byte row = 0; row < 8; row++)
        {
            for (byte column = 0; column < 8; column++)
            {
                SquareValuesFromPosition[column, row] = squareValue;
                squareValue <<= 1;
            }
        }
    }

    private static void CalculateDirectionBoards()
    {
        CalculateUpDirectionBoards();
        CalculateRightDirectionBoards();
        CalculateDownDirectionBoards();
        CalculateLeftDirectionBoards();

        CalculateUpRightDirectionBoards();
        CalculateDownRightDirectionBoards();
        CalculateDownLeftDirectionBoards();
        CalculateUpLeftDirectionBoards();
    }

    private static void CalculateUpDirectionBoards()
    {
        for (var startRank = 0; startRank < 7; startRank++)        //Only goes to 7 as row 8 will all be empty since there are no up moves
        {
            for (var startColumn = 0; startColumn < 8; startColumn++)
            {
                var row = startRank + 1;

                ulong upSquares = 0;

                while(row < 8)
                {
                    upSquares |= SquareValuesFromPosition[startColumn, row];

                    row++;
                }

                UpBoard[(startRank * 8) + startColumn] = upSquares;
            }
        }
    }

    private static void CalculateRightDirectionBoards()
    {
        for (var startRank = 0; startRank < 8; startRank++)
        {
            for (var startColumn = 0; startColumn < 7; startColumn++)    //Only goes to 7 as column 8 will all be empty since there are no right moves
            {
                var column = startColumn + 1;

                ulong rightSquares = 0;

                while (column < 8)
                {
                    rightSquares |= SquareValuesFromPosition[column, startRank];

                    column++;
                }

                RightBoard[(startRank*8)+startColumn] = rightSquares;
            }
        }
    }

    private static void CalculateDownDirectionBoards()
    {
        for (var startRank = 1; startRank < 8; startRank++)        //Starts at 1 as row 0 will all be empty since there are no up moves
        {
            for (var startColumn = 0; startColumn < 8; startColumn++)
            {
                var row = startRank - 1;

                ulong downSquares = 0;

                while (row >= 0)
                {
                    downSquares |= SquareValuesFromPosition[startColumn, row];

                    row--;
                }

                DownBoard[(startRank * 8) + startColumn] = downSquares;
            }
        }
    }

    private static void CalculateLeftDirectionBoards()
    {
        for (var startRank = 0; startRank < 8; startRank++)
        {
            for (var startColumn = 1; startColumn < 8; startColumn++)    //Starts at 1 as column 0 will all be empty since there are no left moves
            {
                var column = startColumn - 1;

                ulong leftSquares = 0;

                while (column >= 0)
                {
                    leftSquares |= SquareValuesFromPosition[column, startRank];

                    column--;
                }

                LeftBoard[(startRank * 8) + startColumn] = leftSquares;
            }
        }
    }

    private static void CalculateUpRightDirectionBoards()
    {
        for (var startRank = 0; startRank < 7; startRank++)
        {
            for (var startColumn = 0; startColumn < 7; startColumn++)    //Stops at 6 as column and row 7 will all be empty since there are no up right moves
            {
                var column = startColumn + 1;
                var row = startRank + 1;

                ulong upRightSquares = 0;

                while (column < 8 && row < 8)
                {
                    upRightSquares |= SquareValuesFromPosition[column, row];

                    column++;
                    row++;
                }

                UpRightBoard[(startRank * 8) + startColumn] = upRightSquares;
            }
        }
    }

    private static void CalculateDownRightDirectionBoards()
    {
        for (var startRank = 1; startRank < 8; startRank++)     //Starts at 1 as row 0 will all be empty since there are no down right moves
        {
            for (var startColumn = 0; startColumn < 7; startColumn++)    //Stops at 6 as column 7 will all be empty since there are no down right moves
            {
                var column = startColumn + 1;
                var row = startRank - 1;

                ulong downRightSquares = 0;

                while (column < 8 && row >= 0)
                {
                    downRightSquares |= SquareValuesFromPosition[column, row];

                    column++;
                    row--;
                }

                DownRightBoard[(startRank * 8) + startColumn] = downRightSquares;
            }
        }
    }

    private static void CalculateDownLeftDirectionBoards()
    {
        for (var startRank = 1; startRank < 8; startRank++)     //Starts at 1 as row 0 will all be empty since there are no down left moves
        {
            for (var startColumn = 1; startColumn < 8; startColumn++)    //Starts at 1 as column 0 will all be empty since there are no down left moves
            {
                var column = startColumn - 1;
                var row = startRank - 1;

                ulong downLeftSquares = 0;

                while (column >= 0 && row >= 0)
                {
                    downLeftSquares |= SquareValuesFromPosition[column, row];

                    column--;
                    row--;
                }

                DownLeftBoard[(startRank * 8) + startColumn] = downLeftSquares;
            }
        }
    }

    private static void CalculateUpLeftDirectionBoards()
    {
        for (var startRank = 0; startRank < 7; startRank++)
        {
            for (var startColumn = 1; startColumn < 8; startColumn++)
            {
                var column = startColumn - 1;
                var row = startRank + 1;

                ulong upLeftSquares = 0;

                while (column >= 0 && row < 8)
                {
                    upLeftSquares |= SquareValuesFromPosition[column, row];

                    column--;
                    row++;
                }

                UpLeftBoard[(startRank * 8) + startColumn] = upLeftSquares;
            }
        }
    }

    /// <summary>
    /// Calculates masks where an entire row or column is set to 1
    /// </summary>
    private static void CalculateRowsAndColumnMasks()
    {
        CalculateRowMasks();
        CalculateColumnMasks();

        CalculateColumnMaskByColumn();
        CalculateColumnMaskByIndex();
    }

    private static void CalculateColumnMasks()
    {
        ColumnMaskA = CalculateHorizontalMask(0);
        ColumnMaskB = CalculateHorizontalMask(1);
        ColumnMaskC = CalculateHorizontalMask(2);
        ColumnMaskD = CalculateHorizontalMask(3);
        ColumnMaskE = CalculateHorizontalMask(4);
        ColumnMaskF = CalculateHorizontalMask(5);
        ColumnMaskG = CalculateHorizontalMask(6);
        ColumnMaskH = CalculateHorizontalMask(7);

    }

    private static ulong CalculateHorizontalMask(int row)
    {
        ulong mask = 0;

        for (var column = 0; column < 8; column++)
        {
            mask |= SquareValuesFromPosition[row, column];
        }

        return mask;
    }

    private static void CalculateRowMasks()
    {
        RowMask1 = CalculateVerticalMask(0);
        RowMask2 = CalculateVerticalMask(1);
        RowMask3 = CalculateVerticalMask(2);
        RowMask4 = CalculateVerticalMask(3);
        RowMask5 = CalculateVerticalMask(4);
        RowMask6 = CalculateVerticalMask(5);
        RowMask7 = CalculateVerticalMask(6);
        RowMask8 = CalculateVerticalMask(7);
    }

    private static ulong CalculateVerticalMask(int column)
    {
        ulong mask = 0;

        for (var row = 0; row < 8; row++)
        {
            mask |= SquareValuesFromPosition[row, column];
        }

        return mask;
    }

    private static void CalculateColumnMaskByColumn()
    {
        ColumnMaskByColumn[0] = ColumnMaskA;
        ColumnMaskByColumn[1] = ColumnMaskB;
        ColumnMaskByColumn[2] = ColumnMaskC;
        ColumnMaskByColumn[3] = ColumnMaskD;
        ColumnMaskByColumn[4] = ColumnMaskE;
        ColumnMaskByColumn[5] = ColumnMaskF;
        ColumnMaskByColumn[6] = ColumnMaskG;
        ColumnMaskByColumn[7] = ColumnMaskH;
    }

    private static void CalculateColumnMaskByIndex()
    {
        for (var index = 0; index < 64; index++)
        {
            var column = index % 8;

            switch (column)
            {
                case 0:
                    ColumnMaskByIndex[index] = ColumnMaskA;
                    break;
                case 1:
                    ColumnMaskByIndex[index] = ColumnMaskB;
                    break;
                case 2:
                    ColumnMaskByIndex[index] = ColumnMaskC;
                    break;
                case 3:
                    ColumnMaskByIndex[index] = ColumnMaskD;
                    break;
                case 4:
                    ColumnMaskByIndex[index] = ColumnMaskE;
                    break;
                case 5:
                    ColumnMaskByIndex[index] = ColumnMaskF;
                    break;
                case 6:
                    ColumnMaskByIndex[index] = ColumnMaskG;
                    break;
                case 7:
                    ColumnMaskByIndex[index] = ColumnMaskH;
                    break;
                default:
                    throw new Exception("Bad column given: " + column);
            }

        }
    }

    private static void CalculatePawnFrontSpans()
    {
        var   notA = 18374403900871474942;
        ulong notH = 9187201950435737471;

        ulong bitboardValue = 1;

        // We calculate it from 0 to 63 even though we will never need the 1st and last row
        // This is just for ease of calculation and understanding
        for (var i = 0; i < 64; i++)
        {
            // White
            var whiteFrontSpan = (bitboardValue << 9 & notA) | bitboardValue << 8 | (bitboardValue << 7 & notH);

            whiteFrontSpan = whiteFrontSpan
                           | (whiteFrontSpan << 8)
                           | (whiteFrontSpan << 16)
                           | (whiteFrontSpan << 24)
                           | (whiteFrontSpan << 32);

            WhitePawnFrontSpan[i] = whiteFrontSpan;

            // Black
            var blackFrontSpan = (bitboardValue >> 9 & notA) | bitboardValue >> 8 | (bitboardValue >> 7 & notH);

            blackFrontSpan = blackFrontSpan
                           | (blackFrontSpan >> 8)
                           | (blackFrontSpan >> 16)
                           | (blackFrontSpan >> 24)
                           | (blackFrontSpan >> 32);

            BlackPawnFrontSpan[i] = blackFrontSpan;

            bitboardValue *= 2;
        }
    }

    private static void InitialiseCastlingPathBoards()
    {
        WhiteCastlingQueensideObstructionPath = B1 | C1 | D1;
        WhiteCastlingQueensideAttackPath = D1;
        WhiteCastlingKingsideObstructionPath = F1 | G1;
        WhiteCastlingKingsideAttackPath = F1;

        BlackCastlingQueensideObstructionPath = B8 | C8 | D8;
        BlackCastlingQueensideAttackPath = D8;
        BlackCastlingKingsideObstructionPath = F8 | G8;
        BlackCastlingKingsideAttackPath = F8;
    }
}
