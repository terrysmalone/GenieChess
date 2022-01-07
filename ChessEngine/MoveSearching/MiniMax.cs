using System.Collections.Generic;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.BoardSearching;
using ChessEngine.PossibleMoves;
using ChessEngine.ScoreCalculation;

namespace ChessEngine.MoveSearching
{
    public class MiniMax
    {
        private Board _boardPosition;
        private readonly IScoreCalculator _scoreCalc;

        private PieceMover _pieceMover;

        private decimal score;

        public MiniMax(Board boardPosition, IScoreCalculator scoreCalc)
        {
            _boardPosition = boardPosition;
            _scoreCalc = scoreCalc;

            _pieceMover = new PieceMover(_boardPosition);
        }

        public PieceMoves MoveCalculate(int depth)
        {
            Debug.Assert(depth >= 1);

            var bestMove = new PieceMoves();

            var max = decimal.MinValue;


            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));


            var isWhite = _boardPosition.WhiteToMove;

            for (var i = 0; i < moveList.Count; i++)
            {
                var skipMove = false;

                if (moveList[i].SpecialMove == SpecialMoveType.KingCastle || moveList[i].SpecialMove == SpecialMoveType.QueenCastle)
                {
                    if (BoardChecking.IsKingInCheck(_boardPosition, _boardPosition.WhiteToMove) 
                        || !MoveGeneration.ValidateCastlingMove(_boardPosition, moveList[i]))
                    {
                        skipMove = true;
                    }
                }

                if (!skipMove)
                {
                    _pieceMover.MakeMove(moveList[i], false);

                    if (MoveGeneration.ValidateMove(_boardPosition))
                    {
                        if (isWhite)
                            score = Min(depth - 1);
                        else
                            score = -Max(depth - 1);

                        if (score > max)
                        {
                            max = score;
                            bestMove = moveList[i];
                        }
                    }

                    _pieceMover.UnMakeLastMove();
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
                return Evaluate(_boardPosition);

            var max = decimal.MinValue;

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                _pieceMover.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(_boardPosition))
                {
                    score = Min(depth - 1);

                    if (score > max)
                        max = score;
                }

                _pieceMover.UnMakeLastMove();
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
                return Evaluate(_boardPosition);

            var min = decimal.MaxValue;
                        
            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                _pieceMover.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(_boardPosition))
                {
                    score = Max(depth - 1);

                    if (score < min)
                        min = score;
                }

                _pieceMover.UnMakeLastMove();
            }

            return min;
        }

        private decimal Evaluate(Board boardPosition)
        {
            return _scoreCalc.CalculateScore(boardPosition);
        }

    }
}
