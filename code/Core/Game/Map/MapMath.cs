namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Authoritative map coordinate conversion utilities. Existing world/cell conversion methods operate on the
    /// legacy horizontal <see cref="MapCoord"/> used by current terrain and build APIs. Chunk ownership methods
    /// operate on global <see cref="MapCellCoord"/> values where X/Z are horizontal and Y is vertical.
    /// </summary>
    public static class MapMath
    {
        #region World To Cell

        /// <summary>
        /// Returns the map cell containing the specified horizontal world position.
        /// Uses half-open centered cell bounds so boundary ties resolve to the cell with the greater coordinate.
        /// </summary>
        /// <param name="position">
        /// The horizontal world position to resolve.
        /// </param>
        /// <returns>
        /// The map cell containing <paramref name="position"/>.
        /// </returns>
        public static MapCoord WorldToCell(WorldCoord position)
        {
            return WorldToCell(position.X, position.Y);
        }

        /// <summary>
        /// Returns the map cell containing the specified horizontal world coordinates.
        /// Uses half-open centered cell bounds so boundary ties resolve to the cell with the greater coordinate.
        /// </summary>
        /// <param name="worldX">
        /// Horizontal world X coordinate.
        /// </param>
        /// <param name="worldY">
        /// Horizontal world Y coordinate.
        /// </param>
        /// <returns>
        /// The map cell containing the specified world coordinates.
        /// </returns>
        public static MapCoord WorldToCell(float worldX, float worldY)
        {
            return new MapCoord(
                FloorToInt((worldX + WorldGridMetrics.HalfGridUnitSize) / WorldGridMetrics.GridUnitSize),
                FloorToInt((worldY + WorldGridMetrics.HalfGridUnitSize) / WorldGridMetrics.GridUnitSize)
            );
        }

        /// <summary>
        /// Returns the vertical map-cell layer containing the specified world-space height.
        /// Uses half-open base-plane bounds so each cell starts at its base plane and extends by one vertical grid unit.
        /// </summary>
        /// <param name="worldY">
        /// World-space height to resolve.
        /// </param>
        /// <returns>
        /// The vertical map-cell layer containing <paramref name="worldY"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="worldY"/> is non-finite or cannot resolve to an <see cref="int"/> layer.
        /// </exception>
        public static int WorldToCellY(float worldY)
        {
            if (!TryWorldToCellY(worldY, out int cellY))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(worldY),
                    worldY,
                    "World-space height must be finite and resolve to a representable map-cell Y coordinate.");
            }

            return cellY;
        }

        /// <summary>
        /// Attempts to return the vertical map-cell layer containing the specified world-space height.
        /// </summary>
        /// <param name="worldY">
        /// World-space height to resolve.
        /// </param>
        /// <param name="cellY">
        /// The resolved vertical map-cell layer, when successful.
        /// </param>
        /// <returns>
        /// <c>true</c> when <paramref name="worldY"/> resolves to a representable vertical layer; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryWorldToCellY(float worldY, out int cellY)
        {
            if (!float.IsFinite(worldY))
            {
                cellY = default;
                return false;
            }

            double scaled = (double)worldY / WorldGridMetrics.VerticalGridUnitSize;
            double floored = Math.Floor(scaled);

            if (floored < int.MinValue || floored > int.MaxValue)
            {
                cellY = default;
                return false;
            }

            cellY = (int)floored;
            return true;
        }

        #endregion

        #region Cell To World

        /// <summary>
        /// Returns the world-space origin of a map cell.
        /// For cell (x, y), this is the inclusive minimum corner (x - 0.5, y - 0.5).
        /// </summary>
        /// <param name="cell">
        /// The map cell to resolve.
        /// </param>
        /// <returns>
        /// The world-space origin of <paramref name="cell"/>.
        /// </returns>
        public static WorldCoord CellToWorldOrigin(MapCoord cell)
        {
            return new WorldCoord(
                (cell.X * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize,
                (cell.Y * WorldGridMetrics.GridUnitSize) - WorldGridMetrics.HalfGridUnitSize
            );
        }

        /// <summary>
        /// Returns the world-space center of a map cell.
        /// For cell (x, y), this is (x, y).
        /// </summary>
        /// <param name="cell">
        /// The map cell to resolve.
        /// </param>
        /// <returns>
        /// The world-space center of <paramref name="cell"/>.
        /// </returns>
        public static WorldCoord CellToWorldCenter(MapCoord cell)
        {
            return new WorldCoord(
                cell.X * WorldGridMetrics.GridUnitSize,
                cell.Y * WorldGridMetrics.GridUnitSize
            );
        }

        /// <summary>
        /// Returns the world-space vertical base plane for a global logical map-cell Y coordinate.
        /// </summary>
        /// <param name="cellY">The global logical vertical map-cell coordinate.</param>
        /// <returns>The world-space vertical base plane for <paramref name="cellY"/>.</returns>
        public static float CellYToWorldBasePlane(int cellY)
        {
            return cellY * WorldGridMetrics.VerticalGridUnitSize;
        }

        /// <summary>
        /// Returns the world-space vertical center for a global logical map-cell Y coordinate.
        /// </summary>
        /// <param name="cellY">The global logical vertical map-cell coordinate.</param>
        /// <returns>The world-space vertical center for <paramref name="cellY"/>.</returns>
        public static float CellYToWorldCenter(int cellY)
        {
            return CellYToWorldBasePlane(cellY) + WorldGridMetrics.HalfVerticalGridUnitSize;
        }

        #endregion

        #region Cell To Chunk

        /// <summary>
        /// Resolves the authoritative map chunk owning the specified global logical cell coordinate.
        /// Only the cell's horizontal X/Z coordinates determine chunk ownership.
        /// </summary>
        /// <param name="cell">The global logical cell coordinate to resolve.</param>
        /// <returns>The global X/Z map chunk coordinate that owns <paramref name="cell"/>.</returns>
        public static MapChunkCoord CellToChunk(MapCellCoord cell)
        {
            return GlobalToChunk(cell.X, cell.Z);
        }

        /// <summary>
        /// Resolves the authoritative map chunk owning the specified global horizontal X/Z coordinates.
        /// </summary>
        /// <param name="x">Global horizontal X coordinate.</param>
        /// <param name="z">Global horizontal Z coordinate.</param>
        /// <returns>The global X/Z map chunk coordinate that owns the specified horizontal position.</returns>
        public static MapChunkCoord GlobalToChunk(int x, int z)
        {
            return new MapChunkCoord(
                FloorDivide(x, MapChunkMetrics.ChunkSize),
                FloorDivide(z, MapChunkMetrics.ChunkSize));
        }

        /// <summary>
        /// Resolves the chunk-local X/Z coordinate of a global logical cell coordinate.
        /// The vertical Y component does not affect the returned local coordinate.
        /// </summary>
        /// <param name="cell">The global logical cell coordinate to resolve.</param>
        /// <returns>The zero-based X/Z coordinate local to the owning map chunk.</returns>
        public static MapChunkLocalCoord CellToChunkLocal(MapCellCoord cell)
        {
            return GlobalToChunkLocal(cell.X, cell.Z);
        }

        /// <summary>
        /// Resolves the chunk-local X/Z coordinate of global horizontal X/Z coordinates.
        /// </summary>
        /// <param name="x">Global horizontal X coordinate.</param>
        /// <param name="z">Global horizontal Z coordinate.</param>
        /// <returns>The zero-based X/Z coordinate local to the owning map chunk.</returns>
        public static MapChunkLocalCoord GlobalToChunkLocal(int x, int z)
        {
            return new MapChunkLocalCoord(
                PositiveModulo(x, MapChunkMetrics.ChunkSize),
                PositiveModulo(z, MapChunkMetrics.ChunkSize));
        }

        /// <summary>
        /// Resolves a global logical cell coordinate from a map chunk coordinate, chunk-local coordinate, and vertical level.
        /// <see cref="MapChunkCoord"/> stores signed <see cref="int"/> chunk coordinates, but not every possible
        /// chunk/local pair can be represented as an <see cref="int"/>-backed <see cref="MapCellCoord"/> after
        /// scaling by <see cref="MapChunkMetrics.ChunkSize"/>. Values outside the representable global cell range are
        /// rejected instead of silently overflowing.
        /// </summary>
        /// <param name="chunk">The global X/Z map chunk coordinate.</param>
        /// <param name="local">The zero-based X/Z coordinate inside <paramref name="chunk"/>.</param>
        /// <param name="y">The global vertical Y coordinate.</param>
        /// <returns>The global logical cell coordinate represented by the chunk/local pair and vertical level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the resolved global X or Z coordinate cannot be represented by <see cref="MapCellCoord"/>.
        /// </exception>
        public static MapCellCoord ChunkLocalToCell(MapChunkCoord chunk, MapChunkLocalCoord local, int y)
        {
            return new MapCellCoord(
                ResolveRepresentableGlobalCellAxis(chunk.X, local.X, nameof(chunk)),
                y,
                ResolveRepresentableGlobalCellAxis(chunk.Z, local.Z, nameof(chunk)));
        }

        #endregion

        #region Helpers

        private static int FloorToInt(float value)
        {
            return (int)MathF.Floor(value);
        }

        private static int FloorDivide(int value, int divisor)
        {
            if (divisor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(divisor), divisor, "Divisor must be positive.");
            }

            int quotient = value / divisor;
            int remainder = value % divisor;

            if (remainder != 0 && value < 0)
            {
                quotient--;
            }

            return quotient;
        }

        private static int PositiveModulo(int value, int modulus)
        {
            if (modulus <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(modulus), modulus, "Modulus must be positive.");
            }

            int remainder = value % modulus;
            return remainder < 0 ? remainder + modulus : remainder;
        }

        private static int ResolveRepresentableGlobalCellAxis(int chunkAxis, int localAxis, string paramName)
        {
            long value = ((long)chunkAxis * MapChunkMetrics.ChunkSize) + localAxis;

            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    paramName,
                    value,
                    $"Resolved global cell coordinate must be within [{int.MinValue}, {int.MaxValue}].");
            }

            return (int)value;
        }

        #endregion
    }
}
