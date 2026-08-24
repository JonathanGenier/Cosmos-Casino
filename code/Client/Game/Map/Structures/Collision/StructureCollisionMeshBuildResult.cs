using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Generated section-local collision geometry and disposable triangle metadata for one structure region.
/// </summary>
internal sealed class StructureCollisionMeshBuildResult
{
    #region Initialization

    /// <summary>
    /// Initializes a collision mesh build result.
    /// </summary>
    /// <param name="triangleVertices">Triangle vertices consumed by <see cref="ConcavePolygonShape3D.SetFaces(Vector3[])"/>.</param>
    /// <param name="triangleTargets">One metadata entry for each emitted triangle.</param>
    /// <param name="blockCount">The number of compatible Blocks in the section interior.</param>
    /// <param name="exposedFaceCount">The number of exposed cube faces emitted.</param>
    internal StructureCollisionMeshBuildResult(
        Vector3[] triangleVertices,
        IReadOnlyList<StructureCollisionTriangleTarget> triangleTargets,
        int blockCount,
        int exposedFaceCount)
    {
        ArgumentNullException.ThrowIfNull(triangleVertices);
        ArgumentNullException.ThrowIfNull(triangleTargets);

        TriangleVertices = triangleVertices;
        TriangleTargets = triangleTargets;
        BlockCount = blockCount;
        ExposedFaceCount = exposedFaceCount;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets triangle vertices consumed by <see cref="ConcavePolygonShape3D.SetFaces(Vector3[])"/>.
    /// </summary>
    internal Vector3[] TriangleVertices { get; }

    /// <summary>
    /// Gets one metadata entry for each emitted triangle.
    /// </summary>
    internal IReadOnlyList<StructureCollisionTriangleTarget> TriangleTargets { get; }

    /// <summary>
    /// Gets the number of generated collision triangles.
    /// </summary>
    internal int TriangleCount => TriangleTargets.Count;

    /// <summary>
    /// Gets the number of compatible Blocks in the section interior.
    /// </summary>
    internal int BlockCount { get; }

    /// <summary>
    /// Gets the number of exposed cube faces emitted.
    /// </summary>
    internal int ExposedFaceCount { get; }

    #endregion
}
