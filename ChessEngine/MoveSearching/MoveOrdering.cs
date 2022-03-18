using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;

namespace ChessEngine.MoveSearching;

internal static class MoveOrdering
{
    internal static void OrderMovesByMvvLva(Board boardPosition, IList<PieceMove> moveList)
    {
        // move list position, victim score, attacker score
        var ordering = new List<Tuple<PieceMove, int, int>>();

        var toRemove = new List<int>();

        //Move capture
        for (var moveNum = 0; moveNum < moveList.Count; moveNum++)
        {
            if (moveList[moveNum].SpecialMove == SpecialMoveType.Capture
                || moveList[moveNum].SpecialMove == SpecialMoveType.ENPassantCapture
                || IsPromotionCapture(moveList[moveNum].SpecialMove))
            {
                var victimType = PieceChecking.GetPieceTypeOnSquare(boardPosition, moveList[moveNum].Moves);

                ordering.Add(new Tuple<PieceMove, int, int>(
                                                            moveList[moveNum],
                                                            GetPieceScore(victimType),
                                                            GetPieceScore(moveList[moveNum].Type)));

                toRemove.Add(moveNum);
            }
        }

        //We need to remove them in reverse so we don't change the numbers
        // of the others to remove
        toRemove = toRemove.OrderByDescending(t => t).ToList();

        foreach (var remove in toRemove)
        {
            moveList.RemoveAt(remove);
        }

        //Order by victim and then attacker. We do it in reverse
        ordering = ordering.OrderByDescending(o => o.Item2).ThenBy(o => o.Item3).ToList();

        for (var orderPosition = 0; orderPosition < ordering.Count; orderPosition++)
        {
            moveList.Insert(orderPosition, ordering[orderPosition].Item1);
        }
    }

    private static bool IsPromotionCapture(SpecialMoveType specialMoveType)
    {
        return    specialMoveType is SpecialMoveType.BishopPromotionCapture
                                  or SpecialMoveType.KnightPromotionCapture
                                  or SpecialMoveType.RookPromotionCapture
                                  or SpecialMoveType.QueenPromotionCapture;
    }

    private static int GetPieceScore(PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.None or PieceType.Pawn => 1,
            PieceType.Knight => 2,
            PieceType.Bishop => 3,
            PieceType.Rook => 4,
            PieceType.Queen => 5,
            PieceType.King => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(pieceType),
                                                       pieceType,
                                                       $"Invalid piece type {pieceType} given")
        };
    }
}
