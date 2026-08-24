using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// Resolves Client-only Godot transforms for repeated structure instances.
/// </summary>
internal static class StructureInstanceTransformResolver
{
    #region Transform

    /// <summary>
    /// Resolves a section-local transform for a repeated structure instance.
    /// </summary>
    /// <param name="sectionCoord">The render section coordinate owning the instance.</param>
    /// <param name="anchor">The authoritative structure anchor.</param>
    /// <param name="rotation">The authoritative structure rotation.</param>
    /// <returns>The section-local Godot transform.</returns>
    internal static Transform3D ResolveSectionLocalTransform(
        StructureRenderSectionCoord sectionCoord,
        MapCellCoord anchor,
        FootprintRotation rotation)
    {
        Vector3 sectionOrigin = StructureRenderSectionMath.ToSectionWorldOrigin(sectionCoord);
        Vector3 localOrigin = anchor.ToGodotCenter() - sectionOrigin;

        return new Transform3D(
            Basis.Identity.Rotated(Vector3.Up, ToGodotYawRadians(rotation)),
            localOrigin);
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
