namespace EngineEvaluation
{
    internal interface IPerformanceEvaluatorFactory
    {
        EnginePerformanceEvaluator CreatePerformanceEvaluator(bool evaluatePerfTPositions,
                                                              bool evaluateMateInXPositions,
                                                              bool evaluateTestPositions,
                                                              int problemsPerSuiteLimit);
    }
}
