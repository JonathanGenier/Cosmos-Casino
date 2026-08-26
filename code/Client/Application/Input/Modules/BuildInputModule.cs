using System;

/// <summary>
/// Emits high-level interaction intent signals based on semantic input state.
/// </summary>
/// <remarks>
/// This module does not interpret raw input. It consumes the centralized
/// InputManager state and emits interaction intents (press / release).
/// </remarks>
public sealed class BuildInputModule : IInputModule, IGameInputModule
{
    #region Fields

    private readonly InputManager _inputManager;
    private bool _isEnabled;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildInputModule class using the specified input manager.
    /// </summary>
    /// <param name="inputManager">The input manager that provides input handling functionality for this module. Cannot be null.</param>
    public BuildInputModule(InputManager inputManager)
    {
        ArgumentNullException.ThrowIfNull(inputManager);
        _inputManager = inputManager;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the input phase associated with the build process.
    /// </summary>
    public InputPhase Phase => InputPhase.Build;

    /// <summary>
    /// Gets the input scope used for this instance.
    /// </summary>
    public InputScope Scope => InputScope.Game;

    /// <summary>
    /// Gets a value indicating whether the feature or component is currently enabled.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    #endregion

    #region Process

    /// <summary>
    /// Processes input events for the current frame, emitting signals for build input actions when enabled.
    /// </summary>
    /// <param name="delta">The elapsed time, in seconds, since the last frame. Used to synchronize input processing with the frame rate.</param>
    public void Process(double delta)
    {
        if (!_isEnabled)
        {
            return;
        }

        if (ProcessCancelInput())
        {
            return;
        }

        if (ProcessMouseReleaseInputs())
        {
            return;
        }

        if (_inputManager.IsInputBlockedByUi)
        {
            return;
        }

        ProcessBuildRotationInput();
        ProcessMouseStartInputs();
    }

    #endregion

    #region Game State

    /// <summary>
    /// Handles changes to the game state by updating the enabled status of the component.
    /// </summary>
    /// <param name="state">The new state of the game. Determines whether the component should be enabled or disabled.</param>
    public void OnGameStateChanged(GameState state)
    {
        _isEnabled = state != GameState.Loading;
    }

    #endregion

    #region Internal Methods

    private bool ProcessMouseReleaseInputs()
    {
        if (_inputManager.TryConsumeReleased(InputButton.Primary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildPlaceReleased);
            return true;
        }

        if (_inputManager.TryConsumeReleased(InputButton.Secondary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildRemoveReleased);
            return true;
        }

        return false;
    }

    private bool ProcessCancelInput()
    {
        if (!_inputManager.TryConsumePressed(InputButton.Escape))
        {
            return false;
        }

        _inputManager.EmitSignal(InputManager.SignalName.BuildCanceled, (int)BuildCancellationScope.BuildContext);
        return true;
    }

    private void ProcessMouseStartInputs()
    {
        // -------------------------------------
        // 1. Mouse Cancel
        // -------------------------------------
        if (_inputManager.IsPrimaryHeld && _inputManager.TryConsumePressed(InputButton.Secondary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildCanceled, (int)BuildCancellationScope.ActiveBuild);
            return;
        }

        if (_inputManager.IsSecondaryHeld && _inputManager.TryConsumePressed(InputButton.Primary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildCanceled, (int)BuildCancellationScope.ActiveBuild);
            return;
        }

        // -------------------------------------
        // 2. Mouse Build
        // -------------------------------------
        if (_inputManager.TryConsumePressed(InputButton.Primary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildPlacePressed);
            return;
        }

        // -------------------------------------
        // 3. Mouse Remove
        // -------------------------------------
        if (_inputManager.TryConsumePressed(InputButton.Secondary))
        {
            _inputManager.EmitSignal(InputManager.SignalName.BuildRemovePressed);
            return;
        }
    }

    private void ProcessBuildRotationInput()
    {
        if (!_inputManager.TryConsumePressed(InputButton.BuildRotate))
        {
            return;
        }

        _inputManager.EmitSignal(InputManager.SignalName.BuildRotated);
    }

    #endregion
}
