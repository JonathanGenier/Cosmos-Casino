/// <summary>
/// Describes the visual validity state for Client-only build preview cells.
/// </summary>
internal enum BuildPreviewValidity
{
    /// <summary>
    /// The previewed operation is expected to be accepted by Core.
    /// </summary>
    Valid,

    /// <summary>
    /// The previewed operation would not mutate authoritative state.
    /// </summary>
    NoOp,

    /// <summary>
    /// The previewed operation is expected to be rejected by Core.
    /// </summary>
    Invalid
}
