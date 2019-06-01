using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardSearching;


namespace ChessEngineTests
{
    [TestClass]
    public class TranslationHelperTests
    {
        #region GetPieceLetter tests

        [TestMethod]
        public void TestGetPieceLetter_pawn_1()
        {
            string pieceLetter =  TranslationHelper.GetPieceLetter(PieceType.Pawn, 281474976710656);

            Assert.AreEqual("a", pieceLetter);
        }

        [TestMethod]
        public void TestGetPieceLetter_pawn_2()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Pawn, 549755813888);

            Assert.AreEqual("h", pieceLetter);
        }

        [TestMethod]
        public void TestGetPieceLetter_Knight()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Knight, 0);

            Assert.AreEqual("N", pieceLetter);
        }

        [TestMethod]
        public void TestGetPieceLetter_Bishop()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Bishop, 0);

            Assert.AreEqual("B", pieceLetter);
        }

        [TestMethod]
        public void TestGetPieceLetter_Rook()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Rook, 0);

            Assert.AreEqual("R", pieceLetter); 
        }

        [TestMethod]
        public void TestGetPieceLetter_Queen()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.Queen, 0);

            Assert.AreEqual("Q", pieceLetter);
        }

        [TestMethod]
        public void TestGetPieceLetter_King()
        {
            string pieceLetter = TranslationHelper.GetPieceLetter(PieceType.King, 0);

            Assert.AreEqual("K", pieceLetter);
        }

        #endregion GetPieceLetter tests
        
        #region SquareBitboardToString tests

        [TestMethod]
        public void TestSquareBitboardToString_a1()
        {
            string square = TranslationHelper.SquareBitboardToSquareString((ulong)1);
            Assert.AreEqual("a1", square);

        }

        [TestMethod]
        public void TestSquareBitboardToString_e4()
        {
            string square = TranslationHelper.SquareBitboardToSquareString((ulong)268435456);
            Assert.AreEqual("e4", square);

        }

        [TestMethod]
        public void TestSquareBitboardToString_h8()
        {
            string square = TranslationHelper.SquareBitboardToSquareString((ulong)9223372036854775808);
            Assert.AreEqual("h8", square);
        }

        #endregion SquareBitboardToString tests

        #region Test BitboardFromSquareString

        [TestMethod]
        public void BitboardFromSquareString_11()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("a1");

            Assert.AreEqual((ulong)1, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_A1()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("A1");

            Assert.AreEqual((ulong)1, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_a5()
        {
            LookupTables.InitialiseAllTables();
            
            ulong result = TranslationHelper.BitboardFromSquareString("a5");

            Assert.AreEqual((ulong)4294967296, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_A5()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("A5");

            Assert.AreEqual((ulong)4294967296, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_h3()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("h3");

            Assert.AreEqual((ulong)8388608, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_H3()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("H3");

            Assert.AreEqual((ulong)8388608, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_c3()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("c3");

            Assert.AreEqual((ulong)262144, result);
        }

        [TestMethod]
        public void BitboardFromSquareString_C3()
        {
            LookupTables.InitialiseAllTables();

            ulong result = TranslationHelper.BitboardFromSquareString("C3");

            Assert.AreEqual((ulong)262144, result);
        }

        #endregion Test BitboardFromSquareString
    }
}
