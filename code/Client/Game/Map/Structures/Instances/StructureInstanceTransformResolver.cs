using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;

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
        return GridPlacementTransformResolver.ResolveLocalTransform(
            anchor,
            rotation,
            StructureRenderSectionMath.ToSectionWorldOrigin(sectionCoord));
    }

    #endregion
}
