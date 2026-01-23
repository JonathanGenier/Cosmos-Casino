/// <summary>
/// Defines the active interaction variant used during a build operation,
/// derived from the current modifier key state and used to alter how build
/// previews and placement logic are generated.
/// </summary>
public enum BuildInteractionMode
{
    /// <summary>
    /// Default build interaction with no modifier keys applied.
    /// </summary>
    Default,

    /// <summary>
    /// Alternative build interaction triggered when the Shift modifier is held.
    /// </summary>
    ShiftAlternative,

    /// <summary>
    /// Alternative build interaction triggered when the Ctrl modifier is held.
    /// </summary>
    CtrlAlternative,

    /// <summary>
    /// Alternative build interaction triggered when the Alt modifier is held.
    /// </summary>
    AltAlternative,

    /// <summary>
    /// Alternative build interaction triggered when both Shift and Ctrl modifiers
    /// are held simultaneously.
    /// </summary>
    ShiftCtrlAlternative,
}