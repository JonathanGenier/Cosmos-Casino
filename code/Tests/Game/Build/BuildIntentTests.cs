using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;
using System.Reflection;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class BuildIntentTests
    {
        #region Fields

        private static readonly StructureDefinition TestDefinition = new(
            new StructureDefinitionId(10),
            new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            }));

        #endregion

        #region Placement

        [Test]
        public void PlaceStructures_NullPlacements_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => BuildIntent.PlaceStructures(null!));
        }

        [Test]
        public void PlaceStructures_EmptyPlacements_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BuildIntent.PlaceStructures(Array.Empty<StructurePlacementRequest>()));
        }

        [Test]
        public void PlaceStructures_NullPlacementEntry_ThrowsArgumentException()
        {
            var placements = new StructurePlacementRequest?[] { null };

            Assert.Throws<ArgumentException>(() => BuildIntent.PlaceStructures(placements!));
        }

        [Test]
        public void PlaceStructures_ValidPlacements_CreatesStructureIntent()
        {
            var request = new StructurePlacementRequest(
                TestDefinition,
                new MapCellCoord(1, 2, 3),
                FootprintRotation.Deg90);

            BuildIntent intent = BuildIntent.PlaceStructures(new[] { request });

            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.PlacementRequests, Has.Count.EqualTo(1));
            Assert.That(intent.PlacementRequests[0], Is.SameAs(request));
            Assert.That(intent.RemovalRequests, Is.Empty);
        }

        [Test]
        public void PlaceStructures_CopiesPlacementCollectionDefensively()
        {
            var request = new StructurePlacementRequest(
                TestDefinition,
                new MapCellCoord(0, 0, 0),
                FootprintRotation.Deg0);
            var placements = new List<StructurePlacementRequest> { request };

            BuildIntent intent = BuildIntent.PlaceStructures(placements);
            placements.Clear();

            Assert.That(intent.PlacementRequests, Has.Count.EqualTo(1));
            Assert.That(intent.PlacementRequests, Is.Not.SameAs(placements));
        }

        [Test]
        public void PlaceStructure_CreatesSinglePlacementRequest()
        {
            var anchor = new MapCellCoord(-1, 4, 2);

            BuildIntent intent = BuildIntent.PlaceStructure(
                TestDefinition,
                anchor,
                FootprintRotation.Deg270);

            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.PlacementRequests, Has.Count.EqualTo(1));
            Assert.That(intent.PlacementRequests[0].Definition, Is.SameAs(TestDefinition));
            Assert.That(intent.PlacementRequests[0].Anchor, Is.EqualTo(anchor));
            Assert.That(intent.PlacementRequests[0].Rotation, Is.EqualTo(FootprintRotation.Deg270));
            Assert.That(intent.RemovalRequests, Is.Empty);
        }

        #endregion

        #region Removal

        [Test]
        public void RemoveStructuresAt_NullTargets_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => BuildIntent.RemoveStructuresAt(null!));
        }

        [Test]
        public void RemoveStructuresAt_EmptyTargets_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => BuildIntent.RemoveStructuresAt(Array.Empty<MapCellCoord>()));
        }

        [Test]
        public void RemoveStructuresAt_ValidTargets_CreatesStructureRemovalIntent()
        {
            var targets = new[]
            {
                new MapCellCoord(1, 0, 1),
                new MapCellCoord(2, 0, 2)
            };

            BuildIntent intent = BuildIntent.RemoveStructuresAt(targets);

            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.PlacementRequests, Is.Empty);
            Assert.That(intent.RemovalRequests.Select(r => r.TargetCell), Is.EqualTo(targets));
        }

        [Test]
        public void RemoveStructuresAt_CopiesTargetsDefensively()
        {
            var targets = new List<MapCellCoord>
            {
                new MapCellCoord(1, 0, 1)
            };

            BuildIntent intent = BuildIntent.RemoveStructuresAt(targets);
            targets.Clear();

            Assert.That(intent.RemovalRequests, Has.Count.EqualTo(1));
            Assert.That(intent.RemovalRequests[0].TargetCell, Is.EqualTo(new MapCellCoord(1, 0, 1)));
        }

        [Test]
        public void RemoveStructureAt_CreatesSingleTargetRequest()
        {
            var target = new MapCellCoord(3, 2, 1);

            BuildIntent intent = BuildIntent.RemoveStructureAt(target);

            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.PlacementRequests, Is.Empty);
            Assert.That(intent.RemovalRequests, Has.Count.EqualTo(1));
            Assert.That(intent.RemovalRequests[0].TargetCell, Is.EqualTo(target));
        }

        #endregion

        #region Immutability

        [Test]
        public void StructureRequestTypes_ExposeImmutableProperties()
        {
            AssertNoPublicSetters(typeof(StructurePlacementRequest));
            AssertNoPublicSetters(typeof(StructureRemovalRequest));
            AssertNoPublicSetters(typeof(StructureDefinition));
            AssertNoPublicSetters(typeof(MapCellFootprint));
            AssertNoPublicSetters(typeof(MapCellOffset));
        }

        [Test]
        public void BuildIntent_DoesNotExposeLegacyFloorWallContract()
        {
            string[] factoryNames = typeof(BuildIntent)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Select(method => method.Name)
                .ToArray();

            Assert.That(factoryNames, Does.Not.Contain("PlaceFloor"));
            Assert.That(factoryNames, Does.Not.Contain("RemoveFloor"));
            Assert.That(factoryNames, Does.Not.Contain("PlaceWall"));
            Assert.That(factoryNames, Does.Not.Contain("RemoveWall"));
            Assert.That(typeof(BuildIntent).GetProperty("Kind"), Is.Null);
            Assert.That(typeof(BuildIntent).GetProperty("Cells"), Is.Null);
            Assert.That(typeof(BuildIntent).GetProperty("Elevation"), Is.Null);
        }

        [Test]
        public void CoreBuildContracts_DoNotExposeRendererData()
        {
            string[] forbiddenTerms =
            {
                "RendererMode",
                "RenderMode",
                "Mesh",
                "MultiMesh",
                "Scene",
                "PackedScene",
                "SpawnVariant",
                "SpawnLayer",
                "Godot"
            };

            Type[] contractTypes =
            {
                typeof(BuildIntent),
                typeof(BuildResult),
                typeof(BuildStructureResult),
                typeof(StructurePlacementRequest),
                typeof(StructureRemovalRequest),
                typeof(StructureDefinition)
            };

            foreach (Type type in contractTypes)
            {
                string[] memberNames = type
                    .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(member => member.Name)
                    .ToArray();

                foreach (string forbiddenTerm in forbiddenTerms)
                {
                    Assert.That(
                        memberNames.Any(name => name.Contains(forbiddenTerm, StringComparison.Ordinal)),
                        Is.False,
                        $"{type.Name} exposes {forbiddenTerm}.");
                }
            }
        }

        #endregion

        #region General

        [Test]
        public void ToString_ReturnsReadableStructureSummary()
        {
            BuildIntent placement = BuildIntent.PlaceStructure(
                TestDefinition,
                new MapCellCoord(0, 0, 0),
                FootprintRotation.Deg0);
            BuildIntent removal = BuildIntent.RemoveStructureAt(new MapCellCoord(0, 0, 0));

            Assert.That(placement.ToString(), Is.EqualTo("Place 1 structures"));
            Assert.That(removal.ToString(), Is.EqualTo("Remove structures from 1 target cells"));
        }

        #endregion

        #region Helpers

        private void AssertNoPublicSetters(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Assert.That(property.SetMethod, Is.Null, $"{type.Name}.{property.Name} should be immutable.");
            }
        }

        #endregion
    }
}
