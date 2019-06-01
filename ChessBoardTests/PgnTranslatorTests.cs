using System;
using ChessGame;
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
            Board board = new Board();
            board.InitaliseStartingPosition();
            
            ulong moveFrom = LookupTables.C2;
            ulong moveTo = LookupTables.C4;
            PieceType pieceToMove = PieceType.Pawn;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("c4", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnMove_Black()
        {
            Board board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            ulong moveFrom = LookupTables.A7;
            ulong moveTo = LookupTables.A5;
            PieceType pieceToMove = PieceType.Pawn;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("a5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceMove_White()
        {
            Board board = new Board();
            board.InitaliseStartingPosition();

            ulong moveFrom = LookupTables.B1;
            ulong moveTo = LookupTables.C3;
            PieceType pieceToMove = PieceType.Knight;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Nc3", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceMove_Black()
        {
            Board board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            ulong moveFrom = LookupTables.C8;
            ulong moveTo = LookupTables.E5;
            PieceType pieceToMove = PieceType.Bishop;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Be5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnCapture_White()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/8/4p3/3P4/8/8/8 w - - 0 1"));

            ulong moveFrom = LookupTables.D4;
            ulong moveTo = LookupTables.E5;
            PieceType pieceToMove = PieceType.Pawn;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe5", move);
        }

        [TestMethod]
        public void TestToPgnMove_PawnCapture_Black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/8/4p3/3P4/8/8/8 b - - 0 1"));

            ulong moveFrom = LookupTables.E5;
            ulong moveTo = LookupTables.D4;
            PieceType pieceToMove = PieceType.Pawn;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("exd4", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceCapture_White()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/4r3/3B4/4p3/3P4/8/8/8 w - - 0 1"));

            ulong moveFrom = LookupTables.D6;
            ulong moveTo = LookupTables.E7;
            PieceType pieceToMove = PieceType.Bishop;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bxe7", move);
        }

        [TestMethod]
        public void TestToPgnMove_PieceCapture_Black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/8/2N2r2/8/8/8/8/8 b - - 0 1"));

            ulong moveFrom = LookupTables.F6;
            ulong moveTo = LookupTables.C6;
            PieceType pieceToMove = PieceType.Rook;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxc6", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_White_Kingside()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1"));

            ulong moveFrom = LookupTables.E1;
            ulong moveTo = LookupTables.G1;
            PieceType pieceToMove = PieceType.King;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_White_Queenside()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1"));

            ulong moveFrom = LookupTables.E1;
            ulong moveTo = LookupTables.C1;
            PieceType pieceToMove = PieceType.King;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_Black_Kingside()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1"));
            
            ulong moveFrom = LookupTables.E8;
            ulong moveTo = LookupTables.G8;
            PieceType pieceToMove = PieceType.King;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Castling_Black_Queenside()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1"));
            
            ulong moveFrom = LookupTables.E8;
            ulong moveTo = LookupTables.C8;
            PieceType pieceToMove = PieceType.King;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void TestToPgnMove_Promotion_White()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1"));
            
            ulong moveFrom = LookupTables.B7;
            ulong moveTo = LookupTables.B8;
            PieceType pieceToMove = PieceType.Queen;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("b8=Q", move);            
        }

        [TestMethod]
        public void TestToPgnMove_Promotion_Black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("8/1P6/2K5/8/8/8/5kp1/8 b - - 0 1"));
             
            ulong moveFrom = LookupTables.G2;
            ulong moveTo = LookupTables.G1;
            PieceType pieceToMove = PieceType.Queen;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

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
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("4k3/8/8/8/8/8/8/4KB2 w - - 0 1"));
            
            ulong moveFrom = LookupTables.F1;
            ulong moveTo = LookupTables.B5;
            PieceType pieceToMove = PieceType.Bishop;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bb5+", move); 
        }

        [TestMethod]
        public void TestToPgnMove_Check_Black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("r3k3/8/8/8/8/8/8/4KB2 b - - 0 1"));

            ulong moveFrom = LookupTables.A8;
            ulong moveTo = LookupTables.A1;
            PieceType pieceToMove = PieceType.Rook;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ra1+", move);
        }

        [TestMethod]
        public void TestToPgnMove_CaptureToCheck_White()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4r3/3P4/3K4/8/8/8/8 w - - 0 1"));

            ulong moveFrom = LookupTables.D6;
            ulong moveTo = LookupTables.E7;
            PieceType pieceToMove = PieceType.Pawn;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe7+", move);
            
        }

        [TestMethod]
        public void TestToPgnMove_CaptureToCheck_Black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4r3/3P4/3KQ3/8/8/8/8 b - - 0 1"));

            ulong moveFrom = LookupTables.E7;
            ulong moveTo = LookupTables.E5;
            PieceType pieceToMove = PieceType.Rook;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxe5+", move);
        }

        [TestMethod]
        public void TestToPgnMove_PromotionToCheck_white()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/3Pr3/8/3KQ3/8/8/8/8 w - - 0 1"));

            ulong moveFrom = LookupTables.D7;
            ulong moveTo = LookupTables.D8;
            PieceType pieceToMove = PieceType.Rook;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("d8=R+", move);   
        }

        [TestMethod]
        public void TestToPgnMove_PromotionToCheck_black()
        {
            Board board = new Board();
            board.ClearBoard();
            board.SetPosition(FenTranslator.ToBoardState("5k2/3Pr3/8/3KQ3/8/8/7p/8 b - - 0 1"));

            ulong moveFrom = LookupTables.H2;
            ulong moveTo = LookupTables.H1;
            PieceType pieceToMove = PieceType.Queen;

            string move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

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
