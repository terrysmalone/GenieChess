namespace ChessEngine.ScoreCalculation
{
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

        public int EarlyQueenMovePenalty { get; set; }

        public int DoubleBishopScore { get; set; }

        public int  SoloQueenScore { get; set; }

        public int DoubledPawnPenalty { get; set; }
        public int PawnChainScore { get; set; }

        public int PassedPawnBonus { get; set; }

        public int PassedPawnAdvancementBonus { get; set; }

        public int ConnectedRookBonus { get; set; }

        public int BoardCoverageBonus { get; set; }
        public int QueenCoverageBonus { get; set; }
        public int RookCoverageBonus { get; set; }
        public int BishopCoverageBonus { get; set; }
        public int AttackBonus { get; set; }
        public int MoreValuablePieceAttackBonus { get; set; }
    }
}
