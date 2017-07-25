using ChessGame.BoardRepresentation;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.BoardSearching;

namespace ChessGame.MoveSearching
{
    public class MiniMax
    {
        private Board boardPosition;
        private ScoreCalculator scoreCalc;

        private decimal score;

        public MiniMax(Board boardPosition, ScoreCalculator scoreCalc)
        {
            this.boardPosition = boardPosition;
            this.scoreCalc = scoreCalc;
        }

        public PieceMoves MoveCalculate(int depth)
        {
            Debug.Assert(depth >= 1);

            PieceMoves bestMove = new PieceMoves();

            decimal max = decimal.MinValue;


            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            PieceColour colour;

            if (boardPosition.WhiteToMove)
                colour = PieceColour.White;
            else
                colour = PieceColour.Black;

            for (int i = 0; i < moveList.Count; i++)
            {
                bool skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    PieceColour friendlyColour = PieceColour.White;

                    if(boardPosition.WhiteToMove == false)
                        friendlyColour = PieceColour.Black;

                    if (BoardChecking.IsKingInCheckFast(boardPosition, friendlyColour) || !MoveGeneration.ValidateCastlingMove(boardPosition, moveList[i]))
                    {
                        skipMove = true;
                    }
                }

                if (!skipMove)
                {
                    boardPosition.MakeMove(moveList[i], false);

                    if (MoveGeneration.ValidateMove(boardPosition))
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

                    boardPosition.UnMakeLastMove();
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
                return Evaluate(boardPosition);

            decimal max = decimal.MinValue;

            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            for (int i = 0; i < moveList.Count; i++)
            {
                boardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(boardPosition))
                {
                    score = Min(depth - 1);

                    if (score > max)
                        max = score;
                }

                boardPosition.UnMakeLastMove();
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
                return Evaluate(boardPosition);

            decimal min = decimal.MaxValue;
                        
            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            for (int i = 0; i < moveList.Count; i++)
            {
                boardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(boardPosition))
                {
                    score = Max(depth - 1);

                    if (score < min)
                        min = score;
                }

                boardPosition.UnMakeLastMove();
            }

            return min;
        }

        private decimal Evaluate(Board boardPosition)
        {
            return scoreCalc.CalculateScore(boardPosition);
        }

    }
}
