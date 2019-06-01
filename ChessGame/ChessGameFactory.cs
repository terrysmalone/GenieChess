using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;

namespace ChessGame
{
    public sealed class ChessGameFactory : IChessGameFactory
    {

        public Game CreateChessGame()
        {
            var scoreCalculator = new ScoreCalculator(ResourceLoader.GetResourcePath("ScoreValues.xml"));

            return new Game(scoreCalculator);
        }
    }
}
