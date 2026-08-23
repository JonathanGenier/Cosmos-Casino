using Godot;

/// <summary>
/// Disposable result of building one generated structure render-section mesh.
/// </summary>
internal readonly struct StructureRenderSectionMeshBuildResult
{
    #region Initialization

    /// <summary>
    /// Initializes a new section mesh build result.
    /// </summary>
    /// <param name="mesh">The generated section mesh.</param>
    /// <param name="blockCount">The number of renderable Blocks in the section interior.</param>
    /// <param name="exposedFaceCount">The number of exposed faces emitted into the mesh.</param>
    internal StructureRenderSectionMeshBuildResult(
        ArrayMesh mesh,
        int blockCount,
        int exposedFaceCount)
    {
        Mesh = mesh;
        BlockCount = blockCount;
        ExposedFaceCount = exposedFaceCount;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the generated section mesh.
    /// </summary>
    internal ArrayMesh Mesh { get; }

    /// <summary>
    /// Gets the number of renderable Blocks in the section interior.
    /// </summary>
    internal int BlockCount { get; }

    /// <summary>
    /// Gets the number of exposed faces emitted into the mesh.
    /// </summary>
    internal int ExposedFaceCount { get; }

    /// <summary>
    /// Gets the generated vertex count.
    /// </summary>
    internal int VertexCount => ExposedFaceCount * StructureRenderSectionMeshBuilder.VerticesPerFace;

    /// <summary>
    /// Gets the generated index count.
    /// </summary>
    internal int IndexCount => ExposedFaceCount * StructureRenderSectionMeshBuilder.IndicesPerFace;

    #endregion
}
