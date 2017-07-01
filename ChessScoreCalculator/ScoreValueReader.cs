using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessScoreCalculator
{    
    internal class ScoreValueReader
    {
        private string scoreValuesFile;
        private List<string> scoreValues;

        private decimal pawnPieceValue;
        private decimal knightPieceValue;        
        private decimal bishopPieceValue;
        private decimal rookPieceValue;        
        private decimal queenPieceValue;
        
        #region properties

        internal decimal PawnPieceValue
        {
            get{ return pawnPieceValue; }
        }

        internal decimal KnightPieceValue
        {
            get { return knightPieceValue; }
        }

        internal decimal BishopPieceValue
        {
            get { return bishopPieceValue; }
        }

        internal decimal RookPieceValue
        {
            get { return rookPieceValue; }
        }

        internal decimal QueenPieceValue
        {
            get { return queenPieceValue; }
        }

        #endregion properties

        internal ScoreValueReader(string scoreValuesFile)
        {
            this.scoreValuesFile = scoreValuesFile;

            ReadScores();
        }

        private void ReadScores()
        {
            scoreValues = new List<string>();

            using (StreamReader reader = new StreamReader(scoreValuesFile))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    scoreValues.Add(line);
                }

                ReadPieceValues();
            }
        }

        private void ReadPieceValues()
        {
            pawnPieceValue = GetValue("pawnPieceValue");
            knightPieceValue = GetValue("knightPieceValue");
            bishopPieceValue = GetValue("bishopPieceValue");
            rookPieceValue = GetValue("rookPieceValue");
            queenPieceValue = GetValue("queenPieceValue");
        }

        private decimal GetValue(string valueString)
        {
            decimal score = 0;

            string match = scoreValues.FirstOrDefault(stringToCheck => stringToCheck.Contains(valueString));

            if (match != null)
            {
                string[] scoreParts = match.Split(null);
                score = decimal.Parse(scoreParts[2]);
            }
            else
                throw new ArgumentException(string.Format("Could not find the string: {0} in the scoreValues list.", valueString));

            return score;
        }
    }
}
