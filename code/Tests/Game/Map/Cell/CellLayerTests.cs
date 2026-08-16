using CosmosCasino.Core.Game.Buildables;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class CellLayerTests
    {
        #region Floor

        [Test]
        public void PlaceFloor_EmptyLayer_OwnsProvidedFloor()
        {
            var layer = new CellLayer();
            var floor = new Floor();

            layer.PlaceFloor(floor);

            Assert.That(layer.Floor, Is.SameAs(floor));
            Assert.That(layer.HasFloor, Is.True);
            Assert.That(layer.IsEmpty, Is.False);
        }

        [Test]
        public void PlaceFloor_DuplicateFloor_ThrowsInvalidOperationException()
        {
            var layer = LayerWithFloor();

            Assert.Throws<InvalidOperationException>(() => layer.PlaceFloor(new Floor()));
        }

        [Test]
        public void PlaceFloor_NullFloor_ThrowsArgumentNullException()
        {
            var layer = new CellLayer();

            Assert.Throws<ArgumentNullException>(() => layer.PlaceFloor(null!));
            Assert.That(layer.IsEmpty, Is.True);
        }

        [Test]
        public void RemoveFloor_MissingFloor_ThrowsInvalidOperationException()
        {
            var layer = new CellLayer();

            Assert.Throws<InvalidOperationException>(() => layer.RemoveFloor());
        }

        [Test]
        public void RemoveFloor_WallRemains_ThrowsInvalidOperationException()
        {
            var layer = LayerWithFloorAndWall();

            Assert.Throws<InvalidOperationException>(() => layer.RemoveFloor());
            Assert.That(layer.HasFloor, Is.True);
            Assert.That(layer.HasWall, Is.True);
        }

        [Test]
        public void RemoveFloor_NoDependentContent_EmptiesLayer()
        {
            var layer = LayerWithFloor();

            layer.RemoveFloor();

            Assert.That(layer.HasFloor, Is.False);
            Assert.That(layer.IsEmpty, Is.True);
        }

        #endregion

        #region Wall

        [Test]
        public void PlaceWall_MissingFloor_ThrowsInvalidOperationException()
        {
            var layer = new CellLayer();

            Assert.Throws<InvalidOperationException>(() => layer.PlaceWall(new Wall()));
        }

        [Test]
        public void PlaceWall_ExistingFloor_OwnsProvidedWall()
        {
            var layer = LayerWithFloor();
            var wall = new Wall();

            layer.PlaceWall(wall);

            Assert.That(layer.Wall, Is.SameAs(wall));
            Assert.That(layer.HasWall, Is.True);
        }

        [Test]
        public void PlaceWall_DuplicateWall_ThrowsInvalidOperationException()
        {
            var layer = LayerWithFloorAndWall();

            Assert.Throws<InvalidOperationException>(() => layer.PlaceWall(new Wall()));
        }

        [Test]
        public void PlaceWall_NullWall_ThrowsArgumentNullException()
        {
            var layer = LayerWithFloor();

            Assert.Throws<ArgumentNullException>(() => layer.PlaceWall(null!));
            Assert.That(layer.HasWall, Is.False);
        }

        [Test]
        public void RemoveWall_MissingWall_ThrowsInvalidOperationException()
        {
            var layer = LayerWithFloor();

            Assert.Throws<InvalidOperationException>(() => layer.RemoveWall());
        }

        [Test]
        public void RemoveWall_ExistingWall_PreservesFloor()
        {
            var layer = LayerWithFloorAndWall();

            layer.RemoveWall();

            Assert.That(layer.HasWall, Is.False);
            Assert.That(layer.HasFloor, Is.True);
            Assert.That(layer.IsEmpty, Is.False);
        }

        #endregion

        #region HELPERS

        private static CellLayer LayerWithFloor()
        {
            var layer = new CellLayer();
            layer.PlaceFloor(new Floor());
            return layer;
        }

        private static CellLayer LayerWithFloorAndWall()
        {
            var layer = LayerWithFloor();
            layer.PlaceWall(new Wall());
            return layer;
        }

        #endregion
    }
}
