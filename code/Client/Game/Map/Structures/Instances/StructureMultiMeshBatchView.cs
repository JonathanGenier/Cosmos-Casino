using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// View node for one repeated-structure MultiMesh batch.
/// </summary>
internal sealed partial class StructureMultiMeshBatchView : Node3D
{
    #region Fields

    private const int InitialCapacity = 4;

    private readonly List<StructureInstanceEntry> _entries = new();
    private readonly Dictionary<StructureId, int> _slotsByStructureId = new();

    private MultiMesh? _multiMesh;
    private MultiMeshInstance3D? _multiMeshInstance;
    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the batch key represented by this view.
    /// </summary>
    internal StructureInstanceBatchKey Key { get; private set; }

    /// <summary>
    /// Gets the number of active visible instances in the batch.
    /// </summary>
    internal int ActiveCount => _entries.Count;

    private MultiMesh MultiMesh
    {
        get => _multiMesh ?? throw new InvalidOperationException($"{nameof(StructureMultiMeshBatchView)} has not been initialized.");
        set => _multiMesh = value;
    }

    private MultiMeshInstance3D MultiMeshInstance
    {
        get => _multiMeshInstance ?? throw new InvalidOperationException($"{nameof(StructureMultiMeshBatchView)} has not been initialized.");
        set => _multiMeshInstance = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the batch view.
    /// </summary>
    /// <param name="key">The batch key.</param>
    /// <param name="presentation">The repeated structure presentation.</param>
    internal void Initialize(
        StructureInstanceBatchKey key,
        StructurePresentationDefinition presentation)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureMultiMeshBatchView)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(presentation);

        if (presentation.RenderStrategy != StructureRenderStrategy.MultiMesh)
        {
            throw new ArgumentException("Batch view requires a MultiMesh presentation.", nameof(presentation));
        }

        Key = key;
        Position = StructureRenderSectionMath.ToSectionWorldOrigin(key.SectionCoord);
        MultiMesh = CreateMultiMesh(presentation);
        MultiMeshInstance = new MultiMeshInstance3D
        {
            Name = "StructureMultiMesh",
            Multimesh = MultiMesh,
            MaterialOverride = presentation.Material
        };

        AddChild(MultiMeshInstance);
        _isInitialized = true;
    }

    #endregion

    #region Instances

    /// <summary>
    /// Adds one structure instance to the batch.
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
            throw new InvalidOperationException($"Structure instance '{structureId}' already exists in batch '{Key}'.");
        }

        EnsureCapacity(_entries.Count + 1);

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
        MultiMesh.SetInstanceTransform(slot, transform);
        MultiMesh.VisibleInstanceCount = _entries.Count;

        return slot;
    }

    /// <summary>
    /// Removes one structure instance from the batch using swap-back compaction.
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
            MultiMesh.SetInstanceTransform(slot, movedEntry.Transform);

            movedStructureId = movedEntry.StructureId;
            movedSlot = slot;
        }

        _entries.RemoveAt(lastSlot);
        _slotsByStructureId.Remove(structureId);
        MultiMesh.VisibleInstanceCount = _entries.Count;
        return true;
    }

    #endregion

    #region Helpers

    private static MultiMesh CreateMultiMesh(StructurePresentationDefinition presentation)
    {
        var multiMesh = new MultiMesh
        {
            Mesh = presentation.Mesh,
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseColors = presentation.UseColors,
            UseCustomData = presentation.UseCustomData,
            InstanceCount = InitialCapacity,
            VisibleInstanceCount = 0
        };

        return multiMesh;
    }

    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity <= MultiMesh.InstanceCount)
        {
            return;
        }

        int newCapacity = Math.Max(InitialCapacity, MultiMesh.InstanceCount);

        while (newCapacity < requiredCapacity)
        {
            newCapacity *= 2;
        }

        MultiMesh.InstanceCount = newCapacity;
        ReplayTransforms();
    }

    private void ReplayTransforms()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            MultiMesh.SetInstanceTransform(i, _entries[i].Transform);
        }

        MultiMesh.VisibleInstanceCount = _entries.Count;
    }

    #endregion
}
