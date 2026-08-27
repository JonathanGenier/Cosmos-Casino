/// <summary>
/// Identifies the Client-owned physical grid used to render footprint preview cells.
/// </summary>
internal enum BuildPreviewCellGeometry
{
    /// <summary>
    /// Uses the existing general world-grid preview volume.
    /// </summary>
    WorldGrid = 0,

    /// <summary>
    /// Uses the canonical one-unit structural preview volume.
    /// </summary>
    StructureGrid = 1
}
