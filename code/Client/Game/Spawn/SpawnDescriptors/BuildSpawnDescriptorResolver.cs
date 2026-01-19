using CosmosCasino.Core.Game.Build;
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
    /// Resolves the specified build intent to a corresponding spawn descriptor for floor or wall objects.
    /// </summary>
    /// <remarks>This method only supports intents with a kind of Floor or Wall. Other kinds will result in a
    /// NotSupportedException.</remarks>
    /// <param name="intent">The build intent to resolve. Specifies the type of object to spawn and its associated kind.</param>
    /// <returns>A BuildSpawnDescriptor representing the spawn configuration for the specified intent. The descriptor will
    /// correspond to either a floor or wall spawn, depending on the intent's kind.</returns>
    /// <exception cref="NotSupportedException">Thrown if the intent's kind is not supported for spawning.</exception>
    public static BuildSpawnDescriptor Resolve(BuildIntent intent)
    {
        return intent.Kind switch
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
                $"Spawning not supported for {intent.Kind}")
        };
    }
}
