namespace ChessEngine.MoveSearching
{
    internal struct ZobristKey
    {
        internal static ulong[,] PiecePositions; //[piece, position]

        internal static ulong BlackToMove;

        internal static ulong WhiteCastleKingside;
        internal static ulong WhiteCastleQueenside;
        internal static ulong BlackCastleKingside;
        internal static ulong BlackCastleQueenside;

        internal static ulong EnPassantA;
        internal static ulong EnPassantB;
        internal static ulong EnPassantC;
        internal static ulong EnPassantD;
        internal static ulong EnPassantE;
        internal static ulong EnPassantF;
        internal static ulong EnPassantG;
        internal static ulong EnPassantH;
    }
}
