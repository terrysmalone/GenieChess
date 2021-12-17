﻿using System.Collections.Generic;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;

namespace ChessEngine.MoveSearching
{
    /// <summary>
    /// Similar to:
    /// http://www.chessbin.com/post/Move-Searching-and-Alpha-Beta
    /// </summary>
    public class NegaMax
    {
        private readonly Board m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalc;

        private decimal m_Score;

        public NegaMax(Board boardPosition, IScoreCalculator scoreCalc)
        {
            m_BoardPosition = boardPosition;
            m_ScoreCalc = scoreCalc;
        }

        public PieceMoves MoveCalculate(int depth)
        {
            Debug.Assert(depth >= 1);

            var bestMove = new PieceMoves();

            var max = decimal.MinValue;

            
            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));


            var isWhite = m_BoardPosition.WhiteToMove;


            for (var i = 0; i < moveList.Count; i++)
            {
                var skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (BoardChecking.IsKingInCheck(m_BoardPosition, m_BoardPosition.WhiteToMove) 
                        || !MoveGeneration.ValidateCastlingMove(m_BoardPosition, moveList[i]))
                    {
                        skipMove = true;
                    }
                }

                if (!skipMove)
                {
                    m_BoardPosition.MakeMove(moveList[i], false);

                    if (MoveGeneration.ValidateMove(m_BoardPosition))
                    {
                        if (isWhite)
                            m_Score = -Calculate(depth - 1, false);
                        else
                            m_Score = -Calculate(depth - 1, true);

                        if (m_Score > max)
                        {
                            max = m_Score;
                            bestMove = moveList[i];
                        }
                    }

                    m_BoardPosition.UnMakeLastMove();
                }
            }

            return bestMove;
        }

        private decimal Calculate(int depth, bool maximisingPlayer)
        {
            if (depth == 0)
                return Evaluate(m_BoardPosition, maximisingPlayer);

            var max = decimal.MinValue;

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                m_BoardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(m_BoardPosition))
                {
                    m_Score = -Calculate(depth - 1, !maximisingPlayer);

                    if (m_Score > max)
                        max = m_Score;
                }

                m_BoardPosition.UnMakeLastMove();
            }

            return max;
        }

        private decimal Evaluate(Board boardPosition, bool maximisingPlayer)
        {
            if (maximisingPlayer)
                return m_ScoreCalc.CalculateScore(boardPosition);
            else
                return -m_ScoreCalc.CalculateScore(boardPosition);
        }
    }
}
