using System;
using ChessEngine.BoardSearching;
using NUnit.Framework;

namespace ChessEngineTests
{
    [TestFixture]
    public class LookupTablesTests
    {
        //[Test]
        //public void TestSquareValueFromIndexTable()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void TestSquareValueFromPositionTable()
        //{
        //    throw new NotImplementedException();
        //}

        [TestCase(0, 72340172838076672u)]    // a1
        [TestCase(7, 9259542123273814016u)]  // h1
        [TestCase(56, 0u)]                   // a8
        [TestCase(63, 0u)]                   // h8
        [TestCase(3, 578721382704613376u)]   // d1
        [TestCase(16, 72340172838010880u)]   // a3
        [TestCase(61, 0u)]                   // f8
        [TestCase(39, 9259541571362095104)]  // h5
        public void UpDirectionBoards(int boardPosition, ulong expectedupBoard)
        {
            LookupTables.InitialiseAllTables();

            Assert.That(LookupTables.UpBoard[boardPosition], Is.EqualTo(expectedupBoard));
        }

        [TestCase(0, 0u)]                     // a1
        [TestCase(7, 0u)]                     // h1
        [TestCase(56, 282578800148737u)]      // a8
        [TestCase(63, 36170086419038336u)]    // h8
        [TestCase(18, 1028u)]                 // c3
        [TestCase(39, 2155905152u)]           // h5
        [TestCase(20, 4112u)]                 // e3
        public void DownDirectionBoards(int boardPosition, ulong expectedDownBoard)
        {
            LookupTables.InitialiseAllTables();

            Assert.That(LookupTables.DownBoard[boardPosition], Is.EqualTo(expectedDownBoard));
        }

        [TestCase(0, 254u)]                   // a1
        [TestCase(7, 0u)]                     // h1
        [TestCase(56, 18302628885633695744u)] // a8
        [TestCase(63, 0u)]                    // h8
        [TestCase(18, 16252928u)]             // c3
        [TestCase(39, 0u)]                    // h5
        [TestCase(20, 14680064u)]             // e3
        public void RightDirectionBoards(int boardPosition, ulong expectedRightBoard)
        {
            LookupTables.InitialiseAllTables();

            Assert.That(LookupTables.RightBoard[boardPosition], Is.EqualTo(expectedRightBoard));
        }

        [TestCase(0, 0u)]                    // a1
        [TestCase(7, 127u)]                  // h1
        [TestCase(56, 0u)]                   // a8
        [TestCase(63, 9151314442816847872u)] // h8
        [TestCase(18, 196608u)]              // c3
        [TestCase(39, 545460846592u)]        // h5
        [TestCase(20, 983040u)]              // e3
        public void LeftDirectionBoards(int boardPosition, ulong expectedLeftBoard)
        {
            LookupTables.InitialiseAllTables();

            Assert.That(LookupTables.LeftBoard[boardPosition], Is.EqualTo(expectedLeftBoard));
        }

        #region mask tests

        [Test]
        public void FileMasks()
        {
            throw new NotImplementedException();
        }

        //[Test]
        //public void TestRankMasks()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void TestFileMasksByColumn()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void TestFileMasksByIndex()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
