using CosmosCasino.Core.Build;
using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Map;
using CosmosCasino.Core.Map.Cell;
using Godot;
using System;
using static Godot.HttpRequest;

/// <summary>
/// Initializes the client build manager with access to required
/// core services through the client bootstrap.
/// </summary>
/// <param name="bootstrap">
/// Bootstrap context providing initialized services.
/// </param>
public sealed partial class ClientBuildManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    /// <summary>
    /// Executes the specified build intent against the authoritative
    /// build system and logs the per-cell operation outcomes.
    /// </summary>
    /// <param name="buildIntent">
    /// The build intent to execute.
    /// </param>
    public void ExecuteBuildIntent(BuildIntent buildIntent)
    {
        BuildResult buildResult = CoreServices.BuildManager.ApplyBuildOperations(buildIntent);

        ApplyFloorVisuals(buildResult);
    }

    private void ApplyFloorVisuals(BuildResult buildResult)
    {
        BuildIntent buildIntent = buildResult.Intent;

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

            default: throw new InvalidOperationException($"{failureReason} not implemented");
        }
    }

    private void PlaceOrReplaceFloor(BuildOperationResult result, BuildIntent buildIntent)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result);
        FloorSpawnVariant variant = new((FloorType)buildIntent.FloorType);
        Vector3 position = MapToWorld.CellToWorld(spawnKey.Coord);

        ClientServices.SpawnManager.Spawn(spawnKey, variant, position, SpawnLayer.Floors);
    }

    private void RemoveFloor(BuildOperationResult result)
    {
        CellSlotSpawnKey spawnKey = GetSpawnKey(result);
        ClientServices.SpawnManager.Despawn(spawnKey);
    }

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
}