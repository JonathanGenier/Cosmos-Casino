
/// <summary>
/// Represents a descriptor for spawning a build variant within a specific layer.
/// </summary>
/// <remarks>Use this structure to encapsulate the information required to spawn a particular variant in a
/// designated layer. This is typically used when managing build processes that involve multiple spawn variants and
/// layers.</remarks>
public readonly struct BuildSpawnDescriptor
{
    /// <summary>
    /// Initializes a new instance of the BuildSpawnDescriptor class with the specified spawn variant and layer.
    /// </summary>
    /// <param name="variant">The spawn variant that defines the characteristics of the entity to be spawned. Cannot be null.</param>
    /// <param name="layer">The spawn layer that determines where the entity will be placed within the environment.</param>
    public BuildSpawnDescriptor(ISpawnVariant variant, SpawnLayer layer)
    {
        Variant = variant;
        Layer = layer;
    }

    /// <summary>
    /// Gets the spawn variant associated with this instance.
    /// </summary>
    public ISpawnVariant Variant { get; }

    /// <summary>
    /// Gets the spawn layer associated with this instance.
    /// </summary>
    public SpawnLayer Layer { get; }
}
