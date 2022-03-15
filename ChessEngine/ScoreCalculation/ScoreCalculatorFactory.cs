using Logging;
using ResourceLoading;

namespace ChessEngine.ScoreCalculation;

public static class ScoreCalculatorFactory
{
    public static ScoreCalculator Create(ILog log = null)
    {
        var scoreValues = new ScoreValues();

        var scoreValueXmlReader = new ScoreValueXmlReader();
        scoreValueXmlReader.ReadScores(scoreValues, new ResourceLoader().GetGameResourcePath("ScoreValues.xml"), log ?? new NullLogger());

        var scoreCalculations = new List<IScoreCalculation>
        {
            new PieceValuesScoreCalculation(scoreValues),
            new PawnStructureScoreCalculation(scoreValues),
            new CentralPieceScoreCalculation(scoreValues),
            new CastlingScoreCalculation(scoreValues),
            new SquareTableScoreCalculation(scoreValues)
        };

        return new ScoreCalculator(scoreCalculations, scoreValues);
    }
}

