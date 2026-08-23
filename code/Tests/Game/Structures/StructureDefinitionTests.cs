using System.Reflection;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Structures
{
    [TestFixture]
    internal sealed class StructureDefinitionTests
    {
        #region Construction

        [Test]
        public void Constructor_NullFootprint_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StructureDefinition(new StructureDefinitionId(1), null!));
        }

        [Test]
        public void Constructor_RetainsStableIdentityAndFootprint()
        {
            var definitionId = new StructureDefinitionId(42);
            MapCellFootprint footprint = SingleCellFootprint();

            var definition = new StructureDefinition(definitionId, footprint);

            Assert.That(definition.Id, Is.EqualTo(definitionId));
            Assert.That(definition.Footprint, Is.SameAs(footprint));
        }

        [Test]
        public void Definition_SingleCellFootprint_ResolvesOneCell()
        {
            var definition = new StructureDefinition(new StructureDefinitionId(1), SingleCellFootprint());
            var anchor = new MapCellCoord(3, 2, -1);

            IReadOnlyList<MapCellCoord> resolved = definition.Footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(resolved, Is.EqualTo(new[] { anchor }));
        }

        [Test]
        public void Definition_ArbitraryMultiCellFootprint_UsesSameFootprintModel()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 2)
            });
            var definition = new StructureDefinition(new StructureDefinitionId(1), footprint);

            Assert.That(
                definition.Footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 2),
                    new MapCellOffset(2, 0, 0)
                }));
        }

        [Test]
        public void Definition_VerticalNonRectangularFootprint_Resolves3dCells()
        {
            var definition = new StructureDefinition(
                new StructureDefinitionId(1),
                new MapCellFootprint(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 1, 0),
                    new MapCellOffset(2, 2, 1)
                }));
            var anchor = new MapCellCoord(-4, 5, 9);

            IReadOnlyList<MapCellCoord> resolved = definition.Footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(-4, 5, 9),
                    new MapCellCoord(-4, 6, 9),
                    new MapCellCoord(-2, 7, 10)
                }));
        }

        [Test]
        public void Definition_FootprintOffsetsCannotMutateAfterConstruction()
        {
            var definition = new StructureDefinition(new StructureDefinitionId(1), SingleCellFootprint());
            var exposedList = (IList<MapCellOffset>)definition.Footprint.Offsets;

            Assert.Throws<NotSupportedException>(() =>
                exposedList[0] = new MapCellOffset(99, 99, 99));
            Assert.That(definition.Footprint.Offsets[0], Is.EqualTo(new MapCellOffset(0, 0, 0)));
        }

        [Test]
        public void Definition_ExposesOnlyCoreDomainProperties()
        {
            Type[] propertyTypes = typeof(StructureDefinition)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(property => property.PropertyType)
                .ToArray();

            Assert.That(
                propertyTypes,
                Is.EquivalentTo(new[]
                {
                    typeof(StructureDefinitionId),
                    typeof(MapCellFootprint)
                }));
        }

        #endregion

        #region Helpers

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
