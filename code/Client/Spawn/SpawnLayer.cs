/// <summary>
/// Defines the visual layer under which spawned nodes are organized
/// in the scene tree.
/// </summary>
public enum SpawnLayer
{
    /// <summary>
    /// Layer containing floor visuals aligned to the map grid.
    /// </summary>
    Floors,

    /// <summary>
    /// Layer containing structure visuals occupying map cell slots.
    /// </summary>
    Structures,
}
