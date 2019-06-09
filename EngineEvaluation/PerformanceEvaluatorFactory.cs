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
                                                                     bool evaluateTestPositions)
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
            if (evaluateTestPositions)
            {
                var testPosLogFile = CreateAndGetLogFile(logFolder, "03 - TestPositionsEvaluator.txt");

                var testPositions = new List<Tuple<string, List<TestPosition>>>();

                var kaufmanTestPositions = m_ResourceLoader.LoadKaufmanTestPositions();
                testPositions.Add(new Tuple<string, List<TestPosition>>("kaufmanTestPositions", kaufmanTestPositions));

                var bratkoKopecPositions = m_ResourceLoader.LoadBratkoKopecPositions();
                testPositions.Add(new Tuple<string, List<TestPosition>>("BratkoKopecPositions", bratkoKopecPositions));
                
                var lctII = m_ResourceLoader.LoadTestPositions("LCTII.txt");
                testPositions.Add(new Tuple<string, List<TestPosition>>("LCTII", lctII));

                var testPositionsEvaluator =
                    new TestPositionsEvaluator(testPositions, highlightsLogFile, testPosLogFile);
                evaluators.Add(testPositionsEvaluator);
            }

            return new EnginePerformanceEvaluator(evaluators);
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