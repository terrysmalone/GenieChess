using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests.NotationHelpers
{
    [TestClass]
    public class PgnTranslatorTests
    {
        [TestMethod]
        public void PawnMove_White()
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
        public void PawnMove_Black()
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
        public void PieceMove_White()
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
        public void PieceMove_Black()
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
        public void PawnCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/4p3/3P4/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D4;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe5", move);
        }

        [TestMethod]
        public void PawnCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/4p3/3P4/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.E5;
            var moveTo = LookupTables.D4;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("exd4", move);
        }

        [TestMethod]
        public void PieceCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/4r3/3B4/4p3/3P4/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bxe7", move);
        }

        [TestMethod]
        public void PieceCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/2N2r2/8/8/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.F6;
            var moveTo = LookupTables.C6;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxc6", move);
        }

        [TestMethod]
        public void Castling_White_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.G1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void Castling_White_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.C1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void Castling_Black_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1");
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.G8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0", move);
        }

        [TestMethod]
        public void Castling_Black_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1");
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.C8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("0-0-0", move);
        }

        [TestMethod]
        public void Promotion_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1");
            
            var moveFrom = LookupTables.B7;
            var moveTo = LookupTables.B8;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("b8=Q", move);            
        }
        
        [TestMethod]
        public void ToPgnMove_LowerPromotion_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1");
            
            var moveFrom = LookupTables.B7;
            var moveTo = LookupTables.B8;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("b8=N", move);            
        }

        [TestMethod]
        public void Promotion_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5kp1/8 b - - 0 1");
             
            var moveFrom = LookupTables.G2;
            var moveTo = LookupTables.G1;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("g1=Q", move);  
        }

        [TestMethod]
        public void Check_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("4k3/8/8/8/8/8/8/4KB2 w - - 0 1");
            
            var moveFrom = LookupTables.F1;
            var moveTo = LookupTables.B5;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Bb5+", move); 
        }

        [TestMethod]
        public void Check_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k3/8/8/8/8/8/8/4KB2 b - - 0 1");

            var moveFrom = LookupTables.A8;
            var moveTo = LookupTables.A1;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ra1+", move);
        }

        [TestMethod]
        public void CaptureToCheck_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/4r3/3P4/3K4/8/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("dxe7+", move);
            
        }

        [TestMethod]
        public void CaptureToCheck_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/4r3/3P4/3KQ3/8/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.E7;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rxe5+", move);
        }

        [TestMethod]
        public void PromotionToCheck_white()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/3Pr3/8/3KQ3/8/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D7;
            var moveTo = LookupTables.D8;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("d8=R+", move);   
        }

        [TestMethod]
        public void PromotionToCheck_black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/3Pr3/8/3KQ3/8/8/7p/8 b - - 0 1");

            var moveFrom = LookupTables.H2;
            var moveTo = LookupTables.H1;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("h1=Q+", move);  
        }

        [TestMethod]
        public void TestAmbiguousMove_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("7k/1R6/8/8/1K2R3/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.E4;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ree7", move);
        }

        [TestMethod]
        public void TestAmbiguousMove_White2()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("4k3/8/1R6/8/8/8/1R6/2K5 w - - 0 1");

            var moveFrom = LookupTables.B4;
            var moveTo = LookupTables.B4;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("R2b4", move);
        }

        [TestMethod]
        public void TestAmbiguousMove_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/2n5/5k2/8/5n2/K7/8 b - - 0 1");

            var moveFrom = LookupTables.C6;
            var moveTo = LookupTables.D4;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ncd4", move);
        }


        [TestMethod]
        public void CheckMate_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("7k/RR6/8/8/1K6/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.A7;
            var moveTo = LookupTables.A8;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Ra8#", move);
        }

        [TestMethod]
        public void CheckMate_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5rk1/8/8/8/8/8/PPP5/1K6 b - - 0 1");

            var moveFrom = LookupTables.F8;
            var moveTo = LookupTables.F1;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Rf1#", move);
        }

        [TestMethod]
        public void PromotionCheck_White()
        {
            
        }
        
        [TestMethod]
        public void PromotionCheckMate_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("2n3rr/Pk3p2/3q4/3N4/3Pp2p/4P1p1/3B1PP1/R4RK1 w - - 1 0");

            var moveFrom = LookupTables.A7;
            var moveTo = LookupTables.A8;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("a8=Q#", move);
        }
        
        [TestMethod]
        public void CaptureCheck_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("k1n3rr/Pp3p2/3q4/3N4/3Pp2p/1Q2P1p1/3B1PP1/R4RK1 w - - 1 0");

            var moveFrom = LookupTables.B3;
            var moveTo = LookupTables.B7;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("Qxb7+", move);
        }
        
        [TestMethod]
        public void CaptureCheckMate_White()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void PromotionCheck_Black()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void PromotionCheckMate_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("1rb4r/p1Pp3p/kb1P3n/3Q6/N3Pp2/8/P1P3PP/7K w - - 1 0");

            var moveFrom = LookupTables.C7;
            var moveTo = LookupTables.B8;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.AreEqual("cxb8=N#", move);
        }
        
        [TestMethod]
        public void CaptureCheck_Black()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void CaptureCheckMate_Black()
        {
            throw new NotImplementedException();
        }
    }
}
