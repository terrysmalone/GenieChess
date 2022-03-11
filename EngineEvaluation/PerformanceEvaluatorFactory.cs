using System.Runtime.InteropServices;
using Logging;
using ResourceLoading;

using Excel = Microsoft.Office.Interop.Excel;

namespace EngineEvaluation;

internal sealed class PerformanceEvaluatorFactory : IPerformanceEvaluatorFactory
{
    private readonly ILog _log;
    private readonly string _logFolder;

    private readonly IResourceLoader _resourceLoader = new ResourceLoader();

    public PerformanceEvaluatorFactory(ILog log, string logFolder)
    {
        _log = log;
        _logFolder = logFolder;
    }

    public EnginePerformanceEvaluator CreatePerformanceEvaluator(bool evaluatePerfTPositions,
                                                                 bool evaluateMateInXPositions,
                                                                 bool evaluateTestSuitePositions,
                                                                 int problemPerSuiteLimit = 0)
    {
        var timeStamp = Path.GetFileNameWithoutExtension(_logFolder);

        var logFileCounter = 1;

        var highlightsLogFile = CreateAndGetLogFile(_logFolder, $"0{logFileCounter} Overview - {timeStamp}.txt");

        var evaluators = new List<IEvaluator>();

        // PerfTEvaluator
        if (evaluatePerfTPositions)
        {
            logFileCounter++;

            var perfTLogFile = CreateAndGetLogFile(_logFolder, $"0{logFileCounter} PerfTEvaluator - {timeStamp}.txt");

            var perfTPositions = _resourceLoader.LoadPerfTPositions();

            var perfTEvaluator = new PerfTEvaluator(perfTPositions, highlightsLogFile, perfTLogFile, _log);
            evaluators.Add(perfTEvaluator);
        }

        // Mate in X evaluator
        // From https://chess.stackexchange.com/questions/19633/chess-problem-database-with-pgn-or-fen
        if (evaluateMateInXPositions)
        {
            logFileCounter++;

            var mateInXLogFile = CreateAndGetLogFile(_logFolder, $"0{logFileCounter} Mate in X Evaluator - {timeStamp}.txt");

            var mateInXExcelLogFile = CreateAndGetExcelLogFile(_logFolder, $"0{logFileCounter} Mate in X Evaluator  - {timeStamp}.xlsx");


            var mateInXPositions = InitialiseMateInXTestPositions(problemPerSuiteLimit);

            var testPositionsEvaluator =
                new MateInXEvaluator(mateInXPositions, highlightsLogFile, mateInXLogFile, mateInXExcelLogFile, _log);

            evaluators.Add(testPositionsEvaluator);

        }

        // Test position evaluator
        if (evaluateTestSuitePositions)
        {
            logFileCounter++;

            var testPosLogFile = CreateAndGetLogFile(_logFolder, $"0{logFileCounter} TestPositionsEvaluator - {timeStamp}.txt");

            var testExcelLogFile = CreateAndGetExcelLogFile(_logFolder, $"0{logFileCounter} TestPositionsEvaluator - {timeStamp}.xlsx");

            var testPositions = InitialiseTestPositions(problemPerSuiteLimit);

            var testPositionsEvaluator =
                new TestPositionsEvaluator(testPositions, highlightsLogFile, testPosLogFile, testExcelLogFile, _log);

            evaluators.Add(testPositionsEvaluator);
        }

        return new EnginePerformanceEvaluator(evaluators, _log);
    }

    private List<Tuple<string, List<MateInXTestPosition>>> InitialiseMateInXTestPositions(int problemsPerSuiteLimit)
    {
        var testSuites = new List<Tuple<string, List<MateInXTestPosition>>>();

        foreach (var mateInXTestFile in _resourceLoader.GetAllPerformanceEvaluationFilePaths("mateInXTests"))
        {
            var testPositions = _resourceLoader.LoadMateInXPositions(mateInXTestFile, problemsPerSuiteLimit);
            testSuites.Add(new Tuple<string, List<MateInXTestPosition>>(Path.GetFileName(mateInXTestFile), testPositions));
        }

        return testSuites;
    }

    private List<Tuple<string, List<TestPosition>>> InitialiseTestPositions(int problemPerSuiteLimit)
    {
        var testSuites = new List<Tuple<string, List<TestPosition>>>();

        foreach (var performanceEvaluationFile in _resourceLoader.GetAllPerformanceEvaluationFilePaths("performanceEvaluationTests"))
        {
            var testPositions = _resourceLoader.LoadTestPositions(performanceEvaluationFile, problemPerSuiteLimit);
            testSuites.Add(new Tuple<string, List<TestPosition>>(Path.GetFileName(performanceEvaluationFile), testPositions));
        }

        return testSuites;
    }

    private string CreateAndGetLogFile(string logLocation, string fileName)
    {

        var logFile = Path.Combine(new[] { logLocation, fileName });

        try
        {
            File.Create(logFile).Close();
            _log.Info($"Created log file {logFile}");
        }
        catch (Exception e)
        {
            _log.Error("Error creating log file for PerformanceEvaluator highlights", e);

            throw;
        }

        return logFile;
    }

    private string CreateAndGetExcelLogFile(string logLocation, string fileName)
    {
        var logFile = Path.Combine(new[] { logLocation, fileName });

        Excel.Application excelApp = null;
        Excel.Workbook excelWorkbook = null;
        Excel._Worksheet testSuiteSheet = null;
        Excel._Worksheet testSheet = null;

        try
        {
            excelApp = new Excel.Application { Visible = false };
            excelApp.DisplayAlerts = false;

            excelWorkbook = excelApp.Workbooks.Add();

            testSuiteSheet = (Excel._Worksheet)excelWorkbook.Worksheets.Add();
            testSuiteSheet.Name = "Test suites";

            testSheet = (Excel._Worksheet)excelWorkbook.Worksheets.Add();
            testSheet.Name = "Tests";

            excelWorkbook.SaveAs(logFile);

            excelApp.Workbooks.Close();
            excelApp.Quit();

            _log.Info($"Created excel log file {fileName}");
        }
        catch (Exception exc)
        {
            _log.Error($"Error creating excel log file: {fileName}", exc);
        }
        finally
        {
            if (testSheet != null) { Marshal.ReleaseComObject(testSheet); }

            if (testSuiteSheet != null) { Marshal.ReleaseComObject(testSuiteSheet); }

            if (excelWorkbook != null) { Marshal.ReleaseComObject(excelWorkbook); }

            if (excelApp != null) { Marshal.ReleaseComObject(excelApp); }
        }

        return logFile;
    }
}

