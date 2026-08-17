using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

/// <summary>
/// Rebuilds one chunk-local concave collision shape from authoritative terrain tile state.
/// </summary>
internal static class TerrainChunkCollisionBuilder
{
    #region Builder

    /// <summary>
    /// Replaces the collision faces with the exact triangles used to render the terrain chunk.
    /// </summary>
    /// <param name="collisionShape">The reusable collision shape owned by the chunk view.</param>
    /// <param name="tiles">The terrain tiles contained in the chunk.</param>
    internal static void Rebuild(ConcavePolygonShape3D collisionShape, TerrainTile[,] tiles)
    {
        ArgumentNullException.ThrowIfNull(collisionShape);
        ArgumentNullException.ThrowIfNull(tiles);

        int sizeX = tiles.GetLength(0);
        int sizeY = tiles.GetLength(1);
        var faces = new Vector3[sizeX * sizeY * TerrainTileGeometry.VerticesPerTile];
        int faceOffset = 0;

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                TerrainTileGeometry.WriteTriangleVertices(
                    tiles[x, y],
                    new Vector2I(x, y),
                    faces.AsSpan(faceOffset, TerrainTileGeometry.VerticesPerTile));

                faceOffset += TerrainTileGeometry.VerticesPerTile;
            }
        }

        collisionShape.SetFaces(faces);
    }

    #endregion
}
