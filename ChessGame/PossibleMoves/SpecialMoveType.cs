using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.PossibleMoves
{
    public enum SpecialMoveType
    {
        Normal = 0,
        DoublePawnPush,
        KingCastle,
        QueenCastle,
        Capture,
        ENPassantCapture,
        KnightPromotion,
        BishopPromotion,
        RookPromotion,
        QueenPromotion,
        KnightPromotionCapture,
        BishopPromotionCapture,
        RookPromotionCapture,
        QueenPromotionCapture
    }
}
