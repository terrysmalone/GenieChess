using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;

namespace ChessEngine.ScoreCalculation;

internal sealed class PawnStructureScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;
    
    public PawnStructureScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }

    public int Calculate(Board currentBoard)
    {
        var pawnStructureScore = CalculateDoubledPawnScore(currentBoard);
        pawnStructureScore += CalculateProtectedPawnScore(currentBoard);
        pawnStructureScore += CalculatePassedPawnScore(currentBoard);

        return pawnStructureScore;
    }
    
    private int CalculateDoubledPawnScore(Board currentBoard)
    {
        var doubledPawnScore = 0;

        var whiteDoubleCount = 0;
        var blackDoubleCount = 0;

        for (var i = 0; i < 8; i++)
        {
            var mask = LookupTables.ColumnMaskByColumn[i];

            if (BitboardOperations.GetPopCount(mask & currentBoard.WhitePawns) > 1)
            {
                whiteDoubleCount++;
            }

            if (BitboardOperations.GetPopCount(mask & currentBoard.BlackPawns) > 1)
            {
                blackDoubleCount++;
            }
        }

        doubledPawnScore += whiteDoubleCount * _scoreValues.DoubledPawnScore;
        doubledPawnScore -= blackDoubleCount * _scoreValues.DoubledPawnScore;

        return doubledPawnScore;
    }

    private int CalculateProtectedPawnScore(Board currentBoard)
    {

        var protectedPawnScore = 0;

        //Pawn chain
        var notA = 18374403900871474942;
        ulong notH = 9187201950435737471;

        //White pawns
        var wPawnAttackSquares = ((currentBoard.WhitePawns << 9) & notA) | (currentBoard.WhitePawns << 7) & notH;
        var wProtectedPawns = wPawnAttackSquares & currentBoard.WhitePawns;

        protectedPawnScore += BitboardOperations.GetPopCount(wProtectedPawns) * _scoreValues.ProtectedPawnScore;

        //Black pawns
        var bPawnAttackSquares = ((currentBoard.BlackPawns >> 9) & notH) | (currentBoard.BlackPawns >> 7) & notA;
        var bProtectedPawns = bPawnAttackSquares & currentBoard.BlackPawns;

        protectedPawnScore -= BitboardOperations.GetPopCount(bProtectedPawns) * _scoreValues.ProtectedPawnScore;

        return protectedPawnScore;
    }

    private int CalculatePassedPawnScore(Board currentBoard)
    {
        var passedPawnScore = 0;

        foreach (var whitePawnBoard in BitboardOperations.SplitBoardToArray(currentBoard.WhitePawns))
        {
            // Pawn is on 7th rank so it can promote
            if ((whitePawnBoard & LookupTables.RowMask7) != 0)
            {
                passedPawnScore += _scoreValues.PassedPawnScore;
                passedPawnScore += _scoreValues.PassedPawnAdvancementScore * 5;

                continue;
            }

            var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(whitePawnBoard);

            var pawnFrontSpan = LookupTables.WhitePawnFrontSpan[pawnIndex];

            if ((pawnFrontSpan & currentBoard.BlackPawns) == 0)
            {
                passedPawnScore += _scoreValues.PassedPawnScore;
                passedPawnScore += _scoreValues.PassedPawnAdvancementScore * ((pawnIndex / 8) - 1);
            }
        }

        foreach (var blackPawnBoard in BitboardOperations.SplitBoardToArray(currentBoard.BlackPawns))
        {
            // TODO: Try doing this for all rows. It might be quicker
            //Pawn is on 2nd rank so it can promote
            if ((blackPawnBoard & LookupTables.RowMask2) != 0)
            {
                passedPawnScore -= _scoreValues.PassedPawnScore;
                passedPawnScore -= _scoreValues.PassedPawnAdvancementScore * 5;
                continue;
            }

            var pawnIndex = BitboardOperations.GetSquareIndexFromBoardValue(blackPawnBoard);

            var pawnFrontSpan = LookupTables.BlackPawnFrontSpan[pawnIndex];

            if ((pawnFrontSpan & currentBoard.WhitePawns) == 0)
            {
                passedPawnScore -= _scoreValues.PassedPawnScore;
                passedPawnScore -= _scoreValues.PassedPawnAdvancementScore * (8 - (pawnIndex / 8) - 2);
            }
        }

        return passedPawnScore;
    }
}
