using System.Numerics;

namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Stateless utility providing deterministic conversions between
    /// world-space coordinates and logical grid cell coordinates.
    /// </summary>
    public static class MapGridMath
    {
        #region CONST

        /// <summary>
        /// Size of a single logical grid cell expressed in world units.
        /// </summary>
        public const float CellSize = 1.0f;

        #endregion

        #region FIELDS

        /// <summary>
        /// World-space origin of the grid.
        /// </summary>
        public static readonly Vector3 Origin = Vector3.Zero;

        #endregion

        #region METHODS


        /// <summary>
        /// Converts a world-space position into a logical grid cell
        /// coordinate by projecting onto the grid plane and snapping
        /// to the containing cell.
        /// World Y is ignored; the resulting cell is always assigned
        /// to layer zero.
        /// </summary>
        /// <param name="worldX">World-space X coordinate.</param>
        /// <param name="worldY">World-space Y coordinate (ignored).</param>
        /// <param name="worldZ">World-space Z coordinate.</param>
        /// <returns>
        /// The grid cell containing the specified world position.
        /// </returns>
        public static CellCoord WorldToCell(float worldX, float worldY, float worldZ)
        {
            float localX = worldX - Origin.X;
            float localZ = worldZ - Origin.Z;

            int cellX = (int)MathF.Floor(localX / CellSize);
            int cellY = (int)MathF.Floor(localZ / CellSize);
            int cellZ = 0;

            return new CellCoord(cellX, cellY, cellZ);
        }

        /// <summary>
        /// Converts a logical grid cell coordinate into the world-space
        /// position of the cell center.
        /// The returned position lies on the grid plane at ground level
        /// and is suitable for spawning or previewing buildable objects.
        /// </summary>
        /// <param name="cell">
        /// The logical grid cell to convert.
        /// </param>
        /// <returns>
        /// World-space position representing the center of the specified cell.
        /// </returns>
        public static Vector3 CellToWorld(CellCoord cell)
        {
            return new Vector3(
                (cell.X * CellSize) + (CellSize * 0.5f),
                0f,
                (cell.Y * CellSize) + (CellSize * 0.5f)
            ) + Origin;
        }

        #endregion
    }
}
