namespace EngineEvaluation
{
    internal interface IPerformanceEvaluatorFactory
    {
        EnginePerformanceEvaluator CreatePerformanceEvaluator(bool evaluatePerfTPositions,
                                                              bool evaluateTestPositions,
                                                              int problemsPerSuiteLimit);
    }
}
