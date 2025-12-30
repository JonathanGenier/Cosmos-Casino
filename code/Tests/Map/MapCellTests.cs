using CosmosCasino.Core.Map;
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
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Assert
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenNoFloorExists()
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
        public void HasStructure_ShouldReturnFalse_WhenNoStructureExists()
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
        public void HasFurniture_ShouldReturnFalse_WhenNoFurnitureExists()
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
            _mapCell!.TrySetFloor(FloorType.Metal);

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
        public void IsEmpty_ShouldReturnTrue_WhenNoFloorAndStructureAndFurnitureExists()
        {
            // Assert
            Assert.That(_mapCell!.IsEmpty, Is.True);
        }

        #endregion

        #region CanRemoveFloor

        [Test]
        public void CanRemoveFloor_ShouldReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanRemoveFloor();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanRemoveFloor_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.CanRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanRemoveFloor_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.CanRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanRemoveFloor_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.CanRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region TrySetFloor

        [Test]
        public void TrySetFloor_ShouldSetFloorAndReturnTrue_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TrySetFloor(FloorType.Metal);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void TrySetFloor_ShouldSetFloorAndReturnTrue_WhenDifferentFloorExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Wood);

            // Act
            var result = _mapCell!.TrySetFloor(FloorType.Metal);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasFloor, Is.True);
            Assert.That(_mapCell!.Floor, Is.EqualTo(FloorType.Metal));
        }

        [Test]
        public void TrySetFloor_ShouldReturnFalse_WhenFloorTypeAlreadyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TrySetFloor(FloorType.Metal);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldSetFloorToNullAndReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasFloor, Is.False);
        }

        [Test]
        public void TryRemoveFloor_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRemoveFloor_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        [Test]
        public void TryRemoveFloor_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryRemoveFloor();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFloor, Is.True);
        }

        #endregion

        #region CanPlaceStructure

        [Test]
        public void CanPlaceStructure_ShouldReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanPlaceStructure();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanPlaceStructure_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.CanPlaceStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanPlaceStructure_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.CanPlaceStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanPlaceStructure_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.CanPlaceFurniture();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CanReplaceStructure

        [Test]
        public void CanReplaceStructure_ShouldReturnTrue_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.CanReplaceStructure();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanReplaceStructure_ShouldReturnFalse_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanReplaceStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanReplaceStructure_ShouldReturnFalse_WhenFloorAndFurnitureOnlyExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.CanReplaceStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanReplaceStructure_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.CanReplaceStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CanRemoveStructure

        [Test]
        public void CanRemoveStructure_ShouldReturnTrue_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.CanRemoveStructure();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanRemoveStructure_ShouldReturnFalse_WhenNoStructureExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanRemoveStructure();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region TryPlaceStructure

        [Test]
        public void TryPlaceStructure_ShouldPlaceStructureAndReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasStructure, Is.True);
        }

        [Test]
        public void TryPlaceStructure_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Door);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.True);
            Assert.That(_mapCell!.Structure, Is.EqualTo(StructureType.Door));
        }

        [Test]
        public void TryPlaceStructure_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryPlaceStructure_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryPlaceStructure_ShouldNotAffectFurniture_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            _mapCell!.TryPlaceStructure(StructureType.Wall);

            // Assert
            Assert.That(_mapCell!.HasFurniture, Is.True);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        #endregion

        #region TryReplaceStructure

        [Test]
        public void TryReplaceStructure_ShouldSetStructureAndReturnTrue_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Door);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.Structure, Is.EqualTo(StructureType.Wall));
        }

        [Test]
        public void TryReplaceStructure_ShouldReturnFalse_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryReplaceStructure_ShouldReturnFalse_WhenFloorAndFurnitureOnlyExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryReplaceStructure_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryReplaceStructure_ShouldReturnFalse_WhenReplacingWithSameType()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.Structure, Is.EqualTo(StructureType.Wall));
        }

        [Test]
        public void TryReplaceStructure_ShouldNotAffectFurniture_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            _mapCell!.TryReplaceStructure(StructureType.Wall);

            // Assert
            Assert.That(_mapCell!.HasFurniture, Is.True);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        #endregion

        #region TryRemoveStructure

        [Test]
        public void TryRemoveStructure_ShouldSetStructureAndReturnTrue_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryRemoveStructure();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        [Test]
        public void TryRemoveStructure_ShouldReturnFalse_WhenNoStructureExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveStructure();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasStructure, Is.False);
        }

        #endregion

        #region CanPlaceFurniture

        [Test]
        public void CanPlaceFurniture_ShouldReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanPlaceFurniture();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanPlaceFurniture_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.CanPlaceFurniture();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanPlaceFurniture_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.CanPlaceFurniture();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanPlaceFurniture_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.CanPlaceFurniture();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region CanRemoveFurniture

        [Test]
        public void CanRemoveFurniture_ShouldReturnTrue_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.CanRemoveFurniture();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanRemoveFurniture_ShouldReturnFalse_WhenNoFurnitureExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.CanRemoveFurniture();

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region TryPlaceFurniture

        [Test]
        public void TryPlaceFurniture_ShouldSetFurnitureAndReturnTrue_WhenFloorOnlyExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasFurniture, Is.True);
        }

        [Test]
        public void TryPlaceFurniture_ShouldReturnFalse_WhenFloorAndStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        [Test]
        public void TryPlaceFurniture_ShouldReturnFalse_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFurniture, Is.True);
        }

        [Test]
        public void TryPlaceFurniture_ShouldReturnFalse_WhenNoFloorExists()
        {
            // Act
            var result = _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        [Test]
        public void TryPlaceFurniture_ShouldNotAffectStructure_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(StructureType.Wall);

            // Act
            _mapCell!.TryPlaceFurniture(FurnitureType.SlotMachine);

            // Assert
            Assert.That(_mapCell!.HasStructure, Is.True);
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        #endregion

        #region TryRemoveFurniture

        [Test]
        public void TryRemoveFurniture_ShouldSetFurnitureAndReturnTrue_WhenFloorAndFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(FurnitureType.SlotMachine);

            // Act
            var result = _mapCell!.TryRemoveFurniture();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        [Test]
        public void TryRemoveFurniture_ShouldReturnFalse_WhenNoFurnitureExists()
        {
            // Arrange
            _mapCell!.TrySetFloor(FloorType.Metal);

            // Act
            var result = _mapCell!.TryRemoveFurniture();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapCell!.HasFurniture, Is.False);
        }

        #endregion

        #region HELPERS

        private void PlaceStructureWithFloor(StructureType structure)
        {
            _mapCell!.TrySetFloor(FloorType.Metal);
            _mapCell!.TryPlaceStructure(structure);
        }

        private void PlaceFurnitureWithFloor(FurnitureType furniture)
        {
            _mapCell!.TrySetFloor(FloorType.Metal);
            _mapCell!.TryPlaceFurniture(furniture);
        }

        #endregion
    }
}