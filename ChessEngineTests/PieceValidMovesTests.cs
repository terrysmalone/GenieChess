using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests
{
    [TestClass]
    public class PieceValidMovesTests
    {
        [TestMethod]
        public void TestKnightMoves()
        {
            PieceValidMoves.GenerateMoveArrays();

            var movesFromD5 = ValidMoveArrays.KnightMoves[35];

            Assert.AreEqual(LookupTables.F4, LookupTables.F4 & movesFromD5);
        }

        //#region Diagonal move tests

        //[TestMethod]
        //public void TestMoveDiagonalUpRight()
        //{            
        //    PieceValidMoves.GenerateMoveArrays();
               
        //    //Check from A1 - index 0
        //    int index = 0;
        //    Assert.AreEqual(7, ValidMoveArrays.BishopTotalMoves1[index]);  //Number of valid up right moves form A1

        //    PieceMoveSet[] bishopUpRightMoves = ValidMoveArrays.BishopMoves1;   //All bishop up right moves
        //    PieceMoveSet bishopUpRightMovesFromA1 = bishopUpRightMoves[index];     //All bishop up right moves from A1

        //    Assert.AreEqual(7, bishopUpRightMovesFromA1.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.B2, bishopUpRightMovesFromA1.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.C3, bishopUpRightMovesFromA1.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.D4, bishopUpRightMovesFromA1.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.E5, bishopUpRightMovesFromA1.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.F6, bishopUpRightMovesFromA1.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.G7, bishopUpRightMovesFromA1.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.H8, bishopUpRightMovesFromA1.Moves[6]);

        //    //Check from B4 - index 25
        //    index = 25;
        //    Assert.AreEqual(4, ValidMoveArrays.BishopTotalMoves1[index]);  //Number of valid up right moves form B4

        //    PieceMoveSet bishopUpRightMovesFromB4 = bishopUpRightMoves[index];     //All bishop up right moves from B4

        //    Assert.AreEqual(4, bishopUpRightMovesFromB4.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.C5, bishopUpRightMovesFromB4.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.D6, bishopUpRightMovesFromB4.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.E7, bishopUpRightMovesFromB4.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.F8, bishopUpRightMovesFromB4.Moves[3]);

        //    //Check from H3 - index 31
        //    index = 31;
        //    Assert.AreEqual(0, ValidMoveArrays.BishopTotalMoves1[index]);  //Number of valid up right moves form H3

        //    PieceMoveSet bishopUpRightMovesFromH3 = bishopUpRightMoves[index];     //All bishop up right moves from H3

        //    Assert.AreEqual(0, bishopUpRightMovesFromH3.Moves.Count);
        //}

        //[TestMethod]
        //public void TestMoveDiagonalDownRight()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from A8 - index 56
        //    int index = 56;
        //    Assert.AreEqual(7, ValidMoveArrays.BishopTotalMoves2[index]);  //Number of valid up right moves form A8

        //    PieceMoveSet[] bishopDownRightMoves = ValidMoveArrays.BishopMoves2;   //All bishop up right moves
        //    PieceMoveSet bishopDownRightMovesFromA8 = bishopDownRightMoves[index];     //All bishop down left moves from A8

        //    Assert.AreEqual(7, bishopDownRightMovesFromA8.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.B7, bishopDownRightMovesFromA8.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.C6, bishopDownRightMovesFromA8.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.D5, bishopDownRightMovesFromA8.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.E4, bishopDownRightMovesFromA8.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.F3, bishopDownRightMovesFromA8.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.G2, bishopDownRightMovesFromA8.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.H1, bishopDownRightMovesFromA8.Moves[6]);

        //    //Check from B4 - index 25
        //    index = 25;
        //    Assert.AreEqual(3, ValidMoveArrays.BishopTotalMoves2[index]);  //Number of valid down right moves form B4

        //    PieceMoveSet bishopDownRightMovesFromB4 = bishopDownRightMoves[index];     //All bishop down right moves from B4

        //    Assert.AreEqual(3, bishopDownRightMovesFromB4.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.C3, bishopDownRightMovesFromB4.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.D2, bishopDownRightMovesFromB4.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.E1, bishopDownRightMovesFromB4.Moves[2]);
        //}

        //[TestMethod]
        //public void TestMoveDiagonalDownLeft()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from H8 - index 63
        //    int index = 63;
        //    Assert.AreEqual(7, ValidMoveArrays.BishopTotalMoves3[index]);  //Number of valid up right moves form H8

        //    PieceMoveSet[] bishopDownLeftMoves = ValidMoveArrays.BishopMoves3;   //All bishop down left moves
        //    PieceMoveSet bishopDownLeftMovesFromH8 = bishopDownLeftMoves[index];     //All bishop up right moves from A8

        //    Assert.AreEqual(7, bishopDownLeftMovesFromH8.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.G7, bishopDownLeftMovesFromH8.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.F6, bishopDownLeftMovesFromH8.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.E5, bishopDownLeftMovesFromH8.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.D4, bishopDownLeftMovesFromH8.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.C3, bishopDownLeftMovesFromH8.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.B2, bishopDownLeftMovesFromH8.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.A1, bishopDownLeftMovesFromH8.Moves[6]);

        //    //Check from D5 - index 35
        //    index = 35;
        //    Assert.AreEqual(3, ValidMoveArrays.BishopTotalMoves3[index]);  //Number of valid down left moves form D5

        //    PieceMoveSet bishopDownLeftMovesFromB4 = bishopDownLeftMoves[index];     //All bishop down left moves from D5

        //    Assert.AreEqual(3, bishopDownLeftMovesFromB4.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.C4, bishopDownLeftMovesFromB4.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.B3, bishopDownLeftMovesFromB4.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.A2, bishopDownLeftMovesFromB4.Moves[2]);
        //}

        //[TestMethod]
        //public void TestMoveDiagonalUpLeft()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from H1 - index 7
        //    int index = 7;
        //    Assert.AreEqual(7, ValidMoveArrays.BishopTotalMoves4[index]);  //Number of valid up left moves form H1

        //    PieceMoveSet[] bishopUpLeftMoves = ValidMoveArrays.BishopMoves4;   //All bishop up left moves
        //    PieceMoveSet bishopUpLeftMovesFromH1 = bishopUpLeftMoves[index];     //All bishop up left moves from H1

        //    Assert.AreEqual(7, bishopUpLeftMovesFromH1.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.G2, bishopUpLeftMovesFromH1.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.F3, bishopUpLeftMovesFromH1.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.E4, bishopUpLeftMovesFromH1.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.D5, bishopUpLeftMovesFromH1.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.C6, bishopUpLeftMovesFromH1.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.B7, bishopUpLeftMovesFromH1.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.A8, bishopUpLeftMovesFromH1.Moves[6]);

        //    //Check from E6 - index 44
        //    index = 44;
        //    Assert.AreEqual(2, ValidMoveArrays.BishopTotalMoves4[index]);  //Number of valid up left moves form E6

        //    PieceMoveSet bishopUpLeftMovesFromE6 = bishopUpLeftMoves[index];     //All bishop down left moves from E6

        //    Assert.AreEqual(2, bishopUpLeftMovesFromE6.Moves.Count);

        //    Assert.AreEqual(BitboardSquare.D7, bishopUpLeftMovesFromE6.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.C8, bishopUpLeftMovesFromE6.Moves[1]);

        //    //Check from A4 - index 24
        //    index = 24;
        //    Assert.AreEqual(0, ValidMoveArrays.BishopTotalMoves4[index]);  //Number of valid up left moves form A4

        //    PieceMoveSet bishopUpLeftMovesFromA4 = bishopUpLeftMoves[index];     //All bishop down left moves from A4

        //    Assert.AreEqual(0, bishopUpLeftMovesFromA4.Moves.Count);

        //    //Check from D8 - index 59
        //    index = 59;
        //    Assert.AreEqual(0, ValidMoveArrays.BishopTotalMoves4[index]);  //Number of valid up left moves form D8

        //    PieceMoveSet bishopUpLeftMovesFromD8 = bishopUpLeftMoves[index];     //All bishop down left moves from D8

        //    Assert.AreEqual(0, bishopUpLeftMovesFromD8.Moves.Count);
        //}

        //#endregion Diagonal move tests

        //#region Straight move tests

        //[TestMethod]
        //public void TestMoveUp()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from H1 - index 7
        //    int index = 7;
        //    Assert.AreEqual(7, ValidMoveArrays.RookTotalMoves1[index]);  //Number of valid up moves form H1

        //    PieceMoveSet[] rookUpMoves = ValidMoveArrays.RookMoves1;   //All rook up left moves
        //    PieceMoveSet rookUpMovesFromH1 = rookUpMoves[index];     //All rook up moves from H1

        //    Assert.AreEqual(7, rookUpMovesFromH1.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.H2, rookUpMovesFromH1.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.H3, rookUpMovesFromH1.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.H4, rookUpMovesFromH1.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.H5, rookUpMovesFromH1.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.H6, rookUpMovesFromH1.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.H7, rookUpMovesFromH1.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.H8, rookUpMovesFromH1.Moves[6]);

        //    //Check from A4 - index 24
        //    index = 24;
        //    Assert.AreEqual(4, ValidMoveArrays.RookTotalMoves1[index]);  //Number of valid up moves form A4

        //    PieceMoveSet rookUpMovesFromA4 = rookUpMoves[index];     //All rook up moves from A4

        //    Assert.AreEqual(4, rookUpMovesFromA4.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.A5, rookUpMovesFromA4.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.A6, rookUpMovesFromA4.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.A7, rookUpMovesFromA4.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.A8, rookUpMovesFromA4.Moves[3]);
        //}

        //[TestMethod]
        //public void TestMoveRight()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from A1 - index 0
        //    int index = 0;
        //    Assert.AreEqual(7, ValidMoveArrays.RookTotalMoves2[index]);  //Number of valid up moves form A1

        //    PieceMoveSet[] rookRightMoves = ValidMoveArrays.RookMoves2;   //All rook right left moves
        //    PieceMoveSet rookRightMovesFromA1 = rookRightMoves[index];     //All rook right moves from A1

        //    Assert.AreEqual(7, rookRightMovesFromA1.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.B1, rookRightMovesFromA1.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.C1, rookRightMovesFromA1.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.D1, rookRightMovesFromA1.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.E1, rookRightMovesFromA1.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.F1, rookRightMovesFromA1.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.G1, rookRightMovesFromA1.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.H1, rookRightMovesFromA1.Moves[6]);

        //    //Check from D8 - index 59
        //    index = 59;
        //    Assert.AreEqual(4, ValidMoveArrays.RookTotalMoves2[index]);  //Number of valid right moves form D8

        //    PieceMoveSet rookRightMovesFromD8 = rookRightMoves[index];     //All rook right moves from D8

        //    Assert.AreEqual(4, rookRightMovesFromD8.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.E8, rookRightMovesFromD8.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.F8, rookRightMovesFromD8.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.G8, rookRightMovesFromD8.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.H8, rookRightMovesFromD8.Moves[3]);
        //}

        //[TestMethod]
        //public void TestMoveDown()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from A8 - index 56
        //    int index = 56;
        //    Assert.AreEqual(7, ValidMoveArrays.RookTotalMoves3[index]);  //Number of valid down moves from A8

        //    PieceMoveSet[] rookDownMoves = ValidMoveArrays.RookMoves3;   //All rook down moves
        //    PieceMoveSet rookDownMovesFromA8 = rookDownMoves[index];     //All rook down moves from A8

        //    Assert.AreEqual(7, rookDownMovesFromA8.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.A7, rookDownMovesFromA8.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.A6, rookDownMovesFromA8.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.A5, rookDownMovesFromA8.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.A4, rookDownMovesFromA8.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.A3, rookDownMovesFromA8.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.A2, rookDownMovesFromA8.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.A1, rookDownMovesFromA8.Moves[6]);

        //    //Check from H5 - index 39
        //    index = 39;
        //    Assert.AreEqual(4, ValidMoveArrays.RookTotalMoves3[index]);  //Number of valid down moves form H5

        //    PieceMoveSet rookDownMovesFromH5 = rookDownMoves[index];     //All rook down moves from H5

        //    Assert.AreEqual(4, rookDownMovesFromH5.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.H4, rookDownMovesFromH5.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.H3, rookDownMovesFromH5.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.H2, rookDownMovesFromH5.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.H1, rookDownMovesFromH5.Moves[3]);
        //}

        //[TestMethod]
        //public void TestMoveLeft()
        //{
        //    PieceValidMoves.GenerateMoveArrays();

        //    //Check from H8 - index 63
        //    int index = 63;
        //    Assert.AreEqual(7, ValidMoveArrays.RookTotalMoves4[index]);  //Number of valid right moves form H8

        //    PieceMoveSet[] rookLeftMoves = ValidMoveArrays.RookMoves4;   //All rook left left moves
        //    PieceMoveSet rookLeftMovesFromH8 = rookLeftMoves[index];     //All rook right moves from H8

        //    Assert.AreEqual(7, rookLeftMovesFromH8.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.G8, rookLeftMovesFromH8.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.F8, rookLeftMovesFromH8.Moves[1]);
        //    Assert.AreEqual(BitboardSquare.E8, rookLeftMovesFromH8.Moves[2]);
        //    Assert.AreEqual(BitboardSquare.D8, rookLeftMovesFromH8.Moves[3]);
        //    Assert.AreEqual(BitboardSquare.C8, rookLeftMovesFromH8.Moves[4]);
        //    Assert.AreEqual(BitboardSquare.B8, rookLeftMovesFromH8.Moves[5]);
        //    Assert.AreEqual(BitboardSquare.A8, rookLeftMovesFromH8.Moves[6]);

        //    //Check from C1 - index 2
        //    index = 2;
        //    Assert.AreEqual(2, ValidMoveArrays.RookTotalMoves4[index]);  //Number of valid left moves from C1

        //    PieceMoveSet rookLeftMovesFromC1 = rookLeftMoves[index];     //All rook down left from C1

        //    Assert.AreEqual(2, rookLeftMovesFromC1.Moves.Count);
        //    Assert.AreEqual(BitboardSquare.B1, rookLeftMovesFromC1.Moves[0]);
        //    Assert.AreEqual(BitboardSquare.A1, rookLeftMovesFromC1.Moves[1]);
        //}

        //#endregion Straight move tests

    }
}
