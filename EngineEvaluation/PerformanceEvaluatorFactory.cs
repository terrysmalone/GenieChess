using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    internal sealed class PerformanceEvaluatorFactory : IPerformanceEvaluatorFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();

        public EnginePerformanceEvaluator CreatePerformanceEvaluator(bool evaluatePerfTPositions,
                                                                     bool evaluateTestSuitePositions,
                                                                     bool runFullTestSuiteEvaluation)
        {
            var logFolder = CreateAndGetLogFolder();

            var highlightsLogFile = CreateAndGetLogFile(logFolder, "01 - Overview.txt");

            var evaluators = new List<IEvaluator>();

            // PerfTEvaluator
            if (evaluatePerfTPositions)
            {
                var perfTLogFile = CreateAndGetLogFile(logFolder, "02 - PerfTEvaluator.txt");

                var perfTPositions = m_ResourceLoader.LoadPerfTPositions();

                var perfTEvaluator = new PerfTEvaluator(perfTPositions, highlightsLogFile, perfTLogFile);
                evaluators.Add(perfTEvaluator);
            }

            // Test position evaluator
            if (evaluateTestSuitePositions)
            {
                var testPosLogFile = CreateAndGetLogFile(logFolder, "03 - TestPositionsEvaluator.txt");

                var testPositions = InitialiseTestPositions(runFullTestSuiteEvaluation);

                var testPositionsEvaluator =
                    new TestPositionsEvaluator(testPositions, highlightsLogFile, testPosLogFile);

                evaluators.Add(testPositionsEvaluator);
            }

            return new EnginePerformanceEvaluator(evaluators);
        }

        private List<Tuple<string, List<TestPosition>>> InitialiseTestPositions(bool runFullTestSuiteEvaluation)
        {
            var testSuites = new List<Tuple<string, List<TestPosition>>>();

            var bratkoKopecPositions = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("BratkoKopecTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("BratkoKopecTestSuite", bratkoKopecPositions));

            var kaufmanTestPositions = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("KaufmanTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("KaufmanTestSuite", kaufmanTestPositions));

            var lctIi = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("LctIiTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("LctIiTestSuite", lctIi));

            var nolotTestSuite = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("NolotTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("NolotTestSuite.epd", nolotTestSuite));

            var nullMoveTestSuite = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("NullMoveTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("NullMoveTestSuite.epd", nullMoveTestSuite));

            var silentButDeadly = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("SilentButDeadlyTestSuite.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("SilentButDeadlyTestSuite", silentButDeadly));

            var sts1 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS1.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS1.epd", sts1));

            var sts2 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS2.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS2.epd", sts2));

            var sts3 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS3.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS3.epd", sts3));

            var sts4 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS4.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS4.epd", sts4));

            var sts5 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS5.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS5.epd", sts5));

            var sts6 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS6.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS6.epd", sts6));

            var sts7 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS7.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS7.epd", sts7));

            var sts8 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS8.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS8.epd", sts8));

            var sts9 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS9.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS9.epd", sts9));

            var sts10 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS10.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS10.epd", sts10));

            var sts11 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS11.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS11.epd", sts11));

            var sts12 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS12.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS12.epd", sts12));

            var sts13 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS13.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS13.epd", sts13));

            var sts14 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS14.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS14.epd", sts14));

            var sts15 = m_ResourceLoader.LoadTestPositions(m_ResourceLoader.GetTestResourcePath("STS15.epd"));
            testSuites.Add(new Tuple<string, List<TestPosition>>("STS15.epd", sts15));

            if (!runFullTestSuiteEvaluation)
            {
                var counter = 0;
                var resetAt = 12;

                foreach (var testSuite in testSuites)
                {
                    for (var i = testSuite.Item2.Count - 1; i >= 0; i--)
                    {
                        if (counter != 0)
                        {
                            testSuite.Item2.RemoveAt(i);
                        }

                        if (counter == resetAt)
                        {
                            counter = 0;
                        }
                        else
                        {
                            counter++;
                        }
                    }
                }
            }

            //var numberOfPositions = 0;

            //foreach (var testSuite in testSuites)
            //{
            //    numberOfPositions += testSuite.Item2.Count;
            //}
            

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
    }
}