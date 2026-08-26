using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Structures;
using System;

/// <summary>
/// Manages the build context and user interactions during build in the game, coordinating UI events and
/// interaction tools.
/// </summary>
/// <remarks>BuildContextFlow subscribes to build UI events to update the build context and interaction system
/// accordingly. It implements IDisposable to ensure event handlers are properly detached when the instance is disposed.
/// This class is typically used as part of the game's flow management to handle building operations initiated by the
/// user.</remarks>
public class BuildContextFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildUiManager _buildUiManager;
    private readonly BuildContext _buildContext;

    private bool _isApplyingUiSelection;
    private bool _disposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildContextFlow class with the specified UI manager, build context, and
    /// interaction manager.
    /// </summary>
    /// <param name="ui">The UI manager responsible for handling build-related user interface events and interactions. Cannot be null.</param>
    /// <param name="context">The build context containing the current state and configuration for the build process. Cannot be null.</param>
    public BuildContextFlow(BuildUiManager ui, BuildContext context)
    {
        _buildUiManager = ui;
        _buildContext = context;

        _buildUiManager.StructureBuildToolSelected += OnStructureBuildToolSelected;
        _buildUiManager.PillarBuildSelected += OnPillarBuildSelected;
        _buildUiManager.DoorBuildSelected += OnDoorBuildSelected;
        _buildUiManager.CasinoTableBuildSelected += OnCasinoTableBuildSelected;
        _buildUiManager.BuildCancelled += OnBuildCancelled;
        _buildContext.ContextDeactivated += OnContextDeactivated;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to unsubscribe from events and allow
    /// for proper resource cleanup. After calling this method, the instance should not be used.</remarks>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _buildUiManager.StructureBuildToolSelected -= OnStructureBuildToolSelected;
        _buildUiManager.PillarBuildSelected -= OnPillarBuildSelected;
        _buildUiManager.DoorBuildSelected -= OnDoorBuildSelected;
        _buildUiManager.CasinoTableBuildSelected -= OnCasinoTableBuildSelected;
        _buildUiManager.BuildCancelled -= OnBuildCancelled;
        _buildContext.ContextDeactivated -= OnContextDeactivated;
        _disposed = true;
    }

    #endregion

    #region Ui Input Action

    private void OnStructureBuildToolSelected(StructureBuildTool buildTool)
    {
        SetContextFromUi(new StructureBuildContext(
            StructureDefinitions.Block,
            buildTool));
    }

    private void OnPillarBuildSelected()
    {
        SetContextFromUi(new SingleStructureBuildContext(StructureDefinitions.Pillar));
    }

    private void OnDoorBuildSelected()
    {
        SetContextFromUi(new SingleStructureBuildContext(StructureDefinitions.Door));
    }

    private void OnCasinoTableBuildSelected()
    {
        SetContextFromUi(new FurnitureBuildContext(FurnitureDefinitions.CasinoTable));
    }

    /// <summary>
    /// Handles cleanup operations when a build process is cancelled.
    /// </summary>
    /// <remarks>This method should be called to ensure that any in-progress build state and related tool
    /// interactions are properly reset after a build cancellation. It is intended for internal use within the build
    /// workflow.</remarks>
    private void OnBuildCancelled()
    {
        _buildContext.CancelContext();
    }

    private void OnContextDeactivated()
    {
        if (_isApplyingUiSelection)
        {
            return;
        }

        _buildUiManager.ClearSelection();
    }

    private void SetContextFromUi(BuildContextBase context)
    {
        _isApplyingUiSelection = true;

        try
        {
            _buildContext.SetContext(context);
        }
        finally
        {
            _isApplyingUiSelection = false;
        }
    }

    #endregion
}
