using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Batched collision geometry and triangle metadata for one repeated-structure collision region.
/// </summary>
internal sealed class StructureInstanceCollisionMeshBuildResult
{
    #region Initialization

    /// <summary>
    /// Initializes a repeated-structure collision mesh build result.
    /// </summary>
    /// <param name="triangleVertices">Triangle vertices consumed by <see cref="ConcavePolygonShape3D.SetFaces(Vector3[])"/>.</param>
    /// <param name="triangleTargets">One metadata entry for each emitted triangle.</param>
    internal StructureInstanceCollisionMeshBuildResult(
        Vector3[] triangleVertices,
        IReadOnlyList<StructureCollisionTriangleTarget> triangleTargets)
    {
        ArgumentNullException.ThrowIfNull(triangleVertices);
        ArgumentNullException.ThrowIfNull(triangleTargets);

        TriangleVertices = triangleVertices;
        TriangleTargets = triangleTargets;
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

    #endregion
}
