using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

public interface IScoreCalculation
{
    public int Calculate(Board currentBoard);
}
