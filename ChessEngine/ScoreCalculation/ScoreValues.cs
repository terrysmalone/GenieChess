// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace ChessEngine.ScoreCalculation;

public class ScoreValues
{
    public int PawnPieceValue { get; set; }

    public int KnightPieceValue { get; set; }

    public int BishopPieceValue { get; set; }

    public int RookPieceValue { get; set; }

    public int QueenPieceValue { get; set; }

    public int InnerCentralPawnScore { get; set; }
    public int OuterCentralPawnScore { get; set; }


    public int InnerCentralKnightScore { get; set; }
    public int OuterCentralKnightScore { get; set; }

    public int InnerCentralBishopScore { get; set; }
    public int OuterCentralBishopScore { get; set; }

    public int InnerCentralRookScore { get; set; }
    public int OuterCentralRookScore { get; set; }

    public int InnerCentralQueenScore { get; set; }
    public int OuterCentralQueenScore { get; set; }

    public int CastlingKingSideScore { get; set; }
    public int CastlingQueenSideScore { get; set; }
    public int CanCastleKingsideScore { get; set; }
    public int CanCastleQueensideScore { get; set; }

    public int[] PawnSquareTable { get; } = new int[64];

    public int[] KnightSquareTable { get; } = new int[64];

    public int[] BishopSquareTable { get; } = new int[64];

    public int[] KingSquareTable { get; } = new int[64];

    public int[] KingEndGameSquareTable { get; } = new int[64];

    public int DevelopedPieceScore { get; set; }

    public int EarlyQueenMoveScore { get; set; }

    public int DoubleBishopScore { get; set; }

    public int  SoloQueenScore { get; set; }

    public int DoubledPawnScore { get; set; }
    public int ProtectedPawnScore { get; set; }

    public int PassedPawnScore { get; set; }

    public int PassedPawnAdvancementScore { get; set; }

    public int ConnectedRookScore { get; set; }

    public int BoardCoverageScore { get; set; }
    public int QueenCoverageScore { get; set; }
    public int RookCoverageScore { get; set; }
    public int BishopCoverageScore { get; set; }
    public int AttackScore { get; set; }
    public int MoreValuablePieceAttackScore { get; set; }

    public int KingProtectionScore { get; set; }
    public int EarlyMoveKingScore { get; set; }
}

