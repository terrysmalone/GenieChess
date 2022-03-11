using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

public interface IScoreCalculator
{
    int CalculateScore(Board currentBoard);
}
