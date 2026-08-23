using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Builds generated section-local geometry for compatible static structure cells.
/// </summary>
internal static class StructureRenderSectionMeshBuilder
{
    #region Constants

    /// <summary>
    /// Number of independent vertices emitted for one exposed face.
    /// </summary>
    internal const int VerticesPerFace = 4;

    /// <summary>
    /// Number of triangle indices emitted for one exposed face.
    /// </summary>
    internal const int IndicesPerFace = 6;

    private const int MaxFacesPerBlock = 6;

    #endregion

    #region Fields

    private static readonly Vector2[] FaceUvs =
    {
        new(0f, 0f),
        new(1f, 0f),
        new(1f, 1f),
        new(0f, 1f)
    };

    #endregion

    #region Builder

    /// <summary>
    /// Builds an ArrayMesh containing only exposed faces for renderable Blocks in the supplied section snapshot.
    /// </summary>
    /// <param name="snapshot">The disposable render-section snapshot.</param>
    /// <returns>The generated mesh and basic mesh statistics.</returns>
    internal static StructureRenderSectionMeshBuildResult Build(StructureRenderSectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        int maxFaceCount = snapshot.BlockCount * MaxFacesPerBlock;
        var vertices = new List<Vector3>(maxFaceCount * VerticesPerFace);
        var normals = new List<Vector3>(maxFaceCount * VerticesPerFace);
        var uvs = new List<Vector2>(maxFaceCount * VerticesPerFace);
        var indices = new List<int>(maxFaceCount * IndicesPerFace);
        int exposedFaceCount = 0;

        foreach (MapCellCoord cell in snapshot.RenderableBlocks)
        {
            Vector3 center = ToSectionLocalCenter(snapshot.OriginCell, cell);

            foreach (BlockFace face in BlockFace.All)
            {
                if (StructureRenderSectionMath.TryAddOffset(cell, face.NeighborOffset, out MapCellCoord neighbor)
                    && snapshot.IsCompatibleOpaqueBlockAt(neighbor))
                {
                    continue;
                }

                AddFace(vertices, normals, uvs, indices, center, face);
                exposedFaceCount++;
            }
        }

        var mesh = new ArrayMesh();

        if (vertices.Count > 0)
        {
            Godot.Collections.Array meshArrays = new();
            meshArrays.Resize((int)Mesh.ArrayType.Max);
            meshArrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
            meshArrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
            meshArrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
            meshArrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArrays);
        }

        return new StructureRenderSectionMeshBuildResult(
            mesh,
            snapshot.BlockCount,
            exposedFaceCount);
    }

    #endregion

    #region Geometry

    private static void AddFace(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> indices,
        Vector3 center,
        BlockFace face)
    {
        int baseIndex = vertices.Count;

        for (int i = 0; i < VerticesPerFace; i++)
        {
            vertices.Add(center + face.Corners[i]);
            normals.Add(face.Normal);
            uvs.Add(FaceUvs[i]);
        }

        indices.Add(baseIndex);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }

    private static Vector3 ToSectionLocalCenter(MapCellCoord originCell, MapCellCoord cell)
    {
        return new Vector3(
            (cell.X - originCell.X) * WorldGridMetrics.GridUnitSize,
            (cell.Y - originCell.Y) * WorldGridMetrics.VerticalGridUnitSize,
            (cell.Z - originCell.Z) * WorldGridMetrics.GridUnitSize);
    }

    #endregion

    #region Face Definitions

    private readonly struct BlockFace
    {
        internal const float HalfWidth = WorldGridMetrics.GridUnitSize * 0.5f;
        internal const float HalfHeight = WorldGridMetrics.VerticalGridUnitSize * 0.5f;
        internal const float HalfDepth = WorldGridMetrics.GridUnitSize * 0.5f;

        /// <summary>
        /// All six cube faces in deterministic top, bottom, north, south, east, west order.
        /// </summary>
        internal static readonly BlockFace[] All =
        {
            Top(),
            Bottom(),
            North(),
            South(),
            East(),
            West()
        };

        private BlockFace(MapCellOffset neighborOffset, Vector3 normal, Vector3[] corners)
        {
            NeighborOffset = neighborOffset;
            Normal = normal;
            Corners = corners;
        }

        /// <summary>
        /// Gets the global logical neighbor tested for this face.
        /// </summary>
        internal MapCellOffset NeighborOffset { get; }

        /// <summary>
        /// Gets the outward Godot normal for this face.
        /// </summary>
        internal Vector3 Normal { get; }

        /// <summary>
        /// Gets the section-local face corners in Godot clockwise winding order.
        /// </summary>
        internal Vector3[] Corners { get; }

        private static BlockFace Top()
        {
            return new BlockFace(
                new MapCellOffset(0, 1, 0),
                Vector3.Up,
                new[]
                {
                    new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, HalfHeight, HalfDepth),
                    new Vector3(-HalfWidth, HalfHeight, HalfDepth)
                });
        }

        private static BlockFace Bottom()
        {
            return new BlockFace(
                new MapCellOffset(0, -1, 0),
                Vector3.Down,
                new[]
                {
                    new Vector3(-HalfWidth, -HalfHeight, -HalfDepth),
                    new Vector3(-HalfWidth, -HalfHeight, HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, -HalfDepth)
                });
        }

        private static BlockFace North()
        {
            return new BlockFace(
                new MapCellOffset(0, 0, -1),
                Vector3.Forward,
                new[]
                {
                    new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                    new Vector3(-HalfWidth, -HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, HalfHeight, -HalfDepth)
                });
        }

        private static BlockFace South()
        {
            return new BlockFace(
                new MapCellOffset(0, 0, 1),
                Vector3.Back,
                new[]
                {
                    new Vector3(-HalfWidth, HalfHeight, HalfDepth),
                    new Vector3(HalfWidth, HalfHeight, HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                    new Vector3(-HalfWidth, -HalfHeight, HalfDepth)
                });
        }

        private static BlockFace East()
        {
            return new BlockFace(
                new MapCellOffset(1, 0, 0),
                Vector3.Right,
                new[]
                {
                    new Vector3(HalfWidth, HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, -HalfDepth),
                    new Vector3(HalfWidth, -HalfHeight, HalfDepth),
                    new Vector3(HalfWidth, HalfHeight, HalfDepth)
                });
        }

        private static BlockFace West()
        {
            return new BlockFace(
                new MapCellOffset(-1, 0, 0),
                Vector3.Left,
                new[]
                {
                    new Vector3(-HalfWidth, HalfHeight, -HalfDepth),
                    new Vector3(-HalfWidth, HalfHeight, HalfDepth),
                    new Vector3(-HalfWidth, -HalfHeight, HalfDepth),
                    new Vector3(-HalfWidth, -HalfHeight, -HalfDepth)
                });
        }
    }

    #endregion
}
