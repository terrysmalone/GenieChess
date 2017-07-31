using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.BoardSearching;
using ChessGame.NotationHelpers;

namespace ChessBoardTests
{
     [TestClass]
    public class BoardCheckingTests
    {
         [TestMethod]
         public void TestIsPieceOnSquare()
         {
             Board board = new Board();
             board.ClearBoard();

             ulong[] squares = LookupTables.SquareValuesFromIndex;

             foreach (ulong square in squares)
	         {
                 Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, square), "Failed at square " + square.ToString());
             }

             //Pawn
             board.PlacePiece(PieceType.Pawn, PieceColour.Black, (ulong)72057594037927936);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)72057594037927936));

             board.RemovePiece((ulong)72057594037927936);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)72057594037927936));

             //Knight
             board.PlacePiece(PieceType.Knight, PieceColour.White, (ulong)137438953472);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)137438953472));

             board.RemovePiece((ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)137438953472));

             //Bishop
             board.PlacePiece(PieceType.Bishop, PieceColour.White, (ulong)1);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)1));

             board.RemovePiece((ulong)1);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)1));

             //Rook
             board.PlacePiece(PieceType.Rook, PieceColour.Black, (ulong)67108864);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)67108864));

             board.RemovePiece((ulong)67108864);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)67108864));

             //Queen
             board.PlacePiece(PieceType.Queen, PieceColour.White, (ulong)9223372036854775808);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)9223372036854775808));

             board.RemovePiece((ulong)9223372036854775808);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)9223372036854775808));

             //King
             board.PlacePiece(PieceType.King, PieceColour.Black, (ulong)562949953421312);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)562949953421312));

             board.RemovePiece((ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)562949953421312));
         }

         [TestMethod]
         public void TestIsEnemyPieceOnSquare()
         {
             Board board = new Board();
             board.ClearBoard();

             ulong[] squares = LookupTables.SquareValuesFromIndex;

             foreach (ulong square in squares)
             {
                 Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, square), "Failed at square " + square.ToString());
             }

             //Pawn
             board.PlacePiece(PieceType.Pawn, PieceColour.Black, (ulong)72057594037927936);
             Assert.IsTrue(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)72057594037927936));

             board.RemovePiece((ulong)72057594037927936);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)72057594037927936));

             //Knight
             board.PlacePiece(PieceType.Knight, PieceColour.White, (ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)137438953472));

             board.RemovePiece((ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)137438953472));

             //Bishop
             board.PlacePiece(PieceType.Bishop, PieceColour.White, (ulong)1);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)1));

             board.RemovePiece((ulong)1);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)1));

             board.SwitchSides();
             //Rook
             board.PlacePiece(PieceType.Rook, PieceColour.Black, (ulong)67108864);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)67108864));

             board.RemovePiece((ulong)67108864);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)67108864));

             //Queen
             board.PlacePiece(PieceType.Queen, PieceColour.White, (ulong)9223372036854775808);
             Assert.IsTrue(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)9223372036854775808));

             board.RemovePiece((ulong)9223372036854775808);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)9223372036854775808));

             //King
             board.PlacePiece(PieceType.King, PieceColour.Black, (ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)562949953421312));

             board.RemovePiece((ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)562949953421312));
         }

         [TestMethod]
         public void TestIsFriendlyPieceOnSquare()
         {
             Board board = new Board();
             board.SetPosition(FenTranslator.ToBoardState("1rb3kr/pp3p1p/2p3pn/n3N3/3pPB1q/2NP4/P1PQBPPP/b4RK1 w - - 0 1"));
             
             //White pieces
             Assert.IsTrue(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)256));
             Assert.IsTrue(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)2048));
             Assert.IsTrue(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)64));
             Assert.IsTrue(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)68719476736));
            
             //Black pieces
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)4294967296));
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)4611686018427387904));
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)281474976710656));

             //No pieces
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)72057594037927936));
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)549755813888));
             Assert.IsFalse(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)67108864));
             
         }

         #region GetPieceTypeOnSquare

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhitePawn()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A2);
             Assert.AreEqual(PieceType.Pawn, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
             Assert.AreNotEqual(PieceType.Pawn, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackPawn()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreEqual(PieceType.Pawn, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
             Assert.AreNotEqual(PieceType.Pawn, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteKnight()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreEqual(PieceType.Knight, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B2);
             Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackKnight()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.G8);
             Assert.AreEqual(PieceType.Knight, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D5);
             Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteBishop()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C1);
             Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackBishop()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C8);
             Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteRook()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H1);
             Assert.AreEqual(PieceType.Rook, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackRook()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H8);
             Assert.AreEqual(PieceType.Rook, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteQueen()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D1);
             Assert.AreEqual(PieceType.Queen, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackQueen()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D8);
             Assert.AreEqual(PieceType.Queen, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteKing()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E1);
             Assert.AreEqual(PieceType.King, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.King, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackKing()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E8);
             Assert.AreEqual(PieceType.King, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.King, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_Empty()
         {
             Board board = new Board();
             board.InitaliseStartingPosition();

             PieceType pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C5);
             Assert.AreEqual(PieceType.None, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A1);
             Assert.AreNotEqual(PieceType.None, pieceOnSquare);
         }

        

         #endregion GetPieceTypeOnSquare

         #region Calculate Allowed Moves

         [TestMethod]
         public void TestCalculateAllowedQueenMoves_White_EmptyBoard()
         {
             Board board = new Board();
             board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
             board.CalculateUsefulBitboards();

             List<byte> queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

             ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.White);

             ulong expected = (ulong)3034571949281478664;

             Assert.AreEqual(allowedMoves, expected);
         }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_EmptyBoard_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.White);

            ulong expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.Black);

            ulong expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.Black);

            ulong expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_WithPieces()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.White);

            ulong expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_WithPieces_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.White);

            ulong expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.Black);

            ulong expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            ulong allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], PieceColour.Black);

            ulong expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_White()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Count);

            ulong allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], PieceColour.White);

            ulong expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_White_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] rookPositions = BitboardOperations.SplitBoardToArray(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Length);

            ulong allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], PieceColour.White);

            ulong expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_Black()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Count);

            ulong allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], PieceColour.Black);

            ulong expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_Black_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] rookPositions = BitboardOperations.SplitBoardToArray(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Length);

            ulong allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], PieceColour.Black);

            ulong expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }
       
        [TestMethod]
        public void TestCalculateAllowedBishopMoves_White()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));
             
            board.CalculateUsefulBitboards();

            List<byte> bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            ulong allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], PieceColour.White);

            ulong expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_White_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] bishopPositions = BitboardOperations.SplitBoardToArray(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            ulong allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], PieceColour.White);

            ulong expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_Black()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            List<byte> bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            ulong allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], PieceColour.Black);

            ulong expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_Black_Ray()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong[] bishopPositions = BitboardOperations.SplitBoardToArray(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            ulong allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], PieceColour.Black);

            ulong expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedUpRightMoves()
        {
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, PieceColour.White);
            ulong blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, PieceColour.Black);

            ulong whiteExpected = (ulong)34426847232;
            ulong blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        [TestMethod]
        public void TestCalculateAllowedUpRightMoves_Ray()
        {
            LookupTables.InitialiseAllTables();
            Board board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            ulong whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], PieceColour.White);
            ulong blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], PieceColour.Black);

            ulong whiteExpected = (ulong)34426847232;
            ulong blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        #region calculateUpMoves methods

        [TestMethod]
        public void TestCalculateAllowedUpMoves_NoBlocking()
        {
            Board board = new Board();

            board.ClearBoard();

            ulong upMoves =  BoardChecking.CalculateAllowedUpMoves(board, 11, PieceColour.White); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_NoBlocking_Ray()
        {
            LookupTables.InitialiseAllTables();
            
            Board board = new Board();

            board.ClearBoard();

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], PieceColour.White); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White()
        {
            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.White, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, PieceColour.White); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.White, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], PieceColour.White); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

         /// <summary>
         /// Friendly moves - Does not include placed blocking piece
         /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black()
        {
            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.Black, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, PieceColour.Black); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);    
        }

        /// <summary>
        /// Friendly moves - Does not include placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();

            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.Black, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], PieceColour.Black); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White()
        {
            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.Black, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, PieceColour.White); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.Black, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], PieceColour.White); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black()
        {
            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.White, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, PieceColour.Black); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();
            Board board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, PieceColour.White, 3, 6); //d7

            ulong upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], PieceColour.Black); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        #endregion calculateUpMoves methods

        #region calculateUpLeftMoves methods
         
        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_NoBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_FriendlyBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_EnemyBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_NoBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_FriendlyBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedUpLeftMoves_EnemyBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        #endregion calculateUpLeftMoves methods

        [TestMethod]
        public void TestCalculateAllowedRightMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedRightMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedLeftMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedLeftMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownRightMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownRightMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownLeftMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCalculateAllowedDownLeftMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }
        
        #endregion Calculate Allowed Moves

        [TestMethod]
        public void TestGetSpecialMoveType()
        {
#warning write tests
            //throw new NotImplementedException();
        }
         
    }
}
