using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

public class KingPositionScoreCalculator : IScoreCalculation
{

    // TODO: Bonus if king is protected by pieces (esp pawns) maybe fianchetto'd bishop
    // TODO: Penalty if king moves early game and prevents himself from castling
    // TODO: Deducts points (not in end game) if the king is under attack
    
    public KingPositionScoreCalculator(ScoreValues scoreValues)
    {
        throw new NotImplementedException();
    }
    public int Calculate(Board currentBoard)
    {
        throw new NotImplementedException();
    }
}