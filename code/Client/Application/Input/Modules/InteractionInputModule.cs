using Godot;
using System;

/// <summary>
/// Handles interaction-related input events and emits interaction intent signals during the appropriate input phase.
/// </summary>
/// <remarks>This module processes input events related to user interactions, such as primary interaction presses
/// and releases, and emits corresponding signals through the provided input manager. It is typically used in game
/// scenarios to handle player interaction input, and is enabled or disabled based on the current game state.</remarks>
public sealed class InteractionInputModule : IUnhandledInputModule, IGameInputModule
{
    #region Fields

    private readonly InputManager _inputManager;
    private bool _isEnabled;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the InteractionInputModule class using the specified input manager.
    /// </summary>
    /// <param name="input">The input manager that provides input events for the module. Cannot be null.</param>
    public InteractionInputModule(InputManager input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _inputManager = input;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the input phase associated with the current module.
    /// </summary>
    public InputPhase Phase => InputPhase.Interaction;

    /// <summary>
    /// Gets the input scope associated with this module.
    /// </summary>
    public InputScope Scope => InputScope.Game;

    /// <summary>
    /// Gets a value indicating whether the module is enabled.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    #endregion

    #region Godot Process

    /// <summary>
    /// Handles unhandled input events, triggering primary interaction signals when appropriate.
    /// </summary>
    /// <remarks>If input is currently blocked by the UI, this method ignores the event. Only primary
    /// interaction actions are processed; all other input events are ignored.</remarks>
    /// <param name="event">The input event to process. Typically represents user actions such as key presses or mouse clicks.</param>
    public void UnhandledInput(InputEvent @event)
    {
        if (_inputManager.IsInputBlockedByUi())
        {
            return;
        }

        if (@event.IsActionPressed("interaction_primary"))
        {
            _inputManager.EmitSignal(InputManager.SignalName.PrimaryInteractionPressed);
            return;
        }

        if (@event.IsActionReleased("interaction_primary"))
        {
            _inputManager.EmitSignal(InputManager.SignalName.PrimaryInteractionReleased);
            return;
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles changes to the game state by updating the enabled status of the component.
    /// </summary>
    /// <param name="state">The new state of the game. Determines whether the component should be enabled or disabled.</param>
    public void OnGameStateChanged(GameState state)
    {
        _isEnabled = state != GameState.Loading;
    }

    #endregion
}
