using System;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using NUnit.Framework;

namespace ChessEngineTests.NotationHelpers
{
    [TestFixture]
    public class PgnTranslatorTests
    {
        [Test]
        public void PawnMove_White()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            
            var moveFrom = LookupTables.C2;
            var moveTo = LookupTables.C4;
            var pieceToMove = PieceType.Pawn;
            
            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("c4"));
        }

        [Test]
        public void PawnMove_Black()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            var moveFrom = LookupTables.A7;
            var moveTo = LookupTables.A5;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("a5"));
        }

        [Test]
        public void PieceMove_White()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var moveFrom = LookupTables.B1;
            var moveTo = LookupTables.C3;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Nc3"));
        }

        [Test]
        public void PieceMove_Black()
        {
            var board = new Board();
            board.InitaliseStartingPosition();
            board.SwitchSides();

            var moveFrom = LookupTables.C8;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Be5"));
        }

        [Test]
        public void PawnCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/4p3/3P4/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D4;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("dxe5"));
        }

        [Test]
        public void PawnCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/4p3/3P4/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.E5;
            var moveTo = LookupTables.D4;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("exd4"));
        }

        [Test]
        public void PieceCapture_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/4r3/3B4/4p3/3P4/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Bishop;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Bxe7"));
        }

        [Test]
        public void PieceCapture_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/2N2r2/8/8/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.F6;
            var moveTo = LookupTables.C6;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Rxc6"));
        }

        [Test]
        public void Castling_White_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.G1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("0-0"));
        }

        [Test]
        public void Castling_White_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

            var moveFrom = LookupTables.E1;
            var moveTo = LookupTables.C1;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("0-0-0"));
        }

        [Test]
        public void Castling_Black_Kingside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1");
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.G8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("0-0"));
        }

        [Test]
        public void Castling_Black_Queenside()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1");
            
            var moveFrom = LookupTables.E8;
            var moveTo = LookupTables.C8;
            var pieceToMove = PieceType.King;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("0-0-0"));
        }

        [Test]
        public void Promotion_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1");
            
            var move = PgnTranslator.ToPgnMove(board, LookupTables.B7, LookupTables.B8, PieceType.Pawn, SpecialMoveType.QueenPromotion);

            Assert.That(move, Is.EqualTo("b8=Q"));            
        }
        
        [Test]
        public void ToPgnMove_LowerPromotion_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5k2/8 w - - 0 1");
            
            var move = PgnTranslator.ToPgnMove(board, LookupTables.B7, LookupTables.B8, PieceType.Pawn, SpecialMoveType.KnightPromotion);

            Assert.That(move, Is.EqualTo("b8=N"));            
        }

        [Test]
        public void Promotion_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/1P6/2K5/8/8/8/5kp1/8 b - - 0 1");
             
            var move = PgnTranslator.ToPgnMove(board, LookupTables.G2, LookupTables.G1, PieceType.Pawn, SpecialMoveType.QueenPromotion);

            Assert.That(move, Is.EqualTo("g1=Q"));  
        }

        [Test]
        public void Check_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("4k3/8/8/8/8/8/8/4KB2 w - - 0 1");
            
            var move = PgnTranslator.ToPgnMove(board, LookupTables.F1, LookupTables.B5, PieceType.Bishop);

            Assert.That(move, Is.EqualTo("Bb5+")); 
        }

        [Test]
        public void Check_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("r3k3/8/8/8/8/8/8/4KB2 b - - 0 1");

            var moveFrom = LookupTables.A8;
            var moveTo = LookupTables.A1;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Ra1+"));
        }

        [Test]
        public void CaptureToCheck_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/4r3/3P4/3K4/8/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.D6;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Pawn;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("dxe7+"));
            
        }

        [Test]
        public void CaptureToCheck_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/4r3/3P4/3KQ3/8/8/8/8 b - - 0 1");

            var moveFrom = LookupTables.E7;
            var moveTo = LookupTables.E5;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Rxe5+"));
        }

        [Test]
        public void PromotionToCheck_white()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/3Pr3/8/3KQ3/8/8/8/8 w - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.D7, LookupTables.D8, PieceType.Pawn, SpecialMoveType.RookPromotion);

            Assert.That(move, Is.EqualTo("d8=R+"));   
        }

        [Test]
        public void PromotionToCheck_black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5k2/3Pr3/8/3KQ3/8/8/7p/8 b - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.H2, LookupTables.H1, PieceType.Pawn, SpecialMoveType.QueenPromotion);

            Assert.That(move, Is.EqualTo("h1=Q+"));  
        }

        [Test]
        public void TestAmbiguousMove_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("7k/1R6/8/8/1K2R3/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.E4;
            var moveTo = LookupTables.E7;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Ree7"));
        }

        [Test]
        public void TestAmbiguousMove_White2()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("4k3/8/1R6/8/8/8/1R6/2K5 w - - 0 1");

            var moveFrom = LookupTables.B4;
            var moveTo = LookupTables.B4;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("R2b4"));
        }

        [Test]
        public void TestAmbiguousMove_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/2n5/5k2/8/5n2/K7/8 b - - 0 1");

            var moveFrom = LookupTables.C6;
            var moveTo = LookupTables.D4;
            var pieceToMove = PieceType.Knight;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Ncd4"));
        }


        [Test]
        public void CheckMate_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("7k/RR6/8/8/1K6/8/8/8 w - - 0 1");

            var moveFrom = LookupTables.A7;
            var moveTo = LookupTables.A8;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Ra8#"));
        }

        [Test]
        public void CheckMate_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("5rk1/8/8/8/8/8/PPP5/1K6 b - - 0 1");

            var moveFrom = LookupTables.F8;
            var moveTo = LookupTables.F1;
            var pieceToMove = PieceType.Rook;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Rf1#"));
        }

        [Test]
        public void PromotionCheck_White()
        {
            
        }
        
        [Test]
        public void PromotionCheckMate_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("2n3rr/Pk3p2/3q4/3N4/3Pp2p/4P1p1/3B1PP1/R4RK1 w - - 1 0");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.A7, LookupTables.A8, PieceType.Pawn, SpecialMoveType.QueenPromotion);

            Assert.That(move, Is.EqualTo("a8=Q#"));
        }
        
        [Test]
        public void CaptureCheck_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("k1n3rr/Pp3p2/3q4/3N4/3Pp2p/1Q2P1p1/3B1PP1/R4RK1 w - - 1 0");

            var moveFrom = LookupTables.B3;
            var moveTo = LookupTables.B7;
            var pieceToMove = PieceType.Queen;

            var move = PgnTranslator.ToPgnMove(board, moveFrom, moveTo, pieceToMove);

            Assert.That(move, Is.EqualTo("Qxb7+"));
        }
        
        [Test]
        public void PromotionCaptureCheckMate_White()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("1rb4r/p1Pp3p/kb1P3n/3Q6/N3Pp2/8/P1P3PP/7K w - - 1 0");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.C7, LookupTables.B8, PieceType.Pawn, SpecialMoveType.KnightPromotionCapture);

            Assert.That(move, Is.EqualTo("cxb8=N#"));
        }
        
        [Test]
        public void CaptureCheckMate_White()
        {   
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("2b4k/2R1R3/2PP4/2K5/5pn1/4p1r1/8/8 w - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.C7, LookupTables.C8, PieceType.Rook, SpecialMoveType.Capture);

            Assert.That(move, Is.EqualTo("Rxc8#"));
        }
        
        [Test]
        public void PromotionCheck_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/8/8/5K2/2kp4/8 b - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.D2, LookupTables.D1, PieceType.Pawn, SpecialMoveType.BishopPromotion);

            Assert.That(move, Is.EqualTo("d1=B+"));
        }

        [Test]
        public void CaptureCheck_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/k7/8/8/8/8/3p4/1KR5 b - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.D2, LookupTables.C1, PieceType.Pawn, SpecialMoveType.RookPromotionCapture);

            Assert.That(move, Is.EqualTo("dxc1=R+"));
        }
        
        [Test]
        public void CaptureCheckMate_Black()
        {
            var board = new Board();
            board.ClearBoard();
            board.SetPosition("8/8/8/5k2/6rn/6BK/4RpPP/6R1 b - - 0 1");

            var move = PgnTranslator.ToPgnMove(board, LookupTables.F2, LookupTables.G1, PieceType.Pawn, SpecialMoveType.KnightPromotionCapture);

            Assert.That(move, Is.EqualTo("fxg1=N#"));
        }
    }
}
