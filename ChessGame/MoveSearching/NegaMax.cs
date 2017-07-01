using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation;
using ChessGame.BoardSearching;
using ChessGame.Enums;
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
        private Board boardPosition;
        private ScoreCalculator scoreCalc;

        private decimal score;

        public NegaMax(Board boardPosition, ScoreCalculator scoreCalc)
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
                            score = -Calculate(depth - 1, false);
                        else
                            score = -Calculate(depth - 1, true);

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

        private decimal Calculate(int depth, bool maximisingPlayer)
        {
            if (depth == 0)
                return Evaluate(boardPosition, maximisingPlayer);

            decimal max = decimal.MinValue;

            ;
            List<PieceMoves> moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(boardPosition));

            for (int i = 0; i < moveList.Count; i++)
            {
                boardPosition.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(boardPosition))
                {
                    score = -Calculate(depth - 1, !maximisingPlayer);

                    if (score > max)
                        max = score;
                }

                boardPosition.UnMakeLastMove();
            }

            return max;
        }

        private decimal Evaluate(Board boardPosition, bool maximisingPlayer)
        {
            if (maximisingPlayer)
                return scoreCalc.CalculateScore(boardPosition);
            else
                return -scoreCalc.CalculateScore(boardPosition);
        }
    }
}
