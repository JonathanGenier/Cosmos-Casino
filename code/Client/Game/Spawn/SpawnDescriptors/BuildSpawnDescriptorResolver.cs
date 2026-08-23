using CosmosCasino.Core.Game.Build.Domain;
using System;

/// <summary>
/// Provides functionality to resolve a spawn descriptor for a build intent based on its kind.
/// </summary>
/// <remarks>Use this class to obtain the appropriate spawn descriptor for supported build kinds, such as floors
/// and walls. If an unsupported build kind is provided, an exception is thrown. This class is static and cannot be
/// instantiated.</remarks>
public static class BuildSpawnDescriptorResolver
{
    /// <summary>
    /// Resolves a legacy build kind to the existing floor or wall spawn descriptor.
    /// </summary>
    /// <param name="kind">The legacy build kind to resolve.</param>
    /// <returns>A build spawn descriptor for the requested legacy client visual.</returns>
    /// <exception cref="NotSupportedException">Thrown if the build kind is not supported for spawning.</exception>
    public static BuildSpawnDescriptor Resolve(BuildKind kind)
    {
        return kind switch
        {
            BuildKind.Floor =>
                new BuildSpawnDescriptor(
                    default(FloorSpawnVariant),
                    SpawnLayer.Floors),

            BuildKind.Wall =>
                new BuildSpawnDescriptor(
                    default(WallSpawnVariant),
                    SpawnLayer.Walls),

            _ => throw new NotSupportedException(
                $"Spawning not supported for {kind}")
        };
    }
}
