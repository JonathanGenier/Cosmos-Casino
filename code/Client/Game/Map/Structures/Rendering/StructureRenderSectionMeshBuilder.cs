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

        IReadOnlyList<StructureSectionExposedFace> exposedFaces = StructureSectionExposedFaceResolver.Resolve(snapshot);
        var vertices = new List<Vector3>(exposedFaces.Count * VerticesPerFace);
        var normals = new List<Vector3>(exposedFaces.Count * VerticesPerFace);
        var uvs = new List<Vector2>(exposedFaces.Count * VerticesPerFace);
        var indices = new List<int>(exposedFaces.Count * IndicesPerFace);

        foreach (StructureSectionExposedFace exposedFace in exposedFaces)
        {
            AddFace(vertices, normals, uvs, indices, exposedFace);
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
            exposedFaces.Count);
    }

    #endregion

    #region Geometry

    private static void AddFace(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> indices,
        StructureSectionExposedFace exposedFace)
    {
        int baseIndex = vertices.Count;

        for (int i = 0; i < VerticesPerFace; i++)
        {
            vertices.Add(exposedFace.Center + exposedFace.Face.Corners[i]);
            normals.Add(exposedFace.Face.Normal);
            uvs.Add(FaceUvs[i]);
        }

        indices.Add(baseIndex);
        indices.Add(baseIndex + 1);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex);
        indices.Add(baseIndex + 2);
        indices.Add(baseIndex + 3);
    }

    #endregion
}
