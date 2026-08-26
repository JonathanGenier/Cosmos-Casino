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

    [Export]
    private Button? _pillarButton;

    [Export]
    private Button? _doorButton;

    [Export]
    private Button? _casinoTableButton;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a structure build interaction tool is selected.
    /// </summary>
    public event Action<StructureBuildTool>? StructureBuildToolSelected;

    /// <summary>
    /// Occurs when the Pillar structure build tool is selected.
    /// </summary>
    public event Action? PillarBuildSelected;

    /// <summary>
    /// Occurs when the Door structure build tool is selected.
    /// </summary>
    public event Action? DoorBuildSelected;

    /// <summary>
    /// Occurs when the Casino Table furniture build tool is selected.
    /// </summary>
    public event Action? CasinoTableBuildSelected;

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

    private Button PillarButton
    {
        get => _pillarButton ?? throw new InvalidOperationException("PillarButton not assigned.");
        set => _pillarButton = value;
    }

    private Button DoorButton
    {
        get => _doorButton ?? throw new InvalidOperationException("DoorButton not assigned.");
        set => _doorButton = value;
    }

    private Button CasinoTableButton
    {
        get => _casinoTableButton ?? throw new InvalidOperationException("CasinoTableButton not assigned.");
        set => _casinoTableButton = value;
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
        PillarButton.Toggled += OnPillarToggled;
        DoorButton.Toggled += OnDoorToggled;
        CasinoTableButton.Toggled += OnCasinoTableToggled;
        CancelButton.Pressed += OnCancelPressed;
    }

    /// <summary>
    /// Disconnects UI input signals when the control is removed from
    /// the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        FloorButton.Toggled -= OnFloorToggled;
        WallButton.Toggled -= OnWallToggled;
        PillarButton.Toggled -= OnPillarToggled;
        DoorButton.Toggled -= OnDoorToggled;
        CasinoTableButton.Toggled -= OnCasinoTableToggled;
        CancelButton.Pressed -= OnCancelPressed;
    }

    #endregion

    #region Buttons Methods

    /// <summary>
    /// Clears all build selection toggles and signals that the current
    /// build operation has been cancelled.
    /// </summary>
    internal void ClearSelection()
    {
        FloorButton.SetPressedNoSignal(false);
        WallButton.SetPressedNoSignal(false);
        PillarButton.SetPressedNoSignal(false);
        DoorButton.SetPressedNoSignal(false);
        CasinoTableButton.SetPressedNoSignal(false);
    }

    private void OnCancelPressed()
    {
        ClearSelection();
        BuildCancelled?.Invoke();
    }

    /// <summary>
    /// Handles toggle state changes for the floor interaction button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnFloorToggled(bool toggled)
    {
        if (toggled)
        {
            SelectStructureBuildTool(StructureBuildTool.Floor);
        }
    }

    /// <summary>
    /// Handles toggle state changes for the wall interaction button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnWallToggled(bool toggled)
    {
        if (toggled)
        {
            SelectStructureBuildTool(StructureBuildTool.Wall);
        }
    }

    /// <summary>
    /// Handles toggle state changes for the Pillar structure button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnPillarToggled(bool toggled)
    {
        if (toggled)
        {
            SelectPillarBuildTool();
        }
    }

    /// <summary>
    /// Handles toggle state changes for the Door structure button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnDoorToggled(bool toggled)
    {
        if (toggled)
        {
            SelectDoorBuildTool();
        }
    }

    /// <summary>
    /// Handles toggle state changes for the Casino Table furniture button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnCasinoTableToggled(bool toggled)
    {
        if (toggled)
        {
            SelectCasinoTableBuildTool();
        }
    }

    #endregion

    #region Selection

    private void SelectStructureBuildTool(StructureBuildTool buildTool)
    {
        FloorButton.SetPressedNoSignal(buildTool == StructureBuildTool.Floor);
        WallButton.SetPressedNoSignal(buildTool == StructureBuildTool.Wall);
        PillarButton.SetPressedNoSignal(false);
        DoorButton.SetPressedNoSignal(false);
        CasinoTableButton.SetPressedNoSignal(false);

        StructureBuildToolSelected?.Invoke(buildTool);
    }

    private void SelectPillarBuildTool()
    {
        FloorButton.SetPressedNoSignal(false);
        WallButton.SetPressedNoSignal(false);
        PillarButton.SetPressedNoSignal(true);
        DoorButton.SetPressedNoSignal(false);
        CasinoTableButton.SetPressedNoSignal(false);

        PillarBuildSelected?.Invoke();
    }

    private void SelectDoorBuildTool()
    {
        FloorButton.SetPressedNoSignal(false);
        WallButton.SetPressedNoSignal(false);
        PillarButton.SetPressedNoSignal(false);
        DoorButton.SetPressedNoSignal(true);
        CasinoTableButton.SetPressedNoSignal(false);

        DoorBuildSelected?.Invoke();
    }

    private void SelectCasinoTableBuildTool()
    {
        FloorButton.SetPressedNoSignal(false);
        WallButton.SetPressedNoSignal(false);
        PillarButton.SetPressedNoSignal(false);
        DoorButton.SetPressedNoSignal(false);
        CasinoTableButton.SetPressedNoSignal(true);

        CasinoTableBuildSelected?.Invoke();
    }

    #endregion
}
