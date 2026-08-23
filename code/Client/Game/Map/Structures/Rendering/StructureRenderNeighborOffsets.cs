using CosmosCasino.Core.Game.Map;

/// <summary>
/// Shared six-axis logical neighbor offsets used by generated structure rendering.
/// </summary>
internal static class StructureRenderNeighborOffsets
{
    /// <summary>
    /// The six orthogonal global-cell neighbors that can affect full-cell face exposure.
    /// </summary>
    internal static readonly MapCellOffset[] SixAxis =
    {
        new(0, 1, 0),
        new(0, -1, 0),
        new(0, 0, -1),
        new(0, 0, 1),
        new(1, 0, 0),
        new(-1, 0, 0)
    };
}
