using ChessEngine;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;

namespace ChessEngineTests.ScoreCalculation;

[TestFixture]
public class ScoreCalculatorTests
{
    [Test]
    public void InitialPositionIsEven()
    {
        var board = new Board();

        board.InitaliseStartingPosition();

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        Assert.That(score, Is.EqualTo(0));
    }

    [Test]
    public void TwoMovesAreEqual_Pawns()
    {
        var board = new Board();
        var pieceMover = new PieceMover(board);

        board.InitaliseStartingPosition();
        pieceMover.MakeMove(LookupTables.E2, LookupTables.E4, PieceType.Pawn, SpecialMoveType.DoublePawnPush);
        pieceMover.MakeMove(LookupTables.E7, LookupTables.E5, PieceType.Pawn, SpecialMoveType.DoublePawnPush);

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        Assert.That(score, Is.EqualTo(0));
    }

    [Test]
    public void TwoMovesAreEqual_Knights()
    {
        var board = new Board();
        var pieceMover = new PieceMover(board);

        board.InitaliseStartingPosition();
        pieceMover.MakeMove(LookupTables.G1, LookupTables.F3, PieceType.Knight, SpecialMoveType.Normal);
        pieceMover.MakeMove(LookupTables.G8, LookupTables.F6, PieceType.Knight, SpecialMoveType.Normal);

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        Assert.That(score, Is.EqualTo(0));
    }

    [Test]
    public void ScoreCalculatorSanityCheck_PawnStructure1()
    {
        var board = new Board();

        board.SetPosition("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1");

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        Assert.That(score, Is.GreaterThan(0));
    }

    [Test]
    public void ScoreCalculatorSanityCheck_PawnStructure2()
    {
        var board = new Board();

        board.SetPosition("4k3/pp4pp/2p2p2/3pp3/8/1P4P1/PP1PPPP1/4K3 b - - 0 1");

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        Assert.That(score, Is.LessThan(0));
    }

    [Test]
    public void ScoreCalculatorSanityCheck_PawnStructure3()
    {
        var board = new Board();

        board.SetPosition("4k3/pp1pppp1/1p4p1/8/3PP3/2P2P2/PP4PP/4K3 w - - 0 1");

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

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

        var scoreCalculator = ScoreCalculatorFactory.Create();

        var score = scoreCalculator.CalculateScore(board);

        var board2 = new Board();

        board2.SetPosition("3k4/8/8/8/8/3Q4/8/3K4 w - - 0 1");

        var score2 = scoreCalculator.CalculateScore(board2);

        Assert.That(score, Is.EqualTo(-score2));
    }

    [Test]
    public void PieceScore_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            PawnPieceValue = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("P7/8/8/8/8/8/8/8 w - - 0 1"); // just a white pawn
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("p7/8/8/8/8/8/8/8 w - - 0 1"); // just a black pawn
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Knight()
    {
        var scoreValues = new ScoreValues
        {
            KnightPieceValue = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("N7/8/8/8/8/8/8/8 w - - 0 1"); // just a white knight
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("n7/8/8/8/8/8/8/8 w - - 0 1"); // just a black black
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            BishopPieceValue = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("B7/8/8/8/8/8/8/8 w - - 0 1"); // just a white bishop
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("b7/8/8/8/8/8/8/8 w - - 0 1"); // just a black bishop
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Rook()
    {
        var scoreValues = new ScoreValues
        {
            RookPieceValue = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("R7/8/8/8/8/8/8/8 w - - 0 1"); // just a white Rook
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("r7/8/8/8/8/8/8/8 w - - 0 1"); // just a black Rook
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_Queen()
    {
        var scoreValues = new ScoreValues
        {
            QueenPieceValue = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("Q7/8/8/8/8/8/8/8 w - - 0 1"); // just a white Queen
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("q7/8/8/8/8/8/8/8 w - - 0 1"); // just a black Queen
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void PieceScore_King()
    {
        var scoreCalculator = new ScoreCalculator(new ScoreValues());

        var board = new Board();

        board.SetPosition("K7/8/8/8/8/8/8/8 w - - 0 1"); // just a white King
        Assert.That(scoreCalculator.CalculateScore(board), Is.GreaterThan(0));

        board.SetPosition("k7/8/8/8/8/8/8/8 w - - 0 1"); // just a black King
        Assert.That(scoreCalculator.CalculateScore(board), Is.LessThan(0));
    }

    [Test]
    public void DoubledPawnScore()
    {
        var scoreValues = new ScoreValues
        {
            DoubledPawnScore = -15
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/3p4/4p3/8/4P3/4P3/8/8 w - - 0 1"); // White has doubled pawns
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-15));

        board.SetPosition("8/p7/p7/8/4PP2/8/8/8 w - - 0 1"); //  Black has doubled pawns
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(15));

        board.SetPosition("8/p5p1/p5p1/8/4PP2/8/8/8 w - - 0 1"); //  Black has 2 sets of doubled pawns
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(30));
    }

    [Test]
    public void ProtectedPawnScore()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/p4pp1/p7/2P5/3P4/4P3/8/8 w - - 0 1"); // White has a pawn chain of 3
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(20));

        board.SetPosition("8/1p6/2p5/3p4/4p3/2PPP3/8/8 b - - 0 1"); // Black has a pawn chain of 4
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-30));
    }

    [Test]
    public void ProtectedPawnScore_MultipleProtectsAreCountedOnce()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/1p1pp2p/8/8/3P4/2P1P3/8/Kk6 b - - 0 1"); // White has an "arrow" position
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(10));
    }

    [Test]
    public void ProtectedPawnScore_ProtectingMultiplePiecesIsCountedTwice()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/3p2p1/2p1p3/8/3PP3/1P6/8/8 w - - 0 1"); // black has an inverse "arrow" position
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-20));
    }

    [Test]
    public void ProtectedPawnScore_ProtectingAnEdge()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/6p1/2ppp3/8/P7/1P2P3/8/8 b - - 0 1"); // White has a protected pawn on a
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(10));
    }

    [Test]
    public void ProtectedPawnScore_ProtectedByEdgeEdge()
    {
        var scoreValues = new ScoreValues
        {
            ProtectedPawnScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/7p/2pp2p1/8/8/PP2P3/8/8 b - - 0 1"); // Black is protecting from h
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-10));
    }

    [Test]
    public void PassedPawnScore_Single()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnScore = 15
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/2pp4/8/8/PP2P3/8/8 w - - 0 1"); // White has 1 passed pawn
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(15));

        board.SetPosition("8/6p1/1ppp4/8/8/PP2P3/8/8 w - - 0 1"); // Black has 1 passed pawn
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-15));
    }

    [Test]
    public void PassedPawnScore_MultiWhite()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnScore = 15
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/2pp2PP/8/8/PP2P3/8/8 w - - 0 1"); // White has 3 passed pawn
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(45));

        board.SetPosition("8/7p/1ppp3P/8/5p2/PP6/8/8 w - - 0 1"); // Black has 2 passed pawn
    }

    [Test]
    public void PassedPawnAdvancementScore()
    {
        var scoreValues = new ScoreValues
        {
            PassedPawnAdvancementScore = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/7p/1P5P/P7/5p2/5P2/8/8 b - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(7));

        board.SetPosition("8/1p5p/1P5P/P7/5p2/8/8/Kk6 b - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-3));
    }

    [Test]
    public void InnerCentralSquare_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralPawnScore = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3P4/8/4p3/P7/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("8/8/8/3p4/8/4P3/p7/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void InnerCentralSquare_Knight()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralKnightScore = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/6N1/8/4N3/2n5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));

        board.SetPosition("8/6n1/8/4n3/2N5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));
    }

    [Test]
    public void InnerCentralSquare_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralBishopScore = 7
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/8/1b2Bb2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(7));

        board.SetPosition("8/8/8/8/1B2bB2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-7));
    }

    [Test]
    public void InnerCentralSquare_Rook()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralRookScore = 1000
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/3R4/4r3/3R4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1000));

        board.SetPosition("8/3r4/4R3/3r4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1000));
    }

    [Test]
    public void InnerCentralSquare_Queen()
    {
        var scoreValues = new ScoreValues
        {
            InnerCentralQueenScore = 900
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3QQ3/8/3q4/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1800));

        board.SetPosition("8/8/8/3Q4/1qqqq3/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-900));
    }

    [Test]
    public void OuterCentralSquare_Pawn()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralPawnScore = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3P4/8/4p3/P7/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));

        board.SetPosition("8/8/8/3p4/8/4P3/p7/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));
    }

    [Test]
    public void OuterCentralSquare_Knight()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralKnightScore = 1
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/6N1/8/4N3/2n5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1));

        board.SetPosition("8/6n1/8/4n3/2N5/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1));
    }

    [Test]
    public void OuterCentralSquare_Bishop()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralBishopScore = 7
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/8/1b2Bb2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-7));

        board.SetPosition("8/8/8/8/1B2bB2/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(7));
    }

    [Test]
    public void OuterCentralSquare_Rook()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralRookScore = 1000
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/3R4/4r3/3R4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-1000));

        board.SetPosition("8/3r4/4R3/3r4/8/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1000));
    }

    [Test]
    public void OuterCentralSquare_Queen()
    {
        var scoreValues = new ScoreValues
        {
            OuterCentralQueenScore = 900
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("8/8/8/3QQ3/8/3q4/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-900));

        board.SetPosition("8/8/8/2Qq4/2QQ4/8/8/8 w - - 0 1");
        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(1800));
    }

    [Test]
    public void CastlingIsIgnoredInEndGame()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 1,
            CastlingKingSideScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("5rk1/8/8/8/8/8/8/1K1R4 w - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(0));
    }

    [Test]
    public void KingCastling_WhiteKingSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 10
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("r3k2r/4p3/2b5/8/8/8/2B2PP1/R4RK1 w - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(10));
    }

    [Test]
    public void KingCastling_WhiteQueenSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 8
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("r3k2r/4p3/2b5/8/8/8/2B2PP1/2KR1R2 b - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(8));
    }

    [Test]
    public void KingCastling_BlackKingSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 12
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("4rrk1/4p3/2b5/8/8/8/2B2PP1/1K1R1R2 w - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-12));
    }

    [Test]
    public void KingCastling_BlackQueenSide()
    {
        var scoreValues = new ScoreValues
        {
            CastlingQueenSideScore = 4
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("2kr3r/4p3/2b5/8/8/8/2B2PP1/1K1R1R2 w - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(-4));
    }

    [Test]
    public void KingCastling_SidesCancelEachOtherOut()
    {
        var scoreValues = new ScoreValues
        {
            CastlingKingSideScore = 15
        };

        var scoreCalculator = new ScoreCalculator(scoreValues);

        var board = new Board();

        board.SetPosition("4rrk1/4p3/2b5/8/8/8/2B2PP1/3R1RK1 b - - 0 1");

        Assert.That(scoreCalculator.CalculateScore(board), Is.EqualTo(0));
    }

    // TODO: Tests for Can castle

    // TODO: Tests for endgame castling

    // TODO: Tests for piece development

    // TODO: Tests for connected rooks

    // TODO: Tests for early queen action

    // TODO: Tests for square tables

    // TODO: Tests for coverage

    // TODO: Tests for Piece Attacks

    // TODO: Tests for MoreValuablePieceAttacks
}

