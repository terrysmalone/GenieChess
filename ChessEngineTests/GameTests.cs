using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests
{
    // Tests to make sure the whole engine behaves as expected
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void TestPlayingGame_VerySimpleCapture_White([Range(1,6)] int thinkingDepth)
        {
            var scoreCalculator = ScoreCalculatorFactory.Create();
            var game = new Game(scoreCalculator, new Board(), null);

            game.SetPosition("7k/8/8/3pK3/8/8/8/8 w - - 0 1");

            game.ThinkingDepth = thinkingDepth;

            game.AllowAllCastling(false);
            
            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFenString(game.GetCurrentBoardState());

            Assert.That(fenNotation, Is.EqualTo(expectedFen));
        }
        
        [Test]
        public void TestPlayingGame_VerySimpleCapture_Black([Range(1, 6)] int thinkingDepth)
        {
            var scoreCalculator = ScoreCalculatorFactory.Create();
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = thinkingDepth;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFenString(game.GetCurrentBoardState());

            Assert.That(fenNotation, Is.EqualTo(expectedFen));
        }
    }
}
