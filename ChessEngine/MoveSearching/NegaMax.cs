using System.Collections.Generic;
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
        private readonly Board _boardPosition;
        private readonly IScoreCalculator _scoreCalc;

        private PieceMover _pieceMover;

        private decimal m_Score;

        public NegaMax(Board boardPosition, IScoreCalculator scoreCalc)
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
                            m_Score = -Calculate(depth - 1, false);
                        else
                            m_Score = -Calculate(depth - 1, true);

                        if (m_Score > max)
                        {
                            max = m_Score;
                            bestMove = moveList[i];
                        }
                    }

                    _pieceMover.UnMakeLastMove();
                }
            }

            return bestMove;
        }

        private decimal Calculate(int depth, bool maximisingPlayer)
        {
            if (depth == 0)
                return Evaluate(_boardPosition, maximisingPlayer);

            var max = decimal.MinValue;

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllPseudoLegalMoves(_boardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                _pieceMover.MakeMove(moveList[i], false);

                if (MoveGeneration.ValidateMove(_boardPosition))
                {
                    m_Score = -Calculate(depth - 1, !maximisingPlayer);

                    if (m_Score > max)
                        max = m_Score;
                }

                _pieceMover.UnMakeLastMove();
            }

            return max;
        }

        private decimal Evaluate(Board boardPosition, bool maximisingPlayer)
        {
            if (maximisingPlayer)
                return _scoreCalc.CalculateScore(boardPosition);
            else
                return -_scoreCalc.CalculateScore(boardPosition);
        }
    }
}
