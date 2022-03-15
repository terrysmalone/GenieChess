using ChessEngine.BoardRepresentation;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class PieceValuesScoreCalculationTests
{
    [Test]
    public void PieceScore_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            PawnPieceValue = 1
        };

        var scoreCalculation = new PieceValuesScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("P7/8/8/8/8/8/8/8 w - - 0 1"); // just a white pawn
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("p7/8/8/8/8/8/8/8 w - - 0 1"); // just a black pawn
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Knight()
    {
        var scoreValues = new ScoreValues
        {
            KnightPieceValue = 1
        };

        var scoreCalculation = new PieceValuesScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("N7/8/8/8/8/8/8/8 w - - 0 1"); // just a white knight
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("n7/8/8/8/8/8/8/8 w - - 0 1"); // just a black black
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            BishopPieceValue = 1
        };

        var scoreCalculation = new PieceValuesScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("B7/8/8/8/8/8/8/8 w - - 0 1"); // just a white bishop
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("b7/8/8/8/8/8/8/8 w - - 0 1"); // just a black bishop
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Rook()
    {
        var scoreValues = new ScoreValues
        {
            RookPieceValue = 1
        };

        var scoreCalculation = new PieceValuesScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("R7/8/8/8/8/8/8/8 w - - 0 1"); // just a white Rook
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("r7/8/8/8/8/8/8/8 w - - 0 1"); // just a black Rook
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Queen()
    {
        var scoreValues = new ScoreValues
        {
            QueenPieceValue = 1
        };

        var scoreCalculation = new PieceValuesScoreCalculation(scoreValues);

        var board = new Board();

        board.SetPosition("Q7/8/8/8/8/8/8/8 w - - 0 1"); // just a white Queen
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(1));

        board.SetPosition("q7/8/8/8/8/8/8/8 w - - 0 1"); // just a black Queen
        Assert.That(scoreCalculation.Calculate(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_King()
    {
        var scoreCalculation = new PieceValuesScoreCalculation(new ScoreValues());

        var board = new Board();

        board.SetPosition("K7/8/8/8/8/8/8/8 w - - 0 1"); // just a white King
        Assert.That(scoreCalculation.Calculate(board), Is.GreaterThan(0));

        board.SetPosition("k7/8/8/8/8/8/8/8 w - - 0 1"); // just a black King
        Assert.That(scoreCalculation.Calculate(board), Is.LessThan(0));
    }
}
