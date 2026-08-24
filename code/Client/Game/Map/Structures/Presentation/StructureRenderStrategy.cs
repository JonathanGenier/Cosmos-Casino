/// <summary>
/// Client-owned rendering strategy for an authoritative Core structure definition.
/// </summary>
internal enum StructureRenderStrategy
{
    /// <summary>
    /// The structure participates in generated section mesh rendering.
    /// </summary>
    GeneratedSectionMesh,

    /// <summary>
    /// The structure renders through section-partitioned MultiMesh batches.
    /// </summary>
    MultiMesh,

    /// <summary>
    /// The structure renders through one reusable scene instance per authoritative Structure.
    /// </summary>
    Scene
}
