using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
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

    private BuildProcessManager _clientBuildManager;
    private SpawnManager _spawnManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildSpawnFlow class with the specified client build manager and spawn
    /// manager.
    /// </summary>
    /// <param name="clientBuildManager">The client build manager used to monitor and manage build completion events. Cannot be null.</param>
    /// <param name="spawnManager">The spawn manager responsible for handling spawn operations. Cannot be null.</param>
    public BuildSpawnFlow(BuildProcessManager clientBuildManager, SpawnManager spawnManager)
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

        foreach (BuildOperationResult result in buildResult.Results)
        {
            if (result.Outcome == BuildOperationOutcome.Invalid)
            {
                DisplayFailureMessage(result.FailureReason);
            }

            if (result.Outcome == BuildOperationOutcome.Valid)
            {
                if (buildIntent.Operation == BuildOperation.Place)
                {
                    SpawnBuild(result, buildIntent);
                    continue;
                }
                else if (buildIntent.Operation == BuildOperation.Remove)
                {
                    RemoveBuild(result, buildIntent.Kind);
                    continue;
                }
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
            case BuildOperationFailureReason.NoWall:
            case BuildOperationFailureReason.NoFloor:
            case BuildOperationFailureReason.Blocked:
            case BuildOperationFailureReason.None:
                ConsoleLog.Info(failureReason.ToString());
                break;

            case BuildOperationFailureReason.NoCell:
                ConsoleLog.Error(failureReason.ToString());
                break;

            default:
                throw new InvalidOperationException($"{failureReason} not implemented");
        }
    }

    #endregion

    #region Spawn/Despawn Methods

    private void SpawnBuild(BuildOperationResult result, BuildIntent buildIntent)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result, buildIntent.Kind);
        Vector3 position = MapMath.MapToWorld(spawnKey.Coord);
        BuildSpawnDescriptor descriptor = BuildSpawnDescriptorResolver.Resolve(buildIntent);

        _spawnManager.Spawn(
            spawnKey,
            descriptor.Variant,
            position,
            descriptor.Layer);
    }

    private void RemoveBuild(BuildOperationResult result, BuildKind buildKind)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result, buildKind);
        _spawnManager.Despawn(spawnKey);
    }

    #endregion

    #region Spawn Key

    private CellSlotSpawnKey GetSpawnKey(BuildOperationResult result, BuildKind buildKind)
    {
        MapCoord coord = result.MapCoord;
        CellSlot slot = buildKind switch
        {
            BuildKind.Floor => CellSlot.Floor,
            BuildKind.Wall => CellSlot.Wall,
            _ => throw new InvalidOperationException($"{nameof(BuildKind)} {buildKind} not implemented"),
        };

        return new CellSlotSpawnKey(coord, slot);
    }

    #endregion
}