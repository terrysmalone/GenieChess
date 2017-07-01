using ChessGame.ResourceLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.Debugging;
using ChessGame.BoardRepresentation;
using ChessGame.ScoreCalculation;
using System.Diagnostics;
using ChessGame.MoveSearching;
using ChessGame.PossibleMoves;
using ChessGame.NotationHelpers;
using ChessGame.Enums;

namespace EngineEvaluation
{
    /// <summary>
    /// Runs and logs a full performance evaluation 
    /// </summary>
    public class EnginePerformanceEvaluator
    {
        string logLocation = Environment.CurrentDirectory;
        string logFile;

        List<PerfTPosition> perfTPositions;
        List<TestPosition> bratkoKopecPositions;
        List<TestPosition> kaufmanPositions;


        #region properties

        public string LogFile
        {
            get { return logFile; }
        }

        #endregion properties

        #region constructor

        public EnginePerformanceEvaluator()
        {
            CreateLogFile();

            perfTPositions = ResourceLoader.LoadPerfTPositions();
            bratkoKopecPositions = ResourceLoader.LoadBratkoKopecPositions();
            kaufmanPositions = ResourceLoader.LoadKaufmanTestPositions();
            
            LogLine("Performance Evaluator");
            LogLine("");
            LogLine(string.Format("Logging started at {0}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")));
            LogLine("");
        }        

        #endregion constructor

        public void EvaluateTestPositions(int depth)
        {
            LogLine("Evaluating Test position performance");
            LogLine("");
             
            LogLine("Bratko Kopec Positions");
            LogLine("");
            EvaluatePositions(bratkoKopecPositions, depth);
             
            LogLine("Kaufman Positions");
            LogLine(""); 
            EvaluatePositions(kaufmanPositions, depth);

             //foreach (TestPosition testPos in testPositions)
             //{
             //    LogLine("--------------------------------------------");
             //    LogLine(testPos.Name);
             //    LogLine(testPos.FenPosition);

             //    for (int i = minDepth; i <= maxDepth; i++)
             //    {
             //        //LogLine("--------------------------------------------");
             //        //LogLine("");
             //        //LogLine(string.Format("Depth:{0}", i));
             //        //LogLine("");
             //        EvaluatePosition(testPos, i);

             //    }
             //}                
        }

        private void EvaluatePositions(List<TestPosition> testPositions, int depth)
        {
            int passed = 0;

            foreach (TestPosition testPos in testPositions)
            {
                LogLine("--------------------------------------------");
                LogLine(testPos.Name);
                LogLine(testPos.FenPosition);

                if (EvaluatePosition(testPos, depth))
                    passed++;
            }

            LogLine(string.Format("Passed:{0}/{1}", passed, testPositions.Count));
            
        }

        private bool EvaluatePosition(TestPosition testPos, int depth)
        {
            CountDebugger.ClearAll();

            Board board = new Board();
            board.SetFENPosition(testPos.FenPosition);

            ScoreCalculator scoreCalc = ResourceLoader.LoadScoreValues("ScoreValues.xml");

            Stopwatch timer = new Stopwatch();

            TranspositionTable.Restart();
            CountDebugger.ClearAll();

            timer.Start();

            AlphaBetaSearch alphaBeta = new AlphaBetaSearch(board, scoreCalc);
            PieceMoves currentMove = alphaBeta.StartSearch(depth);

            timer.Stop();

            TimeSpan speed = new TimeSpan(timer.Elapsed.Ticks);

            List<PVInfo> idInfo = alphaBeta.IdMoves;
            
            //for (int i = 0; i < depth; i++)
            //{
            //    string move = UCIMoveTranslator.ToUCIMove(idInfo[i].Move);

            //    LogLine(string.Format("Depth {0}", i + 1));
            //    LogLine(string.Format("Nodes evaluated:{0}, Accumulated time:{1}, Best move:{2}, Score:{3}", idInfo[i].NodesVisited, idInfo[i].AccumulatedTime, move, idInfo[i].Score));
            //}

            //LogLine("");
            //LogLine(string.Format("Total time: {0}", speed.ToString()));
            //LogLine("");

            //string bestMove = UCIMoveTranslator.ToUCIMove(idInfo[idInfo.Count-1].Move);
            string bestMove = PgnTranslator.ToPgnMove(board, idInfo[idInfo.Count - 1].Move.Position, idInfo[idInfo.Count - 1].Move.Moves, idInfo[idInfo.Count - 1].Move.Type);
            
            string expectedMove = testPos.bestMoveFEN;

            string pass = "FAIL";
            bool passed = false;

            if (bestMove.Equals(expectedMove))
            {
                pass = "PASS";
                passed = true;
            }

            LogLine(string.Format("Depth:{0} - Found move:{1}, Expected move:{2} - {3}", depth, bestMove, expectedMove, pass));
            return passed;        
        }

        #region logging

        private void CreateLogFile()
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\NodeCountLogging.txt";
            //File.Create(logFile);
        }

        private void LogLine(string text)
        {
            using (System.IO.StreamWriter stream = System.IO.File.AppendText(logFile))
            {
                stream.WriteLine(text);
            }
        }

        private void Log(string text)
        {
            using (System.IO.StreamWriter stream = System.IO.File.AppendText(logFile))
            {
                stream.Write(text);
            }
        }

        #endregion logging
    }    
}
