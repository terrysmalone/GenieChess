using ChessEngine.BoardRepresentation;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class BoardStateTests
    {
        [Test]
        public void TestInitialized()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteToMove()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();
            
            board1.WhiteToMove = true;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteToMove = true;
            Assert.That(board1, Is.EqualTo(board2));      
        }

        [Test]
        public void TestWhitePawns()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhitePawns = 67637760;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhitePawns = 67637760;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteKnights()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackPawns = 283794259050496;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackPawns = 283794259050496;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteBishops()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteBishops = 134217730;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteBishops = 134217730;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteRooks()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteRooks = 2147483649;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteRooks = 2147483649;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteQueen()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteQueen = 524288;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteQueen = 524288;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteKing()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteKing = 576460752303423488;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteKing = 576460752303423488;
            Assert.That(board1, Is.EqualTo(board2));
        }        

        [Test]
        public void TestBlackPawns()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackPawns = 848857336381440;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackPawns = 848857336381440;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackKnights()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackKnights = 4755801206503243776;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackKnights = 4755801206503243776;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackBishops()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackBishops = 4611686018427518976;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackBishops = 4611686018427518976;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackRooks()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackRooks = 4683743612465315840;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackRooks = 4683743612465315840;
            Assert.That(board1, Is.EqualTo(board2));
        }
        [Test]
        public void TestBlackQueen()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackQueen = 17592186044416;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackQueen = 17592186044416;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackKing()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackKing = 2251799813685248;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackKing = 2251799813685248;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestEnPassantPosition()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.EnPassantPosition = 4398046511104;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.EnPassantPosition = 4398046511104;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteCanCastleQueenside()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteCanCastleQueenside = true;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteCanCastleQueenside = true;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestWhiteCanCastleKingside()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.WhiteCanCastleKingside = true;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.WhiteCanCastleKingside = true;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackCanCastleQueenside()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackCanCastleQueenside = true;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackCanCastleQueenside = true;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestBlackCanCastleKingside()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.BlackCanCastleKingside = true;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.BlackCanCastleKingside = true;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestHalfMoveClock()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.HalfMoveClock = 5;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.HalfMoveClock = 5;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestFullMoveClock()
        {
            var board1 = new BoardState();
            var board2 = new BoardState();

            board1.HalfMoveClock = 8;
            Assert.That(board1, Is.Not.EqualTo(board2));

            board2.HalfMoveClock = 8;
            Assert.That(board1, Is.EqualTo(board2));
        }

        [Test]
        public void TestZobristKey()
        {
        var board1 = new BoardState();
        var board2 = new BoardState();

        board1.ZobristKey = 7658768968969597578;
        Assert.That(board1, Is.Not.EqualTo(board2));

        board2.ZobristKey = 7658768968969597578;
        Assert.That(board1, Is.EqualTo(board2));
        }
    }
}
