using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

public class KingPositionScoreCalculator : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    // TODO: Bonus if king is protected by pieces (esp pawns) maybe fianchetto'd bishop
    // TODO: Penalty if king moves early game and prevents himself from castling
    // TODO: Deducts points (not in end game) if the king is under attack
    
    public KingPositionScoreCalculator(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
        //scoreValues.KingProtectionScore;
        //scoreValues.EarlyMoveKingScore;

    }
    public int Calculate(Board currentBoard)
    {
        var score = CalculateEarlyMoveKingScore(currentBoard);

        return score;
    }

    private int CalculateEarlyMoveKingScore(Board currentBoard)
    {
        var score = 0;

        var row1 = 255u;
        var row8 = 18374686479671623680u;

        var emptySpaceCount = 4;


        var whitePieces = currentBoard.AllWhiteOccupiedSquares;

        if (BitboardOperations.GetPopCount(~whitePieces & row1) < emptySpaceCount)
        {
            // if king has moved add score
            // We might want to consider a more intelligent check  here (i.e. It's valid for the king to have moved to castle)

            if ((currentBoard.WhiteKing & 16u) == 0)
            {
                score += _scoreValues.EarlyMoveKingScore;
            }
        }

        var blackPieces = currentBoard.AllBlackOccupiedSquares;

        if (BitboardOperations.GetPopCount(~blackPieces & row8) < emptySpaceCount)
        {
            // if king has moved add score
            if ((currentBoard.BlackKing & 1152921504606846976u) == 0)
            {
                score -= _scoreValues.EarlyMoveKingScore;
            }
        }

        return score;
    }
}
