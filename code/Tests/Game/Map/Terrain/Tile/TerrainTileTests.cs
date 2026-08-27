using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map.Terrain.Tile
{
    [TestFixture]
    internal sealed class TerrainTileTests
    {
        #region Constructor / Initialization

        [Test]
        public void Constructor_AllHeightsEqual_IsSlopeFalse()
        {
            // Arrange
            const float height = 10f;

            // Act
            var tile = new TerrainTile(height, height, height, height);

            // Assert
            Assert.That(tile.TopLeftHeight, Is.EqualTo(height));
            Assert.That(tile.TopRightHeight, Is.EqualTo(height));
            Assert.That(tile.BottomLeftHeight, Is.EqualTo(height));
            Assert.That(tile.BottomRightHeight, Is.EqualTo(height));
            Assert.That(tile.IsSlope, Is.False);
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void Constructor_OneCornerDifferent_IsSlopeTrue()
        {
            // Arrange
            const float baseHeight = 10f;
            const float slopeHeight = 11f;

            // Act
            var tile = new TerrainTile(
                baseHeight,
                baseHeight,
                baseHeight,
                slopeHeight);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        [Test]
        public void Constructor_AllCornersDifferent_IsSlopeTrue()
        {
            // Arrange
            // Act
            var tile = new TerrainTile(1f, 2f, 3f, 4f);

            // Assert
            Assert.That(tile.IsSlope, Is.True);
        }

        #endregion

        #region BaseElevation

        [Test]
        public void Constructor_FlatPositiveTile_UsesCornerHeightAsBaseElevation()
        {
            var tile = new TerrainTile(3f, 3f, 3f, 3f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(3)));
        }

        [Test]
        public void Constructor_FlatGridStepTile_PreservesGridStepBaseElevation()
        {
            var tile = new TerrainTile(2f, 2f, 2f, 2f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(2f)));
        }

        [Test]
        public void Constructor_FractionalSlope_UsesFlooredGridStepAsBaseElevation()
        {
            var tile = new TerrainTile(2.5f, 3f, 2.5f, 3f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(2f)));
            Assert.That(tile.IsSlope, Is.True);
        }

        [Test]
        public void Constructor_WholeStepSlope_UsesLowestCornerAsBaseElevation()
        {
            var tile = new TerrainTile(3f, 4f, 3f, 4f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(3f)));
        }

        [Test]
        public void Constructor_NegativeFractionalSlope_UsesFlooredGridStepAsBaseElevation()
        {
            var tile = new TerrainTile(-1.5f, -1f, -1.5f, -1f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(-2f)));
        }

        [TestCase(2.75f, 2f)]
        [TestCase(-1.25f, -2f)]
        public void Constructor_ArbitraryFractionalMinimum_FloorsToGridStep(float minimum, float expected)
        {
            var tile = new TerrainTile(minimum, minimum + 0.25f, minimum, minimum + 0.25f);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(expected)));
        }

        [TestCase(Elevation.MinValue)]
        [TestCase(Elevation.MaxValue)]
        public void Constructor_BaseElevationBoundary_IsValid(float height)
        {
            var tile = new TerrainTile(height, height, height, height);

            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(height)));
        }

        [TestCase(Elevation.MinValue - Elevation.StepSize)]
        [TestCase(Elevation.MaxValue + Elevation.StepSize)]
        public void Constructor_BaseElevationOutsideSupportedRange_Throws(float height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TerrainTile(height, height, height, height));
        }

        #endregion

        #region AddSlopeNeighbor (Flat Tile)

        [Test]
        public void AddSlopeNeighbor_FlatTile_SetsMask()
        {
            // Arrange
            var tile = new TerrainTile(5f, 5f, 5f, 5f);

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.North));
        }

        [Test]
        public void AddSlopeNeighbor_FlatTile_MultipleCalls_AccumulatesMask()
        {
            // Arrange
            var tile = new TerrainTile(5f, 5f, 5f, 5f);

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);
            tile.AddSlopeNeighbor(SlopeNeighborMask.East);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.North | SlopeNeighborMask.East));
        }

        [Test]
        public void AddSlopeNeighbor_FlatTile_SameDirectionTwice_Idempotent()
        {
            // Arrange
            var tile = new TerrainTile(5f, 5f, 5f, 5f);

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.North));
        }

        #endregion

        #region AddSlopeNeighbor (Sloped Tile)

        [Test]
        public void AddSlopeNeighbor_SlopedTile_DoesNothing()
        {
            // Arrange
            var tile = new TerrainTile(5f, 6f, 5f, 5f);

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void AddSlopeNeighbor_SlopedTile_MultipleCalls_StillDoesNothing()
        {
            // Arrange
            var tile = new TerrainTile(1f, 2f, 3f, 4f);

            // Act
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);
            tile.AddSlopeNeighbor(SlopeNeighborMask.East);
            tile.AddSlopeNeighbor(SlopeNeighborMask.South);

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        #endregion

        #region ClearSlopeNeighborMask

        [Test]
        public void ClearSlopeNeighborMask_FlatTile_ResetsMask()
        {
            // Arrange
            var tile = new TerrainTile(5f, 5f, 5f, 5f);
            tile.AddSlopeNeighbor(SlopeNeighborMask.North);
            tile.AddSlopeNeighbor(SlopeNeighborMask.West);

            // Act
            tile.ClearSlopeNeighborMask();

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void ClearSlopeNeighborMask_WhenAlreadyEmpty_RemainsNone()
        {
            // Arrange
            var tile = new TerrainTile(5f, 5f, 5f, 5f);

            // Act
            tile.ClearSlopeNeighborMask();

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [Test]
        public void ClearSlopeNeighborMask_SlopedTile_NoEffectButSafe()
        {
            // Arrange
            var tile = new TerrainTile(1f, 2f, 1f, 1f);

            // Act
            tile.ClearSlopeNeighborMask();

            // Assert
            Assert.That(tile.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        #endregion

        #region Floating Point Edge Cases

        [Test]
        public void Constructor_NaNHeights_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var nan = float.NaN;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TerrainTile(nan, nan, nan, nan));
        }

        [Test]
        public void Constructor_PositiveInfinityVsFinite_IsSlopeAndUsesFiniteMinimum()
        {
            var tile = new TerrainTile(
                float.PositiveInfinity,
                10f,
                10f,
                10f);

            Assert.That(tile.IsSlope, Is.True);
            Assert.That(tile.BaseElevation, Is.EqualTo(new Elevation(10)));
        }

        [Test]
        public void Constructor_AllPositiveInfinity_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var inf = float.PositiveInfinity;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new TerrainTile(inf, inf, inf, inf));
        }

        [Test]
        public void Constructor_NegativeZeroVsZero_IsSlopeFalse()
        {
            // Arrange
            float negativeZero = -0f;
            float positiveZero = 0f;

            // Act
            var tile = new TerrainTile(
                negativeZero,
                positiveZero,
                positiveZero,
                positiveZero);

            // Assert
            Assert.That(tile.IsSlope, Is.False);
        }

        #endregion
    }
}
