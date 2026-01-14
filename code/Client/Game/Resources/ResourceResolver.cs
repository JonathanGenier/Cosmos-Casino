using Godot;
using System;

/// <summary>
/// Provides utility methods for resolving resources from a <see cref="ResourcePreloader"/> instance.
/// </summary>
/// <remarks>This class contains static methods to simplify retrieving and casting resources from a <see
/// cref="ResourcePreloader"/>. It is intended for use in scenarios where resources need to be accessed by key and cast
/// to specific types. All members are static and thread safety depends on the underlying <see
/// cref="ResourcePreloader"/> implementation.</remarks>
public static class ResourceResolver
{
    /// <summary>
    /// Retrieves a packed scene resource from the specified preloader using the given key.
    /// </summary>
    /// <param name="preloader">The resource preloader instance from which to retrieve the packed scene.</param>
    /// <param name="key">The key that identifies the resource to retrieve from the preloader.</param>
    /// <returns>The packed scene associated with the specified key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resource identified by <paramref name="key"/> is not a <see cref="PackedScene"/>.</exception>
    public static PackedScene GetPackedScene(ResourcePreloader preloader, string key)
    {
        var resource = preloader.GetResource(key);

        if (resource is not PackedScene scene)
        {
            throw new InvalidOperationException($"Resource '{key}' is not a PackedScene.");
        }

        return scene;
    }
}
