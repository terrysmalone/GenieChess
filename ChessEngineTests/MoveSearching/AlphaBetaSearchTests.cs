using System;
using System.Collections;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.ScoreCalculation;
using NUnit.Framework;
using static Rhino.Mocks.MockRepository;

namespace ChessEngineTests.MoveSearching
{
    [TestFixture]
    internal sealed class AlphaBetaSearchTests
    {
        private static IEnumerable Constructors_with_null_parameters()
        {
            var stubBoardPosition = GenerateStub<Board>();
            var stubScoreCalculator = GenerateStub<IScoreCalculator>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable ImplicitlyCapturedClosure
            yield return new TestCaseData
            (new TestDelegate(
                () => new AlphaBetaSearch(null, stubScoreCalculator)))
                    .SetName("Null board position");

            yield return new TestCaseData
                (new TestDelegate(
                    () => new AlphaBetaSearch(stubBoardPosition, null)))
                    .SetName("Null score calculator");
            
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestCaseSource(nameof(Constructors_with_null_parameters))]
        public void Cannot_construct_with_null_parameters(TestDelegate constructor)
        {
            Assert.That(constructor, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
