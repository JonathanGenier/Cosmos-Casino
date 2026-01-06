using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Furniture;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Structure;
using NUnit.Framework;

namespace CosmosCasino.Tests.Map
{
    [TestFixture]
    internal class MapCellTests
    {
        #region FIELDS

        private MapCell? _mapCell;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapCell = new();
        }

        #endregion

        #region MapCell

        [Test]
        public void MapCell_ShouldBeEmptyOnCreation()
        {
            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.True);
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Assert
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenFloorDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.HasFloor, Is.False);
        }

        #endregion

        #region HasStructure

        [Test]
        public void HasStructure_ShouldReturnTrue_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Assert
            Assert.That(_mapCell!.HasStructure, Is.True);
        }

        [Test]
        public void HasStructure_ShouldReturnFalse_WhenStructureDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        #endregion

        #region HasFurniture

        [Test]
        public void HasFurniture_ShouldReturnTrue_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Assert
            Assert.That(_mapCell!.HasFurniture, Is.True);
        }

        [Test]
        public void HasFurniture_ShouldReturnFalse_WhenFurnitureDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        #endregion

        #region IsEmpty

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_ShouldReturnTrue_WhenFloorAndStructureAndFurnitureDoesNotExists()
        {
            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.True);
        }

        #endregion

        #region TryPlaceFloor

        [Test]
        public void TryPlaceFloor_ShouldPlaceFloor_WhenFloorDoeNotExist()
        {
            // Act
            var result = _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Placed));
            Assert.That(_mapCell!.HasFloor, Is.True);
            Assert.That(_mapCell.Floor, Is.EqualTo(FloorType.Metal));
        }

        [Test]
        public void TryPlaceFloor_ShouldFail_WhenFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Carbon);

            // Act
            var result = _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell!.HasFloor, Is.True);
            Assert.That(_mapCell!.Floor, Is.EqualTo(FloorType.Carbon));
        }

        #endregion

        #region TryReplaceFloor

        [Test]
        public void TryReplaceFloor_ShouldReplaceFloor_WhenDifferentFloorType()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Carbon);

            // Act
            var result = _mapCell!.TryReplaceFloor(FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Replaced));
            Assert.That(_mapCell.Floor, Is.EqualTo(FloorType.Metal));
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void TryReplaceFloor_ShouldSkip_WhenFloorTypeIsSame()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryReplaceFloor(FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.SameType));
            Assert.That(_mapCell.Floor, Is.EqualTo(FloorType.Metal));
            Assert.That(_mapCell.HasFloor, Is.True);
        }

#if DEBUG
        [Test]
        public void TryReplaceFloor_ShouldThrow_WhenFloorDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _mapCell!.TryReplaceFloor(FloorType.Metal));
        }
#else
        [Test]
        public void TryReplaceFloor_ShouldFail_WhenFloorDoesNotExist()
        {
            // Act
            var result = _mapCell!.TryReplaceFloor(FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
            Assert.That(_mapCell.Floor, Is.Null);
            Assert.That(_mapCell.HasFloor, Is.False);
        }
#endif

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldRemoveFloor_WhenStructureOrFurnitureDoesNotExist()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Removed));
            Assert.That(_mapCell.HasFloor, Is.False);
            Assert.That(_mapCell.Floor, Is.Null);
        }

        [Test]
        public void TryRemoveFloor_ShouldFail_WhenStructureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);
            _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasFloor, Is.True);
        }

        [Test]
        public void TryRemoveFloor_ShouldFail_WhenFurnitureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);
            _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasFloor, Is.True);
        }

#if DEBUG
        [Test]
        public void TryRemoveFloor_ShouldThrow_WhenFloorDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _mapCell!.TryRemoveFloor());
        }
#else
        [Test]
        public void TryRemoveFloor_ShouldFail_WhenFloorDoesNotExist()
        {
            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
            Assert.That(_mapCell.HasFloor, Is.False);
        }
#endif

        #endregion

        #region TryPlaceStructure

        [Test]
        public void TryPlaceStructure_ShouldPlaceStructure_WhenOnlyFloorExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Placed));
            Assert.That(_mapCell.HasStructure, Is.True);
            Assert.That(_mapCell.Structure, Is.EqualTo(StructureType.Wall));
        }

        [Test]
        public void TryPlaceStructure_ShouldFail_WhenFloorDoesNotExist()
        {
            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoFloor));
            Assert.That(_mapCell.HasStructure, Is.False);
        }

        [Test]
        public void TryPlaceStructure_ShouldFail_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasStructure, Is.False);
        }

#if DEBUG
        [Test]
        public void TryPlaceStructure_ShouldThrow_WhenStructureAlreadyExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _mapCell!.TryPlaceStructure(StructureType.Door)
            );
        }
#else
        [Test]
        public void TryPlaceStructure_ShouldFail_WhenStructureAlreadyExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Door);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
            Assert.That(_mapCell.Structure, Is.EqualTo(StructureType.Wall));
        }
#endif

        #endregion

        #region TryReplaceStructure

        [Test]
        public void TryReplaceStructure_ShouldReplaceStructure_WhenDifferentType()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Door);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Replaced));
            Assert.That(_mapCell.Structure, Is.EqualTo(StructureType.Door));
            Assert.That(_mapCell.HasStructure, Is.True);
        }

        [Test]
        public void TryReplaceStructure_ShouldSkip_WhenStructureTypeIsSame()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.SameType));
            Assert.That(_mapCell.Structure, Is.EqualTo(StructureType.Wall));
        }

#if DEBUG
        [Test]
        public void TryReplaceStructure_ShouldThrow_WhenNoFloorExists()
        {
            // Act & assert
            Assert.Throws<InvalidOperationException>(
                () => _mapCell!.TryReplaceStructure(StructureType.Wall)
            );
        }

        [Test]
        public void TryReplaceStructure_ShouldThrow_WhenNoStructureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act & assert
            Assert.Throws<InvalidOperationException>(
                () => _mapCell!.TryReplaceStructure(StructureType.Wall)
            );
        }

        [Test]
        public void TryReplaceStructure_ShouldThrow_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act & assert
            Assert.Throws<InvalidOperationException>(
                () => _mapCell!.TryReplaceStructure(StructureType.Wall)
            );
        }
#else
        [Test]
        public void TryReplaceStructure_ShouldFail_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
        }

        [Test]
        public void TryReplaceStructure_ShouldFail_WhenNoStructureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
        }

        [Test]
        public void TryReplaceStructure_ShouldFail_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.InternalError));
        }
#endif

        #endregion

        #region TryRemoveStructure

        [Test]
        public void TryRemoveStructure_ShouldRemoveStructure_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryRemoveStructure();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Removed));
            Assert.That(_mapCell.HasStructure, Is.False);
            Assert.That(_mapCell.Structure, Is.Null);
        }

        [Test]
        public void TryRemoveStructure_ShouldSkip_WhenNoStructureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveStructure();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoStructure));
            Assert.That(_mapCell.HasStructure, Is.False);
        }

        #endregion

        #region TryPlaceFurniture

        [Test]
        public void TryPlaceFurniture_ShouldPlaceFurniture_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Placed));
            Assert.That(_mapCell.HasFurniture, Is.True);
            Assert.That(_mapCell.Furniture, Is.EqualTo(FurnitureType.SlotMachine));
        }

        [Test]
        public void TryPlaceFurniture_ShouldFail_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoFloor));
            Assert.That(_mapCell.HasFurniture, Is.False);
        }

        [Test]
        public void TryPlaceFurniture_ShouldFail_WhenFurnitureAlreadyExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.Furniture, Is.EqualTo(FurnitureType.SlotMachine));
        }

        [Test]
        public void TryPlaceFurniture_ShouldFail_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.Blocked));
            Assert.That(_mapCell.HasFurniture, Is.False);
        }

        #endregion

        #region TryRemoveFurniture

        [Test]
        public void TryRemoveFurniture_ShouldRemoveFurniture_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryRemoveFurniture();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Removed));
            Assert.That(_mapCell.HasFurniture, Is.False);
            Assert.That(_mapCell.Furniture, Is.Null);
        }

        [Test]
        public void TryRemoveFurniture_ShouldSkip_WhenNoFurnitureExists()
        {
            // Arrange
            _mapCell!.TryPlaceFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveFurniture();

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Skipped));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoFurniture));
            Assert.That(_mapCell.HasFurniture, Is.False);
        }

        #endregion

        #region HELPERS

        private void PlaceStructureWithFloor(StructureType structure)
        {
            _mapCell!.TryPlaceFloor(FloorType.Metal);
            _mapCell!.TryPlaceStructure(structure);
        }

        private void PlaceFurnitureWithFloor(FurnitureType furniture)
        {
            _mapCell!.TryPlaceFloor(FloorType.Metal);
            _mapCell!.TryPlaceFurniture(furniture);
        }

        #endregion
    }
}