using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests.PossibleMoves;

[TestFixture]
public class PieceValidMovesTests
{
    [TestCase(36, 17592186044416u)]
    [TestCase(12, 269484032u)]
    [TestCase(63, 0u)]
    public void WhitePawnMoves(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFrom = ValidMoveArrays.WhitePawnMoves[moveFromPosition];

        Assert.That(movesFrom, Is.EqualTo(expectedMoveToBoard));
    }

    [TestCase(0, 0u)]
    [TestCase(13, 5242880u)]
    [TestCase(23, 1073741824u)]
    public void WhitePawnCaptures(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFrom = ValidMoveArrays.WhitePawnCaptures[moveFromPosition];

        Assert.That(movesFrom, Is.EqualTo(expectedMoveToBoard));
    }

    [TestCase(51, 8830452760576u)]
    [TestCase(16, 256u)]
    [TestCase(47, 549755813888u)]
    public void BlackPawnMoves(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFrom = ValidMoveArrays.BlackPawnMoves[moveFromPosition];

        Assert.That(movesFrom, Is.EqualTo(expectedMoveToBoard));
    }

    [TestCase(48, 2199023255552u)]
    [TestCase(10, 10u)]
    [TestCase(38, 2684354560u)]

    public void BlackPawnCaptures(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFrom = ValidMoveArrays.BlackPawnCaptures[moveFromPosition];

        Assert.That(movesFrom, Is.EqualTo(expectedMoveToBoard));
    }

    [TestCase(0, 132096u)]
    [TestCase(35, 5666883501293568u)]
    [TestCase(61, 38368557762871296u)]

    public void KnightMoves(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFromD5 = ValidMoveArrays.KnightMoves[moveFromPosition];

        Assert.That(movesFromD5, Is.EqualTo(expectedMoveToBoard));
    }

    [TestCase(28, 241192927232u)]
    [TestCase(24, 12918652928u)]
    [TestCase(63, 4665729213955833856u)]
    [TestCase(0, 770u)]
    public void KingMoves(int moveFromPosition, ulong expectedMoveToBoard)
    {
        PieceValidMoves.GenerateMoveArrays();

        var movesFrom = ValidMoveArrays.KingMoves[moveFromPosition];

        Assert.That(movesFrom, Is.EqualTo(expectedMoveToBoard));
    }
}

