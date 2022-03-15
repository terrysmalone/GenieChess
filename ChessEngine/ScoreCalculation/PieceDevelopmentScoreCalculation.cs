using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;

namespace ChessEngine.ScoreCalculation;

internal sealed class PieceDevelopmentScoreCalculation : IScoreCalculation
{
    private readonly ScoreValues _scoreValues;

    public PieceDevelopmentScoreCalculation(ScoreValues scoreValues)
    {
        _scoreValues = scoreValues;
    }
    
    public int Calculate(Board currentBoard)
    {
        var developedPieceScore = 0;

        developedPieceScore += CalculateDevelopedPieceScore(currentBoard);
        developedPieceScore += CalculateConnectedRookScore(currentBoard);

        developedPieceScore += CalculateEarlyQueenScore(currentBoard);

        return developedPieceScore;
    }
    
    // Points for pieces not on the back rank (bishops & knights)
    private int CalculateDevelopedPieceScore(Board currentBoard)
    {
        var developedPiecesScore = 0;

        ulong whiteBack = 255;
        var blackBack = 18374686479671623680;

        var whiteDevelopedPieces = (currentBoard.WhiteBishops ^ currentBoard.WhiteKnights ^ currentBoard.WhiteQueen) & ~whiteBack;
        var developedWhitePieceCount = BitboardOperations.GetPopCount(whiteDevelopedPieces);
        developedPiecesScore += developedWhitePieceCount * _scoreValues.DevelopedPieceScore;

        var blackDevelopedPieces = (currentBoard.BlackBishops ^ currentBoard.BlackKnights ^ currentBoard.BlackQueen) & ~blackBack;
        var developedBlackPieceCount = BitboardOperations.GetPopCount(blackDevelopedPieces);
        developedPiecesScore -= developedBlackPieceCount * _scoreValues.DevelopedPieceScore;

        return developedPiecesScore;
    }

    private int CalculateConnectedRookScore(Board currentBoard)
    {
        var connectedRookScore = 0;

        if (BitboardOperations.GetPopCount(currentBoard.WhiteRooks) > 1)
        {
            var rooks = BitboardOperations.SplitBoardToArray(currentBoard.WhiteRooks);

            var firstRook = rooks[0];
            var secondRook = rooks[1];

            if ((BoardChecking.FindRightBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindLeftBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindUpBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
            else if ((BoardChecking.FindDownBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
            {
                connectedRookScore += _scoreValues.ConnectedRookScore;
            }
        }

        if (BitboardOperations.GetPopCount(currentBoard.BlackRooks) > 1)
        {
            var rooks = BitboardOperations.SplitBoardToArray(currentBoard.BlackRooks);

            var firstRook = rooks[0];
            var secondRook = rooks[1];

            if ((BoardChecking.FindRightBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindLeftBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindUpBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
            else if ((BoardChecking.FindDownBlockingPosition(currentBoard, firstRook) & secondRook) > 0)
                connectedRookScore -= _scoreValues.ConnectedRookScore;
        }

        return connectedRookScore;
    }

    private int CalculateEarlyQueenScore(Board currentBoard)
    {
        var earlyQueenScoreScore = 0;

        ulong whiteUndevelopedPieceBoard = 0;

        whiteUndevelopedPieceBoard |= currentBoard.WhiteBishops & 36;  //Any white bishops on C1 or F1
        whiteUndevelopedPieceBoard |= currentBoard.WhiteKnights & 66;  //Any white knights on B1 orG1

        var whiteUndevelopedPieceCount = BitboardOperations.GetPopCount(whiteUndevelopedPieceBoard);

        // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
        if (whiteUndevelopedPieceCount >= 2 && (currentBoard.WhiteQueen & ~8UL) != 0)
        {
            earlyQueenScoreScore -= _scoreValues.EarlyQueenMoveScore;
        }

        ulong blackUndevelopedPieceBoard = 0;

        //Any black bishops on C8 or F8
        blackUndevelopedPieceBoard |= currentBoard.BlackBishops & 2594073385365405696;
        //Any black knights on B8 or G8
        blackUndevelopedPieceBoard |= currentBoard.BlackKnights & 66;

        int blackUndevelopedPieceCount = BitboardOperations.GetPopCount(blackUndevelopedPieceBoard);

        // If we have at least 2 undeveloped pieces (bishops and knights) and the queen has moved
        if (blackUndevelopedPieceCount >= 2 && (currentBoard.BlackQueen & ~576460752303423488UL) != 0)
        {
            earlyQueenScoreScore += _scoreValues.EarlyQueenMoveScore;
        }

        return earlyQueenScoreScore;
    }
}
