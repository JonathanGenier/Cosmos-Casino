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
    #region PUBLIC METHODS

    /// <summary>
    /// Loads and transitions to a new scene at the specified path.
    /// </summary>
    /// <param name="path">
    /// Resource path to the scene to load (e.g. <c>"res://Scenes/Game.tscn"</c>).
    /// </param>
    /// <param name="coreServices">
    /// Core services container providing access to game-level systems.
    /// </param>
    /// <param name="clientServices">
    /// Client services container providing access to presentation-layer systems.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the scene was successfully loaded and attached;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown in debug builds if <paramref name="path"/> is null or empty.
    /// </exception>
    public static bool Load(string path, CoreServices coreServices, ClientServices clientServices)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            ConsoleLog.Error("Scene", "Scene path is null or empty.");
#if DEBUG
            throw new ArgumentException("Scene path is null or empty.", nameof(path));
#else
            return false;
#endif
        }

        var tree = Engine.GetMainLoop() as SceneTree;

        if (tree == null)
        {
            ConsoleLog.Error("Scene", "SceneTree not available.");
            return false;
        }

        var packedScene = GD.Load<PackedScene>(path);

        if (packedScene == null)
        {
            ConsoleLog.Error("Scene", $"Failed to load scene at path: {path}");
            return false;
        }

        // Remove current scene
        tree.CurrentScene?.QueueFree();

        // Instantiate
        var instance = packedScene.Instantiate<Node>();

        // Inject dependencies
        if (instance is SceneController controller)
        {
            controller.Initialize(coreServices, clientServices);
        }
        else
        {
            ConsoleLog.Warning("Scene", "Loaded scene does not inherit SceneController.");
        }

        // Attach
        tree.Root.AddChild(instance);
        tree.CurrentScene = instance;

        return true;
    }


    #endregion
}