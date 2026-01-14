using Godot;
using System;

/// <summary>
/// Manages the visual preview of build placement within the game world, including grid visualization and cursor
/// tracking during build operations.
/// </summary>
/// <remarks>Use this class to initialize and control the build preview system, which provides real-time feedback
/// to users when placing objects. The manager must be initialized with required resources and context before use. The
/// preview is automatically shown or hidden based on the current build context and cursor position. This class is not
/// thread-safe and should be used on the main thread.</remarks>
public sealed partial class BuildPreviewManager : InitializableNodeManager
{
    #region Fields

    private bool _previewActive;

    private BuildContext? _buildContext;
    private CursorManager? _cursorManager;
    private PackedScene? _gridPreviewScene;
    private BuildGridPreview? _gridPreviewInstance;

    #endregion

    #region Properties

    private BuildContext BuildContext =>
        _buildContext ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");

    private CursorManager CursorManager =>
        _cursorManager ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");

    private PackedScene GridPreviewScene =>
        _gridPreviewScene ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");

    private BuildGridPreview GridPreviewInstance =>
        _gridPreviewInstance ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component with the specified preview resources, build context, and cursor manager.
    /// </summary>
    /// <param name="previewResources">The set of resources used for previewing the grid. Cannot be null.</param>
    /// <param name="buildContext">The context containing build-related information required for initialization. Cannot be null.</param>
    /// <param name="cursorManager">The manager responsible for handling cursor operations during preview and build. Cannot be null.</param>
    public void Initialize(
        PreviewResources previewResources,
        BuildContext buildContext,
        CursorManager cursorManager)
    {
        ArgumentNullException.ThrowIfNull(previewResources);
        ArgumentNullException.ThrowIfNull(buildContext);
        ArgumentNullException.ThrowIfNull(cursorManager);

        _gridPreviewScene = previewResources.GridPreviewScene;
        _buildContext = buildContext;
        _cursorManager = cursorManager;

        MarkInitialized();
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Initializes the node when it enters the scene tree, setting up the grid preview and subscribing to build context
    /// changes.
    /// </summary>
    /// <remarks>This method is called by the Godot engine as part of the node's lifecycle. It instantiates
    /// and configures the grid preview, and attaches an event handler to respond to changes in the build context.
    /// Override this method to perform setup tasks that require the node to be part of the scene tree.</remarks>
    protected override void OnReady()
    {
        _gridPreviewInstance = GridPreviewScene.Instantiate<BuildGridPreview>();
        AddChild(_gridPreviewInstance);
        _gridPreviewInstance.Hide();

        GridPreviewInstance.SetTileDiameter(15);

        BuildContext.ContextChanged += OnBuildContextChanged;
    }

    /// <summary>
    /// Performs cleanup operations when the application exits.
    /// </summary>
    /// <remarks>This method is called during the application's shutdown sequence. Override this method to
    /// release resources or unsubscribe from events to prevent memory leaks. Always call the base implementation if
    /// additional cleanup is required in derived classes.</remarks>
    protected override void OnExit()
    {
        BuildContext.ContextChanged -= OnBuildContextChanged;
    }

    /// <summary>
    /// Processes the grid preview state for the current frame, updating its visibility and position based on the cursor
    /// location.
    /// </summary>
    /// <remarks>The grid preview is only updated when preview mode is active and a valid cursor position is
    /// available. If the cursor position cannot be determined, the grid preview is hidden for the current
    /// frame.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the last frame. This value can be used for time-based calculations within
    /// the processing logic.</param>
    protected override void OnProcess(double delta)
    {
        if (!_previewActive)
        {
            return;
        }

        if (!CursorManager.TryGetCursorPosition(out var worldPos))
        {
            GridPreviewInstance.Hide();
            return;
        }

        GridPreviewInstance.Show();
        GridPreviewInstance.UpdatePosition(worldPos);
    }

    #endregion

    #region Context Handling

    /// <summary>
    /// Handles changes to the build context by updating the preview state.
    /// </summary>
    /// <param name="context">The new build context to apply. If <paramref name="context"/> is <see langword="null"/>, the preview is
    /// deactivated.</param>
    private void OnBuildContextChanged(BuildContextBase? context)
    {
        _previewActive = context != null;

        if (!_previewActive)
        {
            GridPreviewInstance.Hide();
        }
    }

    #endregion
}