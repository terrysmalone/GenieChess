using Logging;

namespace EngineEvaluation;

internal static class Program
{
    private static void Main(string[] args)
    {
        var logFolder = CreateAndGetLogFolder();

        var logger = new SerilogLog(Path.Combine(logFolder, "Debug.log"));

        logger.Info("Created log file");

        var performanceEvaluatorFactory = new PerformanceEvaluatorFactory(logger, logFolder);

        var engineEvaluation =
            performanceEvaluatorFactory.CreatePerformanceEvaluator(false,
                                                                   false,
                                                                   true,
                                                                   5);

        //engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 6, maxThinkingSeconds: 10);
        engineEvaluation.RunFullPerformanceEvaluation(5);
    }

    private static string CreateAndGetLogFolder()
    {
        var logLocation = Path.Combine(new[] { Environment.CurrentDirectory,
                                           DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") });

        Directory.CreateDirectory(logLocation);

        return logLocation;
    }
}

