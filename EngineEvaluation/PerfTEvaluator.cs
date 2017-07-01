using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessBoardTests;
using System.Diagnostics;
using ChessGame.BoardRepresentation;
using ChessGame.ResourceLoading;

namespace EngineEvaluation
{
    /// <summary>
    /// Benchmarks the time it takes to carry out PerfT evaluations
    /// </summary>
    public class PerfTEvaluator
    {
        string logLocation = Environment.CurrentDirectory;
        string logFile;
        const int REPEAT_COUNT_DEFAULT = 1;
        List<PerfTPosition> perfTPositions;

        public string LogFile
        {
            get { return logFile; }
        }

        #region constructor

        public PerfTEvaluator()
        {
            CreateLogFile();

            LogLine("PerfTEvaluator");
            LogLine("");
            LogLine(string.Format("Logging started at {0}", DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss")));
        }

        #endregion constructor

        public void EvaluatePerft(List<PerfTPosition> perftPositions)
        {
            foreach (PerfTPosition perfTPos in perftPositions)
            {
                LogPerftScore(perfTPos);
            }
        }

        private void LogPerftScore(PerfTPosition perfTPos)
        {
            LogLine("");
            LogLine(perfTPos.Name);
            LogLine(perfTPos.FenPosition);

            PerfT perfT = new PerfT();
            //perfT.GetAllMoves(
        }

        internal void EvaluatePerft(int startDepth, int endDepth, bool useHashing)
        {
            EvaluatePerft(startDepth, endDepth, REPEAT_COUNT_DEFAULT, useHashing);
        }

        internal void EvaluatePerft(int startDepth, int endDepth, int repeatCount, bool useHashing)
        {
            perfTPositions = ResourceLoader.LoadPerfTPositions();

            string runs = "runs";

            if (repeatCount < 1)
                repeatCount = 1;

            if (repeatCount == 1)
                runs = "run";


            LogLine(string.Format("startDepth {0}", startDepth));
            LogLine(string.Format("endDepth {0}", endDepth));
            LogLine(string.Format("repeatCount {0}", repeatCount));
            LogLine(string.Format("useHashing {0}", useHashing));
            
            LogLine("");
            LogLine(string.Format("All values taken as an average of {0} {1}", repeatCount, runs));
            
            foreach (PerfTPosition perfTPosition in perfTPositions)
            {
                if (perfTPosition.FenPosition == "4k3/1P6/8/8/8/8/K7/8 w - - 0 1")
                {

                    LogLine("--------------------------------------------");
                    LogLine(perfTPosition.Name);
                    LogLine(perfTPosition.FenPosition);
                    LogLine("");

                    if (startDepth < 1)
                        startDepth = 1;

                    for (int i = startDepth; i <= endDepth; i++)
                    {
                        LogLine(string.Format("Depth:{0}", i));

                        if (perfTPosition.Results.Count >= i)
                        {
                            TimeSpan time = TimePerfT(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1], repeatCount, useHashing);

                            LogLine(string.Format("Time:{0}", time.ToString()));

                        }
                        else
                        {
                            LogLine(string.Format("N/A"));
                        }
                    }
                }
            }
        }

        private TimeSpan TimePerfT(string startingPosition, int depth, ulong expectedResult, int repeatCount, bool useHashing)
        {
            Board board = new Board();
            board.SetFENPosition(startingPosition);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < repeatCount; i++)
            {
                PerfT perft = new PerfT();
                perft.UseHashing = useHashing;

                ulong result = perft.Perft(board, depth);

                if (result != expectedResult)
                    LogLine("PERFT FAILED");
            }

            timer.Stop();
            
            //TimeSpan averageSpeed = new TimeSpan(timer.ElapsedMilliseconds / repeatCount);
            TimeSpan averageSpeed = new TimeSpan(timer.Elapsed.Ticks / repeatCount);

            return averageSpeed;
        }


        #region logging

        private void CreateLogFile()
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\PerfTLogging.txt";
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
