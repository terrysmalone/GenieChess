using System;
using ChessGame;
using ChessGame.BoardRepresentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessBoardTests
{
    [TestClass]
    public class BoardStateTests
    {
        [TestMethod]
        public void TestInitialized()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            Assert.IsTrue(board1.Equals(board2));

        }

        [TestMethod]
        public void TestWhiteToMove()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();
            
            board1.WhiteToMove = true;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteToMove = true;
            Assert.IsTrue(board1.Equals(board2));       
        }

        [TestMethod]
        public void TestWhitePawns()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhitePawns = (ulong)67637760;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhitePawns = (ulong)67637760;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteKnights()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackPawns = (ulong)283794259050496;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackPawns = (ulong)283794259050496;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteBishops()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteBishops = (ulong)134217730;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteBishops = (ulong)134217730;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteRooks()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteRooks = (ulong)2147483649;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteRooks = (ulong)2147483649;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteQueen()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteQueen = (ulong)524288;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteQueen = (ulong)524288;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteKing()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteKing = (ulong)576460752303423488;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteKing = (ulong)576460752303423488;
            Assert.IsTrue(board1.Equals(board2));
        }        

        [TestMethod]
        public void TestBlackPawns()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackPawns = (ulong)848857336381440;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackPawns = (ulong)848857336381440;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackKnights()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackKnights = (ulong)4755801206503243776;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackKnights = (ulong)4755801206503243776;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackBishops()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackBishops = (ulong)4611686018427518976;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackBishops = (ulong)4611686018427518976;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackRooks()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackRooks = (ulong)4683743612465315840;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackRooks = (ulong)4683743612465315840;
            Assert.IsTrue(board1.Equals(board2));
        }
        [TestMethod]
        public void TestBlackQueen()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackQueen = (ulong)17592186044416;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackQueen = (ulong)17592186044416;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackKing()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackKing = (ulong)2251799813685248;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackKing = (ulong)2251799813685248;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestEnPassantPosition()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.EnPassantPosition = (ulong)4398046511104;
            Assert.IsFalse(board1.Equals(board2));

            board2.EnPassantPosition = (ulong)4398046511104;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteCanCastleQueenside()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteCanCastleQueenside = true;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteCanCastleQueenside = true;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestWhiteCanCastleKingside()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.WhiteCanCastleKingside = true;
            Assert.IsFalse(board1.Equals(board2));

            board2.WhiteCanCastleKingside = true;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackCanCastleQueenside()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackCanCastleQueenside = true;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackCanCastleQueenside = true;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestBlackCanCastleKingside()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.BlackCanCastleKingside = true;
            Assert.IsFalse(board1.Equals(board2));

            board2.BlackCanCastleKingside = true;
            Assert.IsTrue(board1.Equals(board2));
        }

        [TestMethod]
        public void TestHalfMoveClock()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.HalfMoveClock = 5;
            Assert.IsFalse(board1.Equals(board2));

            board2.HalfMoveClock = 5;
            Assert.IsTrue(board1.Equals(board2));
        }

         [TestMethod]
        public void TestFullMoveClock()
        {
            BoardState board1 = new BoardState();
            BoardState board2 = new BoardState();

            board1.HalfMoveClock = 8;
            Assert.IsFalse(board1.Equals(board2));

            board2.HalfMoveClock = 8;
            Assert.IsTrue(board1.Equals(board2));
        }

         //[TestMethod]
         //public void TestPgnMove()
         //{
         //    BoardState board1 = new BoardState();
         //    BoardState board2 = new BoardState();

         //    board1.PgnMove = "e4";
         //    Assert.IsFalse(board1.Equals(board2));

         //    board2.PgnMove = "e4";
         //    Assert.IsTrue(board1.Equals(board2));
         //}

         [TestMethod]
         public void TestZobristKey()
         {
             BoardState board1 = new BoardState();
             BoardState board2 = new BoardState();

             board1.ZobristKey = (ulong)7658768968969597578;
             Assert.IsFalse(board1.Equals(board2));

             board2.ZobristKey = (ulong)7658768968969597578;
             Assert.IsTrue(board1.Equals(board2));
         }

    }
}
