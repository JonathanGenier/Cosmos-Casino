/// <summary>
/// Defines execution order for input modules.
/// Input modules are processed in ascending order based on their assigned
/// <see cref="InputPhase"/> value. Lower values are executed earlier in the
/// frame.
/// </summary>
public enum InputPhase
{
    /// <summary>
    /// Highest-priority phase reserved for debug and system-level input.
    /// Input in this phase is processed before all other input and is intended
    /// for developer tools, diagnostics, and non-gameplay controls.
    /// </summary>
    Debug = 0,
}
