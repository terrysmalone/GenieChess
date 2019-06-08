using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests
{
    [TestClass]
    public class ZobristHashTests
    {
        [TestMethod]
        public void TestInitialises()
        {
            ZobristHash.Reset();

            Assert.AreEqual((ulong)0, ZobristKey.BlackCastleKingside);
            Assert.AreEqual((ulong)0, ZobristKey.BlackCastleQueenside);

            Assert.AreEqual((ulong)0, ZobristKey.BlackToMove);

            Assert.AreEqual((ulong)0, ZobristKey.EnPassantA);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantB);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantC);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantD);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantE);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantF);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantG);
            Assert.AreEqual((ulong)0, ZobristKey.EnPassantH);
            
            Assert.AreEqual((ulong)0, ZobristKey.WhiteCastleKingside);
            Assert.AreEqual((ulong)0, ZobristKey.WhiteCastleQueenside);

            
            ZobristHash.Initialise();

            Assert.AreNotEqual((ulong)0, ZobristKey.BlackCastleKingside);
            Assert.AreNotEqual((ulong)0, ZobristKey.BlackCastleQueenside);

            Assert.AreNotEqual((ulong)0, ZobristKey.BlackToMove);

            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantA);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantB);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantC);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantD);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantE);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantF);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantG);
            Assert.AreNotEqual((ulong)0, ZobristKey.EnPassantH);

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    Assert.AreNotEqual((ulong)0, ZobristKey.PiecePositions[i, j]);
                }
            }

            Assert.AreNotEqual((ulong)0, ZobristKey.WhiteCastleKingside);
            Assert.AreNotEqual((ulong)0, ZobristKey.WhiteCastleQueenside);
        }

        [TestMethod]
        public void TestInitialises_OnlyOnce()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            //Save values
            var vals = new List<ulong>();
            vals.Add(ZobristKey.BlackCastleKingside);
            vals.Add(ZobristKey.BlackCastleQueenside);

            vals.Add(ZobristKey.BlackToMove);

            vals.Add(ZobristKey.EnPassantA);
            vals.Add(ZobristKey.EnPassantB);
            vals.Add(ZobristKey.EnPassantC);
            vals.Add(ZobristKey.EnPassantD);
            vals.Add(ZobristKey.EnPassantE);
            vals.Add(ZobristKey.EnPassantF);
            vals.Add(ZobristKey.EnPassantG);
            vals.Add(ZobristKey.EnPassantH);

            vals.Add(ZobristKey.WhiteCastleKingside);
            vals.Add(ZobristKey.WhiteCastleQueenside);

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    vals.Add(ZobristKey.PiecePositions[i, j]);
                }
            }

            ZobristHash.Initialise();

            Assert.AreEqual(vals[0], ZobristKey.BlackCastleKingside);
            Assert.AreEqual(vals[1], ZobristKey.BlackCastleQueenside);

            Assert.AreEqual(vals[2], ZobristKey.BlackToMove);

            Assert.AreEqual(vals[3], ZobristKey.EnPassantA);
            Assert.AreEqual(vals[4], ZobristKey.EnPassantB);
            Assert.AreEqual(vals[5], ZobristKey.EnPassantC);
            Assert.AreEqual(vals[6], ZobristKey.EnPassantD);
            Assert.AreEqual(vals[7], ZobristKey.EnPassantE);
            Assert.AreEqual(vals[8], ZobristKey.EnPassantF);
            Assert.AreEqual(vals[9], ZobristKey.EnPassantG);
            Assert.AreEqual(vals[10], ZobristKey.EnPassantH);

            Assert.AreEqual(vals[11], ZobristKey.WhiteCastleKingside);
            Assert.AreEqual(vals[12], ZobristKey.WhiteCastleQueenside);

            var index = 13;
            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    Assert.AreEqual(vals[index], ZobristKey.PiecePositions[i, j]);

                    index++;
                }
            }
        }

        [TestMethod]
        public void TestHashBoard_BlackToMove()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R w KQq - 0 1"));

            var initialZobristValue1 = board1.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue1);

            var board2 = new Board();
            board2.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1"));

            var initialZobristValue2 = board2.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue2);

            Assert.AreNotEqual(initialZobristValue1, initialZobristValue2);
        }

        [TestMethod]
        public void TestHashBoard_Castling()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestHashBoard_EnPassant()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1"));

            var initialZobristValue1 = board1.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue1);

            var board2 = new Board();
            board2.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq e4 0 1"));

            var initialZobristValue2 = board2.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue2);

            Assert.AreNotEqual(initialZobristValue1, initialZobristValue2);
        }

        [TestMethod]
        public void TestHashBoard_EnPassant_With_Moves()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();
            board1.MakeMove(new PieceMoves() { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            board2.MakeMove(new PieceMoves() { Position = 4096, Moves = 1048576, SpecialMove = SpecialMoveType.Normal, Type = PieceType.Pawn }, true);
            board2.MakeMove(new PieceMoves() { Position = 1048576, Moves = 268435456, SpecialMove = SpecialMoveType.Normal, Type = PieceType.Pawn }, true);

            Assert.AreNotEqual(board1.Zobrist, board2.Zobrist);
        }

        [TestMethod]
        public void TestHashBoard_EnPassant_Match_With_Moves()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();
            board1.MakeMove(new PieceMoves() { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            board2.MakeMove(new PieceMoves() { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);

            Assert.AreEqual(board1.Zobrist, board2.Zobrist);
        }

        [TestMethod]
        public void TestHashBoard_Board_GivesSameKey_1()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();

            var initialZobristValue1 = board1.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue1);

            var board2 = new Board();
            board2.InitaliseStartingPosition();

            var initialZobristValue2 = board2.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue2);

            Assert.AreEqual(initialZobristValue1, initialZobristValue2);
        }

        [TestMethod]
        public void TestHashBoard_Board_GivesSameKey_2()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1"));

            var initialZobristValue1 = board1.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue1);

            var board2 = new Board();
            board2.SetPosition(
                FenTranslator.ToBoardState("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1"));

            var initialZobristValue2 = board2.Zobrist;
            Assert.AreNotEqual((ulong)0, initialZobristValue2);

            Assert.AreEqual(initialZobristValue1, initialZobristValue2);
        }

        [TestMethod]
        public void TestHashBoard_Board_MakesAndUnmakesMove()
        {
            ZobristHash.Initialise();

            var board = new Board();
            board.InitaliseStartingPosition();

            var initialZobristValue = board.Zobrist;

            board.MakeMove((ulong)2048, (ulong)134217728, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true); //d2-d4

            Assert.AreNotEqual(initialZobristValue, board.Zobrist);

            board.UnMakeLastMove();

            Assert.AreEqual(initialZobristValue, board.Zobrist);                        
        }

        [TestMethod]
        public void TestHashBoard_Board_MakingUndoingMoves()
        {
            ZobristHash.Initialise();

            var board = new Board();
            board.InitaliseStartingPosition();

            var initialZobristValue = board.Zobrist;

            board.MakeMove((ulong)2, (ulong)65536, PieceType.Knight, SpecialMoveType.Normal, true); //Nb1-a3
            board.MakeMove((ulong)144115188075855872, (ulong)1099511627776, PieceType.Knight, SpecialMoveType.Normal, true); //Nb8-a6

             Assert.AreNotEqual(initialZobristValue, board.Zobrist);

            var twoMoveZobrist = board.Zobrist;

            board.MakeMove((ulong)64, (ulong)8388608, PieceType.Knight, SpecialMoveType.Normal, true); //Ng1-h3
            board.MakeMove((ulong)4611686018427387904, (ulong)140737488355328, PieceType.Knight, SpecialMoveType.Normal, true); //Ng8-h6

            Assert.AreNotEqual(initialZobristValue, board.Zobrist);
             Assert.AreNotEqual(twoMoveZobrist, board.Zobrist);

            board.MakeMove((ulong)8388608, (ulong)64, PieceType.Knight, SpecialMoveType.Normal, true); //Nh3-g1
            board.MakeMove((ulong)140737488355328, (ulong)4611686018427387904, PieceType.Knight, SpecialMoveType.Normal, true); //h6-g8

            Assert.AreNotEqual(initialZobristValue, board.Zobrist);
            Assert.AreEqual(twoMoveZobrist, board.Zobrist);

            board.MakeMove((ulong)65536, (ulong)2, PieceType.Knight, SpecialMoveType.Normal, true); //a3-b1
            board.MakeMove((ulong)1099511627776, (ulong)144115188075855872, PieceType.Knight, SpecialMoveType.Normal, true); //h6-g8

            Assert.AreEqual(initialZobristValue, board.Zobrist);
            Assert.AreNotEqual(twoMoveZobrist, board.Zobrist);                       
        }

        [TestMethod]
        public void TestHashBoard_Board_TwoGamesReachingSamePosition()
        {
#warning Write tests
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestHashBoard_BoardState()
        {
#warning Write tests
            throw new NotImplementedException();
        }
    }
}
