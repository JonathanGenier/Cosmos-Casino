using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Builds batched section-local collision geometry for repeated structure instances.
/// </summary>
internal static class StructureInstanceCollisionMeshBuilder
{
    #region Constants

    private const int FacesPerInstance = 6;
    private const int TrianglesPerFace = 2;
    private const int VerticesPerTriangle = 3;
    private const int TriangleVerticesPerFace = TrianglesPerFace * VerticesPerTriangle;

    #endregion

    #region Builder

    /// <summary>
    /// Builds prism collision triangles and hit metadata for repeated structure instances.
    /// </summary>
    /// <param name="entries">The active repeated-structure entries in deterministic dense-slot order.</param>
    /// <param name="localBoundsSize">The local collision bounds for each instance.</param>
    /// <returns>The generated collision geometry and triangle metadata.</returns>
    internal static StructureInstanceCollisionMeshBuildResult Build(
        IReadOnlyList<StructureInstanceEntry> entries,
        Vector3 localBoundsSize)
    {
        ArgumentNullException.ThrowIfNull(entries);

        RepeatedStructureCollisionFace[] faces = CreateFaces(localBoundsSize);
        var triangleVertices = new List<Vector3>(entries.Count * FacesPerInstance * TriangleVerticesPerFace);
        var triangleTargets = new List<StructureCollisionTriangleTarget>(entries.Count * FacesPerInstance * TrianglesPerFace);

        foreach (StructureInstanceEntry entry in entries)
        {
            foreach (RepeatedStructureCollisionFace face in faces)
            {
                AddFace(triangleVertices, triangleTargets, entry, face);
            }
        }

        return new StructureInstanceCollisionMeshBuildResult(
            triangleVertices.ToArray(),
            triangleTargets.AsReadOnly());
    }

    #endregion

    #region Geometry

    private static void AddFace(
        List<Vector3> triangleVertices,
        List<StructureCollisionTriangleTarget> triangleTargets,
        StructureInstanceEntry entry,
        RepeatedStructureCollisionFace face)
    {
        var target = new StructureCollisionTriangleTarget(entry.Anchor, face.SurfaceFace);

        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[0]));
        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[1]));
        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[2]));
        triangleTargets.Add(target);

        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[0]));
        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[2]));
        triangleVertices.Add(TransformPoint(entry.Transform, face.Corners[3]));
        triangleTargets.Add(target);
    }

    private static Vector3 TransformPoint(Transform3D transform, Vector3 point)
    {
        return transform * point;
    }

    private static RepeatedStructureCollisionFace[] CreateFaces(Vector3 size)
    {
        float halfWidth = size.X * 0.5f;
        float halfHeight = size.Y * 0.5f;
        float halfDepth = size.Z * 0.5f;

        return new[]
        {
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.Top,
                new[]
                {
                    new Vector3(-halfWidth, halfHeight, -halfDepth),
                    new Vector3(halfWidth, halfHeight, -halfDepth),
                    new Vector3(halfWidth, halfHeight, halfDepth),
                    new Vector3(-halfWidth, halfHeight, halfDepth)
                }),
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.Bottom,
                new[]
                {
                    new Vector3(-halfWidth, -halfHeight, -halfDepth),
                    new Vector3(-halfWidth, -halfHeight, halfDepth),
                    new Vector3(halfWidth, -halfHeight, halfDepth),
                    new Vector3(halfWidth, -halfHeight, -halfDepth)
                }),
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.North,
                new[]
                {
                    new Vector3(-halfWidth, halfHeight, -halfDepth),
                    new Vector3(-halfWidth, -halfHeight, -halfDepth),
                    new Vector3(halfWidth, -halfHeight, -halfDepth),
                    new Vector3(halfWidth, halfHeight, -halfDepth)
                }),
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.South,
                new[]
                {
                    new Vector3(-halfWidth, halfHeight, halfDepth),
                    new Vector3(halfWidth, halfHeight, halfDepth),
                    new Vector3(halfWidth, -halfHeight, halfDepth),
                    new Vector3(-halfWidth, -halfHeight, halfDepth)
                }),
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.East,
                new[]
                {
                    new Vector3(halfWidth, halfHeight, -halfDepth),
                    new Vector3(halfWidth, -halfHeight, -halfDepth),
                    new Vector3(halfWidth, -halfHeight, halfDepth),
                    new Vector3(halfWidth, halfHeight, halfDepth)
                }),
            new RepeatedStructureCollisionFace(
                CursorSurfaceFace.West,
                new[]
                {
                    new Vector3(-halfWidth, halfHeight, -halfDepth),
                    new Vector3(-halfWidth, halfHeight, halfDepth),
                    new Vector3(-halfWidth, -halfHeight, halfDepth),
                    new Vector3(-halfWidth, -halfHeight, -halfDepth)
                })
        };
    }

    #endregion

    #region Nested Types

    private readonly struct RepeatedStructureCollisionFace
    {
        internal RepeatedStructureCollisionFace(
            CursorSurfaceFace surfaceFace,
            Vector3[] corners)
        {
            SurfaceFace = surfaceFace;
            Corners = corners;
        }

        internal CursorSurfaceFace SurfaceFace { get; }

        internal Vector3[] Corners { get; }
    }

    #endregion
}
