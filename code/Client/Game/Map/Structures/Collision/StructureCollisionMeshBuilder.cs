using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Builds section-local collision geometry for generated static Structure surfaces.
/// </summary>
internal static class StructureCollisionMeshBuilder
{
    #region Constants

    private const int TrianglesPerFace = 2;
    private const int VerticesPerTriangle = 3;
    private const int TriangleVerticesPerFace = TrianglesPerFace * VerticesPerTriangle;

    #endregion

    #region Builder

    /// <summary>
    /// Builds collision triangles and hit metadata from the same exposed faces used by structure rendering.
    /// </summary>
    /// <param name="snapshot">The disposable section snapshot.</param>
    /// <returns>The generated collision geometry and triangle metadata.</returns>
    internal static StructureCollisionMeshBuildResult Build(StructureRenderSectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        IReadOnlyList<StructureSectionExposedFace> exposedFaces = StructureSectionExposedFaceResolver.Resolve(snapshot);
        var triangleVertices = new List<Vector3>(exposedFaces.Count * TriangleVerticesPerFace);
        var triangleTargets = new List<StructureCollisionTriangleTarget>(exposedFaces.Count * TrianglesPerFace);

        foreach (StructureSectionExposedFace exposedFace in exposedFaces)
        {
            AddFace(triangleVertices, triangleTargets, exposedFace);
        }

        return new StructureCollisionMeshBuildResult(
            triangleVertices.ToArray(),
            triangleTargets.AsReadOnly(),
            snapshot.BlockCount,
            exposedFaces.Count);
    }

    #endregion

    #region Geometry

    private static void AddFace(
        List<Vector3> triangleVertices,
        List<StructureCollisionTriangleTarget> triangleTargets,
        StructureSectionExposedFace exposedFace)
    {
        Vector3 center = exposedFace.Center;
        Vector3[] corners = exposedFace.Face.Corners;
        var target = new StructureCollisionTriangleTarget(exposedFace.Cell, exposedFace.Face.SurfaceFace);

        triangleVertices.Add(center + corners[0]);
        triangleVertices.Add(center + corners[1]);
        triangleVertices.Add(center + corners[2]);
        triangleTargets.Add(target);

        triangleVertices.Add(center + corners[0]);
        triangleVertices.Add(center + corners[2]);
        triangleVertices.Add(center + corners[3]);
        triangleTargets.Add(target);
    }

    #endregion
}
