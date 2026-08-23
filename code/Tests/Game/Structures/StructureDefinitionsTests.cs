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
    }
}
