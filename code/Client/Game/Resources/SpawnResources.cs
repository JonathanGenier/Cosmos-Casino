using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides a catalog of spawnable scene resources, enabling lookup and retrieval of packed scenes by unique
/// identifier.
/// </summary>
/// <remarks>Use this class to assemble and manage a collection of packed scenes for spawning entities at runtime.
/// SpawnResources instances are typically created via the static Assemble method, which loads scenes from a
/// ResourcePreloader and ensures each resource key is unique. The class is sealed and does not support modification
/// after creation.</remarks>
public sealed class SpawnResources
{
    #region Fields

    /// <summary>
    /// Stores a mapping of spawn identifiers to their corresponding packed scene instances.
    /// </summary>
    /// <remarks>This dictionary is used to retrieve the appropriate scene for spawning based on a string key.
    /// The keys typically represent unique identifiers for each spawnable entity.</remarks>
    private readonly Dictionary<string, PackedScene> _spawnScenes;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the SpawnResources class with the specified spawn scene mappings.
    /// </summary>
    /// <param name="spawnScenes">A dictionary containing scene names as keys and their corresponding PackedScene objects as values. Provides the
    /// initial set of spawnable scenes for this instance.</param>
    private SpawnResources(Dictionary<string, PackedScene> spawnScenes)
    {
        _spawnScenes = new Dictionary<string, PackedScene>(spawnScenes);
    }

    #endregion

    #region Assembly

    /// <summary>
    /// Creates a new SpawnResources instance by assembling packed scenes from the specified resource preloader.
    /// </summary>
    /// <param name="preloader">The ResourcePreloader containing the resources to be assembled. Cannot be null.</param>
    /// <returns>A SpawnResources object containing all packed scenes loaded from the preloader. Each scene is mapped by its
    /// resource key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resource preloader contains duplicate resource keys.</exception>
    public static SpawnResources Assemble(ResourcePreloader preloader)
    {
        ArgumentNullException.ThrowIfNull(preloader);

        var spawnScenes = new Dictionary<string, PackedScene>();

        foreach (string key in preloader.GetResourceList())
        {
            var scene = ResourceResolver.GetPackedScene(preloader, key);

            if (!spawnScenes.TryAdd(key, scene))
            {
                throw new InvalidOperationException($"Duplicate spawn resource key '{key}'.");
            }

        }

        return new SpawnResources(spawnScenes);
    }

    #endregion

    #region Resolution

    /// <summary>
    /// Resolves a spawn identifier to its associated
    /// <see cref="PackedScene"/>.
    /// </summary>
    /// <param name="spawnId">
    /// The unique identifier of the spawnable scene.
    /// </param>
    /// <returns>
    /// The <see cref="PackedScene"/> associated with the specified spawn id.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the specified spawn id is not registered in the catalog.
    /// </exception>
    public PackedScene GetSpawnScene(string spawnId)
    {
        if (!_spawnScenes.TryGetValue(spawnId, out var scene))
        {
            throw new KeyNotFoundException($"SpawnId '{spawnId}' is not registered.");
        }

        return scene;
    }

    #endregion
}
