using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Updates repeated-structure MultiMesh rendering after authoritative build mutations.
/// </summary>
public sealed class StructureInstanceRenderFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly StructureInstanceRenderManager _structureInstanceRenderManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a flow that listens for completed build operations and updates repeated structure instances.
    /// </summary>
    /// <param name="buildProcessManager">The build process manager that emits completion events.</param>
    /// <param name="structureInstanceRenderManager">The repeated structure renderer to update.</param>
    public StructureInstanceRenderFlow(
        BuildProcessManager buildProcessManager,
        StructureInstanceRenderManager structureInstanceRenderManager)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(structureInstanceRenderManager);

        _buildProcessManager = buildProcessManager;
        _structureInstanceRenderManager = structureInstanceRenderManager;

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
        _structureInstanceRenderManager.ApplyBuildResult(buildResult);
    }

    #endregion
}
