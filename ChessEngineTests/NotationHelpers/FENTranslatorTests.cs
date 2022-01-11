using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests.NotationHelpers
{
    [TestFixture]
    public class FenTranslatorTests
    {
        [Test]
        public void TwoWayTranslate1()
        {
            LookupTables.InitialiseAllTables();

            var before = "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1";

            var boardState = FenTranslator.ToBoardState(before);

            var after = FenTranslator.ToFenString(boardState);

            Assert.That(after, Is.EqualTo(before));
        }

        [Test]
        public void TwoWayTranslate2()
        {
            LookupTables.InitialiseAllTables();

            var before = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq e3 12 10";

            var boardState = FenTranslator.ToBoardState(before);

            var after = FenTranslator.ToFenString(boardState);

            Assert.That(after, Is.EqualTo(before));
        }

        [Test]
        public void ToBoardState_WhiteToMove()
        {
            var state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/8 w - - 0 1");
            Assert.That(state.WhiteToMove, Is.True);
        }

        [Test]
        public void ToBoardState_BlackToMove()
        {
            var state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/8 b - - 0 1");
            Assert.That(state.WhiteToMove, Is.False);
        }

        [Test]
        public void ToBoardState_WhiteCanCastleKingside()
        {
            var state = FenTranslator.ToBoardState("8/3p4/2pPp3/5p2/5P2/4P3/PP5p/4K2R w K - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.True);
            Assert.That(state.WhiteCanCastleQueenside, Is.False);
            Assert.That(state.BlackCanCastleKingside, Is.False);
            Assert.That(state.BlackCanCastleQueenside, Is.False);
        }

        [Test]
        public void ToBoardState_WhiteCanCastleQueenside()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w Q - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.False);
            Assert.That(state.WhiteCanCastleQueenside, Is.True);
            Assert.That(state.BlackCanCastleKingside, Is.False);
            Assert.That(state.BlackCanCastleQueenside, Is.False);
        }

        [Test]
        public void ToBoardState_BlackCanCastleKingside()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w k - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.False);
            Assert.That(state.WhiteCanCastleQueenside, Is.False);
            Assert.That(state.BlackCanCastleKingside, Is.True);
            Assert.That(state.BlackCanCastleQueenside, Is.False);
        }

        [Test]
        public void ToBoardState_BlackCanCastleQueenside()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w q - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.False);
            Assert.That(state.WhiteCanCastleQueenside, Is.False);
            Assert.That(state.BlackCanCastleKingside, Is.False);
            Assert.That(state.BlackCanCastleQueenside, Is.True);
        }

        [Test]
        public void ToBoardState_AllCanCastle()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w KQkq - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.True);
            Assert.That(state.WhiteCanCastleQueenside, Is.True);
            Assert.That(state.BlackCanCastleKingside, Is.True);
            Assert.That(state.BlackCanCastleQueenside, Is.True);
        }

        [Test]
        public void ToBoardState_AllCantCastle()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.That(state.WhiteCanCastleKingside, Is.False);
            Assert.That(state.WhiteCanCastleQueenside, Is.False);
            Assert.That(state.BlackCanCastleKingside, Is.False);
            Assert.That(state.BlackCanCastleQueenside, Is.False);
        }


        [Test]
        public void ToBoardState_EnPassant_White()
        {
            var state = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 1 1");

            Assert.That(state.EnPassantPosition, Is.EqualTo(1048576));
        }

        [Test]
        public void ToBoardState_EnPassant_Black()
        {
            var state = FenTranslator.ToBoardState("rnbqkbnr/p1pppppp/8/1p6/4P3/8/PPPP1PPP/RNBQKBNR w KQkq b6 0 2");

            Assert.That(state.EnPassantPosition, Is.EqualTo(2199023255552));
        }

        [Test]
        public void ToBoardState_HalfMoveCount()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.That(state.HalfMoveClock, Is.EqualTo(0));

            state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 6 1");
            Assert.That(state.HalfMoveClock, Is.EqualTo(6));
        }

        [Test]
        public void ToBoardState_FullMoveCount()
        {
            var state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.That(state.FullMoveClock, Is.EqualTo(1));

            state = FenTranslator.ToBoardState("8/8/8/8/8/8/8/R3K3 w - - 6 43");
            Assert.That(state.FullMoveClock, Is.EqualTo(43));
        }

        [Test]
        public void ToBoardState_WhitePawns()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("rq3k2/2pp4/1pb5/p7/2P5/3PN3/4PP2/R3K3 w Q - 0 1");

            Assert.That(state.WhitePawns, Is.EqualTo(67645440));
        }

        [Test]
        public void ToBoardState_WhiteKnights1()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");

            Assert.That(state.WhiteKnights, Is.EqualTo(66));
        }

        [Test]
        public void ToBoardState_WhiteKnights2()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4P3/2N5/PPPP1PPP/R1BQKB1R w KQkq - 0 1");

            Assert.That(state.WhiteKnights, Is.EqualTo(68719738880));
        }

        [Test]
        public void ToBoardState_WhiteBishops1()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4P3/2N5/PPPP1PPP/R1BQKB1R w KQkq - 0 1");

            Assert.That(state.WhiteBishops, Is.EqualTo(36));
        }

        [Test]
        public void ToBoardState_WhiteBishops2()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2QK2R w KQkq - 0 1");

            Assert.That(state.WhiteBishops, Is.EqualTo(536875008));
        }

        [Test]
        public void ToBoardState_WhiteRooks()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2Q1RK1 w kq - 0 1");

            Assert.That(state.WhiteRooks, Is.EqualTo(33));
        }

        [Test]
        public void ToBoardState_WhiteQueen1()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPP1BPPP/R2Q1RK1 w kq - 0 1");

            Assert.That(state.WhiteQueen, Is.EqualTo(8));
        }

        [Test]
        public void ToBoardState_WhiteQueen2()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            Assert.That(state.WhiteQueen, Is.EqualTo(2048));
        }

        [Test]
        public void ToBoardState_WhiteKing()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            Assert.That(state.WhiteKing, Is.EqualTo(64));
        }

        [Test]
        public void ToBoardState_BlackPawns1()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pppppppp/7n/n3N3/4PB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            Assert.That(state.BlackPawns, Is.EqualTo(71776119061217280));
        }

        [Test]
        public void ToBoardState_BlackPawns2()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            Assert.That(state.BlackPawns, Is.EqualTo(45955188128743424));
        }

        [Test]
        public void ToBoardState_BlackKnights()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqkb1r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/PPPQBPPP/R4RK1 w kq - 0 1");

            Assert.That(state.BlackKnights, Is.EqualTo(140741783322624));
        }

        [Test]
        public void ToBoardState_BlackBishops()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("r1bqk2r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/P1PQBPPP/b4RK1 w kq - 0 1");

            Assert.That(state.BlackBishops, Is.EqualTo(288230376151711745));
        }

        [Test]
        public void ToBoardState_BlackRooks()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("1rbqk2r/pp3p1p/2p3pn/n3N3/3pPB2/2NP4/P1PQBPPP/b4RK1 w k - 0 1");

            Assert.That(state.BlackRooks, Is.EqualTo(9367487224930631680));
        }

        [Test]
        public void ToBoardState_BlackQueen()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("1rb1k2r/pp3p1p/2p3pn/n3N3/3pPB1q/2NP4/P1PQBPPP/b4RK1 w k - 0 1");

            Assert.That(state.BlackQueen, Is.EqualTo(2147483648));
        }

        [Test]
        public void ToBoardState_BlackKing()
        {
            LookupTables.InitialiseAllTables();

            var state = FenTranslator.ToBoardState("1rb3kr/pp3p1p/2p3pn/n3N3/3pPB1q/2NP4/P1PQBPPP/b4RK1 w - - 0 1");

            Assert.That(state.BlackKing, Is.EqualTo(4611686018427387904));
        }

        [Test]
        public void ToFen_WithEnPassantForWhite()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceMover = new PieceMover(board);
            pieceMover.MakeMove(new PieceMove() { Position = 4096, Moves = 268435456, SpecialMove = SpecialMoveType.DoublePawnPush, Type = PieceType.Pawn }, true);

            var boardString = FenTranslator.ToFenString(board.GetCurrentBoardState());

            Assert.That(boardString, Is.EqualTo("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 1 1"));
        }
    }
}
