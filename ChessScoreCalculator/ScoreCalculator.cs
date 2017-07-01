using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBoard;

namespace ChessScoreCalculator
{
    /// <summary>
    /// Calculates the score of a particular board position
    /// </summary>
    public class ScoreCalculator
    {
        private BitboardSearches bitboardSearches = new BitboardSearches();

        private Board currentBoard;

        private bool isEndGame = false;
                
        #region score values

        #region sub scores (mostly used for debugging)

        decimal pieceScore;

        #endregion sub scores
        
        #region Piece values

        private decimal pawnValue;
        private decimal knightValue;
        private decimal bishopValue;
        private decimal rookValue;
        private decimal queenValue;
        
        #endregion Piece values
        
        #endregion score values

        #region Properties

        #region Sub scores (mostly used for debugging)

        /// <summary>
        /// The calculated material value score
        /// </summary>
        public decimal PieceScore
        {
            get { return pieceScore; }
        }

        #endregion Sub scores

        #region Piece values

        public decimal PawnValue
        {
            get { return pawnValue; }
        }

        public decimal KnightValue
        {
            get { return knightValue; }
        }

        public decimal BishopValue
        {
            get { return bishopValue; }
        }

        public decimal RookValue
        {
            get { return rookValue; }
        }

        public decimal QueenValue
        {
            get { return queenValue; }
        }

        #endregion Piece values

        #endregion Properties



        #region Initialisation methods

        public ScoreCalculator(string scoreValuesFile)
        {            
            ReadScoreValues(scoreValuesFile);
        }

        private void ReadScoreValues(string scoreValuesFile)
        {
            ScoreValueReader valueReader = new ScoreValueReader(scoreValuesFile);

            pawnValue = valueReader.PawnPieceValue;
            knightValue = valueReader.KnightPieceValue;
            bishopValue = valueReader.BishopPieceValue;
            rookValue = valueReader.RookPieceValue;
            queenValue = valueReader.QueenPieceValue;
        }

        #endregion

        public decimal CalculateScore(Board currentBoard)
        {
            this.currentBoard = currentBoard;

            decimal score = 0;

            score += CalculatePieceValues();

            return score;
        }

        private decimal CalculatePieceValues()
        {
            pieceScore = 0;

            //Calculate white piece values
            pieceScore += bitboardSearches.GetPopCount(currentBoard.WhitePawns) * pawnValue;
            pieceScore += bitboardSearches.GetPopCount(currentBoard.WhiteKnights) * knightValue;
            pieceScore += bitboardSearches.GetPopCount(currentBoard.WhiteBishops) * bishopValue;
            pieceScore += bitboardSearches.GetPopCount(currentBoard.WhiteRooks) * rookValue;
            pieceScore += bitboardSearches.GetPopCount(currentBoard.WhiteQueen) * queenValue;

            //Calculate black piece values
            pieceScore -= bitboardSearches.GetPopCount(currentBoard.BlackPawns) * pawnValue;
            pieceScore -= bitboardSearches.GetPopCount(currentBoard.BlackKnights) * knightValue;
            pieceScore -= bitboardSearches.GetPopCount(currentBoard.BlackBishops) * bishopValue;
            pieceScore -= bitboardSearches.GetPopCount(currentBoard.BlackRooks) * rookValue;
            pieceScore -= bitboardSearches.GetPopCount(currentBoard.BlackQueen) * queenValue;

            return pieceScore;
        }
    }
}
