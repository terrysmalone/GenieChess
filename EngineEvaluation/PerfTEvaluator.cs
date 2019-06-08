using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.NotationHelpers;
using ChessEngineTests;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    // Benchmarks the time it takes to carry out PerfT evaluations
    public class PerfTEvaluator : IEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly string m_FullLogFile;
        private readonly string m_HighlightsLogFile;

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        public PerfTEvaluator(string highlightsLogFile, string fullLogFile)
        {
            if (highlightsLogFile == null)
            {
                Log.Error("No highlightsLogFile was passed to PerfTEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            m_HighlightsLogFile = highlightsLogFile;

            if (fullLogFile == null)
            {
                Log.Error("No fullLogFile was passed to PerfTEvaluator");
                throw new ArgumentNullException(nameof(highlightsLogFile));
            }

            m_FullLogFile = fullLogFile;

            LogLine("PerfTEvaluator");
        }

        public void Evaluate()
        {
            LogLineAsDetailed($"Evaluation started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");

            var perfTPositions = m_ResourceLoader.LoadPerfTPositions();

            foreach (var perfTPos in perfTPositions)
            {
                LogExpectedPerftScore(perfTPos);
            }
        }

        private void LogExpectedPerftScore(PerfTPosition perfTPos)
        {
            LogLineAsDetailed("");
            LogLineAsDetailed(perfTPos.Name);
            LogLineAsDetailed(perfTPos.FenPosition);
        }

        //internal void EvaluatePerft(int startDepth, int endDepth, bool useHashing)
        //{
        //    EvaluatePerft(startDepth, endDepth, REPEAT_COUNT_DEFAULT, useHashing);
        //}

        //internal void EvaluatePerft(int startDepth, int endDepth, int repeatCount, bool useHashing)
        //{
        //    // Read the file as one string. 

        //   // perfTPositions = m_ResourceLoader.LoadPerfTPositions();

        //    var runs = "runs";

        //    if (repeatCount < 1)
        //        repeatCount = 1;

        //    if (repeatCount == 1)
        //        runs = "run";


        //    LogLine($"startDepth {startDepth}");
        //    LogLine($"endDepth {endDepth}");
        //    LogLine($"repeatCount {repeatCount}");
        //    LogLine($"useHashing {useHashing}");
            
        //    LogLine("");
        //    LogLine($"All values taken as an average of {repeatCount} {runs}");
            
        //    foreach (var perfTPosition in perfTPositions)
        //    {
        //        //if (perfTPosition.FenPosition == "4k3/1P6/8/8/8/8/K7/8 w - - 0 1")
        //        //{

        //            LogLine("--------------------------------------------");
        //            LogLine(perfTPosition.Name);
        //            LogLine(perfTPosition.FenPosition);
        //            LogLine("");

        //            if (startDepth < 1)
        //                startDepth = 1;

        //            for (var i = startDepth; i <= endDepth; i++)
        //            {
        //                LogLine($"Depth:{i}");

        //                if (perfTPosition.Results.Count >= i)
        //                {
        //                    var time = TimePerfT(perfTPosition.FenPosition, i, perfTPosition.Results[i - 1], repeatCount, useHashing);

        //                    LogLine($"Time:{time.ToString()}");

        //                    LogLine($"Total nodes:{perfTPosition.Results[i-1]} - VisitedNodes:{CountDebugger.Nodes.ToString()}");
        //                    CountDebugger.ClearAll();
        //                }
        //                else
        //                {
        //                    LogLine("N/A");
        //                }
        //            }
        //       // }
        //    }
        //}

        //private TimeSpan TimePerfT(string startingPosition, int depth, ulong expectedResult, int repeatCount, bool useHashing)
        //{
        //    var board = new Board();
        //    board.SetPosition(FenTranslator.ToBoardState(startingPosition));

        //    var timer = new Stopwatch();
        //    timer.Start();

        //    for (var i = 0; i < repeatCount; i++)
        //    {
        //        var perft = new PerfT
        //        {
        //            UseHashing = useHashing
        //        };

        //        var result = perft.Perft(board, depth);

        //        if (result != expectedResult)
        //            LogLine("PERFT FAILED");
        //    }

        //    timer.Stop();
            
        //    //TimeSpan averageSpeed = new TimeSpan(timer.ElapsedMilliseconds / repeatCount);
        //    var averageSpeed = new TimeSpan(timer.Elapsed.Ticks / repeatCount);

        //    return averageSpeed;
        //}

        private void LogLineAsHighlight(string text)
        {
            using (var stream = File.AppendText(m_HighlightsLogFile))
            {
                stream.WriteLine(text);
            }
        }

        private void LogLinesAsHighlight(IEnumerable text)
        {
            using (var stream = File.AppendText(m_HighlightsLogFile))
            {
                foreach (var line in text)
                {
                    stream.WriteLine(line);
                }
            }
        }

        private void LogLine(string text)
        {
            LogLineAsDetailed(text);
            LogLineAsHighlight(text);
        }

        private void LogLineAsDetailed(string text)
        {
            using (var stream = File.AppendText(m_FullLogFile))
            {
                stream.WriteLine(text);
            }
        }

        private void LogLinesAsDetailed(IEnumerable text)
        {
            using (var stream = File.AppendText(m_FullLogFile))
            {
                foreach (var line in text)
                {
                    stream.WriteLine(line);
                }
            }
        }
    }
}
