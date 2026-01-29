using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Chunk;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the client-side rendering lifecycle of terrain chunks.
/// Responsible for instantiating, initializing, and owning <see cref="TerrainChunkView"/> nodes
/// that visually represent authoritative terrain data managed by <see cref="TerrainManager"/>.
/// This class acts strictly as a view-layer coordinator and does not generate or mutate terrain data.
/// </summary>
public sealed partial class TerrainRenderManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<TerrainChunkGridCoord, TerrainChunkView> _chunkViews = new();
    private TerrainManager? _terrainManager;
    private PackedScene? _chunkScene;

    private bool _isGenerated;

    #endregion

    #region Properties

    private TerrainManager TerrainManager
    {
        get => _terrainManager ?? throw new InvalidOperationException($"{nameof(TerrainManager)} has not been initialized.");
        set => _terrainManager = value;
    }

    private PackedScene TerrainChunkViewScene
    {
        get => _chunkScene ?? throw new InvalidOperationException($"{nameof(TerrainChunkViewScene)} not initialized.");
        set => _chunkScene = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the terrain render manager with the required terrain manager and rendering resources.
    /// Loads the terrain chunk view scene and generates the initial set of chunk views
    /// corresponding to the terrain bounds.
    /// </summary>
    /// <param name="terrainManager">
    /// The authoritative terrain manager providing access to terrain chunks and bounds.
    /// </param>
    /// <param name="terrainResources">
    /// Client-side resources required for rendering terrain, including the chunk view scene.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="terrainManager"/> or <paramref name="terrainResources"/> is null.
    /// </exception>
    public void Initialize(TerrainManager terrainManager, TerrainResources terrainResources)
    {
        ArgumentNullException.ThrowIfNull(terrainManager);
        ArgumentNullException.ThrowIfNull(terrainResources);

        TerrainManager = terrainManager;
        TerrainChunkViewScene = terrainResources.TerrainChunkViewScene;

        GenerateInitialTerrainChunkViews();
        MarkInitialized();
    }

    #endregion

    #region Generation

    /// <summary>
    /// Generates and attaches the initial set of terrain chunk views based on the current terrain bounds.
    /// Each chunk view is instantiated, initialized with its corresponding terrain chunk,
    /// and added as a child node for rendering.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if chunk views have already been generated or if an expected terrain chunk is missing.
    /// </exception>
    private void GenerateInitialTerrainChunkViews()
    {
        if (_isGenerated)
        {
            throw new InvalidOperationException("Terrain chunks have already been generated.");
        }

        var bounds = TerrainManager.Bounds;

        for (int y = bounds.MinY; y <= bounds.MaxY; y++)
        {
            for (int x = bounds.MinX; x <= bounds.MaxX; x++)
            {
                var coord = new TerrainChunkGridCoord(x, y);

                if (!TerrainManager.TryGetChunk(coord, out var terrainChunk))
                {
                    throw new InvalidOperationException($"Terrain chunk at {coord} does not exist.");
                }

                var chunkView = TerrainChunkViewScene.Instantiate<TerrainChunkView>();
                chunkView.Initialize(terrainChunk);
                _chunkViews.Add(coord, chunkView);
                AddChild(chunkView);
            }
        }

        _isGenerated = true;
    }

    #endregion
}
