using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class KingPositionScoreCalculatorTests
{
    [Test]
    public void AfterMidGameDoesntCount_WhiteMovedEarly()
    {
        var scoreValues = new ScoreValues
        {
            EarlyMoveKingScore = -100
        };

        var kingPositionScoreCalculator = new KingPositionScoreCalculator(scoreValues);

        var board = new Board();
        board.SetPosition("r2qk2r/pppb1ppp/2np1b1n/4p1B1/N3P3/2PP1N2/PP2BPPP/R2Q1K1R w kq - 0 1"); // It's in the mid game. It's no longer early

        Assert.That(kingPositionScoreCalculator.Calculate(board), Is.EqualTo(0));
    }

    [Test]
    public void AfterMidGameDoesntCount_BlackMovedEarly()
    {
        var scoreValues = new ScoreValues
        {
            EarlyMoveKingScore = -100
        };

        var kingPositionScoreCalculator = new KingPositionScoreCalculator(scoreValues);

        var board = new Board();
        board.SetPosition("r2q1k1r/pppb1ppp/2np1b1n/4p1B1/N3P3/2PP1N2/PP2BPPP/R2QK2R w KQ - 0 1"); // It's in the mid game. It's no longer early

        Assert.That(kingPositionScoreCalculator.Calculate(board), Is.EqualTo(0));
    }

    [Test]
    public void EarlyMoveKingScore_White()
    {
        var scoreValues = new ScoreValues
        {
            EarlyMoveKingScore = -10
        };

        var kingPositionScoreCalculator = new KingPositionScoreCalculator(scoreValues);

        var board = new Board();
        board.SetPosition("rn1qk1nr/pppbbppp/3p4/4p3/4P3/3P1N2/PPP1BPPP/RNBQ1K1R b kq - 0 1"); //// White has moved his king early

        Assert.That(kingPositionScoreCalculator.Calculate(board), Is.EqualTo(-10));
    }

    [Test]
    public void EarlyMoveKingScore_Black()
    {
        var scoreValues = new ScoreValues
        {
            EarlyMoveKingScore = -10
        };

        var kingPositionScoreCalculator = new KingPositionScoreCalculator(scoreValues);

        var board = new Board();
        board.SetPosition("rn1q1knr/pppbbppp/3p4/4p3/4P3/3P1N2/PPP1BPPP/RNBQK2R w KQ - 0 1"); //// Black has moved his king early

        Assert.That(kingPositionScoreCalculator.Calculate(board), Is.EqualTo(10));
    }
}
