using CosmosCasino.Core.Game.Map.Terrain;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

/// <summary>
/// Owns the Client-side render and collision representation of one terrain chunk.
/// </summary>
public sealed partial class TerrainChunkView : Node3D
{
    #region Fields

    private TerrainTile[,]? _tiles;
    private MeshInstance3D? _groundMesh;
    private StaticBody3D? _terrainBody;
    private ConcavePolygonShape3D? _terrainCollision;

    private bool _isInitialized;

    #endregion

    #region Properties

    private TerrainTile[,] Tiles
    {
        get => _tiles ?? throw new InvalidOperationException($"Chunk tiles has not been initialized.");
    }

    private MeshInstance3D GroundMesh
    {
        get => _groundMesh ?? throw new InvalidOperationException("GroundMesh has not been initialized.");
        set => _groundMesh = value;
    }

    private StaticBody3D TerrainBody
    {
        get => _terrainBody ?? throw new InvalidOperationException("TerrainBody has not been initialized.");
        set => _terrainBody = value;
    }

    private ConcavePolygonShape3D TerrainCollision
    {
        get => _terrainCollision ?? throw new InvalidOperationException("TerrainCollision has not been initialized.");
        set => _terrainCollision = value;
    }

    private TerrainChunkGridCoord Coord { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the chunk view with terrain tile data and its signed chunk-grid coordinate.
    /// </summary>
    /// <param name="tiles">The terrain tiles contained in this chunk.</param>
    /// <param name="coord">The signed chunk-grid coordinate identifying the chunk.</param>
    public void Initialize(TerrainTile[,] tiles, TerrainChunkGridCoord coord)
    {
        ArgumentNullException.ThrowIfNull(tiles);

        _tiles = tiles;
        Coord = coord;
        _isInitialized = true;
    }

    #endregion

    #region Godot Processes

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Positions the chunk in world space and builds its render and collision representations.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the view has not been initialized prior to readiness.
    /// </exception>
    public override void _Ready()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(TerrainChunkView)} has not been initialized.");
        }

        GroundMesh = GetNode<MeshInstance3D>("GroundMesh");
        TerrainBody = GetNode<StaticBody3D>("TerrainBody");

        TerrainBody.CollisionLayer = CollisionLayers.Terrain;
        TerrainBody.CollisionMask = CollisionLayers.None;

        TerrainCollision = new ConcavePolygonShape3D();
        GetNode<CollisionShape3D>("TerrainBody/TerrainCollision").Shape = TerrainCollision;

        var chunkOriginTile = TerrainMath.ChunkLocalToWorldTileCoord(
            Coord,
            new TerrainChunkLocalCoord(0, 0),
            Tiles.GetLength(0));

        Position = TerrainMath.TileToWorldOrigin(chunkOriginTile).ToGodotVector3();

        RebuildTerrainRepresentation();
    }

    #endregion

    #region Representation

    /// <summary>
    /// Rebuilds the chunk-local render mesh and updates the existing collision shape.
    /// </summary>
    private void RebuildTerrainRepresentation()
    {
        GroundMesh.Mesh = TerrainChunkMeshBuilder.Build(Tiles);
        TerrainChunkCollisionBuilder.Rebuild(TerrainCollision, Tiles);
    }

    #endregion
}
