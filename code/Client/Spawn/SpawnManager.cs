using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Client-side manager responsible for spawning and despawning
/// visual instances based on logical spawn keys and variants.
/// Owns spawned node hierarchy and enforces one-to-one visual identity
/// per spawn key.
/// </summary>
/// <param name="bootstrap">
/// Bootstrap context providing access to client services.
/// </param>
public sealed partial class SpawnManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region Fields

    /// <summary>
    /// Mapping of spawn layers to their corresponding scene tree nodes.
    /// </summary>
    private readonly Dictionary<SpawnLayer, Node3D> _layers = new();

    /// <summary>
    /// Registry mapping logical spawn keys to live Godot instance IDs.
    /// </summary>
    private readonly SpawnRegistry _registry = new();

    /// <summary>
    /// Catalog resolving spawn identifiers to spawnable scenes.
    /// </summary>
    private SpawnCatalog _catalog;

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes spawn layer nodes and attaches them to the scene tree.
    /// </summary>
    public override void _Ready()
    {
        foreach (SpawnLayer layer in Enum.GetValues<SpawnLayer>())
        {
            var node = CreateSpawnLayer(layer.ToString());
            _layers[layer] = node;
            AddChild(node);
        }
    }

    /// <summary>
    /// Cleans up all spawned visuals when the node exits the scene tree.
    /// </summary>
    public override void _ExitTree()
    {
        if (_registry.Count > 0)
        {
            DespawnAll();
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Configures the spawn manager with a spawn catalog.
    /// This method may only be called once during the manager lifetime.
    /// </summary>
    /// <param name="catalog">
    /// Catalog containing all spawnable scenes.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the spawn manager has already been configured.
    /// </exception>
    public void Configure(SpawnCatalog catalog)
    {
        if (_catalog != null)
        {
            throw new InvalidOperationException("SpawnManager already configured.");
        }

        _catalog = catalog;
    }

    #endregion

    #region Spawn & Despawn

    /// <summary>
    /// Spawns or replaces a visual instance for the specified spawn key.
    /// Any existing visual associated with the key is removed before spawning.
    /// </summary>
    /// <param name="key">
    /// Logical key uniquely identifying the spawn location and slot.
    /// </param>
    /// <param name="variant">
    /// Variant describing which concrete visual should be spawned.
    /// </param>
    /// <param name="position">
    /// World-space position where the visual should be spawned.
    /// </param>
    /// <param name="layer">
    /// Spawn layer determining the visual parent node.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the spawn manager has not been configured or if the resolved
    /// spawn scene does not instantiate a Node3D.
    /// </exception>
    public void Spawn(ISpawnKey key, ISpawnVariant variant, Vector3 position, SpawnLayer layer)
    {
        if (_catalog == null)
        {
            throw new InvalidOperationException("SpawnManager not configured.");
        }

        var spawnId = SpawnResolver.Resolve(key, variant);
        var transform = new Transform3D(Basis.Identity, position);

        ReplaceIfExists(key);

        var scene = _catalog.GetSpawnScene(spawnId);
        var instance = scene.Instantiate();

        if (instance is not Node3D node)
        {
            throw new InvalidOperationException($"SpawnId '{spawnId}' does not instantiate a Node3D.");
        }

        node.Transform = transform;
        AddChildToSpawnLayer(node, layer);

        _registry.Add(key, node.GetInstanceId());
    }

    /// <summary>
    /// Despawns the visual instance associated with the specified spawn key.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key identifying the visual to remove.
    /// </param>
    public void Despawn(ISpawnKey key)
    {
        if (_registry.TryGet(key, out var objectId))
        {
            DespawnInternal(objectId);
            _registry.Remove(key);
        }
    }

    /// <summary>
    /// Despawns all currently spawned visuals and clears the registry.
    /// </summary>
    public void DespawnAll()
    {
        foreach (var id in _registry.SnapshotIds())
        {
            DespawnInternal(id);
        }

        _registry.Clear();
    }

    /// <summary>
    /// Removes a spawned node instance from the scene tree using its instance ID.
    /// </summary>
    /// <param name="objectId">
    /// Godot instance ID of the node to despawn.
    /// </param>
    private static void DespawnInternal(ulong objectId)
    {
        if (GodotObject.InstanceFromId(objectId) is Node node)
        {
            node.QueueFree();
        }
    }

    #endregion

    #region Spawn Layers

    /// <summary>
    /// Creates a new node representing a spawn layer.
    /// </summary>
    /// <param name="name">
    /// Name assigned to the spawn layer node.
    /// </param>
    /// <returns>
    /// Newly created Node3D representing the spawn layer.
    /// </returns>
    private static Node3D CreateSpawnLayer(string name)
    {
        return new Node3D { Name = name };
    }

    /// <summary>
    /// Attaches a spawned node to the specified spawn layer.
    /// </summary>
    /// <param name="node">
    /// Spawned node to attach.
    /// </param>
    /// <param name="layer">
    /// Target spawn layer.
    /// </param>
    private void AddChildToSpawnLayer(Node3D node, SpawnLayer layer)
    {
        var parent = GetSpawnLayer(layer);
        parent.AddChild(node);
    }

    /// <summary>
    /// Retrieves the node associated with the specified spawn layer.
    /// </summary>
    /// <param name="layer">
    /// Spawn layer to resolve.
    /// </param>
    /// <returns>
    /// Node representing the resolved spawn layer.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the requested spawn layer has not been initialized.
    /// </exception>
    private Node3D GetSpawnLayer(SpawnLayer layer)
    {
        if (!_layers.TryGetValue(layer, out var parent))
        {
            throw new InvalidOperationException(
                $"Spawn layer '{layer}' has not been initialized.");
        }

        return parent;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Removes any existing visual associated with the specified spawn key.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key to replace.
    /// </param>
    private void ReplaceIfExists(ISpawnKey key)
    {
        if (_registry.TryGet(key, out var existingId))
        {
            DespawnInternal(existingId);
            _registry.Remove(key);
        }
    }

    #endregion
}
