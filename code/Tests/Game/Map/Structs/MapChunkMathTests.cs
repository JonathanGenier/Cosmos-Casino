using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapChunkMathTests
    {
        #region Global To Chunk

        [Test]
        public void GlobalToChunk_MapsOriginAndPositiveBoundaries()
        {
            int size = MapChunkMetrics.ChunkSize;

            Assert.That(MapMath.GlobalToChunk(0, 0), Is.EqualTo(new MapChunkCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunk(size - 1, size - 1), Is.EqualTo(new MapChunkCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunk(size, size), Is.EqualTo(new MapChunkCoord(1, 1)));
            Assert.That(MapMath.GlobalToChunk((size * 3) + 2, (size * 4) + 1), Is.EqualTo(new MapChunkCoord(3, 4)));
        }

        [Test]
        public void GlobalToChunk_MapsNegativeBoundariesUsingFloorDivision()
        {
            int size = MapChunkMetrics.ChunkSize;

            Assert.That(MapMath.GlobalToChunk(-1, -1), Is.EqualTo(new MapChunkCoord(-1, -1)));
            Assert.That(MapMath.GlobalToChunk(-size, -size), Is.EqualTo(new MapChunkCoord(-1, -1)));
            Assert.That(MapMath.GlobalToChunk(-size - 1, -size - 1), Is.EqualTo(new MapChunkCoord(-2, -2)));
            Assert.That(MapMath.GlobalToChunk((-size * 3) - 1, (-size * 4) - 1), Is.EqualTo(new MapChunkCoord(-4, -5)));
        }

        #endregion

        #region Global To Local

        [Test]
        public void GlobalToChunkLocal_MapsOriginAndPositiveBoundaries()
        {
            int size = MapChunkMetrics.ChunkSize;

            Assert.That(MapMath.GlobalToChunkLocal(0, 0), Is.EqualTo(new MapChunkLocalCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunkLocal(size - 1, size - 1), Is.EqualTo(new MapChunkLocalCoord(size - 1, size - 1)));
            Assert.That(MapMath.GlobalToChunkLocal(size, size), Is.EqualTo(new MapChunkLocalCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunkLocal((size * 3) + 2, (size * 4) + 1), Is.EqualTo(new MapChunkLocalCoord(2, 1)));
        }

        [Test]
        public void GlobalToChunkLocal_MapsNegativeCoordinatesToPositiveLocalIndexes()
        {
            int size = MapChunkMetrics.ChunkSize;

            Assert.That(MapMath.GlobalToChunkLocal(-1, -1), Is.EqualTo(new MapChunkLocalCoord(size - 1, size - 1)));
            Assert.That(MapMath.GlobalToChunkLocal(-size, -size), Is.EqualTo(new MapChunkLocalCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunkLocal(-size - 1, -size - 1), Is.EqualTo(new MapChunkLocalCoord(size - 1, size - 1)));
            Assert.That(MapMath.GlobalToChunkLocal((-size * 3) - 2, (-size * 4) - 3), Is.EqualTo(new MapChunkLocalCoord(size - 2, size - 3)));
        }

        #endregion

        #region Round Trip

        [Test]
        public void ChunkLocalRoundTrip_ReturnsOriginalGlobalCellCoordinate()
        {
            int size = MapChunkMetrics.ChunkSize;
            var cells = new[]
            {
                new MapCellCoord(0, 0, 0),
                new MapCellCoord(size - 1, 2, size - 1),
                new MapCellCoord(size, -2, size),
                new MapCellCoord(-1, 8, -1),
                new MapCellCoord(-size, -8, -size),
                new MapCellCoord((-size * 3) - 2, 11, (size * 4) + 1),
            };

            foreach (var cell in cells)
            {
                MapChunkCoord chunk = MapMath.CellToChunk(cell);
                MapChunkLocalCoord local = MapMath.CellToChunkLocal(cell);

                MapCellCoord roundTrip = MapMath.ChunkLocalToCell(chunk, local, cell.Y);

                Assert.That(roundTrip, Is.EqualTo(cell));
            }
        }

        [Test]
        public void ChunkLocalToCell_LargestRepresentablePositiveCoordinate_ReturnsIntMaxValue()
        {
            int size = MapChunkMetrics.ChunkSize;
            int chunkAxis = int.MaxValue / size;
            int localAxis = int.MaxValue % size;
            var chunk = new MapChunkCoord(chunkAxis, chunkAxis);
            var local = new MapChunkLocalCoord(localAxis, localAxis);

            MapCellCoord cell = MapMath.ChunkLocalToCell(chunk, local, y: 0);

            Assert.That(cell.X, Is.EqualTo(int.MaxValue));
            Assert.That(cell.Z, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void ChunkLocalToCell_SmallestRepresentableNegativeCoordinate_ReturnsIntMinValue()
        {
            int size = MapChunkMetrics.ChunkSize;
            int localAxis = PositiveModulo(int.MinValue, size);
            int chunkAxis = (int)(((long)int.MinValue - localAxis) / size);
            var chunk = new MapChunkCoord(chunkAxis, chunkAxis);
            var local = new MapChunkLocalCoord(localAxis, localAxis);

            MapCellCoord cell = MapMath.ChunkLocalToCell(chunk, local, y: 0);

            Assert.That(cell.X, Is.EqualTo(int.MinValue));
            Assert.That(cell.Z, Is.EqualTo(int.MinValue));
        }

        [Test]
        public void ChunkLocalToCell_PositiveOverflow_ThrowsArgumentOutOfRangeException()
        {
            int size = MapChunkMetrics.ChunkSize;
            int maxChunkAxis = int.MaxValue / size;
            int maxLocalAxis = int.MaxValue % size;
            var chunk = maxLocalAxis < size - 1
                ? new MapChunkCoord(maxChunkAxis, maxChunkAxis)
                : new MapChunkCoord(maxChunkAxis + 1, maxChunkAxis + 1);
            var local = maxLocalAxis < size - 1
                ? new MapChunkLocalCoord(maxLocalAxis + 1, maxLocalAxis + 1)
                : new MapChunkLocalCoord(0, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                MapMath.ChunkLocalToCell(chunk, local, y: 0));
        }

        [Test]
        public void ChunkLocalToCell_NegativeOverflow_ThrowsArgumentOutOfRangeException()
        {
            int size = MapChunkMetrics.ChunkSize;
            int minLocalAxis = PositiveModulo(int.MinValue, size);
            int minChunkAxis = (int)(((long)int.MinValue - minLocalAxis) / size);
            var chunk = minLocalAxis > 0
                ? new MapChunkCoord(minChunkAxis, minChunkAxis)
                : new MapChunkCoord(minChunkAxis - 1, minChunkAxis - 1);
            var local = minLocalAxis > 0
                ? new MapChunkLocalCoord(minLocalAxis - 1, minLocalAxis - 1)
                : new MapChunkLocalCoord(size - 1, size - 1);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                MapMath.ChunkLocalToCell(chunk, local, y: 0));
        }

        #endregion

        #region Vertical Invariance

        [Test]
        public void CellToChunk_SameHorizontalCoordinatesWithDifferentVerticalY_ResolveToSameChunk()
        {
            var below = new MapCellCoord(5, -100, 7);
            var baseLevel = new MapCellCoord(5, 0, 7);
            var above = new MapCellCoord(5, 100, 7);

            var expected = MapMath.CellToChunk(baseLevel);

            Assert.That(MapMath.CellToChunk(below), Is.EqualTo(expected));
            Assert.That(MapMath.CellToChunk(above), Is.EqualTo(expected));
        }

        [Test]
        public void CellToChunkLocal_SameHorizontalCoordinatesWithDifferentVerticalY_ResolveToSameLocalCoord()
        {
            var below = new MapCellCoord(5, -100, 7);
            var baseLevel = new MapCellCoord(5, 0, 7);
            var above = new MapCellCoord(5, 100, 7);

            var expected = MapMath.CellToChunkLocal(baseLevel);

            Assert.That(MapMath.CellToChunkLocal(below), Is.EqualTo(expected));
            Assert.That(MapMath.CellToChunkLocal(above), Is.EqualTo(expected));
        }

        #endregion

        #region Cross-Boundary Neighbors

        [Test]
        public void CellToChunk_CrossBoundaryNeighborOnX_ResolvesToAdjacentChunk()
        {
            int size = MapChunkMetrics.ChunkSize;
            var westOfBoundary = new MapCellCoord(size - 1, 3, 0);
            var eastOfBoundary = new MapCellCoord(westOfBoundary.X + 1, westOfBoundary.Y, westOfBoundary.Z);

            Assert.That(MapMath.CellToChunk(westOfBoundary), Is.EqualTo(new MapChunkCoord(0, 0)));
            Assert.That(MapMath.CellToChunk(eastOfBoundary), Is.EqualTo(new MapChunkCoord(1, 0)));
        }

        [Test]
        public void CellToChunk_CrossBoundaryNeighborOnNegativeX_ResolvesToAdjacentChunk()
        {
            var eastOfBoundary = new MapCellCoord(0, 3, 0);
            var westOfBoundary = new MapCellCoord(eastOfBoundary.X - 1, eastOfBoundary.Y, eastOfBoundary.Z);

            Assert.That(MapMath.CellToChunk(eastOfBoundary), Is.EqualTo(new MapChunkCoord(0, 0)));
            Assert.That(MapMath.CellToChunk(westOfBoundary), Is.EqualTo(new MapChunkCoord(-1, 0)));
        }

        #endregion

        #region Helpers

        private static int PositiveModulo(int value, int modulus)
        {
            int remainder = value % modulus;
            return remainder < 0 ? remainder + modulus : remainder;
        }

        #endregion
    }
}
