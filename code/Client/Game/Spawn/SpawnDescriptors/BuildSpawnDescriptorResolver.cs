using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Structures;
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
    /// Resolves a structure definition identity to the existing floor or wall spawn descriptor.
    /// </summary>
    /// <param name="definitionId">The structure definition identity to resolve.</param>
    /// <returns>A build spawn descriptor for the registered client visual.</returns>
    /// <exception cref="NotSupportedException">Thrown if the definition is not supported for spawning.</exception>
    public static BuildSpawnDescriptor Resolve(StructureDefinitionId definitionId)
    {
        if (!BuildStructureDefinitions.TryGetBuildKind(definitionId, out BuildKind kind))
        {
            throw new NotSupportedException($"Spawning not supported for structure definition {definitionId}.");
        }

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
