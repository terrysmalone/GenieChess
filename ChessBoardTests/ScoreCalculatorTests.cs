using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;
using ChessGame.PossibleMoves;
using ChessGame.ResourceLoading;
using ChessGame.ScoreCalculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessGame.NotationHelpers;

namespace ChessEngineTests
{
    [TestClass]  
    public class ScoreCalculatorTests
    {
        [TestMethod]
        public void TestInitialPositionIsEven()
        {
            Board board = new Board();

            board.InitaliseStartingPosition();

            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Assert.AreEqual(0, score);

        }

        [TestMethod]
        public void TestTwoMovesAreEqual_Pawns()
        {
            Board board = new Board();

            board.InitaliseStartingPosition();
            board.MakeMove(LookupTables.E2, LookupTables.E4, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);
            board.MakeMove(LookupTables.E7, LookupTables.E5, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);
            
            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Assert.AreEqual(0, score);
        }

        [TestMethod]
        public void TestTwoMovesAreEqual_Knights()
        {
            Board board = new Board();

            board.InitaliseStartingPosition();
            board.MakeMove(LookupTables.G1, LookupTables.F3, PieceType.Knight, SpecialMoveType.Normal, true);
            board.MakeMove(LookupTables.G8, LookupTables.F6, PieceType.Knight, SpecialMoveType.Normal, true);
            
            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Assert.AreEqual(0, score);
        }

        #region sanity checks

        #region pawn structure - Checks that player is rewarded for a better pawn structure

        [TestMethod]
        public void TestScoreCalculatorSanityCheck_PawnStructure1()
        {
            Board board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1"));

            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Assert.IsTrue(score > 0);
        }

        [TestMethod]
        public void TestScoreCalculatorSanityCheck_PawnStructure2()
        {
            Board board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1"));

            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Assert.IsTrue(score < 0);
        }

        [TestMethod]
        public void TestScoreCalculatorSanityCheck_PawnStructure3()
        {
            Board board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1"));

            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);

            Board board2 = new Board();

            board2.SetPosition(FenTranslator.ToBoardState("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1"));

            
            decimal score2 = scoreCalculator.CalculateScore(board2);

            Assert.IsTrue(score == -score2);
        }

        #endregion sanity checks

        [TestMethod]
        public void TestScoreCalculatorSanityCheck()
        {
            Board board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k2B/8/6N1/8/2N5/4P3/8/4K3 w - - 0 1"));

            ScoreCalculator scoreCalculator = new ScoreCalculator(ResourceLoader.GetTestResourcePath("ScoreValues.xml"));

            decimal score = scoreCalculator.CalculateScore(board);
        }            

        #endregion pawn structure

    }
}
