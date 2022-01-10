using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
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
        public void InitialPositionIsEven()
        {
            var board = new Board();

            board.InitaliseStartingPosition();

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void TwoMovesAreEqual_Pawns()
        {
            var board = new Board();
            var pieceMover = new PieceMover(board);

            board.InitaliseStartingPosition();
            pieceMover.MakeMove(LookupTables.E2, LookupTables.E4, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);
            pieceMover.MakeMove(LookupTables.E7, LookupTables.E5, PieceType.Pawn, SpecialMoveType.DoublePawnPush, true);

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void TwoMovesAreEqual_Knights()
        {
            var board = new Board();
            var pieceMover = new PieceMover(board);

            board.InitaliseStartingPosition();
            pieceMover.MakeMove(LookupTables.G1, LookupTables.F3, PieceType.Knight, SpecialMoveType.Normal, true);
            pieceMover.MakeMove(LookupTables.G8, LookupTables.F6, PieceType.Knight, SpecialMoveType.Normal, true);

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void ScoreCalculatorSanityCheck_PawnStructure1()
        {
            var board = new Board();

            board.SetPosition("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.GreaterThan(0));
        }

        [Test]
        public void ScoreCalculatorSanityCheck_PawnStructure2()
        {
            var board = new Board();

            board.SetPosition("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.LessThan(0));
        }

        [Test]
        public void ScoreCalculatorSanityCheck_PawnStructure3()
        {
            var board = new Board();

            board.SetPosition("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            var board2 = new Board();

            board2.SetPosition("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1");


            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score, Is.EqualTo(-score2));
        }

        [Test]
        public void MirroredBoard_EvenPositions_Knights()
        {
            var board = new Board();

            board.SetPosition("8/5k2/5n2/8/8/2N5/2K5/8 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));

            var board2 = new Board();

            board2.SetPosition("8/2K5/2N5/8/8/5n2/5k2/8 b - - 0 1");
            
            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score2, Is.EqualTo(0));
        }

        [Test]
        public void MirroredBoard_EvenPositions_Knights_and_pawns()
        {
            var board = new Board();

            board.SetPosition("8/5k2/4np2/4p3/3P4/2PN4/2K5/8 b - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));

            var board2 = new Board();

            board2.SetPosition("8/2K5/2PN4/3P4/4p3/4np2/5k2/8 w - - 0 1");

            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score2, Is.EqualTo(0));
        }

        [Test]
        public void MirroredBoard_EvenPositions_Bishops()
        {
            var board = new Board();

            board.SetPosition("4k3/8/8/2b2b2/2B2B2/8/8/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));

            var board2 = new Board();

            board2.SetPosition("4K3/8/8/2B2B2/2b2b2/8/8/4k3 b - - 0 1");

            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score2, Is.EqualTo(0));
        }

        [Test]
        public void MirroredBoard_EvenPositions_Queens_and_pawns()
        {
            var board = new Board();

            board.SetPosition("4k3/3p1p2/3Qp3/8/8/3qP3/3P1P2/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            Assert.That(score, Is.EqualTo(0));

            var board2 = new Board();

            board2.SetPosition("4K3/3P1P2/3qP3/8/8/3Qp3/3p1p2/4k3 b - - 0 1");

            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score2, Is.EqualTo(0));
        }

        [Test]
        public void FlippedBoard_UnEvenPositions_BishopAttackingQueen()
        {
            var board = new Board();

            board.SetPosition("4k3/8/4q3/8/8/1B6/8/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            var board2 = new Board();

            board2.SetPosition("3k4/8/6b1/8/8/3Q4/8/3K4 w - - 0 1");

            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score, Is.EqualTo(-score2));
        }

        [Test]
        public void FlippedBoard_JustQueenCheckingKing()
        {
            var board = new Board();

            board.SetPosition("4k3/8/4q3/8/8/8/8/4K3 w - - 0 1");

            var scoreCalculator = new ScoreCalculator(m_ResourceLoader.GetGameResourcePath("ScoreValues.xml"));

            var score = scoreCalculator.CalculateScore(board);

            var board2 = new Board();

            board2.SetPosition("3k4/8/8/8/8/3Q4/8/3K4 w - - 0 1");

            var score2 = scoreCalculator.CalculateScore(board2);

            Assert.That(score, Is.EqualTo(-score2));
        }
    }
}
