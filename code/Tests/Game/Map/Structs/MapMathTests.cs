using CosmosCasino.Core.Game;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal class MapMathTests
    {
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
            Assert.That(worldX, Is.LessThan(origin.X + MapMath.CellSize));
            Assert.That(worldY, Is.GreaterThanOrEqualTo(origin.Y));
            Assert.That(worldY, Is.LessThan(origin.Y + MapMath.CellSize));
            Assert.That(MapMath.WorldToCell(center), Is.EqualTo(expectedCell));
        }

        #endregion
    }
}
