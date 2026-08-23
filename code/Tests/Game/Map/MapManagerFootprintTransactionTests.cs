using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapManagerFootprintTransactionTests
    {
        #region Structure Reservation

        [Test]
        public void ValidateReserveStructureFootprint_MissingCells_ReturnsValidWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            manager.GenerateMap(seed: 0, mapSize: 5);
            int chunkCount = manager.ChunkCount;

            MapCellFootprintTransactionResult result = manager.ValidateReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.None));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
        }

        [Test]
        public void TryReserveStructureFootprint_MultiCellFootprint_ReservesAllResolvedCells()
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 0, 1)
            });
            var anchor = Origin();
            var structureId = new StructureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(3));
            AssertStructureReserved(manager, footprint.Resolve(anchor, FootprintRotation.Deg0), structureId);
        }

        [Test]
        public void TryReserveStructureFootprint_RotatedFootprint_UsesRotatedCoordinates()
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 0, 1)
            });
            var anchor = new MapCellCoord(2, 0, 2);
            var structureId = new StructureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg90);

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg90,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            AssertStructureReserved(
                manager,
                new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(2, 0, 1),
                    new MapCellCoord(3, 0, 1)
                },
                structureId);
        }

        [Test]
        public void TryReserveStructureFootprint_SameStructureEverywhere_ReturnsNoOpWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            manager.TryReserveStructureFootprint(anchor, footprint, FootprintRotation.Deg0, structureId);
            int cellCount = manager.CellCount;

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.NoOp));
            Assert.That(manager.CellCount, Is.EqualTo(cellCount));
            AssertStructureReserved(manager, footprint.Resolve(anchor, FootprintRotation.Deg0), structureId);
        }

        [Test]
        public void TryReserveStructureFootprint_PartialSameStructure_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveStructure(manager, coords[0], structureId);

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.InconsistentReservationState));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[0]));
            AssertStructureReserved(manager, new[] { coords[0] }, structureId);
            Assert.That(manager.TryGetCell(coords[1], out _), Is.False);
        }

        [Test]
        public void TryReserveStructureFootprint_FurnitureConflict_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[1], new FurnitureId(7));

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.FurniturePresent));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertFurnitureReserved(manager, new[] { coords[1] }, new FurnitureId(7));
        }

        [Test]
        public void TryReserveStructureFootprint_ItemConflict_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveItem(manager, coords[1], new ItemId(3));

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.ItemsPresent));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertItemReserved(manager, coords[1], new ItemId(3));
        }

        [Test]
        public void TryReserveStructureFootprint_DifferentStructureConflict_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveStructure(manager, coords[1], new StructureId(2));

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertStructureReserved(manager, new[] { coords[1] }, new StructureId(2));
        }

        #endregion

        #region Furniture Reservation

        [Test]
        public void TryReserveFurnitureFootprint_MultiCellFootprint_ReservesAllResolvedCells()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(2));
            AssertFurnitureReserved(manager, footprint.Resolve(anchor, FootprintRotation.Deg0), furnitureId);
        }

        [Test]
        public void TryReserveFurnitureFootprint_ItemsPresent_ReservesFurnitureAndKeepsItems()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var itemId = new ItemId(5);
            var furnitureId = new FurnitureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveItem(manager, coords[0], itemId);

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            AssertFurnitureReserved(manager, coords, furnitureId);
            AssertItemReserved(manager, coords[0], itemId);
            Assert.That(manager.CellCount, Is.EqualTo(2));
        }

        [Test]
        public void TryReserveFurnitureFootprint_StructureConflict_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveStructure(manager, coords[1], new StructureId(4));

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertStructureReserved(manager, new[] { coords[1] }, new StructureId(4));
        }

        [Test]
        public void TryReserveFurnitureFootprint_SameFurnitureEverywhere_ReturnsNoOpWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            manager.TryReserveFurnitureFootprint(anchor, footprint, FootprintRotation.Deg0, furnitureId);
            int cellCount = manager.CellCount;

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.NoOp));
            Assert.That(manager.CellCount, Is.EqualTo(cellCount));
            AssertFurnitureReserved(manager, footprint.Resolve(anchor, FootprintRotation.Deg0), furnitureId);
        }

        [Test]
        public void TryReserveFurnitureFootprint_PartialSameFurniture_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[0], furnitureId);

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.InconsistentReservationState));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[0]));
            AssertFurnitureReserved(manager, new[] { coords[0] }, furnitureId);
            Assert.That(manager.TryGetCell(coords[1], out _), Is.False);
        }

        [Test]
        public void TryReserveFurnitureFootprint_DifferentFurnitureConflict_ReturnsInvalidWithoutPartialMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[1], new FurnitureId(2));

            MapCellFootprintTransactionResult result = manager.TryReserveFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.FurniturePresent));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertFurnitureReserved(manager, new[] { coords[1] }, new FurnitureId(2));
        }

        #endregion

        #region Release

        [Test]
        public void TryReleaseStructureFootprint_FullyReserved_ReleasesAllCellsAndRemovesEmptySparseCells()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryReserveStructureFootprint(anchor, footprint, FootprintRotation.Deg0, structureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            AssertCellsMissing(manager, coords);
            AssertTerrainStillExists(manager, coords);
        }

        [Test]
        public void TryReleaseFurnitureFootprint_FullyReserved_ReleasesAllCellsAndRemovesEmptySparseCells()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryReserveFurnitureFootprint(anchor, footprint, FootprintRotation.Deg0, furnitureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            AssertCellsMissing(manager, coords);
            AssertTerrainStillExists(manager, coords);
        }

        [Test]
        public void TryReleaseStructureFootprint_AbsentEverywhere_ReturnsNoOpWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            int chunkCount = manager.ChunkCount;

            MapCellFootprintTransactionResult result = manager.TryReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.NoOp));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
        }

        [Test]
        public void TryReleaseFurnitureFootprint_AbsentEverywhere_ReturnsNoOpWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            int chunkCount = manager.ChunkCount;

            MapCellFootprintTransactionResult result = manager.TryReleaseFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.NoOp));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
        }

        [Test]
        public void TryReleaseStructureFootprint_WrongOwner_ReturnsInvalidWithoutMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var owner = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryReserveStructureFootprint(anchor, footprint, FootprintRotation.Deg0, owner);

            MapCellFootprintTransactionResult result = manager.TryReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(2));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[0]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.ReservationMismatch));
            AssertStructureReserved(manager, coords, owner);
        }

        [Test]
        public void TryReleaseFurnitureFootprint_WrongOwner_ReturnsInvalidWithoutMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var owner = new FurnitureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryReserveFurnitureFootprint(anchor, footprint, FootprintRotation.Deg0, owner);

            MapCellFootprintTransactionResult result = manager.TryReleaseFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new FurnitureId(2));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[0]));
            Assert.That(result.OccupancyFailureReason, Is.EqualTo(CellOccupancyFailureReason.ReservationMismatch));
            AssertFurnitureReserved(manager, coords, owner);
        }

        [Test]
        public void TryReleaseStructureFootprint_MixedOwnedAndAbsent_ReturnsInvalidWithoutMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveStructure(manager, coords[0], structureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.InconsistentReservationState));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            AssertStructureReserved(manager, new[] { coords[0] }, structureId);
            Assert.That(manager.TryGetCell(coords[1], out _), Is.False);
        }

        [Test]
        public void TryReleaseFurnitureFootprint_MixedOwnedAndAbsent_ReturnsInvalidWithoutMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[0], furnitureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.InconsistentReservationState));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            AssertFurnitureReserved(manager, new[] { coords[0] }, furnitureId);
            Assert.That(manager.TryGetCell(coords[1], out _), Is.False);
        }

        [Test]
        public void TryReleaseStructureFootprint_CellStillHasFloor_RemainsStored()
        {
            var manager = new MapManager();
            var footprint = SingleCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            manager.TryPlace(BuildKind.Floor, new MapCoord(anchor.X, anchor.Z), new Elevation(0));
            ReserveStructure(manager, anchor, structureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(1));
            Assert.That(manager.TryGetCell(anchor, out var cell), Is.True);
            Assert.That(cell!.HasFloor(), Is.True);
            Assert.That(cell.StructureId, Is.Null);
        }

        [Test]
        public void TryReleaseFurnitureFootprint_CellStillHasItems_RemainsStored()
        {
            var manager = new MapManager();
            var footprint = SingleCellFootprint();
            var anchor = Origin();
            var furnitureId = new FurnitureId(1);
            var itemId = new ItemId(8);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);
            ReserveItem(manager, anchor, itemId);
            ReserveFurniture(manager, anchor, furnitureId);

            MapCellFootprintTransactionResult result = manager.TryReleaseFurnitureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(1));
            Assert.That(manager.TryGetCell(anchor, out var cell), Is.True);
            Assert.That(cell!.FurnitureId, Is.Null);
            Assert.That(cell.HasItem(itemId), Is.True);
        }

        [Test]
        public void ValidateReleaseStructureFootprint_FullyReserved_DoesNotMutateOrCleanup()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            var structureId = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            manager.TryReserveStructureFootprint(anchor, footprint, FootprintRotation.Deg0, structureId);
            int cellCount = manager.CellCount;

            MapCellFootprintTransactionResult result = manager.ValidateReleaseStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            Assert.That(manager.CellCount, Is.EqualTo(cellCount));
            AssertStructureReserved(manager, coords, structureId);
        }

        #endregion

        #region World Bounds And Chunks

        [Test]
        public void TryReserveStructureFootprint_OutsideGeneratedTerrain_ReturnsInvalidWithoutAllocating()
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(99, 0, 0)
            });
            var anchor = Origin();
            manager.GenerateMap(seed: 0, mapSize: 5);
            int chunkCount = manager.ChunkCount;

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OutsideGeneratedWorld));
            Assert.That(result.FailedCoord, Is.EqualTo(new MapCellCoord(99, 0, 0)));
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(chunkCount));
            Assert.That(manager.TryGetCell(anchor, out _), Is.False);
        }

        [Test]
        public void TryReserveStructureFootprint_CoordinateOverflow_ReturnsInvalidWithoutAllocation()
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(1, 0, 0)
            });

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                new MapCellCoord(int.MaxValue, 0, 0),
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.CoordinateOverflow));
            Assert.That(result.FailedCoord, Is.Null);
            Assert.That(manager.CellCount, Is.EqualTo(0));
            Assert.That(manager.ChunkCount, Is.EqualTo(0));
        }

        [Test]
        public void TryReserveStructureFootprint_VerticalCellsAboveAndBelowTerrain_AreAllowed()
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, -2, 0),
                new MapCellOffset(0, 2, 0)
            });
            var anchor = Origin();
            var structureId = new StructureId(1);
            StoreTerrainForFootprint(manager, anchor, footprint, FootprintRotation.Deg0);

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            AssertStructureReserved(
                manager,
                new[]
                {
                    new MapCellCoord(0, -2, 0),
                    new MapCellCoord(0, 2, 0)
                },
                structureId);
        }

        [TestCaseSource(nameof(CrossChunkFootprintCases))]
        public void TryReserveStructureFootprint_CrossChunkBoundary_RoutesAllCells(
            MapCellCoord anchor,
            MapCellOffset[] offsets,
            MapChunkCoord[] expectedChunkCoords)
        {
            var manager = new MapManager();
            var footprint = new MapCellFootprint(offsets);
            var structureId = new StructureId(1);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                structureId);

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Valid));
            AssertStructureReserved(manager, coords, structureId);
            Assert.That(coords.Select(manager.ResolveChunkCoord).ToArray(), Is.EqualTo(expectedChunkCoords));
        }

        [Test]
        public void TryReserveStructureFootprint_CrossChunkConflict_IsAtomicAcrossChunks()
        {
            int size = MapChunkMetrics.ChunkSize;
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = new MapCellCoord(size - 1, 0, 0);
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[1], new FurnitureId(6));

            MapCellFootprintTransactionResult result = manager.TryReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(MapCellFootprintTransactionOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFootprintTransactionFailureReason.OccupancyConflict));
            Assert.That(result.FailedCoord, Is.EqualTo(coords[1]));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertFurnitureReserved(manager, new[] { coords[1] }, new FurnitureId(6));
        }

        [Test]
        public void ValidateReserveStructureFootprint_RepeatedFailures_ReturnSameFailedCoordinateWithoutMutation()
        {
            var manager = new MapManager();
            var footprint = TwoCellFootprint();
            var anchor = Origin();
            IReadOnlyList<MapCellCoord> coords = footprint.Resolve(anchor, FootprintRotation.Deg0);
            StoreTerrainForCoords(manager, coords);
            ReserveFurniture(manager, coords[1], new FurnitureId(6));
            int cellCount = manager.CellCount;

            MapCellFootprintTransactionResult first = manager.ValidateReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));
            MapCellFootprintTransactionResult second = manager.ValidateReserveStructureFootprint(
                anchor,
                footprint,
                FootprintRotation.Deg0,
                new StructureId(1));

            Assert.That(second.Outcome, Is.EqualTo(first.Outcome));
            Assert.That(second.FailureReason, Is.EqualTo(first.FailureReason));
            Assert.That(second.FailedCoord, Is.EqualTo(first.FailedCoord));
            Assert.That(manager.CellCount, Is.EqualTo(cellCount));
            Assert.That(manager.TryGetCell(coords[0], out _), Is.False);
            AssertFurnitureReserved(manager, new[] { coords[1] }, new FurnitureId(6));
        }

        #endregion

        #region Helpers

        private static IEnumerable<TestCaseData> CrossChunkFootprintCases()
        {
            int size = MapChunkMetrics.ChunkSize;

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) },
                new[] { new MapChunkCoord(0, 0), new MapChunkCoord(1, 0) })
                .SetName("PositiveXBoundary");

            yield return new TestCaseData(
                new MapCellCoord(0, 0, size - 1),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(0, 0, 1) },
                new[] { new MapChunkCoord(0, 0), new MapChunkCoord(0, 1) })
                .SetName("PositiveZBoundary");

            yield return new TestCaseData(
                new MapCellCoord(size - 1, 0, size - 1),
                new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1)
                },
                new[]
                {
                    new MapChunkCoord(0, 0),
                    new MapChunkCoord(0, 1),
                    new MapChunkCoord(1, 0),
                    new MapChunkCoord(1, 1)
                })
                .SetName("PositiveCornerBoundary");

            yield return new TestCaseData(
                new MapCellCoord(-1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) },
                new[] { new MapChunkCoord(-1, 0), new MapChunkCoord(0, 0) })
                .SetName("NegativeToOriginXBoundary");

            yield return new TestCaseData(
                new MapCellCoord(-size - 1, 0, 0),
                new[] { new MapCellOffset(0, 0, 0), new MapCellOffset(1, 0, 0) },
                new[] { new MapChunkCoord(-2, 0), new MapChunkCoord(-1, 0) })
                .SetName("NegativeXBoundary");
        }

        private static MapCellCoord Origin()
        {
            return new MapCellCoord(0, 0, 0);
        }

        private static MapCellFootprint SingleCellFootprint()
        {
            return new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            });
        }

        private static MapCellFootprint TwoCellFootprint()
        {
            return new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0)
            });
        }

        private static void StoreTerrainForFootprint(
            MapManager manager,
            MapCellCoord anchor,
            MapCellFootprint footprint,
            FootprintRotation rotation)
        {
            StoreTerrainForCoords(manager, footprint.Resolve(anchor, rotation));
        }

        private static void StoreTerrainForCoords(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                var terrainCoord = new TerrainTileWorldCoord(coord.X, coord.Z);

                if (!manager.TryGetTerrain(terrainCoord, out _))
                {
                    manager.StoreGeneratedTerrain(terrainCoord, FlatTile());
                }
            }
        }

        private static void ReserveStructure(MapManager manager, MapCellCoord coord, StructureId structureId)
        {
            Cell cell = manager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveStructure(structureId);
            cell.ReserveStructure(validation, structureId);
        }

        private static void ReserveFurniture(MapManager manager, MapCellCoord coord, FurnitureId furnitureId)
        {
            Cell cell = manager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveFurniture(furnitureId);
            cell.ReserveFurniture(validation, furnitureId);
        }

        private static void ReserveItem(MapManager manager, MapCellCoord coord, ItemId itemId)
        {
            Cell cell = manager.GetOrCreateCell(coord);
            CellOccupancyValidationResult validation = cell.ValidateReserveItem(itemId);
            cell.ReserveItem(validation, itemId);
        }

        private static TerrainTile FlatTile()
        {
            return new TerrainTile(0f, 0f, 0f, 0f);
        }

        private static void AssertStructureReserved(
            MapManager manager,
            IEnumerable<MapCellCoord> coords,
            StructureId structureId)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
                Assert.That(cell!.StructureId, Is.EqualTo(structureId), coord.ToString());
            }
        }

        private static void AssertFurnitureReserved(
            MapManager manager,
            IEnumerable<MapCellCoord> coords,
            FurnitureId furnitureId)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
                Assert.That(cell!.FurnitureId, Is.EqualTo(furnitureId), coord.ToString());
            }
        }

        private static void AssertItemReserved(MapManager manager, MapCellCoord coord, ItemId itemId)
        {
            Assert.That(manager.TryGetCell(coord, out var cell), Is.True, coord.ToString());
            Assert.That(cell!.HasItem(itemId), Is.True, coord.ToString());
        }

        private static void AssertCellsMissing(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetCell(coord, out _), Is.False, coord.ToString());
            }
        }

        private static void AssertTerrainStillExists(MapManager manager, IEnumerable<MapCellCoord> coords)
        {
            foreach (MapCellCoord coord in coords)
            {
                Assert.That(manager.TryGetTerrain(new TerrainTileWorldCoord(coord.X, coord.Z), out _), Is.True, coord.ToString());
            }
        }

        #endregion
    }
}
