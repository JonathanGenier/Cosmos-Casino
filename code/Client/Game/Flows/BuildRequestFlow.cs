using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;
using System;

/// <summary>
/// Coordinates the process of handling build requests within a game flow, managing interactions and delegating build
/// operations to the appropriate client manager.
/// </summary>
/// <remarks>This class subscribes to build request events from an interaction manager and forwards build intents
/// to a client build manager for execution. It implements IDisposable to ensure event handlers are properly
/// unsubscribed and resources are released when the flow is disposed. Instances of this class are not
/// thread-safe.</remarks>
public class BuildRequestFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildProcessManager _clientBuildManager;
    private readonly FurnitureProcessManager _furnitureProcessManager;
    private readonly BuildContext _buildContext;

    private bool _isDisposed;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the BuildRequestFlow class with the specified build process manager and build
    /// context.
    /// </summary>
    /// <param name="clientBuildManager">The build process manager used to coordinate and manage build operations. Cannot be null.</param>
    /// <param name="furnitureProcessManager">The furniture process manager used to coordinate furniture operations. Cannot be null.</param>
    /// <param name="buildContext">The build context that provides information and state for the build process. Cannot be null.</param>
    public BuildRequestFlow(
        BuildProcessManager clientBuildManager,
        FurnitureProcessManager furnitureProcessManager,
        BuildContext buildContext)
    {
        ArgumentNullException.ThrowIfNull(clientBuildManager);
        ArgumentNullException.ThrowIfNull(furnitureProcessManager);
        ArgumentNullException.ThrowIfNull(buildContext);

        _clientBuildManager = clientBuildManager;
        _furnitureProcessManager = furnitureProcessManager;
        _buildContext = buildContext;

        _buildContext.BuildEnded += OnBuildEnded;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when you are finished using the object to release any resources it holds.
    /// After calling Dispose, the object should not be used.</remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _buildContext.BuildEnded -= OnBuildEnded;
        _isDisposed = true;
    }

    #endregion

    #region BuildRequest Methods

    private void OnBuildEnded()
    {
        var buildIntent = _buildContext.TryCreateStructureBuildIntent();

        if (buildIntent != null)
        {
            ExecuteBuildIntent(buildIntent);
            return;
        }

        var furnitureRequest = _buildContext.TryCreateFurniturePlacementRequest();

        if (furnitureRequest != null)
        {
            ExecuteFurniturePlacement(furnitureRequest);
        }
    }

    private void ExecuteBuildIntent(BuildIntent buildIntent)
    {
        BuildResult buildResult = _clientBuildManager.ExecuteBuildIntent(buildIntent);

        if (buildResult.Outcome == BuildOperationOutcome.Invalid)
        {
            DisplayBuildFailureMessage(buildResult.FailureReason);
        }
    }

    private void ExecuteFurniturePlacement(FurniturePlacementRequest request)
    {
        FurnitureOperationResult furnitureResult = _furnitureProcessManager.Place(request);

        if (furnitureResult.Outcome == FurnitureOperationOutcome.Invalid)
        {
            DisplayFurnitureFailureMessage(furnitureResult.FailureReason);
        }
    }

    #endregion

    #region Display Failure

    private void DisplayBuildFailureMessage(BuildFailureReason failureReason)
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

    private void DisplayFurnitureFailureMessage(FurnitureFailureReason failureReason)
    {
        switch (failureReason)
        {
            case FurnitureFailureReason.None:
            case FurnitureFailureReason.FootprintCoordinateOverflow:
            case FurnitureFailureReason.OutsideGeneratedWorld:
            case FurnitureFailureReason.StructurePresent:
            case FurnitureFailureReason.FurniturePresent:
            case FurnitureFailureReason.OccupancyConflict:
            case FurnitureFailureReason.InconsistentReservationState:
            case FurnitureFailureReason.FurnitureIdAllocationExhausted:
            case FurnitureFailureReason.FurnitureIdAlreadyExists:
            case FurnitureFailureReason.FurnitureStateInconsistent:
                ConsoleLog.Info(failureReason.ToString());
                break;

            default:
                throw new InvalidOperationException($"{failureReason} not implemented");
        }
    }

    #endregion
}
