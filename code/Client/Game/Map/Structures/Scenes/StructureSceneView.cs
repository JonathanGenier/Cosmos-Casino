using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// Transient Client view for one scene-rendered authoritative Structure.
/// </summary>
internal sealed partial class StructureSceneView : Node3D
{
    #region Fields

    private Node3D? _sceneRoot;
    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative structure identity represented by this view.
    /// </summary>
    internal StructureId StructureId { get; private set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the scene view from an authoritative structure snapshot and Client presentation metadata.
    /// </summary>
    /// <param name="snapshot">The authoritative structure snapshot.</param>
    /// <param name="presentation">The scene-rendered presentation metadata.</param>
    internal void Initialize(
        StructureSnapshot snapshot,
        StructurePresentationDefinition presentation)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureSceneView)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(presentation);

        if (presentation.RenderStrategy != StructureRenderStrategy.Scene)
        {
            throw new ArgumentException("Scene view requires a Scene presentation.", nameof(presentation));
        }

        StructureId = snapshot.Id;
        Transform = GridPlacementTransformResolver.ResolveWorldTransform(
            StructureGridMetrics.ToGodotCenter(snapshot.Anchor),
            snapshot.Rotation);
        _sceneRoot = InstantiateSceneRoot(presentation);
        AddChild(_sceneRoot);
        AddCollisionProxies(snapshot, presentation);
        _isInitialized = true;
    }

    #endregion

    #region Collision

    private void AddCollisionProxies(
        StructureSnapshot snapshot,
        StructurePresentationDefinition presentation)
    {
        foreach (MapCellCoord cell in snapshot.Definition.Footprint.Resolve(snapshot.Anchor, snapshot.Rotation))
        {
            AddCollisionProxy(cell, presentation.LocalBoundsSize);
        }
    }

    private void AddCollisionProxy(
        MapCellCoord cell,
        Vector3 localBoundsSize)
    {
        var body = new StaticBody3D
        {
            Name = $"StructureSceneCollision_{cell.X}_{cell.Y}_{cell.Z}",
            CollisionLayer = CollisionLayers.Buildable,
            CollisionMask = CollisionLayers.None
        };
        var collisionShape = new CollisionShape3D
        {
            Name = "StructureSceneCollisionShape",
            Shape = new BoxShape3D
            {
                Size = localBoundsSize
            }
        };
        var pickTarget = new StructureSceneCollisionTarget();
        pickTarget.Initialize(cell);

        body.AddChild(collisionShape);
        body.AddChild(pickTarget);
        AddChild(body);
        body.GlobalTransform = new Transform3D(Basis.Identity, StructureGridMetrics.ToGodotCenter(cell));
    }

    #endregion

    #region Helpers

    private Node3D InstantiateSceneRoot(StructurePresentationDefinition presentation)
    {
        Node scene = presentation.Scene.Instantiate();

        if (scene is not Node3D sceneRoot)
        {
            throw new InvalidOperationException(
                $"Scene presentation '{presentation.PresentationKey}' must instantiate a {nameof(Node3D)}.");
        }

        sceneRoot.Position = presentation.SceneLocalOffset;
        return sceneRoot;
    }

    #endregion
}
