using System;
using ChessGame.BoardRepresentation;
using ChessGame.ScoreCalculation;
using log4net;

namespace ChessGame.MoveSearching
{
    public sealed class AlphaBetaSearch
    {
        private readonly IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
        private readonly ILog m_Log;

        public AlphaBetaSearch(IBoard boardPosition, IScoreCalculator scoreCalculator, ILog log)
        {
            m_BoardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));

            m_Log = log;
        }
    }
}