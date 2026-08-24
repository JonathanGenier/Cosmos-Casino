using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Client-owned collision projection for one generated structure section.
/// </summary>
internal sealed partial class StructureCollisionRegionView : Node3D
{
    #region Fields

    private StaticBody3D? _body;
    private CollisionShape3D? _collisionShapeNode;
    private ConcavePolygonShape3D? _collisionShape;
    private IReadOnlyList<StructureCollisionTriangleTarget> _triangleTargets =
        Array.Empty<StructureCollisionTriangleTarget>();

    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the section represented by this collision region.
    /// </summary>
    internal StructureRenderSectionCoord Coord { get; private set; }

    private StaticBody3D Body
    {
        get => _body ?? throw new InvalidOperationException($"{nameof(StructureCollisionRegionView)} has not been initialized.");
        set => _body = value;
    }

    private CollisionShape3D CollisionShapeNode
    {
        get => _collisionShapeNode ?? throw new InvalidOperationException($"{nameof(StructureCollisionRegionView)} has not been initialized.");
        set => _collisionShapeNode = value;
    }

    private ConcavePolygonShape3D CollisionShape
    {
        get => _collisionShape ?? throw new InvalidOperationException($"{nameof(StructureCollisionRegionView)} has not been initialized.");
        set => _collisionShape = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes this region with one reusable body and one reusable collision shape.
    /// </summary>
    /// <param name="coord">The generated structure section coordinate.</param>
    internal void Initialize(StructureRenderSectionCoord coord)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureCollisionRegionView)} is already initialized.");
        }

        Coord = coord;
        Position = StructureRenderSectionMath.ToSectionWorldOrigin(coord);

        Body = new StaticBody3D
        {
            Name = "StructureCollisionBody",
            CollisionLayer = CollisionLayers.Buildable,
            CollisionMask = CollisionLayers.None
        };

        CollisionShape = new ConcavePolygonShape3D();
        CollisionShapeNode = new CollisionShape3D
        {
            Name = "StructureCollisionShape",
            Shape = CollisionShape
        };

        var pickTarget = new StructureCollisionRegionTarget();
        pickTarget.Initialize(this);

        Body.AddChild(CollisionShapeNode);
        Body.AddChild(pickTarget);
        AddChild(Body);
        _isInitialized = true;
    }

    #endregion

    #region Representation

    /// <summary>
    /// Applies generated collision geometry to this region's single collision shape.
    /// </summary>
    /// <param name="buildResult">The generated collision geometry and triangle metadata.</param>
    internal void ApplyCollision(StructureCollisionMeshBuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

        CollisionShape.SetFaces(buildResult.TriangleVertices);
        CollisionShapeNode.Disabled = buildResult.TriangleVertices.Length == 0;
        _triangleTargets = buildResult.TriangleTargets;
    }

    /// <summary>
    /// Clears generated collision geometry and metadata from this region.
    /// </summary>
    internal void ClearCollision()
    {
        if (_collisionShape != null)
        {
            _collisionShape.SetFaces(Array.Empty<Vector3>());
        }

        if (_collisionShapeNode != null)
        {
            _collisionShapeNode.Disabled = true;
        }

        _triangleTargets = Array.Empty<StructureCollisionTriangleTarget>();
    }

    #endregion

    #region Hit Resolution

    /// <summary>
    /// Resolves a physics hit on this region to the global occupied cell and surface face.
    /// </summary>
    /// <param name="hit">The Godot physics hit.</param>
    /// <param name="cell">The global occupied structure cell represented by the hit.</param>
    /// <param name="face">The logical surface face represented by the hit.</param>
    /// <returns><c>true</c> when the hit can be resolved; otherwise, <c>false</c>.</returns>
    internal bool TryResolveHit(
        CursorPhysicsHit hit,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        if (TryResolveFromFaceIndex(hit.FaceIndex, out cell, out face))
        {
            return true;
        }

        return StructureCollisionHitResolver.TryResolve(
            hit.WorldPosition,
            hit.WorldNormal,
            out cell,
            out face);
    }

    private bool TryResolveFromFaceIndex(
        int faceIndex,
        out MapCellCoord cell,
        out CursorSurfaceFace face)
    {
        if (faceIndex >= 0 && faceIndex < _triangleTargets.Count)
        {
            StructureCollisionTriangleTarget target = _triangleTargets[faceIndex];
            cell = target.Cell;
            face = target.SurfaceFace;
            return true;
        }

        cell = default;
        face = default;
        return false;
    }

    #endregion
}
