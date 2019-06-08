using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.NotationHelpers;
using ChessEngineTests;
using ResourceLoading;

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

        private protected List<PerfTPosition> perfTPositions;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

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

            LogLine($"Logging started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");
        }

        #endregion constructor

        public void EvaluatePerft(List<PerfTPosition> perftPositions)
        {
            foreach (var perfTPos in perftPositions)
            {
                LogPerftScore(perfTPos);
            }
        }

        private void LogPerftScore(PerfTPosition perfTPos)
        {
            LogLine("");
            LogLine(perfTPos.Name);
            LogLine(perfTPos.FenPosition);

            var perfT = new PerfT();
        }

        internal void EvaluatePerft(int startDepth, int endDepth, bool useHashing)
        {
            EvaluatePerft(startDepth, endDepth, REPEAT_COUNT_DEFAULT, useHashing);
        }

        internal void EvaluatePerft(int startDepth, int endDepth, int repeatCount, bool useHashing)
        {
            // Read the file as one string. 

            perfTPositions = m_ResourceLoader.LoadPerfTPositions();

            var runs = "runs";

            if (repeatCount < 1)
                repeatCount = 1;

            if (repeatCount == 1)
                runs = "run";


            LogLine($"startDepth {startDepth}");
            LogLine($"endDepth {endDepth}");
            LogLine($"repeatCount {repeatCount}");
            LogLine($"useHashing {useHashing}");
            
            LogLine("");
            LogLine($"All values taken as an average of {repeatCount} {runs}");
            
            foreach (var perfTPosition in perfTPositions)
            {
                //if (perfTPosition.FenPosition == "4k3/1P6/8/8/8/8/K7/8 w - - 0 1")
                //{

                    LogLine("--------------------------------------------");
                    LogLine(perfTPosition.Name);
                    LogLine(perfTPosition.FenPosition);
                    LogLine("");

                    if (startDepth < 1)
                        startDepth = 1;

                    for (var i = startDepth; i <= endDepth; i++)
                    {
                        LogLine($"Depth:{i}");

                        if (perfTPosition.Results.Count >= i)
                        {
                            var time = TimePerfT(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1], repeatCount, useHashing);

                            LogLine($"Time:{time.ToString()}");

                            LogLine($"Total nodes:{perfTPosition.Results[i-1]} - VisitedNodes:{CountDebugger.Nodes.ToString()}");
                            CountDebugger.ClearAll();
                        }
                        else
                        {
                            LogLine("N/A");
                        }
                    }
               // }
            }
        }

        private TimeSpan TimePerfT(string startingPosition, int depth, ulong expectedResult, int repeatCount, bool useHashing)
        {
            var board = new Board();
            board.SetPosition(FenTranslator.ToBoardState(startingPosition));

            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < repeatCount; i++)
            {
                var perft = new PerfT
                {
                    UseHashing = useHashing
                };

                var result = perft.Perft(board, depth);

                if (result != expectedResult)
                    LogLine("PERFT FAILED");
            }

            timer.Stop();
            
            //TimeSpan averageSpeed = new TimeSpan(timer.ElapsedMilliseconds / repeatCount);
            var averageSpeed = new TimeSpan(timer.Elapsed.Ticks / repeatCount);

            return averageSpeed;
        }


        #region logging

        private void CreateLogFile()
        {
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logLocation += @"\" + timeStamp;

            Directory.CreateDirectory(logLocation);

            logFile = logLocation + @"\PerfTLogging.txt";
            //File.Create(logFile);
        }

        private void LogLine(string text)
        {
            using (var stream = System.IO.File.AppendText(logFile))
            {
                stream.WriteLine(text);
            }
        }

        private void Log(string text)
        {
            using (var stream = System.IO.File.AppendText(logFile))
            {
                stream.Write(text);
            }
        }

        #endregion logging

       
    }
}
