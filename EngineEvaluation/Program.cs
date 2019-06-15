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
                                                                                          runFullTestSuiteEvaluation: false);
            
            engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 5);
        }
    }
}
