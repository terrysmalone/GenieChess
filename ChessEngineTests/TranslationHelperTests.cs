using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.NotationHelpers;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class TranslationHelperTests
    {
        [TestCase(281474976710656u, "a")]
        [TestCase(549755813888u, "h")]
        public void GetPieceLetter_Pawn(ulong pieceBitBoard, string expectedLetter)
        {
            var pieceLetter =  TranslationHelper.GetPieceLetter(PieceType.Pawn, pieceBitBoard);

            Assert.That(pieceLetter, Is.EqualTo(expectedLetter));
        }

        [Test]
        public void PieceLetter_Knight()
        {
            var pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Knight, 0);

            Assert.That(pieceLetter, Is.EqualTo("N"));
        }

        [Test]
        public void GetPieceLetter_Bishop()
        {
            var pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Bishop, 0);

            Assert.That(pieceLetter, Is.EqualTo("B"));
        }

        [Test]
        public void GetPieceLetter_Rook()
        {
            var pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Rook, 0);

            Assert.That(pieceLetter, Is.EqualTo("R"));
        }

        [Test]
        public void GetPieceLetter_Queen()
        {
            var pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Queen, 0);

            Assert.That(pieceLetter, Is.EqualTo("Q"));
        }

        [Test]
        public void GetPieceLetter_King()
        {
            var pieceLetter = TranslationHelper.GetPieceLetter(PieceType.King, 0);

            Assert.That(pieceLetter, Is.EqualTo("K"));
        }

        [TestCase(1u, "a1")]
        [TestCase(4294967296u, "a5")]
        [TestCase(262144u, "c3")]
        [TestCase(268435456u, "e4")]
        [TestCase(8388608u, "h3")]
        [TestCase(9223372036854775808u, "h8")]
        public void GetSquareNotation(ulong bitboard, string expectedString)
        {
            var square = TranslationHelper.GetSquareNotation(bitboard);
            Assert.That(square, Is.EqualTo(expectedString));
        }

        [TestCase(1u, "A1")]
        [TestCase(1u, "a1")]
        [TestCase(4294967296u, "A5")]
        [TestCase(4294967296u, "a5")]
        [TestCase(8388608u, "H3")]
        [TestCase(8388608u, "h3")]
        public void GetBitboard(ulong expectedBitboard, string notation)
        {
            var square = TranslationHelper.GetBitboard(notation);
            Assert.That(square, Is.EqualTo(expectedBitboard));
        }
    }
}
