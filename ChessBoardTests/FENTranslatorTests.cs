using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation;
using ChessGame.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardSearching;
using ChessGame.PossibleMoves;
using ChessGame.Enums;

namespace ChessBoardTests
{
    [TestClass]
    public class FENTranslatorTests
    {
        [TestMethod]
        public void TestTwoWayTranslate1()
        {
            string before = "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1";

            BoardState boardState = FenTranslator.ToBoardState(before);

            string after = FenTranslator.ToFENString(boardState);

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public void TestTwoWayTranslate2()
        {
            string before = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq e3 12 10";

            BoardState boardState = FenTranslator.ToBoardState(before);

            string after = FenTranslator.ToFENString(boardState);

            Assert.AreEqual(before, after);
        }

        #region ToBoardState tests

        [TestMethod]
        public void TestToBoardState_WhiteToMove()
        {
            BoardState state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/8 w - - 0 1");
            Assert.IsTrue(state.WhiteToMove);
        }

        [TestMethod]
        public void TestToBoardState_BlackToMove()
        {
            BoardState state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/8 b - - 0 1");
            Assert.IsFalse(state.WhiteToMove);
        }

        [TestMethod]
        public void TestToBoardState_WhiteCanCastleKingside()
        {
            BoardState state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/4K2R w K - 0 1");
            Assert.IsTrue(state.WhiteCanCastleKingside);
            Assert.IsFalse(state.WhiteCanCastleQueenside);
            Assert.IsFalse(state.BlackCanCastleKingside);
            Assert.IsFalse(state.BlackCanCastleQueenside);
        }

        [TestMethod]
        public void TestToBoardState_WhiteCanCastleQueenside()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w Q - 0 1");
            Assert.IsFalse(state.WhiteCanCastleKingside);
            Assert.IsTrue(state.WhiteCanCastleQueenside);
            Assert.IsFalse(state.BlackCanCastleKingside);
            Assert.IsFalse(state.BlackCanCastleQueenside);
        }

        [TestMethod]
        public void TestToBoardState_BlackCanCastleKingside()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w k - 0 1");
            Assert.IsFalse(state.WhiteCanCastleKingside);
            Assert.IsFalse(state.WhiteCanCastleQueenside);
            Assert.IsTrue(state.BlackCanCastleKingside);
            Assert.IsFalse(state.BlackCanCastleQueenside);
        }

        [TestMethod]
        public void TestToBoardState_BlackCanCastleQueenside()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w q - 0 1");
            Assert.IsFalse(state.WhiteCanCastleKingside);
            Assert.IsFalse(state.WhiteCanCastleQueenside);
            Assert.IsFalse(state.BlackCanCastleKingside);
            Assert.IsTrue(state.BlackCanCastleQueenside);
        }

        [TestMethod]
        public void TestToBoardState_AllCanCastle()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w KQkq - 0 1");
            Assert.IsTrue(state.WhiteCanCastleKingside);
            Assert.IsTrue(state.WhiteCanCastleQueenside);
            Assert.IsTrue(state.BlackCanCastleKingside);
            Assert.IsTrue(state.BlackCanCastleQueenside);
        }

        [TestMethod]
        public void TestToBoardState_AllCantCastle()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.IsFalse(state.WhiteCanCastleKingside);
            Assert.IsFalse(state.WhiteCanCastleQueenside);
            Assert.IsFalse(state.BlackCanCastleKingside);
            Assert.IsFalse(state.BlackCanCastleQueenside);
        }


        [TestMethod]
        public void TestToBoardState_EnPassant_White()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestToBoardState_EnPassant_Black()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestToBoardState_HalfMoveCount()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.AreEqual(0, state.HalfMoveClock);

            state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 6 1");
            Assert.AreEqual(6, state.HalfMoveClock);
        }

        [TestMethod]
        public void TestToBoardState_FullMoveCount()
        {
            BoardState state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.AreEqual(1, state.FullMoveClock);

            state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 6 43");
            Assert.AreEqual(43, state.FullMoveClock);
        }

        [TestMethod]
        public void TestToBoardState_WhitePawns()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("rq3k2/2pp4/1pb5/p7/2P5/3PN3/4PP2/R3K3 w Q - 0 1");

            ulong expected = 67645440;

            Assert.AreEqual(expected, state.WhitePawns);
        }

        [TestMethod]
        public void TestToBoardState_WhiteKnights_1()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");

            ulong expected = 66;

            Assert.AreEqual(expected, state.WhiteKnights);
        }

        [TestMethod]
        public void TestToBoardState_WhiteKnights_2()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4P3/2N5/PPPP1PPP/R1BQKB1R w KQkq - 0 1");

            ulong expected = 68719738880;

            Assert.AreEqual(expected, state.WhiteKnights);
        }

        [TestMethod]
        public void TestToBoardState_WhiteBishops_1()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4P3/2N5/PPPP1PPP/R1BQKB1R w KQkq - 0 1");

            ulong expected = 36;

            Assert.AreEqual(expected, state.WhiteBishops);
        }

        [TestMethod]
        public void TestToBoardState_WhiteBishops_2()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2QK2R w KQkq - 0 1");

            ulong expected = 536875008;

            Assert.AreEqual(expected, state.WhiteBishops);
        }

        [TestMethod]
        public void TestToBoardState_WhiteRooks()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2Q1RK1 w kq - 0 1");

            ulong expected = 33;

            Assert.AreEqual(expected, state.WhiteRooks);
        }

        [TestMethod]
        public void TestToBoardState_WhiteQueen_1()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2Q1RK1 w kq - 0 1");

            ulong expected = 8;

            Assert.AreEqual(expected, state.WhiteQueen);
        }

        [TestMethod]
        public void TestToBoardState_WhiteQueen_2()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            ulong expected = 2048;

            Assert.AreEqual(expected, state.WhiteQueen);
        }

        [TestMethod]
        public void TestToBoardState_WhiteKing()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            ulong expected = 64;

            Assert.AreEqual(expected, state.WhiteKing);
        }

        [TestMethod]
        public void TestToBoardState_BlackPawns_1()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            ulong expected = 71776119061217280;

            Assert.AreEqual(expected, state.BlackPawns);
        }

        [TestMethod]
        public void TestToBoardState_BlackPawns_2()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            ulong expected = 45955188128743424;

            Assert.AreEqual(expected, state.BlackPawns);
        }

        [TestMethod]
        public void TestToBoardState_BlackKnights()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqkb1r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            ulong expected = 140741783322624;

            Assert.AreEqual(expected, state.BlackKnights);
        }

        [TestMethod]
        public void TestToBoardState_BlackBishops()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("r1bqk2r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/P1PQBPPP/b4RK1 w kq - 0 1");

            ulong expected = 288230376151711745;

            Assert.AreEqual(expected, state.BlackBishops);
        }

        [TestMethod]
        public void TestToBoardState_BlackRooks()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("1rbqk2r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/P1PQBPPP/b4RK1 w k - 0 1");

            ulong expected = 9367487224930631680;

            Assert.AreEqual(expected, state.BlackRooks);
        }

        [TestMethod]
        public void TestToBoardState_BlackQueen()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("1rb1k2r/pp3p1p/2p3pn/n3N3/3pPB1q/2NP4/P1PQBPPP/b4RK1 w k - 0 1");

            ulong expected = 2147483648;

            Assert.AreEqual(expected, state.BlackQueen);
        }

        [TestMethod]
        public void TestToBoardState_BlackKing()
        {
            LookupTables.InitialiseAllTables();

            BoardState state = FenTranslator.ToBoardState("1rb3kr/pp3p1p/2p3pn/n3N3/3pPB1q/2NP4/P1PQBPPP/b4RK1 w - - 0 1");

            ulong expected = 4611686018427387904;

            Assert.AreEqual(expected, state.BlackKing);
        }

        #endregion ToBoardState tests

        [TestMethod]
        public void TestToFEN_EnPassant_White()
        {
            Board board = new Board();
            board.InitaliseStartingPosition();
            board.MakeMove(new PieceMoves() { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);

            string boardString = FenTranslator.ToFENString(board.GetCurrentBoardState());

            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 1 1", boardString);
        }
    }
}
