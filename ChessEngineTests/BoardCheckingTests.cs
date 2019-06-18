using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessEngineTests
{
     [TestClass]
    public class BoardCheckingTests
    {
         [TestMethod]
         public void TestIsPieceOnSquare()
         {
             var board = new Board();
             board.ClearBoard();

             var squares = LookupTables.SquareValuesFromIndex;

             foreach (var square in squares)
	         {
                 Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, square), "Failed at square " + square.ToString());
             }

             //Pawn
             board.PlacePiece(PieceType.Pawn, pieceIsWhite: false, (ulong)72057594037927936);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)72057594037927936));

             board.RemovePiece((ulong)72057594037927936);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)72057594037927936));

             //Knight
             board.PlacePiece(PieceType.Knight, pieceIsWhite: true, (ulong)137438953472);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)137438953472));

             board.RemovePiece((ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)137438953472));

             //Bishop
             board.PlacePiece(PieceType.Bishop, pieceIsWhite: true, (ulong)1);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)1));

             board.RemovePiece((ulong)1);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)1));

             //Rook
             board.PlacePiece(PieceType.Rook, pieceIsWhite: false, (ulong)67108864);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)67108864));

             board.RemovePiece((ulong)67108864);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)67108864));

             //Queen
             board.PlacePiece(PieceType.Queen, pieceIsWhite: true, (ulong)9223372036854775808);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)9223372036854775808));

             board.RemovePiece((ulong)9223372036854775808);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)9223372036854775808));

             //King
             board.PlacePiece(PieceType.King, pieceIsWhite: false, (ulong)562949953421312);
             Assert.IsTrue(BoardChecking.IsPieceOnSquare(board, (ulong)562949953421312));

             board.RemovePiece((ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsPieceOnSquare(board, (ulong)562949953421312));
         }

         [TestMethod]
         public void TestIsEnemyPieceOnSquare()
         {
             var board = new Board();
             board.ClearBoard();

             var squares = LookupTables.SquareValuesFromIndex;

             foreach (var square in squares)
             {
                 Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, square), "Failed at square " + square.ToString());
             }

             //Pawn
             board.PlacePiece(PieceType.Pawn, pieceIsWhite: false, (ulong)72057594037927936);
             Assert.IsTrue(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)72057594037927936));

             board.RemovePiece((ulong)72057594037927936);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)72057594037927936));

             //Knight
             board.PlacePiece(PieceType.Knight, pieceIsWhite: true, (ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)137438953472));

             board.RemovePiece((ulong)137438953472);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)137438953472));

             //Bishop
             board.PlacePiece(PieceType.Bishop, pieceIsWhite: true, (ulong)1);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)1));

             board.RemovePiece((ulong)1);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)1));

             board.SwitchSides();
             //Rook
             board.PlacePiece(PieceType.Rook, pieceIsWhite: false, (ulong)67108864);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)67108864));

             board.RemovePiece((ulong)67108864);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)67108864));

             //Queen
             board.PlacePiece(PieceType.Queen, pieceIsWhite: true, (ulong)9223372036854775808);
             Assert.IsTrue(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)9223372036854775808));

             board.RemovePiece((ulong)9223372036854775808);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)9223372036854775808));

             //King
             board.PlacePiece(PieceType.King, pieceIsWhite: false, (ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)562949953421312));

             board.RemovePiece((ulong)562949953421312);
             Assert.IsFalse(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)562949953421312));
         }

         [TestMethod]
         public void TestIsFriendlyPieceOnSquare()
         {
             var board = new Board();
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
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A2);
             Assert.AreEqual(PieceType.Pawn, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
             Assert.AreNotEqual(PieceType.Pawn, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackPawn()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreEqual(PieceType.Pawn, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
             Assert.AreNotEqual(PieceType.Pawn, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteKnight()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreEqual(PieceType.Knight, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B2);
             Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackKnight()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.G8);
             Assert.AreEqual(PieceType.Knight, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D5);
             Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteBishop()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C1);
             Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackBishop()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C8);
             Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteRook()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H1);
             Assert.AreEqual(PieceType.Rook, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackRook()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H8);
             Assert.AreEqual(PieceType.Rook, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
             Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteQueen()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D1);
             Assert.AreEqual(PieceType.Queen, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackQueen()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D8);
             Assert.AreEqual(PieceType.Queen, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_WhiteKing()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E1);
             Assert.AreEqual(PieceType.King, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.King, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_BlackKing()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E8);
             Assert.AreEqual(PieceType.King, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
             Assert.AreNotEqual(PieceType.King, pieceOnSquare);
         }

         [TestMethod]
         public void TestGetPieceTypeOnSquare_Empty()
         {
             var board = new Board();
             board.InitaliseStartingPosition();

             var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C5);
             Assert.AreEqual(PieceType.None, pieceOnSquare);

             pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A1);
             Assert.AreNotEqual(PieceType.None, pieceOnSquare);
         }

        

         #endregion GetPieceTypeOnSquare

         #region Calculate Allowed Moves

         [TestMethod]
         public void TestCalculateAllowedQueenMoves_White_EmptyBoard()
         {
             var board = new Board();
             board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
             board.CalculateUsefulBitboards();

             var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

             var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: true);

             var expected = (ulong)3034571949281478664;

             Assert.AreEqual(allowedMoves, expected);
         }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_EmptyBoard_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: true);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: false);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: false);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_WithPieces()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: true);

            var expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_White_WithPieces_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: true);

            var expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: false);

            var expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], whiteToMove: false);

            var expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_White()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], whiteToMove: true);

            var expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_White_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var rookPositions = BitboardOperations.SplitBoardToArray(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], whiteToMove: true);

            var expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_Black()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], whiteToMove: false);

            var expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedRookMoves_Black_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            board.CalculateUsefulBitboards();

            var rookPositions = BitboardOperations.SplitBoardToArray(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], whiteToMove: false);

            var expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }
       
        [TestMethod]
        public void TestCalculateAllowedBishopMoves_White()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));
             
            board.CalculateUsefulBitboards();

            var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], whiteToMove: true);

            var expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_White_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));
            board.CalculateUsefulBitboards();

            var bishopPositions = BitboardOperations.SplitBoardToArray(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], whiteToMove: true);

            var expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_Black()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], whiteToMove: false);

            var expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedBishopMoves_Black_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            var bishopPositions = BitboardOperations.SplitBoardToArray(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], whiteToMove: false);

            var expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [TestMethod]
        public void TestCalculateAllowedUpRightMoves()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            var whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, whiteToMove: true);
            var blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, whiteToMove: false);

            var whiteExpected = (ulong)34426847232;
            var blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        [TestMethod]
        public void TestCalculateAllowedUpRightMoves_Ray()
        {
            LookupTables.InitialiseAllTables();
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            board.CalculateUsefulBitboards();

            var whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], whiteToMove: true);
            var blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], whiteToMove: false);

            var whiteExpected = (ulong)34426847232;
            var blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        #region calculateUpMoves methods

        [TestMethod]
        public void TestCalculateAllowedUpMoves_NoBlocking()
        {
            var board = new Board();

            board.ClearBoard();

            var upMoves =  BoardChecking.CalculateAllowedUpMoves(board, 11, whiteToMove: true); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_NoBlocking_Ray()
        {
            LookupTables.InitialiseAllTables();
            
            var board = new Board();

            board.ClearBoard();

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], whiteToMove: true); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White()
        {
            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: true, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, whiteToMove: true); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: true, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], whiteToMove: true); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

         /// <summary>
         /// Friendly moves - Does not include placed blocking piece
         /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black()
        {
            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: false, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, whiteToMove: false); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);    
        }

        /// <summary>
        /// Friendly moves - Does not include placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: false, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], whiteToMove: false); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White()
        {
            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: false, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, whiteToMove: true); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: false, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], whiteToMove: true); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black()
        {
            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: true, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, whiteToMove: false); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [TestMethod]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();
            var board = new Board();

            board.ClearBoard();
            board.PlacePiece(PieceType.Knight, pieceIsWhite: true, 3, 6); //d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], whiteToMove: false); //d2

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
