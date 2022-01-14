namespace ChessEngine.PossibleMoves
{
    // We only need some pieces here since we use ray attacks for sliding piece generation.
    // This will be replaced with rotated bitboards eventually
    public struct ValidMoveArrays
    {
        public static ulong[] WhitePawnMoves;
        public static ulong[] WhitePawnCaptures;
        public static ulong[] BlackPawnMoves;
        public static ulong[] BlackPawnCaptures;
        
        public static ulong[] KnightMoves;
        public static ulong[] KingMoves;
    }
}
