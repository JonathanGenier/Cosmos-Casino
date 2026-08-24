using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;

/// <summary>
/// CPU-side copy of one repeated structure instance in a MultiMesh batch.
/// </summary>
internal readonly struct StructureInstanceEntry
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance entry.
    /// </summary>
    /// <param name="structureId">The authoritative structure identity.</param>
    /// <param name="anchor">The authoritative structure anchor.</param>
    /// <param name="rotation">The authoritative structure rotation.</param>
    /// <param name="transform">The section-local Godot transform.</param>
    internal StructureInstanceEntry(
        StructureId structureId,
        MapCellCoord anchor,
        FootprintRotation rotation,
        Transform3D transform)
    {
        StructureId = structureId;
        Anchor = anchor;
        Rotation = rotation;
        Transform = transform;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative structure identity.
    /// </summary>
    internal StructureId StructureId { get; }

    /// <summary>
    /// Gets the authoritative structure anchor.
    /// </summary>
    internal MapCellCoord Anchor { get; }

    /// <summary>
    /// Gets the authoritative structure rotation.
    /// </summary>
    internal FootprintRotation Rotation { get; }

    /// <summary>
    /// Gets the section-local Godot transform.
    /// </summary>
    internal Transform3D Transform { get; }

    #endregion
}
