namespace ChessEngine.Debugging;

public static class CountDebugger
{
    public static ulong Transposition_HashAdded;
    public static ulong Transposition_HashReplaced;
    public static ulong Transposition_Searches;
    public static ulong Transposition_CollisionCount;
    public static ulong Transposition_MatchCount;
    public static ulong Transposition_HashFound;
    public static ulong Transposition_MatchAndUsed;

    public static ulong NullMovesPruned = 0;

    public static ulong Evaluations = 0;
    public static ulong Nodes = 0;

    public static void ClearAll()
    {
        ClearTranspositionValues();
        ClearNodesAndEvaluations();
    }

    public static void ClearTranspositionValues()
    {
        Transposition_HashAdded = 0;
        Transposition_HashReplaced = 0;
        Transposition_CollisionCount = 0;
        Transposition_MatchCount = 0;
        Transposition_MatchAndUsed = 0;
        Transposition_Searches = 0;
        Transposition_HashFound = 0;
    }

    public static void ClearNodesAndEvaluations()
    {
        NullMovesPruned = 0;

        Evaluations = 0;
        Nodes = 0;
    }
}

