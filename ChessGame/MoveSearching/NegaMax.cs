using System.Collections.Generic;
using System.Diagnostics;
using ChessGame.BoardRepresentation;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;

namespace ChessGame.MoveSearching
{
    /// <summary>
    /// Similar to:
    /// http://www.chessbin.com/post/Move-Searching-and-Alpha-Beta
    /// </summary>
    public class NegaMax
    {
        private readonly IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalc;

        private decimal m_Score;

        public NegaMax(IBoard boardPosition, IScoreCalculator scoreCalc)
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

        private decimal Evaluate(IBoard boardPosition, bool maximisingPlayer)
        {
            if (maximisingPlayer)
                return m_ScoreCalc.CalculateScore(boardPosition);
            else
                return -m_ScoreCalc.CalculateScore(boardPosition);
        }
    }
}
