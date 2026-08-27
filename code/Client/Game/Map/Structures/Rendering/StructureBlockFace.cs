using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Client-side generated geometry definition for one exposed Block face.
/// </summary>
internal readonly struct StructureBlockFace
{
    #region Constants

    /// <summary>
    /// Half of one generated Block's horizontal width in Godot units.
    /// </summary>
    internal const float HalfWidth = StructureGridMetrics.HalfCellSize;

    /// <summary>
    /// Half of one generated Block's vertical height in Godot units.
    /// </summary>
    internal const float HalfHeight = StructureGridMetrics.HalfCellSize;

    /// <summary>
    /// Half of one generated Block's horizontal depth in Godot units.
    /// </summary>
    internal const float HalfDepth = StructureGridMetrics.HalfCellSize;

    #endregion

    #region Fields

    /// <summary>
    /// All six cube faces in deterministic top, bottom, north, south, east, west order.
    /// </summary>
    internal static readonly StructureBlockFace[] All =
    {
        Top(),
        Bottom(),
        North(),
        South(),
        East(),
        West()
    };

    #endregion

    #region Initialization

    private StructureBlockFace(
        CursorSurfaceFace surfaceFace,
        MapCellOffset neighborOffset,
        Vector3 normal,
        Vector3[] corners)
    {
        SurfaceFace = surfaceFace;
        NeighborOffset = neighborOffset;
        Normal = normal;
        Corners = corners;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the logical surface face represented by this generated face.
    /// </summary>
    internal CursorSurfaceFace SurfaceFace { get; }

    /// <summary>
    /// Gets the global logical neighbor tested for this face.
    /// </summary>
    internal MapCellOffset NeighborOffset { get; }

    /// <summary>
    /// Gets the outward Godot normal for this face.
    /// </summary>
    internal Vector3 Normal { get; }

    /// <summary>
    /// Gets the section-local face corners in Godot clockwise winding order.
    /// </summary>
    internal Vector3[] Corners { get; }

    #endregion

    #region Factories

    private static StructureBlockFace Top()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.Top,
            new MapCellOffset(0, 1, 0),
            Vector3.Up,
            new[]
            {
                new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, HalfHeight, HalfDepth),
                new Vector3(-HalfWidth, HalfHeight, HalfDepth)
            });
    }

    private static StructureBlockFace Bottom()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.Bottom,
            new MapCellOffset(0, -1, 0),
            Vector3.Down,
            new[]
            {
                new Vector3(-HalfWidth, -HalfHeight, -HalfDepth),
                new Vector3(-HalfWidth, -HalfHeight, HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, -HalfDepth)
            });
    }

    private static StructureBlockFace North()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.North,
            new MapCellOffset(0, 0, -1),
            Vector3.Forward,
            new[]
            {
                new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                new Vector3(-HalfWidth, -HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, HalfHeight, -HalfDepth)
            });
    }

    private static StructureBlockFace South()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.South,
            new MapCellOffset(0, 0, 1),
            Vector3.Back,
            new[]
            {
                new Vector3(-HalfWidth, HalfHeight, HalfDepth),
                new Vector3(HalfWidth, HalfHeight, HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                new Vector3(-HalfWidth, -HalfHeight, HalfDepth)
            });
    }

    private static StructureBlockFace East()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.East,
            new MapCellOffset(1, 0, 0),
            Vector3.Right,
            new[]
            {
                new Vector3(HalfWidth, HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, -HalfDepth),
                new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                new Vector3(HalfWidth, HalfHeight, HalfDepth)
            });
    }

    private static StructureBlockFace West()
    {
        return new StructureBlockFace(
            CursorSurfaceFace.West,
            new MapCellOffset(-1, 0, 0),
            Vector3.Left,
            new[]
            {
                new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                new Vector3(-HalfWidth, HalfHeight, HalfDepth),
                new Vector3(-HalfWidth, -HalfHeight, HalfDepth),
                new Vector3(-HalfWidth, -HalfHeight, -HalfDepth)
            });
    }

    #endregion
}
