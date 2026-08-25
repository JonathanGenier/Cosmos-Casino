using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Furniture
{
    [TestFixture]
    internal sealed class FurnitureDefinitionsTests
    {
        #region Casino Table Definition

        [Test]
        public void CasinoTable_HasStableExpectedDefinitionId()
        {
            Assert.That(FurnitureDefinitions.CasinoTableDefinitionIdValue, Is.EqualTo(2000));
            Assert.That(FurnitureDefinitions.CasinoTableDefinitionId.Value, Is.EqualTo(FurnitureDefinitions.CasinoTableDefinitionIdValue));
            Assert.That(FurnitureDefinitions.CasinoTable.Id, Is.EqualTo(FurnitureDefinitions.CasinoTableDefinitionId));
        }

        [Test]
        public void CasinoTable_FootprintContainsExpectedCanonicalOffsets()
        {
            Assert.That(
                FurnitureDefinitions.CasinoTable.Footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(0, 0, 1),
                    new MapCellOffset(1, 0, 0),
                    new MapCellOffset(1, 0, 1),
                    new MapCellOffset(2, 0, 0),
                    new MapCellOffset(2, 0, 1)
                }));
        }

        [TestCase(
            FootprintRotation.Deg0,
            new[] { 10, 2, -7, 10, 2, -6, 11, 2, -7, 11, 2, -6, 12, 2, -7, 12, 2, -6 })]
        [TestCase(
            FootprintRotation.Deg90,
            new[] { 10, 2, -7, 11, 2, -7, 10, 2, -8, 11, 2, -8, 10, 2, -9, 11, 2, -9 })]
        [TestCase(
            FootprintRotation.Deg180,
            new[] { 10, 2, -7, 10, 2, -8, 9, 2, -7, 9, 2, -8, 8, 2, -7, 8, 2, -8 })]
        [TestCase(
            FootprintRotation.Deg270,
            new[] { 10, 2, -7, 9, 2, -7, 10, 2, -6, 9, 2, -6, 10, 2, -5, 9, 2, -5 })]
        public void CasinoTable_ResolveAtArbitraryAnchor_UsesExpectedRotatedFootprint(
            FootprintRotation rotation,
            int[] expectedCoordComponents)
        {
            var anchor = new MapCellCoord(10, 2, -7);
            MapCellCoord[] expected = ToCoords(expectedCoordComponents);

            IReadOnlyList<MapCellCoord> resolved = FurnitureDefinitions.CasinoTable.Footprint.Resolve(anchor, rotation);

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
