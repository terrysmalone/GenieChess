using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class CastlingScoreCalculationTests
{
    [Test]
    public void CastlingIsIgnoredInEndGame()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 1,
            CastlingKingSideScore = 10
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("5rk1/8/8/8/8/8/8/1K1R4 w - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(0));
    }

    [Test]
    public void KingCastling_WhiteKingSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 10
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("r3k2r/4p3/2b5/8/8/8/2B2PP1/R4RK1 w - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(10));
    }

    [Test]
    public void KingCastling_WhiteQueenSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 8
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("r3k2r/4p3/2b5/8/8/8/2B2PP1/2KR1R2 b - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(8));
    }

    [Test]
    public void KingCastling_BlackKingSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 12
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("4rrk1/4p3/2b5/8/8/8/2B2PP1/1K1R1R2 w - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-12));
    }

    [Test]
    public void KingCastling_BlackQueenSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 4
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("2kr3r/4p3/2b5/8/8/8/2B2PP1/1K1R1R2 w - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-4));
    }

    [Test]
    public void KingCastling_SidesCancelEachOtherOut()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 15
        };

        var scoreCalculation = new CastlingScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("4rrk1/4p3/2b5/8/8/8/2B2PP1/3R1RK1 b - - 0 1");

        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(0));
    }
}
