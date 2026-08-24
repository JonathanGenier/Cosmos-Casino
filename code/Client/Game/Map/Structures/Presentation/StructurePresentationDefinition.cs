using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// Client-owned presentation metadata for one authoritative Core structure definition.
/// </summary>
internal sealed class StructurePresentationDefinition
{
    #region Fields

    private readonly Mesh? _mesh;
    private readonly PackedScene? _scene;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new presentation definition.
    /// </summary>
    /// <param name="definitionId">The authoritative Core structure definition identity.</param>
    /// <param name="presentationKey">The Client presentation key used for batching.</param>
    /// <param name="renderStrategy">The Client rendering strategy.</param>
    /// <param name="mesh">The repeated-instance mesh, when this presentation uses MultiMesh rendering.</param>
    /// <param name="material">The repeated-instance material override.</param>
    /// <param name="scene">The reusable scene, when this presentation uses Scene rendering.</param>
    /// <param name="localBoundsSize">The local presentation bounds used by batched collision.</param>
    /// <param name="sceneLocalOffset">The local visual offset applied to scene-rendered presentation roots.</param>
    /// <param name="useColors">Whether the MultiMesh should allocate per-instance color data.</param>
    /// <param name="useCustomData">Whether the MultiMesh should allocate per-instance custom data.</param>
    internal StructurePresentationDefinition(
        StructureDefinitionId definitionId,
        StructurePresentationKey presentationKey,
        StructureRenderStrategy renderStrategy,
        Mesh? mesh,
        Material? material,
        PackedScene? scene,
        Vector3 localBoundsSize,
        Vector3 sceneLocalOffset,
        bool useColors = false,
        bool useCustomData = false)
    {
        if (renderStrategy == StructureRenderStrategy.MultiMesh)
        {
            ArgumentNullException.ThrowIfNull(mesh);
        }
        else if (renderStrategy == StructureRenderStrategy.Scene)
        {
            ArgumentNullException.ThrowIfNull(scene);
        }

        DefinitionId = definitionId;
        PresentationKey = presentationKey;
        RenderStrategy = renderStrategy;
        _mesh = mesh;
        _scene = scene;
        Material = material;
        LocalBoundsSize = localBoundsSize;
        SceneLocalOffset = sceneLocalOffset;
        UseColors = useColors;
        UseCustomData = useCustomData;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative Core structure definition identity.
    /// </summary>
    internal StructureDefinitionId DefinitionId { get; }

    /// <summary>
    /// Gets the Client presentation key used for batching.
    /// </summary>
    internal StructurePresentationKey PresentationKey { get; }

    /// <summary>
    /// Gets the Client rendering strategy.
    /// </summary>
    internal StructureRenderStrategy RenderStrategy { get; }

    /// <summary>
    /// Gets the repeated-instance mesh for MultiMesh presentations.
    /// </summary>
    internal Mesh Mesh => _mesh ?? throw new InvalidOperationException("Presentation does not define a mesh.");

    /// <summary>
    /// Gets the reusable scene for Scene presentations.
    /// </summary>
    internal PackedScene Scene => _scene ?? throw new InvalidOperationException("Presentation does not define a scene.");

    /// <summary>
    /// Gets the repeated-instance material override.
    /// </summary>
    internal Material? Material { get; }

    /// <summary>
    /// Gets the local presentation bounds used by batched collision.
    /// </summary>
    internal Vector3 LocalBoundsSize { get; }

    /// <summary>
    /// Gets the local visual offset applied to scene-rendered presentation roots.
    /// </summary>
    internal Vector3 SceneLocalOffset { get; }

    /// <summary>
    /// Gets a value indicating whether the MultiMesh should allocate per-instance color data.
    /// </summary>
    internal bool UseColors { get; }

    /// <summary>
    /// Gets a value indicating whether the MultiMesh should allocate per-instance custom data.
    /// </summary>
    internal bool UseCustomData { get; }

    #endregion
}
