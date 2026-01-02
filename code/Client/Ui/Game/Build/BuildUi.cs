using CosmosCasino.Core.Map;
using Godot;
using System;

/// <summary>
/// UI controller responsible for presenting build selection controls
/// to the player during build mode.
/// <see cref="BuildUi"/> is a passive view component: it owns UI widgets,
/// listens to user interaction events, and emits high-level build intent
/// signals without directly modifying game state or interaction tools.
/// This class does not interpret gameplay meaning, apply builds, or
/// coordinate input routing. Those responsibilities are delegated to
/// higher-level UI managers and client-side build systems.
/// </summary>
public sealed partial class BuildUi : Control
{
    #region NODES

    /// <summary>
    /// Button used to cancel the current build selection and exit build mode.
    /// </summary>
    [Export]
    private Button _cancelButton = null!;

    /// <summary>
    /// Toggle button selecting the metal floor build intent.
    /// </summary>
    [Export]
    private Button _metalFloorButton = null!;

    /// <summary>
    /// Toggle button selecting the carbon floor build intent.
    /// </summary>
    [Export]
    private Button _carbonFloorButton = null!;

    #endregion

    #region EVENTS

    /// <summary>
    /// Raised when the player selects a floor type to build.
    /// This event communicates intent only and does not apply
    /// any build logic directly.
    /// </summary>
    public event Action<FloorType> FloorSelected;

    /// <summary>
    /// Raised when the player cancels the current build selection.
    /// Consumers are expected to clear build context and restore
    /// default interaction state.
    /// </summary>
    public event Action BuildCancelled;

    #endregion

    #region METHODS

    /// <summary>
    /// Connects UI input signals when the control enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        _metalFloorButton.Toggled += OnMetalFloorToggled;
        _carbonFloorButton.Toggled += OnCarbonFloorToggled;
        _cancelButton.Pressed += ClearToggles;
    }

    /// <summary>
    /// Disconnects UI input signals when the control is removed from
    /// the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        _metalFloorButton.Toggled -= OnMetalFloorToggled;
        _carbonFloorButton.Toggled -= OnCarbonFloorToggled;
        _cancelButton.Pressed -= ClearToggles;
    }

    /// <summary>
    /// Clears all build selection toggles and signals that the current
    /// build operation has been cancelled.
    /// </summary>
    private void ClearToggles()
    {
        _metalFloorButton.SetPressedNoSignal(false);
        _carbonFloorButton.SetPressedNoSignal(false);
        BuildCancelled?.Invoke();
    }

    /// <summary>
    /// Handles toggle state changes for the metal floor button.
    /// Emits a floor selection intent when toggled on.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnMetalFloorToggled(bool toggled)
    {
        if (!toggled)
        {
            return;
        }

        FloorSelected?.Invoke(FloorType.Metal);
    }

    /// <summary>
    /// Handles toggle state changes for the carbon floor button.
    /// Emits a floor selection intent when toggled on.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnCarbonFloorToggled(bool toggled)
    {
        if (!toggled)
        {
            return;
        }

        FloorSelected?.Invoke(FloorType.Carbon);
    }

    #endregion
}