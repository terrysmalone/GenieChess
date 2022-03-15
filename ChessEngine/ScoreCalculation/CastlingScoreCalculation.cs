using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;

namespace ChessEngine.ScoreCalculation;

internal sealed class CastlingScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    private const byte _endGameCount = 3;

    public CastlingScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }

    public int Calculate(Board currentBoard)
    {
        var score = CalculateCastlingScores(currentBoard);
        score += CalculateCanCastleScores(currentBoard);

        return score;
    }

    private int CalculateCastlingScores(Board currentBoard)
    {
        if (IsEndGame(currentBoard))
        {
            return 0;
        }

        var castlingScore = 0;

        if (currentBoard.WhiteKing == LookupTables.G1 && (currentBoard.WhiteRooks & LookupTables.F1) != 0)
        {
            castlingScore += _scoreValues.CastlingKingSideScore;
        }
        else if (currentBoard.WhiteKing == LookupTables.C1 && (currentBoard.WhiteRooks & LookupTables.D1) != 0)
        {
            castlingScore += _scoreValues.CastlingQueenSideScore;
        }

        if (currentBoard.BlackKing == LookupTables.G8 && (currentBoard.BlackRooks & LookupTables.F8) != 0)
        {
            castlingScore -= _scoreValues.CastlingKingSideScore;
        }
        else if (currentBoard.BlackKing == LookupTables.C8 && (currentBoard.BlackRooks & LookupTables.D8) != 0)
        {
            castlingScore -= _scoreValues.CastlingQueenSideScore;
        }

        return castlingScore;
    }

    private static bool IsEndGame(Board currentBoard)
    {
        if(BitboardOperations.GetPopCount(currentBoard.WhiteNonEndGamePieces) < _endGameCount
           || BitboardOperations.GetPopCount(currentBoard.BlackNonEndGamePieces) < _endGameCount)
        {
            return true;
        }

        return false;
    }

    private int CalculateCanCastleScores(Board currentBoard)
    {
        var canCastleScore = 0;

        var state = currentBoard.GetCurrentBoardState();

        canCastleScore += Convert.ToInt32(state.WhiteCanCastleKingside) * _scoreValues.CanCastleKingsideScore;
        canCastleScore += Convert.ToInt32(state.WhiteCanCastleQueenside) * _scoreValues.CanCastleQueensideScore;

        canCastleScore -= Convert.ToInt32(state.BlackCanCastleKingside) * _scoreValues.CanCastleKingsideScore;
        canCastleScore -= Convert.ToInt32(state.BlackCanCastleQueenside) * _scoreValues.CanCastleQueensideScore;

        return canCastleScore;
    }
}
