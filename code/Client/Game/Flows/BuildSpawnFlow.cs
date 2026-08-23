using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using System;


/// <summary>
/// Coordinates the flow for building and spawning buildable elements in the game, handling build completion events
/// and managing the placement or removal of buildable visuals.
/// </summary>
/// <remarks>This class subscribes to build completion events from the client build manager and uses the spawn
/// manager to update the game world accordingly. It is responsible for displaying failure messages and ensuring that
/// buildable objects are spawned or despawned based on the outcome of build operations. Instances of this class should
/// be disposed when no longer needed to unsubscribe from events and release resources.</remarks>
public class BuildSpawnFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _clientBuildManager;

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
        ArgumentNullException.ThrowIfNull(clientBuildManager);
        ArgumentNullException.ThrowIfNull(spawnManager);

        _clientBuildManager = clientBuildManager;

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
        if (buildResult.Outcome == BuildOperationOutcome.Invalid)
        {
            DisplayFailureMessage(buildResult.FailureReason);
            return;
        }

        if (buildResult.Outcome == BuildOperationOutcome.NoOp)
        {
            return;
        }

        foreach (BuildStructureResult result in buildResult.Structures)
        {
            if (result.Outcome != BuildOperationOutcome.Valid)
            {
                continue;
            }

            if (result.Kind == BuildStructureResultKind.Created)
            {
                SpawnBuild(result);
                continue;
            }

            if (result.Kind == BuildStructureResultKind.Removed)
            {
                RemoveBuild(result);
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
    private void DisplayFailureMessage(BuildFailureReason failureReason)
    {
        // Intentionally logging all failure reasons during development.
        // This will be replaced with UI feedback later.
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

    #region Spawn/Despawn Methods

    /// <summary>
    /// Spawns successful structure build visuals at their affected cells.
    /// </summary>
    /// <param name="result">The successful structure creation result.</param>
    private void SpawnBuild(BuildStructureResult result)
    {
        // Structure visuals require a real definition-based renderer registration.
        // Until that exists, successful Structure creation has no legacy Floor/Wall visual side effect.
    }

    private void RemoveBuild(BuildStructureResult result)
    {
        // Structure visuals require a real definition-based renderer registration.
        // Until that exists, successful Structure removal has no legacy Floor/Wall visual side effect.
    }

    #endregion
}
