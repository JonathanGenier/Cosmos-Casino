using CosmosCasino.Core.Debug.Logging;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Central input dispatcher for the client layer.
/// <para>
/// <see cref="InputManager"/> owns the input execution pipeline and acts as the
/// single emission surface for all input-related signals.
/// </para>
/// <para>
/// Input logic itself is delegated to registered <see cref="IInputModule"/>
/// implementations, which are executed in a deterministic order based on their
/// declared <see cref="InputPhase"/>.
/// </para>
/// </summary>
public partial class InputManager : Node
{
    #region FIELDS

    /// <summary>
    /// Ordered collection of input modules executed every frame.
    /// Modules are sorted once during initialization based on their
    /// <see cref="IInputModule.Phase"/> value.
    /// </summary>
    private readonly List<IInputModule> _modules = new();

    #endregion

    #region SIGNALS

    /// <summary>
    /// Emitted when the debug console input intent is detected.
    /// This signal represents a semantic input decision and does not imply
    /// any specific UI or gameplay behavior.
    /// </summary>
    [Signal]
    public delegate void ToggleConsoleUiEventHandler();

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Initializes the input pipeline.
    /// Registers all input modules and establishes a fixed execution order
    /// before the first frame is processed.
    /// </summary>
    public override void _Ready()
    {
        DevLog.System("InputManager", "Setting up...");
        RegisterModules();
        SortModules();
        DevLog.System("InputManager", "Ready");
    }

    /// <summary>
    /// Executes all registered input modules for the current frame.
    /// Modules are processed sequentially in ascending <see cref="InputPhase"/>
    /// order. This method contains no input logic itself.
    /// </summary>
    /// <param name="delta">Frame delta time in seconds.</param>
    public override void _Process(double delta)
    {
        foreach (var module in _modules)
        {
            module.Process(delta);
        }
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Registers all input modules used by the application.
    /// Module registration is intentionally centralized and occurs only once
    /// during initialization to ensure a stable and predictable input pipeline.
    /// </summary>
    private void RegisterModules()
    {
        _modules.Add(new DebugInputModule(this));
    }

    /// <summary>
    /// Sorts registered input modules by execution priority.
    /// Lower <see cref="InputPhase"/> values are executed earlier in the frame.
    /// This ordering is fixed after initialization and is not modified at runtime.
    /// </summary>
    private void SortModules()
    {
        _modules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
    }

    #endregion
}
