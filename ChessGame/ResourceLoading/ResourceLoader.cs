using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.ScoreCalculation;
using ChessGame.NotationHelpers;
using ChessGame.Books;

namespace ChessGame.ResourceLoading
{
    /// <summary>
    /// Loads resource files 
    /// </summary>
    public static class ResourceLoader
    {
        public static List<Opening> LoadOpeningBook(string bookName)
        {
            List<Opening> openings = new List<Opening>();

            string bookFile = GetEngineResourcePath() + bookName;

            string[] lines = File.ReadAllLines(bookFile);

            foreach (string line in lines)
            {
                string[] moves = SplitIntoChunks(line, 4);

                Opening opening = new Opening();
                opening.moves = moves;

                openings.Add(opening);
            }

            return openings;
        }

        static string[] SplitIntoChunks(string str, int chunkSize)
        {
            int moveCount = Convert.ToInt32(Math.Ceiling((double)str.Length/(double)chunkSize));

            string[] parts = new string[moveCount];

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = str.Substring(i*chunkSize, chunkSize);
            }

            //Last one may be smaller so get it here
            //int lastSize = str.Length % chunkSize;
            //parts[parts.Length - 1] = str.Substring(start, lastSize);

            return parts;
        }

        public static PerfTPosition LoadPerfTPosition(string perfTName)
        {
            List<PerfTPosition> positions = LoadPerfTPositions();

            PerfTPosition position = positions.Find(p => p.Name.ToLowerInvariant().Equals(perfTName.ToLowerInvariant()));

            return position;
        }

        public static List<PerfTPosition> LoadPerfTPositions()
        {
            List<PerfTPosition> position = new List<PerfTPosition>();

            string perfTFile = GetEvaluationResourcePath() + "PerfTPositions.txt";

            string[] lines = File.ReadAllLines(perfTFile);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                PerfTPosition perfTPos = new PerfTPosition();
                perfTPos.Name = parts[0];
                perfTPos.FenPosition = parts[1];

                List<ulong> results = new List<ulong>();

                for (int i = 2; i < parts.Length; i++)
                {
                    results.Add(Convert.ToUInt64(parts[i]));
                }

                perfTPos.Results = results;
                
                position.Add(perfTPos); 
            }

            return position;
        }

        public static List<TestPosition> LoadKaufmanTestPositions()
        {
            string perfTFile = GetEvaluationResourcePath() + "KaufmanTestPositions.txt";

            return LoadTestPositions(perfTFile);
        }

        public static List<TestPosition> LoadBratkoKopecPositions()
        {
            string perfTFile = GetEvaluationResourcePath() + "BratkoKopecPositions.txt";

            return LoadTestPositions(perfTFile);
        }

        private static List<TestPosition> LoadTestPositions(string testFilePath)
        {
            List<TestPosition> position = new List<TestPosition>();
            
            string[] lines = File.ReadAllLines(testFilePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                TestPosition testPos = new TestPosition();

                testPos.FenPosition = parts[0];
                testPos.bestMoveFEN = parts[1];
                testPos.Name = parts[2];

                position.Add(testPos);
            }

            return position;
        }

        #region resource paths

        private static string GetEvaluationResourcePath()
        {
            string current = Environment.CurrentDirectory;
            string parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\..\"));
            string evaluationResources = parent + @"EngineEvaluation\Resources\";

            return evaluationResources;
        }

        private static string GetTestResourcePath()
        {
            string current = Environment.CurrentDirectory;
            string parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\..\"));
            string evaluationResources = parent + @"ChessBoardTests\Resources\";

            return evaluationResources;
        }

        private static string GetEngineResourcePath()
        {
            string current = Environment.CurrentDirectory;
            string parent = Path.GetFullPath(Path.Combine(current, @"..\..\..\..\"));
            string evaluationResources = parent + @"ChessGame\Resources\";

            return evaluationResources;
        }

        #endregion resource paths

        public static ScoreCalculator LoadScoreValues(string fileName)
        {
            string scoreFile = GetEngineResourcePath() + fileName;
            ScoreCalculator scoreCalc = new ScoreCalculator(scoreFile);

            return scoreCalc;
        }

        internal static string LoadScoreValuesPath(string fileName)
        {
            return GetEngineResourcePath() + fileName;
        }
    }
}
