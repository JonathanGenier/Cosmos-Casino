using CosmosCasino.Core.Console.Logging;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the application's input pipeline by coordinating input modules, processing input events, and emitting
/// high-level input signals for game systems and UI components.
/// </summary>
/// <remarks>The InputManager centralizes input handling by registering and executing input modules in a defined
/// order. It emits semantic signals for common input intents, such as camera movement or interaction, allowing other
/// systems to respond without coupling to low-level input details. Input modules are registered once during
/// initialization to ensure consistent processing. This class is typically used as a singleton or root node to provide
/// a unified input interface across the application.</remarks>
public sealed partial class InputManager : Node
{
    #region Fields

    private readonly List<IInputModule> _modules = new();
    private readonly List<IProcessInputModule> _processModules = new();
    private readonly List<IUnhandledInputModule> _unhandledInputModules = new();

    #endregion

    #region Signals

    /// <summary>
    /// Represents the method that handles a request to toggle the visibility of the console user interface.
    /// </summary>
    [Signal]
    public delegate void ToggleConsoleUiEventHandler();

    /// <summary>
    /// Represents the method that handles camera movement events by specifying a direction vector.
    /// </summary>
    /// <param name="direction">The direction and magnitude of the camera movement, represented as a two-dimensional vector. Each component
    /// indicates movement along the corresponding axis.</param>
    [Signal]
    public delegate void MoveCameraEventHandler(Vector2 direction);

    /// <summary>
    /// Represents the method that handles a camera rotation event, providing the direction of rotation.
    /// </summary>
    /// <param name="direction">The direction in which to rotate the camera. Positive values indicate rotation to the right; negative values
    /// indicate rotation to the left.</param>
    [Signal]
    public delegate void RotateCameraEventHandler(float direction);

    /// <summary>
    /// Represents the method that handles a camera zoom event, providing the zoom direction and magnitude.
    /// </summary>
    /// <param name="direction">The amount and direction of the zoom. Positive values indicate zooming in; negative values indicate zooming out.</param>
    [Signal]
    public delegate void ZoomCameraEventHandler(float direction);

    /// <summary>
    /// Represents the method that handles the event when the primary interaction (Mouse Left Click) is pressed.
    /// </summary>
    /// <remarks>This delegate is typically used to subscribe to events triggered by a primary user action,
    /// such as a unit selection or build input gesture. The event does not provide additional event data.</remarks>
    [Signal]
    public delegate void PrimaryInteractionPressedEventHandler();

    /// <summary>
    /// Emitted when the primary interaction input is released (for example: left mouse button or primary touch).
    /// This signal marks the completion of a primary interaction gesture and indicates that any ongoing selection,
    /// drag, or build action should be finalized or cancelled by listeners.
    /// </summary>
    [Signal]
    public delegate void PrimaryInteractionReleasedEventHandler();

    #endregion

    #region Godot Process

    /// <summary>
    /// Called when the node is added to the scene for the first time. Performs initialization logic for the input
    /// manager.
    /// </summary>
    /// <remarks>This method is invoked automatically by the Godot engine when the node enters the scene tree.
    /// Override this method to set up input modules or perform other startup tasks specific to the input
    /// manager.</remarks>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(InputManager)))
        {
            RegisterInputModules();
        }
    }

    /// <summary>
    /// Updates all enabled process modules for the current frame.
    /// </summary>
    /// <remarks>Only modules that are enabled are processed during each call. This method is typically called
    /// once per frame by the engine.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. Used to update module logic based on frame timing.</param>
    public override void _Process(double delta)
    {
        foreach (var module in _processModules)
        {
            if (!module.IsEnabled)
            {
                continue;
            }

            module.Process(delta);
        }
    }

    /// <summary>
    /// Handles input events that were not consumed by other input handlers.
    /// </summary>
    /// <remarks>This method is typically called by the engine when an input event is not handled by any other
    /// input handler. It delegates the unhandled input event to all enabled modules registered for unhandled input
    /// processing. Override this method to customize how unhandled input events are processed in your
    /// application.</remarks>
    /// <param name="event">The input event that was not handled by previous input processing. Cannot be null.</param>
    public override void _UnhandledInput(InputEvent @event)
    {
        foreach (var module in _unhandledInputModules)
        {
            if (!module.IsEnabled)
            {
                continue;
            }

            module.UnhandledInput(@event);
        }
    }

    #endregion

    #region Input Blockers

    /// <summary>
    /// Determines whether user input is currently blocked by a UI control.
    /// </summary>
    /// <remarks>This method can be used to check if input events should be ignored due to UI interaction,
    /// such as when a menu or dialog is open and under the pointer.</remarks>
    /// <returns>true if a UI control is currently hovered and input is blocked; otherwise, false.</returns>
    public bool IsInputBlockedByUi()
    {
        return GetViewport().GuiGetHoveredControl() != null;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Notifies all registered input modules of a change in the game state.
    /// </summary>
    /// <remarks>This method forwards the game state change to all input modules that implement the
    /// IGameInputModule interface. Use this method to ensure that input modules can update their behavior in response
    /// to changes in the overall game state.</remarks>
    /// <param name="state">The new state of the game to be communicated to input modules.</param>
    public void OnGameStateChanged(GameState state)
    {
        foreach (IInputModule module in _modules)
        {
            if (module is IGameInputModule gameInputModule)
            {
                gameInputModule.OnGameStateChanged(state);
            }
        }
    }

    #endregion

    #region Module Registration

    /// <summary>
    /// Registers all input modules used by the application.
    /// Module registration is intentionally centralized and occurs only once
    /// during initialization to ensure a stable and predictable input pipeline.
    /// </summary>
    private void RegisterInputModules()
    {
        IInputModule[] modules =
        {
            new ConsoleInputModule(this),
            new CameraInputModule(this),
            new InteractionInputModule(this),
        };

        foreach (IInputModule module in modules)
        {
            if (module.Scope == InputScope.Game && module is not IGameInputModule)
            {
                throw new InvalidOperationException($"{module.GetType().Name} has Game scope but does not implement IGameInputModule.");
            }

            _modules.Add(module);

            if (module is IProcessInputModule processInputModule)
            {
                _processModules.Add(processInputModule);
            }

            if (module is IUnhandledInputModule unhandledInputModule)
            {
                _unhandledInputModules.Add(unhandledInputModule);
            }
        }

        _modules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
        _processModules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
        _unhandledInputModules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
    }

    #endregion
}
