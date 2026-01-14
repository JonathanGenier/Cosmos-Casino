using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Provides access to core services required during a build process, including build management, context information,
/// cursor control, and preview resources.
/// </summary>
/// <remarks>Use this class to coordinate and manage build-related operations by accessing its service properties.
/// All service dependencies must be supplied when constructing an instance. This class is sealed and cannot be
/// inherited.</remarks>
public sealed class BuildProcessServices
{
    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildProcessServices class with the specified build manager, build context,
    /// cursor manager, and preview resources.
    /// </summary>
    /// <param name="buildManager">The BuildManager instance that manages build operations and coordination.</param>
    /// <param name="buildContext">The BuildContext instance that provides contextual information for the build process.</param>
    /// <param name="cursorManager">The CursorManager instance used to control cursor state during build operations.</param>
    /// <param name="previewResources">The PreviewResources instance that supplies resources for previewing build results.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    public BuildProcessServices(
        BuildManager buildManager,
        BuildContext buildContext,
        CursorManager cursorManager,
        PreviewResources previewResources)
    {
        BuildManager = buildManager ?? throw new ArgumentNullException(nameof(buildManager));
        BuildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
        CursorManager = cursorManager ?? throw new ArgumentNullException(nameof(cursorManager));
        PreviewResources = previewResources ?? throw new ArgumentNullException(nameof(previewResources));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the build manager used to coordinate and execute build operations.
    /// </summary>
    public BuildManager BuildManager { get; }

    /// <summary>
    /// Gets the current build context associated with the instance.
    /// </summary>
    public BuildContext BuildContext { get; }

    /// <summary>
    /// Gets the manager responsible for handling cursor operations within the application.
    /// </summary>
    public CursorManager CursorManager { get; }

    /// <summary>
    /// Gets the collection of resources available for preview in the current context.
    /// </summary>
    public PreviewResources PreviewResources { get; }

    #endregion
}