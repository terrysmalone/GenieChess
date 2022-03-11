using ChessEngine.BoardRepresentation;

namespace ChessEngine.BoardSearching;

/// <summary>
/// Performs all checks on static board positions required for Score calculations.
/// </summary>
public static class StaticBoardChecks
{
    public static ulong WhiteRangeBoard { get; private set; } // All non-white squares withing range of white pieces

    public static ulong BlackRangeBoard { get; private set; } // All non-black squares withing range of black pieces

    public static ulong WhiteAttackBoard{ get; private set; } // All black pieces within range of white pieces

    public static ulong BlackAttackBoard{ get; private set; } // All white pieces within range of black pieces

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
        WhiteRangeBoard = 0;

        WhiteRangeBoard |= board.AllWhiteOccupiedSquares;

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

