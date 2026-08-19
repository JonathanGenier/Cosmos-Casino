using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Map.Terrain.Tile;
using Godot;
using System;

/// <summary>
/// Calculates the chunk-local triangle geometry shared by terrain rendering and collision.
/// </summary>
internal static class TerrainTileGeometry
{
    #region Constants

    /// <summary>
    /// The number of vertices required to describe the four terrain triangles for one tile.
    /// </summary>
    internal const int VerticesPerTile = 12;

    #endregion

    #region Geometry

    /// <summary>
    /// Writes the four terrain triangles for one tile in render winding order.
    /// </summary>
    /// <param name="terrainTile">The authoritative terrain tile state.</param>
    /// <param name="chunkPosition">The tile position within its chunk.</param>
    /// <param name="vertices">The destination for the twelve triangle vertices.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="vertices"/> cannot contain all triangle vertices.
    /// </exception>
    internal static void WriteTriangleVertices(
        TerrainTile terrainTile,
        Vector2I chunkPosition,
        Span<Vector3> vertices)
    {
        if (vertices.Length < VerticesPerTile)
        {
            throw new ArgumentException(
                $"Terrain tile geometry requires {VerticesPerTile} vertices.",
                nameof(vertices));
        }

        float x = chunkPosition.X * WorldGridMetrics.GridUnitSize;
        float z = chunkPosition.Y * WorldGridMetrics.GridUnitSize;

        float topLeftHeight = terrainTile.TopLeftHeight;
        float topRightHeight = terrainTile.TopRightHeight;
        float bottomLeftHeight = terrainTile.BottomLeftHeight;
        float bottomRightHeight = terrainTile.BottomRightHeight;
        float centerHeight = CalculateCenterHeight(
            topLeftHeight,
            topRightHeight,
            bottomLeftHeight,
            bottomRightHeight);

        var topLeftVertex = new Vector3(x, topLeftHeight, z);
        var topRightVertex = new Vector3(
            x + WorldGridMetrics.GridUnitSize,
            topRightHeight,
            z);
        var bottomLeftVertex = new Vector3(
            x,
            bottomLeftHeight,
            z + WorldGridMetrics.GridUnitSize);
        var bottomRightVertex = new Vector3(
            x + WorldGridMetrics.GridUnitSize,
            bottomRightHeight,
            z + WorldGridMetrics.GridUnitSize);
        var centerVertex = new Vector3(
            x + WorldGridMetrics.HalfGridUnitSize,
            centerHeight,
            z + WorldGridMetrics.HalfGridUnitSize);

        vertices[0] = bottomLeftVertex;
        vertices[1] = centerVertex;
        vertices[2] = bottomRightVertex;

        vertices[3] = bottomRightVertex;
        vertices[4] = centerVertex;
        vertices[5] = topRightVertex;

        vertices[6] = topRightVertex;
        vertices[7] = centerVertex;
        vertices[8] = topLeftVertex;

        vertices[9] = topLeftVertex;
        vertices[10] = centerVertex;
        vertices[11] = bottomLeftVertex;
    }

    #endregion

    #region Heights

    /// <summary>
    /// Calculates the deterministic center height used by the four terrain triangles.
    /// </summary>
    private static float CalculateCenterHeight(
        float topLeft,
        float topRight,
        float bottomLeft,
        float bottomRight)
    {
        // Four of a kind (flat tile).
        if (topLeft == topRight && topRight == bottomLeft && bottomLeft == bottomRight)
        {
            return topLeft;
        }

        // Three of a kind (convex corner slope).
        if (topLeft == bottomRight && topLeft == bottomLeft)
        {
            return topLeft;
        }

        if (topLeft == topRight && topLeft == bottomLeft)
        {
            return topLeft;
        }

        if (topLeft == topRight && topLeft == bottomRight)
        {
            return topLeft;
        }

        if (topRight == bottomRight && topRight == bottomLeft)
        {
            return topRight;
        }

        // Two of a kind (convex diagonal slope).
        if (topLeft == bottomRight && topLeft != topRight && topLeft != bottomLeft)
        {
            return topLeft;
        }

        if (topRight == bottomLeft && topRight != topLeft && topRight != bottomRight)
        {
            return topRight;
        }

        // Two of a kind (linear slope).
        if (topLeft == topRight && bottomLeft == bottomRight && topLeft != bottomLeft)
        {
            return (topLeft + bottomLeft) * 0.5f;
        }

        if (topLeft == bottomLeft && topRight == bottomRight && topLeft != topRight)
        {
            return (topLeft + topRight) * 0.5f;
        }

        return topLeft + ((topRight - topLeft) * 0.5f) + ((bottomLeft - topLeft) * 0.5f);
    }

    #endregion
}
