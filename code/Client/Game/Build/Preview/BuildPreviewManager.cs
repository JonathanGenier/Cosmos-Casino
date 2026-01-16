using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map.Cell;
using Godot;
using System;
using System.Collections.Generic;

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

    private const int DefaultFloorPoolSize = 128;
    private const int DefaultWallPoolSize = 64;

    private readonly List<FloorPreview> _floorPreviews = new();
    private readonly List<WallPreview> _wallPreviews = new();

    private ClientPool<FloorPreview>? _floorPool;
    private ClientPool<WallPreview>? _wallPool;

    private PackedScene? _gridPreviewScene;
    private PackedScene? _floorPreviewScene;
    private PackedScene? _wallPreviewScene;

    private BuildGridPreview? _gridPreviewInstance;
    private FloorPreview? _floorPreviewInstance;
    private WallPreview? _wallPreviewInstance;

    private bool _isBuilding;

    #endregion

    #region Properties

    private PackedScene GridPreviewScene
    {
        get => _gridPreviewScene ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");
        set => _gridPreviewScene = value;
    }

    private PackedScene FloorPreviewScene
    {
        get => _floorPreviewScene ?? throw new InvalidOperationException($"{nameof(PackedScene)} not initialized.");
        set => _floorPreviewScene = value;
    }

    private PackedScene WallPreviewScene
    {
        get => _wallPreviewScene ?? throw new InvalidOperationException($"{nameof(PackedScene)} not initialized.");
        set => _wallPreviewScene = value;
    }

    private BuildGridPreview GridPreviewInstance
    {
        get => _gridPreviewInstance ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} not initialized.");
        set => _gridPreviewInstance = value;
    }

    private FloorPreview FloorPreviewInstance
    {
        get => _floorPreviewInstance ?? throw new InvalidOperationException($"{nameof(FloorPreview)} not initialized.");
        set => _floorPreviewInstance = value;
    }

    private WallPreview WallPreviewInstance
    {
        get => _wallPreviewInstance ?? throw new InvalidOperationException($"{nameof(WallPreview)} not initialized.");
        set => _wallPreviewInstance = value;
    }

    private ClientPool<FloorPreview> FloorPool
    {
        get => _floorPool ?? throw new InvalidOperationException($"{nameof(ClientPool<FloorPreview>)} not initialized.");
        set => _floorPool = value;
    }

    private ClientPool<WallPreview> WallPool
    {
        get => _wallPool ?? throw new InvalidOperationException($"{nameof(ClientPool<WallPreview>)} not initialized.");
        set => _wallPool = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component with the specified preview resources, build context, and cursor manager.
    /// </summary>
    /// <param name="previewResources">The set of resources used for previewing the grid. Cannot be null.</param>
    public void Initialize(PreviewResources previewResources)
    {
        ArgumentNullException.ThrowIfNull(previewResources);

        GridPreviewScene = previewResources.GridPreviewScene;
        FloorPreviewScene = previewResources.FloorPreviewScene;
        WallPreviewScene = previewResources.WallPreviewScene;

        FloorPool = new ClientPool<FloorPreview>(
            DefaultFloorPoolSize,
            CreatePoolFloorPreview,
            ResetPoolFloorPreview
        );

        WallPool = new ClientPool<WallPreview>(
            DefaultWallPoolSize,
            CreatePoolWallPreview,
            ResetPoolWallPreview
        );

        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Displays a visual preview of the specified build type at the given world position.
    /// </summary>
    /// <remarks>If a build operation is currently in progress, the preview will not be shown. Only one type
    /// of preview (floor or wall) is visible at a time, depending on the specified build kind.</remarks>
    /// <param name="kind">The type of build element to preview. Determines whether a floor or wall preview is shown.</param>
    /// <param name="worldPosition">The position in world coordinates where the preview will be displayed.</param>
    public void ShowPreview(BuildKind kind, Vector3 worldPosition)
    {
        ShowGridCursorPreview(worldPosition);

        if (_isBuilding)
        {
            return;
        }

        switch (kind)
        {
            case BuildKind.Floor:
                ShowFloorCursorPreview(worldPosition);
                HideWallCursorPreview();
                break;
            case BuildKind.Wall:
                ShowWallCursorPreview(worldPosition);
                HideFloorCursorPreview();
                break;
            default:
                HideFloorCursorPreview();
                HideWallCursorPreview();
                break;
        }
    }

    /// <summary>
    /// Hides all currently visible preview elements, including grid, floor, and wall previews.
    /// </summary>
    /// <remarks>Call this method to clear all preview visuals from the interface. This is typically used when
    /// resetting the preview state or when previews are no longer needed.</remarks>
    public void HideAllPreviews()
    {
        HideGridCursorPreview();
        HideFloorCursorPreview();
        HideWallCursorPreview();
    }

    /// <summary>
    /// Displays a visual preview of the specified build type over the given map cells.
    /// </summary>
    /// <remarks>This method enables build preview mode, allowing users to visualize placement before
    /// confirming a build action. The preview style depends on the specified build kind.</remarks>
    /// <param name="kind">The type of build to preview, such as floor or wall. Determines the style of the preview shown.</param>
    /// <param name="cells">A read-only list of map cell coordinates where the build preview will be displayed. Cannot be null or empty.</param>
    public void ShowBuildPreview(BuildKind kind, IReadOnlyList<MapCellCoord> cells)
    {
        _isBuilding = true;

        switch (kind)
        {
            case BuildKind.Floor:
                ShowFloorDragPreview(cells);
                break;

            case BuildKind.Wall:
                ShowWallDragPreview(cells);
                break;
        }
    }

    /// <summary>
    /// Removes all build preview elements from the scene and resets the building state.
    /// </summary>
    /// <remarks>Call this method to clear any temporary floor or wall previews that were created during the
    /// build process. After calling this method, the system will no longer be in build preview mode.</remarks>
    public void ClearBuildPreview()
    {
        ClearFloorDragPreview();
        ClearWallDragPreview();
        _isBuilding = false;
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
        InitializeGridCursorPreview();
        InitializeFloorCursorPreview();
        InitializeWallCursorPreview();
    }

    #endregion

    #region Floor Dragging Preview

    /// <summary>
    /// Displays a visual preview of floor tiles at the specified map cell coordinates, updating or creating preview
    /// objects as needed.
    /// </summary>
    /// <remarks>Existing preview objects are reused or returned to the pool to optimize performance. This
    /// method hides any previous floor previews before displaying the new ones.</remarks>
    /// <param name="cells">A read-only list of map cell coordinates where floor previews should be shown. The list must not be null.</param>
    private void ShowFloorDragPreview(IReadOnlyList<MapCellCoord> cells)
    {
        HideFloorCursorPreview();

        int i = 0;

        // Reuse existing previews
        for (; i < cells.Count && i < _floorPreviews.Count; i++)
        {
            _floorPreviews[i].SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            _floorPreviews[i].Show();
        }

        // Fetch new previews if needed
        for (; i < cells.Count; i++)
        {
            var preview = FloorPool!.Fetch();
            preview.SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            preview.Show();
            _floorPreviews.Add(preview);
        }

        // Return unused previews to pool
        for (int j = cells.Count; j < _floorPreviews.Count; j++)
        {
            FloorPool.Return(_floorPreviews[j]);
        }

        // Trim the list
        if (_floorPreviews.Count > cells.Count)
        {
            _floorPreviews.RemoveRange(cells.Count, _floorPreviews.Count - cells.Count);
        }
    }

    /// <summary>
    /// Removes all floor preview objects from the scene and returns them to the object pool.
    /// </summary>
    /// <remarks>This method should be called when floor previews are no longer needed, such as when resetting
    /// or updating the scene. All returned objects are cleared from the internal collection and made available for
    /// reuse.</remarks>
    private void ClearFloorDragPreview()
    {
        foreach (var preview in _floorPreviews)
        {
            FloorPool!.Return(preview);
        }

        _floorPreviews.Clear();
    }

    #endregion

    #region Wall Dragging Preview

    /// <summary>
    /// Displays a visual preview of wall segments at the specified map cell coordinates.
    /// </summary>
    /// <remarks>This method replaces any existing wall previews with new previews at the provided
    /// coordinates. It is typically used to give users visual feedback before confirming wall placement.</remarks>
    /// <param name="cells">A read-only list of map cell coordinates where wall previews will be shown. Cannot be null.</param>
    private void ShowWallDragPreview(IReadOnlyList<MapCellCoord> cells)
    {
        HideWallCursorPreview();
        ClearWallDragPreview();

        for (int i = 0; i < cells.Count; i++)
        {
            var preview = WallPool!.Fetch();
            preview.SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            preview.Show();
            _wallPreviews.Add(preview);
        }
    }

    /// <summary>
    /// Removes all wall preview objects and returns them to the wall pool.
    /// </summary>
    /// <remarks>Call this method to clear the current set of wall previews and release their resources. This
    /// is typically used when resetting or updating the wall preview state.</remarks>
    private void ClearWallDragPreview()
    {
        foreach (var preview in _wallPreviews)
        {
            WallPool!.Return(preview);
        }

        _wallPreviews.Clear();
    }

    #endregion

    #region Pooling

    /// <summary>
    /// Creates and adds a hidden pool floor preview to the scene.
    /// </summary>
    /// <remarks>The returned preview is initially hidden and must be shown explicitly if needed.</remarks>
    /// <returns>A new instance of <see cref="FloorPreview"/> representing the pool floor preview.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the root node of <c>FloorPreviewScene</c> is not of type <see cref="FloorPreview"/>.</exception>
    private FloorPreview CreatePoolFloorPreview()
    {
        var node = FloorPreviewScene.Instantiate();
        var preview = node as FloorPreview ?? throw new InvalidOperationException("FloorPreviewScene root must be FloorPreview");

        AddChild(preview);
        preview.Hide();
        return preview;
    }

    /// <summary>
    /// Resets the specified pool floor preview by hiding it.
    /// </summary>
    /// <param name="preview">The floor preview instance to be reset. Cannot be null.</param>
    private void ResetPoolFloorPreview(FloorPreview preview)
    {
        preview.Hide();
    }

    /// <summary>
    /// Creates and adds a hidden pool wall preview to the current scene.
    /// </summary>
    /// <remarks>The returned preview is hidden by default and must be shown explicitly before use.</remarks>
    /// <returns>A new instance of <see cref="WallPreview"/> that has been added as a child and is initially hidden.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the root node of <c>WallPreviewScene</c> is not of type <see cref="WallPreview"/>.</exception>
    private WallPreview CreatePoolWallPreview()
    {
        var node = WallPreviewScene.Instantiate();
        var preview = node as WallPreview ?? throw new InvalidOperationException("WallPreviewScene root must be WallPreview");

        AddChild(preview);
        preview.Hide();
        return preview;
    }

    /// <summary>
    /// Resets the specified pool wall preview by hiding it.
    /// </summary>
    /// <param name="preview">The wall preview instance to be reset. Cannot be null.</param>
    private void ResetPoolWallPreview(WallPreview preview)
    {
        preview.Hide();
    }

    #endregion

    #region Cursor Preview Initialization

    /// <summary>
    /// Initializes the grid preview instance and adds it to the scene for display and interaction.
    /// </summary>
    /// <remarks>This method creates a new grid preview, configures its initial state, and ensures it is
    /// hidden until explicitly shown. The grid preview is set up with a default tile diameter of 15 units.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the root node of the instantiated scene does not have a BuildGridPreview script attached.</exception>
    private void InitializeGridCursorPreview()
    {
        var node = GridPreviewScene.Instantiate();
        GridPreviewInstance = node as BuildGridPreview ?? throw new InvalidOperationException($"{nameof(PackedScene)} root node must have {nameof(BuildGridPreview)} script attached.");
        AddChild(GridPreviewInstance);
        GridPreviewInstance.Hide();
        GridPreviewInstance.SetTileDiameter(15);
    }

    /// <summary>
    /// Initializes the floor preview by instantiating the preview scene and adding it as a child node. The preview is
    /// hidden after initialization.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the root node of the instantiated scene does not have a FloorPreview script attached.</exception>
    private void InitializeFloorCursorPreview()
    {
        var node = FloorPreviewScene.Instantiate();
        FloorPreviewInstance = node as FloorPreview ?? throw new InvalidOperationException($"{nameof(PackedScene)} root node must have {nameof(FloorPreview)} script attached.");
        AddChild(FloorPreviewInstance);
        FloorPreviewInstance.Hide();
    }

    /// <summary>
    /// Initializes the wall preview instance by instantiating the associated scene and adding it as a child node.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the root node of the instantiated scene does not have the WallPreview script attached.</exception>
    private void InitializeWallCursorPreview()
    {
        var node = WallPreviewScene.Instantiate();
        WallPreviewInstance = node as WallPreview ?? throw new InvalidOperationException($"{nameof(PackedScene)} root node must have {nameof(WallPreview)} script attached.");
        AddChild(WallPreviewInstance);
        WallPreviewInstance.Hide();
    }

    #endregion

    #region Floor Cursor Preview

    /// <summary>
    /// Displays the floor preview at the specified world position if the preview instance is active in the scene.
    /// </summary>
    /// <param name="worldPosition">The position in world coordinates where the floor preview should be shown.</param>
    private void ShowFloorCursorPreview(Vector3 worldPosition)
    {
        if (!FloorPreviewInstance.IsInsideTree())
        {
            return;
        }

        FloorPreviewInstance.Show();
        FloorPreviewInstance.SetWorldPosition(worldPosition);
    }

    /// <summary>
    /// Hides the currently displayed floor preview, if one is visible.
    /// </summary>
    private void HideFloorCursorPreview()
    {
        FloorPreviewInstance.Hide();
    }

    #endregion

    #region Wall Cursor Preview

    /// <summary>
    /// Displays the wall preview at the specified world position if the preview instance is active in the scene.
    /// </summary>
    /// <remarks>This method has no effect if the wall preview instance is not currently part of the scene
    /// tree.</remarks>
    /// <param name="worldPosition">The position in world coordinates where the wall preview should be shown.</param>
    private void ShowWallCursorPreview(Vector3 worldPosition)
    {
        if (!WallPreviewInstance.IsInsideTree())
        {
            return;
        }

        WallPreviewInstance.Show();
        WallPreviewInstance.SetWorldPosition(worldPosition);
    }

    /// <summary>
    /// Hides the currently displayed wall preview, if visible.
    /// </summary>
    private void HideWallCursorPreview()
    {
        WallPreviewInstance.Hide();
    }

    #endregion

    #region Build Grid Cursor Preview

    /// <summary>
    /// Displays the grid preview at the specified world position.
    /// </summary>
    /// <param name="worldPosition">The position in world coordinates where the grid preview will be shown.</param>
    private void ShowGridCursorPreview(Vector3 worldPosition)
    {
        GridPreviewInstance.Show();
        GridPreviewInstance.UpdatePosition(worldPosition);
    }

    /// <summary>
    /// Hides the preview grid from the user interface.
    /// </summary>
    private void HideGridCursorPreview()
    {
        GridPreviewInstance.Hide();
    }

    #endregion
}