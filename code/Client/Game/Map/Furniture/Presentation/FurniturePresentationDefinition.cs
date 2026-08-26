using CosmosCasino.Core.Game.Furniture;
using Godot;
using System;

/// <summary>
/// Client-owned presentation metadata for one authoritative Core furniture definition.
/// </summary>
internal sealed class FurniturePresentationDefinition
{
    #region Initialization

    /// <summary>
    /// Initializes a new furniture presentation definition.
    /// </summary>
    /// <param name="definitionId">The authoritative Core furniture definition identity.</param>
    /// <param name="scene">The reusable Client scene for this furniture presentation.</param>
    /// <param name="sceneLocalOffset">The local visual offset applied to the scene root.</param>
    internal FurniturePresentationDefinition(
        FurnitureDefinitionId definitionId,
        PackedScene scene,
        Vector3 sceneLocalOffset)
    {
        ArgumentNullException.ThrowIfNull(scene);

        DefinitionId = definitionId;
        Scene = scene;
        SceneLocalOffset = sceneLocalOffset;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative Core furniture definition identity.
    /// </summary>
    internal FurnitureDefinitionId DefinitionId { get; }

    /// <summary>
    /// Gets the reusable Client scene for this furniture presentation.
    /// </summary>
    internal PackedScene Scene { get; }

    /// <summary>
    /// Gets the local visual offset applied to the scene root.
    /// </summary>
    internal Vector3 SceneLocalOffset { get; }

    #endregion
}
