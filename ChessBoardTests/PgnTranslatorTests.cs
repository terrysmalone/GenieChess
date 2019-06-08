using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardSearching;

namespace ChessEngineTests
{
    [TestClass]
    public class PgnTranslatorTests
    {
        [TestMethod]
        public void TestToBoard()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestToPgnString()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        #region ToPgnMove tests

        [TestMethod]
        public void TestToPgnMove_PawnMove_White()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            
            var moveFrom = LookupTables.C2;
            var moveTo = LookupTables.C4;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("c4", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnMove_Black()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            var moveFrom = LookupTables.A7;
            var moveTo = LookupTables.A5;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("a5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceMove_White()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var moveFrom = LookupTables.B1;
            var moveTo = LookupTables.C3;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Nc3", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceMove_Black()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            var moveFrom = LookupTables.C8;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Be5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/8/4p3/3P4/8/8/8 w - - 0 1"));

            var moveFrom = LookupTables.D4;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/8/4p3/3P4/8/8/8 b - - 0 1"));

            var moveFrom = LookupTables.E5;
            var moveTo = LookupTables.D4;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("exd4", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/4r3/3B4/4p3/3P4/8/8/8 w - - 0 1"));

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bxe7", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/2N2r2/8/8/8/8/8 b - - 0 1"));

            var moveFrom = LookupTables.F6;
            var moveTo = LookupTables.C6;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxc6", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_White_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1"));

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.G1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_White_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1"));

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.C1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_Black_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1"));
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.G8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_Black_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1"));
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.C8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Promotion_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1"));
            
            var moveFrom = LookupTables.B7;
            var moveTo = LookupTables.B8;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("b8=Q", move);            
        }

        [TestMethod]
        public void TestToPgnMove_Promotion_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/1P6/2K5/8/8/8/5kp1/8 b - - 0 1"));
             
            var moveFrom = LookupTables.G2;
            var moveTo = LookupTables.G1;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("g1=Q", move);  
        }

        [TestMethod]
        public void TestToPgnMove_AmbiguousMoves()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestToPgnMove_Check_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("4k3/8/8/8/8/8/8/4KB2 w - - 0 1"));
            
            var moveFrom = LookupTables.F1;
            var moveTo = LookupTables.B5;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bb5+", move); 
        }

        [TestMethod]
        public void TestToPgnMove_Check_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k3/8/8/8/8/8/8/4KB2 b - - 0 1"));

            var moveFrom = LookupTables.A8;
            var moveTo = LookupTables.A1;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ra1+", move);
        }

        [TestMethod]
        public void TestToPgnMove_CaptureToCheck_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4r3/3P4/3K4/8/8/8/8 w - - 0 1"));

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe7+", move);
            
        }

        [TestMethod]
        public void TestToPgnMove_CaptureToCheck_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4r3/3P4/3KQ3/8/8/8/8 b - - 0 1"));

            var moveFrom = LookupTables.E7;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxe5+", move);
        }

        [TestMethod]
        public void TestToPgnMove_PromotionToCheck_white()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/3Pr3/8/3KQ3/8/8/8/8 w - - 0 1"));

            var moveFrom = LookupTables.D7;
            var moveTo = LookupTables.D8;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("d8=R+", move);   
        }

        [TestMethod]
        public void TestToPgnMove_PromotionToCheck_black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/3Pr3/8/3KQ3/8/8/7p/8 b - - 0 1"));

            var moveFrom = LookupTables.H2;
            var moveTo = LookupTables.H1;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("h1=Q+", move);  
        }


        [TestMethod]
        public void TestToPgnMove_CheckMate_White()
        {
#warning Write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestToPgnMove_CheckMate_Black()
        {
#warning Write tests
            //throw new NotImplementedException();
        }


        #endregion ToPgnMove tests

    }
}
