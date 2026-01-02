using CosmosCasino.Core.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Map
{
    [TestFixture]
    internal class MapManagerTests
    {
        #region FIELDS

        private MapManager? _mapManager;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
        }

        #endregion

        #region CanRemoveFloor

        [Test]
        public void CanRemoveFloor_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.CanRemoveFloor(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanRemoveFloor_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanRemoveFloor(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.HasFloor(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.HasFloor(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region GetFloorType

        [Test]
        public void GetFloorType_ShouldReturnType_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            var floorType = FloorType.Metal;
            _mapManager!.TrySetFloor(coord, floorType);

            // Act
            var result = _mapManager!.GetFloorType(coord);

            // Assert
            Assert.That(result, Is.EqualTo(floorType));
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void GetFloorType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.GetFloorType(coord);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TrySetFloor

        [Test]
        public void TrySetFloor_ShouldCreateNewCellAndReturnTrue_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.HasFloor(coord), Is.True);
        }

        [Test]
        public void TrySetFloor_ShouldReplaceFloorAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Carbon);

            // Act
            var result = _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.GetFloorType(coord), Is.EqualTo(FloorType.Metal));
        }

        [Test]
        public void TrySetFloor_ShouldNotReplaceFloorAndReturnFalse_WhenFloorTypeIsTheSame()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.GetFloorType(coord), Is.EqualTo(FloorType.Metal));
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldRemoveFloorAndRemoveCellAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.TryRemoveFloor(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveFloor_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryRemoveFloor(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveFloor_ShouldNotRemoveFloorAndNotRemoveCellAndReturnFalse_WhenCellIsNotEmpty()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord);

            // Act
            var result = _mapManager!.TryRemoveFloor(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasStructure(coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        #endregion

        #region CanPlaceStructure

        [Test]
        public void CanPlaceStructure_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.CanPlaceStructure(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanPlaceStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanPlaceStructure(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region CanReplaceStructure

        [Test]
        public void CanReplaceStructure_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord);

            // Act
            var result = _mapManager!.CanReplaceStructure(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanReplaceStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanReplaceStructure(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region CanRemoveStructure

        [Test]
        public void CanRemoveStructure_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord);

            // Act
            var result = _mapManager!.CanRemoveStructure(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanRemoveStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanRemoveStructure(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region HasStructure

        [Test]
        public void HasStructure_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord);

            // Act
            var result = _mapManager!.HasStructure(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void HasStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.HasStructure(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region GetStructureType

        [Test]
        public void GetStructureType_ShouldReturnStructureType_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord, StructureType.Wall);

            // Act
            var result = _mapManager!.GetStructureType(coord);

            // Assert
            Assert.That(result, Is.EqualTo(StructureType.Wall));
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void GetStructureType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.GetStructureType(coord);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TryPlaceStructure

        [Test]
        public void TryPlaceStructure_ShouldPlaceStructureAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.TryPlaceStructure(coord, StructureType.Wall);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasStructure(coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryPlaceStructure_ShouldNotPlaceStructureAndReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryPlaceStructure(coord, StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TryReplaceStructure

        [Test]
        public void TryReplaceStructure_ShouldReplaceStructureAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord, StructureType.Door);

            // Act
            var result = _mapManager!.TryReplaceStructure(coord, StructureType.Wall);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasStructure(coord), Is.True);
            Assert.That(_mapManager!.GetStructureType(coord), Is.EqualTo(StructureType.Wall));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryReplaceStructure_ShouldNotReplaceStructureAndReturnFalse_WhenReplacingWithSameStructureType()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord, StructureType.Wall);

            // Act
            var result = _mapManager!.TryReplaceStructure(coord, StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasStructure(coord), Is.True);
            Assert.That(_mapManager!.GetStructureType(coord), Is.EqualTo(StructureType.Wall));
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryReplaceStructure_ShouldNotReplaceStructureAndReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryReplaceStructure(coord, StructureType.Wall);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TryRemoveStructure

        [Test]
        public void TryRemoveStructure_ShouldRemoveStructureAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceStructureWithFloor(coord);

            // Act
            var result = _mapManager!.TryRemoveStructure(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasStructure(coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryRemoveStructure(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region CanPlaceFurniture

        [Test]
        public void CanPlaceFuniture_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.CanPlaceFurniture(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanPlaceFurniture_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanPlaceFurniture(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region CanRemoveFurniture

        [Test]
        public void CanRemoveFurniture_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceFurnitureWithFloor(coord);

            // Act
            var result = _mapManager!.CanRemoveFurniture(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void CanRemoveFurniture_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.CanRemoveFurniture(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region HasFurniture

        [Test]
        public void HasFuniture_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceFurnitureWithFloor(coord);

            // Act
            var result = _mapManager!.HasFurniture(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void HasFuniture_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.HasFurniture(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region GetFurnitureType

        [Test]
        public void GetFurnitureType_ShouldReturnStructureType_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceFurnitureWithFloor(coord, FurnitureType.SlotMachine);

            // Act
            var result = _mapManager!.GetFurnitureType(coord);

            // Assert
            Assert.That(result, Is.EqualTo(FurnitureType.SlotMachine));
            Assert.That(_mapManager!.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void GetFurnitureType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.GetFurnitureType(coord);

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TryPlaceFurniture

        [Test]
        public void TryPlaceFurniture_ShouldPlaceFurnitureAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            _mapManager!.TrySetFloor(coord, FloorType.Metal);

            // Act
            var result = _mapManager!.TryPlaceFurniture(coord, FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasFurniture(coord), Is.True);
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryPlaceFurniture_ShouldNotPlaceFurnitureAndReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryPlaceFurniture(coord, FurnitureType.SlotMachine);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region TryRemoveFurniture

        [Test]
        public void TryRemoveFurniture_ShouldRemoveFurnitureAndReturnTrue_WhenCellExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);
            PlaceFurnitureWithFloor(coord);

            // Act
            var result = _mapManager!.TryRemoveFurniture(coord);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mapManager.HasFloor(coord), Is.True);
            Assert.That(_mapManager.HasFurniture(coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveFurniture_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Arrange
            var coord = new CellCoord(1, 1, 1);

            // Act
            var result = _mapManager!.TryRemoveFurniture(coord);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        #endregion

        #region HELPERS

        private void PlaceStructureWithFloor(
            CellCoord coord,
            StructureType structure = StructureType.Wall,
            FloorType floor = FloorType.Metal)
        {
            _mapManager!.TrySetFloor(coord, floor);
            _mapManager!.TryPlaceStructure(coord, structure);
        }

        private void PlaceFurnitureWithFloor(
            CellCoord coord,
            FurnitureType furniture = FurnitureType.SlotMachine,
            FloorType floor = FloorType.Metal)
        {
            _mapManager!.TrySetFloor(coord, floor);
            _mapManager!.TryPlaceFurniture(coord, furniture);
        }

        #endregion
    }
}