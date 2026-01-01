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

    /// <summary>
    /// Input phase reserved for camera-related input intent.
    /// Modules in this phase are responsible for detecting player camera controls
    /// such as movement, rotation, and zoom, and emitting semantic camera intent
    /// without directly manipulating camera state.
    /// Camera input is processed after debug and system-level input, but before
    /// gameplay and UI interaction phases.
    /// </summary>
    Camera = 1,

    /// <summary>
    /// Input phase reserved for primary world interaction intent.
    /// Modules in this phase are responsible for translating player input
    /// into interaction gestures such as selection, building, or other
    /// tool-driven world interactions, without executing gameplay logic
    /// directly.
    /// Interaction input is processed after camera input and before any
    /// higher-level gameplay or command resolution systems.
    /// </summary>
    Interaction = 2,
}
