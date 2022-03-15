using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;

namespace ChessEngine.ScoreCalculation;

    // Calculates the score of a particular board position
public class ScoreCalculator : IScoreCalculator
{
    private readonly List<IScoreCalculation> _scoreCalculations;

    public ScoreCalculator(List<IScoreCalculation> scoreCalculations)
    {
        _scoreCalculations = scoreCalculations;
    }

    // Calculates the score with black advantage as negative and white as positive
    public int CalculateScore(Board currentBoard)
    {
        CountDebugger.Evaluations++;

        //StaticBoardChecks.Calculate(currentBoard);

        var score = 0;

        foreach (var scoreCalculation in _scoreCalculations)
        {
            score += scoreCalculation.Calculate(currentBoard);
        }

        //Add king pawn protection scores
        //Add king position scores (stay away from the middle early/mid game)

        return score;
    }
}

