using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Updates scene-rendered structure views after authoritative build mutations.
/// </summary>
public sealed class StructureSceneRenderFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly StructureSceneRenderManager _structureSceneRenderManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a flow that listens for completed build operations and updates scene-rendered structures.
    /// </summary>
    /// <param name="buildProcessManager">The build process manager that emits completion events.</param>
    /// <param name="structureSceneRenderManager">The scene-rendered structure manager to update.</param>
    public StructureSceneRenderFlow(
        BuildProcessManager buildProcessManager,
        StructureSceneRenderManager structureSceneRenderManager)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(structureSceneRenderManager);

        _buildProcessManager = buildProcessManager;
        _structureSceneRenderManager = structureSceneRenderManager;

        _buildProcessManager.BuildCompleted += OnBuildCompleted;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Unsubscribes from build completion events.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _buildProcessManager.BuildCompleted -= OnBuildCompleted;
        _isDisposed = true;
    }

    #endregion

    #region Events

    private void OnBuildCompleted(BuildResult buildResult)
    {
        _structureSceneRenderManager.ApplyBuildResult(buildResult);
    }

    #endregion
}
