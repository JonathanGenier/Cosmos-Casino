using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Updates repeated-structure batched collision after authoritative build mutations.
/// </summary>
public sealed class StructureInstanceCollisionFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly StructureInstanceCollisionManager _structureInstanceCollisionManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a flow that listens for completed build operations and updates repeated structure collision.
    /// </summary>
    /// <param name="buildProcessManager">The build process manager that emits completion events.</param>
    /// <param name="structureInstanceCollisionManager">The repeated structure collision projection to update.</param>
    public StructureInstanceCollisionFlow(
        BuildProcessManager buildProcessManager,
        StructureInstanceCollisionManager structureInstanceCollisionManager)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(structureInstanceCollisionManager);

        _buildProcessManager = buildProcessManager;
        _structureInstanceCollisionManager = structureInstanceCollisionManager;

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
        _structureInstanceCollisionManager.ApplyBuildResult(buildResult);
    }

    #endregion
}
