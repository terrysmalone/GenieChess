
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChessEngine.BoardRepresentation;
using ChessEngine.Debugging;
using ChessEngine.MoveSearching;
using ChessEngine.NotationHelpers;
using ChessEngine.ScoreCalculation;
using log4net;
using ResourceLoading;

namespace EngineEvaluation
{
    // Runs and logs a full performance evaluation 
    public class EnginePerformanceEvaluator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IEvaluator> m_Evaluators;
        
        private readonly IResourceLoader m_ResourceLoader = new ResourceLoader();
        
        public EnginePerformanceEvaluator(List<IEvaluator> evaluators)
        {
            if (evaluators == null)
            {
                Log.Error("Evaluators list passed into EnginePerformanceEvaluator was null");
                throw new ArgumentNullException(nameof(evaluators));
            }

            m_Evaluators = evaluators;
        }

        public void RunFullPerformanceEvaluation(int maxDepth)
        {
            foreach (var evaluator in m_Evaluators)
            {
                evaluator.Evaluate(maxDepth);
            }
        }
    }
}
