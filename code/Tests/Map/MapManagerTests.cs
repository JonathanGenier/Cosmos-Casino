using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Furniture;
using CosmosCasino.Core.Map;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Structure;
using NUnit.Framework;

namespace CosmosCasino.Tests.Map
{
    [TestFixture]
    internal class MapManagerTests
    {
        #region FIELDS

        private MapManager? _mapManager;
        private MapCellCoord _coord;

        #endregion

        #region SETUP & TEARDOWN

        [SetUp]
        public void Setup()
        {
            _mapManager = new MapManager();
            _coord = new MapCellCoord(1, 1, 1);
        }

        #endregion

        #region HasFloor

        [Test]
        public void HasFloor_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Act
            var result = _mapManager!.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasFloor_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.HasFloor(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetFloorType

        [Test]
        public void GetFloorType_ShouldReturnType_WhenCellExists()
        {
            // Arrange
            var floorType = FloorType.Metal;
            _mapManager!.TryPlaceFloor(_coord, floorType);

            // Act
            var result = _mapManager!.GetFloorType(_coord);

            // Assert
            Assert.That(result, Is.EqualTo(floorType));
        }

        [Test]
        public void GetFloorType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.GetFloorType(_coord);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region TryPlaceFloor

        [Test]
        public void TryPlaceFloor_CreatesCell_WhenCellDoesNotExist()
        {
            // Arrange
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Assert
            Assert.That(_mapManager!.CellCount, Is.EqualTo(prevCellCount + 1));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        [Test]
        public void TryPlaceFloor_PreservesExistingCell_WhenCellAlreadyExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Carbon);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Assert
            Assert.That(_mapManager!.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
            Assert.That(_mapManager.GetFloorType(_coord), Is.EqualTo(FloorType.Carbon));
        }

        #endregion

        #region TryReplaceFloor

        [Test]
        public void TryReplaceFloor_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            MapOperationResult result = _mapManager!.TryReplaceFloor(_coord, FloorType.Metal);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryReplaceFloor_ShouldReplaceFloor_WhenCellExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Carbon);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryReplaceFloor(_coord, FloorType.Metal);

            // Assert
            Assert.That(_mapManager.GetFloorType(_coord), Is.EqualTo(FloorType.Metal));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryReplaceFloor_ShouldNotMutate_WhenReplacementIsBlocked()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Carbon);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryReplaceFloor(_coord, FloorType.Carbon);

            // Assert
            Assert.That(_mapManager.GetFloorType(_coord), Is.EqualTo(FloorType.Carbon));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region TryRemoveFloor

        [Test]
        public void TryRemoveFloor_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryRemoveFloor(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveFloor_ShouldRemoveCell_WhenFloorIsRemoved()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryRemoveFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount - 1));
            Assert.That(_mapManager.HasFloor(_coord), Is.False);
        }

        [Test]
        public void TryRemoveFloor_ShouldNotDeleteCell_WhenRemovalIsBlocked()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            _mapManager!.TryPlaceStructure(_coord, StructureType.Wall);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryRemoveFloor(_coord);

            // Assert
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
            Assert.That(_mapManager.HasFloor(_coord), Is.True);
        }

        #endregion

        #region HasStructure

        [Test]
        public void HasStructure_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            PlaceStructureWithFloor(_coord);

            // Act
            var result = _mapManager!.HasStructure(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasStructure_ShouldReturnFalse_WhenCellExistsWithoutStructure()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Act
            var result = _mapManager!.HasStructure(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasStructure_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.HasStructure(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetStructureType

        [Test]
        public void GetStructureType_ShouldReturnStructureType_WhenCellExists()
        {
            // Arrange
            PlaceStructureWithFloor(_coord, StructureType.Wall);

            // Act
            var result = _mapManager!.GetStructureType(_coord);

            // Assert
            Assert.That(result, Is.EqualTo(StructureType.Wall));
        }

        [Test]
        public void GetStructureType_ShouldReturnNull_WhenCellExistsWithoutStructure()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Act
            var result = _mapManager!.GetStructureType(_coord);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetStructureType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.GetStructureType(_coord);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region TryPlaceStructure

        [Test]
        public void TryPlaceStructure_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryPlaceStructure(_coord, StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryPlaceStructure_ShouldPlaceStructure_WhenCellExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryPlaceStructure(_coord, StructureType.Wall);

            // Assert
            Assert.That(_mapManager.HasStructure(_coord), Is.True);
            Assert.That(_mapManager.GetStructureType(_coord), Is.EqualTo(StructureType.Wall));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region TryReplaceStructure

        [Test]
        public void TryReplaceStructure_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryReplaceStructure(_coord, StructureType.Wall);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryReplaceStructure_ShouldReplaceStructure_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(_coord, StructureType.Wall);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryReplaceStructure(_coord, StructureType.Door);

            // Assert
            Assert.That(_mapManager.GetStructureType(_coord), Is.EqualTo(StructureType.Door));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryReplaceStructure_ShouldNotMutate_WhenReplacementIsSameType()
        {
            // Arrange
            PlaceStructureWithFloor(_coord, StructureType.Wall);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryReplaceStructure(_coord, StructureType.Wall);

            // Assert
            Assert.That(_mapManager.GetStructureType(_coord), Is.EqualTo(StructureType.Wall));
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region TryRemoveStructure

        [Test]
        public void TryRemoveStructure_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryRemoveStructure(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveStructure_ShouldRemoveStructure_WhenStructureExists()
        {
            // Arrange
            PlaceStructureWithFloor(_coord);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryRemoveStructure(_coord);

            // Assert
            Assert.That(_mapManager.HasStructure(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryRemoveStructure_ShouldNotMutate_WhenNoStructureExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryRemoveStructure(_coord);

            // Assert
            Assert.That(_mapManager.HasStructure(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region HasFurniture

        [Test]
        public void HasFurniture_ShouldReturnTrue_WhenCellExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(_coord);

            // Act
            var result = _mapManager!.HasFurniture(_coord);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasFurniture_ShouldReturnFalse_WhenCellExistsWithoutFurniture()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Act
            var result = _mapManager!.HasFurniture(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasFurniture_ShouldReturnFalse_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.HasFurniture(_coord);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region GetFurnitureType

        [Test]
        public void GetFurnitureType_ShouldReturnFurnitureType_WhenCellExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(_coord, FurnitureType.SlotMachine);

            // Act
            var result = _mapManager!.GetFurnitureType(_coord);

            // Assert
            Assert.That(result, Is.EqualTo(FurnitureType.SlotMachine));
        }

        [Test]
        public void GetFurnitureType_ShouldReturnNull_WhenCellExistsWithoutFurniture()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);

            // Act
            var result = _mapManager!.GetFurnitureType(_coord);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetFurnitureType_ShouldReturnNull_WhenCellDoesNotExists()
        {
            // Act
            var result = _mapManager!.GetFurnitureType(_coord);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region TryPlaceFurniture

        [Test]
        public void TryPlaceFurniture_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryPlaceFurniture(_coord, FurnitureType.SlotMachine);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager!.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryPlaceFurniture_ShouldPlaceFurniture_WhenCellExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryPlaceFurniture(_coord, FurnitureType.SlotMachine);

            // Assert
            Assert.That(_mapManager!.HasFurniture(_coord), Is.True);
            Assert.That(_mapManager!.GetFurnitureType(_coord), Is.EqualTo(FurnitureType.SlotMachine));
            Assert.That(_mapManager!.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region TryRemoveFurniture

        [Test]
        public void TryRemoveFurniture_ShouldFail_WhenCellDoesNotExist()
        {
            // Act
            var result = _mapManager!.TryRemoveFurniture(_coord);

            // Assert
            Assert.That(result.Outcome, Is.EqualTo(MapCellOutcome.Failed));
            Assert.That(result.FailureReason, Is.EqualTo(MapCellFailureReason.NoCell));
            Assert.That(_mapManager.CellCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveFurniture_ShouldRemoveFurniture_WhenFurnitureExists()
        {
            // Arrange
            PlaceFurnitureWithFloor(_coord);
            int prevCellCount = _mapManager!.CellCount;

            // Act
            _mapManager!.TryRemoveFurniture(_coord);

            // Assert
            Assert.That(_mapManager.HasFurniture(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        [Test]
        public void TryRemoveFurniture_ShouldNotMutate_WhenNoFurnitureExists()
        {
            // Arrange
            _mapManager!.TryPlaceFloor(_coord, FloorType.Metal);
            int prevCellCount = _mapManager.CellCount;

            // Act
            _mapManager!.TryRemoveFurniture(_coord);

            // Assert
            Assert.That(_mapManager.HasFurniture(_coord), Is.False);
            Assert.That(_mapManager.CellCount, Is.EqualTo(prevCellCount));
        }

        #endregion

        #region HELPERS

        private void PlaceStructureWithFloor(
            MapCellCoord coord,
            StructureType structure = StructureType.Wall,
            FloorType floor = FloorType.Metal)
        {
            _mapManager!.TryPlaceFloor(coord, floor);
            _mapManager!.TryPlaceStructure(coord, structure);
        }

        private void PlaceFurnitureWithFloor(
            MapCellCoord coord,
            FurnitureType furniture = FurnitureType.SlotMachine,
            FloorType floor = FloorType.Metal)
        {
            _mapManager!.TryPlaceFloor(coord, floor);
            _mapManager!.TryPlaceFurniture(coord, furniture);
        }

        #endregion
    }
}