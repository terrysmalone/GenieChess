﻿using System.Collections.Generic;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardRepresentation.Enums;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;

namespace ChessEngine.MoveSearching
{
    public class MiniMax
    {
        private IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalc;

        private decimal score;

        public MiniMax(IBoard boardPosition, IScoreCalculator scoreCalc)
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

            PieceColour colour;

            if (m_BoardPosition.WhiteToMove)
                colour = PieceColour.White;
            else
                colour = PieceColour.Black;

            for (var i = 0; i < moveList.Count; i++)
            {
                var skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    var friendlyColour = PieceColour.White;

                    if(m_BoardPosition.WhiteToMove == false)
                        friendlyColour = PieceColour.Black;

                    if (BoardChecking.IsKingInCheckFast(m_BoardPosition, friendlyColour) || !MoveGeneration.ValidateCastlingMove(m_BoardPosition, moveList[i]))
                    {
                        skipMove = true;
                    }
                }

                if (!skipMove)
                {
                    m_BoardPosition.MakeMove(moveList[i], false);

                    if (MoveGeneration.ValidateMove(m_BoardPosition))
                    {
                        if (colour == PieceColour.White)
                            score = Min(depth - 1);
                        else
                            score = -Max(depth - 1);

                        if (score > max)
                        {
                            max = score;
                            bestMove = moveList[i];
                        }
                    }

                    m_BoardPosition.UnMakeLastMove();
                }
            }

            return bestMove;
        }

        /// <summary>
        /// From whites perspective - returns largest score
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private decimal Max(int depth)
        {
            if (depth == 0)
                return Evaluate(m_BoardPosition);

            var max = decimal.MinValue;

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                m_BoardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(m_BoardPosition))
                {
                    score = Min(depth - 1);

                    if (score > max)
                        max = score;
                }

                m_BoardPosition.UnMakeLastMove();
            }

            return max;
        }

        /// <summary>
        /// From blacks perspective - returns lowest score
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private decimal Min(int depth)
        {
            if (depth == 0)
                return Evaluate(m_BoardPosition);

            var min = decimal.MaxValue;
                        
            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(m_BoardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                m_BoardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(m_BoardPosition))
                {
                    score = Max(depth - 1);

                    if (score < min)
                        min = score;
                }

                m_BoardPosition.UnMakeLastMove();
            }

            return min;
        }

        private decimal Evaluate(IBoard boardPosition)
        {
            return m_ScoreCalc.CalculateScore(boardPosition);
        }

    }
}