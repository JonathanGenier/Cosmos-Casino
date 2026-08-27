using CosmosCasino.Core.Game;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapMathTests
    {
        #region Metrics

        [Test]
        public void VerticalGridUnitSize_MatchesHorizontalGridUnitSize()
        {
            Assert.That(WorldGridMetrics.VerticalGridUnitSize, Is.EqualTo(WorldGridMetrics.GridUnitSize));
        }

        #endregion

        #region World To Cell

        [TestCase(0.0f, 0)]
        [TestCase(0.49f, 0)]
        [TestCase(0.5f, 1)]
        [TestCase(0.999f, 1)]
        [TestCase(1.0f, 1)]
        [TestCase(1.49f, 1)]
        [TestCase(1.5f, 2)]
        [TestCase(12.49f, 12)]
        [TestCase(12.5f, 13)]
        [TestCase(-0.49f, 0)]
        [TestCase(-0.5f, 0)]
        [TestCase(-0.51f, -1)]
        [TestCase(-1.0f, -1)]
        [TestCase(-1.49f, -1)]
        [TestCase(-1.5f, -1)]
        [TestCase(-1.51f, -2)]
        public void WorldToCell_XAxis_UsesCenteredHalfOpenBounds(float worldX, int expectedX)
        {
            var cell = MapMath.WorldToCell(worldX, 0f);

            Assert.That(cell, Is.EqualTo(new MapCoord(expectedX, 0)));
        }

        [TestCase(0.0f, 0)]
        [TestCase(0.49f, 0)]
        [TestCase(0.5f, 1)]
        [TestCase(0.999f, 1)]
        [TestCase(1.0f, 1)]
        [TestCase(1.49f, 1)]
        [TestCase(1.5f, 2)]
        [TestCase(12.49f, 12)]
        [TestCase(12.5f, 13)]
        [TestCase(-0.49f, 0)]
        [TestCase(-0.5f, 0)]
        [TestCase(-0.51f, -1)]
        [TestCase(-1.0f, -1)]
        [TestCase(-1.49f, -1)]
        [TestCase(-1.5f, -1)]
        [TestCase(-1.51f, -2)]
        public void WorldToCell_YAxis_UsesCenteredHalfOpenBounds(float worldY, int expectedY)
        {
            var cell = MapMath.WorldToCell(0f, worldY);

            Assert.That(cell, Is.EqualTo(new MapCoord(0, expectedY)));
        }

        [TestCase(-1.01f, -2)]
        [TestCase(-1.0f, -1)]
        [TestCase(-0.01f, -1)]
        [TestCase(0.0f, 0)]
        [TestCase(0.49f, 0)]
        [TestCase(0.999f, 0)]
        [TestCase(1.0f, 1)]
        [TestCase(1.49f, 1)]
        [TestCase(12.999f, 12)]
        public void WorldToCellY_UsesBasePlaneHalfOpenBounds(float worldY, int expectedCellY)
        {
            int cellY = MapMath.WorldToCellY(worldY);

            Assert.That(cellY, Is.EqualTo(expectedCellY));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void WorldToCellY_NonFiniteHeight_ThrowsArgumentOutOfRangeException(float worldY)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MapMath.WorldToCellY(worldY));
            Assert.That(MapMath.TryWorldToCellY(worldY, out _), Is.False);
        }

        #endregion

        #region Cell To World

        [TestCase(0, 0, 0f, 0f)]
        [TestCase(1, 0, 1f, 0f)]
        [TestCase(-1, 0, -1f, 0f)]
        [TestCase(17, -8, 17f, -8f)]
        public void CellToWorldCenter_ReturnsCenterOfCell(int cellX, int cellY, float expectedX, float expectedY)
        {
            var center = MapMath.CellToWorldCenter(new MapCoord(cellX, cellY));

            Assert.That(center, Is.EqualTo(new WorldCoord(expectedX, expectedY)));
        }

        [TestCase(0, 0, -0.5f, -0.5f)]
        [TestCase(1, 0, 0.5f, -0.5f)]
        [TestCase(-1, 0, -1.5f, -0.5f)]
        [TestCase(-2, -3, -2.5f, -3.5f)]
        public void CellToWorldOrigin_ReturnsMinimumCornerOfCell(int cellX, int cellY, float expectedX, float expectedY)
        {
            var origin = MapMath.CellToWorldOrigin(new MapCoord(cellX, cellY));

            Assert.That(origin, Is.EqualTo(new WorldCoord(expectedX, expectedY)));
        }

        [TestCase(0, 0f)]
        [TestCase(1, WorldGridMetrics.VerticalGridUnitSize)]
        [TestCase(-1, -WorldGridMetrics.VerticalGridUnitSize)]
        [TestCase(41, 41f)]
        [TestCase(100_000, 100_000f)]
        [TestCase(-100_000, -100_000f)]
        public void CellYToWorldBasePlane_UsesUnboundedVerticalGridMetric(int cellY, float expectedWorldY)
        {
            float worldY = MapMath.CellYToWorldBasePlane(cellY);

            Assert.That(worldY, Is.EqualTo(expectedWorldY));
        }

        [TestCase(0, 0.5f)]
        [TestCase(1, 1.5f)]
        [TestCase(-1, -0.5f)]
        [TestCase(41, 41.5f)]
        [TestCase(100_000, 100_000.5f)]
        [TestCase(-100_000, -99_999.5f)]
        public void CellYToWorldCenter_UsesBasePlanePlusHalfVerticalUnit(int cellY, float expectedWorldY)
        {
            float worldY = MapMath.CellYToWorldCenter(cellY);

            Assert.That(worldY, Is.EqualTo(expectedWorldY));
        }

        [Test]
        public void CellYToWorldBasePlane_AdjacentLayersTouchWithoutOverlapOrGap()
        {
            const int lowerCellY = 4;
            const int upperCellY = lowerCellY + 1;

            float lowerTop = MapMath.CellYToWorldBasePlane(lowerCellY)
                + WorldGridMetrics.VerticalGridUnitSize;
            float upperBase = MapMath.CellYToWorldBasePlane(upperCellY);

            Assert.That(upperBase, Is.EqualTo(lowerTop));
        }

        #endregion

        #region Round Trip

        [TestCase(0.0f, 0.0f, 0, 0)]
        [TestCase(0.49f, 0.49f, 0, 0)]
        [TestCase(0.5f, 0.5f, 1, 1)]
        [TestCase(1.0f, 0.0f, 1, 0)]
        [TestCase(1.49f, 2.49f, 1, 2)]
        [TestCase(-0.49f, 0.0f, 0, 0)]
        [TestCase(-0.5f, -0.5f, 0, 0)]
        [TestCase(-0.51f, -0.51f, -1, -1)]
        [TestCase(-1.0f, -1.0f, -1, -1)]
        [TestCase(-1.51f, -2.51f, -2, -3)]
        public void WorldToCell_CellBoundsAndCenter_RoundTripToExpectedCell(
            float worldX,
            float worldY,
            int expectedCellX,
            int expectedCellY)
        {
            var cell = MapMath.WorldToCell(new WorldCoord(worldX, worldY));
            var expectedCell = new MapCoord(expectedCellX, expectedCellY);

            var origin = MapMath.CellToWorldOrigin(cell);
            var center = MapMath.CellToWorldCenter(cell);

            Assert.That(cell, Is.EqualTo(expectedCell));
            Assert.That(worldX, Is.GreaterThanOrEqualTo(origin.X));
            Assert.That(worldX, Is.LessThan(origin.X + WorldGridMetrics.GridUnitSize));
            Assert.That(worldY, Is.GreaterThanOrEqualTo(origin.Y));
            Assert.That(worldY, Is.LessThan(origin.Y + WorldGridMetrics.GridUnitSize));
            Assert.That(MapMath.WorldToCell(center), Is.EqualTo(expectedCell));
        }

        [TestCase(-100_000)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100_000)]
        public void WorldToCellY_CellCenter_RoundTripsToCellY(int cellY)
        {
            float worldCenterY = MapMath.CellYToWorldCenter(cellY);

            Assert.That(MapMath.WorldToCellY(worldCenterY), Is.EqualTo(cellY));
        }

        #endregion
    }
}
