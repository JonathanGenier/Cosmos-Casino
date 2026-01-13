using CosmosCasino.Core.Game.Floor;
using Godot;
using System;

/// <summary>
/// Manages the user interface for building actions, including floor selection and build cancellation, within the game
/// scene.
/// </summary>
/// <remarks>BuildUiManager coordinates the build UI lifecycle and exposes events for user intent, allowing other
/// components to respond to build-related actions. It is responsible for initializing the build UI, handling user input
/// for build operations, and ensuring proper event wiring when entering or exiting the scene tree. This class does not
/// perform build logic directly; it serves as an intermediary between the UI and the build system.</remarks>
public sealed partial class BuildUiManager : InitializableNodeManager
{
    #region Fields

    private BuildUi _buildUi = null!;

    #endregion

    #region Events

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

    #region Initialization

    /// <summary>
    /// Initializes the component and prepares it for use.
    /// </summary>
    public void Initialize()
    {
        MarkInitialized();
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Instantiates the build UI scene and wires UI intent events when
    /// this manager enters the scene tree.
    /// </summary>
    protected override void OnReady()
    {
        _buildUi = AddNode(GD.Load<PackedScene>(GameUiPaths.Build).Instantiate<BuildUi>());
        _buildUi.FloorSelected += OnFloorSelected;
        _buildUi.BuildCancelled += OnBuildCancelled;
    }

    /// <summary>
    /// Disconnects UI intent event bindings when this manager is removed
    /// from the scene tree to prevent dangling callbacks.
    /// </summary>
    protected override void OnExit()
    {
        _buildUi.FloorSelected -= OnFloorSelected;
        _buildUi.BuildCancelled -= OnBuildCancelled;
    }

    #endregion

    #region BuildUi Callbacks

    /// <summary>
    /// Handles floor selection intent emitted by the build UI by updating
    /// the active build context and switching the interaction system into
    /// build mode.
    /// </summary>
    /// <param name="floorType">
    /// The selected floor type to build.
    /// </param>
    private void OnFloorSelected(FloorType floorType)
    {
        FloorSelected?.Invoke(floorType);
    }

    /// <summary>
    /// Handles build cancellation intent emitted by the build UI by
    /// clearing the active build context and restoring the default
    /// interaction tool.
    /// </summary>
    private void OnBuildCancelled()
    {
        BuildCancelled?.Invoke();
    }

    #endregion
}