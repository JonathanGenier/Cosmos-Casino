using Godot;
using System;

/// <summary>
/// Immutable container for client-side terrain rendering resources.
/// Provides strongly-typed access to preloaded scenes required to
/// visually represent terrain chunks.
/// This class is assembly-only and intentionally non-mutable after creation
/// to guarantee consistency across terrain rendering systems.
/// </summary>
public sealed class TerrainResources
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the PreviewResources class with the specified grid preview scene.
    /// </summary>
    /// <param name="chunkScene">The PackedScene instance to use for grid previews. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="chunkScene"/> is null.</exception>
    private TerrainResources(PackedScene chunkScene)
    {
        TerrainChunkViewScene = chunkScene ?? throw new ArgumentNullException(nameof(chunkScene));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the packed scene used to display a terrain chunk.
    /// </summary>
    public PackedScene TerrainChunkViewScene { get; }

    #endregion

    #region Assembly

    /// <summary>
    /// Creates a new instance of <see cref="PreviewResources"/> using the specified resource preloader.
    /// </summary>
    /// <param name="resources">The resource preloader containing the assets required to assemble the preview resources. Cannot be <see
    /// langword="null"/>.</param>
    /// <returns>A <see cref="PreviewResources"/> object initialized with the packed scene from the provided resources.</returns>
    public static TerrainResources Assemble(ResourcePreloader resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        return new TerrainResources(
            ResourceResolver.GetPackedScene(resources, "terrain_chunk_view")
        );
    }

    #endregion
}