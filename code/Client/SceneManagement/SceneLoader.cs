using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Provides a centralized and engine-safe way to change scenes.
/// <para>
/// <see cref="SceneLoader"/> isolates all scene-loading logic from
/// application state management and UI code, ensuring that scene
/// transitions occur through a single, consistent entry point.
/// </para>
/// </summary>
public static class SceneLoader
{
    #region METHODS

    /// <summary>
    /// Loads and activates a new scene at the specified path.
    /// <para>
    /// The current scene is safely removed, the new scene is instantiated,
    /// and core and client service dependencies are injected before the scene
    /// becomes active.
    /// </para>
    /// </summary>
    /// <param name="path">
    /// Resource path of the scene to load.
    /// </param>
    /// <param name="coreServices">
    /// Core services shared across all application layers.
    /// </param>
    /// <param name="clientServices">
    /// Client-side services required by presentation-layer systems.
    /// </param>
    /// <returns>
    /// The instantiated root node of the loaded scene, or <c>null</c>
    /// if the scene could not be loaded.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown in debug builds if the scene path is null or empty.
    /// </exception>
    public static Node Load(string path, CoreServices coreServices, ClientServices clientServices)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            ConsoleLog.Error(nameof(SceneLoader), "Scene path is null or empty.");
#if DEBUG
            throw new ArgumentException("Scene path is null or empty.", nameof(path));
#else
            return null;
#endif
        }

        var tree = Engine.GetMainLoop() as SceneTree;

        if (tree == null)
        {
            ConsoleLog.Error(nameof(SceneLoader), $"{nameof(SceneTree)} not available.");
            return null;
        }

        var packedScene = GD.Load<PackedScene>(path);

        if (packedScene == null)
        {
            ConsoleLog.Error(nameof(SceneLoader), $"Failed to load scene at {path}");
            return null;
        }

        // Remove current scene
        tree.CurrentScene?.QueueFree();

        // Instantiate
        var instance = packedScene.Instantiate<Node>();

        // Inject dependencies
        if (instance is SceneController sceneController)
        {
            sceneController.Initialize(coreServices, clientServices);
        }
        else
        {
            ConsoleLog.Warning(nameof(SceneLoader), $"Loaded scene does not inherit {nameof(SceneController)}.");
        }

        // Attach
        tree.Root.AddChild(instance);
        tree.CurrentScene = instance;

        return instance;
    }


    #endregion
}