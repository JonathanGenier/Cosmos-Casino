/// <summary>
/// Describes how a build cancellation input should be applied by build interaction flows.
/// </summary>
public enum BuildCancellationScope
{
    /// <summary>
    /// Cancels only the active build gesture while keeping the selected build context.
    /// </summary>
    ActiveBuild,

    /// <summary>
    /// Cancels the selected build context and any active build gesture.
    /// </summary>
    BuildContext
}
