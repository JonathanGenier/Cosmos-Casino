using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// One exposed generated Block face resolved from a structure section snapshot.
/// </summary>
internal readonly struct StructureSectionExposedFace
{
    #region Initialization

    /// <summary>
    /// Initializes an exposed generated Block face.
    /// </summary>
    /// <param name="cell">The global occupied cell that owns this exposed face.</param>
    /// <param name="center">The section-local center of the occupied cell.</param>
    /// <param name="face">The generated Block face definition.</param>
    internal StructureSectionExposedFace(
        MapCellCoord cell,
        Vector3 center,
        StructureBlockFace face)
    {
        Cell = cell;
        Center = center;
        Face = face;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the global occupied cell that owns this exposed face.
    /// </summary>
    internal MapCellCoord Cell { get; }

    /// <summary>
    /// Gets the section-local center of the occupied cell.
    /// </summary>
    internal Vector3 Center { get; }

    /// <summary>
    /// Gets the generated Block face definition.
    /// </summary>
    internal StructureBlockFace Face { get; }

    #endregion
}
