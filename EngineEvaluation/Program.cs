namespace EngineEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var performanceEvaluatorFactory = new PerformanceEvaluatorFactory();

            var engineEvaluation = performanceEvaluatorFactory.CreatePerformanceEvaluator(evaluatePerfTPositions:     false, 
                                                                                          evaluateTestSuitePositions: true,
                                                                                          runFullTestSuiteEvaluation: true);
            
            engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 2);
        }
    }
}
