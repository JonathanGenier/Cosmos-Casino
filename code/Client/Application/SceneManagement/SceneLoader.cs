using CosmosCasino.Core.Debug.Logging;
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

    /// <summary>
    /// Loads the specified scene and replaces the current scene tree root.
    /// <para>
    /// If the scene tree is not available, the method logs an error and
    /// exits gracefully without throwing.
    /// </para>
    /// </summary>
    /// <param name="path">Path to the scene to load.</param>
    public static bool Load(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            DevLog.Error("Scene", "Given scene path is null or empty.");
#if DEBUG
            throw new ArgumentException($"No scene path defined.");
#else
            return false;
#endif
        }

        var tree = Engine.GetMainLoop() as SceneTree;

        if(tree == null)
        {
            DevLog.Error("Scene", "SceneTree not available to change scenes.");
            return false;
        }

        tree.ChangeSceneToFile(path);
        return true;
    }
}