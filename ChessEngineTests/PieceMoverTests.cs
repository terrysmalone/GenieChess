using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.PossibleMoves;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ChessEngineTests
{
    [TestClass]
    public class PieceMoverTests
    {
        [TestMethod]
        public void TestMakeUnMakeMove()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceMover = new PieceMover(board);

            var moves = MoveGeneration.CalculateAllMoves(board);
            Assert.AreEqual(20, moves.Count);

            pieceMover.MakeMove(512, 33554432, PieceType.Pawn, SpecialMoveType.DoublePawnPush, false);  //b2-b4

            var moves2 = MoveGeneration.CalculateAllMoves(board);
            Assert.AreEqual(20, moves2.Count);

            pieceMover.MakeMove(562949953421312, 8589934592, PieceType.Pawn, SpecialMoveType.DoublePawnPush, false);  //b7-b5
            pieceMover.UnMakeLastMove();
        }
    }
}