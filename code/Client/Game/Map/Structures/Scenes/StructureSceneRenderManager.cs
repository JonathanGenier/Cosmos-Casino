using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns scene-rendered Client views for authoritative Core structures.
/// </summary>
public sealed partial class StructureSceneRenderManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<StructureId, StructureSceneView> _viewsByStructureId = new();

    private MapManager? _mapManager;
    private StructurePresentationCatalog? _presentationCatalog;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live scene-rendered structure views owned by this Client projection.
    /// </summary>
    internal int ViewCount => _viewsByStructureId.Count;

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    private StructurePresentationCatalog PresentationCatalog
    {
        get => _presentationCatalog ?? throw new InvalidOperationException($"{nameof(StructurePresentationCatalog)} has not been initialized.");
        set => _presentationCatalog = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the scene-rendered structure projection and reconstructs from authoritative Core state.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    /// <param name="presentationCatalog">The Client presentation catalog.</param>
    public void Initialize(
        MapManager mapManager,
        StructurePresentationCatalog presentationCatalog)
    {
        ArgumentNullException.ThrowIfNull(mapManager);
        ArgumentNullException.ThrowIfNull(presentationCatalog);

        MapManager = mapManager;
        PresentationCatalog = presentationCatalog;
        RebuildAllFromAuthoritativeState();
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Rebuilds the complete disposable scene-rendered projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        ClearViews();

        foreach (StructureSnapshot snapshot in MapManager.GetStructureSnapshots())
        {
            if (TryGetScenePresentation(snapshot.Definition.Id, out StructurePresentationDefinition presentation))
            {
                AddView(snapshot, presentation);
            }
        }
    }

    /// <summary>
    /// Applies one successful authoritative build result to the scene-rendered structure projection.
    /// </summary>
    /// <param name="buildResult">The completed authoritative build result.</param>
    public void ApplyBuildResult(BuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

        if (buildResult.Outcome == BuildOperationOutcome.Invalid
            || buildResult.Outcome == BuildOperationOutcome.NoOp)
        {
            return;
        }

        ApplyChanges(buildResult.Structures);
    }

    /// <summary>
    /// Applies scene-rendered structure changes from one successful authoritative transaction.
    /// </summary>
    /// <param name="structures">The already-classified structure changes to apply.</param>
    public void ApplyChanges(IReadOnlyList<BuildStructureResult> structures)
    {
        ArgumentNullException.ThrowIfNull(structures);

        foreach (BuildStructureResult structure in structures)
        {
            if (structure.Outcome != BuildOperationOutcome.Valid)
            {
                continue;
            }

            switch (structure.Kind)
            {
                case BuildStructureResultKind.Created:
                    AddCreatedView(structure);
                    break;

                case BuildStructureResultKind.Removed:
                    RemoveView(structure.StructureId);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported structure result kind: {structure.Kind}");
            }
        }
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Releases scene-rendered structure views owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        ClearViews();
    }

    #endregion

    #region View Operations

    private void AddCreatedView(BuildStructureResult structure)
    {
        if (!TryGetScenePresentation(structure.DefinitionId, out StructurePresentationDefinition presentation))
        {
            return;
        }

        if (structure.AffectedCells.Count == 0)
        {
            throw new InvalidOperationException($"Created scene-rendered structure '{structure.StructureId}' had no affected cells.");
        }

        MapCellCoord occupiedCell = structure.AffectedCells[0];

        if (!MapManager.TryGetStructureSnapshotAt(occupiedCell, out StructureSnapshot snapshot))
        {
            throw new InvalidOperationException(
                $"Created scene-rendered structure '{structure.StructureId}' could not be resolved from occupied cell '{occupiedCell}'.");
        }

        if (snapshot.Id != structure.StructureId)
        {
            throw new InvalidOperationException(
                $"Occupied cell '{occupiedCell}' resolved structure '{snapshot.Id}' instead of expected '{structure.StructureId}'.");
        }

        AddView(snapshot, presentation);
    }

    private void AddView(
        StructureSnapshot snapshot,
        StructurePresentationDefinition presentation)
    {
        RemoveView(snapshot.Id);

        var view = new StructureSceneView();
        view.Initialize(snapshot, presentation);
        AddChild(view);
        _viewsByStructureId.Add(snapshot.Id, view);
    }

    private void RemoveView(StructureId structureId)
    {
        if (!_viewsByStructureId.Remove(structureId, out StructureSceneView? view))
        {
            return;
        }

        view.QueueFree();
    }

    private void ClearViews()
    {
        foreach (StructureSceneView view in _viewsByStructureId.Values.ToArray())
        {
            view.QueueFree();
        }

        _viewsByStructureId.Clear();
    }

    #endregion

    #region Helpers

    private bool TryGetScenePresentation(
        StructureDefinitionId definitionId,
        out StructurePresentationDefinition presentation)
    {
        if (PresentationCatalog.TryGetDefinition(definitionId, out presentation)
            && presentation.RenderStrategy == StructureRenderStrategy.Scene)
        {
            return true;
        }

        presentation = null!;
        return false;
    }

    #endregion
}
