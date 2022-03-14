using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests.MoveSearching;

[TestFixture]
public class MoveOrderingTests
{
    [Test]
    public void SamePieceCapturingDifferentPieces()
    {
        var board = new Board();
        board.SetPosition("3k4/2pnbrq1/8/8/8/2RRRRR1/8/3K4 w - - 0 1");

        var pieceMoves = new List<PieceMove>
        {
            new() { Position = 8u, Moves = 4u, Type = PieceType.King, SpecialMove = SpecialMoveType.Normal }, // Random king move
            new() { Position = 131072u, Moves = 9007199254740992u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Normal }, // Rook with no capture
            new() { Position = 262144u, Moves = 1125899906842624u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capturing pawn
            new() { Position = 524288u, Moves = 2251799813685248u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capturing knight
            new() { Position = 1048576u, Moves = 4503599627370496u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capturing bishop
            new() { Position = 2097152u, Moves = 9007199254740992u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capturing rook
            new() { Position = 4194304u, Moves = 18014398509481984u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capturing queen
        };

        MoveOrdering.OrderMovesByMvvLva(board, pieceMoves);

        Assert.That(pieceMoves[0].Position, Is.EqualTo(4194304u));  // Queen capture
        Assert.That(pieceMoves[1].Position, Is.EqualTo(2097152u));  // Rook capture
        Assert.That(pieceMoves[2].Position, Is.EqualTo(1048576u));  // Bishop capture
        Assert.That(pieceMoves[3].Position, Is.EqualTo(524288u));  // Knight capture
        Assert.That(pieceMoves[4].Position, Is.EqualTo(262144u));  // Pawn capture
        Assert.That(pieceMoves[5].Position, Is.EqualTo(8u));  // King with no capture
        Assert.That(pieceMoves[6].Position, Is.EqualTo(131072u));  // Rook with no capture

    }

    [Test]
    public void DifferentPiecesCapturingSamePiece()
    {
        var board = new Board();
        board.SetPosition("8/8/1b6/3n4/4kp2/r3B2q/8/4K3 b - - 0 1");

        var pieceMoves = new List<PieceMove>
        {
            new() { Position = 268435456u, Moves = 1048576u, Type = PieceType.King, SpecialMove = SpecialMoveType.Capture }, // King capture
            new() { Position = 268435456u, Moves = 68719476736u, Type = PieceType.King, SpecialMove = SpecialMoveType.Normal }, // King move
            new() { Position = 65536u, Moves = 1048576u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture }, // Rook capture
            new() { Position = 34359738368u, Moves = 1048576u, Type = PieceType.Knight, SpecialMove = SpecialMoveType.Capture }, // Knight capture
            new() { Position = 536870912u, Moves = 1048576u, Type = PieceType.Pawn, SpecialMove = SpecialMoveType.Capture }, // pawn capture
            new() { Position = 8388608u, Moves = 1048576u, Type = PieceType.Queen, SpecialMove = SpecialMoveType.Capture }, // Queen capture
            new() { Position = 2199023255552u, Moves = 1048576u, Type = PieceType.Bishop, SpecialMove = SpecialMoveType.Capture } // Bishop capture
        };

        MoveOrdering.OrderMovesByMvvLva(board, pieceMoves);

        Assert.That(pieceMoves[0].Position, Is.EqualTo(536870912u));  // Pawn capture
        Assert.That(pieceMoves[1].Position, Is.EqualTo(34359738368u));  // Knight capture
        Assert.That(pieceMoves[2].Position, Is.EqualTo(2199023255552u));  // Bishop capture
        Assert.That(pieceMoves[3].Position, Is.EqualTo(65536u));  // Rook capture
        Assert.That(pieceMoves[4].Position, Is.EqualTo(8388608u));  // Queen capture
        Assert.That(pieceMoves[5].Position, Is.EqualTo(268435456u));  // King capture
        Assert.That(pieceMoves[5].Moves, Is.EqualTo(1048576u));
        Assert.That(pieceMoves[6].Position, Is.EqualTo(268435456u));  // King move
        Assert.That(pieceMoves[6].Moves, Is.EqualTo(68719476736u));
    }

    [Test]
    public void METHOD()
    {
        var board = new Board();
        board.SetPosition("4k3/R3p3/8/6B1/8/3q4/4P3/4K3 w - - 0 1");

        var pieceMoves = new List<PieceMove>
        {
            new() { Position = 274877906944u, Moves = 9007199254740992u, Type = PieceType.Bishop, SpecialMove = SpecialMoveType.Capture }, // Bishop capturing pawn
            new() { Position = 4096u, Moves = 524288u, Type = PieceType.Pawn, SpecialMove = SpecialMoveType.Capture }, // Pawn capturing queen
            new() { Position = 281474976710656u, Moves = 9007199254740992u, Type = PieceType.Rook, SpecialMove = SpecialMoveType.Capture } // Rook capturing pawn
        };

        MoveOrdering.OrderMovesByMvvLva(board, pieceMoves);

        Assert.That(pieceMoves[0].Type, Is.EqualTo(PieceType.Pawn));
        Assert.That(pieceMoves[1].Type, Is.EqualTo(PieceType.Bishop));
        Assert.That(pieceMoves[2].Type, Is.EqualTo(PieceType.Rook));
    }
}
