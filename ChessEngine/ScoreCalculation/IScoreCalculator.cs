﻿using ChessEngine.BoardRepresentation;

namespace ChessEngine.ScoreCalculation
{
    public interface IScoreCalculator
    {
        decimal CalculateScore(IBoard currentBoard);
    }
}