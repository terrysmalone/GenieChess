using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation;

internal sealed class CentralPieceScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    public CentralPieceScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }

    public int Calculate(Board currentBoard)
    {
        const ulong innerCentralSquares = 103481868288;
        const ulong outerCentralSquares = 66125924401152;

        var piecePositionScore = 0;

        //Pawns
        var whitePawnBoard = currentBoard.WhitePawns;

        piecePositionScore += CalculatePositionScores(whitePawnBoard, innerCentralSquares) * _scoreValues.InnerCentralPawnScore;
        piecePositionScore += CalculatePositionScores(whitePawnBoard, outerCentralSquares) * _scoreValues.OuterCentralPawnScore;

        var blackPawnBoard = currentBoard.BlackPawns;

        piecePositionScore -= CalculatePositionScores(blackPawnBoard, innerCentralSquares) * _scoreValues.InnerCentralPawnScore;
        piecePositionScore -= CalculatePositionScores(blackPawnBoard, outerCentralSquares) * _scoreValues.OuterCentralPawnScore;

        //Knights
        var whiteKnightBoard = currentBoard.WhiteKnights;

        piecePositionScore += CalculatePositionScores(whiteKnightBoard, innerCentralSquares) * _scoreValues.InnerCentralKnightScore;
        piecePositionScore += CalculatePositionScores(whiteKnightBoard, outerCentralSquares) * _scoreValues.OuterCentralKnightScore;

        var blackKnightBoard = currentBoard.BlackKnights;

        piecePositionScore -= CalculatePositionScores(blackKnightBoard, innerCentralSquares) * _scoreValues.InnerCentralKnightScore;
        piecePositionScore -= CalculatePositionScores(blackKnightBoard, outerCentralSquares) * _scoreValues.OuterCentralKnightScore;

        //Bishops
        var whiteBishopBoard = currentBoard.WhiteBishops;

        piecePositionScore += CalculatePositionScores(whiteBishopBoard, innerCentralSquares) * _scoreValues.InnerCentralBishopScore;
        piecePositionScore += CalculatePositionScores(whiteBishopBoard, outerCentralSquares) * _scoreValues.OuterCentralBishopScore;

        var blackBishopBoard = currentBoard.BlackBishops;

        piecePositionScore -= CalculatePositionScores(blackBishopBoard, innerCentralSquares) * _scoreValues.InnerCentralBishopScore;
        piecePositionScore -= CalculatePositionScores(blackBishopBoard, outerCentralSquares) * _scoreValues.OuterCentralBishopScore;

        //Rooks
        var whiteRookBoard = currentBoard.WhiteRooks;

        piecePositionScore += CalculatePositionScores(whiteRookBoard, innerCentralSquares) * _scoreValues.InnerCentralRookScore;
        piecePositionScore += CalculatePositionScores(whiteRookBoard, outerCentralSquares) * _scoreValues.OuterCentralRookScore;

        var blackRookBoard = currentBoard.BlackRooks;

        piecePositionScore -= CalculatePositionScores(blackRookBoard, innerCentralSquares) * _scoreValues.InnerCentralRookScore;
        piecePositionScore -= CalculatePositionScores(blackRookBoard, outerCentralSquares) * _scoreValues.OuterCentralRookScore;

        //Queens
        var whiteQueenBoard = currentBoard.WhiteQueen;

        piecePositionScore += CalculatePositionScores(whiteQueenBoard, innerCentralSquares) * _scoreValues.InnerCentralQueenScore;
        piecePositionScore += CalculatePositionScores(whiteQueenBoard, outerCentralSquares) * _scoreValues.OuterCentralQueenScore;

        var blackQueenBoard = currentBoard.BlackQueen;

        piecePositionScore -= CalculatePositionScores(blackQueenBoard, innerCentralSquares) * _scoreValues.InnerCentralQueenScore;
        piecePositionScore -= CalculatePositionScores(blackQueenBoard, outerCentralSquares) * _scoreValues.OuterCentralQueenScore;

        return piecePositionScore;
    }

    private static int CalculatePositionScores(ulong pieces, ulong positions)
    {
        var inPosition = pieces & positions;

        return inPosition > 0 ? BitboardOperations.GetPopCount(inPosition) : 0;
    }
}
