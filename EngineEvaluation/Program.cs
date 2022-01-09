namespace EngineEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var performanceEvaluatorFactory = new PerformanceEvaluatorFactory();

            var engineEvaluation = 
                performanceEvaluatorFactory.CreatePerformanceEvaluator(true,
                                                                       true,
                                                                       false);
            
            //engineEvaluation.RunFullPerformanceEvaluation(maxDepth: 6, maxThinkingSeconds: 10);
            engineEvaluation.RunFullPerformanceEvaluation(5);
        }
    }
}
