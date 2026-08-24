using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;

/// <summary>
/// Rebuilds generated structure collision regions after successful authoritative build mutations.
/// </summary>
public sealed class StructureCollisionFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly StructureCollisionManager _structureCollisionManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a flow that listens for completed build operations and invalidates generated collision regions.
    /// </summary>
    /// <param name="buildProcessManager">The build process manager that emits completion events.</param>
    /// <param name="structureCollisionManager">The generated structure collision projection to update.</param>
    public StructureCollisionFlow(
        BuildProcessManager buildProcessManager,
        StructureCollisionManager structureCollisionManager)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(structureCollisionManager);

        _buildProcessManager = buildProcessManager;
        _structureCollisionManager = structureCollisionManager;

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
        ArgumentNullException.ThrowIfNull(buildResult);

        if (buildResult.Outcome != BuildOperationOutcome.Valid)
        {
            return;
        }

        var affectedCells = new List<MapCellCoord>();

        foreach (BuildStructureResult structure in buildResult.Structures)
        {
            if (structure.Outcome == BuildOperationOutcome.Valid)
            {
                affectedCells.AddRange(structure.AffectedCells);
            }
        }

        _structureCollisionManager.RebuildAffectedCells(affectedCells);
    }

    #endregion
}
