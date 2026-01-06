using CosmosCasino.Core.Floor;
using Godot;

/// <summary>
/// Client-side coordinator responsible for connecting build-related UI
/// interactions to the client interaction and build intent systems.
/// <see cref="BuildUiManager"/> owns the lifecycle of the build UI,
/// subscribes to user intent events emitted by <see cref="BuildUi"/>,
/// and translates those intents into updates to the active
/// <see cref="BuildContext"/> and <see cref="InteractionManager"/>.
/// This class does not execute builds, modify the world, or perform
/// validation. Its sole responsibility is to synchronize UI-driven
/// build intent with client-side interaction state.
/// </summary>
/// <param name="bootstrap">
/// Client bootstrap providing access to initialized client services.
/// </param>
public sealed partial class BuildUiManager(ClientBootstrap bootstrap)
        : ClientManager(bootstrap)
{
    #region FIELDS

    private BuildUi _buildUi = null!;
    private BuildContext BuildContext => ClientServices.BuildContext;
    private InteractionManager InteractionManager => ClientServices.InteractionManager;

    #endregion

    #region METHODS

    /// <summary>
    /// Instantiates the build UI scene and wires UI intent events when
    /// this manager enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        _buildUi = AddOwnedNode(GD.Load<PackedScene>(UiPaths.Build).Instantiate<BuildUi>(), nameof(BuildUi));
        _buildUi.FloorSelected += OnFloorSelected;
        _buildUi.BuildCancelled += OnBuildCancelled;
    }

    /// <summary>
    /// Disconnects UI intent event bindings when this manager is removed
    /// from the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        _buildUi.FloorSelected -= OnFloorSelected;
        _buildUi.BuildCancelled -= OnBuildCancelled;
    }

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
        BuildContext.SetFloor(floorType);
        InteractionManager.SetTool(InteractionTool.Build);
    }

    /// <summary>
    /// Handles build cancellation intent emitted by the build UI by
    /// clearing the active build context and restoring the default
    /// interaction tool.
    /// </summary>
    private void OnBuildCancelled()
    {
        BuildContext.Clear();
        InteractionManager.ResetTool();
    }

    #endregion
}
