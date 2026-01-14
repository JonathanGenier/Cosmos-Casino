/// <summary>
/// Represents a spawn variant that is associated with wall-based spawning behavior.
/// </summary>
/// <remarks>Use this struct to specify or identify spawn variants that are intended for wall placement scenarios.
/// The default value is provided by the <see cref="Default"/> field.</remarks>
public readonly struct WallSpawnVariant : ISpawnVariant
{
    /// <summary>
    /// Represents the default value for the WallSpawnVariant type.
    /// </summary>
    public static readonly WallSpawnVariant Default = default(WallSpawnVariant);
}
