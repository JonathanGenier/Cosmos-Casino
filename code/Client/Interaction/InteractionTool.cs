/// <summary>
/// Defines the active interaction tool used to interpret primary
/// world interaction gestures.
/// The selected tool determines how gesture input is translated into
/// selection, building, or other tool-specific world actions.
/// </summary>
public enum InteractionTool
{
    /// <summary>
    /// Default interaction tool used for selecting and inspecting
    /// world elements without modifying them.
    /// </summary>
    Selection,

    /// <summary>
    /// Interaction tool used for placing, previewing, and modifying
    /// buildable world elements.
    /// </summary>
    Build,
}
