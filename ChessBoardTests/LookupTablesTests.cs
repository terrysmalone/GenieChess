using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardSearching;

namespace ChessBoardTests
{
    [TestClass]
    public class LookupTablesTests
    {
        //[TestMethod]
        //public void TestSquareValueFromIndexTable()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestSquareValueFromPositionTable()
        //{
        //    throw new NotImplementedException();
        //}

        [TestMethod]
        public void TestUpDirectionBoards()
        {
            Assert.AreEqual((ulong)578721382704613376, LookupTables.UpBoard[3]);     //d1
            Assert.AreEqual((ulong)72340172838010880, LookupTables.UpBoard[16]);     //a3
            Assert.AreEqual((ulong)0, LookupTables.UpBoard[61]);     //f8
            Assert.AreEqual((ulong)9259541571362095104, LookupTables.UpBoard[39]);     //h5
        }

        [TestMethod]
        public void TestRightDirectionBoards()
        {
            Assert.AreEqual((ulong)16252928, LookupTables.RightBoard[18]);     //c3
        }

        //[TestMethod]
        //public void TestDownDirectionBoards()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestLeftDirectionBoards()
        //{
        //    throw new NotImplementedException();
        //}

        #region mask tests

        //[TestMethod]
        //public void TestFileMasks()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestRankMasks()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestFileMasksByColumn()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestFileMasksByIndex()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
