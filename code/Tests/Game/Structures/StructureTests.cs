using System.Reflection;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Structures
{
    [TestFixture]
    internal sealed class StructureTests
    {
        #region Construction

        [Test]
        public void Constructor_NullDefinition_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Structure(new StructureId(1), null!, new MapCellCoord(0, 0, 0), FootprintRotation.Deg0));
        }

        [Test]
        public void Constructor_RetainsIdentityDefinitionAnchorAndRotation()
        {
            var id = new StructureId(7);
            StructureDefinition definition = Definition(SingleCellFootprint());
            var anchor = new MapCellCoord(-2, 3, 4);

            var structure = new Structure(id, definition, anchor, FootprintRotation.Deg180);

            Assert.That(structure.Id, Is.EqualTo(id));
            Assert.That(structure.Definition, Is.SameAs(definition));
            Assert.That(structure.Anchor, Is.EqualTo(anchor));
            Assert.That(structure.Rotation, Is.EqualTo(FootprintRotation.Deg180));
        }

        [Test]
        public void PlacementProperties_DoNotExposeSetters()
        {
            string[] mutableProperties = typeof(Structure)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(property => property.SetMethod is not null)
                .Select(property => property.Name)
                .ToArray();

            Assert.That(mutableProperties, Is.Empty);
        }

        [Test]
        public void Structure_ExposesOnlyCoreDomainProperties()
        {
            Type[] propertyTypes = typeof(Structure)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(property => property.PropertyType)
                .ToArray();

            Assert.That(
                propertyTypes,
                Is.EquivalentTo(new[]
                {
                    typeof(StructureId),
                    typeof(StructureDefinition),
                    typeof(MapCellCoord),
                    typeof(FootprintRotation)
                }));
        }

        #endregion

        #region Occupied Cells

        [Test]
        public void ResolveOccupiedCells_SingleCellStructure_ReturnsAnchor()
        {
            var anchor = new MapCellCoord(1, 2, 3);
            var structure = new Structure(new StructureId(1), Definition(SingleCellFootprint()), anchor, FootprintRotation.Deg0);

            IReadOnlyList<MapCellCoord> resolved = structure.ResolveOccupiedCells();

            Assert.That(resolved, Is.EqualTo(new[] { anchor }));
        }

        [Test]
        public void ResolveOccupiedCells_MultiCellStructure_ReturnsAllFootprintCells()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 1, 1)
            });
            var structure = new Structure(
                new StructureId(1),
                Definition(footprint),
                new MapCellCoord(5, 10, -5),
                FootprintRotation.Deg0);

            IReadOnlyList<MapCellCoord> resolved = structure.ResolveOccupiedCells();

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(5, 10, -5),
                    new MapCellCoord(6, 10, -5),
                    new MapCellCoord(6, 11, -4)
                }));
        }

        [Test]
        public void ResolveOccupiedCells_RotationUsesFootprintSemantics()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 0, 1)
            });
            var structure = new Structure(new StructureId(1), Definition(footprint), new MapCellCoord(2, 0, 2), FootprintRotation.Deg90);

            IReadOnlyList<MapCellCoord> resolved = structure.ResolveOccupiedCells();

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(2, 0, 2),
                    new MapCellCoord(2, 0, 1),
                    new MapCellCoord(3, 0, 1)
                }));
        }

        [Test]
        public void ResolveOccupiedCells_NegativeAnchor_ReturnsNegativeCoordinates()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(-1, 0, 0),
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0)
            });
            var structure = new Structure(new StructureId(1), Definition(footprint), new MapCellCoord(-10, 2, -4), FootprintRotation.Deg0);

            IReadOnlyList<MapCellCoord> resolved = structure.ResolveOccupiedCells();

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(-11, 2, -4),
                    new MapCellCoord(-10, 2, -4),
                    new MapCellCoord(-9, 2, -4)
                }));
        }

        [Test]
        public void ResolveOccupiedCells_RepeatedCalls_ReturnSameCoordinateOrder()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(0, 1, 1),
                new MapCellOffset(1, 0, 0)
            });
            var structure = new Structure(new StructureId(1), Definition(footprint), new MapCellCoord(-3, 4, 5), FootprintRotation.Deg270);

            IReadOnlyList<MapCellCoord> first = structure.ResolveOccupiedCells();
            IReadOnlyList<MapCellCoord> second = structure.ResolveOccupiedCells();

            Assert.That(second, Is.EqualTo(first));
        }

        #endregion

        #region Helpers

        private static StructureDefinition Definition(MapCellFootprint footprint)
        {
            return new StructureDefinition(new StructureDefinitionId(1), footprint);
        }

        private static MapCellFootprint SingleCellFootprint()
        {
            return new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            });
        }

        #endregion
    }
}
