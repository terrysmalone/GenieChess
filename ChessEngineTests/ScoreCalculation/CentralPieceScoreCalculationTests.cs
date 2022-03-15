using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class CentralPieceScoreCalculationTests
{
    [Test]
    public void InnerCentralSquare_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralPawnScore = 1
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3P4/8/4p3/P7/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("8/8/8/3p4/8/4P3/p7/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void InnerCentralSquare_Knight()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralKnightScore = 1
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/6N1/8/4N3/2n5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("8/6n1/8/4n3/2N5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void InnerCentralSquare_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralBishopScore = 7
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/8/1b2Bb2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(7));

        board.SetPosition("8/8/8/8/1B2bB2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-7));
    }

    [Test]
    public void InnerCentralSquare_Rook()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralRookScore = 1000
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/3R4/4r3/3R4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1000));

        board.SetPosition("8/3r4/4R3/3r4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1000));
    }

    [Test]
    public void InnerCentralSquare_Queen()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralQueenScore = 900
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3QQ3/8/3q4/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1800));

        board.SetPosition("8/8/8/3Q4/1qqqq3/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-900));
    }

    [Test]
    public void OuterCentralSquare_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralPawnScore = 1
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3P4/8/4p3/P7/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));

        board.SetPosition("8/8/8/3p4/8/4P3/p7/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));
    }

    [Test]
    public void OuterCentralSquare_Knight()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralKnightScore = 1
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/6N1/8/4N3/2n5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));

        board.SetPosition("8/6n1/8/4n3/2N5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));
    }

    [Test]
    public void OuterCentralSquare_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralBishopScore = 7
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/8/1b2Bb2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-7));

        board.SetPosition("8/8/8/8/1B2bB2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(7));
    }

    [Test]
    public void OuterCentralSquare_Rook()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralRookScore = 1000
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/3R4/4r3/3R4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1000));

        board.SetPosition("8/3r4/4R3/3r4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1000));
    }

    [Test]
    public void OuterCentralSquare_Queen()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralQueenScore = 900
        };

        var scoreCalculation = new CentralPieceScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3QQ3/8/3q4/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-900));

        board.SetPosition("8/8/8/2Qq4/2QQ4/8/8/8 w - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1800));
    }
}
