using CosmosCasino.Core.Game.Build;
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
    private readonly InteractionManager _interactionManager;
    private bool _disposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildContextFlow class with the specified UI manager, build context, and
    /// interaction manager.
    /// </summary>
    /// <param name="ui">The UI manager responsible for handling build-related user interface events and interactions. Cannot be null.</param>
    /// <param name="context">The build context containing the current state and configuration for the build process. Cannot be null.</param>
    /// <param name="interaction">The interaction manager used to coordinate user interactions during the build process. Cannot be null.</param>
    public BuildContextFlow(BuildUiManager ui, BuildContext context, InteractionManager interaction)
    {
        _buildUiManager = ui;
        _buildContext = context;
        _interactionManager = interaction;

        _buildUiManager.BuildKindSelected += OnBuildKindSelected;
        _buildUiManager.BuildCancelled += OnBuildCancelled;
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

        _buildUiManager.BuildKindSelected -= OnBuildKindSelected;
        _buildUiManager.BuildCancelled -= OnBuildCancelled;
        _disposed = true;
    }

    #endregion

    #region Ui Input Action

    private void OnBuildKindSelected(BuildKind buildKind)
    {
        BuildContextBase context = buildKind switch
        {
            BuildKind.Floor => new FloorBuildContext(),
            BuildKind.Wall => new WallBuildContext(),
            _ => throw new NotSupportedException(
            $"Build kind '{buildKind}' is not supported.")
        };

        _buildContext.Set(context);
        _interactionManager.SetTool(InteractionTool.Build);
    }

    /// <summary>
    /// Handles cleanup operations when a build process is cancelled.
    /// </summary>
    /// <remarks>This method should be called to ensure that any in-progress build state and related tool
    /// interactions are properly reset after a build cancellation. It is intended for internal use within the build
    /// workflow.</remarks>
    private void OnBuildCancelled()
    {
        _buildContext.Clear();
        _interactionManager.ResetTool();
    }

    #endregion
}
