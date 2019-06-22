using System;
using System.Collections.Generic;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests.MoveSearching
{
    [TestFixture]
    internal class ZobristHashTests
    {
        [Test]
        public void HashInitialisesCorrectly()
        {
            ZobristHash.Reset();

            Assert.That(ZobristKey.BlackCastleKingside,  Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.BlackCastleQueenside, Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.BlackToMove,          Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantA,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantB,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantC,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantD,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantE,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantF,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantG,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantH,           Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.WhiteCastleKingside,  Is.EqualTo((ulong) 0));
            Assert.That(ZobristKey.WhiteCastleQueenside, Is.EqualTo((ulong) 0));

            ZobristHash.Initialise();

            Assert.That(ZobristKey.BlackCastleKingside,  Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.BlackCastleQueenside, Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.BlackToMove,          Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantA,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantB,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantC,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantD,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantE,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantF,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantG,           Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.EnPassantH,           Is.Not.EqualTo((ulong) 0));

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    Assert.That(ZobristKey.PiecePositions[i, j], Is.Not.EqualTo(0));
                }
            }

            Assert.That(ZobristKey.WhiteCastleKingside,  Is.Not.EqualTo((ulong) 0));
            Assert.That(ZobristKey.WhiteCastleQueenside, Is.Not.EqualTo((ulong) 0));
        }

        [Test]
        public void HashInitialisesOnlyOnce()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            //Save values
            var values = new List<ulong>
                         {
                             ZobristKey.BlackCastleKingside,
                             ZobristKey.BlackCastleQueenside,
                             ZobristKey.BlackToMove,
                             ZobristKey.EnPassantA,
                             ZobristKey.EnPassantB,
                             ZobristKey.EnPassantC,
                             ZobristKey.EnPassantD,
                             ZobristKey.EnPassantE,
                             ZobristKey.EnPassantF,
                             ZobristKey.EnPassantG,
                             ZobristKey.EnPassantH,
                             ZobristKey.WhiteCastleKingside,
                             ZobristKey.WhiteCastleQueenside
                         };

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    values.Add(ZobristKey.PiecePositions[i, j]);
                }
            }

            ZobristHash.Initialise();

            Assert.That(ZobristKey.BlackCastleKingside,  Is.EqualTo(values[0]));
            Assert.That(ZobristKey.BlackCastleQueenside, Is.EqualTo(values[1]));
            Assert.That(ZobristKey.BlackToMove,          Is.EqualTo(values[2]));
            Assert.That(ZobristKey.EnPassantA,           Is.EqualTo(values[3]));
            Assert.That(ZobristKey.EnPassantB,           Is.EqualTo(values[4]));
            Assert.That(ZobristKey.EnPassantC,           Is.EqualTo(values[5]));
            Assert.That(ZobristKey.EnPassantD,           Is.EqualTo(values[6]));
            Assert.That(ZobristKey.EnPassantE,           Is.EqualTo(values[7]));
            Assert.That(ZobristKey.EnPassantF,           Is.EqualTo(values[8]));
            Assert.That(ZobristKey.EnPassantG,           Is.EqualTo(values[9]));
            Assert.That(ZobristKey.EnPassantH,           Is.EqualTo(values[10]));
            Assert.That(ZobristKey.WhiteCastleKingside,  Is.EqualTo(values[11]));
            Assert.That(ZobristKey.WhiteCastleQueenside, Is.EqualTo(values[12]));

            var index = 13;
            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    Assert.That(ZobristKey.PiecePositions[i, j], Is.EqualTo(values[index]));

                    index++;
                }
            }
        }

        [Test]
        public void PieceRemovalAndReplacementGivesSameValue()
        {
            ZobristHash.Reset();
            ZobristHash.Initialise();

            ulong zobrist = 0;

            ZobristHash.GetPieceValue(PieceType.Knight, true);

            // Place knight on e3
            zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(PieceType.Knight, true), BitboardOperations.GetSquareIndexFromBoardValue(1048576)];

            // Remove knight from
            zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(PieceType.Knight, true), BitboardOperations.GetSquareIndexFromBoardValue(1048576)];
        }

        [Test]
        public void MovingPiecesBackToStartGivesSameScore()
        {
            ZobristHash.Reset();
            ZobristHash.Initialise();

            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3nk3/8/8/8/8/8/8/3NK3 w - - 0 1"));

            var initialZobrist = board.Zobrist;

            board.MakeMove(UciMoveTranslator.ToGameMove("d1f2", board), false);
            board.MakeMove(UciMoveTranslator.ToGameMove("d8f7", board), false);

            board.MakeMove(UciMoveTranslator.ToGameMove("f2d1", board), false);
            board.MakeMove(UciMoveTranslator.ToGameMove("f7d8", board), false);

            var endZobrist = board.Zobrist;

            Assert.That(initialZobrist, Is.EqualTo(endZobrist));
        }

        [Test]
        public void CalculateZobristRecognisesEnPassantSquare()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                FenTranslator.ToBoardState("r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq c6 0 2"));

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong) 0));

            var board2 = new Board();
            board2.SetPosition(
                FenTranslator.ToBoardState("r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq - 0 2"));

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong) 0));

            Assert.That(initialZobristValue1, Is.Not.EqualTo(initialZobristValue2));
        }

        [Test]
        public void CalculateZobristRecognisesNoEnPassantSquare()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();
            
            board1.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("e3e4", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d5d6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d2d4", board1), false);

            var zobrist1 = board1.Zobrist;
            Assert.That(zobrist1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.InitaliseStartingPosition();

            board1.MakeMove(UciMoveTranslator.ToGameMove("e2e4", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d5d6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d3d4", board1), false);

            var zobrist2 = board2.Zobrist;
            Assert.That(zobrist2, Is.Not.EqualTo((ulong)0));

            Assert.That(zobrist1, Is.Not.EqualTo(zobrist2));
        }

        [Test]
        public void CalculateZobristDoesNotRememberPastEnPassantSquare()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();

            var startZobrist1 = board1.Zobrist;

            board1.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("d7d6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("e2e4", board1), false);   //En-passant move
            board1.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1), false);
            board1.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board1), false);

            var zobrist1 = board1.Zobrist;
            Assert.That(zobrist1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.InitaliseStartingPosition();

            var startZobrist2 = board2.Zobrist;

            board2.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("d7d6", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("c8e6", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("e3e4", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("e6d7", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("f3g1", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("d7c8", board2), false);
            board2.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board2), false);

            var zobrist2 = board2.Zobrist;
            Assert.That(zobrist2, Is.Not.EqualTo((ulong)0));

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }
    }
}
