using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class CellOccupancyTests
    {
        #region Initial State

        [Test]
        public void Constructor_InitializesEmptyOccupancy()
        {
            var cell = CellAtOrigin();

            Assert.That(cell.StructureId, Is.Null);
            Assert.That(cell.FurnitureId, Is.Null);
            Assert.That(cell.ItemIds, Is.Empty);
            Assert.That(cell.HasStructure, Is.False);
            Assert.That(cell.HasFurniture, Is.False);
            Assert.That(cell.HasItems, Is.False);
            Assert.That(cell.HasOccupancy, Is.False);
            Assert.That(cell.IsEmpty, Is.True);
        }

        #endregion

        #region Structure

        [Test]
        public void ValidateReserveStructure_EmptyCell_ReturnsValidWithoutMutation()
        {
            var cell = CellAtOrigin();
            var structureId = new StructureId(1);

            var first = cell.ValidateReserveStructure(structureId);
            var second = cell.ValidateReserveStructure(structureId);

            Assert.That(first.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(second.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.StructureId, Is.Null);
            Assert.That(cell.IsEmpty, Is.True);
        }

        [Test]
        public void ReserveStructure_ValidResult_StoresStructureReference()
        {
            var cell = CellAtOrigin();
            var structureId = new StructureId(1);

            ReserveStructure(cell, structureId);

            Assert.That(cell.StructureId, Is.EqualTo(structureId));
            Assert.That(cell.HasStructure, Is.True);
            Assert.That(cell.HasOccupancy, Is.True);
            Assert.That(cell.IsEmpty, Is.False);
        }

        [Test]
        public void ValidateReserveStructure_SameStructure_ReturnsNoOp()
        {
            var cell = CellAtOrigin();
            var structureId = new StructureId(1);
            ReserveStructure(cell, structureId);

            var result = cell.ValidateReserveStructure(structureId);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
        }

        [Test]
        public void ValidateReserveStructure_DifferentStructure_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveStructure(cell, new StructureId(1));

            var result = cell.ValidateReserveStructure(new StructureId(2));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
            Assert.That(cell.StructureId, Is.EqualTo(new StructureId(1)));
        }

        [Test]
        public void ValidateReserveStructure_WhenFurnitureExists_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveFurniture(cell, new FurnitureId(1));

            var result = cell.ValidateReserveStructure(new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.FurniturePresent));
        }

        [Test]
        public void ValidateReserveStructure_WhenItemsExist_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveItem(cell, new ItemId(1));

            var result = cell.ValidateReserveStructure(new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.ItemsPresent));
        }

        [Test]
        public void ReleaseStructure_ValidResult_ClearsStructureReference()
        {
            var cell = CellAtOrigin();
            var structureId = new StructureId(1);
            ReserveStructure(cell, structureId);

            var validation = cell.ValidateReleaseStructure(structureId);
            cell.ReleaseStructure(validation, structureId);

            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.StructureId, Is.Null);
            Assert.That(cell.HasOccupancy, Is.False);
            Assert.That(cell.IsEmpty, Is.True);
        }

        [Test]
        public void ValidateReleaseStructure_MissingStructure_ReturnsNoOp()
        {
            var cell = CellAtOrigin();

            var result = cell.ValidateReleaseStructure(new StructureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
            Assert.That(cell.StructureId, Is.Null);
        }

        [Test]
        public void ValidateReleaseStructure_MatchingStructure_ReturnsValid()
        {
            var cell = CellAtOrigin();
            var structureId = new StructureId(1);
            ReserveStructure(cell, structureId);

            var result = cell.ValidateReleaseStructure(structureId);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
            Assert.That(cell.StructureId, Is.EqualTo(structureId));
        }

        [Test]
        public void ValidateReleaseStructure_DifferentStructure_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            var owner = new StructureId(1);
            var other = new StructureId(2);
            ReserveStructure(cell, owner);

            var result = cell.ValidateReleaseStructure(other);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.ReservationMismatch));
            Assert.Throws<InvalidOperationException>(() => cell.ReleaseStructure(result, other));
            Assert.That(cell.StructureId, Is.EqualTo(owner));
        }

        #endregion

        #region Furniture

        [Test]
        public void ValidateReserveFurniture_EmptyCell_ReturnsValidWithoutMutation()
        {
            var cell = CellAtOrigin();
            var furnitureId = new FurnitureId(1);

            var first = cell.ValidateReserveFurniture(furnitureId);
            var second = cell.ValidateReserveFurniture(furnitureId);

            Assert.That(first.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(second.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.FurnitureId, Is.Null);
            Assert.That(cell.IsEmpty, Is.True);
        }

        [Test]
        public void ReserveFurniture_ValidResult_StoresFurnitureReference()
        {
            var cell = CellAtOrigin();
            var furnitureId = new FurnitureId(1);

            ReserveFurniture(cell, furnitureId);

            Assert.That(cell.FurnitureId, Is.EqualTo(furnitureId));
            Assert.That(cell.HasFurniture, Is.True);
            Assert.That(cell.HasOccupancy, Is.True);
            Assert.That(cell.IsEmpty, Is.False);
        }

        [Test]
        public void ValidateReserveFurniture_SameFurniture_ReturnsNoOp()
        {
            var cell = CellAtOrigin();
            var furnitureId = new FurnitureId(1);
            ReserveFurniture(cell, furnitureId);

            var result = cell.ValidateReserveFurniture(furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
        }

        [Test]
        public void ValidateReserveFurniture_DifferentFurniture_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveFurniture(cell, new FurnitureId(1));

            var result = cell.ValidateReserveFurniture(new FurnitureId(2));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.FurniturePresent));
            Assert.That(cell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
        }

        [Test]
        public void ValidateReserveFurniture_WhenStructureExists_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveStructure(cell, new StructureId(1));

            var result = cell.ValidateReserveFurniture(new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
        }

        [Test]
        public void ValidateReserveFurniture_WhenItemsExist_ReturnsValid()
        {
            var cell = CellAtOrigin();
            ReserveItem(cell, new ItemId(1));

            var result = cell.ValidateReserveFurniture(new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.HasItem(new ItemId(1)), Is.True);
            Assert.That(cell.FurnitureId, Is.Null);
        }

        [Test]
        public void ReserveFurniture_WhenItemsExist_CoexistsWithItems()
        {
            var cell = CellAtOrigin();
            ReserveItem(cell, new ItemId(1));

            ReserveFurniture(cell, new FurnitureId(1));

            Assert.That(cell.HasItem(new ItemId(1)), Is.True);
            Assert.That(cell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
            Assert.That(cell.HasOccupancy, Is.True);
        }

        [Test]
        public void ReleaseFurniture_ValidResult_ClearsFurnitureReferenceAndLeavesItems()
        {
            var cell = CellAtOrigin();
            var furnitureId = new FurnitureId(1);
            var itemId = new ItemId(1);
            ReserveItem(cell, itemId);
            ReserveFurniture(cell, furnitureId);

            var validation = cell.ValidateReleaseFurniture(furnitureId);
            cell.ReleaseFurniture(validation, furnitureId);

            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.FurnitureId, Is.Null);
            Assert.That(cell.HasItem(itemId), Is.True);
            Assert.That(cell.IsEmpty, Is.False);
        }

        [Test]
        public void ValidateReleaseFurniture_MissingFurniture_ReturnsNoOp()
        {
            var cell = CellAtOrigin();

            var result = cell.ValidateReleaseFurniture(new FurnitureId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
            Assert.That(cell.FurnitureId, Is.Null);
        }

        [Test]
        public void ValidateReleaseFurniture_MatchingFurniture_ReturnsValid()
        {
            var cell = CellAtOrigin();
            var furnitureId = new FurnitureId(1);
            ReserveFurniture(cell, furnitureId);

            var result = cell.ValidateReleaseFurniture(furnitureId);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
            Assert.That(cell.FurnitureId, Is.EqualTo(furnitureId));
        }

        [Test]
        public void ValidateReleaseFurniture_DifferentFurniture_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            var owner = new FurnitureId(1);
            var other = new FurnitureId(2);
            ReserveFurniture(cell, owner);

            var result = cell.ValidateReleaseFurniture(other);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.ReservationMismatch));
            Assert.Throws<InvalidOperationException>(() => cell.ReleaseFurniture(result, other));
            Assert.That(cell.FurnitureId, Is.EqualTo(owner));
        }

        #endregion

        #region Items

        [Test]
        public void ValidateReserveItem_EmptyCell_ReturnsValidWithoutMutation()
        {
            var cell = CellAtOrigin();
            var itemId = new ItemId(1);

            var first = cell.ValidateReserveItem(itemId);
            var second = cell.ValidateReserveItem(itemId);

            Assert.That(first.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(second.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.ItemIds, Is.Empty);
            Assert.That(cell.IsEmpty, Is.True);
        }

        [Test]
        public void ReserveItem_ValidResult_StoresItemReference()
        {
            var cell = CellAtOrigin();
            var itemId = new ItemId(1);

            ReserveItem(cell, itemId);

            Assert.That(cell.HasItem(itemId), Is.True);
            Assert.That(cell.ItemIds, Is.EquivalentTo(new[] { itemId }));
            Assert.That(cell.HasItems, Is.True);
            Assert.That(cell.HasOccupancy, Is.True);
            Assert.That(cell.IsEmpty, Is.False);
        }

        [Test]
        public void ReserveItem_MultipleDifferentItems_StoresEachItemReference()
        {
            var cell = CellAtOrigin();
            var first = new ItemId(1);
            var second = new ItemId(2);

            ReserveItem(cell, first);
            ReserveItem(cell, second);

            Assert.That(cell.ItemIds, Is.EquivalentTo(new[] { first, second }));
        }

        [Test]
        public void ValidateReserveItem_SameItem_ReturnsNoOpWithoutDuplicating()
        {
            var cell = CellAtOrigin();
            var itemId = new ItemId(1);
            ReserveItem(cell, itemId);

            var result = cell.ValidateReserveItem(itemId);

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.NoOp));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.None));
            Assert.That(cell.ItemIds, Is.EquivalentTo(new[] { itemId }));
        }

        [Test]
        public void ValidateReserveItem_WhenFurnitureExists_ReturnsValid()
        {
            var cell = CellAtOrigin();
            ReserveFurniture(cell, new FurnitureId(1));

            var result = cell.ValidateReserveItem(new ItemId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
            Assert.That(cell.ItemIds, Is.Empty);
        }

        [Test]
        public void ReserveItem_WhenFurnitureExists_CoexistsWithFurniture()
        {
            var cell = CellAtOrigin();
            var itemId = new ItemId(1);
            ReserveFurniture(cell, new FurnitureId(1));

            ReserveItem(cell, itemId);

            Assert.That(cell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
            Assert.That(cell.HasItem(itemId), Is.True);
        }

        [Test]
        public void ValidateReserveItem_WhenStructureExists_ReturnsInvalid()
        {
            var cell = CellAtOrigin();
            ReserveStructure(cell, new StructureId(1));

            var result = cell.ValidateReserveItem(new ItemId(1));

            Assert.That(result.Outcome, Is.EqualTo(CellOccupancyOutcome.Invalid));
            Assert.That(result.FailureReason, Is.EqualTo(CellOccupancyFailureReason.StructurePresent));
        }

        [Test]
        public void ReleaseItem_ValidResult_RemovesOnlyRequestedItem()
        {
            var cell = CellAtOrigin();
            var first = new ItemId(1);
            var second = new ItemId(2);
            ReserveItem(cell, first);
            ReserveItem(cell, second);

            var validation = cell.ValidateReleaseItem(first);
            cell.ReleaseItem(validation, first);

            Assert.That(validation.Outcome, Is.EqualTo(CellOccupancyOutcome.Valid));
            Assert.That(cell.HasItem(first), Is.False);
            Assert.That(cell.HasItem(second), Is.True);
            Assert.That(cell.IsEmpty, Is.False);
        }

        [Test]
        public void ReleaseItem_LastItem_EmptiesOccupancy()
        {
            var cell = CellAtOrigin();
            var itemId = new ItemId(1);
            ReserveItem(cell, itemId);

            var validation = cell.ValidateReleaseItem(itemId);
            cell.ReleaseItem(validation, itemId);

            Assert.That(cell.HasItems, Is.False);
            Assert.That(cell.HasOccupancy, Is.False);
            Assert.That(cell.IsEmpty, Is.True);
        }

        #endregion

        #region Operation Guards

        [Test]
        public void ReserveOccupancy_NonValidResult_ThrowsWithoutMutation()
        {
            var cell = CellAtOrigin();
            ReserveStructure(cell, new StructureId(1));
            var invalidFurniture = cell.ValidateReserveFurniture(new FurnitureId(1));
            var invalidItem = cell.ValidateReserveItem(new ItemId(1));
            var noOpStructure = cell.ValidateReserveStructure(new StructureId(1));

            Assert.Throws<InvalidOperationException>(() =>
                cell.ReserveFurniture(invalidFurniture, new FurnitureId(1)));
            Assert.Throws<InvalidOperationException>(() =>
                cell.ReserveItem(invalidItem, new ItemId(1)));
            Assert.Throws<InvalidOperationException>(() =>
                cell.ReserveStructure(noOpStructure, new StructureId(1)));
            Assert.That(cell.StructureId, Is.EqualTo(new StructureId(1)));
            Assert.That(cell.FurnitureId, Is.Null);
            Assert.That(cell.ItemIds, Is.Empty);
        }

        [Test]
        public void ReserveOccupancy_StaleValidResult_ThrowsWithoutBypassingConflicts()
        {
            var structureCell = CellAtOrigin();
            var furnitureCell = CellAtOrigin();
            var validResult = CellOccupancyValidationResult.Valid();
            ReserveStructure(structureCell, new StructureId(1));
            ReserveFurniture(furnitureCell, new FurnitureId(1));

            Assert.Throws<InvalidOperationException>(() =>
                structureCell.ReserveFurniture(validResult, new FurnitureId(2)));
            Assert.Throws<InvalidOperationException>(() =>
                structureCell.ReserveItem(validResult, new ItemId(1)));
            Assert.Throws<InvalidOperationException>(() =>
                furnitureCell.ReserveStructure(validResult, new StructureId(2)));
            Assert.That(structureCell.StructureId, Is.EqualTo(new StructureId(1)));
            Assert.That(structureCell.FurnitureId, Is.Null);
            Assert.That(structureCell.ItemIds, Is.Empty);
            Assert.That(furnitureCell.StructureId, Is.Null);
            Assert.That(furnitureCell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
        }

        [Test]
        public void ReleaseOccupancy_NonValidResult_ThrowsWithoutMutation()
        {
            var cell = CellAtOrigin();
            ReserveFurniture(cell, new FurnitureId(1));
            var invalidFurniture = cell.ValidateReleaseFurniture(new FurnitureId(2));
            var noOpItem = cell.ValidateReleaseItem(new ItemId(1));

            Assert.Throws<InvalidOperationException>(() =>
                cell.ReleaseFurniture(invalidFurniture, new FurnitureId(2)));
            Assert.Throws<InvalidOperationException>(() =>
                cell.ReleaseItem(noOpItem, new ItemId(1)));
            Assert.That(cell.FurnitureId, Is.EqualTo(new FurnitureId(1)));
            Assert.That(cell.ItemIds, Is.Empty);
        }

        [Test]
        public void ReleaseOccupancy_StaleValidResult_ThrowsWithoutClearingDifferentReservation()
        {
            var cell = CellAtOrigin();
            var validResult = CellOccupancyValidationResult.Valid();
            ReserveStructure(cell, new StructureId(1));

            Assert.Throws<InvalidOperationException>(() =>
                cell.ReleaseStructure(validResult, new StructureId(2)));

            Assert.That(cell.StructureId, Is.EqualTo(new StructureId(1)));
        }

        #endregion

        #region Helpers

        private static Cell CellAtOrigin()
        {
            return new Cell(new MapCellCoord(0, 0, 0));
        }

        private static void ReserveStructure(Cell cell, StructureId structureId)
        {
            var validation = cell.ValidateReserveStructure(structureId);
            cell.ReserveStructure(validation, structureId);
        }

        private static void ReserveFurniture(Cell cell, FurnitureId furnitureId)
        {
            var validation = cell.ValidateReserveFurniture(furnitureId);
            cell.ReserveFurniture(validation, furnitureId);
        }

        private static void ReserveItem(Cell cell, ItemId itemId)
        {
            var validation = cell.ValidateReserveItem(itemId);
            cell.ReserveItem(validation, itemId);
        }

        #endregion
    }
}
