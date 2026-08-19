using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

/// <summary>
/// Builds one chunk-local render mesh from authoritative terrain tile state.
/// </summary>
internal static class TerrainChunkMeshBuilder
{
    #region Builder

    /// <summary>
    /// Builds a render mesh containing all terrain tiles in a chunk.
    /// </summary>
    /// <param name="tiles">The terrain tiles contained in the chunk.</param>
    /// <returns>The completed chunk-local terrain mesh.</returns>
    internal static ArrayMesh Build(TerrainTile[,] tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);

        var surfaceTool = new SurfaceTool();
        var mesh = new ArrayMesh();

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        int sizeX = tiles.GetLength(0);
        int sizeY = tiles.GetLength(1);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                TerrainTileBuilder.BuildTile(
                    surfaceTool,
                    tiles[x, y],
                    new Vector2I(x, y));
            }
        }

        surfaceTool.GenerateNormals();
        surfaceTool.Commit(mesh);

        return mesh;
    }

    #endregion
}
