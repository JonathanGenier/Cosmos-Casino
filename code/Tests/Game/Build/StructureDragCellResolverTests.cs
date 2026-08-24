using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Build
{
    [TestFixture]
    internal sealed class StructureDragCellResolverTests
    {
        #region Rectangle Area

        [Test]
        public void FloorDefault_PositiveDrag_ReturnsRectangleArea()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.Default, Cell(0, 4, 0), Cell(2, 9, 1)),
                Cell(0, 4, 0),
                Cell(0, 4, 1),
                Cell(1, 4, 0),
                Cell(1, 4, 1),
                Cell(2, 4, 0),
                Cell(2, 4, 1));
        }

        [Test]
        public void FloorDefault_ReverseDrag_ReturnsRectangleAreaInDeterministicOrder()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.Default, Cell(2, 4, 1), Cell(0, 9, 0)),
                Cell(0, 4, 0),
                Cell(0, 4, 1),
                Cell(1, 4, 0),
                Cell(1, 4, 1),
                Cell(2, 4, 0),
                Cell(2, 4, 1));
        }

        [Test]
        public void FloorDefault_OneCellWideRectangle_ReturnsVerticalStrip()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.Default, Cell(3, 2, 1), Cell(3, 6, 3)),
                Cell(3, 2, 1),
                Cell(3, 2, 2),
                Cell(3, 2, 3));
        }

        [Test]
        public void FloorDefault_OneCellHighRectangle_ReturnsHorizontalStrip()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.Default, Cell(1, 2, 4), Cell(3, 6, 4)),
                Cell(1, 2, 4),
                Cell(2, 2, 4),
                Cell(3, 2, 4));
        }

        [Test]
        public void FloorDefault_SingleCell_ReturnsStartCell()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.Default, Cell(-2, 7, 5), Cell(-2, 9, 5)),
                Cell(-2, 7, 5));
        }

        #endregion

        #region Rectangle Outline

        [Test]
        public void WallDefault_NormalRectangle_ReturnsPerimeterWithoutDuplicateCorners()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Wall,
                BuildInteractionMode.Default,
                Cell(0, 3, 0),
                Cell(2, 8, 2));

            AssertCells(
                cells,
                Cell(0, 3, 0),
                Cell(1, 3, 0),
                Cell(2, 3, 0),
                Cell(0, 3, 2),
                Cell(1, 3, 2),
                Cell(2, 3, 2),
                Cell(0, 3, 1),
                Cell(2, 3, 1));
        }

        [Test]
        public void WallDefault_OneCellWideRectangle_ReturnsSingleColumnPerimeter()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.Default, Cell(0, 3, 0), Cell(0, 8, 2)),
                Cell(0, 3, 0),
                Cell(0, 3, 2),
                Cell(0, 3, 1));
        }

        [Test]
        public void WallDefault_OneCellHighRectangle_ReturnsSingleRowPerimeter()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.Default, Cell(0, 3, 0), Cell(2, 8, 0)),
                Cell(0, 3, 0),
                Cell(1, 3, 0),
                Cell(2, 3, 0));
        }

        [Test]
        public void WallDefault_SingleCell_ReturnsStartCell()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.Default, Cell(4, 3, -1), Cell(4, 8, -1)),
                Cell(4, 3, -1));
        }

        #endregion

        #region Square Area

        [TestCase(1, 2, 0, 0)]
        [TestCase(-1, 2, -2, 0)]
        [TestCase(1, -2, 0, -2)]
        [TestCase(-1, -2, -2, -2)]
        public void FloorShift_AllDragQuadrants_ReturnsAnchoredSquareArea(
            int currentX,
            int currentZ,
            int expectedMinX,
            int expectedMinZ)
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.ShiftAlternative,
                Cell(0, 6, 0),
                Cell(currentX, 99, currentZ));

            Assert.That(cells, Has.Count.EqualTo(9));
            AssertContains(cells, Cell(expectedMinX, 6, expectedMinZ));
            AssertContains(cells, Cell(expectedMinX + 2, 6, expectedMinZ + 2));
            AssertNoDuplicates(cells);
        }

        [Test]
        public void FloorShift_DominantX_ReturnsSquareArea()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.ShiftAlternative,
                Cell(0, 6, 0),
                Cell(3, 99, 1));

            Assert.That(cells, Has.Count.EqualTo(16));
            AssertContains(cells, Cell(3, 6, 3));
            AssertNoDuplicates(cells);
        }

        [Test]
        public void FloorShift_DominantZ_ReturnsSquareArea()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.ShiftAlternative,
                Cell(0, 6, 0),
                Cell(1, 99, 3));

            Assert.That(cells, Has.Count.EqualTo(16));
            AssertContains(cells, Cell(3, 6, 3));
            AssertNoDuplicates(cells);
        }

        [Test]
        public void FloorShift_EqualDimensions_ReturnsSquareArea()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.ShiftAlternative, Cell(0, 6, 0), Cell(2, 99, 2)),
                Cell(0, 6, 0),
                Cell(0, 6, 1),
                Cell(0, 6, 2),
                Cell(1, 6, 0),
                Cell(1, 6, 1),
                Cell(1, 6, 2),
                Cell(2, 6, 0),
                Cell(2, 6, 1),
                Cell(2, 6, 2));
        }

        #endregion

        #region Square Outline

        [TestCase(1, 2, 0, 0)]
        [TestCase(-1, 2, -2, 0)]
        [TestCase(1, -2, 0, -2)]
        [TestCase(-1, -2, -2, -2)]
        public void WallShift_AllDragDirections_ReturnsAnchoredSquareOutlineWithoutDuplicateCorners(
            int currentX,
            int currentZ,
            int expectedMinX,
            int expectedMinZ)
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Wall,
                BuildInteractionMode.ShiftAlternative,
                Cell(0, 6, 0),
                Cell(currentX, 99, currentZ));

            Assert.That(cells, Has.Count.EqualTo(8));
            AssertContains(cells, Cell(expectedMinX, 6, expectedMinZ));
            AssertContains(cells, Cell(expectedMinX + 2, 6, expectedMinZ + 2));
            AssertNoDuplicates(cells);
        }

        #endregion

        #region Straight Snapped Line

        [Test]
        public void Ctrl_HorizontalSnap_ReturnsHorizontalLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(5, 9, 1)),
                Cell(0, 1, 0),
                Cell(1, 1, 0),
                Cell(2, 1, 0),
                Cell(3, 1, 0),
                Cell(4, 1, 0),
                Cell(5, 1, 0));
        }

        [Test]
        public void Ctrl_VerticalSnap_ReturnsVerticalLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(1, 9, 5)),
                Cell(0, 1, 0),
                Cell(0, 1, 1),
                Cell(0, 1, 2),
                Cell(0, 1, 3),
                Cell(0, 1, 4),
                Cell(0, 1, 5));
        }

        [Test]
        public void Ctrl_DiagonalSnap_ReturnsDiagonalLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(4, 9, 4)),
                Cell(0, 1, 0),
                Cell(1, 1, 1),
                Cell(2, 1, 2),
                Cell(3, 1, 3),
                Cell(4, 1, 4));
        }

        [Test]
        public void Ctrl_NegativeDiagonalSnap_ReturnsDiagonalLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(-3, 9, 3)),
                Cell(0, 1, 0),
                Cell(-1, 1, 1),
                Cell(-2, 1, 2),
                Cell(-3, 1, 3));
        }

        [Test]
        public void Ctrl_SnapAngleNearHorizontalThreshold_ReturnsHistoricalHorizontalOrDiagonalSelection()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(3, 9, 1)),
                Cell(0, 1, 0),
                Cell(1, 1, 0),
                Cell(2, 1, 0),
                Cell(3, 1, 0));

            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, Cell(0, 1, 0), Cell(2, 9, 1)),
                Cell(0, 1, 0),
                Cell(1, 1, 1));
        }

        #endregion

        #region Dynamic Line

        [Test]
        public void Alt_ShallowSlope_ReturnsBresenhamLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.AltAlternative, Cell(0, 2, 0), Cell(5, 9, 2)),
                Cell(0, 2, 0),
                Cell(1, 2, 0),
                Cell(2, 2, 1),
                Cell(3, 2, 1),
                Cell(4, 2, 2),
                Cell(5, 2, 2));
        }

        [Test]
        public void Alt_SteepSlope_ReturnsBresenhamLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.AltAlternative, Cell(0, 2, 0), Cell(2, 9, 5)),
                Cell(0, 2, 0),
                Cell(0, 2, 1),
                Cell(1, 2, 2),
                Cell(1, 2, 3),
                Cell(2, 2, 4),
                Cell(2, 2, 5));
        }

        [Test]
        public void Alt_ReversedEndpoints_ReturnsBresenhamLineFromStartToCurrent()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.AltAlternative, Cell(5, 2, 2), Cell(0, 9, 0)),
                Cell(5, 2, 2),
                Cell(4, 2, 2),
                Cell(3, 2, 1),
                Cell(2, 2, 1),
                Cell(1, 2, 0),
                Cell(0, 2, 0));
        }

        [Test]
        public void Alt_NegativeCoordinates_ReturnsBresenhamLine()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.AltAlternative, Cell(-3, 2, -2), Cell(2, 9, 1)),
                Cell(-3, 2, -2),
                Cell(-2, 2, -1),
                Cell(-1, 2, -1),
                Cell(0, 2, 0),
                Cell(1, 2, 0),
                Cell(2, 2, 1));
        }

        #endregion

        #region Circle

        [Test]
        public void FloorShiftCtrl_SmallRadius_ReturnsSingleFilledCell()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Floor, BuildInteractionMode.ShiftCtrlAlternative, Cell(4, 8, 4), Cell(4, 99, 4)),
                Cell(4, 8, 4));
        }

        [Test]
        public void FloorShiftCtrl_LargerRadius_ReturnsSymmetricFilledCircle()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.ShiftCtrlAlternative,
                Cell(0, 8, 0),
                Cell(4, 99, 4));

            Assert.That(cells, Has.Count.EqualTo(21));
            AssertContains(cells, Cell(2, 8, 2));
            AssertContains(cells, Cell(0, 8, 2));
            AssertContains(cells, Cell(4, 8, 2));
            AssertContains(cells, Cell(2, 8, 0));
            AssertContains(cells, Cell(2, 8, 4));
            AssertSymmetricAbout(cells, 2, 8, 2);
            AssertNoDuplicates(cells);
        }

        [Test]
        public void WallShiftCtrl_SmallRadius_ReturnsSingleOutlineCell()
        {
            AssertCells(
                ResolvePlacement(StructureBuildTool.Wall, BuildInteractionMode.ShiftCtrlAlternative, Cell(4, 8, 4), Cell(4, 99, 4)),
                Cell(4, 8, 4));
        }

        [Test]
        public void WallShiftCtrl_LargerRadius_ReturnsSymmetricOutlineWithoutDuplicateCells()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Wall,
                BuildInteractionMode.ShiftCtrlAlternative,
                Cell(0, 8, 0),
                Cell(4, 99, 4));

            Assert.That(cells, Has.Count.EqualTo(12));
            AssertContains(cells, Cell(0, 8, 2));
            AssertContains(cells, Cell(4, 8, 2));
            AssertContains(cells, Cell(2, 8, 0));
            AssertContains(cells, Cell(2, 8, 4));
            AssertSymmetricAbout(cells, 2, 8, 2);
            AssertNoDuplicates(cells);
        }

        #endregion

        #region 3D Adaptation

        [Test]
        public void Placement_CurrentTargetHasDifferentY_UsesStartingPlacementY()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.Default,
                Cell(10, 4, 10),
                Cell(15, 7, 15));

            Assert.That(cells, Has.Count.EqualTo(36));
            AssertContains(cells, Cell(15, 4, 15));
            Assert.That(cells.All(cell => cell.Y == 4), Is.True);
        }

        [Test]
        public void Placement_NegativeCoordinatesAcrossChunkBoundary_UsesPlainGlobalCells()
        {
            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                StructureBuildTool.Floor,
                BuildInteractionMode.Default,
                Cell(-1, 4, 14),
                Cell(1, 9, 15));

            Assert.That(cells, Has.Count.EqualTo(6));
            AssertCells(
                cells,
                Cell(-1, 4, 14),
                Cell(-1, 4, 15),
                Cell(0, 4, 14),
                Cell(0, 4, 15),
                Cell(1, 4, 14),
                Cell(1, 4, 15));
        }

        [Test]
        public void Placement_UsesPlacementCellsRatherThanTargetCells()
        {
            CursorTarget start = CursorTarget.Structure(
                Cell(50, 4, 50),
                Cell(0, 6, 0),
                new StructureId(1),
                CursorSurfaceFace.Top);

            CursorTarget current = CursorTarget.Structure(
                Cell(70, 9, 70),
                Cell(1, 12, 1),
                new StructureId(2),
                CursorSurfaceFace.Top);

            IReadOnlyList<MapCellCoord> cells = StructureDragCellResolver.Resolve(
                StructureBuildTool.Floor,
                BuildOperation.Place,
                BuildInteractionMode.Default,
                start,
                current);

            AssertCells(
                cells,
                Cell(0, 6, 0),
                Cell(0, 6, 1),
                Cell(1, 6, 0),
                Cell(1, 6, 1));
        }

        [Test]
        public void Removal_UsesTargetCellsAndStartingTargetY()
        {
            CursorTarget start = CursorTarget.Structure(
                Cell(0, 5, 0),
                Cell(90, 10, 90),
                new StructureId(1),
                CursorSurfaceFace.Top);

            CursorTarget current = CursorTarget.Structure(
                Cell(2, 7, 1),
                Cell(99, 11, 99),
                new StructureId(2),
                CursorSurfaceFace.Top);

            IReadOnlyList<MapCellCoord> cells = StructureDragCellResolver.Resolve(
                StructureBuildTool.Wall,
                BuildOperation.Remove,
                BuildInteractionMode.ShiftCtrlAlternative,
                start,
                current);

            AssertCells(
                cells,
                Cell(0, 5, 0),
                Cell(0, 5, 1),
                Cell(1, 5, 0),
                Cell(1, 5, 1),
                Cell(2, 5, 0),
                Cell(2, 5, 1));
        }

        #endregion

        #region Mapping And Intents

        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.Default, 9)]
        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.ShiftAlternative, 9)]
        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.CtrlAlternative, 3)]
        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.AltAlternative, 3)]
        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.ShiftCtrlAlternative, 21)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.Default, 8)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.ShiftAlternative, 8)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.CtrlAlternative, 3)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.AltAlternative, 3)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.ShiftCtrlAlternative, 12)]
        public void Mapping_EachToolAndInteractionMode_UsesHistoricalShape(
            StructureBuildTool buildTool,
            BuildInteractionMode buildInteractionMode,
            int expectedCount)
        {
            MapCellCoord current = buildInteractionMode == BuildInteractionMode.ShiftCtrlAlternative
                ? Cell(4, 9, 4)
                : Cell(2, 9, 2);

            IReadOnlyList<MapCellCoord> cells = ResolvePlacement(
                buildTool,
                buildInteractionMode,
                Cell(0, 4, 0),
                current);

            Assert.That(cells, Has.Count.EqualTo(expectedCount));
            AssertNoDuplicates(cells);
        }

        [TestCase(StructureBuildTool.Floor)]
        [TestCase(StructureBuildTool.Wall)]
        public void StartEqualsEnd_ReturnsSingleCellForPlacementAndRemoval(StructureBuildTool buildTool)
        {
            AssertCells(
                ResolvePlacement(buildTool, BuildInteractionMode.Default, Cell(3, 4, -2), Cell(3, 99, -2)),
                Cell(3, 4, -2));

            AssertCells(
                StructureDragCellResolver.Resolve(
                    buildTool,
                    BuildOperation.Remove,
                    BuildInteractionMode.AltAlternative,
                    CursorTarget.Terrain(Cell(3, 4, -2)),
                    CursorTarget.Terrain(Cell(3, 99, -2))),
                Cell(3, 4, -2));
        }

        [Test]
        public void SameInput_ProducesSameOrderedOutput()
        {
            IReadOnlyList<MapCellCoord> first = ResolvePlacement(
                StructureBuildTool.Wall,
                BuildInteractionMode.ShiftCtrlAlternative,
                Cell(-2, 4, -2),
                Cell(2, 9, 2));

            IReadOnlyList<MapCellCoord> second = ResolvePlacement(
                StructureBuildTool.Wall,
                BuildInteractionMode.ShiftCtrlAlternative,
                Cell(-2, 4, -2),
                Cell(2, 9, 2));

            Assert.That(second, Is.EqualTo(first));
            AssertNoDuplicates(first);
        }

        [TestCase(StructureBuildTool.Floor, BuildInteractionMode.Default, 9)]
        [TestCase(StructureBuildTool.Wall, BuildInteractionMode.Default, 8)]
        public void StructureBuildContext_PlacementCreatesOneBatchIntentOfBlockRequests(
            StructureBuildTool buildTool,
            BuildInteractionMode buildInteractionMode,
            int expectedRequestCount)
        {
            var context = new StructureBuildContext(StructureDefinitions.Block, buildTool);

            bool created = context.TryCreateBuildIntent(
                CursorTarget.Terrain(Cell(0, 4, 0)),
                CursorTarget.Terrain(Cell(2, 9, 2)),
                BuildOperation.Place,
                buildInteractionMode,
                out BuildIntent intent);

            Assert.That(created, Is.True);
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Place));
            Assert.That(intent.PlacementRequests, Has.Count.EqualTo(expectedRequestCount));
            Assert.That(intent.RemovalRequests, Is.Empty);
            Assert.That(intent.PlacementRequests.All(request => ReferenceEquals(request.Definition, StructureDefinitions.Block)), Is.True);
            Assert.That(intent.PlacementRequests.Select(request => request.Anchor), Is.EqualTo(
                ResolvePlacement(buildTool, buildInteractionMode, Cell(0, 4, 0), Cell(2, 9, 2))));
        }

        [TestCase(StructureBuildTool.Floor)]
        [TestCase(StructureBuildTool.Wall)]
        public void StructureBuildContext_RemovalCreatesOneBatchIntentFromRectangleArea(StructureBuildTool buildTool)
        {
            var context = new StructureBuildContext(StructureDefinitions.Block, buildTool);

            bool created = context.TryCreateBuildIntent(
                CursorTarget.Terrain(Cell(0, 4, 0)),
                CursorTarget.Terrain(Cell(2, 9, 1)),
                BuildOperation.Remove,
                BuildInteractionMode.ShiftCtrlAlternative,
                out BuildIntent intent);

            Assert.That(created, Is.True);
            Assert.That(intent.Operation, Is.EqualTo(BuildOperation.Remove));
            Assert.That(intent.PlacementRequests, Is.Empty);
            Assert.That(intent.RemovalRequests, Has.Count.EqualTo(6));
            Assert.That(intent.RemovalRequests.Select(request => request.TargetCell), Is.EqualTo(new[]
            {
                Cell(0, 4, 0),
                Cell(0, 4, 1),
                Cell(1, 4, 0),
                Cell(1, 4, 1),
                Cell(2, 4, 0),
                Cell(2, 4, 1)
            }));
        }

        #endregion

        #region Helpers

        private static IReadOnlyList<MapCellCoord> ResolvePlacement(
            StructureBuildTool buildTool,
            BuildInteractionMode buildInteractionMode,
            MapCellCoord startCell,
            MapCellCoord currentCell)
        {
            return StructureDragCellResolver.Resolve(
                buildTool,
                BuildOperation.Place,
                buildInteractionMode,
                CursorTarget.Terrain(startCell),
                CursorTarget.Terrain(currentCell));
        }

        private static MapCellCoord Cell(int x, int y, int z)
        {
            return new MapCellCoord(x, y, z);
        }

        private static void AssertCells(
            IReadOnlyList<MapCellCoord> actual,
            params MapCellCoord[] expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
            AssertNoDuplicates(actual);
        }

        private static void AssertContains(
            IReadOnlyList<MapCellCoord> cells,
            MapCellCoord expected)
        {
            Assert.That(cells, Does.Contain(expected));
        }

        private static void AssertNoDuplicates(IReadOnlyList<MapCellCoord> cells)
        {
            Assert.That(cells.Distinct().Count(), Is.EqualTo(cells.Count));
        }

        private static void AssertSymmetricAbout(
            IReadOnlyList<MapCellCoord> cells,
            int centerX,
            int y,
            int centerZ)
        {
            foreach (MapCellCoord cell in cells)
            {
                Assert.That(cell.Y, Is.EqualTo(y));
                AssertContains(cells, Cell((2 * centerX) - cell.X, y, cell.Z));
                AssertContains(cells, Cell(cell.X, y, (2 * centerZ) - cell.Z));
                AssertContains(cells, Cell((2 * centerX) - cell.X, y, (2 * centerZ) - cell.Z));
            }
        }

        #endregion
    }
}
