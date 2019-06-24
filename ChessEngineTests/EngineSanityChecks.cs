using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using ResourceLoading;

namespace ChessEngineTests
{
    [TestClass]
    public class EngineSanityChecks
    {
        private readonly IResourceLoader resourceLoader = new ResourceLoader();

        #region Tests that player attempts check mate

        #region Mate in one

        [TestMethod]
        public void TestMateInOne_White_Depth2()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

            game.ThinkingDepth = 2;
            game.FindAndMakeBestMove();

            Assert.AreEqual("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_White_Depth3()
        {
            ;
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

            game.ThinkingDepth = 3;
            game.FindAndMakeBestMove();

            Assert.AreEqual("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_White_Depth4()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

            game.ThinkingDepth = 4;
            game.FindAndMakeBestMove();

            Assert.AreEqual("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_White_Depth5()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

            game.ThinkingDepth = 5;
            game.FindAndMakeBestMove();

            Assert.AreEqual("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_White_Depth6()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/2p2p1r/8/Q7/8/8/8/3K2R1 w - - 0 1");

            game.ThinkingDepth = 6;
            game.FindAndMakeBestMove();

            Assert.AreEqual("Q6k/2p2p1r/8/8/8/8/8/3K2R1 b - - 1 1",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_Black_Depth2()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

            game.ThinkingDepth = 2;
            game.FindAndMakeBestMove();

            Assert.AreEqual("1k4q1/7r/8/8/8/8/8/7K w - - 1 2",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_Black_Depth3()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

            game.ThinkingDepth = 3;
            game.FindAndMakeBestMove();

            Assert.AreEqual("1k4q1/7r/8/8/8/8/8/7K w - - 1 2",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_Black_Depth4()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

            game.ThinkingDepth = 4;
            game.FindAndMakeBestMove();

            Assert.AreEqual("1k4q1/7r/8/8/8/8/8/7K w - - 1 2",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_Black_Depth5()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

            game.ThinkingDepth = 5;
            game.FindAndMakeBestMove();

            Assert.AreEqual("1k4q1/7r/8/8/8/8/8/7K w - - 1 2",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        [TestMethod]
        public void TestMateInOne_Black_Depth6()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("1k4q1/2r5/8/8/8/8/8/7K b - - 0 1");

            game.ThinkingDepth = 6;
            game.FindAndMakeBestMove();

            Assert.AreEqual("1k4q1/7r/8/8/8/8/8/7K w - - 1 2",
                            FenTranslator.ToFENString(game.GetCurrentBoardState()));
        }

        #endregion Mate in one

        #endregion Tests that player attempts check mate

        #region Horizon problem 

        #region Check horizon

        /// <summary>
        /// Tests the horizon problem - where a sure win is outwith the search depth. 
        /// 
        /// In the below example white is guaranteed a win
        /// </summary>
        [TestMethod]
        public void TestHorizon_1()
        {
            throw new NotImplementedException();

            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("8/1p1P4/k1p5/8/8/3PPPPP/r7/7K b - - 0 1");
        }

        [TestMethod]
        public void TestHorizon_2()
        {
            throw new NotImplementedException();

            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("5r1k/4Qpq1/4p3/1p1p2P1/2p2P2/1p2P3/3P4/BK6 b - -");

        }

        /// <summary>
        /// Leonid's Position
        /// </summary>
        [TestMethod, Timeout(5000)]
        public void TestForCheckExplosion()
        {
            throw new NotImplementedException();

            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("q2k2q1/2nqn2b/1n1P1n1b/2rnr2Q/1NQ1QN1Q/3Q3B/2RQR2B/Q2K2Q1 w - -");

            game.ThinkingDepth = 1;
            game.FindAndMakeBestMove();
        }

        #endregion Check horizon

        #endregion Horizon problem

        /// <summary>
        /// Tests zugzwang causing null moves to fail
        /// </summary>
        [TestMethod]
        public void TestNullMove_1()
        {
            throw new NotImplementedException();
        }

        #region Iterative deepening

        #endregion Iterative deepening        

        [TestMethod]
        public void TestKingCastlingWhileInCheckBug()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("r3k2r/p2b1ppp/2p2n2/b2p4/5B2/3B4/PPP1NPPP/R3K2R w KQkq - 0 1");

            var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

            Assert.AreNotEqual("e1g1", move);
        }

        #region Quiescence search checks

        // If we search this position to a shallow depth of 2 it looks like white would lose the exchange
        // The quiescence search should broaden the search to show that white easily wins
        [TestMethod]
        public void QuiescenceCausesWhiteToCapture()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("3k4/3r4/3r4/8/3Q4/3R4/8/3K4 w - - 0 1");

            game.ThinkingDepth = 2;

            var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

            Assert.AreEqual("d4d6", move);
        }

        // At a depth of 2 this moe seems like white would win from the capture but the 
        // quiescence should show otherwise
        [TestMethod]
        public void QuiescenceStopsWhiteFromCapturing()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("8/8/3k4/3r4/3P4/8/3n1Q2/3K4 w - - 0 1");

            game.ThinkingDepth = 2;

            var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

            Assert.AreNotEqual("f2d2", move);

        }

        // If we search this position to a shallow depth of 2 it looks like black would lose the exchange
        // The quiescence search should broaden the search to show that black easily wins
        [TestMethod]
        public void QuiescenceCausesBlackToCapture()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("7k/6bp/5q2/8/3R4/8/1Q6/K7 b - - 0 1");

            game.ThinkingDepth = 2;

            var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

            Assert.AreEqual("f6d4", move);
        }

        // At a depth of 2 this move seems like black would win from the capture but the 
        // quiescence should show otherwise
        [TestMethod]
        public void QuiescenceStopsBlackFromCapturing()
        {
            var scoreCalculator = new ScoreCalculator(resourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var game = new Game(scoreCalculator, new Board(), openingBook: null);
            game.ClearBoard();
            game.SetFENPosition("5n2/4k3/5q2/4N3/4p3/1QN5/3KR3/8 b - - 0 1");

            game.ThinkingDepth = 2;

            var move = UciMoveTranslator.ToUciMove(game.GetBestMove());

            Assert.AreNotEqual("f6e5", move);

        }

        #endregion Quiescence search checks
    }


}
