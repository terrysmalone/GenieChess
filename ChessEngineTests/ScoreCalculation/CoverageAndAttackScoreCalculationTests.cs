using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class CoverageAndAttackScoreCalculationTests
{
    // TODO: Test BoardCoverageScore
    [Test]
    public void BoardCoverageScore()
    {
        var scoreValues = new ScoreValues
        {
            BoardCoverageScore = 1
        };

        var scoreCalculation = new CoverageAndAttackScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("7k/5Q2/2B5/4R3/N7/4P3/3P4/7K b - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(34));
    }
    
    
    
    // TODO: Test AttackScore
    
    // TODO: Test BishopCoverageScore
    [TestCase ("3k4/8/8/3B4/8/8/8/3K4 w - - 0 1", 1, 13)]   // Just a white bishop
    [TestCase ("3k4/8/8/6B1/8/3B4/3K4/8 b - - 0 1", 2, 34)]   // Just multiple white bishops
    [TestCase ("1k6/8/3b4/8/5K2/8/8/8 w - - 0 1", 10, -70)]   // Just a black bishop
    [TestCase ("5b2/5b2/8/2k4K/8/8/8/8 b - - 0 1", 1, -12)]   // Just multiple black bishops
    [TestCase ("8/2k2b2/8/3K4/2B5/8/3B4/8 b - - 0 1", 1, 11)]   // Both side bishops
    [TestCase ("4k3/2b5/5p2/Q7/1n6/8/5P2/3KB2R w - - 0 1", 3, -18)]   // Both side bishops with other pieces
    public void BishopCoverageScore(string fenPosition, int scoreValue, int expectedScore)
    {
        var scoreValues = new ScoreValues
        {
            BishopCoverageScore = scoreValue
        };

        var scoreCalculation = new CoverageAndAttackScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition(fenPosition);
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(expectedScore));
    }



    // TODO: Test RookCoverageScore
    // TODO: Test QueenCoverageScore
    
    // TODO: Test MoreValuablePieceAttackScore
}
