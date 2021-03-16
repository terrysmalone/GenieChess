namespace EngineEvaluation
{
    public interface IEvaluator
    {
        void Evaluate(int maxDepth, int maxThinkingSeconds);

        void Evaluate(int maxDepth);
    }
}