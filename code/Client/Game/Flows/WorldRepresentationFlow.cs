using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Furniture;
using System;

/// <summary>
/// Bridges authoritative world mutation completion events to disposable Client world representations.
/// </summary>
internal sealed class WorldRepresentationFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly FurnitureProcessManager _furnitureProcessManager;
    private readonly WorldRepresentationCoordinator _worldRepresentationCoordinator;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a world representation flow.
    /// </summary>
    /// <param name="buildProcessManager">The Client build mutation adapter.</param>
    /// <param name="furnitureProcessManager">The Client furniture mutation adapter.</param>
    /// <param name="worldRepresentationCoordinator">The coordinator that routes successful mutations.</param>
    internal WorldRepresentationFlow(
        BuildProcessManager buildProcessManager,
        FurnitureProcessManager furnitureProcessManager,
        WorldRepresentationCoordinator worldRepresentationCoordinator)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(furnitureProcessManager);
        ArgumentNullException.ThrowIfNull(worldRepresentationCoordinator);

        _buildProcessManager = buildProcessManager;
        _furnitureProcessManager = furnitureProcessManager;
        _worldRepresentationCoordinator = worldRepresentationCoordinator;

        _buildProcessManager.BuildCompleted += OnBuildCompleted;
        _furnitureProcessManager.FurnitureCompleted += OnFurnitureCompleted;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Unsubscribes from mutation completion events.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _buildProcessManager.BuildCompleted -= OnBuildCompleted;
        _furnitureProcessManager.FurnitureCompleted -= OnFurnitureCompleted;
        _isDisposed = true;
    }

    #endregion

    #region Events

    private void OnBuildCompleted(BuildResult buildResult)
    {
        _worldRepresentationCoordinator.ApplyBuildResult(buildResult);
    }

    private void OnFurnitureCompleted(FurnitureOperationResult furnitureResult)
    {
        _worldRepresentationCoordinator.ApplyFurnitureResult(furnitureResult);
    }

    #endregion
}
