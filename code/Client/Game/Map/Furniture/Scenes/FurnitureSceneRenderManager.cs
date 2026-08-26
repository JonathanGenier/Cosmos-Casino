using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns scene-rendered Client views for authoritative Core furniture.
/// </summary>
public sealed partial class FurnitureSceneRenderManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<FurnitureId, FurnitureSceneView> _viewsByFurnitureId = new();

    private MapManager? _mapManager;
    private FurniturePresentationCatalog? _presentationCatalog;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live scene-rendered furniture views owned by this Client projection.
    /// </summary>
    internal int ViewCount => _viewsByFurnitureId.Count;

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    private FurniturePresentationCatalog PresentationCatalog
    {
        get => _presentationCatalog ?? throw new InvalidOperationException($"{nameof(FurniturePresentationCatalog)} has not been initialized.");
        set => _presentationCatalog = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the scene-rendered furniture projection and reconstructs from authoritative Core state.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    /// <param name="presentationCatalog">The Client furniture presentation catalog.</param>
    public void Initialize(
        MapManager mapManager,
        FurniturePresentationCatalog presentationCatalog)
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
    /// Rebuilds the complete disposable furniture projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        ClearViews();

        foreach (FurnitureSnapshot snapshot in MapManager.GetFurnitureSnapshots())
        {
            if (PresentationCatalog.TryGetDefinition(snapshot.Definition.Id, out FurniturePresentationDefinition presentation))
            {
                AddView(snapshot, presentation);
            }
        }
    }

    /// <summary>
    /// Applies successful authoritative furniture changes to the scene-rendered projection.
    /// </summary>
    /// <param name="changes">The completed furniture changes to apply.</param>
    public void ApplyChanges(IReadOnlyList<FurnitureChangeResult> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        foreach (FurnitureChangeResult change in changes)
        {
            switch (change.Kind)
            {
                case FurnitureChangeResultKind.Created:
                    AddCreatedView(change);
                    break;

                case FurnitureChangeResultKind.Removed:
                    RemoveView(change.FurnitureId);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported furniture change kind: {change.Kind}");
            }
        }
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Releases scene-rendered furniture views owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        ClearViews();
    }

    #endregion

    #region View Operations

    private void AddCreatedView(FurnitureChangeResult change)
    {
        if (!PresentationCatalog.TryGetDefinition(change.DefinitionId, out FurniturePresentationDefinition presentation))
        {
            return;
        }

        if (!MapManager.TryGetFurnitureSnapshotAt(change.Anchor, out FurnitureSnapshot snapshot))
        {
            throw new InvalidOperationException($"Created furniture '{change.FurnitureId}' was missing from Core state.");
        }

        AddView(snapshot, presentation);
    }

    private void AddView(
        FurnitureSnapshot snapshot,
        FurniturePresentationDefinition presentation)
    {
        RemoveView(snapshot.Id);

        var view = new FurnitureSceneView();
        view.Initialize(snapshot, presentation);
        AddChild(view);
        _viewsByFurnitureId.Add(snapshot.Id, view);
    }

    private void RemoveView(FurnitureId furnitureId)
    {
        if (!_viewsByFurnitureId.Remove(furnitureId, out FurnitureSceneView? view))
        {
            return;
        }

        view.QueueFree();
    }

    private void ClearViews()
    {
        foreach (FurnitureSceneView view in _viewsByFurnitureId.Values.ToArray())
        {
            view.QueueFree();
        }

        _viewsByFurnitureId.Clear();
    }

    #endregion
}
