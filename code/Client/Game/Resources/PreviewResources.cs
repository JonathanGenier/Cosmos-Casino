using Godot;
using System;

/// <summary>
/// Provides access to editor preview resources, including the packed scene used for grid previews.
/// </summary>
/// <remarks>Use this class to retrieve preview assets required for displaying grid previews in the editor.
/// Instances are created via the <see cref="Assemble"/> method, which loads the necessary resources from a <see
/// cref="ResourcePreloader"/>. This class is sealed and cannot be inherited.</remarks>
public sealed class PreviewResources
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the PreviewResources class with the specified grid preview scene.
    /// </summary>
    /// <param name="gridPreviewScene">The PackedScene instance to use for grid previews. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="gridPreviewScene"/> is null.</exception>
    private PreviewResources(PackedScene gridPreviewScene)
    {
        GridPreviewScene = gridPreviewScene ?? throw new ArgumentNullException(nameof(gridPreviewScene));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the packed scene used to display the grid preview in the editor.
    /// </summary>
    public PackedScene GridPreviewScene { get; }

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
            ResourceResolver.GetPackedScene(resources, "build_grid_preview")
        );
    }

    #endregion
}
