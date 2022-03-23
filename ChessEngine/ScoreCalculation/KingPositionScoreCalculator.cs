using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;

namespace ChessEngine.ScoreCalculation;

public class KingPositionScoreCalculator : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    // TODO: Deducts points (not in end game) if the king is under attack
    
    public KingPositionScoreCalculator(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }
    public int Calculate(Board currentBoard)
    {
        var score = CalculateEarlyMoveKingScore(currentBoard);
        score += CalculateKingProtectionScore(currentBoard);

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

    private int CalculateKingProtectionScore(Board currentBoard)
    {
        var score = 0;

        var surroundingWhiteKingSpace = PieceChecking.GetSurroundingSpace(currentBoard.WhiteKing);
        var surroundingWhitePieces = surroundingWhiteKingSpace & currentBoard.AllWhiteOccupiedSquares;

        score += BitboardOperations.GetPopCount(surroundingWhitePieces) * _scoreValues.KingProtectionScore;

        var surroundingBlackKingSpace = PieceChecking.GetSurroundingSpace(currentBoard.BlackKing);
        var surroundingBlackPieces = surroundingBlackKingSpace & currentBoard.AllBlackOccupiedSquares;

        score -= BitboardOperations.GetPopCount(surroundingBlackPieces) * _scoreValues.KingProtectionScore;

        return score;
   }
}
