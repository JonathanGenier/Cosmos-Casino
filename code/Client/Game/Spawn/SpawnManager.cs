using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the spawning and despawning of visual instances in the scene, organizing them by logical keys and spawn
/// layers.
/// </summary>
/// <remarks>The SpawnManager coordinates the creation, replacement, and removal of visual nodes based on logical
/// spawn keys and variants. It maintains a registry of active instances and ensures that visuals are attached to the
/// appropriate layer nodes within the scene tree. The manager must be initialized with a SpawnCatalog before use. All
/// spawned visuals are automatically cleaned up when the node exits the scene tree.</remarks>
public sealed partial class SpawnManager : InitializableNodeManager
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
    private SpawnResources? _spawnResources;

    #endregion

    #region Properties

    private SpawnResources SpawnResources
    {
        get => _spawnResources ?? throw new InvalidOperationException($"{nameof(SpawnResources)} is not initialized.");
        set => _spawnResources = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the spawn manager with the specified spawn catalog.
    /// </summary>
    /// <param name="spawnResources">The spawn catalog to associate with the spawn manager. Cannot be null.</param>
    /// <exception cref="InvalidOperationException">Thrown if the spawn manager has already been initialized with a spawn catalog.</exception>
    public void Initialize(SpawnResources spawnResources)
    {
        if (_spawnResources != null)
        {
            throw new InvalidOperationException($"{nameof(SpawnManager)} : Spawn Catalog already set.");
        }

        SpawnResources = spawnResources;
        MarkInitialized();
    }

    #endregion

    #region Spawn & Despawn Methods

    /// <summary>
    /// Spawns a new instance of the specified object at the given position and adds it to the specified spawn layer.
    /// </summary>
    /// <param name="key">The key that uniquely identifies the type of object to spawn.</param>
    /// <param name="variant">The variant information used to resolve the specific object to spawn.</param>
    /// <param name="position">The world position where the object will be spawned.</param>
    /// <param name="layer">The spawn layer to which the new object will be added.</param>
    /// <exception cref="InvalidOperationException">Thrown if the spawn manager is not configured or if the resolved spawn does not instantiate a Node3D.</exception>
    public void Spawn(ISpawnKey key, ISpawnVariant variant, Vector3 position, SpawnLayer layer)
    {
        var spawnId = SpawnResolver.Resolve(key, variant);
        var transform = new Transform3D(Basis.Identity, position);

        ReplaceIfExists(key);

        var scene = SpawnResources.GetSpawnScene(spawnId);
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

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the node and creates child nodes for each spawn layer when the scene is ready.
    /// </summary>
    /// <remarks>This method is called by the Godot engine when the node enters the scene tree. Override this
    /// method to perform setup that depends on the scene being fully loaded.</remarks>
    protected override void OnReady()
    {
        foreach (SpawnLayer layer in Enum.GetValues<SpawnLayer>())
        {
            var node = CreateSpawnLayer(layer.ToString());
            _layers[layer] = node;
            AddChild(node);
        }
    }

    /// <summary>
    /// Performs cleanup operations when the application exits.
    /// </summary>
    /// <remarks>This method is called during the application's shutdown sequence. It ensures that any
    /// remaining registered resources are properly released before the application terminates. Override this method to
    /// implement additional exit logic if necessary.</remarks>
    protected override void OnExit()
    {
        if (_registry.Count > 0)
        {
            DespawnAll();
        }
    }

    #endregion

    #region Internal Despawn Methods

    /// <summary>
    /// Despawns all currently spawned visuals and clears the registry.
    /// </summary>
    private void DespawnAll()
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
    private void DespawnInternal(ulong objectId)
    {
        if (GodotObject.InstanceFromId(objectId) is Node node)
        {
            if (node.IsQueuedForDeletion())
            {
                ConsoleLog.Warning(nameof(SpawnManager), $"Node {objectId} already queued for deletion.");
            }

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
    private Node3D CreateSpawnLayer(string name)
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