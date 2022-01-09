using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceLoading;

namespace ChessEngineTests
{
    /// <summary>
    /// Tests to make sure the whole engine behaves as expected
    /// </summary>
    [TestClass]
    public class GameTests
    {
        private readonly IResourceLoader _resourceLoader = new ResourceLoader();
        
        #region simple capture

        #region MiniMax

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_MiniMax_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.SetFENPosition("7k/8/8/3pK3/8/8/8/8 w - - 0 1");

            game.ThinkingDepth = 1;

            game.AllowAllCastling(false);
            
            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_MiniMax_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.SetFENPosition("7k/8/8/3pK3/8/8/8/8 w - - 0 1");

            game.ThinkingDepth = 2;

            game.AllowAllCastling(false);
            
            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }  
        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_MiniMax_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.SetFENPosition("7k/8/8/3pK3/8/8/8/8 w - - 0 1");

            game.ThinkingDepth = 6;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }   

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_MiniMax_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 1;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_MiniMax_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 3;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }
        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_MiniMax_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 6;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        #endregion MiniMax

        #region NegaMax

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_NegaMax_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 1;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_NegaMax_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 2;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }
        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_NegaMax_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 6;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_NegaMax_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 1;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_NegaMax_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 3;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }
        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_NegaMax_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 6;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        #endregion NegaMax

        #region AlphaBeta

        #region King capture only piece (pawn)

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_AlphaBeta_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 1;
            
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();
            
            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_AlphaBeta_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 2;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();
            
            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_AlphaBeta_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 4);
            game.PlacePiece(PieceType.Pawn, false, 3, 4);
            game.PlacePiece(PieceType.King, false, 7, 7);

            game.ThinkingDepth = 6;
            
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();
            
            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        #endregion King capture only piece (pawn)

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_AlphaBeta_1()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 1;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();
            
            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_AlphaBeta_3()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 3;
            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }
        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_Black_AlphaBeta_6()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.SetFENPosition("8/8/4Pk2/8/8/8/1K6/8 b - - 0 1");

            game.ThinkingDepth = 6;

            game.AllowAllCastling(false);

            game.FindAndMakeBestMove();

            var expectedFen = "8/8/4k3/8/8/8/1K6/8 w - - 1 2";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        #endregion AlphaBeta

        #endregion Simple capture

        [TestMethod]
        private void TestPlayingGameSimpleMaterialGain()
        {
            var scoreCalculator = new ScoreCalculator(_resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, true, 4, 0);
            game.PlacePiece(PieceType.King, false, 4, 7);

            game.PlacePiece(PieceType.Pawn, true, 0, 3);
            game.PlacePiece(PieceType.Pawn, true, 7, 3);

            game.PlacePiece(PieceType.Pawn, false, 0, 5);
            game.PlacePiece(PieceType.Pawn, false, 7, 5);

            game.PlacePiece(PieceType.Pawn, true, 3, 4);
            game.PlacePiece(PieceType.Pawn, false, 4, 5);

            game.AllowAllCastling(false);

            //game.Play();
        }

        #region UCI interface tests

        //[TestMethod]
        //public void TestReceiveUCIMoves()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestReceiveUCIMove()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void TestReceiveUCIMove_Castling()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod]
        //public void FindBestMove_UCI()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion UCI interface tests


    }
}
