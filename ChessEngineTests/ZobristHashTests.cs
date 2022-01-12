using System.Collections.Generic;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class ZobristHashTests
    {
        [Test]
        public void InitialBoardsaMatch()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.InitaliseStartingPosition();

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong)0));

            Assert.That(board1.Zobrist, Is.EqualTo(board2.Zobrist));
        }

        [Test]
        public void InitialisesOnlyOnce()
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

            ZobristHash.Initialise();   // This should not cause the values to change

            Assert.That(ZobristKey.BlackCastleKingside, Is.EqualTo(vals[0]));
            Assert.That(ZobristKey.BlackCastleQueenside, Is.EqualTo(vals[1]));

            Assert.That(ZobristKey.BlackToMove, Is.EqualTo(vals[2]));

            Assert.That(ZobristKey.EnPassantA, Is.EqualTo(vals[3]));
            Assert.That(ZobristKey.EnPassantB, Is.EqualTo(vals[4]));
            Assert.That(ZobristKey.EnPassantC, Is.EqualTo(vals[5]));
            Assert.That(ZobristKey.EnPassantD, Is.EqualTo(vals[6]));
            Assert.That(ZobristKey.EnPassantE, Is.EqualTo(vals[7]));
            Assert.That(ZobristKey.EnPassantF, Is.EqualTo(vals[8]));
            Assert.That(ZobristKey.EnPassantG, Is.EqualTo(vals[9]));
            Assert.That(ZobristKey.EnPassantH, Is.EqualTo(vals[10]));

            Assert.That(ZobristKey.WhiteCastleKingside, Is.EqualTo(vals[11]));
            Assert.That(ZobristKey.WhiteCastleQueenside, Is.EqualTo(vals[12]));

            var index = 13;

            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    Assert.That(ZobristKey.PiecePositions[i, j], Is.EqualTo(vals[index]));

                    index++;
                }
            }
        }

        [Test]
        public void PieceToMove()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R w KQq - 0 1");

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1");

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong)0));

            Assert.That(initialZobristValue1, Is.Not.EqualTo(initialZobristValue2));
        }

        [TestCase("r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w KQq - 0 1", "r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w KQ - 0 1")]
        [TestCase("r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w q - 0 1", "r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w - - 0 1")]
        [TestCase("r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w - - 0 1", "r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w Q - 0 1")]
        [TestCase("r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w k - 0 1", "r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w - - 0 1")]
        [TestCase("r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w - - 0 1", "r2qk2r/p4p2/8/8/6Q1/8/P4PP1/RN2K2R w K - 0 1")]
        public void Castling(string position1, string position2)
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(position1);

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.SetPosition(position2);

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong)0));

            Assert.That(initialZobristValue1, Is.Not.EqualTo(initialZobristValue2));
        }

        [Test]
        public void EnPassant()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1");

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq e4 0 1");

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong)0));

            Assert.That(initialZobristValue1, Is.Not.EqualTo(initialZobristValue2));
        }

        [Test]
        public void EnPassantWithMoves_NonMatchWithMoves()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();
            var pieceMover = new PieceMover(board1);
            pieceMover.MakeMove(new PieceMove { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn });

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            pieceMover = new PieceMover(board2);
            pieceMover.MakeMove(new PieceMove { Position = 4096, Moves = 1048576, SpecialMove = SpecialMoveType.Normal, Type = PieceType.Pawn });
            pieceMover.MakeMove(new PieceMove { Position = 1048576, Moves = 268435456, SpecialMove = SpecialMoveType.Normal, Type = PieceType.Pawn });

            Assert.That(board1.Zobrist, Is.Not.EqualTo(board2.Zobrist));
        }

        [Test]
        public void EnPassant_MatchWithMoves()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.InitaliseStartingPosition();
            var pieceMover = new PieceMover(board1);
            pieceMover.MakeMove(new PieceMove { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn });

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            pieceMover = new PieceMover(board2);
            pieceMover.MakeMove(new PieceMove { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn });

            Assert.That(board1.Zobrist, Is.EqualTo(board2.Zobrist));
        }

        [Test]
        public void SamePositionsMatch()
        {
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1");

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.SetPosition("rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1");

            var initialZobristValue2 = board2.Zobrist;
            Assert.That(initialZobristValue2, Is.Not.EqualTo((ulong)0));

            Assert.That(initialZobristValue1, Is.EqualTo(initialZobristValue2));
        }

        [Test]
        public void MakeAndUnmakeMatches()
        {
            ZobristHash.Initialise();

            var board = new Board();
            board.InitaliseStartingPosition();
            var pieceMover = new PieceMover(board);

            var initialZobristValue = board.Zobrist;

            pieceMover.MakeMove(2048, 134217728, PieceType.Pawn, SpecialMoveType.DoublePawnPush); //d2-d4

            Assert.That(board.Zobrist, Is.Not.EqualTo(initialZobristValue));

            pieceMover.UnMakeLastMove();

            Assert.That(board.Zobrist, Is.EqualTo(initialZobristValue));
        }

        [Test]
        public void MakeAndUnmakeMultipleMovesMatches()
        {
            ZobristHash.Initialise();

            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceMover = new PieceMover(board);

            var initialZobristValue = board.Zobrist;

            pieceMover.MakeMove(2, 65536, PieceType.Knight, SpecialMoveType.Normal); //Nb1-a3
            pieceMover.MakeMove(144115188075855872, 1099511627776, PieceType.Knight, SpecialMoveType.Normal); //Nb8-a6

            Assert.That(board.Zobrist, Is.Not.EqualTo(initialZobristValue));

            var twoMoveZobrist = board.Zobrist;

            pieceMover.MakeMove(64, 8388608, PieceType.Knight, SpecialMoveType.Normal); //Ng1-h3
            pieceMover.MakeMove(4611686018427387904, 140737488355328, PieceType.Knight, SpecialMoveType.Normal); //Ng8-h6

            Assert.That(board.Zobrist, Is.Not.EqualTo(initialZobristValue));


            pieceMover.MakeMove(8388608, 64, PieceType.Knight, SpecialMoveType.Normal); //Nh3-g1
            pieceMover.MakeMove(140737488355328, 4611686018427387904, PieceType.Knight, SpecialMoveType.Normal); //h6-g8

            Assert.That(board.Zobrist, Is.Not.EqualTo(initialZobristValue));
            Assert.That(board.Zobrist, Is.EqualTo(twoMoveZobrist));     // It should be back to the same as after two moves

            pieceMover.MakeMove(65536, 2, PieceType.Knight, SpecialMoveType.Normal); //a3-b1
            pieceMover.MakeMove(1099511627776, 144115188075855872, PieceType.Knight, SpecialMoveType.Normal); //h6-g8

            Assert.That(board.Zobrist, Is.EqualTo(initialZobristValue));    // IT should be back to the initial position
            Assert.That(board.Zobrist, Is.Not.EqualTo(twoMoveZobrist));
        }

        [Test]
        public void TwoGamesReachingSamePosition()
        {
            var startPosition = "rnbqkr2/pp3ppp/2p4n/3p4/1b1pP3/1PN2N2/PBPQ1PPP/R3KB1R b KQq - 0 1";
            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(startPosition);

            var board2 = new Board();
            board2.SetPosition(startPosition);

            Assert.That(board1.Zobrist, Is.EqualTo(board2.Zobrist));

            // Make board 1 moves
            var pieceMover = new PieceMover(board1);
            pieceMover.MakeMove(1073741824, 134217728, PieceType.Queen, SpecialMoveType.Normal); // Qg4-d4
            pieceMover.MakeMove(576460752303423488, 1152921504606846976, PieceType.Queen, SpecialMoveType.Normal); // QQd8-d7
            pieceMover.MakeMove(134217728, 9223372036854775808, PieceType.Queen, SpecialMoveType.Capture); // QQd4xh8

            Assert.That(board1.Zobrist, Is.Not.EqualTo(board2.Zobrist));

            //Make board 2 moves
            pieceMover = new PieceMover(board2);
            pieceMover.MakeMove(1073741824, 2147483648, PieceType.Queen, SpecialMoveType.Normal); // Qg4-h4
            pieceMover.MakeMove(576460752303423488, 1152921504606846976, PieceType.Queen, SpecialMoveType.Normal); // QQd8-d7
            pieceMover.MakeMove(2147483648, 9223372036854775808, PieceType.Queen, SpecialMoveType.Capture); // QQh4xh8

            Assert.That(board1.Zobrist, Is.EqualTo(board2.Zobrist));
        }
    }
}
