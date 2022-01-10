using ChessEngine.BoardRepresentation;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void InitialPieceBitboardsAreCorrect()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            Assert.That(board.WhitePawns, Is.EqualTo((ulong)65280));
            Assert.That(board.WhiteKnights, Is.EqualTo((ulong)66));
            Assert.That(board.WhiteBishops, Is.EqualTo((ulong)36));
            Assert.That(board.WhiteRooks, Is.EqualTo((ulong)129));
            Assert.That(board.WhiteQueen, Is.EqualTo((ulong)8));
            Assert.That(board.WhiteKing, Is.EqualTo((ulong)16));

            Assert.That(board.BlackPawns, Is.EqualTo((ulong)71776119061217280));
            Assert.That(board.BlackKnights, Is.EqualTo((ulong)4755801206503243776));
            Assert.That(board.BlackBishops, Is.EqualTo((ulong)2594073385365405696));
            Assert.That(board.BlackRooks, Is.EqualTo(9295429630892703744));
            Assert.That(board.BlackQueen, Is.EqualTo((ulong)576460752303423488));
            Assert.That(board.BlackKing, Is.EqualTo((ulong)1152921504606846976));

            Assert.That(board.WhiteToMove, Is.True);

            Assert.That(board.BlackCanCastleKingside, Is.True);
            Assert.That(board.BlackCanCastleQueenside, Is.True);
            Assert.That(board.WhiteCanCastleKingside, Is.True);
            Assert.That(board.WhiteCanCastleQueenside, Is.True);
        }

        [Test]
        public void InitialUsefulBitboardsAreCorrect()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            Assert.That(board.AllWhiteOccupiedSquares, Is.EqualTo((ulong)65535));
            Assert.That(board.AllBlackOccupiedSquares, Is.EqualTo(18446462598732840960));
            Assert.That(board.AllOccupiedSquares, Is.EqualTo(18446462598732906495));
            Assert.That(board.EmptySquares, Is.EqualTo(281474976645120));
            Assert.That(board.WhiteOrEmpty, Is.EqualTo(281474976710655));
            Assert.That(board.BlackOrEmpty, Is.EqualTo(18446744073709486080));
        }

        [Test]
        public void SetPosition_Initial()
        {
            var board = new Board();
            board.SetPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
             
            Assert.That(board.WhitePawns, Is.EqualTo((ulong)65280));
            Assert.That(board.WhiteKnights, Is.EqualTo((ulong)66));
            Assert.That(board.WhiteBishops, Is.EqualTo((ulong)36));
            Assert.That(board.WhiteRooks, Is.EqualTo((ulong)129));
            Assert.That(board.WhiteQueen, Is.EqualTo((ulong)8));
            Assert.That(board.WhiteKing, Is.EqualTo((ulong)16));

            Assert.That(board.BlackPawns, Is.EqualTo((ulong)71776119061217280));
            Assert.That(board.BlackKnights, Is.EqualTo((ulong)4755801206503243776));
            Assert.That(board.BlackBishops, Is.EqualTo((ulong)2594073385365405696));
            Assert.That(board.BlackRooks, Is.EqualTo(9295429630892703744));
            Assert.That(board.BlackQueen, Is.EqualTo((ulong)576460752303423488));
            Assert.That(board.BlackKing, Is.EqualTo((ulong)1152921504606846976));

            Assert.That(board.WhiteToMove, Is.True);

            Assert.That(board.BlackCanCastleKingside, Is.True);
            Assert.That(board.BlackCanCastleQueenside, Is.True);
            Assert.That(board.WhiteCanCastleKingside, Is.True);
            Assert.That(board.WhiteCanCastleQueenside, Is.True);

            Assert.That(board.AllWhiteOccupiedSquares, Is.EqualTo((ulong)65535));
            Assert.That(board.AllBlackOccupiedSquares, Is.EqualTo(18446462598732840960));
            Assert.That(board.AllOccupiedSquares, Is.EqualTo(18446462598732906495));
            Assert.That(board.EmptySquares, Is.EqualTo(281474976645120));
            Assert.That(board.WhiteOrEmpty, Is.EqualTo(281474976710655));
            Assert.That(board.BlackOrEmpty, Is.EqualTo(18446744073709486080));
        }

        [Test]
        public void SetPosition2()
        {
            var board = new Board();
            board.SetPosition("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -");

            Assert.That(board.WhitePawns, Is.EqualTo((ulong)8594161664));
            Assert.That(board.WhiteKnights, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteBishops, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteRooks, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteQueen, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteKing, Is.EqualTo((ulong)16777216));

            Assert.That(board.BlackPawns, Is.EqualTo((ulong)281476050452480));
            Assert.That(board.BlackKnights, Is.EqualTo((ulong)0));
            Assert.That(board.BlackBishops, Is.EqualTo((ulong)0));
            Assert.That(board.BlackRooks, Is.EqualTo((ulong)0));
            Assert.That(board.BlackQueen, Is.EqualTo((ulong)0));
            Assert.That(board.BlackKing, Is.EqualTo((ulong)67108864));

            Assert.That(board.WhiteToMove, Is.True);

            Assert.That(board.WhiteCanCastleKingside, Is.False);
            Assert.That(board.WhiteCanCastleQueenside, Is.False);
            Assert.That(board.BlackCanCastleKingside, Is.False);
            Assert.That(board.BlackCanCastleQueenside, Is.False);

            Assert.That(board.EnPassantPosition, Is.EqualTo((ulong)0));
        }

        [Test]
        public void ClearBoard()
        {
            var board = new Board();

            board.InitaliseStartingPosition();

            Assert.That(board.AllOccupiedSquares, Is.Not.EqualTo((ulong)0));
            Assert.That(board.AllWhiteOccupiedSquares, Is.Not.EqualTo((ulong)0));
            Assert.That(board.AllBlackOccupiedSquares, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackOrEmpty, Is.Not.EqualTo(18446744073709551615));
            Assert.That(board.WhiteOrEmpty, Is.Not.EqualTo(18446744073709551615));
            Assert.That(board.EmptySquares, Is.Not.EqualTo(18446744073709551615));

            Assert.That(board.WhitePawns, Is.Not.EqualTo((ulong)0));
            Assert.That(board.WhiteQueen, Is.Not.EqualTo((ulong)0));
            Assert.That(board.WhiteKing, Is.Not.EqualTo((ulong)0));
            Assert.That(board.WhiteBishops, Is.Not.EqualTo((ulong)0));
            Assert.That(board.WhiteKnights, Is.Not.EqualTo((ulong)0));
            Assert.That(board.WhiteRooks, Is.Not.EqualTo((ulong)0));

            Assert.That(board.BlackPawns, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackQueen, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackKing, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackBishops, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackKnights, Is.Not.EqualTo((ulong)0));
            Assert.That(board.BlackRooks, Is.Not.EqualTo((ulong)0));
            
            board.ClearBoard();

            Assert.That(board.AllOccupiedSquares, Is.EqualTo((ulong)0));
            Assert.That(board.AllWhiteOccupiedSquares, Is.EqualTo((ulong)0));
            Assert.That(board.AllBlackOccupiedSquares, Is.EqualTo((ulong)0));
            Assert.That(board.BlackOrEmpty, Is.EqualTo(18446744073709551615));
            Assert.That(board.WhiteOrEmpty, Is.EqualTo(18446744073709551615));
            Assert.That(board.EmptySquares, Is.EqualTo(18446744073709551615));

            Assert.That(board.WhitePawns, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteQueen, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteKing, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteBishops, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteKnights, Is.EqualTo((ulong)0));
            Assert.That(board.WhiteRooks, Is.EqualTo((ulong)0));

            Assert.That(board.BlackPawns, Is.EqualTo((ulong)0));
            Assert.That(board.BlackQueen, Is.EqualTo((ulong)0));
            Assert.That(board.BlackKing, Is.EqualTo((ulong)0));
            Assert.That(board.BlackBishops, Is.EqualTo((ulong)0));
            Assert.That(board.BlackKnights, Is.EqualTo((ulong)0));
            Assert.That(board.BlackRooks, Is.EqualTo((ulong)0));
        }

        [Test]
        public void MoveColour()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            Assert.That(board.WhiteToMove, Is.True);

            board.SwitchSides();
            Assert.That(board.WhiteToMove, Is.False);


        }

        [Test]
        public void KingInCheckFlags()
        {
            // TODO: write tests
            //throw new NotImplementedException();
        }
    }
}
