/// <summary>
/// Dense-slot handle for a repeated structure instance inside a MultiMesh batch.
/// </summary>
internal readonly struct StructureInstanceHandle
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance handle.
    /// </summary>
    /// <param name="batchKey">The batch that owns the instance.</param>
    /// <param name="slot">The dense MultiMesh slot.</param>
    internal StructureInstanceHandle(
        StructureInstanceBatchKey batchKey,
        int slot)
    {
        BatchKey = batchKey;
        Slot = slot;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the batch that owns the instance.
    /// </summary>
    internal StructureInstanceBatchKey BatchKey { get; }

    /// <summary>
    /// Gets the dense MultiMesh slot.
    /// </summary>
    internal int Slot { get; }

    #endregion
}
