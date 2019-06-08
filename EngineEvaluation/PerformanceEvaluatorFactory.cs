using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace EngineEvaluation
{
    internal sealed class PerformanceEvaluatorFactory : IPerformanceEvaluatorFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EnginePerformanceEvaluator CreatePerformanceEvaluator()
        {
            var logFolder = CreateAndGetLogFolder();

            var highlightsLogFile = CreateAndGetLogFile(logFolder, "01 - Overview.txt");

            var evaluators = new List<IEvaluator>();

            // PerfTEvaluator
            var perfTLogFile = CreateAndGetLogFile(logFolder, "02 - PerfTEvaluator.txt");

            var perfTEvaluator = new PerfTEvaluator(highlightsLogFile, perfTLogFile);
            evaluators.Add(perfTEvaluator);
            
            return new EnginePerformanceEvaluator(logFolder, highlightsLogFile,  evaluators);
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
                File.Create(logFile);
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