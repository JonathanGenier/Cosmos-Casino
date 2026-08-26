using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// Resolves Client-only Godot transforms for authoritative grid-placement anchors and rotations.
/// </summary>
internal static class GridPlacementTransformResolver
{
    #region Transform

    /// <summary>
    /// Resolves a world-space Godot transform for an authoritative grid-placement anchor and rotation.
    /// </summary>
    /// <param name="anchor">The authoritative grid-placement anchor.</param>
    /// <param name="rotation">The authoritative footprint rotation.</param>
    /// <returns>The world-space Godot transform.</returns>
    internal static Transform3D ResolveWorldTransform(
        MapCellCoord anchor,
        FootprintRotation rotation)
    {
        return ResolveWorldTransform(anchor.ToGodotCenter(), rotation);
    }

    /// <summary>
    /// Resolves a world-space Godot transform for a pre-resolved world-space anchor and rotation.
    /// </summary>
    /// <param name="worldAnchor">The world-space anchor position.</param>
    /// <param name="rotation">The authoritative footprint rotation.</param>
    /// <returns>The world-space Godot transform.</returns>
    internal static Transform3D ResolveWorldTransform(
        Vector3 worldAnchor,
        FootprintRotation rotation)
    {
        return new Transform3D(
            Basis.Identity.Rotated(Vector3.Up, ToGodotYawRadians(rotation)),
            worldAnchor);
    }

    /// <summary>
    /// Resolves a local Godot transform relative to the specified world origin.
    /// </summary>
    /// <param name="anchor">The authoritative grid-placement anchor.</param>
    /// <param name="rotation">The authoritative footprint rotation.</param>
    /// <param name="worldOrigin">The world-space origin of the local coordinate frame.</param>
    /// <returns>The local Godot transform.</returns>
    internal static Transform3D ResolveLocalTransform(
        MapCellCoord anchor,
        FootprintRotation rotation,
        Vector3 worldOrigin)
    {
        return ResolveLocalTransform(anchor.ToGodotCenter(), rotation, worldOrigin);
    }

    /// <summary>
    /// Resolves a local Godot transform from a pre-resolved world-space anchor relative to the specified world origin.
    /// </summary>
    /// <param name="worldAnchor">The world-space anchor position.</param>
    /// <param name="rotation">The authoritative footprint rotation.</param>
    /// <param name="worldOrigin">The world-space origin of the local coordinate frame.</param>
    /// <returns>The local Godot transform.</returns>
    internal static Transform3D ResolveLocalTransform(
        Vector3 worldAnchor,
        FootprintRotation rotation,
        Vector3 worldOrigin)
    {
        return new Transform3D(
            Basis.Identity.Rotated(Vector3.Up, ToGodotYawRadians(rotation)),
            worldAnchor - worldOrigin);
    }

    #endregion

    #region Rotation

    private static float ToGodotYawRadians(FootprintRotation rotation)
    {
        return rotation switch
        {
            FootprintRotation.Deg0 => 0f,
            FootprintRotation.Deg90 => MathF.PI * 0.5f,
            FootprintRotation.Deg180 => MathF.PI,
            FootprintRotation.Deg270 => MathF.PI * 1.5f,
            _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, "Unsupported footprint rotation.")
        };
    }

    #endregion
}
