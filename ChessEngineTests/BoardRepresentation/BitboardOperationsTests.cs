using ChessEngine.BoardRepresentation;
using NUnit.Framework;

namespace ChessEngineTests.BoardRepresentation
{
    [TestFixture]
    public class BitboardOperationsTests
    {
        [Test]
        public void TestFlipVertical_1()
        {
            ulong before = 16;
            
            var after = BitboardOperations.FlipVertical(before);

            Assert.That(after, Is.EqualTo((ulong)1152921504606846976));

        }

        [Test]
        public void TestFlipVertical_2()
        {
            ulong before = 137707913216;

            var after = BitboardOperations.FlipVertical(before);

            Assert.That(after, Is.EqualTo((ulong)8865349369856));
        }

        [Test]
        public void TestFlipVertical_3()
        {
            ulong before = 2160124144;

            var after = BitboardOperations.FlipVertical(before);

            Assert.That(after, Is.EqualTo(17357084619874238464));
        }

        [Test]
        public void TestPopCount()
        {
            Assert.That(BitboardOperations.GetPopCount(0), Is.EqualTo(0));
            Assert.That(BitboardOperations.GetPopCount(3298601992192), Is.EqualTo(3));
            Assert.That(BitboardOperations.GetPopCount(217018310850510848), Is.EqualTo(6));
            Assert.That(BitboardOperations.GetPopCount(13835058055315849731), Is.EqualTo(7));
            Assert.That(BitboardOperations.GetPopCount(848857606930560), Is.EqualTo(8));
        }

        [Test]
        public void TestGetSquareIndexFromBoardValue()
        {
            Assert.That(BitboardOperations.GetSquareIndexFromBoardValue(1), Is.EqualTo(0));
            Assert.That(BitboardOperations.GetSquareIndexFromBoardValue(1024), Is.EqualTo(10));
            Assert.That(BitboardOperations.GetSquareIndexFromBoardValue(2097152), Is.EqualTo(21));
            Assert.That(BitboardOperations.GetSquareIndexFromBoardValue(9223372036854775808), Is.EqualTo(63));
        }

        [Test]
        public void TestGetSquareIndexesFromBoardValue()
        {
            //byte[] returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(16140901064495857664);
            var returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(16140901064495857664);


            Assert.That(returnedIndexes.Count, Is.EqualTo(3));
            Assert.That(returnedIndexes[0], Is.EqualTo(61));
            Assert.That(returnedIndexes[1], Is.EqualTo(62));
            Assert.That(returnedIndexes[2], Is.EqualTo(63));

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(65793);
            Assert.That(returnedIndexes.Count, Is.EqualTo(3));
            Assert.That(returnedIndexes[0], Is.EqualTo(0));
            Assert.That(returnedIndexes[1], Is.EqualTo(8));
            Assert.That(returnedIndexes[2], Is.EqualTo(16));

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(0);
            Assert.That(returnedIndexes.Count, Is.EqualTo(0));

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(17179869184);
            Assert.That(returnedIndexes.Count, Is.EqualTo(1));
            Assert.That(returnedIndexes[0], Is.EqualTo(34));
        }

        [Test]
        public void TestSplitBoard()
        {
            var split = BitboardOperations.SplitBoardToArray(17626747109376);

            Assert.That(split.Length, Is.EqualTo(4));

            Assert.That(split[0], Is.EqualTo((ulong)67108864));
            Assert.That(split[1], Is.EqualTo((ulong)134217728));
            Assert.That(split[2], Is.EqualTo((ulong)34359738368));
            Assert.That(split[3], Is.EqualTo((ulong)17592186044416));
        }
    }
}
