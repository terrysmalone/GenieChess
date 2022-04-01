using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngine.ScoreCalculation;

internal sealed class CoverageAndAttackScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    public CoverageAndAttackScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }

    public int Calculate(Board currentBoard)
    {
        var attackScore = 0;

        ulong whiteCoverageBoard = 0; // All the empty squares white can move to next turn

        ulong whiteAttackBoard = 0; // All squares with black pieces white can move to next turn

        // White
        // Score for bishop
        var whiteBishopPossibleMoves = PieceChecking.CalculateAllowedBishopMoves(currentBoard, currentBoard.WhiteBishops, true);

        if (whiteBishopPossibleMoves > 0)
        {
            var whiteBishopCoverageBoard = whiteBishopPossibleMoves & ~currentBoard.AllBlackOccupiedSquares;

            attackScore += BitboardOperations.GetPopCount(whiteBishopCoverageBoard)
                           * _scoreValues.BishopCoverageScore; // Add points for white bishop coverage

            whiteCoverageBoard |= whiteBishopCoverageBoard;

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(whiteBishopPossibleMoves & (currentBoard.BlackQueen | currentBoard.BlackRooks))
                           * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whiteBishopPossibleMoves & currentBoard.AllBlackOccupiedSquares;
        }

        // Score for rook coverage
        var whiteRookPossibleMoves = PieceChecking.CalculateAllowedRookMoves(currentBoard, currentBoard.WhiteRooks, true);

        if (whiteRookPossibleMoves > 0)
        {
            var whiteRookCoverageBoard = whiteRookPossibleMoves & ~currentBoard.AllBlackOccupiedSquares;

            attackScore += BitboardOperations.GetPopCount(whiteRookCoverageBoard) * _scoreValues.RookCoverageScore;

            whiteCoverageBoard |= whiteRookCoverageBoard;

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(whiteRookPossibleMoves & currentBoard.BlackQueen)
                           * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whiteRookPossibleMoves & currentBoard.AllBlackOccupiedSquares;
        }

        // Score for queen coverage
        var whiteQueenPossibleMoves = PieceChecking.CalculateAllowedQueenMoves(currentBoard,
                                                                                    currentBoard.WhiteQueen,
                                                                                    true);
        if (whiteQueenPossibleMoves > 0)
        {
            var whiteQueenCoverageBoard = whiteQueenPossibleMoves & ~currentBoard.AllBlackOccupiedSquares;

            attackScore += BitboardOperations.GetPopCount(whiteQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

            whiteCoverageBoard |= whiteQueenCoverageBoard;

            whiteAttackBoard |= whiteQueenPossibleMoves & currentBoard.AllBlackOccupiedSquares;
        }

        // TODO: Add Knight coverage score

        // Score for knight coverage
        ulong whiteKnightPossibleMoves = 0;

        var whiteKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteKnights);

        foreach (var knightPos in whiteKnightPositions)
        {
            var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

            possibleMoves &= ~currentBoard.AllWhiteOccupiedSquares;
            whiteKnightPossibleMoves |= possibleMoves;
        }

        whiteCoverageBoard |= whiteKnightPossibleMoves & ~currentBoard.AllBlackOccupiedSquares;

        whiteAttackBoard |= whiteKnightPossibleMoves & currentBoard.AllBlackOccupiedSquares;

        // Points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(whiteKnightPossibleMoves & (currentBoard.BlackQueen | currentBoard.BlackRooks))
                       * _scoreValues.MoreValuablePieceAttackScore;

        //Pawns
        const ulong notA = 18374403900871474942;
        const ulong notH = 9187201950435737471;

        var whitePawnPossibleAttackMoves = ((currentBoard.WhitePawns << 9) & notA)
                                                | ((currentBoard.WhitePawns << 7) & notH);

        if (whitePawnPossibleAttackMoves > 0)
        {
            // There is no Score for pawn coverage, just pawn attacks

            // Add points for every attack on a more valuable piece
            attackScore += BitboardOperations.GetPopCount(whitePawnPossibleAttackMoves
                                                          & (currentBoard.BlackQueen
                                                             | currentBoard.BlackRooks
                                                             | currentBoard.BlackBishops
                                                             | currentBoard.BlackKnights))
                           * _scoreValues.MoreValuablePieceAttackScore;

            whiteAttackBoard |= whitePawnPossibleAttackMoves & currentBoard.AllBlackOccupiedSquares;
        }

        // Points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(whiteKnightPossibleMoves & (currentBoard.BlackQueen | currentBoard.BlackRooks))
                       * _scoreValues.MoreValuablePieceAttackScore;

        // Points for overall board coverage
        attackScore += BitboardOperations.GetPopCount(whiteCoverageBoard) * _scoreValues.BoardCoverageScore;

        // Points for board attacks
        attackScore += BitboardOperations.GetPopCount(whiteAttackBoard) * _scoreValues.AttackScore;

        //Black
        ulong blackCoverageBoard = 0; // All the empty squares black can move to next turn
        ulong blackAttackBoard = 0; // All white squares black can move to next turn

        // Score for bishop coverage
        var blackBishopPossibleMoves = PieceChecking.CalculateAllowedBishopMoves(currentBoard,
                                                                                      currentBoard.BlackBishops,
                                                                                      false);

        if (blackBishopPossibleMoves > 0)
        {
            var blackBishopCoverageBoard = blackBishopPossibleMoves & ~currentBoard.AllWhiteOccupiedSquares;

            // Add points for black bishop coverage
            attackScore -= BitboardOperations.GetPopCount(blackBishopCoverageBoard)
                           * _scoreValues.BishopCoverageScore;

            blackCoverageBoard |= blackBishopCoverageBoard;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(blackBishopPossibleMoves & (currentBoard.WhiteQueen | currentBoard.WhiteRooks))
                           * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackBishopPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
        }

        // Score for rook coverage
        var blackRookPossibleMoves = PieceChecking.CalculateAllowedRookMoves(currentBoard,
                                                                                  currentBoard.BlackRooks,
                                                                                  false);

        if (blackRookPossibleMoves > 0)
        {
            var blackRookCoverageBoard = blackRookPossibleMoves & ~currentBoard.AllWhiteOccupiedSquares;

            // Add points for black rook coverage
            attackScore -= BitboardOperations.GetPopCount(blackRookCoverageBoard) * _scoreValues.RookCoverageScore;

            blackCoverageBoard |= blackRookCoverageBoard;

            // Points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(blackRookPossibleMoves & currentBoard.WhiteQueen)
                           * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackRookPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
        }

        // Score for queen coverage
        var blackQueenPossibleMoves = PieceChecking.CalculateAllowedQueenMoves(currentBoard,
                                                                                    currentBoard.BlackQueen,
                                                                                    false);

        if (blackQueenPossibleMoves > 0)
        {
            var blackQueenCoverageBoard = blackQueenPossibleMoves & ~currentBoard.AllWhiteOccupiedSquares;

            attackScore -= BitboardOperations.GetPopCount(blackQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

            blackCoverageBoard |= blackQueenCoverageBoard;

            blackAttackBoard |= blackQueenPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
        }

        //// Score for knight coverage
        ulong blackKnightPossibleMoves = 0;

        var blackKnightPositions = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackKnights);

        foreach (var knightPos in blackKnightPositions)
        {
            var possibleMoves = ValidMoveArrays.KnightMoves[knightPos];

            possibleMoves &= ~currentBoard.AllBlackOccupiedSquares;
            blackKnightPossibleMoves |= possibleMoves;
        }

        blackCoverageBoard |= blackKnightPossibleMoves & ~currentBoard.AllWhiteOccupiedSquares;

        blackAttackBoard |= blackKnightPossibleMoves & currentBoard.AllWhiteOccupiedSquares;

        // Points for every attack on a more valuable piece
        attackScore -= BitboardOperations.GetPopCount(blackKnightPossibleMoves & (currentBoard.WhiteQueen | currentBoard.WhiteRooks))
                       * _scoreValues.MoreValuablePieceAttackScore;

        //Pawns
        var blackPawnPossibleAttackMoves = ((currentBoard.BlackPawns >> 7) & notA)
                                                | ((currentBoard.BlackPawns >> 7) & notH);

        if (blackPawnPossibleAttackMoves > 0)
        {
            // There is no Score for pawn coverage, just pawn attacks
            // Add points for every attack on a more valuable piece
            attackScore -= BitboardOperations.GetPopCount(blackPawnPossibleAttackMoves & (currentBoard.WhiteQueen
                                                                                          | currentBoard.WhiteRooks
                                                                                          | currentBoard.WhiteBishops
                                                                                          | currentBoard.WhiteKnights))
                           * _scoreValues.MoreValuablePieceAttackScore;

            blackAttackBoard |= blackPawnPossibleAttackMoves & currentBoard.AllWhiteOccupiedSquares;
        }

        // Points for overall board coverage
        attackScore -= BitboardOperations.GetPopCount(blackCoverageBoard) * _scoreValues.BoardCoverageScore;

        // Points for board attacks
        attackScore -= BitboardOperations.GetPopCount(blackAttackBoard) * _scoreValues.AttackScore;

        return attackScore;
    }
}
