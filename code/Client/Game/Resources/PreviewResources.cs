using Godot;
using System;

/// <summary>
/// Provides access to build preview resources.
/// </summary>
/// <remarks>Use this class to retrieve preview assets required for displaying build previews.
/// Instances are created via the <see cref="Assemble"/> method, which loads the necessary resources from a <see
/// cref="ResourcePreloader"/>. This class is sealed and cannot be inherited.</remarks>
public sealed class PreviewResources
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the PreviewResources class with the specified preview scenes.
    /// </summary>
    /// <param name="floorPreviewScene">The PackedScene instance to use for floor previews. Cannot be null.</param>
    /// <param name="wallPreviewScene">The PackedScene instance to use for wall previews. Cannot be null.</param>
    private PreviewResources(
        PackedScene floorPreviewScene,
        PackedScene wallPreviewScene)
    {
        FloorPreviewScene = floorPreviewScene ?? throw new ArgumentNullException(nameof(floorPreviewScene));
        WallPreviewScene = wallPreviewScene ?? throw new ArgumentNullException(nameof(wallPreviewScene));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the scene used to display a preview of the floor in the editor or during runtime.
    /// </summary>
    public PackedScene FloorPreviewScene { get; }

    /// <summary>
    /// Gets the scene resource used to display a preview of a wall in the editor or during placement operations.
    /// </summary>
    public PackedScene WallPreviewScene { get; }

    #endregion

    #region Assembly

    /// <summary>
    /// Creates a new instance of <see cref="PreviewResources"/> using the specified resource preloader.
    /// </summary>
    /// <param name="resources">The resource preloader containing the assets required to assemble the preview resources. Cannot be <see
    /// langword="null"/>.</param>
    /// <returns>A <see cref="PreviewResources"/> object initialized with the packed scene from the provided resources.</returns>
    public static PreviewResources Assemble(ResourcePreloader resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        return new PreviewResources(
            ResourceResolver.GetPackedScene(resources, "floor_preview"),
            ResourceResolver.GetPackedScene(resources, "wall_preview")
        );
    }

    #endregion
}
