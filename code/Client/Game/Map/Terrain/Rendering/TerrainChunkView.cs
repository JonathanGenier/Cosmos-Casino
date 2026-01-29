using CosmosCasino.Core.Game.Map.Terrain.Chunk;
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

    private TerrainChunk? _terrainChunk;
    private MeshInstance3D? _groundMesh;

    private bool _isInitialized;
    private bool _isMeshBuilt;

    #endregion

    #region Properties

    private TerrainChunk TerrainChunk
    {
        get => _terrainChunk ?? throw new InvalidOperationException($"{nameof(TerrainChunk)} has not been initialized.");
        set => _terrainChunk = value;
    }

    private MeshInstance3D GroundMesh
    {
        get => _groundMesh ?? throw new InvalidOperationException("GroundMesh has not been initialized.");
        set => _groundMesh = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the terrain chunk view with its backing terrain data.
    /// Must be called before the node enters the scene tree.
    /// </summary>
    /// <param name="terrainChunk">
    /// The terrain chunk providing the data to render.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="terrainChunk"/> is null.
    /// </exception>
    public void Initialize(TerrainChunk terrainChunk)
    {
        ArgumentNullException.ThrowIfNull(terrainChunk);
        TerrainChunk = terrainChunk;
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

        Position = new Vector3(TerrainChunk.WorldOrigin.X, 0, TerrainChunk.WorldOrigin.Y);
        GroundMesh = GetNode<MeshInstance3D>("GroundMesh");

        BuildFullMesh();
    }

    #endregion

    #region Mesh Generation

    /// <summary>
    /// Builds and assigns the complete terrain mesh for this chunk.
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
        Vector3 chunkOrigin = Vector3.Zero;

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        foreach (var tile in TerrainChunk.Tiles)
        {
            TerrainTileBuilder.BuildTile(surfaceTool, tile, chunkOrigin);
        }

        surfaceTool.GenerateNormals();
        surfaceTool.Commit(mesh);
        GroundMesh.Mesh = mesh;
        _isMeshBuilt = true;
    }

    #endregion
}
