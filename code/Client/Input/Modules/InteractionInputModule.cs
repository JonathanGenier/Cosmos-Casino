using Godot;
using System;

/// <summary>
/// Input module responsible for translating raw pointer input
/// into high-level interaction intent signals
/// This module emits interaction intent only (primary / secondary)
/// and contains no knowledge of interaction modes, cursor context,
/// or gameplay behavior.
/// </summary>
public sealed class InteractionInputModule : IUnhandledInputModule
{
    #region FIELDS

    private readonly InputManager _input;
    private readonly Func<bool> _isCursorBlockedByUi;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionInputModule"/>
    /// and wires required client-side services used to evaluate UI blocking.
    /// </summary>
    /// <param name="clientServices">
    /// Client service container providing UI state queries.
    /// </param>
    /// <param name="input">
    /// Input manager through which interaction intent signals are emitted.
    /// </param>
    public InteractionInputModule(ClientServices clientServices, InputManager input)
    {
        ArgumentNullException.ThrowIfNull(clientServices);
        ArgumentNullException.ThrowIfNull(input);

        _input = input;
        _isCursorBlockedByUi = () => clientServices.UiManager.IsCursorBlockedByUi;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Execution phase for this input module.
    /// </summary>
    public InputPhase Phase => InputPhase.Interaction;

    #endregion

    #region METHODS

    /// <summary>
    /// Processes discrete interaction-related input during the unhandled
    /// input phase and emits interaction intent signals.
    /// </summary>
    /// <param name="event">
    /// The unhandled input event received from the engine.
    /// </param>
    public void UnhandledInput(InputEvent @event)
    {
        if (_isCursorBlockedByUi())
        {
            return;
        }

        if (@event.IsActionPressed("interaction_primary"))
        {
            _input.EmitSignal(InputManager.SignalName.PrimaryInteractionPressed);
            return;
        }

        if (@event.IsActionReleased("interaction_primary"))
        {
            _input.EmitSignal(InputManager.SignalName.PrimaryInteractionReleased);
            return;
        }
    }

    #endregion
}
