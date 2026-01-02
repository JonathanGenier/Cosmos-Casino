using CosmosCasino.Core.Map;
using NUnit.Framework;
using System.Numerics;

namespace CosmosCasino.Tests.Map
{
    [TestFixture]
    internal class MapGridMathTests
    {
        #region WorldToCell

        [Test]
        public void WorldToCell_AtOrigin_ReturnsZeroCell()
        {
            CellCoord cell = MapGridMath.WorldToCell(0f, 0f, 0f);

            Assert.That(cell.X, Is.EqualTo(0));
            Assert.That(cell.Y, Is.EqualTo(0));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_WithinSameCell_ReturnsSameCell()
        {
            CellCoord cell = MapGridMath.WorldToCell(0.1f, 0f, 0.9f);

            Assert.That(cell.X, Is.EqualTo(0));
            Assert.That(cell.Y, Is.EqualTo(0));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_OnPositiveBoundary_SnapsToNextCell()
        {
            CellCoord cell = MapGridMath.WorldToCell(1.0f, 0f, 1.0f);

            Assert.That(cell.X, Is.EqualTo(1));
            Assert.That(cell.Y, Is.EqualTo(1));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_NegativeCoordinates_SnapCorrectly()
        {
            CellCoord cell = MapGridMath.WorldToCell(-0.1f, 0f, -0.1f);

            Assert.That(cell.X, Is.EqualTo(-1));
            Assert.That(cell.Y, Is.EqualTo(-1));
            Assert.That(cell.Z, Is.EqualTo(0));
        }

        [Test]
        public void WorldToCell_IgnoresWorldY()
        {
            CellCoord cellLow = MapGridMath.WorldToCell(0.5f, 0f, 0.5f);
            CellCoord cellHigh = MapGridMath.WorldToCell(0.5f, 999f, 0.5f);

            Assert.That(cellLow, Is.EqualTo(cellHigh));
        }

        #endregion

        #region CellToWorld

        [Test]
        public void CellToWorld_ZeroCell_ReturnsCenteredWorldPosition()
        {
            CellCoord cell = new CellCoord(0, 0, 0);
            Vector3 world = MapGridMath.CellToWorld(cell);

            Assert.That(world.X, Is.EqualTo(0.5f));
            Assert.That(world.Y, Is.EqualTo(0f));
            Assert.That(world.Z, Is.EqualTo(0.5f));
        }

        [Test]
        public void CellToWorld_PositiveCell_ReturnsCorrectCenter()
        {
            CellCoord cell = new CellCoord(2, 3, 0);
            Vector3 world = MapGridMath.CellToWorld(cell);

            Assert.That(world.X, Is.EqualTo(2.5f));
            Assert.That(world.Y, Is.EqualTo(0f));
            Assert.That(world.Z, Is.EqualTo(3.5f));
        }

        [Test]
        public void CellToWorld_NegativeCell_ReturnsCorrectCenter()
        {
            CellCoord cell = new CellCoord(-1, -2, 0);
            Vector3 world = MapGridMath.CellToWorld(cell);

            Assert.That(world.X, Is.EqualTo(-0.5f));
            Assert.That(world.Y, Is.EqualTo(0f));
            Assert.That(world.Z, Is.EqualTo(-1.5f));
        }

        #endregion

        #region Round-trip consistency

        [Test]
        public void WorldToCell_Then_CellToWorld_ReturnsSameCellCenter()
        {
            float worldX = 4.25f;
            float worldZ = 7.9f;

            CellCoord cell = MapGridMath.WorldToCell(worldX, 0f, worldZ);
            Vector3 worldCenter = MapGridMath.CellToWorld(cell);

            Assert.That(worldCenter.X, Is.EqualTo(cell.X + 0.5f));
            Assert.That(worldCenter.Z, Is.EqualTo(cell.Y + 0.5f));
            Assert.That(worldCenter.Y, Is.EqualTo(0f));
        }

        #endregion
    }
}
