﻿using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.Exceptions;
using NUnit.Framework;

namespace ChessEngineTests.BoardSearching;

[TestFixture]
public class PieceCheckingTests
{
    [Test]
    public void IsEnemyPieceOnSquare_exceptionIsThrown()
    {
        Assert.Throws<BitboardException>(() => PieceChecking.IsEnemyPieceOnSquare(new Board(), 4432407298056));
    }

    [Test]
    public void GetPieceTypeOnSquare_exceptionIsThrown()
    {
        Assert.Throws<BitboardException>(() => PieceChecking.GetPieceTypeOnSquare(new Board(), 0));
    }

    [Test]
    public void IsEnemyPieceOnSquare_WithEmptyBoard()
    {
        var board = new Board();
        board.ClearBoard();

        var squares = LookupTables.SquareValuesFromIndex;

        foreach (var square in squares)
        {
            Assert.That(PieceChecking.IsEnemyPieceOnSquare(board, square), Is.False, $"Failed at square {square} ");
        }
    }

    [TestCase("8/1P6/8/8/8/8/8/8 w - - 0 1", 562949953421312u, false)]
    [TestCase("8/8/8/8/4r3/8/8/8 b - - 0 1", 268435456u, false)]
    [TestCase("8/1P6/8/8/8/8/8/8 b - - 0 1", 562949953421312u, true)]
    [TestCase("8/8/8/8/4r3/8/8/8 w - - 0 1", 268435456u, true)]
    public void IsEnemyPieceOnSquare_False(string fenString, ulong squareToCheck, bool isEnemyPieceOnSquare)
    {
        var board = new Board();
        board.SetPosition(fenString);

        Assert.AreEqual(PieceChecking.IsEnemyPieceOnSquare(board, squareToCheck), isEnemyPieceOnSquare);
    }

    [Test]
    public void GetPieceTypeOnSquare_WhitePawn()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A2);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Pawn));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Pawn));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackPawn()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Pawn));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Pawn));
    }

    [Test]
    public void GetPieceTypeOnSquare_WhiteKnight()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Knight));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B2);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Knight));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackKnight()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.G8);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Knight));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.D5);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Knight));
    }

    [Test]
    public void GetPieceTypeOnSquare_WhiteBishop()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.C1);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Bishop));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Bishop));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackBishop()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.C8);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Bishop));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Bishop));
    }

    [Test]
    public void GetPieceTypeOnSquare_WhiteRook()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.H1);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Rook));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Rook));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackRook()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.H8);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Rook));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Rook));
    }

    [Test]
    public void GetPieceTypeOnSquare_WhiteQueen()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.D1);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Queen));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Queen));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackQueen()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.D8);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Queen));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Queen));
    }

    [Test]
    public void GetPieceTypeOnSquare_WhiteKing()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.E1);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.King));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.King));
    }

    [Test]
    public void GetPieceTypeOnSquare_BlackKing()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.E8);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.King));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.King));
    }

    [Test]
    public void GetPieceTypeOnSquare_Empty()
    {
        var board = new Board();
        board.InitaliseStartingPosition();

        var pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.C5);
        Assert.That(pieceOnSquare, Is.EqualTo(PieceType.None));

        pieceOnSquare = PieceChecking.GetPieceTypeOnSquare(board, LookupTables.A1);
        Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.None));
    }

    [Test]
    public void CalculateAllowedQueenMoves_White_EmptyBoard()
    {
        var board = new Board();
        board.SetPosition("8/8/3Q4/8/8/8/8/8 w - - 0 1");


        var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

        var expected = (ulong)3034571949281478664;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_White_EmptyBoard_Ray()
    {
        var board = new Board();
        board.SetPosition("8/8/3Q4/8/8/8/8/8 w - - 0 1");


        var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

        var expected = (ulong)3034571949281478664;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_Black_EmptyBoard()
    {
        var board = new Board();
        board.SetPosition("8/8/3q4/8/8/8/8/8 b - - 0 1");


        var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

        var expected = (ulong)3034571949281478664;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_Black_EmptyBoard_Ray()
    {
        var board = new Board();
        board.SetPosition("8/8/3q4/8/8/8/8/8 b - - 0 1");


        var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

        var expected = (ulong)3034571949281478664;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_White_WithPieces()
    {
        var board = new Board();
        board.SetPosition("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1");


        var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

        var expected = (ulong)163334998696951808;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_White_WithPieces_Ray()
    {
        var board = new Board();
        board.SetPosition("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1");


        var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

        var expected = (ulong)163334998696951808;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_Black_WithPieces()
    {
        var board = new Board();
        board.SetPosition("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1");


        var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

        var expected = (ulong)1188500000215553;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedQueenMoves_Black_WithPieces_Ray()
    {
        var board = new Board();
        board.SetPosition("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1");


        var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

        var allowedMoves = PieceChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

        var expected = (ulong)1188500000215553;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedRookMoves_White()
    {
        var board = new Board();
        board.SetPosition("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1");


        var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteRooks);

        Assert.That(rookPositions.Count, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedRookMoves(board, rookPositions[0], true);

        var expected = (ulong)35325504126976;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedRookMoves_White_Ray()
    {
        var board = new Board();
        board.SetPosition("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1");


        var rookPositions = BitboardOperations.SplitBoardToArray(board.WhiteRooks);

        Assert.That(rookPositions.Length, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedRookMoves(board, rookPositions[0], true);

        var expected = (ulong)35325504126976;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedRookMoves_Black()
    {
        var board = new Board();
        board.SetPosition("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1");


        var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackRooks);

        Assert.That(rookPositions.Count, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedRookMoves(board, rookPositions[0], false);

        var expected = (ulong)139018117120;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedRookMoves_Black_Ray()
    {
        var board = new Board();
        board.SetPosition("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1");


        var rookPositions = BitboardOperations.SplitBoardToArray(board.BlackRooks);

        Assert.That(rookPositions.Length, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedRookMoves(board, rookPositions[0], false);

        var expected = (ulong)139018117120;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedBishopMoves_White()
    {
        var board = new Board();
        board.SetPosition("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1");



        var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteBishops);

        Assert.That(bishopPositions.Count, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], true);

        var expected = (ulong)585457745920;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedBishopMoves_White_Ray()
    {
        var board = new Board();
        board.SetPosition("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1");


        var bishopPositions = BitboardOperations.SplitBoardToArray(board.WhiteBishops);

        Assert.That(bishopPositions.Length, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], true);

        var expected = (ulong)585457745920;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedBishopMoves_Black()
    {
        var board = new Board();
        board.SetPosition("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1");


        var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackBishops);

        Assert.That(bishopPositions.Count, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], false);

        var expected = (ulong)5664958784208896;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void CalculateAllowedBishopMoves_Black_Ray()
    {
        var board = new Board();
        board.SetPosition("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1");


        var bishopPositions = BitboardOperations.SplitBoardToArray(board.BlackBishops);

        Assert.That(bishopPositions.Length, Is.EqualTo(1));

        var allowedMoves = PieceChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], false);

        var expected = (ulong)5664958784208896;

        Assert.That(expected, Is.EqualTo(allowedMoves));
    }

    [Test]
    public void GetSpecialMoveType()
    {
        // TODO: write tests
    }

    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", true, false)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", false, false)]
    [TestCase("4k3/4q3/8/6Q1/8/8/3P4/4K3 w - - 0 1", true, true)]
    [TestCase("4k3/4q3/8/6Q1/8/8/3P4/4K3 w - - 0 1", false, false)]
    [TestCase("4k3/4q3/3N4/6Q1/8/8/3PPP2/4K3 w - - 0 1", true, false)]
    [TestCase("4k3/4q3/3N4/6Q1/8/8/3PPP2/4K3 w - - 0 1", false, true)]
    [TestCase("8/8/5k2/4P3/4K3/8/8/8 w - - 0 1", false, true)]
    [TestCase("8/8/5k2/4P3/4K3/8/8/8 w - - 0 1", true, false)]
    public void IsKingInCheck(string fenString, bool whiteKing, bool expectedIsInCheck)
    {
        var board = new Board();
        board.SetPosition(fenString);

        Assert.That(PieceChecking.IsKingInCheck(board, whiteKing), Is.EqualTo(expectedIsInCheck));
    }

    [TestCase(134217728u, 120596463616u)]
    [TestCase(1u, 770u)]
    [TestCase(9223372036854775808u, 4665729213955833856u)]
    public void GetSurroundingSpace(ulong initialSquare, ulong expectedSurroundingSpace)
    {
        Assert.That(PieceChecking.GetSurroundingSpace(initialSquare), Is.EqualTo(expectedSurroundingSpace));
    }
}

