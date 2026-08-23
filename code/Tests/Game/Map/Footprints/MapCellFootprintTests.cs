using CosmosCasino.Core.Game.Map;
using NUnit.Framework;

namespace CosmosCasino.Tests.Game.Map
{
    [TestFixture]
    internal sealed class MapCellFootprintTests
    {
        #region Construction

        [Test]
        public void Constructor_NullOffsets_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MapCellFootprint(null!));
        }

        [Test]
        public void Constructor_EmptyOffsets_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new MapCellFootprint(Array.Empty<MapCellOffset>()));
        }

        [Test]
        public void Constructor_DuplicateOffsets_ThrowsArgumentException()
        {
            var offsets = new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(0, 0, 0)
            };

            Assert.Throws<ArgumentException>(() =>
                new MapCellFootprint(offsets));
        }

        [Test]
        public void Constructor_CopiesOffsetsAndExposesReadOnlyView()
        {
            var offsets = new List<MapCellOffset>
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0)
            };

            var footprint = new MapCellFootprint(offsets);
            offsets[0] = new MapCellOffset(99, 99, 99);
            var exposedList = (IList<MapCellOffset>)footprint.Offsets;

            Assert.That(
                footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(1, 0, 0)
                }));
            Assert.Throws<NotSupportedException>(() =>
                exposedList[0] = new MapCellOffset(5, 5, 5));
        }

        [Test]
        public void Constructor_SortsOffsetsByCanonicalXyzOrder()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(-1, 4, 0),
                new MapCellOffset(-1, -1, 3),
                new MapCellOffset(-1, -1, -1),
                new MapCellOffset(0, 0, 0)
            });

            Assert.That(
                footprint.Offsets,
                Is.EqualTo(new[]
                {
                    new MapCellOffset(-1, -1, -1),
                    new MapCellOffset(-1, -1, 3),
                    new MapCellOffset(-1, 4, 0),
                    new MapCellOffset(0, 0, 0),
                    new MapCellOffset(2, 0, 0)
                }));
        }

        #endregion

        #region Resolution

        [Test]
        public void Resolve_SingleCellFootprint_ReturnsAnchor()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0)
            });
            var anchor = new MapCellCoord(-4, 3, 7);

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(resolved, Is.EqualTo(new[] { anchor }));
        }

        [Test]
        public void Resolve_RectangularFootprint_ReturnsTwoByOneByThreeCellsInCanonicalOrder()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(1, 0, 2),
                new MapCellOffset(0, 0, 1),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(0, 0, 2),
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 1)
            });
            var anchor = new MapCellCoord(10, 2, -10);

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(10, 2, -10),
                    new MapCellCoord(10, 2, -9),
                    new MapCellCoord(10, 2, -8),
                    new MapCellCoord(11, 2, -10),
                    new MapCellCoord(11, 2, -9),
                    new MapCellCoord(11, 2, -8)
                }));
        }

        [Test]
        public void Resolve_NonRectangularFootprint_ReturnsCellsInCanonicalOffsetOrder()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(0, 1, 1),
                new MapCellOffset(0, 0, 0)
            });
            var anchor = new MapCellCoord(-3, 5, 4);

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(-3, 5, 4),
                    new MapCellCoord(-3, 6, 5),
                    new MapCellCoord(-1, 5, 4)
                }));
        }

        [Test]
        public void Resolve_Deg0_LeavesOffsetsUnchanged()
        {
            var footprint = LFootprint();

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(Origin(), FootprintRotation.Deg0);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(0, 0, 0),
                    new MapCellCoord(1, 0, 0),
                    new MapCellCoord(1, 0, 1)
                }));
        }

        [Test]
        public void Resolve_Deg90_RotatesOffsetsAroundVerticalAxis()
        {
            var footprint = LFootprint();

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(Origin(), FootprintRotation.Deg90);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(0, 0, 0),
                    new MapCellCoord(0, 0, -1),
                    new MapCellCoord(1, 0, -1)
                }));
        }

        [Test]
        public void Resolve_Deg180_RotatesOffsetsAroundVerticalAxis()
        {
            var footprint = LFootprint();

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(Origin(), FootprintRotation.Deg180);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(0, 0, 0),
                    new MapCellCoord(-1, 0, 0),
                    new MapCellCoord(-1, 0, -1)
                }));
        }

        [Test]
        public void Resolve_Deg270_RotatesOffsetsAroundVerticalAxis()
        {
            var footprint = LFootprint();

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(Origin(), FootprintRotation.Deg270);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(0, 0, 0),
                    new MapCellCoord(0, 0, 1),
                    new MapCellCoord(-1, 0, 1)
                }));
        }

        [Test]
        public void Rotate_FourQuarterTurns_ReturnsOriginalOffsets()
        {
            var offsets = new[]
            {
                new MapCellOffset(-2, 1, 3),
                new MapCellOffset(0, -1, 0),
                new MapCellOffset(4, 2, -5)
            };

            MapCellOffset[] rotated = offsets
                .Select(offset => offset
                    .Rotate(FootprintRotation.Deg90)
                    .Rotate(FootprintRotation.Deg90)
                    .Rotate(FootprintRotation.Deg90)
                    .Rotate(FootprintRotation.Deg90))
                .ToArray();

            Assert.That(rotated, Is.EqualTo(offsets));
        }

        [Test]
        public void Resolve_RotationPreservesVerticalOffsets()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(1, -2, 0),
                new MapCellOffset(1, 3, 1)
            });
            var anchor = new MapCellCoord(5, 10, -4);

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(anchor, FootprintRotation.Deg90);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(5, 8, -5),
                    new MapCellCoord(6, 13, -5)
                }));
        }

        [Test]
        public void Resolve_NegativeAnchorAndMixedOffsets_ReturnsRepresentableCells()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(-2, 1, 3),
                new MapCellOffset(1, -1, -4)
            });
            var anchor = new MapCellCoord(-10, 0, -10);

            IReadOnlyList<MapCellCoord> resolved = footprint.Resolve(anchor, FootprintRotation.Deg0);

            Assert.That(
                resolved,
                Is.EqualTo(new[]
                {
                    new MapCellCoord(-12, 1, -7),
                    new MapCellCoord(-9, -1, -14)
                }));
        }

        [Test]
        public void Resolve_AnchorAdditionOverflow_ThrowsArgumentOutOfRangeException()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(1, 0, 0)
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                footprint.Resolve(new MapCellCoord(int.MaxValue, 0, 0), FootprintRotation.Deg0));
        }

        [Test]
        public void Resolve_RotationNegationOverflow_ThrowsArgumentOutOfRangeException()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(int.MinValue, 0, 0)
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                footprint.Resolve(Origin(), FootprintRotation.Deg90));
        }

        [Test]
        public void Resolve_RepeatedCalls_ReturnSameCoordinateOrder()
        {
            var footprint = new MapCellFootprint(new[]
            {
                new MapCellOffset(2, 0, 0),
                new MapCellOffset(0, 0, 2),
                new MapCellOffset(1, 0, 1)
            });
            var anchor = new MapCellCoord(-7, 4, 9);

            IReadOnlyList<MapCellCoord> first = footprint.Resolve(anchor, FootprintRotation.Deg270);
            IReadOnlyList<MapCellCoord> second = footprint.Resolve(anchor, FootprintRotation.Deg270);

            Assert.That(second, Is.EqualTo(first));
        }

        #endregion

        #region Helpers

        private static MapCellCoord Origin()
        {
            return new MapCellCoord(0, 0, 0);
        }

        private static MapCellFootprint LFootprint()
        {
            return new MapCellFootprint(new[]
            {
                new MapCellOffset(0, 0, 0),
                new MapCellOffset(1, 0, 0),
                new MapCellOffset(1, 0, 1)
            });
        }

        #endregion
    }
}
