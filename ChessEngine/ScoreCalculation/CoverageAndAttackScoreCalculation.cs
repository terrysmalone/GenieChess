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
        var whiteBishopIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteBishops);

        ulong whiteBishopCoverageBoard = 0;
        ulong whiteBishopMoreValuableAttackBoard = 0;

        foreach (var bishopIndex in whiteBishopIndexes)
        {
            var whiteBishopPossibleMoves = PieceChecking.CalculateAllowedBishopMoves(currentBoard, bishopIndex, true);

            if (whiteBishopPossibleMoves > 0)
            {
                whiteBishopCoverageBoard |= whiteBishopPossibleMoves;

                whiteBishopMoreValuableAttackBoard |= whiteBishopPossibleMoves & (currentBoard.BlackQueen | currentBoard.BlackRooks);

                whiteAttackBoard |= whiteBishopPossibleMoves & currentBoard.AllBlackOccupiedSquares;
            }
        }

        // Add points for white bishop coverage
        attackScore += BitboardOperations.GetPopCount(whiteBishopCoverageBoard)
                       * _scoreValues.BishopCoverageScore;

        // Add points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(whiteBishopMoreValuableAttackBoard)
                       * _scoreValues.MoreValuablePieceAttackScore;

        whiteCoverageBoard |= whiteBishopCoverageBoard;

        // Score for rook coverage
        var whiteRookIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteRooks);

        ulong whiteRookCoverageBoard = 0;
        ulong whiteRookMoreValuableAttackBoard = 0;

        foreach (var whiteRookIndex in whiteRookIndexes)
        {
            var whiteRookPossibleMoves = PieceChecking.CalculateAllowedRookMoves(currentBoard, whiteRookIndex, true);

            if (whiteRookPossibleMoves > 0)
            {
                whiteRookCoverageBoard |= whiteRookPossibleMoves;

                whiteRookMoreValuableAttackBoard |= whiteRookPossibleMoves & currentBoard.BlackQueen;

                whiteAttackBoard |= whiteRookPossibleMoves & currentBoard.AllBlackOccupiedSquares;
            }
        }

        // Add points for white bishop coverage
        attackScore += BitboardOperations.GetPopCount(whiteRookCoverageBoard)
                       * _scoreValues.RookCoverageScore;

        // Add points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(whiteRookMoreValuableAttackBoard)
                       * _scoreValues.MoreValuablePieceAttackScore;

        whiteCoverageBoard |= whiteRookCoverageBoard;

        // Score for queen coverage
        var whiteQueenIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.WhiteQueen);

        foreach (var whiteQueenIndex in whiteQueenIndexes)
        {
            var whiteQueenPossibleMoves = PieceChecking.CalculateAllowedQueenMoves(currentBoard,
                                                                                   whiteQueenIndex,
                                                                                   true);
            if (whiteQueenPossibleMoves > 0)
            {
                var whiteQueenCoverageBoard = whiteQueenPossibleMoves & ~currentBoard.AllBlackOccupiedSquares;

                attackScore += BitboardOperations.GetPopCount(whiteQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

                whiteCoverageBoard |= whiteQueenCoverageBoard;

                whiteAttackBoard |= whiteQueenPossibleMoves & currentBoard.AllBlackOccupiedSquares;
            }
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
        var blackBishopIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackBishops);

        ulong blackBishopCoverageBoard = 0;
        ulong blackBishopMoreValuableAttackBoard = 0;

        foreach (var bishopIndex in blackBishopIndexes)
        {
            var blackBishopPossibleMoves = PieceChecking.CalculateAllowedBishopMoves(currentBoard, bishopIndex, false);

            if (blackBishopPossibleMoves > 0)
            {
                blackBishopCoverageBoard |= blackBishopPossibleMoves;

                blackBishopMoreValuableAttackBoard |= blackBishopPossibleMoves & (currentBoard.WhiteQueen | currentBoard.WhiteRooks);

                blackAttackBoard |= blackBishopPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
            }
        }

        // Add points for white bishop coverage
        attackScore -= BitboardOperations.GetPopCount(blackBishopCoverageBoard)
                       * _scoreValues.BishopCoverageScore;

        // Add points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(blackBishopMoreValuableAttackBoard)
                       * _scoreValues.MoreValuablePieceAttackScore;

        blackCoverageBoard |= blackBishopCoverageBoard;

        // Score for rook coverage
        var blackRookIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackRooks);

        ulong blackRookCoverageBoard = 0;
        ulong blackRookMoreValuableAttackBoard = 0;

        foreach (var blackRookIndex in blackRookIndexes)
        {
            var blackRookPossibleMoves = PieceChecking.CalculateAllowedRookMoves(currentBoard,
                                                                                      blackRookIndex,
                                                                                      false);

            if (blackRookPossibleMoves > 0)
            {
                blackRookCoverageBoard |= blackRookPossibleMoves;

                blackRookMoreValuableAttackBoard |= blackRookPossibleMoves & currentBoard.WhiteQueen;

                blackAttackBoard |= blackRookPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
            }
        }

        // Add points for white bishop coverage
        attackScore -= BitboardOperations.GetPopCount(blackRookCoverageBoard)
                       * _scoreValues.RookCoverageScore;

        // Add points for every attack on a more valuable piece
        attackScore += BitboardOperations.GetPopCount(blackRookMoreValuableAttackBoard)
                       * _scoreValues.MoreValuablePieceAttackScore;

        // Score for queen coverage
        var blackQueenIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(currentBoard.BlackQueen);

        foreach (var blackQueenIndex in blackQueenIndexes)
        {
            var blackQueenPossibleMoves = PieceChecking.CalculateAllowedQueenMoves(currentBoard,
                                                                                   blackQueenIndex,
                                                                                   false);

            if (blackQueenPossibleMoves > 0)
            {
                var blackQueenCoverageBoard = blackQueenPossibleMoves & ~currentBoard.AllWhiteOccupiedSquares;

                attackScore -= BitboardOperations.GetPopCount(blackQueenCoverageBoard) * _scoreValues.QueenCoverageScore;

                blackCoverageBoard |= blackQueenCoverageBoard;

                blackAttackBoard |= blackQueenPossibleMoves & currentBoard.AllWhiteOccupiedSquares;
            }
        }

        // Score for knight coverage
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
