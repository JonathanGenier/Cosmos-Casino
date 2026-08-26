using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Transient Client view for one scene-rendered authoritative Furniture aggregate.
/// </summary>
internal sealed partial class FurnitureSceneView : Node3D
{
    #region Fields

    private Node3D? _sceneRoot;
    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative furniture identity represented by this view.
    /// </summary>
    internal FurnitureId FurnitureId { get; private set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the scene view from an authoritative furniture snapshot and Client presentation metadata.
    /// </summary>
    /// <param name="snapshot">The authoritative furniture snapshot.</param>
    /// <param name="presentation">The scene presentation metadata.</param>
    internal void Initialize(
        FurnitureSnapshot snapshot,
        FurniturePresentationDefinition presentation)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(FurnitureSceneView)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(presentation);

        FurnitureId = snapshot.Id;
        Name = $"FurnitureSceneView_{snapshot.Id.Value}";
        Transform = GridPlacementTransformResolver.ResolveWorldTransform(snapshot.Anchor, snapshot.Rotation);
        _sceneRoot = InstantiateSceneRoot(presentation);
        AddChild(_sceneRoot);
        _isInitialized = true;
    }

    #endregion

    #region Helpers

    private static Node3D InstantiateSceneRoot(FurniturePresentationDefinition presentation)
    {
        Node scene = presentation.Scene.Instantiate();

        if (scene is not Node3D sceneRoot)
        {
            throw new InvalidOperationException(
                $"Furniture scene presentation '{presentation.DefinitionId}' must instantiate a {nameof(Node3D)}.");
        }

        sceneRoot.Position = presentation.SceneLocalOffset;
        return sceneRoot;
    }

    #endregion
}
