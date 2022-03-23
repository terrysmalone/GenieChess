using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;

namespace ChessEngine.ScoreCalculation;

    // Calculates the score of a particular board position
public class ScoreCalculator : IScoreCalculator
{
    private readonly List<IScoreCalculation> _scoreCalculations;

    public ScoreCalculator(List<IScoreCalculation> scoreCalculations)
    {
        _scoreCalculations = scoreCalculations;
    }

    // Calculates the score with black advantage as negative and white as positive
    public int CalculateScore(Board currentBoard)
    {
        CountDebugger.Evaluations++;

        //StaticBoardChecks.Calculate(currentBoard);

        var score = 0;

        foreach (var scoreCalculation in _scoreCalculations)
        {
            score += scoreCalculation.Calculate(currentBoard);
        }

        // TODO: Check number of pieces within range of enemy pieces
        // TODO: Deduction for non protected pieces
        // TODO: Deduction for non protected pawns
        // TODO: Bonus if rook is on an open (by own pawns) file
        // TODO: Bonus if king is protected by pieces (esp pawns) maybe fianchetto'd bishop
        // TODO: Bonus if bishop is fianchettod
        // TODO: Bonus if you are attacking a more valuable piece / unprotected piece with something blocking
        // TODO: Bonus if a piece/pawn is attacking a more valuable / unprotected piece
        // TODO: Bonus if a piece is attacking more than one target
        // TODO: Bonus points for having multiple pieces attacking one enemy piece (multiplier for number of pieces and value of attacked piece)
        // TODO: Penalty if king moves early game and prevents himself from castling
        // TODO: Deducts points (not in end game) if the king is under attack
        // TODO: <ThreateningKingBonus>0.15</ThreateningKingBonus> <!-- Bonus for every attack on the king-->
        // TODO: Add points for attacks behind other peieces. i.e. attacking a rook behind a pawn


        //Add king pawn protection scores
        //Add king position scores (stay away from the middle early/mid game)

        return score;
    }
}

