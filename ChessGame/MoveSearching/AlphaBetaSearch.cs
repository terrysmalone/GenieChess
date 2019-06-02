using System;
using System.Collections.Generic;
using ChessGame.BoardRepresentation;
using ChessGame.NotationHelpers;
using ChessGame.PossibleMoves;
using ChessGame.ScoreCalculation;
using log4net;

namespace ChessGame.MoveSearching
{
    public sealed class AlphaBetaSearch
    {
        private static readonly ILog m_Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IBoard m_BoardPosition;
        private readonly IScoreCalculator m_ScoreCalculator;
        
        public AlphaBetaSearch(IBoard boardPosition, IScoreCalculator scoreCalculator)
        {
            m_BoardPosition = boardPosition ?? throw new ArgumentNullException(nameof(boardPosition));

            m_ScoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        public PieceMoves CalculateBestMove(int depth)
        {
            var alpha = decimal.MinValue / 2 - 1;
            var beta = decimal.MaxValue / 2 + 1;

            var bestScore = alpha;
            var bestMove = new PieceMoves();

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));

#if Debug
            m_Log.Info("======================================================");
            m_Log.Info($"Moves to Check: {moveList.Count}");
#endif

            for (var i = 0; i < moveList.Count; i++)
            {
#if UCI
                Console.WriteLine($"info currmove {UCIMoveTranslator.ToUCIMove(moveList[i])} currmovenumber {i + 1}");
#endif

                m_BoardPosition.MakeMove(moveList[i], false);

                var score = AlphaBeta(-beta, -alpha, depth - 1);

                if (score > bestScore)
                {
                    bestMove = moveList[i];
                    bestScore = score;
                }

#if Debug
                m_Log.Info($"Move: {UCIMoveTranslator.ToUCIMove(moveList[i])}  - Score: {score}");
#endif

                m_BoardPosition.UnMakeLastMove();
            }

            return bestMove;
        }

        private decimal AlphaBeta(decimal alpha, decimal beta, int depthLeft)
        {
            var bestScore = decimal.MinValue / 2 + 1;

            if (depthLeft == 0)
            {
                //TODO: Perform quiescience search

                return Evaluate(m_BoardPosition);
            }

            var moveList = new List<PieceMoves>(MoveGeneration.CalculateAllMoves(m_BoardPosition));

            for (var i = 0; i < moveList.Count; i++)
            {
                m_BoardPosition.MakeMove(moveList[i], false);

                var score = -AlphaBeta(-beta, -alpha, depthLeft - 1);

                if (score >= beta)
                {
                    m_BoardPosition.UnMakeLastMove();
                    return score;
                }

                if (score > bestScore)
                {
                    bestScore = score;

                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }

            m_BoardPosition.UnMakeLastMove();

            return bestScore;
        }

        /// <summary>
        /// Evaluates the score relative to the current player
        /// i.e. A high score means the position is better for the current player 
        /// </summary>
        /// <param name="boardPosition"></param>
        /// <returns></returns>
        private decimal Evaluate(IBoard boardPosition)
        {
            if (m_BoardPosition.WhiteToMove)
            {
                //return m_ScoreCalculator.CalculateScore(boardPosition);
                return 20;
            }
            else
            {
                //return -m_ScoreCalculator.CalculateScore(boardPosition);
                return -20;
            }
        }
    }
}