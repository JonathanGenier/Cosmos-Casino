using CosmosCasino.Core.Configs;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

// TODO TerrainChunkView currently has too many responsibilities
// such as Data Binding, Positioning, Mesh Generation.
// Split into TerrainChunkMeshBuilder, 

/// <summary>
/// Client-side visual representation of a single terrain chunk.
/// Responsible for positioning the chunk in world space and
/// building its renderable mesh from core terrain data.
/// </summary>
public sealed partial class TerrainChunkView : Node3D
{
    #region Fields

    private TerrainTile[,]? _tiles;         // Array of tiles in this chunk
    private MeshInstance3D? _groundMesh;

    private bool _isInitialized;
    private bool _isMeshBuilt;

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

    private MapCoord Coord { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the chunk view with terrain tile data and its map coordinate.
    /// </summary>
    /// <param name="tiles">The terrain tiles contained in this chunk.</param>
    /// <param name="coord">The map coordinate identifying the chunk.</param>
    public void Initialize(TerrainTile[,] tiles, MapCoord coord)
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
    /// Positions the chunk in world space and builds its renderable mesh.
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

        float half = TerrainConfigs.ChunkSize * TerrainConfigs.ChunkCountPerAxis / 2;
        float originX = Coord.X - half - 0.5f;
        float originZ = Coord.Y - half - 0.5f;

        Position = new Vector3(originX, 0, originZ);

        BuildFullMesh();
    }

    #endregion

    #region Mesh Generation

    /// <summary>
    /// Builds and assigns the renderable mesh for the terrain chunk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the mesh has already been built.
    /// </exception>
    private void BuildFullMesh()
    {
        if (_isMeshBuilt)
        {
            throw new InvalidOperationException("Mesh has already been built.");
        }

        var surfaceTool = new SurfaceTool();
        var mesh = new ArrayMesh();

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        int sizeX = Tiles.GetLength(0);
        int sizeY = Tiles.GetLength(1);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                var tile = Tiles[x, y];
                var cellChunkPosition = new Vector2I(x, y);

                TerrainTileBuilder.BuildTile(
                    surfaceTool,
                    tile,
                    cellChunkPosition
                );
            }
        }

        surfaceTool.GenerateNormals();
        surfaceTool.Commit(mesh);
        GroundMesh.Mesh = mesh;

        _isMeshBuilt = true;
    }

    #endregion
}