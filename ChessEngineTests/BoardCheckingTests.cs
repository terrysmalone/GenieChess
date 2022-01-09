using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using NUnit.Framework;

namespace ChessEngineTests
{
    // TODO: Change everything to Assert.That
    [TestFixture]
    public class BoardCheckingTests
    {
        [Test]
        public void IsPieceOnSquare_WithEmptyBoard()
        {
            var board = new Board();
            board.ClearBoard();

            var squares = LookupTables.SquareValuesFromIndex;

            foreach (var square in squares)
            {
                Assert.That(BoardChecking.IsPieceOnSquare(board, square), Is.False, $"Failed at square {square}");
            }
        }
        
        [TestCase("8/1p6/8/8/8/8/8/8 w - - 0 1", 562949953421312u)]
        [TestCase("8/8/8/5N2/8/8/8/8 w - - 0 1", 137438953472u)]
        [TestCase("7k/8/8/8/8/8/8/8 w - - 0 1", 9223372036854775808u)]
        [TestCase("8/8/8/8/8/8/8/7K w - - 0 1", 128u)]
        public void IsPieceOnSquare(string fenString, ulong squareToCheck)
        {
            var board = new Board();
            board.SetPosition(fenString);

            Assert.That(BoardChecking.IsPieceOnSquare(board, (ulong)squareToCheck), Is.True);
        }
        
        [Test]
        public void IsEnemyPieceOnSquare_WithEmptyBoard()
        {
            var board = new Board();
            board.ClearBoard();

            var squares = LookupTables.SquareValuesFromIndex;

            foreach (var square in squares)
            {
                Assert.That(BoardChecking.IsEnemyPieceOnSquare(board, square), Is.False, $"Failed at square {square} ");
            }
        }

        [TestCase("8/1P6/8/8/8/8/8/8 w - - 0 1", 562949953421312u)]
        [TestCase("8/8/8/8/4r3/8/8/8 b - - 0 1", 268435456u)]
        public void IsFriendlyPieceOnSquare_True(string fenString, ulong squareToCheck)
        {
            var board = new Board();
            board.SetPosition(fenString);
            
            Assert.That(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong)squareToCheck), Is.True);
            Assert.That(BoardChecking.IsEnemyPieceOnSquare(board, (ulong)squareToCheck), Is.False);
        }

        [TestCase("8/1P6/8/8/8/8/8/8 b - - 0 1", 562949953421312u)]
        [TestCase("8/8/8/8/4r3/8/8/8 w - - 0 1", 268435456u)]
        public void IsFriendlyPieceOnSquare_False(string fenString, ulong squareToCheck)
        {
            var board = new Board();
            board.SetPosition(fenString);

            Assert.That(BoardChecking.IsFriendlyPieceOnSquare(board, (ulong) squareToCheck), Is.False);
            Assert.That(BoardChecking.IsEnemyPieceOnSquare(board, (ulong) squareToCheck), Is.True);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhitePawn()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A2);
            Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Pawn));

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
            Assert.That(pieceOnSquare, Is.Not.EqualTo(PieceType.Pawn));
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackPawn()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
            Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Pawn));

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A5);
            Assert.AreNotEqual(PieceType.Pawn, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhiteKnight()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
            Assert.That(pieceOnSquare, Is.EqualTo(PieceType.Knight));

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B2);
            Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackKnight()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.G8);
            Assert.AreEqual(PieceType.Knight, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D5);
            Assert.AreNotEqual(PieceType.Knight, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhiteBishop()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C1);
            Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
            Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackBishop()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C8);
            Assert.AreEqual(PieceType.Bishop, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
            Assert.AreNotEqual(PieceType.Bishop, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhiteRook()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H1);
            Assert.AreEqual(PieceType.Rook, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
            Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackRook()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.H8);
            Assert.AreEqual(PieceType.Rook, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.B1);
            Assert.AreNotEqual(PieceType.Rook, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhiteQueen()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D1);
            Assert.AreEqual(PieceType.Queen, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
            Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackQueen()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.D8);
            Assert.AreEqual(PieceType.Queen, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
            Assert.AreNotEqual(PieceType.Queen, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_WhiteKing()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E1);
            Assert.AreEqual(PieceType.King, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
            Assert.AreNotEqual(PieceType.King, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_BlackKing()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.E8);
            Assert.AreEqual(PieceType.King, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A7);
            Assert.AreNotEqual(PieceType.King, pieceOnSquare);
        }

        [Test]
        public void TestGetPieceTypeOnSquare_Empty()
        {
            var board = new Board();
            board.InitaliseStartingPosition();

            var pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.C5);
            Assert.AreEqual(PieceType.None, pieceOnSquare);

            pieceOnSquare = BoardChecking.GetPieceTypeOnSquare(board, LookupTables.A1);
            Assert.AreNotEqual(PieceType.None, pieceOnSquare);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_White_EmptyBoard()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
            

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_White_EmptyBoard_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3Q4/8/8/8/8/8 w - - 0 1"));
            

            var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_Black_EmptyBoard_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("8/8/3q4/8/8/8/8/8 b - - 0 1"));
            

            var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

            var expected = (ulong)3034571949281478664;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_White_WithPieces()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

            var expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_White_WithPieces_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/8/8/3b2Q1/5PP1/4K3 w - - 0 1"));
            

            var queenPositions = BitboardOperations.SplitBoardToArray(board.WhiteQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], true);

            var expected = (ulong)163334998696951808;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            

            var queenPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

            var expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedQueenMoves_Black_WithPieces_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("1r2k3/4pppp/8/4q3/8/3b2Q1/4PPP1/4K3 w - - 0 1"));
            

            var queenPositions = BitboardOperations.SplitBoardToArray(board.BlackQueen);

            var allowedMoves = BoardChecking.CalculateAllowedQueenMoves(board, queenPositions[0], false);

            var expected = (ulong)1188500000215553;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedRookMoves_White()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            

            var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], true);

            var expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedRookMoves_White_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3R1q/8/5PP1/4K3 w - - 0 1"));
            

            var rookPositions = BitboardOperations.SplitBoardToArray(board.WhiteRooks);

            Assert.AreEqual(1, rookPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], true);

            var expected = (ulong)35325504126976;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedRookMoves_Black()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            

            var rookPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], false);

            var expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedRookMoves_Black_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("5k2/4p3/5p2/8/1N3r1q/8/5PP1/4K3 w - - 0 1"));
            

            var rookPositions = BitboardOperations.SplitBoardToArray(board.BlackRooks);

            Assert.AreEqual(1, rookPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedRookMoves(board, rookPositions[0], false);

            var expected = (ulong)139018117120;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedBishopMoves_White()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));

            

            var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], true);

            var expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedBishopMoves_White_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/8/3r4/8/5B2/4P3/7K w - - 0 1"));
            

            var bishopPositions = BitboardOperations.SplitBoardToArray(board.WhiteBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], true);

            var expected = (ulong)585457745920;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedBishopMoves_Black()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            

            var bishopPositions = BitboardOperations.GetSquareIndexesFromBoardValue(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Count);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], false);

            var expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedBishopMoves_Black_Ray()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            

            var bishopPositions = BitboardOperations.SplitBoardToArray(board.BlackBishops);

            Assert.AreEqual(1, bishopPositions.Length);

            var allowedMoves = BoardChecking.CalculateAllowedBishopMoves(board, bishopPositions[0], false);

            var expected = (ulong)5664958784208896;

            Assert.AreEqual(allowedMoves, expected);
        }

        [Test]
        public void TestCalculateAllowedUpRightMoves()
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            

            var whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, true);
            var blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, 17, false);

            var whiteExpected = (ulong)34426847232;
            var blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        [Test]
        public void TestCalculateAllowedUpRightMoves_Ray()
        {
            LookupTables.InitialiseAllTables();
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState("3b2k1/5pp1/1q6/3r2N1/8/5B2/4P3/7K b - - 0 1"));
            

            var whiteMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], true);
            var blackMoves = BoardChecking.CalculateAllowedUpRightMoves(board, LookupTables.SquareValuesFromIndex[17], false);

            var whiteExpected = (ulong)34426847232;
            var blackExpected = (ulong)67108864;

            Assert.AreEqual(whiteExpected, whiteMoves);
            Assert.AreEqual(blackExpected, blackMoves);

        }

        #region calculateUpMoves methods

        [Test]
        public void TestCalculateAllowedUpMoves_NoBlocking()
        {
            var board = new Board();

            board.ClearBoard();

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, true); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [Test]
        public void TestCalculateAllowedUpMoves_NoBlocking_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();

            board.ClearBoard();

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], true); //d2

            Assert.AreEqual((ulong)578721382704611328, upMoves);
        }

        [Test]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White()
        {
            var board = new Board();
            board.SetPosition("8/3N4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, true); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        [Test]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();
            board.SetPosition("8/3N4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board,
                                                                LookupTables.SquareValuesFromIndex[11],
                                                                true); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        /// <summary>
        /// Friendly moves - Does not include placed blocking piece
        /// </summary>
        [Test]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black()
        {
            var board = new Board();

            board.ClearBoard();
            board.SetPosition("8/3n4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, false); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        /// <summary>
        /// Friendly moves - Does not include placed blocking piece
        /// </summary>
        [Test]
        public void TestCalculateAllowedUpMoves_FriendlyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();
            board.SetPosition("8/3n4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7
            

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], false); //d2

            Assert.AreEqual((ulong)8830587502592, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [Test]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White()
        {
            var board = new Board();
            board.SetPosition("8/3n4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7
            

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, true); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        /// <summary>
        /// Enemy  moves - includes placed blocking piece
        /// </summary>
        [Test]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_White_Ray()
        {
            LookupTables.InitialiseAllTables();

            var board = new Board();
            board.SetPosition("8/3n4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7
            

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], true); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }
        
        // Enemy  moves - includes placed blocking piece
        [Test]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black()
        {
            var board = new Board();
            board.SetPosition("8/3N4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, 11, false); // d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }
        
        // Enemy  moves - includes placed blocking piece
        [Test]
        public void TestCalculateAllowedUpMoves_EnemyBlocking_Black_Ray()
        {
            LookupTables.InitialiseAllTables();
            var board = new Board();
            board.SetPosition("8/3N4/8/8/8/8/8/8 w - - 0 1"); // Knight on d7

            var upMoves = BoardChecking.CalculateAllowedUpMoves(board, LookupTables.SquareValuesFromIndex[11], false); //d2

            Assert.AreEqual((ulong)2260630401187840, upMoves);
        }

        #region calculateUpLeftMoves methods

        [Test]
        public void TestCalculateAllowedUpLeftMoves_NoBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedUpLeftMoves_FriendlyBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedUpLeftMoves_EnemyBlocking_White()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedUpLeftMoves_NoBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedUpLeftMoves_FriendlyBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedUpLeftMoves_EnemyBlocking_Black()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        #endregion calculateUpLeftMoves methods

        [Test]
        public void TestCalculateAllowedRightMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedRightMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedLeftMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedLeftMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownRightMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownRightMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownLeftMoves()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        [Test]
        public void TestCalculateAllowedDownLeftMoves_Ray()
        {
#warning write tests
            //throw new NotImplementedException();
        }

        #endregion Calculate Allowed Moves

        [Test]
        public void TestGetSpecialMoveType()
        {
#warning write tests
            //throw new NotImplementedException();
        }

    }
}
