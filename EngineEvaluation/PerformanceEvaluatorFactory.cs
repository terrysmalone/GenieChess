﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using log4net;
using ResourceLoading;

using Excel = Microsoft.Office.Interop.Excel;

namespace EngineEvaluation
{
    internal sealed class PerformanceEvaluatorFactory : IPerformanceEvaluatorFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IResourceLoader _resourceLoader = new ResourceLoader();

        public EnginePerformanceEvaluator CreatePerformanceEvaluator(bool evaluatePerfTPositions,
                                                                     bool evaluateTestSuitePositions,
                                                                     int problemPerSuiteLimit = 0)
        {
            var logFolder = CreateAndGetLogFolder();

            var timeStamp = Path.GetFileNameWithoutExtension(logFolder);

            var highlightsLogFile = CreateAndGetLogFile(logFolder, $"01 - Overview - {timeStamp}.txt");

            var evaluators = new List<IEvaluator>();

            // PerfTEvaluator
            if (evaluatePerfTPositions)
            {
                var perfTLogFile = CreateAndGetLogFile(logFolder, $"02 - PerfTEvaluator - {timeStamp}.txt");

                var perfTPositions = _resourceLoader.LoadPerfTPositions();

                var perfTEvaluator = new PerfTEvaluator(perfTPositions, highlightsLogFile, perfTLogFile);
                evaluators.Add(perfTEvaluator);
            }

            // Test position evaluator
            if (evaluateTestSuitePositions)
            {
                var testPosLogFile = CreateAndGetLogFile(logFolder, $"03 - TestPositionsEvaluator - {timeStamp}.txt");

                var testExcelLogFile = CreateAndGetExcelLogFile(logFolder, $"03b - TestPositionsEvaluator - {timeStamp}.xlsx");

                var testPositions = InitialiseTestPositions(problemPerSuiteLimit);


                var testPositionsEvaluator =
                    new TestPositionsEvaluator(testPositions, highlightsLogFile, testPosLogFile, testExcelLogFile);

                evaluators.Add(testPositionsEvaluator);
            }

            return new EnginePerformanceEvaluator(evaluators);
        }

        private List<Tuple<string, List<TestPosition>>> InitialiseTestPositions(int problemPerSuiteLimit)
        {
            var testSuites = new List<Tuple<string, List<TestPosition>>>();

            foreach (var performanceEvaluationFile in _resourceLoader.GetAllPerformanceEvaluationFilePaths())
            {
                var testPositions = _resourceLoader.LoadTestPositions(performanceEvaluationFile, problemPerSuiteLimit);
                testSuites.Add(new Tuple<string, List<TestPosition>>(Path.GetFileName(performanceEvaluationFile), testPositions));
            }

            return testSuites;
        }

        private static string CreateAndGetLogFolder()
        {
            var logLocation = Path.Combine(new[] { Environment.CurrentDirectory,
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") });

            try
            {
                Directory.CreateDirectory(logLocation);
            }
            catch (Exception e)
            {
                Log.Error("Error creating log folder", e);

                throw;
            }

            return logLocation;
        }

        private static string CreateAndGetLogFile(string logLocation, string fileName)
        {

            var logFile = Path.Combine(new[] { logLocation, fileName });

            try
            {
                File.Create(logFile).Close();
            }
            catch (Exception e)
            {
                Log.Error("Error creating log file for PerformanceEvaluator highlights", e);

                throw;
            }

            return logFile;
        }

        private static string CreateAndGetExcelLogFile(string logLocation, string fileName)
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

                testSuiteSheet = excelWorkbook.Worksheets.Add();
                testSuiteSheet.Name = "Test suites";

                testSheet = excelWorkbook.Worksheets.Add();
                testSheet.Name = "Tests";

                excelWorkbook.SaveAs(logFile);

                excelApp.Workbooks.Close();
                excelApp.Quit();
            }
            catch (Exception exc)
            {
                Log.Error($"Error creating excel log file: {fileName}", exc);
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
}
