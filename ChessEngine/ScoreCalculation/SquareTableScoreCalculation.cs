using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

internal sealed class SquareTableScoreCalculation : IScoreCalculation
{
    private const byte _endGameCount = 3;
    
    private readonly ScoreValues _scoreValues;

    public SquareTableScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }
    
    public int Calculate(Board currentBoard)
    {
        var squareTableScores = 0;

        squareTableScores += CalculatePawnSquareTableScores(currentBoard);
        squareTableScores += CalculateKnightSquareTableScores(currentBoard);
        squareTableScores += CalculateBishopSquareTableScores(currentBoard);

        if(!IsEndGame(currentBoard))
            squareTableScores += CalculateKingSquareTableScores(currentBoard);
        else
            squareTableScores += CalculateKingEndGameSquareTableScores(currentBoard);

        return squareTableScores;
    }
    
    private int CalculatePawnSquareTableScores(Board currentBoard)
    {
        var pawnSquareTableScore = 0;

        pawnSquareTableScore += CalculateTableScores(currentBoard.WhitePawns, _scoreValues.PawnSquareTable, true);

        pawnSquareTableScore -= CalculateTableScores(currentBoard.BlackPawns, _scoreValues.PawnSquareTable, false);

        return pawnSquareTableScore;
    }

    private int CalculateKnightSquareTableScores(Board currentBoard)
    {
        var knightSquareTableScore = 0;

        knightSquareTableScore += CalculateTableScores(currentBoard.WhiteKnights, _scoreValues.KnightSquareTable, true);

        knightSquareTableScore -= CalculateTableScores(currentBoard.BlackKnights, _scoreValues.KnightSquareTable, false);

        return knightSquareTableScore;
    }

    private int CalculateBishopSquareTableScores(Board currentBoard)
    {
        var bishopSquareTableScore = 0;

        bishopSquareTableScore += CalculateTableScores(currentBoard.WhiteBishops, _scoreValues.BishopSquareTable, true);

        bishopSquareTableScore -= CalculateTableScores(currentBoard.BlackBishops, _scoreValues.BishopSquareTable, false);

        return bishopSquareTableScore;
    }

    private int CalculateKingSquareTableScores(Board currentBoard)
    {
        var kingSquareTableScore = 0;

        kingSquareTableScore += CalculateTableScores(currentBoard.WhiteKing, _scoreValues.KingSquareTable, true);

        kingSquareTableScore -= CalculateTableScores(currentBoard.BlackKing, _scoreValues.KingSquareTable, false);

        return kingSquareTableScore;
    }

    private int CalculateKingEndGameSquareTableScores(Board currentBoard)
    {
        var kingEndGameSquareTableScore = 0;

        kingEndGameSquareTableScore += CalculateTableScores(currentBoard.WhiteKing, _scoreValues.KingEndGameSquareTable, true);

        kingEndGameSquareTableScore -= CalculateTableScores(currentBoard.BlackKing, _scoreValues.KingEndGameSquareTable, false);

        return kingEndGameSquareTableScore;
    }

    private static int CalculateTableScores(ulong board, int[] squareTableValues, bool isWhite)
    {
        var positions =
            BitboardOperations.GetSquareIndexesFromBoardValue(isWhite ? board : BitboardOperations.FlipVertical(board));

        return positions.Sum(squareIndex => squareTableValues[squareIndex]);
    }
    
    private static bool IsEndGame(Board currentBoard)
    {
        if(BitboardOperations.GetPopCount(currentBoard.WhiteNonEndGamePieces) < _endGameCount
           || BitboardOperations.GetPopCount(currentBoard.BlackNonEndGamePieces) < _endGameCount)
        {
            return true;
        }

        return false;
    }
}
