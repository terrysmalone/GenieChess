using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.Extensions;
namespace ChessEngine.MoveSearching;

/// <summary>
/// Performs a Zobrist hash on a board position
/// </summary>
internal static class ZobristHash
{
    #region constants

    internal const int WHITE_PAWN = 0;
    internal const int WHITE_KNIGHT = 1;
    internal const int WHITE_BISHOP = 2;
    internal const int WHITE_ROOK = 3;
    internal const int WHITE_QUEEN = 4;
    internal const int WHITE_KING = 5;
    internal const int BLACK_PAWN = 6;
    internal const int BLACK_KNIGHT = 7;
    internal const int BLACK_BISHOP = 8;
    internal const int BLACK_ROOK = 9;
    internal const int BLACK_QUEEN = 10;
    internal const int BLACK_KING = 11;

    #endregion constants

    static bool initialised;

    /// <summary>
    /// Clears and reinitialises the zobrist hashing
    /// </summary>
    internal static void Restart()
    {
        initialised = false;
        Initialise();
    }

    internal static void Initialise()
    {
        var rand = new Random(1);

        if (initialised == false)
        {
            ZobristKey.PiecePositions = new ulong[12,64];

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    ZobristKey.PiecePositions[i, j] = rand.NextUlong();
                }
            }

            ZobristKey.BlackToMove = rand.NextUlong();

            ZobristKey.WhiteCastleKingside = rand.NextUlong();
            ZobristKey.WhiteCastleQueenside = rand.NextUlong();
            ZobristKey.BlackCastleKingside = rand.NextUlong();
            ZobristKey.BlackCastleQueenside = rand.NextUlong();

            ZobristKey.EnPassantA = rand.NextUlong();
            ZobristKey.EnPassantB = rand.NextUlong();
            ZobristKey.EnPassantC = rand.NextUlong();
            ZobristKey.EnPassantD = rand.NextUlong();
            ZobristKey.EnPassantE = rand.NextUlong();
            ZobristKey.EnPassantF = rand.NextUlong();
            ZobristKey.EnPassantG = rand.NextUlong();
            ZobristKey.EnPassantH = rand.NextUlong();

            initialised = true;
        }
    }

    #region hash whole board methods

    internal static ulong HashBoard(BoardState boardState)
    {
        ulong hash = 0;

        hash ^= HashWhitePawns(boardState);
        hash ^= HashWhiteKnights(boardState);
        hash ^= HashWhiteBishops(boardState);
        hash ^= HashWhiteRooks(boardState);
        hash ^= HashWhiteQueens(boardState);
        hash ^= HashWhiteKing(boardState);

        hash ^= HashBlackPawns(boardState);
        hash ^= HashBlackKnights(boardState);
        hash ^= HashBlackBishops(boardState);
        hash ^= HashBlackRooks(boardState);
        hash ^= HashBlackQueens(boardState);
        hash ^= HashBlackKing(boardState);

        hash ^= HashEnPassantColumn(boardState.EnPassantPosition);

        hash ^= HashCastlingRights(boardState);

        if (!boardState.WhiteToMove)
            hash ^= ZobristKey.BlackToMove;

        return hash;
    }

    #region Hash piece positions

    private static ulong HashWhitePawns(BoardState boardState)
    {
        ulong hash = 0;

        var whitePawns = boardState.WhitePawns;

        hash ^= HashBitboard(whitePawns, WHITE_PAWN);

        return hash;
    }

    private static ulong HashWhiteKnights(BoardState boardState)
    {
        ulong hash = 0;

        var whiteKnights = boardState.WhiteKnights;

        hash ^= HashBitboard(whiteKnights, WHITE_KNIGHT);

        return hash;
    }

    private static ulong HashWhiteBishops(BoardState boardState)
    {
        ulong hash = 0;

        var whiteBishops = boardState.WhiteBishops;

        hash ^= HashBitboard(whiteBishops, WHITE_BISHOP);

        return hash;
    }

    private static ulong HashWhiteRooks(BoardState boardState)
    {
        ulong hash = 0;

        var whiteRooks = boardState.WhiteRooks;

        hash ^= HashBitboard(whiteRooks, WHITE_ROOK);

        return hash;
    }

    private static ulong HashWhiteQueens(BoardState boardState)
    {
        ulong hash = 0;

        var whiteQueens = boardState.WhiteQueen;

        hash ^= HashBitboard(whiteQueens, WHITE_QUEEN);

        return hash;
    }

    private static ulong HashWhiteKing(BoardState boardState)
    {
        ulong hash = 0;

        var whiteKing = boardState.WhiteKing;

        hash ^= HashBitboard(whiteKing, WHITE_KING);

        return hash;
    }

    private static ulong HashBlackPawns(BoardState boardState)
    {
        ulong hash = 0;

        var blackPawns = boardState.BlackPawns;

        hash ^= HashBitboard(blackPawns, BLACK_PAWN);

        return hash;
    }

    private static ulong HashBlackKnights(BoardState boardState)
    {
        ulong hash = 0;

        var blackKnights = boardState.BlackKnights;

        hash ^= HashBitboard(blackKnights, BLACK_KNIGHT);

        return hash;
    }

    private static ulong HashBlackBishops(BoardState boardState)
    {
        ulong hash = 0;

        var blackBishops = boardState.BlackBishops;

        hash ^= HashBitboard(blackBishops, BLACK_BISHOP);

        return hash;
    }

    private static ulong HashBlackRooks(BoardState boardState)
    {
        ulong hash = 0;

        var blackRooks = boardState.BlackRooks;

        hash ^= HashBitboard(blackRooks, BLACK_ROOK);

        return hash;
    }

    private static ulong HashBlackQueens(BoardState boardState)
    {
        ulong hash = 0;

        var blackQueens = boardState.BlackQueen;

        hash ^= HashBitboard(blackQueens, BLACK_QUEEN);

        return hash;
    }

    private static ulong HashBlackKing(BoardState boardState)
    {
        ulong hash = 0;

        var blackKing = boardState.BlackKing;

        hash ^= HashBitboard(blackKing, BLACK_KING);

        return hash;
    }

    private static ulong HashBitboard(ulong board, int piece)
    {
        ulong hash = 0;

        var indexes = BitboardOperations.GetSquareIndexesFromBoardValue(board);

        foreach (var index in indexes)
        {
            hash ^= ZobristKey.PiecePositions[piece, index];
        }

        return hash;
    }

    #endregion Hash piece positions

    internal static ulong HashEnPassantColumn(ulong enPassantPosition)
    {

        if (enPassantPosition > 0)
        {
            if ((enPassantPosition & LookupTables.ColumnMaskA) > 0)
                return ZobristKey.EnPassantA;
            if ((enPassantPosition & LookupTables.ColumnMaskB) > 0)
                return ZobristKey.EnPassantB;
            if ((enPassantPosition & LookupTables.ColumnMaskC) > 0)
                return ZobristKey.EnPassantC;
            if ((enPassantPosition & LookupTables.ColumnMaskD) > 0)
                return ZobristKey.EnPassantD;
            if ((enPassantPosition & LookupTables.ColumnMaskE) > 0)
                return ZobristKey.EnPassantE;
            if ((enPassantPosition & LookupTables.ColumnMaskF) > 0)
                return ZobristKey.EnPassantF;
            if ((enPassantPosition & LookupTables.ColumnMaskG) > 0)
                return ZobristKey.EnPassantG;
            if ((enPassantPosition & LookupTables.ColumnMaskH) > 0)
                return ZobristKey.EnPassantH;
        }

        return 0;
    }

    private static ulong HashCastlingRights(BoardState boardState)
    {
         ulong hash = 0;

         if (boardState.WhiteCanCastleKingside)
             hash ^= ZobristKey.WhiteCastleKingside;

         if (boardState.WhiteCanCastleQueenside)
             hash ^= ZobristKey.WhiteCastleQueenside;

         if (boardState.BlackCanCastleKingside)
             hash ^= ZobristKey.BlackCastleKingside;

         if (boardState.BlackCanCastleQueenside)
             hash ^= ZobristKey.BlackCastleQueenside;

         return hash;
     }

    #endregion hash whole board methods

    internal static int GetPieceValue(PieceType pieceType, bool whitePiece)
    {
        var val = 0;

        switch (pieceType)
        {
            case PieceType.Pawn:
                val = 0;
                break;
            case PieceType.Knight:
                val = 1;
                break;
            case PieceType.Bishop:
                val = 2;
                break;
            case PieceType.Rook:
                val = 3;
                break;
            case PieceType.Queen:
                val = 4;
                break;
            case PieceType.King:
                val = 5;
                break;
        }

        if (!whitePiece)
        {
            val += 6;
        }

        return val;
    }

    internal static void Reset()
    {
        initialised = false;

        ZobristKey.PiecePositions = new ulong[12, 64];

        for (var i = 0; i < 12; i++)
        {
            for (var j = 0; j < 64; j++)
            {
                ZobristKey.PiecePositions[i, j] = 0;
            }
        }

        ZobristKey.BlackToMove = 0;

        ZobristKey.WhiteCastleKingside = 0;
        ZobristKey.WhiteCastleQueenside = 0;
        ZobristKey.BlackCastleKingside = 0;
        ZobristKey.BlackCastleQueenside = 0;

        ZobristKey.EnPassantA = 0;
        ZobristKey.EnPassantB = 0;
        ZobristKey.EnPassantC = 0;
        ZobristKey.EnPassantD = 0;
        ZobristKey.EnPassantE = 0;
        ZobristKey.EnPassantF = 0;
        ZobristKey.EnPassantG = 0;
        ZobristKey.EnPassantH = 0;
    }
}

