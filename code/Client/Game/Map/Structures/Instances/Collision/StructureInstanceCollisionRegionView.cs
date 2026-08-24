using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Batched collision projection for one repeated-structure instance section.
/// </summary>
internal sealed partial class StructureInstanceCollisionRegionView : Node3D
{
    #region Fields

    private readonly List<StructureInstanceEntry> _entries = new();
    private readonly Dictionary<StructureId, int> _slotsByStructureId = new();

    private StaticBody3D? _body;
    private CollisionShape3D? _collisionShapeNode;
    private ConcavePolygonShape3D? _collisionShape;
    private StructurePresentationDefinition? _presentation;
    private IReadOnlyList<StructureCollisionTriangleTarget> _triangleTargets =
        Array.Empty<StructureCollisionTriangleTarget>();

    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the batch key represented by this collision region.
    /// </summary>
    internal StructureInstanceBatchKey Key { get; private set; }

    /// <summary>
    /// Gets the number of active collision instances in the region.
    /// </summary>
    internal int ActiveCount => _entries.Count;

    private CollisionShape3D CollisionShapeNode
    {
        get => _collisionShapeNode ?? throw new InvalidOperationException($"{nameof(StructureInstanceCollisionRegionView)} has not been initialized.");
        set => _collisionShapeNode = value;
    }

    private ConcavePolygonShape3D CollisionShape
    {
        get => _collisionShape ?? throw new InvalidOperationException($"{nameof(StructureInstanceCollisionRegionView)} has not been initialized.");
        set => _collisionShape = value;
    }

    private StructurePresentationDefinition Presentation
    {
        get => _presentation ?? throw new InvalidOperationException($"{nameof(StructureInstanceCollisionRegionView)} has not been initialized.");
        set => _presentation = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes this collision region with one reusable body and one reusable collision shape.
    /// </summary>
    /// <param name="key">The repeated structure batch key.</param>
    /// <param name="presentation">The repeated structure presentation.</param>
    internal void Initialize(
        StructureInstanceBatchKey key,
        StructurePresentationDefinition presentation)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureInstanceCollisionRegionView)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(presentation);

        if (presentation.RenderStrategy != StructureRenderStrategy.MultiMesh)
        {
            throw new ArgumentException("Instance collision requires a MultiMesh presentation.", nameof(presentation));
        }

        Key = key;
        Presentation = presentation;
        Position = StructureRenderSectionMath.ToSectionWorldOrigin(key.SectionCoord);

        _body = new StaticBody3D
        {
            Name = "StructureInstanceCollisionBody",
            CollisionLayer = CollisionLayers.Buildable,
            CollisionMask = CollisionLayers.None
        };

        CollisionShape = new ConcavePolygonShape3D();
        CollisionShapeNode = new CollisionShape3D
        {
            Name = "StructureInstanceCollisionShape",
            Shape = CollisionShape,
            Disabled = true
        };

        var pickTarget = new StructureInstanceCollisionTarget();
        pickTarget.Initialize(this);

        _body.AddChild(CollisionShapeNode);
        _body.AddChild(pickTarget);
        AddChild(_body);
        _isInitialized = true;
    }

    #endregion

    #region Instances

    /// <summary>
    /// Adds one structure instance to the collision region.
    /// </summary>
    /// <param name="structureId">The authoritative structure identity.</param>
    /// <param name="anchor">The authoritative structure anchor.</param>
    /// <param name="rotation">The authoritative structure rotation.</param>
    /// <returns>The dense slot assigned to the instance.</returns>
    internal int AddInstance(
        StructureId structureId,
        MapCellCoord anchor,
        FootprintRotation rotation)
    {
        if (_slotsByStructureId.ContainsKey(structureId))
        {
            throw new InvalidOperationException($"Structure instance '{structureId}' already exists in collision batch '{Key}'.");
        }

        int slot = _entries.Count;
        Transform3D transform = StructureInstanceTransformResolver.ResolveSectionLocalTransform(
            Key.SectionCoord,
            anchor,
            rotation);
        var entry = new StructureInstanceEntry(
            structureId,
            anchor,
            rotation,
            transform);

        _entries.Add(entry);
        _slotsByStructureId.Add(structureId, slot);
        RebuildCollision();
        return slot;
    }

    /// <summary>
    /// Removes one structure instance from the collision region using swap-back compaction.
    /// </summary>
    /// <param name="structureId">The authoritative structure identity to remove.</param>
    /// <param name="movedStructureId">The moved structure identity when another slot was compacted into the removed slot.</param>
    /// <param name="movedSlot">The moved structure's new slot.</param>
    /// <returns><c>true</c> when an instance was removed; otherwise, <c>false</c>.</returns>
    internal bool RemoveInstance(
        StructureId structureId,
        out StructureId? movedStructureId,
        out int movedSlot)
    {
        movedStructureId = null;
        movedSlot = -1;

        if (!_slotsByStructureId.TryGetValue(structureId, out int slot))
        {
            return false;
        }

        int lastSlot = _entries.Count - 1;

        if (slot != lastSlot)
        {
            StructureInstanceEntry movedEntry = _entries[lastSlot];
            _entries[slot] = movedEntry;
            _slotsByStructureId[movedEntry.StructureId] = slot;

            movedStructureId = movedEntry.StructureId;
            movedSlot = slot;
        }

        _entries.RemoveAt(lastSlot);
        _slotsByStructureId.Remove(structureId);
        RebuildCollision();
        return true;
    }

    #endregion

    #region Representation

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
        if (TryResolveFromFaceIndex(hit.FaceIndex, out cell, out CursorSurfaceFace metadataFace))
        {
            if (CursorSurfaceFaceResolver.TryResolve(hit.WorldNormal, out face))
            {
                return true;
            }

            face = metadataFace;
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

    #region Helpers

    private void RebuildCollision()
    {
        StructureInstanceCollisionMeshBuildResult buildResult =
            StructureInstanceCollisionMeshBuilder.Build(_entries, Presentation.LocalBoundsSize);

        CollisionShape.SetFaces(buildResult.TriangleVertices);
        CollisionShapeNode.Disabled = buildResult.TriangleCount == 0;
        _triangleTargets = buildResult.TriangleTargets;
    }

    #endregion
}
