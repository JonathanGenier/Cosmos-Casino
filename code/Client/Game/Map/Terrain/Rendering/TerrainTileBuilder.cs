using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;

/// <summary>
/// Provides utility methods for constructing terrain tiles into a mesh using a <see cref="SurfaceTool"/>.
/// Responsible for converting logical <see cref="Cell"/> data into renderable geometry,
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
    /// <param name="chunkPosition">The position of the chunk within the world.</param>
    public static void BuildTile(SurfaceTool surfaceTool, TerrainTile terrainTile, Vector2I chunkPosition)
    {
        float x = chunkPosition.X;
        float z = chunkPosition.Y;

        float topLeftHeight = terrainTile.TopLeftHeight;
        float topRightHeight = terrainTile.TopRightHeight;
        float bottomLeftHeight = terrainTile.BottomLeftHeight;
        float bottomRightHeight = terrainTile.BottomRightHeight;
        float centerHeight = CalculateCenterHeight(topLeftHeight, topRightHeight, bottomLeftHeight, bottomRightHeight);

        Vector3 topLeftVertex = new(x, topLeftHeight, z);
        Vector3 topRightVertex = new(x + 1, topRightHeight, z);
        Vector3 bottomLeftVertex = new(x, bottomLeftHeight, z + 1);
        Vector3 bottomRightVertex = new(x + 1, bottomRightHeight, z + 1);
        Vector3 centerVertex = new(x + 0.5f, centerHeight, z + 0.5f);

        Color colorMask = EncodeTileMask(terrainTile);

        AddTriangle(surfaceTool, colorMask, bottomLeftVertex, centerVertex, bottomRightVertex, 0);
        AddTriangle(surfaceTool, colorMask, bottomRightVertex, centerVertex, topRightVertex, 3);
        AddTriangle(surfaceTool, colorMask, topRightVertex, centerVertex, topLeftVertex, 6);
        AddTriangle(surfaceTool, colorMask, topLeftVertex, centerVertex, bottomLeftVertex, 9);
    }

    #endregion

    #region Triangles and Vertices

    /// <summary>
    /// Emits a single triangle into the mesh by adding three vertices with predefined UVs
    /// and a shared encoded color mask.
    /// </summary>
    /// <param name="surfaceTool">The surface tool used to add vertices.</param>
    /// <param name="colorMask">A color encoding slope and neighbor metadata for the tile.</param>
    /// <param name="vertexA">The first vertex position.</param>
    /// <param name="vertexB">The second vertex position.</param>
    /// <param name="vertexC">The third vertex position.</param>
    /// <param name="uvIndex">
    /// The starting index into the internal UV array corresponding to this triangle.
    /// </param>
    private static void AddTriangle(SurfaceTool surfaceTool, Color colorMask, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, int uvIndex)
    {

        AddVertex(surfaceTool, colorMask, Uvs[uvIndex + 0], vertexA);
        AddVertex(surfaceTool, colorMask, Uvs[uvIndex + 1], vertexB);
        AddVertex(surfaceTool, colorMask, Uvs[uvIndex + 2], vertexC);
    }

    /// <summary>
    /// Adds a single vertex to the mesh with UVs, smoothing group, color mask,
    /// and position fully configured.
    /// </summary>
    /// <param name="surfaceTool">The surface tool receiving the vertex.</param>
    /// <param name="colorMask">A color encoding tile slope and neighbor information.</param>
    /// <param name="uv">The UV coordinate for this vertex.</param>
    /// <param name="vertex">The world-space position of the vertex.</param>
    private static void AddVertex(SurfaceTool surfaceTool, Color colorMask, Vector2 uv, Vector3 vertex)
    {
        surfaceTool.SetUV(uv);
        surfaceTool.SetUV2(uv);
        surfaceTool.SetSmoothGroup(1);
        surfaceTool.SetColor(colorMask);
        surfaceTool.AddVertex(vertex);
    }

    #endregion

    #region Heights

    /// <summary>
    /// Calculates the height of the center vertex of a tile based on the heights of its four corners.
    /// Applies deterministic rules to preserve sharp slopes, diagonals, and flat regions,
    /// avoiding unwanted smoothing artifacts.
    /// </summary>
    /// <param name="topLeft">Height of the top-left corner.</param>
    /// <param name="topRight">Height of the top-right corner.</param>
    /// <param name="bottomLeft">Height of the bottom-left corner.</param>
    /// <param name="bottomRight">Height of the bottom-right corner.</param>
    /// <returns>
    /// The computed center height for the tile.
    /// </returns>
    private static float CalculateCenterHeight(float topLeft, float topRight, float bottomLeft, float bottomRight)
    {
        // ------------------------------------------------------------------------------
        // Four of a kind (Flat Tile)
        // ------------------------------------------------------------------------------

        // X X
        // X X
        if (topLeft == topRight && topRight == bottomLeft && bottomLeft == bottomRight)
        {
            return topLeft;
        }

        // ------------------------------------------------------------------------------
        // Three of a kind: (Corner convex sloped)
        // ------------------------------------------------------------------------------

        // X Y
        // X X
        if (topLeft == bottomRight && topLeft == bottomLeft)
        {
            return topLeft;
        }

        // X X
        // X Y
        if (topLeft == topRight && topLeft == bottomLeft)
        {
            return topLeft;
        }

        // X X
        // Y X
        if (topLeft == topRight && topLeft == bottomRight)
        {
            return topLeft;
        }

        // Y X
        // X X
        if (topRight == bottomRight && topRight == bottomLeft)
        {
            return topRight;
        }

        // ------------------------------------------------------------------------------
        // 2 of a kind (Diagonal Convex Sloped)
        // ------------------------------------------------------------------------------

        // X Y
        // Y X
        if (topLeft == bottomRight && topLeft != topRight && topLeft != bottomLeft)
        {
            return topLeft;
        }

        // Y X
        // X Y
        if (topRight == bottomLeft && topRight != topLeft && topRight != bottomRight)
        {
            return topRight;
        }

        // ------------------------------------------------------------------------------
        // 2 of a kind (Opposites Linear Sloped)
        // ------------------------------------------------------------------------------

        // Check if heights on opposite sides are consistent.
        // X X
        // Y Y
        if (topLeft == topRight && bottomLeft == bottomRight && topLeft != bottomLeft)
        {
            return (topLeft + bottomLeft) * 0.5f;
        }

        // X Y
        // X Y
        if (topLeft == bottomLeft && topRight == bottomRight && topLeft != topRight)
        {
            return (topLeft + topRight) * 0.5f;
        }

        // Default: Smooth interpolation based on slopes.
        return topLeft + ((topRight - topLeft) * 0.5f) + ((bottomLeft - topLeft) * 0.5f);
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