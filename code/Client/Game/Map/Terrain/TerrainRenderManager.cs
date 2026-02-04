using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the client-side rendering of terrain by instantiating and maintaining
/// visual terrain chunk views derived from the authoritative map terrain data.
/// </summary>
public sealed partial class TerrainRenderManager : InitializableNodeManager
{
    #region Fields

    private MapManager? _mapManager;
    private PackedScene? _chunkScene;

    private bool _isGenerated;

    #endregion

    #region Properties

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    private PackedScene TerrainChunkViewScene
    {
        get => _chunkScene ?? throw new InvalidOperationException($"{nameof(TerrainChunkViewScene)} not initialized.");
        set => _chunkScene = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the terrain rendering system by binding the map manager,
    /// loading terrain resources, and generating initial terrain chunk views.
    /// </summary>
    /// <param name="mapManager">
    /// The map manager providing access to terrain and map state.
    /// </param>
    /// <param name="terrainResources">
    /// The terrain rendering resources used to spawn chunk views.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="mapManager"/> or <paramref name="terrainResources"/> is <c>null</c>.
    /// </exception>
    public void Initialize(MapManager mapManager, TerrainResources terrainResources)
    {
        ArgumentNullException.ThrowIfNull(mapManager);
        ArgumentNullException.ThrowIfNull(terrainResources);

        MapManager = mapManager;
        TerrainChunkViewScene = terrainResources.TerrainChunkViewScene;

        GenerateInitialTerrainChunkViews();
        MarkInitialized();
    }

    #endregion

    #region Generation

    /// <summary>
    /// Generates and attaches the initial set of terrain chunk views for all
    /// chunks within the configured terrain bounds.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if chunk views have already been generated or required terrain data is missing.
    /// </exception>
    private void GenerateInitialTerrainChunkViews()
    {
        if (_isGenerated)
        {
            throw new InvalidOperationException("Terrain chunks have already been generated.");
        }

        for (int y = 0; y < TerrainConfigs.ChunkCountPerAxis; y++)
        {
            for (int x = 0; x < TerrainConfigs.ChunkCountPerAxis; x++)
            {
                var chunkX = x * TerrainConfigs.ChunkSize;
                var chunkY = y * TerrainConfigs.ChunkSize;
                var chunkCoord = new MapCoord(chunkX, chunkY);

                GenerateChunkView(chunkCoord);
            }
        }

        _isGenerated = true;
    }

    /// <summary>
    /// Creates and initializes a visual terrain chunk view for the specified
    /// chunk coordinate using the corresponding terrain tile data.
    /// </summary>
    /// <param name="chunkCoord">The map coordinate of the chunk to generate.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any terrain tile required for the chunk is missing.
    /// </exception>
    private void GenerateChunkView(MapCoord chunkCoord)
    {
        int chunkSize = TerrainConfigs.ChunkSize;
        var tiles = new TerrainTile[chunkSize, chunkSize];

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                var coord = new MapCoord(chunkCoord.X + x, chunkCoord.Y + y);

                if (!MapManager.TryGetTerrain(coord, out var terrain))
                {
                    throw new InvalidOperationException($"Missing terrain at {coord}");
                }

                tiles[x, y] = terrain;
            }
        }

        var chunkView = TerrainChunkViewScene.Instantiate<TerrainChunkView>();
        chunkView.Initialize(tiles, chunkCoord);
        AddChild(chunkView);
    }

    #endregion
}