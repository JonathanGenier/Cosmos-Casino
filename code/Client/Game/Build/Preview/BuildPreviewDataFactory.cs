using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Adapts authoritative Core operation results into Client-only preview presentation data.
/// </summary>
internal static class BuildPreviewDataFactory
{
    #region Structure

    /// <summary>
    /// Creates preview data from a Structure build result.
    /// </summary>
    /// <param name="buildResult">The authoritative Structure build evaluation result.</param>
    /// <returns>Client-only preview data.</returns>
    internal static BuildPreviewData FromBuildResult(BuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

        return new BuildPreviewData(
            GetStructurePreviewCells(buildResult),
            ToPreviewValidity(buildResult.Outcome));
    }

    #endregion

    #region Furniture

    /// <summary>
    /// Creates preview data from a Furniture placement request and evaluation result.
    /// </summary>
    /// <param name="request">The authoritative Furniture placement request being previewed.</param>
    /// <param name="result">The authoritative Furniture placement evaluation result.</param>
    /// <returns>Client-only preview data.</returns>
    internal static BuildPreviewData FromFurniturePlacement(
        FurniturePlacementRequest request,
        FurnitureOperationResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        return new BuildPreviewData(
            GetFurniturePreviewCells(request),
            ToPreviewValidity(result.Outcome));
    }

    #endregion

    #region Structure Cells

    private static IReadOnlyList<MapCellCoord> GetStructurePreviewCells(BuildResult buildResult)
    {
        if (buildResult.Structures.Count > 0)
        {
            return buildResult.Structures
                .SelectMany(structure => structure.AffectedCells)
                .ToArray();
        }

        return buildResult.Intent.Operation switch
        {
            BuildOperation.Place => buildResult.Intent.PlacementRequests
                .SelectMany(GetStructurePlacementPreviewCells)
                .ToArray(),
            BuildOperation.Remove => buildResult.Intent.RemovalRequests
                .Select(request => request.TargetCell)
                .ToArray(),
            _ => Array.Empty<MapCellCoord>()
        };
    }

    private static IReadOnlyList<MapCellCoord> GetStructurePlacementPreviewCells(StructurePlacementRequest request)
    {
        try
        {
            return request.Definition.Footprint.Resolve(request.Anchor, request.Rotation);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new[] { request.Anchor };
        }
    }

    #endregion

    #region Furniture Cells

    private static IReadOnlyList<MapCellCoord> GetFurniturePreviewCells(FurniturePlacementRequest request)
    {
        try
        {
            return request.Definition.Footprint.Resolve(request.Anchor, request.Rotation);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new[] { request.Anchor };
        }
    }

    #endregion

    #region Validity

    private static BuildPreviewValidity ToPreviewValidity(BuildOperationOutcome outcome)
    {
        return outcome switch
        {
            BuildOperationOutcome.Valid => BuildPreviewValidity.Valid,
            BuildOperationOutcome.NoOp => BuildPreviewValidity.NoOp,
            BuildOperationOutcome.Invalid => BuildPreviewValidity.Invalid,
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "Unsupported build outcome.")
        };
    }

    private static BuildPreviewValidity ToPreviewValidity(FurnitureOperationOutcome outcome)
    {
        return outcome switch
        {
            FurnitureOperationOutcome.Valid => BuildPreviewValidity.Valid,
            FurnitureOperationOutcome.NoOp => BuildPreviewValidity.NoOp,
            FurnitureOperationOutcome.Invalid => BuildPreviewValidity.Invalid,
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "Unsupported furniture outcome.")
        };
    }

    #endregion
}
