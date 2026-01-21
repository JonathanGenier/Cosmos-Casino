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

    private readonly InputState _inputState = new();
    private readonly InputState _previousInputState = new();
    private readonly List<IInputModule> _modules = new();

    private bool _isInputBlockedByUi;

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
    /// Emitted when the primary build action is initiated (for example: left mouse button pressed).
    /// This signal marks the start of a build placement gesture.
    /// </summary>
    [Signal]
    public delegate void BuildPlacePressedEventHandler();

    /// <summary>
    /// Emitted when the primary build action is released.
    /// This signal finalizes the current build placement gesture.
    /// </summary>
    [Signal]
    public delegate void BuildPlaceReleasedEventHandler();

    /// <summary>
    /// Emitted when the remove build action is initiated (for example: right mouse button pressed).
    /// This signal marks the start of a build removal gesture.
    /// </summary>
    [Signal]
    public delegate void BuildRemovePressedEventHandler();

    /// <summary>
    /// Emitted when the remove build action is released.
    /// This signal finalizes the current build removal gesture.
    /// </summary>
    [Signal]
    public delegate void BuildRemoveReleasedEventHandler();

    /// <summary>
    /// Emitted when the current build operation is canceled.
    /// This signal aborts any active build or removal gesture and resets build state.
    /// </summary>
    [Signal]
    public delegate void BuildCanceledEventHandler();

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current input state for the component.
    /// </summary>
    public InputState State => _inputState;

    /// <summary>
    /// Gets a value indicating whether user input is currently blocked by the user interface.
    /// </summary>
    public bool IsInputBlockedByUi => _isInputBlockedByUi;

    /// <summary>
    /// Gets a value indicating whether the primary input button is currently pressed.
    /// </summary>
    public bool IsPrimaryPressed => IsPressed(_inputState[InputButton.Primary], _previousInputState[InputButton.Primary]);

    /// <summary>
    /// Gets a value indicating whether the primary input button was released during the current input update.
    /// </summary>
    public bool IsPrimaryReleased => IsReleased(_inputState[InputButton.Primary], _previousInputState[InputButton.Primary]);

    /// <summary>
    /// Gets a value indicating whether the primary input button is currently held down.
    /// </summary>
    public bool IsPrimaryHeld => IsHeld(_inputState[InputButton.Primary]);

    /// <summary>
    /// Gets a value indicating whether the secondary input button is currently pressed.
    /// </summary>
    public bool IsSecondaryPressed => IsPressed(_inputState[InputButton.Secondary], _previousInputState[InputButton.Secondary]);

    /// <summary>
    /// Gets a value indicating whether the secondary input button was released during the current input update.
    /// </summary>
    public bool IsSecondaryReleased => IsReleased(_inputState[InputButton.Secondary], _previousInputState[InputButton.Secondary]);

    /// <summary>
    /// Gets a value indicating whether the secondary input button is currently held down.
    /// </summary>
    public bool IsSecondaryHeld => IsHeld(_inputState[InputButton.Secondary]);

    /// <summary>
    /// Gets a value indicating whether the Shift key is currently held down.
    /// </summary>
    public bool IsShiftHeld => IsHeld(_inputState[InputButton.Shift]);

    /// <summary>
    /// Gets a value indicating whether the toggle console input button was pressed during the current input state.
    /// </summary>
    public bool IsToggleConsolePressed => IsPressed(_inputState[InputButton.ToggleConsole], _previousInputState[InputButton.ToggleConsole]);

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
        foreach (var module in _modules)
        {
            if (!module.IsEnabled)
            {
                continue;
            }

            module.Process(delta);
        }

        _inputState.CopyTo(_previousInputState);
        _inputState.ResetFrameState();
        _isInputBlockedByUi = GetViewport().GuiGetHoveredControl() != null;
    }

    /// <summary>
    /// Processes input events to update the internal input state based on keyboard and mouse actions.
    /// </summary>
    /// <param name="event">The input event to process. This can be a keyboard event or a mouse button event; other event types are ignored.</param>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.Echo)
            {
                return;
            }

            bool pressed = key.Pressed;

            switch (key.Keycode)
            {
                // ─────────────────────────────
                // Modifiers
                // ─────────────────────────────
                case Key.Shift:
                    _inputState[InputButton.Shift] = pressed;
                    break;

                case Key.Ctrl:
                    _inputState[InputButton.Ctrl] = pressed;
                    break;

                case Key.Alt:
                    _inputState[InputButton.Alt] = pressed;
                    break;

                // ─────────────────────────────
                // Movement
                // ─────────────────────────────
                case Key.W:
                    _inputState[InputButton.MoveForward] = pressed;
                    break;

                case Key.S:
                    _inputState[InputButton.MoveBackward] = pressed;
                    break;

                case Key.A:
                    _inputState[InputButton.MoveLeft] = pressed;
                    break;

                case Key.D:
                    _inputState[InputButton.MoveRight] = pressed;
                    break;

                // ─────────────────────────────
                // Rotation
                // ─────────────────────────────
                case Key.Q:
                    _inputState[InputButton.RotateLeft] = pressed;
                    break;

                case Key.E:
                    _inputState[InputButton.RotateRight] = pressed;
                    break;

                // ─────────────────────────────
                // Console
                // ─────────────────────────────
                case Key.F1:
                    _inputState[InputButton.ToggleConsole] = pressed;
                    break;
            }
        }
        else if (@event is InputEventMouseButton mouse)
        {
            bool pressed = mouse.Pressed;

            switch (mouse.ButtonIndex)
            {
                // ─────────────────────────────
                // Mouse Buttons
                // ─────────────────────────────
                case MouseButton.Left:
                    _inputState[InputButton.Primary] = pressed;
                    break;

                case MouseButton.Right:
                    _inputState[InputButton.Secondary] = pressed;
                    break;

                case MouseButton.Middle:
                    _inputState[InputButton.Middle] = pressed;
                    break;

                // ─────────────────────────────
                // Mouse Wheel (frame-based)
                // ─────────────────────────────
                case MouseButton.WheelUp:
                    _inputState.ScrollDelta += 1f;
                    break;

                case MouseButton.WheelDown:
                    _inputState.ScrollDelta -= 1f;
                    break;
            }
        }
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

    #region Input State Helpers

    /// <summary>
    /// Determines whether an input has transitioned from a released state to a pressed state.
    /// </summary>
    /// <param name="current">The current state of the input. Specify <see langword="true"/> if the input is currently pressed; otherwise,
    /// <see langword="false"/>.</param>
    /// <param name="previous">The previous state of the input. Specify <see langword="true"/> if the input was pressed in the previous check;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns>Returns <see langword="true"/> if the input is currently pressed and was not pressed in the previous state;
    /// otherwise, <see langword="false"/>.</returns>
    private static bool IsPressed(bool current, bool previous)
    {
        return current && !previous;
    }

    /// <summary>
    /// Determines whether a transition from pressed to released state has occurred between two boolean values.
    /// </summary>
    /// <param name="current">The current state value. Specify <see langword="true"/> if pressed; otherwise, <see langword="false"/>.</param>
    /// <param name="previous">The previous state value. Specify <see langword="true"/> if pressed; otherwise, <see langword="false"/>.</param>
    /// <returns>Returns <see langword="true"/> if the state has changed from pressed (<paramref name="previous"/> is <see
    /// langword="true"/>) to released (<paramref name="current"/> is <see langword="false"/>); otherwise, <see
    /// langword="false"/>.</returns>
    private static bool IsReleased(bool current, bool previous)
    {
        return !current && previous;
    }

    /// <summary>
    /// Determines whether the resource is currently held based on the specified state.
    /// </summary>
    /// <param name="current">A value indicating the current state of the resource. Specify <see langword="true"/> if the resource is held;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns>A value indicating whether the resource is held. Returns <see langword="true"/> if the resource is held;
    /// otherwise, <see langword="false"/>.</returns>
    private static bool IsHeld(bool current)
    {
        return current;
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
            new BuildInputModule(this),
        };

        foreach (IInputModule module in modules)
        {
            if (module.Scope == InputScope.Game && module is not IGameInputModule)
            {
                throw new InvalidOperationException($"{module.GetType().Name} has Game scope but does not implement IGameInputModule.");
            }

            _modules.Add(module);
        }

        _modules.Sort((a, b) => a.Phase.CompareTo(b.Phase));
    }

    #endregion
}