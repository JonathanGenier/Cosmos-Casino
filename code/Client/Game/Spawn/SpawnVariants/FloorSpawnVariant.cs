using CosmosCasino.Core.Game.Floor;

/// <summary>
/// Spawn variant describing which floor type should be used
/// when resolving a floor visual.
/// </summary>
public readonly struct FloorSpawnVariant : ISpawnVariant
{
    #region Constructor

    /// <summary>
    /// Creates a new floor spawn variant for the specified floor type.
    /// </summary>
    /// <param name="floorType">
    /// Floor type to resolve into a concrete visual.
    /// </param>
    public FloorSpawnVariant(FloorType floorType)
    {
        FloorType = floorType;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the floor type associated with this spawn variant.
    /// </summary>
    public FloorType FloorType { get; }

    #endregion
}
