using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Systems;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Generation;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapManagerTests
    {
        #region Generation Bounds

        [Test]
        public void GenerateMap_OddMapSize_CreatesSymmetricTerrainBoundsAroundZeroWithoutCells()
        {
            var manager = new MapManager();

            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.TryGetTerrain(new MapCoord(-2, -2), out _), Is.True);
            Assert.That(manager.TryGetTerrain(new MapCoord(0, 0), out _), Is.True);
            Assert.That(manager.TryGetTerrain(new MapCoord(2, 2), out _), Is.True);
            Assert.That(manager.TryGetTerrain(new MapCoord(-3, 0), out _), Is.False);
            Assert.That(manager.TryGetTerrain(new MapCoord(3, 0), out _), Is.False);
            Assert.That(manager.TryGetTerrain(new MapCoord(0, -3), out _), Is.False);
            Assert.That(manager.TryGetTerrain(new MapCoord(0, 3), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(-2, -2), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(0, 0), out _), Is.False);
            Assert.That(manager.TryGetCell(new MapCoord(2, 2), out _), Is.False);
        }

        [Test]
        public void GenerateMap_EvenPositiveMapSize_ThrowsArgumentException()
        {
            var manager = new MapManager();

            Assert.Throws<ArgumentException>(() =>
                manager.GenerateMap(seed: 0, mapSize: 4));
        }

        [Test]
        public void ConfiguredMapSize_HasUniqueCenterTile()
        {
            Assert.That(TerrainConfigs.TileCountPerAxis, Is.Positive);
            Assert.That(TerrainConfigs.TileCountPerAxis % 2, Is.EqualTo(1));
        }

        [Test]
        public void GenerateMap_CenteredTerrainMayPopulatePartialBoundaryMapChunks()
        {
            var manager = new MapManager();

            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(manager.ChunkCount, Is.EqualTo(4));
            Assert.That(manager.TryGetChunk(new MapChunkCoord(-1, -1), out var negativeChunk), Is.True);
            Assert.That(negativeChunk!.TerrainTileCount, Is.GreaterThan(0));
            Assert.That(negativeChunk.TerrainTileCount, Is.LessThan(MapChunkMetrics.ChunkSize * MapChunkMetrics.ChunkSize));
            Assert.That(manager.TryGetTerrain(new TerrainTileWorldCoord(-3, 0), out _), Is.False);
        }

        #endregion

        #region Build Elevation Compatibility

        [Test]
        public void CoordinateOnlyBuildOperations_TargetMapChunkTerrainBaseElevation()
        {
            var manager = new MapManager();
            var coord = new MapCoord(-1, 1);
            manager.GenerateMap(seed: 0, mapSize: 5);
            Assert.That(manager.TryGetTerrainBaseElevation(coord, out var baseElevation), Is.True);
            Assert.That(manager.TryGetCell(coord, out _), Is.False);

            var result = manager.TryPlace(BuildKind.Floor, coord);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, baseElevation), out var cell), Is.True);
            Assert.That(cell!.HasFloorAt(baseElevation), Is.True);
        }

        #endregion

        #region Sparse Cell Storage

        [Test]
        public void TryGetCell_GlobalEmptyWorld_DoesNotCreateChunksOrCells()
        {
            var manager = new MapManager();
            var coord = new MapCellCoord(999, 42, -999);

            bool found = manager.TryGetCell(coord, out var cell);

            Assert.That(found, Is.False);
            Assert.That(cell, Is.Null);
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
        }

        [Test]
        public void GenerateMap_DoesNotCreateSparseCells()
        {
            var manager = new MapManager();

            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(manager.ChunkCount, Is.GreaterThan(0));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.TryGetTerrain(new MapCoord(0, 0), out _), Is.True);
            Assert.That(manager.TryGetCell(new MapCoord(0, 0), out _), Is.False);
        }

        [Test]
        public void TryPlaceFloor_ExplicitElevation_CreatesSparseCellAtGlobalCellCoordinate()
        {
            var manager = new MapManager();
            var coord = new MapCoord(4, -3);
            var elevation = new Elevation(2.5f);
            var cellCoord = ToMapCellCoord(coord, elevation);

            var result = manager.TryPlace(BuildKind.Floor, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(1));
            Assert.That(manager.TryGetCell(cellCoord, out var cell), Is.True);
            Assert.That(cell!.Coord, Is.EqualTo(cellCoord));
            Assert.That(cell.HasFloorAt(elevation), Is.True);
            Assert.That(manager.TryGetCell(coord, out _), Is.False);
        }

        [Test]
        public void TryPlaceWall_MissingSparseCell_DoesNotAllocateStorage()
        {
            var manager = new MapManager();
            var coord = new MapCoord(0, 0);
            var elevation = new Elevation(0);

            var result = manager.TryPlace(BuildKind.Wall, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, elevation), out _), Is.False);
        }

        [Test]
        public void TryRemoveMissingSparseCell_DoesNotAllocateStorage()
        {
            var manager = new MapManager();
            var coord = new MapCoord(0, 0);
            var elevation = new Elevation(0);

            var result = manager.TryRemove(BuildKind.Floor, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, elevation), out _), Is.False);
        }

        [Test]
        public void ExplicitElevationValidationAndHas_DoNotAllocateSparseStorage()
        {
            var manager = new MapManager();
            var coord = new MapCoord(0, 0);
            var elevation = new Elevation(1.5f);

            var canPlaceFloor = manager.CanPlace(BuildKind.Floor, coord, elevation);
            var canPlaceWall = manager.CanPlace(BuildKind.Wall, coord, elevation);
            var canRemoveFloor = manager.CanRemove(BuildKind.Floor, coord, elevation);
            bool hasFloor = manager.Has(BuildKind.Floor, coord, elevation);

            Assert.That(canPlaceFloor.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(canPlaceWall.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(canPlaceWall.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoFloor));
            Assert.That(canRemoveFloor.Outcome, Is.EqualTo(BuildOperationOutcome.NoOp));
            Assert.That(hasFloor, Is.False);
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
        }

        [Test]
        public void CoordinateOnlyValidation_WithTerrain_DoesNotCreateSparseCells()
        {
            var manager = new MapManager();
            var coord = new MapCoord(0, 0);
            manager.GenerateMap(seed: 0, mapSize: 5);
            int chunkCount = manager.ChunkCount;

            var result = manager.CanPlace(BuildKind.Floor, coord);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
            Assert.That(manager.TryGetCell(coord, out _), Is.False);
        }

        [Test]
        public void CoordinateOnlyValidation_WithoutTerrain_ReturnsNoCellWithoutAllocating()
        {
            var manager = new MapManager();

            var result = manager.CanPlace(BuildKind.Floor, new MapCoord(99, 0));

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(BuildOperationFailureReason.NoCell));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
        }

        [Test]
        public void TryPlaceFloor_SameHorizontalCoordAtDifferentElevations_CreatesDistinctCellsInSameChunk()
        {
            var manager = new MapManager();
            var coord = new MapCoord(2, -3);
            var lower = new Elevation(-1f);
            var origin = new Elevation(0f);
            var upper = new Elevation(1.5f);

            manager.TryPlace(BuildKind.Floor, coord, lower);
            manager.TryPlace(BuildKind.Floor, coord, origin);
            manager.TryPlace(BuildKind.Floor, coord, upper);

            Assert.That(manager.ChunkCount, Is.EqualTo(1));
            Assert.That(manager.CellCount, Is.EqualTo(3));
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, lower), out var lowerCell), Is.True);
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, origin), out var originCell), Is.True);
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, upper), out var upperCell), Is.True);
            Assert.That(lowerCell, Is.Not.SameAs(originCell));
            Assert.That(originCell, Is.Not.SameAs(upperCell));
            Assert.That(lowerCell!.Coord.Y, Is.EqualTo(lower.MapCellY));
            Assert.That(originCell!.Coord.Y, Is.EqualTo(origin.MapCellY));
            Assert.That(upperCell!.Coord.Y, Is.EqualTo(upper.MapCellY));
        }

        [Test]
        public void ResolveChunkCoord_DifferentGlobalY_ReturnsSameHorizontalChunkAndLocalCoord()
        {
            var manager = new MapManager();
            var lower = new MapCellCoord(-1, -100, 15);
            var upper = new MapCellCoord(-1, 100, 15);

            Assert.That(manager.ResolveChunkCoord(lower), Is.EqualTo(manager.ResolveChunkCoord(upper)));
            Assert.That(manager.ResolveChunkLocalCoord(lower), Is.EqualTo(manager.ResolveChunkLocalCoord(upper)));
        }

        [Test]
        public void GetOrCreateCell_RoutesSparseCellsAcrossPositiveNegativeBoundariesAndCorners()
        {
            int size = MapChunkMetrics.ChunkSize;
            var manager = new MapManager();

            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(size - 1, 0, 0),
                new MapChunkCoord(0, 0),
                new MapChunkLocalCoord(size - 1, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(size, 0, 0),
                new MapChunkCoord(1, 0),
                new MapChunkLocalCoord(0, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(0, 0, size),
                new MapChunkCoord(0, 1),
                new MapChunkLocalCoord(0, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(size, 0, size),
                new MapChunkCoord(1, 1),
                new MapChunkLocalCoord(0, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(-1, 0, 0),
                new MapChunkCoord(-1, 0),
                new MapChunkLocalCoord(size - 1, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(-size, 0, 0),
                new MapChunkCoord(-1, 0),
                new MapChunkLocalCoord(0, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(-size - 1, 0, 0),
                new MapChunkCoord(-2, 0),
                new MapChunkLocalCoord(size - 1, 0));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(0, 0, -1),
                new MapChunkCoord(0, -1),
                new MapChunkLocalCoord(0, size - 1));
            AssertSparseCellRoutesToChunk(
                manager,
                new MapCellCoord(-1, 0, -1),
                new MapChunkCoord(-1, -1),
                new MapChunkLocalCoord(size - 1, size - 1));
        }

        [Test]
        public void TryGetCell_AdjacentSparseCellsAcrossChunkBoundary_RetrievesBoth()
        {
            int size = MapChunkMetrics.ChunkSize;
            var manager = new MapManager();
            var westCoord = new MapCoord(size - 1, 0);
            var eastCoord = new MapCoord(size, 0);
            var elevation = new Elevation(0);

            manager.TryPlace(BuildKind.Floor, westCoord, elevation);
            manager.TryPlace(BuildKind.Floor, eastCoord, elevation);

            Assert.That(manager.CellCount, Is.EqualTo(2));
            Assert.That(manager.ChunkCount, Is.EqualTo(2));
            Assert.That(manager.TryGetCell(ToMapCellCoord(westCoord, elevation), out var westCell), Is.True);
            Assert.That(manager.TryGetCell(ToMapCellCoord(eastCoord, elevation), out var eastCell), Is.True);
            Assert.That(westCell!.HasFloor(), Is.True);
            Assert.That(eastCell!.HasFloor(), Is.True);
        }

        [Test]
        public void TryRemoveFloor_LastBuildable_RemovesSparseCell()
        {
            var manager = new MapManager();
            var coord = new MapCoord(-2, 3);
            var elevation = new Elevation(1);
            var cellCoord = ToMapCellCoord(coord, elevation);
            manager.TryPlace(BuildKind.Floor, coord, elevation);
            Assert.That(manager.TryGetCell(cellCoord, out _), Is.True);

            var result = manager.TryRemove(BuildKind.Floor, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.TryGetCell(cellCoord, out _), Is.False);
        }

        [Test]
        public void TryRemoveWall_CellStillContainingFloor_RemainsStored()
        {
            var manager = new MapManager();
            var coord = new MapCoord(-2, 3);
            var elevation = new Elevation(1);
            var cellCoord = ToMapCellCoord(coord, elevation);
            manager.TryPlace(BuildKind.Floor, coord, elevation);
            manager.TryPlace(BuildKind.Wall, coord, elevation);

            var result = manager.TryRemove(BuildKind.Wall, coord, elevation);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(1));
            Assert.That(manager.TryGetCell(cellCoord, out var cell), Is.True);
            Assert.That(cell!.HasFloor(), Is.True);
            Assert.That(cell.HasWall(), Is.False);
        }

        [Test]
        public void TryRemoveFloor_AfterCoordinateOnlyBuild_RemovesSparseCellAndKeepsTerrain()
        {
            var manager = new MapManager();
            var coord = new MapCoord(0, 0);
            manager.GenerateMap(seed: 0, mapSize: 5);
            Assert.That(manager.TryGetTerrainBaseElevation(coord, out var baseElevation), Is.True);
            manager.TryPlace(BuildKind.Floor, coord);

            var result = manager.TryRemove(BuildKind.Floor, coord);

            Assert.That(result.Outcome, Is.EqualTo(BuildOperationOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.TryGetCell(ToMapCellCoord(coord, baseElevation), out _), Is.False);
            Assert.That(manager.TryGetCell(coord, out _), Is.False);
            Assert.That(manager.TryGetTerrain(coord, out _), Is.True);
        }

        #endregion

        #region Terrain Queries

        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        public void TryGetTerrainBaseElevation_ExistingCoordinate_ReturnsTerrainBaseElevation(int x, int y)
        {
            var manager = new MapManager();
            var coord = new MapCoord(x, y);
            manager.GenerateMap(seed: 0, mapSize: 5);
            Assert.That(manager.TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Y), out var terrainTile), Is.True);

            bool found = manager.TryGetTerrainBaseElevation(coord, out var elevation);

            Assert.That(found, Is.True);
            Assert.That(elevation, Is.EqualTo(terrainTile.BaseElevation));
        }

        [Test]
        public void TryGetTerrainBaseElevation_MissingCoordinate_ReturnsFalse()
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            bool found = manager.TryGetTerrainBaseElevation(new MapCoord(3, 0), out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryGetTerrain_TerrainWorldCoordAndMapCoordIdentifySameTile()
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(0, 0), out var centerTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(0, 0), out var centerMapTerrain),
                Is.True);
            Assert.That(centerTerrain, Is.SameAs(centerMapTerrain));

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(1, 0), out var eastTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(1, 0), out var eastMapTerrain),
                Is.True);
            Assert.That(eastTerrain, Is.SameAs(eastMapTerrain));

            Assert.That(
                manager.TryGetTerrain(new TerrainTileWorldCoord(-1, 0), out var westTerrain),
                Is.True);
            Assert.That(
                manager.TryGetTerrain(new MapCoord(-1, 0), out var westMapTerrain),
                Is.True);
            Assert.That(westTerrain, Is.SameAs(westMapTerrain));
        }

        [TestCase(1, 1)]
        [TestCase(-1, -1)]
        [TestCase(15, 0)]
        [TestCase(-15, 0)]
        public void TryGetTerrain_GlobalTerrainCoordinate_RoutesThroughMapChunks(int x, int y)
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 33);

            bool found = manager.TryGetTerrain(new TerrainTileWorldCoord(x, y), out var terrain);

            Assert.That(found, Is.True);
            Assert.That(terrain, Is.Not.Null);
        }

        [Test]
        public void TryGetTerrain_AdjacentCoordinatesAcrossMapChunkBoundary_ResolveTransparently()
        {
            int size = MapChunkMetrics.ChunkSize;
            var westOfBoundary = new TerrainTileWorldCoord(size - 1, 0);
            var eastOfBoundary = new TerrainTileWorldCoord(size, 0);
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 33);

            Assert.That(MapMath.GlobalToChunk(westOfBoundary.X, westOfBoundary.Y), Is.EqualTo(new MapChunkCoord(0, 0)));
            Assert.That(MapMath.GlobalToChunk(eastOfBoundary.X, eastOfBoundary.Y), Is.EqualTo(new MapChunkCoord(1, 0)));
            Assert.That(manager.TryGetTerrain(westOfBoundary, out var westTerrain), Is.True);
            Assert.That(manager.TryGetTerrain(eastOfBoundary, out var eastTerrain), Is.True);
            Assert.That(westTerrain, Is.Not.Null);
            Assert.That(eastTerrain, Is.Not.Null);
        }

        [Test]
        public void TryGetTerrain_TerrainWorldY_RoutesToMapChunkZ()
        {
            var coord = new TerrainTileWorldCoord(0, 15);
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 33);

            Assert.That(manager.TryGetTerrain(coord, out var terrain), Is.True);
            Assert.That(manager.TryGetChunk(new MapChunkCoord(0, 1), out var chunk), Is.True);
            Assert.That(chunk!.TryGetTerrain(new MapChunkLocalCoord(0, 0), out var chunkTerrain), Is.True);
            Assert.That(chunkTerrain, Is.SameAs(terrain));
        }

        [Test]
        public void TryGetTerrain_OutsideGeneratedTerrain_ReturnsFalse()
        {
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            bool found = manager.TryGetTerrain(new TerrainTileWorldCoord(99, 0), out _);

            Assert.That(found, Is.False);
        }

        #endregion

        #region Deterministic Generation

        [Test]
        public void GenerateMap_SameSeedAndMapSize_ProducesEquivalentTerrainValues()
        {
            const int seed = 23;
            const int mapSize = 33;
            var manager = new MapManager();
            var terrainSystem = new TerrainSystem();
            var expectedSink = new TestTerrainTileSink();

            terrainSystem.GenerateTerrain(seed, mapSize, expectedSink);
            terrainSystem.ResolveSlopeNeighbors(
                expectedSink.Received.Keys,
                coord => expectedSink.Received.TryGetValue(coord, out var tile) ? tile : null);

            manager.GenerateMap(seed, mapSize);

            foreach (var expected in expectedSink.Received)
            {
                Assert.That(manager.TryGetTerrain(expected.Key, out var actual), Is.True);
                AssertTerrainEquivalent(actual, expected.Value);
            }
        }

        [TestCase(-2, -2)]
        [TestCase(0, 0)]
        [TestCase(2, 2)]
        public void GenerateMap_UsesTerrainWorldCoordinatesForHeightSampling(int x, int y)
        {
            const int seed = 17;
            var coord = new TerrainTileWorldCoord(x, y);
            var manager = new MapManager();
            var heightGenerator = new TerrainHeightGenerator(seed);
            var origin = TerrainMath.TileToWorldOrigin(coord);

            manager.GenerateMap(seed, mapSize: 5);

            Assert.That(manager.TryGetTerrain(coord, out var terrain), Is.True);
            Assert.That(terrain.TopLeftHeight, Is.EqualTo(heightGenerator.GetHeight(origin.X, origin.Y)));
            Assert.That(terrain.TopRightHeight, Is.EqualTo(heightGenerator.GetHeight(origin.X + WorldGridMetrics.GridUnitSize, origin.Y)));
            Assert.That(terrain.BottomLeftHeight, Is.EqualTo(heightGenerator.GetHeight(origin.X, origin.Y + WorldGridMetrics.GridUnitSize)));
            Assert.That(terrain.BottomRightHeight, Is.EqualTo(heightGenerator.GetHeight(origin.X + WorldGridMetrics.GridUnitSize, origin.Y + WorldGridMetrics.GridUnitSize)));
        }

        #endregion

        #region Slope Neighbors

        [Test]
        public void ResolveSlopeNeighbors_SlopedNeighborAcrossMapChunkBoundary_UpdatesFlatTileMask()
        {
            int size = MapChunkMetrics.ChunkSize;
            var flatCoord = new TerrainTileWorldCoord(size - 1, 0);
            var slopeCoord = new TerrainTileWorldCoord(size, 0);
            var flat = FlatTile(1f);
            var slope = new TerrainTile(1f, 2f, 1f, 1f);
            var manager = new MapManager();
            var terrainSystem = new TerrainSystem();

            manager.StoreGeneratedTerrain(flatCoord, flat);
            manager.StoreGeneratedTerrain(slopeCoord, slope);

            terrainSystem.ResolveSlopeNeighbors(
                new[] { flatCoord, slopeCoord },
                coord => manager.TryGetTerrain(coord, out var terrain) ? terrain : null);

            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.East));
        }

        #endregion

        #region Terrain Mutation Boundary

        [Test]
        public void TryReplaceTerrain_ExistingTerrain_ReplacesThroughGlobalMapBoundary()
        {
            var coord = new TerrainTileWorldCoord(-1, 15);
            var manager = new MapManager();
            manager.StoreGeneratedTerrain(coord, FlatTile(1f));
            var replacement = FlatTile(4f);

            bool replaced = manager.TryReplaceTerrain(coord, replacement);

            Assert.That(replaced, Is.True);
            Assert.That(manager.TryGetTerrain(coord, out var terrain), Is.True);
            Assert.That(terrain, Is.SameAs(replacement));
        }

        [Test]
        public void TryReplaceTerrain_FlatToSlopeAcrossMapChunkBoundary_UpdatesAdjacentFlatTileMask()
        {
            int size = MapChunkMetrics.ChunkSize;
            var flatCoord = new TerrainTileWorldCoord(size - 1, 0);
            var eastCoord = new TerrainTileWorldCoord(size, 0);
            var flat = FlatTile(1f);
            var manager = new MapManager();
            manager.StoreGeneratedTerrain(flatCoord, flat);
            manager.StoreGeneratedTerrain(eastCoord, FlatTile(1f));
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));

            bool replaced = manager.TryReplaceTerrain(eastCoord, SlopedTile());

            Assert.That(replaced, Is.True);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.East));
        }

        [Test]
        public void TryReplaceTerrain_SlopeToFlatAcrossMapChunkBoundary_RemovesAdjacentFlatTileMask()
        {
            int size = MapChunkMetrics.ChunkSize;
            var flatCoord = new TerrainTileWorldCoord(size - 1, 0);
            var eastCoord = new TerrainTileWorldCoord(size, 0);
            var flat = FlatTile(1f);
            var manager = new MapManager();
            manager.StoreGeneratedTerrain(flatCoord, flat);
            manager.StoreGeneratedTerrain(eastCoord, SlopedTile());
            ResolveSlopeNeighbors(manager, flatCoord, eastCoord);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.East));

            bool replaced = manager.TryReplaceTerrain(eastCoord, FlatTile(1f));

            Assert.That(replaced, Is.True);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));
        }

        [TestCase(int.MaxValue, 0, int.MaxValue - 1, 0, SlopeNeighborMask.East)]
        [TestCase(int.MinValue, 0, int.MinValue + 1, 0, SlopeNeighborMask.West)]
        [TestCase(0, int.MaxValue, 0, int.MaxValue - 1, SlopeNeighborMask.South)]
        [TestCase(0, int.MinValue, 0, int.MinValue + 1, SlopeNeighborMask.North)]
        public void TryReplaceTerrain_IntBoundaryCoordinate_RefreshesRepresentableNeighborWithoutCreatingChunks(
            int changedX,
            int changedY,
            int flatX,
            int flatY,
            SlopeNeighborMask expectedMask)
        {
            var changedCoord = new TerrainTileWorldCoord(changedX, changedY);
            var flatCoord = new TerrainTileWorldCoord(flatX, flatY);
            var flat = FlatTile(1f);
            var manager = new MapManager();
            manager.StoreGeneratedTerrain(flatCoord, flat);
            manager.StoreGeneratedTerrain(changedCoord, FlatTile(1f));
            int chunkCount = manager.ChunkCount;
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.None));

            bool replaced = manager.TryReplaceTerrain(changedCoord, SlopedTile());

            Assert.That(replaced, Is.True);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(expectedMask));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
        }

        [Test]
        public void TryReplaceTerrain_MissingTerrain_ReturnsFalseWithoutCreatingTerrain()
        {
            var coord = new TerrainTileWorldCoord(99, 0);
            var manager = new MapManager();
            manager.GenerateMap(seed: 0, mapSize: 5);

            bool replaced = manager.TryReplaceTerrain(coord, FlatTile(4f));

            Assert.That(replaced, Is.False);
            Assert.That(manager.TryGetTerrain(coord, out _), Is.False);
        }

        [Test]
        public void TryReplaceTerrain_MissingTerrain_DoesNotRefreshOrCreateNeighboringState()
        {
            int size = MapChunkMetrics.ChunkSize;
            var flatCoord = new TerrainTileWorldCoord(size - 1, 0);
            var eastCoord = new TerrainTileWorldCoord(size, 0);
            var missingCoord = new TerrainTileWorldCoord(size * 3, 0);
            var flat = FlatTile(1f);
            var manager = new MapManager();
            manager.StoreGeneratedTerrain(flatCoord, flat);
            manager.StoreGeneratedTerrain(eastCoord, SlopedTile());
            ResolveSlopeNeighbors(manager, flatCoord, eastCoord);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.East));
            int chunkCount = manager.ChunkCount;

            bool replaced = manager.TryReplaceTerrain(missingCoord, FlatTile(1f));

            Assert.That(replaced, Is.False);
            Assert.That(flat.SlopeNeighborMask, Is.EqualTo(SlopeNeighborMask.East));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
            Assert.That(manager.TryGetTerrain(missingCoord, out _), Is.False);
        }

        #endregion

        #region Helpers

        private static MapCellCoord ToMapCellCoord(MapCoord coord, Elevation elevation)
        {
            return new MapCellCoord(coord.X, elevation.MapCellY, coord.Y);
        }

        private static void AssertSparseCellRoutesToChunk(
            MapManager manager,
            MapCellCoord cellCoord,
            MapChunkCoord expectedChunkCoord,
            MapChunkLocalCoord expectedLocalCoord)
        {
            var cell = manager.GetOrCreateCell(cellCoord);

            Assert.That(cell.Coord, Is.EqualTo(cellCoord));
            Assert.That(manager.ResolveChunkCoord(cellCoord), Is.EqualTo(expectedChunkCoord));
            Assert.That(manager.ResolveChunkLocalCoord(cellCoord), Is.EqualTo(expectedLocalCoord));
            Assert.That(manager.TryGetCell(cellCoord, out var storedCell), Is.True);
            Assert.That(storedCell, Is.SameAs(cell));
            Assert.That(manager.TryGetChunk(expectedChunkCoord, out var chunk), Is.True);
            Assert.That(chunk!.Contains(cellCoord), Is.True);
        }

        private static TerrainTile FlatTile(float height)
        {
            return new TerrainTile(height, height, height, height);
        }

        private static TerrainTile SlopedTile()
        {
            return new TerrainTile(1f, 2f, 1f, 1f);
        }

        private static void ResolveSlopeNeighbors(MapManager manager, params TerrainTileWorldCoord[] coords)
        {
            var terrainSystem = new TerrainSystem();

            terrainSystem.ResolveSlopeNeighbors(
                coords,
                coord => manager.TryGetTerrain(coord, out var terrain) ? terrain : null);
        }

        private static void AssertTerrainEquivalent(TerrainTile actual, TerrainTile expected)
        {
            Assert.That(actual.TopLeftHeight, Is.EqualTo(expected.TopLeftHeight));
            Assert.That(actual.TopRightHeight, Is.EqualTo(expected.TopRightHeight));
            Assert.That(actual.BottomLeftHeight, Is.EqualTo(expected.BottomLeftHeight));
            Assert.That(actual.BottomRightHeight, Is.EqualTo(expected.BottomRightHeight));
            Assert.That(actual.IsSlope, Is.EqualTo(expected.IsSlope));
            Assert.That(actual.BaseElevation, Is.EqualTo(expected.BaseElevation));
            Assert.That(actual.SlopeNeighborMask, Is.EqualTo(expected.SlopeNeighborMask));
        }

        private sealed class TestTerrainTileSink : ITerrainTileSink
        {
            private readonly Dictionary<TerrainTileWorldCoord, TerrainTile> _received = new();

            internal Dictionary<TerrainTileWorldCoord, TerrainTile> Received => _received;

            public void ReceiveTerrainTile(TerrainTileWorldCoord coord, TerrainTile tile)
            {
                _received.Add(coord, tile);
            }
        }

        #endregion
    }
}
