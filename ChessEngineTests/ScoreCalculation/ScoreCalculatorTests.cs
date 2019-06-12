using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;
using ResourceLoading;

namespace ChessEngineTests.ScoreCalculation
{
    [TestFixture]  
    public class ScoreCalculatorTests
    {
        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        [Test]
        public void TestInitialPositionIsEven()
        {
            var board = new Board();

            board.InitaliseStartingPosition();

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));
            
            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void TestTwoMovesAreEqual_Pawns()
        {
            var board = new Board();

            board.InitaliseStartingPosition();
            board.MakeMove(LookupTables.E2, LookupTables.E4, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);
            board.MakeMove(LookupTables.E7, LookupTables.E5, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);
            
            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void TestTwoMovesAreEqual_Knights()
        {
            var board = new Board();

            board.InitaliseStartingPosition();
            board.MakeMove(LookupTables.G1, LookupTables.F3, PieceType.Knight, SpecialMoveType.Normal, true);
            board.MakeMove(LookupTables.G8, LookupTables.F6, PieceType.Knight, SpecialMoveType.Normal, true);
            
            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        #region sanity checks

        #region pawn structure - Checks that player is rewarded for a better pawn structure

        [Test]
        public void TestScoreCalculatorSanityCheck_PawnStructure1()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1"));

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.GreaterThan(0));
        }

        [Test]
        public void TestScoreCalculatorSanityCheck_PawnStructure2()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1"));

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.LessThan(0));
        }

        [Test]
        public void TestScoreCalculatorSanityCheck_PawnStructure3()
        {
            var board = new Board();

            board.SetPosition(FenTranslator.ToBoardState("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1"));

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            var board2 = new Board();

            board2.SetPosition(FenTranslator.ToBoardState("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1"));

            
            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score, Is.EqualTo(-score2));
        }

        #endregion sanity checks

        #endregion pawn structure
    }
}
