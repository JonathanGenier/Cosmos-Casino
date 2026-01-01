using CosmosCasino.Core.Console.Logging;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Central input dispatcher for the client layer.
/// <para>
/// <see cref="InputManager"/> owns the input execution pipeline and acts as the
/// single emission surface for all input-related signals.
/// </para>
/// <para>
/// Input logic itself is delegated to registered <see cref="IProcessInputModule"/>
/// implementations, which are executed in a deterministic order based on their
/// declared <see cref="InputPhase"/>.
/// </para>
/// </summary>
public sealed partial class InputManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region FIELDS

    private readonly List<IProcessInputModule> _processModules = new();
    private readonly List<IUnhandledInputModule> _unhandledInputModules = new();

    #endregion

    #region EVENTS

    /// <summary>
    /// Emitted when the debug console input intent is detected.
    /// This signal represents a semantic input decision and does not imply
    /// any specific UI or gameplay behavior.
    /// </summary>
    [Signal]
    public delegate void ToggleConsoleUiEventHandler();

    /// <summary>
    /// Emitted when a camera movement intent is detected.
    /// </summary>
    /// <param name="direction">
    /// Normalized directional vector describing camera translation intent.
    /// </param>
    [Signal]
    public delegate void MoveCameraEventHandler(Vector2 direction);

    /// <summary>
    /// Emitted when a camera rotation intent is detected.
    /// </summary>
    /// <param name="direction">
    /// Rotation direction or magnitude.
    /// </param>
    [Signal]
    public delegate void RotateCameraEventHandler(float direction);

    /// <summary>
    /// Emitted when a camera zoom intent is detected.
    /// </summary>
    /// <param name="direction">
    /// Zoom direction or magnitude.
    /// </param>
    [Signal]
    public delegate void ZoomCameraEventHandler(float direction);

    /// <summary>
    /// Emitted when the primary interaction input is pressed.
    /// This signal represents the start of a primary interaction gesture
    /// and is consumed by systems responsible for interpreting world
    /// interaction intent.
    /// </summary>
    [Signal]
    public delegate void PrimaryInteractionPressedEventHandler();

    /// <summary>
    /// Emitted when the primary interaction input is released.
    /// This signal represents the end of a primary interaction gesture
    /// and is consumed by systems responsible for finalizing world
    /// interaction intent.
    /// </summary>
    [Signal]
    public delegate void PrimaryInteractionReleasedEventHandler();

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the input pipeline.
    /// Registers all input modules and establishes a fixed execution order
    /// before the first frame is processed.
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(InputManager)))
        {
            RegisterInputModules();
        }
    }

    /// <summary>
    /// Executes all registered input modules for the current frame.
    /// Modules are processed sequentially in ascending <see cref="InputPhase"/>
    /// order. This method contains no input logic itself.
    /// </summary>
    /// <param name="delta">Frame delta time in seconds.</param>
    public override void _Process(double delta)
    {
        foreach (var module in _processModules)
        {
            module.Process(delta);
        }
    }

    /// <summary>
    /// Dispatches unhandled input events to all registered
    /// unhandled input modules in deterministic phase order.
    /// </summary>
    /// <param name="event">
    /// The unhandled input event received from the engine.
    /// </param>
    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        foreach (var module in _unhandledInputModules)
        {
            module.UnhandledInput(@event);
        }
    }

    /// <summary>
    /// Registers all input modules used by the application.
    /// Module registration is intentionally centralized and occurs only once
    /// during initialization to ensure a stable and predictable input pipeline.
    /// </summary>
    private void RegisterInputModules()
    {
        object[] modules =
        {
            new ConsoleInputModule(this),
            new CameraInputModule(ClientServices, this),
            new InteractionInputModule(ClientServices, this),
        };

        foreach (var module in modules)
        {
            if (module is IProcessInputModule processInputModule)
            {
                _processModules.Add(processInputModule);
            }

            if (module is IUnhandledInputModule unhandledInputModule)
            {
                _unhandledInputModules.Add(unhandledInputModule);
            }
        }

        _processModules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
        _unhandledInputModules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
    }

    #endregion
}
