using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;

/// <summary>
/// Rebuilds generated structure render sections after successful authoritative build mutations.
/// </summary>
public sealed class StructureRenderFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _buildProcessManager;
    private readonly StructureRenderManager _structureRenderManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a flow that listens for completed build operations and invalidates generated structure sections.
    /// </summary>
    /// <param name="buildProcessManager">The build process manager that emits completion events.</param>
    /// <param name="structureRenderManager">The generated structure renderer to update.</param>
    public StructureRenderFlow(
        BuildProcessManager buildProcessManager,
        StructureRenderManager structureRenderManager)
    {
        ArgumentNullException.ThrowIfNull(buildProcessManager);
        ArgumentNullException.ThrowIfNull(structureRenderManager);

        _buildProcessManager = buildProcessManager;
        _structureRenderManager = structureRenderManager;

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

        if (buildResult.Outcome == BuildOperationOutcome.Invalid)
        {
            DisplayFailureMessage(buildResult.FailureReason);
            return;
        }

        if (buildResult.Outcome == BuildOperationOutcome.NoOp)
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

        _structureRenderManager.RebuildAffectedCells(affectedCells);
    }

    #endregion

    #region Display Failure

    private void DisplayFailureMessage(BuildFailureReason failureReason)
    {
        switch (failureReason)
        {
            case BuildFailureReason.None:
            case BuildFailureReason.FootprintCoordinateOverflow:
            case BuildFailureReason.OutsideGeneratedWorld:
            case BuildFailureReason.OccupancyConflict:
            case BuildFailureReason.InconsistentReservationState:
            case BuildFailureReason.IntraBatchFootprintOverlap:
            case BuildFailureReason.StructureIdAllocationExhausted:
            case BuildFailureReason.StructureIdAlreadyExists:
            case BuildFailureReason.StructureStateInconsistent:
                ConsoleLog.Info(failureReason.ToString());
                break;

            default:
                throw new InvalidOperationException($"{failureReason} not implemented");
        }
    }

    #endregion
}
