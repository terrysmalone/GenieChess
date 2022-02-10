using ResourceLoading;

namespace ChessEngine.ScoreCalculation
{
    public static class ScoreCalculatorFactory
    {
        public static ScoreCalculator Create()
        {
            var scoreValues = new ScoreValues();

            ScoreValueXmlReader.ReadScores(scoreValues, new ResourceLoader().GetGameResourcePath("ScoreValues.xml"));

            return new ScoreCalculator(scoreValues);
        }
    }
}
