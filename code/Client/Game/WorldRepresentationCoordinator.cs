using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;

/// <summary>
/// Routes authoritative world mutation results to specialized disposable Client representation managers.
/// </summary>
internal sealed class WorldRepresentationCoordinator
{
    #region Fields

    private readonly StructurePresentationCatalog _structurePresentationCatalog;
    private readonly StructureRenderManager _structureRenderManager;
    private readonly StructureCollisionManager _structureCollisionManager;
    private readonly StructureInstanceRenderManager _structureInstanceRenderManager;
    private readonly StructureInstanceCollisionManager _structureInstanceCollisionManager;
    private readonly StructureSceneRenderManager _structureSceneRenderManager;
    private readonly FurnitureSceneRenderManager _furnitureSceneRenderManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a world representation coordinator.
    /// </summary>
    /// <param name="structurePresentationCatalog">The Client structure presentation catalog.</param>
    /// <param name="structureRenderManager">The generated structure renderer.</param>
    /// <param name="structureCollisionManager">The generated structure collision projection.</param>
    /// <param name="structureInstanceRenderManager">The repeated structure renderer.</param>
    /// <param name="structureInstanceCollisionManager">The repeated structure collision projection.</param>
    /// <param name="structureSceneRenderManager">The scene-rendered structure projection.</param>
    /// <param name="furnitureSceneRenderManager">The scene-rendered furniture projection.</param>
    internal WorldRepresentationCoordinator(
        StructurePresentationCatalog structurePresentationCatalog,
        StructureRenderManager structureRenderManager,
        StructureCollisionManager structureCollisionManager,
        StructureInstanceRenderManager structureInstanceRenderManager,
        StructureInstanceCollisionManager structureInstanceCollisionManager,
        StructureSceneRenderManager structureSceneRenderManager,
        FurnitureSceneRenderManager furnitureSceneRenderManager)
    {
        ArgumentNullException.ThrowIfNull(structurePresentationCatalog);
        ArgumentNullException.ThrowIfNull(structureRenderManager);
        ArgumentNullException.ThrowIfNull(structureCollisionManager);
        ArgumentNullException.ThrowIfNull(structureInstanceRenderManager);
        ArgumentNullException.ThrowIfNull(structureInstanceCollisionManager);
        ArgumentNullException.ThrowIfNull(structureSceneRenderManager);
        ArgumentNullException.ThrowIfNull(furnitureSceneRenderManager);

        _structurePresentationCatalog = structurePresentationCatalog;
        _structureRenderManager = structureRenderManager;
        _structureCollisionManager = structureCollisionManager;
        _structureInstanceRenderManager = structureInstanceRenderManager;
        _structureInstanceCollisionManager = structureInstanceCollisionManager;
        _structureSceneRenderManager = structureSceneRenderManager;
        _furnitureSceneRenderManager = furnitureSceneRenderManager;
    }

    #endregion

    #region Reconstruction

    /// <summary>
    /// Rebuilds every disposable world representation from authoritative Core snapshots.
    /// </summary>
    internal void RebuildAllFromAuthoritativeState()
    {
        _structureRenderManager.RebuildAllFromAuthoritativeState();
        _structureCollisionManager.RebuildAllFromAuthoritativeState();
        _structureInstanceRenderManager.RebuildAllFromAuthoritativeState();
        _structureInstanceCollisionManager.RebuildAllFromAuthoritativeState();
        _structureSceneRenderManager.RebuildAllFromAuthoritativeState();
        _furnitureSceneRenderManager.RebuildAllFromAuthoritativeState();
    }

    #endregion

    #region Routing

    /// <summary>
    /// Applies one completed authoritative build transaction to disposable Client representations.
    /// </summary>
    /// <param name="buildResult">The completed authoritative build result.</param>
    internal void ApplyBuildResult(BuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

        if (buildResult.Outcome != BuildOperationOutcome.Valid)
        {
            return;
        }

        StructureRoutingBatch batch = ClassifyStructureChanges(buildResult.Structures);

        if (batch.GeneratedCells.Count > 0)
        {
            IReadOnlyList<MapCellCoord> generatedCells = SortCells(batch.GeneratedCells);
            _structureRenderManager.RebuildAffectedCells(generatedCells);
            _structureCollisionManager.RebuildAffectedCells(generatedCells);
        }

        if (batch.MultiMeshChanges.Count > 0)
        {
            _structureInstanceRenderManager.ApplyChanges(batch.MultiMeshChanges);
            _structureInstanceCollisionManager.ApplyChanges(batch.MultiMeshChanges);
        }

        if (batch.SceneChanges.Count > 0)
        {
            _structureSceneRenderManager.ApplyChanges(batch.SceneChanges);
        }
    }

    /// <summary>
    /// Applies one completed authoritative furniture operation to disposable Client representations.
    /// </summary>
    /// <param name="furnitureResult">The completed authoritative furniture result.</param>
    internal void ApplyFurnitureResult(FurnitureOperationResult furnitureResult)
    {
        ArgumentNullException.ThrowIfNull(furnitureResult);

        if (furnitureResult.Outcome != FurnitureOperationOutcome.Valid)
        {
            return;
        }

        _furnitureSceneRenderManager.ApplyChanges(furnitureResult.Changes);
    }

    #endregion

    #region Structure Classification

    private static void AddAffectedCells(
        HashSet<MapCellCoord> generatedCells,
        IReadOnlyList<MapCellCoord> affectedCells)
    {
        foreach (MapCellCoord cell in affectedCells)
        {
            generatedCells.Add(cell);
        }
    }

    #endregion

    #region Sorting

    private static IReadOnlyList<MapCellCoord> SortCells(HashSet<MapCellCoord> cells)
    {
        var sortedCells = new List<MapCellCoord>(cells);
        sortedCells.Sort(CompareCells);
        return sortedCells;
    }

    private static int CompareCells(MapCellCoord left, MapCellCoord right)
    {
        int xComparison = left.X.CompareTo(right.X);

        if (xComparison != 0)
        {
            return xComparison;
        }

        int yComparison = left.Y.CompareTo(right.Y);

        if (yComparison != 0)
        {
            return yComparison;
        }

        return left.Z.CompareTo(right.Z);
    }

    #endregion

    #region Helpers

    private StructureRoutingBatch ClassifyStructureChanges(IReadOnlyList<BuildStructureResult> structures)
    {
        var batch = new StructureRoutingBatch();

        foreach (BuildStructureResult structure in structures)
        {
            if (structure.Outcome != BuildOperationOutcome.Valid)
            {
                continue;
            }

            if (!_structurePresentationCatalog.TryGetDefinition(
                structure.DefinitionId,
                out StructurePresentationDefinition presentation))
            {
                continue;
            }

            switch (presentation.RenderStrategy)
            {
                case StructureRenderStrategy.GeneratedSectionMesh:
                    AddAffectedCells(batch.GeneratedCells, structure.AffectedCells);
                    break;

                case StructureRenderStrategy.MultiMesh:
                    batch.MultiMeshChanges.Add(structure);
                    break;

                case StructureRenderStrategy.Scene:
                    batch.SceneChanges.Add(structure);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported structure render strategy: {presentation.RenderStrategy}");
            }
        }

        return batch;
    }

    #endregion

    #region Types

    private sealed class StructureRoutingBatch
    {
        internal HashSet<MapCellCoord> GeneratedCells { get; } = new();

        internal List<BuildStructureResult> MultiMeshChanges { get; } = new();

        internal List<BuildStructureResult> SceneChanges { get; } = new();
    }

    #endregion
}
