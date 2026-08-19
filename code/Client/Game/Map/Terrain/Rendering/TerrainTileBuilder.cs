using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

/// <summary>
/// Provides utility methods for constructing terrain tiles into a mesh using a <see cref="SurfaceTool"/>.
/// Responsible for converting logical <see cref="TerrainTile"/> data into renderable geometry,
/// including vertex positions, UVs, color-encoded tile metadata, and slope-aware triangulation.
/// </summary>
public static class TerrainTileBuilder
{
    #region Constants

    private static readonly Vector2[] Uvs =
    {
        new(0, 1), new(0.5f, 0.5f), new(1, 1),
        new(1, 1), new(0.5f, 0.5f), new(1, 0),
        new(1, 0), new(0.5f, 0.5f), new(0, 0),
        new(0, 0), new(0.5f, 0.5f), new(0, 1)
    };

    #endregion

    #region Builder

    /// <summary>
    /// Builds the mesh geometry for a single terrain tile by emitting four triangles
    /// arranged around a computed center vertex. Heights, UVs, and encoded tile metadata
    /// are applied directly to the provided <see cref="SurfaceTool"/>.
    /// </summary>
    /// <param name="surfaceTool">The surface tool used to emit vertices and build the mesh.</param>
    /// <param name="terrainTile">The logical terrain tile containing height and slope data.</param>
    /// <param name="chunkPosition">The tile position within its chunk.</param>
    public static void BuildTile(SurfaceTool surfaceTool, TerrainTile terrainTile, Vector2I chunkPosition)
    {
        Span<Vector3> vertices = stackalloc Vector3[TerrainTileGeometry.VerticesPerTile];
        TerrainTileGeometry.WriteTriangleVertices(terrainTile, chunkPosition, vertices);
        Color colorMask = EncodeTileMask(terrainTile);

        for (int i = 0; i < vertices.Length; i++)
        {
            AddVertex(surfaceTool, colorMask, Uvs[i], vertices[i]);
        }
    }

    #endregion

    #region Triangles and Vertices

    /// <summary>
    /// Adds a single vertex to the mesh with UVs, smoothing group, color mask,
    /// and position fully configured.
    /// </summary>
    /// <param name="surfaceTool">The surface tool receiving the vertex.</param>
    /// <param name="colorMask">A color encoding tile slope and neighbor information.</param>
    /// <param name="uv">The UV coordinate for this vertex.</param>
    /// <param name="vertex">The chunk-local position of the vertex.</param>
    private static void AddVertex(SurfaceTool surfaceTool, Color colorMask, Vector2 uv, Vector3 vertex)
    {
        surfaceTool.SetUV(uv);
        surfaceTool.SetUV2(uv);
        surfaceTool.SetSmoothGroup(1);
        surfaceTool.SetColor(colorMask);
        surfaceTool.AddVertex(vertex);
    }

    #endregion

    #region SlopeNeighborMask

    /// <summary>
    /// Encodes slope and slope-neighbor information from a terrain tile into a color mask.
    /// The resulting color is used by the terrain shader to determine blending,
    /// edge behavior, and slope-specific rendering logic.
    /// </summary>
    /// <param name="tile">The terrain tile whose slope metadata is encoded.</param>
    /// <returns>
    /// A <see cref="Color"/> where:
    /// R indicates whether the tile itself is a slope,
    /// G encodes cardinal slope neighbors,
    /// B encodes diagonal slope neighbors.
    /// </returns>
    private static Color EncodeTileMask(TerrainTile tile)
    {
        if (tile.IsSlope)
        {
            // R=1 means slope; G/B intentionally zeroed
            return new Color(1f, 0f, 0f, 0f);
        }

        // Cardinal directions
        int cardinalMask =
        ((tile.SlopeNeighborMask & SlopeNeighborMask.North) != 0 ? 1 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.East) != 0 ? 2 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.South) != 0 ? 4 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.West) != 0 ? 8 : 0);

        // Diagonals
        int diagonalMask =
        ((tile.SlopeNeighborMask & SlopeNeighborMask.NorthEast) != 0 ? 1 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.SouthEast) != 0 ? 2 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.SouthWest) != 0 ? 4 : 0) |
        ((tile.SlopeNeighborMask & SlopeNeighborMask.NorthWest) != 0 ? 8 : 0);

        return new Color(
            0f,
            cardinalMask / 15f,
            diagonalMask / 15f,
            0f
        );
    }

    #endregion
}
