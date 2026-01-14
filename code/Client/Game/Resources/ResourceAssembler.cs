using Godot;
using System;

/// <summary>
/// Provides functionality to assemble and expose spawn and preview resources for use within the node hierarchy.
/// </summary>
/// <remarks>This class initializes and manages access to spawn and preview resources by assembling them from
/// associated resource preloaders when the node is ready. Accessing the resources before initialization will result in
/// an exception. The class is sealed and intended for use as a component within a node tree.</remarks>
public sealed partial class ResourceAssembler : Node
{
    private SpawnResources? _spawnResources;
    private PreviewResources? _previewResources;

    #region Properties

    /// <summary>
    /// Gets the resource configuration used for spawning entities in the current context.
    /// </summary>
    /// <remarks>Accessing this property before it is initialized will result in an exception. This property
    /// is intended for internal use and cannot be set directly outside the class.</remarks>
    public SpawnResources SpawnResources
    {
        get => _spawnResources ?? throw new InvalidOperationException($"{nameof(SpawnResources)} is not initialized.");
        private set => _spawnResources = value;
    }

    /// <summary>
    /// Gets the set of resources used for preview operations.
    /// </summary>
    /// <remarks>Accessing this property before it is initialized will result in an exception. This property
    /// is intended for internal use and is not settable outside the containing class.</remarks>
    public PreviewResources PreviewResources
    {
        get => _previewResources ?? throw new InvalidOperationException($"{nameof(PreviewResources)} is not initialized.");
        private set => _previewResources = value;
    }

    #endregion

    #region Godot Processes

    /// <summary>
    /// Initializes the node when it enters the scene tree, preparing resource preloaders for spawning and previewing
    /// objects.
    /// </summary>
    /// <remarks>This method is called automatically by the Godot engine when the node is added to the scene.
    /// Override this method to perform setup tasks that require access to child nodes or resources. Ensure that the
    /// required child nodes 'SpawnResources' and 'PreviewResources' exist and are of type ResourcePreloader to avoid
    /// runtime errors.</remarks>
    public override void _Ready()
    {
        var spawnPreloader = GetNode<ResourcePreloader>("SpawnResources");
        var previewPreloader = GetNode<ResourcePreloader>("PreviewResources");

        Assemble(spawnPreloader, previewPreloader);
    }

    #endregion

    #region Assembly

    /// <summary>
    /// Initializes the spawn and preview resource collections using the specified resource preloaders.
    /// </summary>
    /// <param name="spawnPreloader">The resource preloader used to assemble the spawn resource collection. Cannot be null.</param>
    /// <param name="previewPreloader">The resource preloader used to assemble the preview resource collection. Cannot be null.</param>
    private void Assemble(
        ResourcePreloader spawnPreloader,
        ResourcePreloader previewPreloader)
    {
        SpawnResources = SpawnResources.Assemble(spawnPreloader);
        PreviewResources = PreviewResources.Assemble(previewPreloader);
    }

    #endregion
}
