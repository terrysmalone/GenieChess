using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardRepresentation;

namespace ChessBoardTests
{
    /// <summary>
    /// Summary description for BitboardOperationsTests
    /// </summary>
    [TestClass]
    public class BitboardOperationsTests
    {
        public BitboardOperationsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestFlipVertical_1()
        {
            ulong before = 16;
            
            ulong after = BitboardOperations.FlipVertical(before);

            Assert.AreEqual((ulong)1152921504606846976, after);

        }

        [TestMethod]
        public void TestFlipVertical_2()
        {
            ulong before = 137707913216;

            ulong after = BitboardOperations.FlipVertical(before);

            Assert.AreEqual((ulong)8865349369856, after);

        }

        [TestMethod]
        public void TestFlipVertical_3()
        {
            ulong before = 2160124144;

            ulong after = BitboardOperations.FlipVertical(before);

            Assert.AreEqual((ulong)17357084619874238464, after);

        }

        [TestMethod]
        public void TestPopCount()
        {
            Assert.AreEqual(0, BitboardOperations.GetPopCount(0));
            Assert.AreEqual(3, BitboardOperations.GetPopCount(3298601992192));
            Assert.AreEqual(6, BitboardOperations.GetPopCount(217018310850510848));
            Assert.AreEqual(7, BitboardOperations.GetPopCount(13835058055315849731));
            Assert.AreEqual(8, BitboardOperations.GetPopCount(848857606930560));
        }

        [TestMethod]
        public void TestGetSquareIndexFromBoardValue()
        {
            Assert.AreEqual(0, BitboardOperations.GetSquareIndexFromBoardValue(1));
            Assert.AreEqual(10, BitboardOperations.GetSquareIndexFromBoardValue(1024));
            Assert.AreEqual(21, BitboardOperations.GetSquareIndexFromBoardValue(2097152));
            Assert.AreEqual(63, BitboardOperations.GetSquareIndexFromBoardValue(9223372036854775808));
        }

        [TestMethod]
        public void TestGetSquareIndexFromBoardValueSpeed()
        {
            Random rand = new Random();

            DateTime start = DateTime.UtcNow;

            for (int i = 0; i < 4000000; i++)
            {
                byte position = BitboardOperations.GetSquareIndexFromBoardValue((ulong)Math.Pow(1, rand.Next(64)));
            }

            DateTime end = DateTime.UtcNow;

            TimeSpan diff = end - start;

            Console.WriteLine(string.Format("New:{0}", diff));

            start = DateTime.UtcNow;

            for (int i = 0; i < 4000000; i++)
            {
                byte position = BitboardOperations.GetSquareIndexFromBoardValueOld((ulong)Math.Pow(1, rand.Next(64)));
            }

            end = DateTime.UtcNow;

            diff = end - start;

            Console.WriteLine(string.Format("Old:{0}", diff));
        }

        [TestMethod]
        public void TestGetSquareIndexesFromBoardValue()
        {
            //byte[] returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(16140901064495857664);
            List<byte> returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(16140901064495857664);


            Assert.AreEqual(3, returnedIndexes.Count);
            Assert.AreEqual(61, returnedIndexes[0]);
            Assert.AreEqual(62, returnedIndexes[1]);
            Assert.AreEqual(63, returnedIndexes[2]);

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(65793);
            Assert.AreEqual(3, returnedIndexes.Count);
            Assert.AreEqual(0, returnedIndexes[0]);
            Assert.AreEqual(8, returnedIndexes[1]);
            Assert.AreEqual(16, returnedIndexes[2]);

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(0);
            Assert.AreEqual(0, returnedIndexes.Count);

            //
            returnedIndexes = BitboardOperations.GetSquareIndexesFromBoardValue(17179869184);
            Assert.AreEqual(1, returnedIndexes.Count);
            Assert.AreEqual(34, returnedIndexes[0]);
        }

        [TestMethod]
        public void TestGetSquareIndexesFromBoardValueSpeed()
        {
            Random rand = new Random();
            DateTime start = DateTime.UtcNow;

            for (int i = 0; i < 1000000; i++)
            {
                List<byte> positions = BitboardOperations.GetSquareIndexesFromBoardValue((ulong)Math.Pow(1, rand.Nextulong()));
            }

            DateTime end = DateTime.UtcNow;

            TimeSpan diff = end - start;

            Console.WriteLine(string.Format("New:{0}", diff));
        }

        [TestMethod]
        public void TestSplitBoard()
        {
            ulong[] split = BitboardOperations.SplitBoardToArray(17626747109376);

            Assert.AreEqual(4, split.Length);

            Assert.AreEqual((ulong)67108864, split[0]);
            Assert.AreEqual((ulong)134217728, split[1]);
            Assert.AreEqual((ulong)34359738368, split[2]);
            Assert.AreEqual((ulong)17592186044416, split[3]);
        }

        [TestMethod]
        private void TestFastestPopCount()
        {
            Random rand = new Random();
            DateTime start = DateTime.UtcNow;

            for (int i = 0; i < 10000000; i++)
            {
                List<byte> positions = BitboardOperations.GetSquareIndexesFromBoardValue((ulong)Math.Pow(1, rand.Nextulong()));
                //positions.Count();
            }

            DateTime end = DateTime.UtcNow;

            TimeSpan diff = end - start;

            Console.WriteLine(string.Format("Finding bits:{0}", diff));

            rand = new Random();

            start = DateTime.UtcNow;

            for (int i = 0; i < 10000000; i++)
            {
                byte popCount = BitboardOperations.GetPopCount(rand.Nextulong());
            }

            end = DateTime.UtcNow;

            diff = end - start;

            Console.WriteLine(string.Format("PopCount:{0}", diff));
        }
    }
}
