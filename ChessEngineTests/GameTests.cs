﻿using ChessEngine;
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
        private readonly IResourceLoader resourceLoader = new ResourceLoader();
        
        //[TestMethod]
        //private void TestZobrist()
        //{
        //    var watch = new Stopwatch();
        //    Game game = new Game(resourceLoader.LoadScoreValues("ScoreValues.xml"));

        //    game.SetSearchType(SearchStrategy.AlphaBeta);
        //    game.ThinkingDepth =6;

        //    game.InitaliseStartingPosition();
        //    //game.SetFENPosition("r1bqkb1r/pppp1ppp/2n2n2/4P3/4P3/2N5/PPP2PPP/R1BQKBNR b KQkq - 0 1");
        //    CountDebugger.ClearAll();

        //    while (true)
        //    {
        //        Console.WriteLine("Make move:");
        //        string move = Console.ReadLine();
        //        game.ReceiveUciMove(move);

        //        string bestMove = game.FindBestMove_UCI();
        //        game.ReceiveUciMove(bestMove);
        //    }
            
        //    //string bestMove = game.FindBestMove_UCI();
        //    //game.ReceiveUCIMove(bestMove);

        //    //while (true)
        //    //{
        //    //    string bestMove = game.FindBestMove_UCI();
        //    //    game.ReceiveUCIMove(bestMove);
        //    //}
        //    //int[] transpositionSearches = new int[50];
        //    //int[] evaluations = new int[50];

        //    //int toalTranspositionSearches = 0;
        //    //int totalEvaluations = 0;
        //    //for (int i = 0; i < 20; i++)
        //    //{
        //        //string bestMove = game.FindBestMove_UCI();
        //        //game.ReceiveUCIMove(bestMove);
        //    //    transpositionSearches[i] = CountDebugger.Transposition_Searches; 
        //    //    toalTranspositionSearches += CountDebugger.Transposition_Searches; 
                
        //    //    evaluations[i] = CountDebugger.Evaluations;
        //    //    totalEvaluations += CountDebugger.Evaluations;
        //    //}

        //    //int averageEvaluations = totalEvaluations / 50;
        //    //int averageTranspositionSearches = toalTranspositionSearches / 50;
            
        //    //bestMove = game.FindBestMove_UCI();
        //    //game.ReceiveUCIMove(bestMove);
        //    //int move2Seach = TranspositionDebugger.Transposition_TotalSearches;
            
        //    //bestMove = game.FindBestMove_UCI();
        //    //game.ReceiveUCIMove(bestMove);
        //    //int move3Seach = TranspositionDebugger.Transposition_TotalSearches;
            
        //    //game.ReceiveUCIMove("e2e4");

        //    //game.SetFENPosition("4k3/8/8/8/5b2/4P3/5r1p/7K w - - 0 1");            

            

        //    //string bestMove = game.FindBestMove_UCI();
        //    //game.ReceiveUCIMove(bestMove);

        //    //game.ReceiveUCIMove("e4e5");
        //    //string bestMove2 = game.FindBestMove_UCI();

        //}

        //[TestMethod]
        //private void TestComputerPlayingFullGame()
        //{
        //    ScoreCalculator scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));

        //    Game game = new Game(scoreCalculator);

        //    scoreCalculator.CalculateScore(game.CurrentBoard);

        //    game.WhiteSearchType = SearchStrategy.AlphaBeta;
        //    game.BlackSearchType = SearchStrategy.AlphaBeta;

        //    game.ThinkingDepth = 5;
                        
        //    //game.Play();
        //}

        #region simple capture

        #region MiniMax

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_MiniMax_1()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

            game.ThinkingDepth = 1;

            game.AllowAllCastling(false);
            
            game.FindAndMakeBestMove();

            var expectedFen = "7k/8/8/3K4/8/8/8/8 b - - 1 1";
            var fenNotation = FenTranslator.ToFENString(game.GetCurrentBoardState());

            //Tests
            Assert.AreEqual(expectedFen, fenNotation);
        }

        [TestMethod]
        public void TestPlayingGame_VerySimpleCapture_White_MiniMax_3()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 3, 4);
            game.PlacePiece(PieceType.King, PieceColour.Black, 7, 7);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));
            var game = new Game(scoreCalculator, new Board(), openingBook: null);

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
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetTestResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);

            game.ClearBoard();

            game.PlacePiece(PieceType.King, PieceColour.White, 4, 0);
            game.PlacePiece(PieceType.King, PieceColour.Black, 4, 7);

            game.PlacePiece(PieceType.Pawn, PieceColour.White, 0, 3);
            game.PlacePiece(PieceType.Pawn, PieceColour.White, 7, 3);

            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 0, 5);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 7, 5);

            game.PlacePiece(PieceType.Pawn, PieceColour.White, 3, 4);
            game.PlacePiece(PieceType.Pawn, PieceColour.Black, 4, 5);

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