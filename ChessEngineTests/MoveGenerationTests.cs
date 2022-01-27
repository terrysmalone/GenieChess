using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class MoveGenerationTests
    {
        [Test]
        public void TestPawnsBlockPawns_White()
        {
            var board = new Board();
            
             board.SetPosition("8/1K3k2/8/8/8/1p6/1P6/8 w - - 0 1");

             var moveList = MoveGeneration.CalculateAllMoves(board);

            Assert.That(moveList.Count, Is.EqualTo(8));
        }
        
        [Test]
        public void TestPawnsBlockPawns_Black()
        {
            var board = new Board();
            
            board.SetPosition("8/1K3k2/8/8/8/1p6/1P6/8 b - - 0 1");
            board.SwitchSides();

            var moveList = MoveGeneration.CalculateAllMoves(board);

            Assert.That(moveList.Count, Is.EqualTo(8));
        }
        
        [Test]
        public void TestEnPassantCaptureWhite()
        {
            var board = new Board();

            board.SetPosition("k7/5p2/8/6P1/4K3/8/8/8 b - - 0 1");

            var pieceMover = new PieceMover(board);
            //Move black f-pawn 2 spaces
            pieceMover.MakeMove(9007199254740992, 137438953472, PieceType.Pawn, SpecialMoveType.DoublePawnPush);

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.That(allMoves.Count, Is.EqualTo(9));
        }

        [Test]
        public void TestEnPassantCaptureBlack()
        {
            var board = new Board();

            board.SetPosition("k7/8/8/8/6p1/8/7P/K7 w - - 0 1");

            var pieceMover = new PieceMover(board);
            // Move h pawn 2 spaces
            pieceMover.MakeMove(32768, 2147483648, PieceType.Pawn, SpecialMoveType.DoublePawnPush); 

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.That(allMoves.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestDoubleCheck()
        {
            var board = new Board();

            board.SetPosition("8/8/4r2q/8/2B4R/2N1KP2/r7/8 w - - 0 1");

            var allMoves = MoveGeneration.CalculateAllMoves(board);
            
            Assert.AreEqual(2, allMoves.Count);

            var move1 = allMoves[0];
            Assert.That(move1.Type, Is.EqualTo(PieceType.King));
            Assert.That(move1.Position, Is.EqualTo((ulong)1048576));
            Assert.That(move1.Moves, Is.EqualTo((ulong)524288));

            var move2 = allMoves[1];
            Assert.That(move2.Type, Is.EqualTo(PieceType.King));
            Assert.That(move2.Position, Is.EqualTo((ulong)1048576));
            Assert.That(move2.Moves, Is.EqualTo((ulong)134217728)); 
        }
        
        [Test]
        public void TestBlockingPieceCantMove1()
        {
            var board = new Board();

            board.SetPosition("b4k2/8/8/8/8/8/6P1/7K w - - 0 1");

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(2, allMoves.Count);

            var move1 = allMoves[0];
            Assert.That(move1.Type, Is.EqualTo(PieceType.King));
            Assert.That(move1.Position, Is.EqualTo((ulong)128));
            Assert.That(move1.Moves, Is.EqualTo((ulong)64));

            var move2 = allMoves[1];
            Assert.That(move2.Type, Is.EqualTo(PieceType.King));
            Assert.That(move2.Position, Is.EqualTo((ulong)128));
            Assert.That(move2.Moves, Is.EqualTo((ulong)32768));        
        }

        [Test]
        public void TestBlockingPieceCantMove2()
        {
            var board = new Board();

            board.SetPosition("5k2/8/4q3/8/8/8/4N3/4K3 w - - 0 1");

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.AreEqual(4, allMoves.Count);

            foreach (var pieceMove in allMoves)
            {
                Assert.That(pieceMove.Type, Is.EqualTo(PieceType.King));
                Assert.That(pieceMove.Position, Is.EqualTo((ulong)16));
            }
        }

        [Test]
        public void TestCheckWrapping()
        {
            var board = new Board();

            board.SetPosition("5b2/8/8/8/1P5b/KP5r/1P5q/8 w - - 0 1");

            var allMoves = MoveGeneration.CalculateAllMoves(board);

            Assert.That(allMoves.Count, Is.EqualTo(2));
        }
    }
}
