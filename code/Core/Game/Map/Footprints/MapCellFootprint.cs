using System.Collections.ObjectModel;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Immutable reusable collection of map-cell offsets that defines a multi-cell footprint.
    /// </summary>
    internal sealed class MapCellFootprint
    {
        #region Fields

        private readonly ReadOnlyCollection<MapCellOffset> _offsets;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new footprint from the specified unique offsets.
        /// </summary>
        /// <param name="offsets">The offsets occupied by the footprint relative to an anchor coordinate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="offsets"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="offsets"/> is empty or contains duplicate offsets.
        /// </exception>
        internal MapCellFootprint(IEnumerable<MapCellOffset> offsets)
        {
            ArgumentNullException.ThrowIfNull(offsets);

            List<MapCellOffset> sortedOffsets = offsets.ToList();

            if (sortedOffsets.Count == 0)
            {
                throw new ArgumentException("A footprint must contain at least one offset.", nameof(offsets));
            }

            sortedOffsets.Sort(CompareOffsets);

            for (int i = 1; i < sortedOffsets.Count; i++)
            {
                if (sortedOffsets[i] == sortedOffsets[i - 1])
                {
                    throw new ArgumentException(
                        $"A footprint cannot contain duplicate offset {sortedOffsets[i]}.",
                        nameof(offsets));
                }
            }

            _offsets = sortedOffsets.AsReadOnly();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the canonical offsets in deterministic X/Y/Z sort order.
        /// </summary>
        internal IReadOnlyList<MapCellOffset> Offsets => _offsets;

        #endregion

        #region Resolution

        /// <summary>
        /// Resolves this footprint into global map-cell coordinates around the specified anchor.
        /// </summary>
        /// <param name="anchor">The global map-cell coordinate used as the footprint origin.</param>
        /// <param name="rotation">The quarter-turn rotation to apply before anchoring offsets.</param>
        /// <returns>The resolved global coordinates in canonical footprint order.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when rotation or anchor addition would exceed the representable map-cell range.
        /// </exception>
        internal IReadOnlyList<MapCellCoord> Resolve(MapCellCoord anchor, FootprintRotation rotation)
        {
            var resolved = new MapCellCoord[_offsets.Count];

            for (int i = 0; i < _offsets.Count; i++)
            {
                MapCellOffset rotatedOffset = _offsets[i].Rotate(rotation);
                resolved[i] = ResolveCoordinate(anchor, rotatedOffset);
            }

            return Array.AsReadOnly(resolved);
        }

        #endregion

        #region Helpers

        private static int CompareOffsets(MapCellOffset left, MapCellOffset right)
        {
            int xComparison = left.X.CompareTo(right.X);

            if (xComparison != 0)
            {
                return xComparison;
            }

            int yComparison = left.Y.CompareTo(right.Y);

            if (yComparison != 0)
            {
                return yComparison;
            }

            return left.Z.CompareTo(right.Z);
        }

        private static MapCellCoord ResolveCoordinate(MapCellCoord anchor, MapCellOffset offset)
        {
            return new MapCellCoord(
                AddAxis(anchor.X, offset.X, nameof(anchor)),
                AddAxis(anchor.Y, offset.Y, nameof(anchor)),
                AddAxis(anchor.Z, offset.Z, nameof(anchor)));
        }

        private static int AddAxis(int anchorAxis, int offsetAxis, string paramName)
        {
            long value = (long)anchorAxis + offsetAxis;

            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    value,
                    $"Resolved map-cell coordinates must be within [{int.MinValue}, {int.MaxValue}].");
            }

            return (int)value;
        }

        #endregion
    }
}
