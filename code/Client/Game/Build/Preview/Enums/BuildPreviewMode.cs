/// <summary>
/// Defines the active visualization mode used by the build preview system,
/// determining how placement feedback is rendered to the player.
/// </summary>
public enum BuildPreviewMode
{
    /// <summary>
    /// Cursor-based preview mode where feedback is shown for a single cell
    /// under the cursor without committing or spanning multiple cells.
    /// </summary>
    Cursor,

    /// <summary>
    /// Drag-based preview mode where feedback is shown across multiple cells
    /// during an active drag or build operation prior to commitment.
    /// </summary>
    Drag,
}
