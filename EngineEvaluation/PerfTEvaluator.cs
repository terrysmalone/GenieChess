using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.PossibleMoves;
using ChessEngineTests;
using Logging;
using ResourceLoading;

namespace EngineEvaluation;

// Benchmarks the time it takes to carry out PerfT evaluations
public sealed class PerfTEvaluator : IEvaluator
{
    private ILog _log;

    private List<PerfTPosition> m_PerfTPositions;

    private readonly string m_FullLogFile;
    private readonly string m_HighlightsLogFile;

    public PerfTEvaluator(List<PerfTPosition> perfTPositions, string highlightsLogFile, string fullLogFile, ILog log)
    {
        _log = log;

        if (perfTPositions is null)
        {
            _log.Error("No perfTPositions were passed to PerfTEvaluator");
            throw new ArgumentNullException(nameof(perfTPositions));
        }

        m_PerfTPositions = perfTPositions;

        if (highlightsLogFile is null)
        {
            _log.Error("No highlightsLogFile was passed to PerfTEvaluator");
            throw new ArgumentNullException(nameof(highlightsLogFile));
        }

        m_HighlightsLogFile = highlightsLogFile;

        if (fullLogFile is null)
        {
            _log.Error("No fullLogFile was passed to PerfTEvaluator");
            throw new ArgumentNullException(nameof(highlightsLogFile));
        }

        m_FullLogFile = fullLogFile;
    }

    public void Evaluate(int evaluationDepth, int maxThinkingSeconds)
    {
        Evaluate(evaluationDepth);
    }

    public void Evaluate(int evaluationDepth)
    {
        LogLine("====================================================================");
        LogLine("PerfTEvaluator");

        LogLineAsDetailed($"Evaluation started at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}");
        LogLineAsDetailed($"Logging PerfT scores with a max search of  {evaluationDepth}");

        foreach (var perfTPos in m_PerfTPositions)
        {
            var maxDepth = Math.Min(evaluationDepth, perfTPos.Results.Count);

            LogLine("--------------------------------------------------------------");
            LogLine($"{perfTPos.Name} - {perfTPos.FenPosition} - Depth {maxDepth}");

            var totalTime = TimeSpan.Zero;

            var passedOverall = "PASSED";

            for (var i = 0; i < maxDepth; i++)
            {
                var depth = i + 1;

                var board = new Board();
                board.SetPosition(perfTPos.FenPosition);

                var perft = new PerfT(new MoveGeneration()) { UseHashing = true };

                var timer = new Stopwatch();
                timer.Start();

                var result = perft.Perft(board, depth);

                timer.Stop();

                totalTime = totalTime.Add(timer.Elapsed);

                LogPerfTResults(depth, result, perfTPos.Results[i], timer.Elapsed);

                if (result != perfTPos.Results[i])
                {
                    passedOverall = "FAILED";
                }
            }

            LogLine($"{passedOverall} - Total time: {totalTime}");
        }
    }

    private void LogPerfTResults(int depth, long result, long expectedResult, TimeSpan elapsedTime)
    {
        var passed = "FAILED";

        if (result == expectedResult)
        {
            passed = "Passed";
        }

        LogLineAsDetailed($"Depth: {depth} - Expected count: {expectedResult} - Actual count {result} - Time {elapsedTime} - {passed}");
    }

    private void LogLine(string text)
    {
        LogLineAsDetailed(text);
        LogLineAsHighlight(text);
    }

    private void LogLineAsHighlight(string text)
    {
        using (var stream = File.AppendText(m_HighlightsLogFile))
        {
            stream.WriteLine(text);
        }
    }

    private void LogLineAsDetailed(string text)
    {
        using (var stream = File.AppendText(m_FullLogFile))
        {
            stream.WriteLine(text);
        }
    }
}

