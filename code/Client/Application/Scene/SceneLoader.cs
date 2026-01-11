using Godot;
using System;

/// <summary>
/// Provides functionality to load and replace scenes within a scene tree.
/// </summary>
/// <remarks>Use this class to manage scene transitions by loading new scenes and replacing the current scene in
/// the associated scene tree. This class is intended for use with Godot's scene management system and is not
/// thread-safe.</remarks>
public sealed class SceneLoader
{
    #region Fields

    private readonly SceneTree _sceneTree;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the SceneLoader class using the specified scene tree.
    /// </summary>
    /// <param name="sceneTree">The SceneTree instance to be used for scene management. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if sceneTree is null.</exception>
    public SceneLoader(SceneTree sceneTree)
    {
        _sceneTree = sceneTree ?? throw new ArgumentNullException(nameof(sceneTree));
    }

    #endregion

    #region Load Method

    /// <summary>
    /// Loads a scene from the specified path and sets it as the current scene in the scene tree.
    /// </summary>
    /// <remarks>If a current scene exists, it is freed before the new scene is loaded and set as current. The
    /// loaded scene is added as a child of the root node in the scene tree.</remarks>
    /// <param name="path">The file system path to the scene resource to load. Cannot be null, empty, or consist only of white-space
    /// characters.</param>
    /// <returns>The root node of the loaded scene instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the scene resource at the specified path cannot be loaded.</exception>
    public Node Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, $"Scene path is null or empty for {nameof(path)}");

        var packedScene = GD.Load<PackedScene>(path) ?? throw new InvalidOperationException($"Failed to load scene at {path}");

        _sceneTree.CurrentScene?.QueueFree();

        var instance = packedScene.Instantiate<Node>();
        _sceneTree.Root.AddChild(instance);
        _sceneTree.CurrentScene = instance;

        return instance;
    }

    #endregion
}