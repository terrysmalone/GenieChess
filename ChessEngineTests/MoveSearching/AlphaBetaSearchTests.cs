using System;
using System.Collections;
using ChessEngine.BoardRepresentation;
using ChessEngine.MoveSearching;
using ChessEngine.PossibleMoves;
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
            // TODO: Switch away from Rhino mocks so we can make classes sealed (we can't mock a sealed class)
            var stubMoveGeneration = GenerateStub<MoveGeneration>();
            var stubBoardPosition = GenerateStub<Board>();
            var stubScoreCalculator = GenerateStub<IScoreCalculator>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable ImplicitlyCapturedClosure
            yield return new TestCaseData
                (new TestDelegate(
                    () => new AlphaBetaSearch(null, stubBoardPosition, stubScoreCalculator)))
                        .SetName("Null move generation");

            yield return new TestCaseData
                (new TestDelegate(
                    () => new AlphaBetaSearch(stubMoveGeneration, null, stubScoreCalculator)))
                        .SetName("Null board position");

            yield return new TestCaseData
                (new TestDelegate(
                    () => new AlphaBetaSearch(stubMoveGeneration, stubBoardPosition, null)))
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
