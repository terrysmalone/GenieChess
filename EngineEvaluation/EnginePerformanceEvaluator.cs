using Logging;

namespace EngineEvaluation;

// Runs and logs a full performance evaluation
public class EnginePerformanceEvaluator
{
    private readonly List<IEvaluator> _evaluators;

    public EnginePerformanceEvaluator(List<IEvaluator> evaluators, ILog log)
    {

        if (evaluators == null)
        {
            log.Error("Evaluators list passed into EnginePerformanceEvaluator was null");
            throw new ArgumentNullException(nameof(evaluators));
        }

        _evaluators = evaluators;
    }

    public void RunFullPerformanceEvaluation(int maxDepth)
    {
        foreach (var evaluator in _evaluators)
        {
            evaluator.Evaluate(maxDepth);
        }
    }
}

