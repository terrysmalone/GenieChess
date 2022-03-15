using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

internal sealed class PieceValuesScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;
    
    public PieceValuesScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }
    
    // Calculate points for the pieces each player has on the board
    public int Calculate(Board currentBoard)
    {
        const int kingScore = 357913941;

        var pieceScore = 0;

        //Calculate white piece values
        pieceScore += BitboardOperations.GetPopCount(currentBoard.WhitePawns) * _scoreValues.PawnPieceValue;
        pieceScore += BitboardOperations.GetPopCount(currentBoard.WhiteKnights) * _scoreValues.KnightPieceValue;

        var whiteBishopCount = BitboardOperations.GetPopCount(currentBoard.WhiteBishops);
        pieceScore += whiteBishopCount * _scoreValues.BishopPieceValue;

        if (whiteBishopCount == 2)
        {
            pieceScore += _scoreValues.DoubleBishopScore;
        }

        pieceScore += BitboardOperations.GetPopCount(currentBoard.WhiteRooks) * _scoreValues.RookPieceValue;

        var whiteQueenCount = BitboardOperations.GetPopCount(currentBoard.WhiteQueen);

        pieceScore += whiteQueenCount * _scoreValues.QueenPieceValue;
        pieceScore += whiteQueenCount * _scoreValues.SoloQueenScore;

        var count = BitboardOperations.GetPopCount(currentBoard.WhiteKing);
        pieceScore += count * kingScore;

        //Calculate black piece values
        pieceScore -= BitboardOperations.GetPopCount(currentBoard.BlackPawns) * _scoreValues.PawnPieceValue;
        pieceScore -= BitboardOperations.GetPopCount(currentBoard.BlackKnights) * _scoreValues.KnightPieceValue;

        var blackBishopCount = BitboardOperations.GetPopCount(currentBoard.BlackBishops);
        pieceScore -= blackBishopCount * _scoreValues.BishopPieceValue;

        if (blackBishopCount == 2)
        {
            pieceScore -= _scoreValues.DoubleBishopScore;
        }

        pieceScore -= BitboardOperations.GetPopCount(currentBoard.BlackRooks) * _scoreValues.RookPieceValue;

        var blackQueenCount = BitboardOperations.GetPopCount(currentBoard.BlackQueen);

        pieceScore -= blackQueenCount * _scoreValues.QueenPieceValue;
        pieceScore -= blackQueenCount * _scoreValues.SoloQueenScore;


        var popCount = BitboardOperations.GetPopCount(currentBoard.BlackKing);
        pieceScore -= popCount * kingScore;

        return pieceScore;
    }
}
