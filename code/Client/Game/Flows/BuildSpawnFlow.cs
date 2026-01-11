using CosmosCasino.Core.Build;
using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Map;
using CosmosCasino.Core.Map.Cell;
using Godot;
using System;

/// <summary>
/// Coordinates the flow for building and spawning floor elements in the game, handling build completion events and
/// managing the placement or removal of floor objects.
/// </summary>
/// <remarks>This class subscribes to build completion events from the client build manager and uses the spawn
/// manager to update the game world accordingly. It is responsible for displaying failure messages and ensuring that
/// floor objects are spawned or despawned based on the outcome of build operations. Instances of this class should be
/// disposed when no longer needed to unsubscribe from events and release resources.</remarks>
public class BuildSpawnFlow : IGameFlow, IDisposable
{
    #region Fields

    private ClientBuildManager _clientBuildManager;
    private SpawnManager _spawnManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildSpawnFlow class with the specified client build manager and spawn
    /// manager.
    /// </summary>
    /// <param name="clientBuildManager">The client build manager used to monitor and manage build completion events. Cannot be null.</param>
    /// <param name="spawnManager">The spawn manager responsible for handling spawn operations. Cannot be null.</param>
    public BuildSpawnFlow(ClientBuildManager clientBuildManager, SpawnManager spawnManager)
    {
        _clientBuildManager = clientBuildManager;
        _spawnManager = spawnManager;

        _clientBuildManager.BuildCompleted += OnBuildCompleted;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when you are finished using the instance to unsubscribe from events and
    /// allow for proper resource cleanup. After calling this method, the instance should not be used.</remarks>
    public void Dispose()
    {
        _clientBuildManager.BuildCompleted -= OnBuildCompleted;
    }

    #endregion

    #region Build Methods

    /// <summary>
    /// Handles the completion of a build operation by processing the results and performing appropriate actions based
    /// on each operation's outcome.
    /// </summary>
    /// <param name="buildResult">The result of the completed build operation, containing the intent and a collection of individual operation
    /// results to process. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if an unrecognized build operation outcome is encountered in the results.</exception>
    private void OnBuildCompleted(BuildResult buildResult)
    {
        BuildIntent buildIntent = buildResult.Intent;

        ConsoleLog.Warning(nameof(BuildSpawnFlow), buildIntent.ToString());
        foreach (BuildOperationResult result in buildResult.Results)
        {
            switch (result.Outcome)
            {
                case BuildOperationOutcome.Failed:
                case BuildOperationOutcome.Skipped:
                    DisplayFailureMessage(result.FailureReason);
                    continue;

                case BuildOperationOutcome.Placed:
                case BuildOperationOutcome.Replaced:
                    PlaceOrReplaceFloor(result, buildIntent);
                    continue;

                case BuildOperationOutcome.Removed:
                    RemoveFloor(result);
                    continue;

                default:
                    throw new InvalidOperationException($"{result.Outcome} not implemented");
            }
        }
    }

    #endregion

    #region Display Failure

    /// <summary>
    /// Logs a message indicating the reason for a build operation failure.
    /// </summary>
    /// <remarks>This method is intended for development and diagnostic purposes. In production, failure
    /// reasons may be communicated to the user through the UI instead of logging.</remarks>
    /// <param name="failureReason">The reason the build operation failed. Specifies the type of failure encountered.</param>
    /// <exception cref="InvalidOperationException">Thrown if an unrecognized or unsupported failure reason is provided.</exception>
    private void DisplayFailureMessage(BuildOperationFailureReason failureReason)
    {
        // Intentionally logging all failure reasons during development.
        // This will be replaced with UI feedback later.
        switch (failureReason)
        {
            case BuildOperationFailureReason.NoStructure:
            case BuildOperationFailureReason.NoFurniture:
            case BuildOperationFailureReason.NoFunds:
            case BuildOperationFailureReason.NoFloor:
            case BuildOperationFailureReason.Blocked:
            case BuildOperationFailureReason.None:
            case BuildOperationFailureReason.SameType:
                ConsoleLog.Info(nameof(ClientBuildManager), failureReason.ToString());
                break;

            case BuildOperationFailureReason.InternalError:
            case BuildOperationFailureReason.NoCell:
                ConsoleLog.Error(nameof(ClientBuildManager), failureReason.ToString());
                break;

            default:
                throw new InvalidOperationException($"{failureReason} not implemented");
        }
    }

    #endregion

    #region Spawn/Despawn Methods

    /// <summary>
    /// Places a new floor or replaces an existing floor at the specified location based on the build operation result
    /// and intent.
    /// </summary>
    /// <param name="result">The result of the build operation, containing information about the target cell and operation outcome.</param>
    /// <param name="buildIntent">The intent specifying the type of floor to place or replace.</param>
    private void PlaceOrReplaceFloor(BuildOperationResult result, BuildIntent buildIntent)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result);
        FloorSpawnVariant variant = new((FloorType)buildIntent.FloorType);
        Vector3 position = MapToWorld.CellToWorld(spawnKey.Coord);

        _spawnManager.Spawn(spawnKey, variant, position, SpawnLayer.Floors);
    }

    /// <summary>
    /// Removes the floor associated with the specified build operation result.
    /// </summary>
    /// <param name="result">The result of the build operation that identifies the floor to remove. Cannot be null.</param>
    private void RemoveFloor(BuildOperationResult result)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result);
        _spawnManager.Despawn(spawnKey);
    }

    /// <summary>
    /// Creates a new spawn key for the specified build operation result.
    /// </summary>
    /// <param name="result">The result of the build operation containing the cell information to use for the spawn key.</param>
    /// <returns>A <see cref="CellSlotSpawnKey"/> representing the spawn location for the specified build operation result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> does not contain a valid cell coordinate.</exception>
    private CellSlotSpawnKey GetSpawnKey(BuildOperationResult result)
    {
        if (result.Cell is not MapCellCoord coord)
        {
            throw new InvalidOperationException(
                $"BuildOperationOutcome '{result.Outcome}' requires a cell, but none was provided.");
        }

        var slot = MapCellSlot.Floor;
        return new CellSlotSpawnKey(coord, slot);
    }

    #endregion
}