using ChessGame.BoardRepresentation;

namespace ChessGame.ScoreCalculation
{
    public interface IScoreCalculator
    {
        decimal CalculateScore(Board currentBoard);
    }
}