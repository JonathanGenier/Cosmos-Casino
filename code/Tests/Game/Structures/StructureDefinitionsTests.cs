using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Structures
{
    [TestFixture]
    internal sealed class StructureDefinitionsTests
    {
        #region Block Definition

        [Test]
        public void Block_HasStableExpectedDefinitionId()
        {
            Assert.That(StructureDefinitions.BlockDefinitionIdValue, Is.EqualTo(1000));
            Assert.That(StructureDefinitions.BlockDefinitionId.Value, Is.EqualTo(StructureDefinitions.BlockDefinitionIdValue));
            Assert.That(StructureDefinitions.Block.Id, Is.EqualTo(StructureDefinitions.BlockDefinitionId));
        }

        [Test]
        public void Block_FootprintContainsOnlyAnchorOffset()
        {
            Assert.That(
                StructureDefinitions.Block.Footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0)
                }));
        }

        [TestCase(FootprintRotation.Deg0)]
        [TestCase(FootprintRotation.Deg90)]
        [TestCase(FootprintRotation.Deg180)]
        [TestCase(FootprintRotation.Deg270)]
        public void Block_ResolveAtArbitraryAnchor_OccupiesOnlyAnchor(FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(-7, 25, 11);

            IReadOnlyList<MapCellCoord> resolved = StructureDefinitions.Block.Footprint.Resolve(anchor, rotation);

            Assert.That(resolved, Is.EqualTo(new[] { anchor }));
        }

        #endregion

        #region Pillar Definition

        [Test]
        public void Pillar_HasStableExpectedDefinitionId()
        {
            Assert.That(StructureDefinitions.PillarDefinitionIdValue, Is.EqualTo(1001));
            Assert.That(StructureDefinitions.PillarDefinitionId.Value, Is.EqualTo(StructureDefinitions.PillarDefinitionIdValue));
            Assert.That(StructureDefinitions.Pillar.Id, Is.EqualTo(StructureDefinitions.PillarDefinitionId));
            Assert.That(StructureDefinitions.PillarDefinitionId, Is.Not.EqualTo(StructureDefinitions.BlockDefinitionId));
        }

        [Test]
        public void Pillar_FootprintContainsOnlyAnchorOffset()
        {
            Assert.That(
                StructureDefinitions.Pillar.Footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0)
                }));
        }

        [TestCase(FootprintRotation.Deg0)]
        [TestCase(FootprintRotation.Deg90)]
        [TestCase(FootprintRotation.Deg180)]
        [TestCase(FootprintRotation.Deg270)]
        public void Pillar_ResolveAtArbitraryAnchor_OccupiesOnlyAnchor(FootprintRotation rotation)
        {
            var anchor = new MapCellCoord(-7, 25, 11);

            IReadOnlyList<MapCellCoord> resolved = StructureDefinitions.Pillar.Footprint.Resolve(anchor, rotation);

            Assert.That(resolved, Is.EqualTo(new[] { anchor }));
        }

        #endregion

        #region Door Definition

        [Test]
        public void Door_HasStableExpectedDefinitionId()
        {
            Assert.That(StructureDefinitions.DoorDefinitionIdValue, Is.EqualTo(1002));
            Assert.That(StructureDefinitions.DoorDefinitionId.Value, Is.EqualTo(StructureDefinitions.DoorDefinitionIdValue));
            Assert.That(StructureDefinitions.Door.Id, Is.EqualTo(StructureDefinitions.DoorDefinitionId));
            Assert.That(StructureDefinitions.DoorDefinitionId, Is.Not.EqualTo(StructureDefinitions.BlockDefinitionId));
            Assert.That(StructureDefinitions.DoorDefinitionId, Is.Not.EqualTo(StructureDefinitions.PillarDefinitionId));
        }

        [Test]
        public void Door_FootprintContainsExpectedCanonicalOffsets()
        {
            Assert.That(
                StructureDefinitions.Door.Footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 1, 0),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 1, 0)
                }));
        }

        [TestCase(
            FootprintRotation.Deg0,
            new[] { 10, 2, -7, 10, 3, -7, 11, 2, -7, 11, 3, -7 })]
        [TestCase(
            FootprintRotation.Deg90,
            new[] { 10, 2, -7, 10, 3, -7, 10, 2, -8, 10, 3, -8 })]
        [TestCase(
            FootprintRotation.Deg180,
            new[] { 10, 2, -7, 10, 3, -7, 9, 2, -7, 9, 3, -7 })]
        [TestCase(
            FootprintRotation.Deg270,
            new[] { 10, 2, -7, 10, 3, -7, 10, 2, -6, 10, 3, -6 })]
        public void Door_ResolveAtArbitraryAnchor_UsesExpectedRotatedFootprint(
            FootprintRotation rotation,
            int[] expectedCoordComponents)
        {
            var anchor = new MapCellCoord(10, 2, -7);
            MapCellCoord[] expected = ToCoords(expectedCoordComponents);

            IReadOnlyList<MapCellCoord> resolved = StructureDefinitions.Door.Footprint.Resolve(anchor, rotation);

            Assert.That(resolved, Is.EqualTo(expected));
        }

        #endregion

        #region Helpers

        private MapCellCoord[] ToCoords(int[] components)
        {
            Assert.That(components.Length % 3, Is.EqualTo(0));

            var coords = new MapCellCoord[components.Length / 3];

            for (int i = 0; i < coords.Length; i++)
            {
                int componentIndex = i * 3;
                coords[i] = new MapCellCoord(
                    components[componentIndex],
                    components[componentIndex + 1],
                    components[componentIndex + 2]);
            }

            return coords;
        }

        #endregion
    }
}
