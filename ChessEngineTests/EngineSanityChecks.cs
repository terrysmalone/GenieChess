using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests;

[TestFixture]
public class EngineSanityChecks
{
    [Test]
    public void MateInOne_White([Range(1, 6)] int depth)
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

        game.ThinkingDepth = depth;
        game.MakeBestMove();

        Assert.That(game.GetPosition(), Is.EqualTo("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1"));
    }

    [Test]
    public void MateInOne_Black([Range(1, 6)] int depth)
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

        game.ThinkingDepth = depth;
        game.MakeBestMove();

        Assert.That(game.GetPosition(), Is.EqualTo("1k4q1/7r/8/8/8/8/8/7K w - - 1 2"));
    }

    // Anything less than 8 ply can't find 1..f6.
    // 1..b2 loses the queen to 4 pawns
    [ Test]
    public void HorizonEffect([Range (3, 6)] int depth)
    {
        // TODO: write tests

        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

        game.ThinkingDepth = depth;
        game.MakeBestMove();

        Assert.That(game.GetPosition(), Is.EqualTo("5r1k/4Q1q1/4pp2/1p1p2P1/2p2P2/1p2P3/3P4/BK6 w - - 1 1"));
    }

    /// <summary>
    /// Leonid's Position
    /// </summary>
    [ Test, Timeout(5000)]
    public void TestForCheckExplosion()
    {
        // TODO: write tests
        throw new NotImplementedException();

        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("q2k2q1/2nqn2b/1n1P1n1b/2rnr2Q/1NQ1QN1Q/3Q3B/2RQR2B/Q2K2Q1 w - -");

        game.ThinkingDepth = 1;
        game.MakeBestMove();
    }

    /// <summary>
    /// Tests zugzwang causing null moves to fail
    /// </summary>
    [ Test]
    public void TestNullMove_1()
    {
        // TODO: Write test
        throw new NotImplementedException();
    }

    [ Test]
    public void TestKingCastlingWhileInCheckBug()
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("r3k2r/p2b1ppp/2p2n2/b2p4/5B2/3B4/PPP1NPPP/R3K2R w KQkq - 0 1");

        var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

        Assert.That(move, Is.Not.EqualTo("e1g1"));
    }

    // If we search this position to a shallow depth of 2 it looks like white would lose the exchange
    // The quiescence search should broaden the search to show that white easily wins
    [ Test]
    public void QuiescenceCausesWhiteToCapture()
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("3k4/3r4/3r4/8/3Q4/3R4/8/3K4 w - - 0 1");

        game.ThinkingDepth = 2;

        var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

        Assert.That(move, Is.EqualTo("d4d6"));
    }

    // At a depth of 2 this moe seems like white would win from the capture but the
    // quiescence should show otherwise
    [ Test]
    public void QuiescenceStopsWhiteFromCapturing()
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("8/8/3k4/3r4/3P4/8/3n1Q2/3K4 w - - 0 1");

        game.ThinkingDepth = 2;

        var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

        Assert.That(move, Is.Not.EqualTo("f2d2"));

    }

    // If we search this position to a shallow depth of 2 it looks like black would lose the exchange
    // The quiescence search should broaden the search to show that black easily wins
    [ Test]
    public void QuiescenceCausesBlackToCapture()
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("7k/6bp/5q2/8/3R4/8/1Q6/K7 b - - 0 1");

        game.ThinkingDepth = 2;

        var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

        Assert.That(move, Is.EqualTo("f6d4"));
    }

    // At a depth of 2 this move seems like black would win from the capture but the
    // quiescence should show otherwise
    [ Test]
    public void QuiescenceStopsBlackFromCapturing()
    {
        var moveGeneration = new MoveGeneration();
        var scoreCalculator = ScoreCalculatorFactory.Create();

        var game = new Game(moveGeneration, scoreCalculator, new Board(), null);
        game.ClearBoard();
        game.SetPosition("5n2/4k3/5q2/4N3/4p3/1QN5/3KR3/8 b - - 0 1");

        game.ThinkingDepth = 2;

        var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

        Assert.That(move, Is.Not.EqualTo("f6e5"));

    }
}
