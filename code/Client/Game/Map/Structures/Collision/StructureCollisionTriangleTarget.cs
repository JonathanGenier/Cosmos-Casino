using CosmosCasino.Core.Game.Map;

/// <summary>
/// Disposable Client metadata for one generated collision triangle.
/// </summary>
internal readonly struct StructureCollisionTriangleTarget
{
    #region Initialization

    /// <summary>
    /// Initializes triangle hit metadata.
    /// </summary>
    /// <param name="cell">The global occupied structure cell represented by the triangle.</param>
    /// <param name="surfaceFace">The logical surface face represented by the triangle.</param>
    internal StructureCollisionTriangleTarget(MapCellCoord cell, CursorSurfaceFace surfaceFace)
    {
        Cell = cell;
        SurfaceFace = surfaceFace;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the global occupied structure cell represented by the triangle.
    /// </summary>
    internal MapCellCoord Cell { get; }

    /// <summary>
    /// Gets the logical surface face represented by the triangle.
    /// </summary>
    internal CursorSurfaceFace SurfaceFace { get; }

    #endregion
}
