using CosmosCasino.Core.Game.Build.Domain;
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
    #region Inspector Nodes

    /// <summary>
    /// Button used to cancel the current build selection and exit build mode.
    /// </summary>
    [Export]
    private Button? _cancelButton;

    [Export]
    private Button? _floorButton;

    [Export]
    private Button? _wallButton;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a build kind is selected.
    /// </summary>
    public event Action<BuildKind>? BuildKindSelected;

    /// <summary>
    /// Raised when the player cancels the current build selection.
    /// Consumers are expected to clear build context and restore
    /// default interaction state.
    /// </summary>
    public event Action? BuildCancelled;

    #endregion

    #region Properties

    private Button CancelButton
    {
        get => _cancelButton ?? throw new InvalidOperationException("CancelButton not assigned.");
        set => _cancelButton = value;
    }

    private Button FloorButton
    {
        get => _floorButton ?? throw new InvalidOperationException("FloorButton not assigned.");
        set => _floorButton = value;
    }

    private Button WallButton
    {
        get => _wallButton ?? throw new InvalidOperationException("WallButton not assigned.");
        set => _wallButton = value;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Connects UI input signals when the control enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        FloorButton.Toggled += OnFloorToggled;
        WallButton.Toggled += OnWallToggled;
        CancelButton.Pressed += ClearToggles;
    }

    /// <summary>
    /// Disconnects UI input signals when the control is removed from
    /// the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        FloorButton.Toggled -= OnFloorToggled;
        WallButton.Toggled -= OnWallToggled;
        CancelButton.Pressed -= ClearToggles;
    }

    #endregion

    #region Buttons Methods

    /// <summary>
    /// Clears all build selection toggles and signals that the current
    /// build operation has been cancelled.
    /// </summary>
    private void ClearToggles()
    {
        FloorButton.SetPressedNoSignal(false);
        WallButton.SetPressedNoSignal(false);
        BuildCancelled?.Invoke();
    }

    /// <summary>
    /// Handles toggle state changes for the metal floor button.
    /// Emits a floor selection intent when toggled on.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnFloorToggled(bool toggled)
    {
        if (toggled)
        {
            SelectBuildKind(BuildKind.Floor);
        }
    }

    private void OnWallToggled(bool toggled)
    {
        if (toggled)
        {
            SelectBuildKind(BuildKind.Wall);
        }
    }

    #endregion

    private void SelectBuildKind(BuildKind kind)
    {
        FloorButton.SetPressedNoSignal(kind == BuildKind.Floor);
        WallButton.SetPressedNoSignal(kind == BuildKind.Wall);

        BuildKindSelected?.Invoke(kind);
    }

}