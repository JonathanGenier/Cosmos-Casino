using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Cell;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal class BuildOperationResultTests
    {
        #region TranslateOutcomeFromMapOperationResult

        [TestCase(MapCellOutcome.Placed, BuildOperationOutcome.Placed)]
        [TestCase(MapCellOutcome.Removed, BuildOperationOutcome.Removed)]
        [TestCase(MapCellOutcome.Skipped, BuildOperationOutcome.Skipped)]
        [TestCase(MapCellOutcome.Failed, BuildOperationOutcome.Failed)]
        public void TranslatesOutcomeFromMapOperationResult_TranslatesOutcomeCorrectly(MapCellOutcome mapOutcome, BuildOperationOutcome expected)
        {
            // Arrange
            var cellResult = mapOutcome switch
            {
                MapCellOutcome.Placed => MapCellResult.Placed(),
                MapCellOutcome.Removed => MapCellResult.Removed(),
                MapCellOutcome.Skipped => MapCellResult.Skipped(MapCellFailureReason.None),
                MapCellOutcome.Failed => MapCellResult.Failed(MapCellFailureReason.None),
                _ => throw new ArgumentOutOfRangeException(nameof(mapOutcome))
            };

            var coord = default(MapCellCoord);
            var mapResult = MapOperationResult.FromMapCellResult(coord, cellResult);

            // Act
            var result = BuildOperationResult.FromMapOperationResult(mapResult);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(expected));
        }

        #endregion

        #region TranslateFailureFromMapOperationResult

        [TestCase(MapCellFailureReason.None, BuildOperationFailureReason.None)]
        [TestCase(MapCellFailureReason.NoFloor, BuildOperationFailureReason.NoFloor)]
        [TestCase(MapCellFailureReason.NoWall, BuildOperationFailureReason.NoWall)]
        [TestCase(MapCellFailureReason.Blocked, BuildOperationFailureReason.Blocked)]
        [TestCase(MapCellFailureReason.InternalError, BuildOperationFailureReason.InternalError)]
        [TestCase(MapCellFailureReason.NoCell, BuildOperationFailureReason.NoCell)]
        public void TranslateFailureFromMapOperationResult_TranslatesFailureCorrectly(MapCellFailureReason mapFailureReason, BuildOperationFailureReason expected)
        {
            var cellResult = MapCellResult.Failed(mapFailureReason);

            var coord = default(MapCellCoord);
            var mapResult = MapOperationResult.FromMapCellResult(coord, cellResult);

            // Act
            var result = BuildOperationResult.FromMapOperationResult(mapResult);

            // Assert
            Assert.That(result.FailureReason, Is.EqualTo(expected));
        }

        [Test]
        public void TranslateFailureFromMapOperationResult_ShouldThrow_WhenFailureReasonIsUnknown()
        {
            var invalidReason = (MapCellFailureReason)999;
            var cellResult = MapCellResult.Failed(invalidReason);
            var mapResult = MapOperationResult.FromMapCellResult(default, cellResult);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                BuildOperationResult.FromMapOperationResult(mapResult)
            );
        }

        #endregion
    }
}
