using System.Collections.Generic;
using ChessEngine;
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

            Assert.That(zobrist, Is.Not.EqualTo(0));

            // Remove knight from
            zobrist ^= ZobristKey.PiecePositions[ZobristHash.GetPieceValue(PieceType.Knight, true), BitboardOperations.GetSquareIndexFromBoardValue(1048576)];

            Assert.That(zobrist, Is.EqualTo(0));
        }

        [Test]
        public void MovingPiecesBackToStartGivesSameScore()
        {
            ZobristHash.Reset();
            ZobristHash.Initialise();

            var board = new Board();
            board.SetPosition("3nk3/8/8/8/8/8/8/3NK3 w - - 0 1");

            var pieceMover = new PieceMover(board);

            var initialZobrist = board.Zobrist;

            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d1f2", board));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d8f7", board));

            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("f2d1", board));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("f7d8", board));

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
                "r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq c6 0 2");

            var initialZobristValue1 = board1.Zobrist;
            Assert.That(initialZobristValue1, Is.Not.EqualTo((ulong) 0));

            var board2 = new Board();
            board2.SetPosition(
                "r3k2r/p2pqpb1/bn2pnp1/2pPN3/1p2P3/2N1BQ1p/PPP1BPPP/R3K2R w KQkq - 0 2");

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
            var pieceMover = new PieceMover(board1);
            
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e3e4", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d5d6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d2d4", board1));

            var zobrist1 = board1.Zobrist;
            Assert.That(zobrist1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            pieceMover = new PieceMover(board2);

            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e4", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d5d6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d3d4", board1));

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
            var pieceMover = new PieceMover(board1);

            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d7d6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e4", board1));   //En-passant move
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board1));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board1));

            var zobrist1 = board1.Zobrist;
            Assert.That(zobrist1, Is.Not.EqualTo((ulong)0));

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            pieceMover = new PieceMover(board2);

            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d7d6", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("c8e6", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e3e4", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e6d7", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e7e6", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("f3g1", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d7c8", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("g1f3", board2));

            var zobrist2 = board2.Zobrist;
            Assert.That(zobrist2, Is.Not.EqualTo((ulong)0));

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }

        [Test]
        public void CalculateZobristReachesSameConclusionAsUpdateZobrist_SwitchingSides()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                
                    "2kr3r/pp1qppbp/2p2np1/2np1b2/8/BPNPPP1N/P1PQBKPP/R6R w - - 3 11");

            var zobrist1 = board1.Zobrist;

            var board2 = new Board();
            board2.SetPosition(
                
                    "2kr3r/pp1qppbp/2p2np1/2np1b2/8/BPNPPP1N/P1PQBKPP/R6R b - - 3 11");

            board2.SwitchSides();

            var zobrist2 = board2.Zobrist;

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }

        [Test]
        public void CalculateZobristReachesSameConclusionAsUpdateZobrist_SimplePawnMove()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                                   "rnbqkbnr/pppppppp/8/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1");   // After e2e3

            var zobrist1 = board1.Zobrist;

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            var pieceMover = new PieceMover(board2);
            
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board2));
            var zobrist2 = board2.Zobrist;

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }

        [Test]
        public void CalculateZobristReachesSameConclusionAsUpdateZobrist_TwoSimplePawnMoves()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                
                    "rnbqkbnr/pp1ppppp/2p5/8/8/4P3/PPPP1PPP/RNBQKBNR w KQkq - 0 2"); // After 1.e2e3 c7c6

            var zobrist1 = board1.Zobrist;

            var board2 = new Board();
            board2.InitaliseStartingPosition();
            var pieceMover = new PieceMover(board2);
            
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("c7c6", board2));
            var zobrist2 = board2.Zobrist;

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }
        
        [Test]
        public void CalculateZobristReachesSameConclusionAsUpdateZobrist_ThreeSimplePawnMoves()
        {
            ZobristHash.Reset();

            ZobristHash.Initialise();

            var board1 = new Board();
            board1.SetPosition(
                
                    "rnbqkbnr/pp1ppppp/2p5/8/8/3PP3/PPP2PPP/RNBQKBNR b KQkq - 0 2"); // After 1.e2e3 c7c6 2.d2d3
            
            var zobrist1 = board1.Zobrist;

            var board2 = new Board();
            board2.InitaliseStartingPosition();

            var pieceMover = new PieceMover(board2);
            
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("e2e3", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("c7c6", board2));
            pieceMover.MakeMove(UciMoveTranslator.ToGameMove("d2d3", board2));
            var zobrist2 = board2.Zobrist;

            Assert.That(zobrist1, Is.EqualTo(zobrist2));
        }

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
