using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class PawnStructureScoreCalculationTests
{
    [Test]
    public void DoubledPawnScore()
    {
        var scoreValues = new ScoreValues
        {
            DoubledPawnScore = -15
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/3p4/4p3/8/4P3/4P3/8/8 w - - 0 1"); // White has doubled pawns
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-15));

        board.SetPosition("8/p7/p7/8/4PP2/8/8/8 w - - 0 1"); //  Black has doubled pawns
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(15));

        board.SetPosition("8/p5p1/p5p1/8/4PP2/8/8/8 w - - 0 1"); //  Black has 2 sets of doubled pawns
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(30));
    }

    [Test]
    public void ProtectedPawnScore()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/p4pp1/p7/2P5/3P4/4P3/8/8 w - - 0 1"); // White has a pawn chain of 3
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(20));

        board.SetPosition("8/1p6/2p5/3p4/4p3/2PPP3/8/8 b - - 0 1"); // Black has a pawn chain of 4
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-30));
    }

    [Test]
    public void ProtectedPawnScore_MultipleProtectsAreCountedOnce()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/1p1pp2p/8/8/3P4/2P1P3/8/Kk6 b - - 0 1"); // White has an "arrow" position
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(10));
    }

    [Test]
    public void ProtectedPawnScore_ProtectingMultiplePiecesIsCountedTwice()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/3p2p1/2p1p3/8/3PP3/1P6/8/8 w - - 0 1"); // black has an inverse "arrow" position
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-20));
    }

    [Test]
    public void ProtectedPawnScore_ProtectingAnEdge()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/6p1/2ppp3/8/P7/1P2P3/8/8 b - - 0 1"); // White has a protected pawn on a
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(10));
    }

    [Test]
    public void ProtectedPawnScore_ProtectedByEdgeEdge()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/7p/2pp2p1/8/8/PP2P3/8/8 b - - 0 1"); // Black is protecting from h
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-10));
    }

    [Test]
    public void PassedPawnScore_Single()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnScore = 15
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/2pp4/8/8/PP2P3/8/8 w - - 0 1"); // White has 1 passed pawn
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(15));

        board.SetPosition("8/6p1/1ppp4/8/8/PP2P3/8/8 w - - 0 1"); // Black has 1 passed pawn
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-15));
    }

    [Test]
    public void PassedPawnScore_MultiWhite()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnScore = 15
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/2pp2PP/8/8/PP2P3/8/8 w - - 0 1"); // White has 3 passed pawn
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(45));

        board.SetPosition("8/7p/1ppp3P/8/5p2/PP6/8/8 w - - 0 1"); // Black has 2 passed pawn
    }

    [Test]
    public void PassedPawnAdvancementScore()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnAdvancementScore = 1
        };

        var scoreCalculation = new PawnStructureScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("8/7p/1P5P/P7/5p2/5P2/8/8 b - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(7));

        board.SetPosition("8/1p5p/1P5P/P7/5p2/8/8/Kk6 b - - 0 1");
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-3));
    }
}
