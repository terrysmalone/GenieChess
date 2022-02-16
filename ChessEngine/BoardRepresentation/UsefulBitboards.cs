namespace ChessEngine.BoardRepresentation
{
    public struct UsefulBitboards
    {
        public ulong AllWhiteOccupiedSquares;
        public ulong AllBlackOccupiedSquares;
        
        public ulong WhiteNonEndGamePieces;
        public ulong BlackNonEndGamePieces;

        public ulong AllOccupiedSquares;

        public ulong EmptySquares;

        public ulong WhiteOrEmpty;
        public ulong BlackOrEmpty;
    }
}