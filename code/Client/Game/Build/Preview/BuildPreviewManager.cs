using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    private BuildPreviewMode _currentMode;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current build preview mode in use.
    /// </summary>
    public BuildPreviewMode CurrentMode => _currentMode;

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

        _currentMode = BuildPreviewMode.Cursor;
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Enables drag mode for the build preview, allowing objects to be positioned interactively.
    /// </summary>
    public void EnterDragMode()
    {
        if (_currentMode == BuildPreviewMode.Drag)
        {
            return;
        }

        _currentMode = BuildPreviewMode.Drag;
        ClearCursorPreview();
    }

    /// <summary>
    /// Exits drag mode and returns to cursor mode, clearing any active drag preview.
    /// </summary>
    /// <remarks>Call this method to end a drag operation and reset the preview state. If the preview is
    /// already in cursor mode, this method has no effect.</remarks>
    public void ExitDragMode()
    {
        if (_currentMode == BuildPreviewMode.Cursor)
        {
            return;
        }

        _currentMode = BuildPreviewMode.Cursor;
        ClearDragPreview();
    }

    /// <summary>
    /// Displays a visual preview of the specified build result based on the current preview mode.
    /// </summary>
    /// <remarks>The preview mode determines how the build result is displayed. For example, the preview may
    /// follow the cursor or appear as a draggable element, depending on the current mode.</remarks>
    /// <param name="buildResult">The build result to preview. Provides the data used to generate the visual representation.</param>
    public void ShowPreview(BuildResult buildResult)
    {
        switch (_currentMode)
        {
            case BuildPreviewMode.Cursor:
                ShowCursorPreview(buildResult);
                break;

            case BuildPreviewMode.Drag:
                ShowDragPreview(buildResult);
                break;
        }
    }

    /// <summary>
    /// Displays the grid preview at the specified cursor location.
    /// </summary>
    /// <param name="cursorContext">The context containing the current cursor position in world coordinates. Cannot be null.</param>
    public void ShowGridPreview(CursorContext cursorContext)
    {
        GridPreviewInstance.Show();
        GridPreviewInstance.UpdatePosition(cursorContext.WorldPosition);
    }

    /// <summary>
    /// Resizes the grid preview by setting the diameter of each tile to the specified value.
    /// </summary>
    /// <param name="size">The diameter, in tiles, to set the grid preview. Must be a positive integer.</param>
    public void ResizeGridPreview(int size)
    {
        GridPreviewInstance.SetTileDiameter(size);
    }

    /// <summary>
    /// Removes any active cursor preview from the user interface.
    /// </summary>
    /// <remarks>Call this method to clear both floor and wall cursor previews, typically when the user
    /// cancels a placement action or when the preview is no longer needed.</remarks>
    public void ClearCursorPreview()
    {
        HideFloorCursorPreview();
        HideWallCursorPreview();
    }

    /// <summary>
    /// Clears any active drag preview, resetting the build preview to the default cursor mode.
    /// </summary>
    /// <remarks>Call this method to remove any visual indicators or previews shown during a drag operation.
    /// After calling this method, no drag preview will be displayed until a new drag operation is initiated.</remarks>
    public void ClearDragPreview()
    {
        ClearFloorDragPreview();
        ClearWallDragPreview();
        _currentMode = BuildPreviewMode.Cursor;
    }

    /// <summary>
    /// Hides the current grid preview, removing it from view.
    /// </summary>
    public void ClearGridPreview()
    {
        GridPreviewInstance.Hide();
    }

    /// <summary>
    /// Clears any active grid and cursor preview overlays from the user interface.
    /// </summary>
    /// <remarks>Call this method to remove temporary visual previews related to grid and cursor actions. This
    /// is typically used to reset the interface before applying new previews or when cancelling an operation.</remarks>
    public void ClearGridAndCursorPreviews()
    {
        ClearGridPreview();
        ClearCursorPreview();
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

    #region Dragging Preview

    /// <summary>
    /// Displays a visual preview of the drag operation based on the specified build result.
    /// </summary>
    /// <param name="buildResult">The build result containing the intent and associated cell data used to determine the type and placement of the
    /// drag preview. Cannot be null.</param>
    private void ShowDragPreview(BuildResult buildResult)
    {
        var cells = buildResult.Intent.Cells;
        var results = buildResult.Results.ToDictionary(r => r.Cell);

        switch (buildResult.Intent.Kind)
        {
            case BuildKind.Floor:
                ShowFloorDragPreview(cells, results);
                break;

            case BuildKind.Wall:
                ShowWallDragPreview(cells, results);
                break;
        }
    }

    /// <summary>
    /// Displays a visual preview for floor placement at the specified map cell coordinates, indicating the validity of
    /// each placement based on the provided build results.
    /// </summary>
    /// <remarks>This method updates, creates, or removes floor preview visuals to match the specified cells.
    /// Each preview reflects whether placement at the corresponding cell is valid, as determined by the associated
    /// build operation result.</remarks>
    /// <param name="cells">A read-only list of map cell coordinates where floor previews should be shown.</param>
    /// <param name="results">A read-only dictionary mapping each map cell coordinate to its corresponding build operation result, used to
    /// determine the validity of each preview.</param>
    private void ShowFloorDragPreview(IReadOnlyList<MapCellCoord> cells, IReadOnlyDictionary<MapCellCoord, BuildOperationResult> results)
    {
        int i = 0;

        // Reuse existing previews
        for (; i < cells.Count && i < _floorPreviews.Count; i++)
        {
            var cell = cells[i];
            var result = GetResultOrThrow(results, cell);

            _floorPreviews[i].SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            _floorPreviews[i].SetValidity(result.Outcome);
            _floorPreviews[i].Show();
        }

        // Fetch new previews if needed
        for (; i < cells.Count; i++)
        {
            var cell = cells[i];
            var result = GetResultOrThrow(results, cell);
            var preview = FloorPool!.Fetch();

            preview.SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            preview.SetValidity(result.Outcome);
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
    /// Displays a visual preview for wall placement at the specified map cell coordinates, indicating the validity of
    /// each placement based on the provided build results.
    /// </summary>
    /// <remarks>This method updates, creates, or removes wall preview visuals to match the specified cells.
    /// Each preview reflects whether placement at the corresponding cell is valid, as determined by the associated
    /// build operation result.</remarks>
    /// <param name="cells">A read-only list of map cell coordinates where wall previews should be shown.</param>
    /// <param name="results">A read-only dictionary mapping each map cell coordinate to its corresponding build operation result, used to
    /// determine the validity of each preview.</param>
    private void ShowWallDragPreview(IReadOnlyList<MapCellCoord> cells, IReadOnlyDictionary<MapCellCoord, BuildOperationResult> results)
    {
        int i = 0;

        // Reuse existing previews
        for (; i < cells.Count && i < _wallPreviews.Count; i++)
        {
            var cell = cells[i];
            var result = GetResultOrThrow(results, cell);

            _wallPreviews[i].SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            _wallPreviews[i].SetValidity(result.Outcome);
            _wallPreviews[i].Show();
        }

        // Fetch new previews if needed
        for (; i < cells.Count; i++)
        {
            var cell = cells[i];
            var result = GetResultOrThrow(results, cell);
            var preview = WallPool!.Fetch();

            preview.SetWorldPosition(MapToWorld.CellToWorld(cells[i]));
            preview.SetValidity(result.Outcome);
            preview.Show();
            _wallPreviews.Add(preview);
        }

        // Return unused previews to pool
        for (int j = cells.Count; j < _wallPreviews.Count; j++)
        {
            WallPool.Return(_wallPreviews[j]);
        }

        // Trim the list
        if (_wallPreviews.Count > cells.Count)
        {
            _wallPreviews.RemoveRange(cells.Count, _wallPreviews.Count - cells.Count);
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

    #region Cursor Preview

    /// <summary>
    /// Displays a visual preview at the cursor's target cell based on the specified build result.
    /// </summary>
    /// <remarks>This method updates the cursor preview to reflect the type of build action (such as floor or
    /// wall) at the specified location. It hides any irrelevant previews to ensure only the appropriate cursor is
    /// shown.</remarks>
    /// <param name="buildResult">The result of the build operation, containing the intended cell and build kind for which to show the cursor
    /// preview.</param>
    /// <exception cref="InvalidOperationException">Thrown if the build result does not specify exactly one target cell.</exception>
    private void ShowCursorPreview(BuildResult buildResult)
    {
        if (buildResult.Intent.Cells.Count != 1)
        {
            throw new InvalidOperationException("Cursor preview expects exactly one cell.");
        }

        var worldPosition = MapToWorld.CellToWorld(buildResult.Intent.Cells.First());
        var kind = buildResult.Intent.Kind;
        var outcome = buildResult.Results.First().Outcome;

        switch (kind)
        {
            case BuildKind.Floor:
                ShowFloorCursorPreview(worldPosition, outcome);
                HideWallCursorPreview();
                break;
            case BuildKind.Wall:
                ShowWallCursorPreview(worldPosition, outcome);
                HideFloorCursorPreview();
                break;
            default:
                HideFloorCursorPreview();
                HideWallCursorPreview();
                break;
        }
    }

    /// <summary>
    /// Displays a visual preview of the floor cursor at the specified world position, updating its appearance based on
    /// the provided build operation outcome.
    /// </summary>
    /// <param name="worldPosition">The position in world coordinates where the floor cursor preview should be displayed.</param>
    /// <param name="outcome">The result of the build operation that determines the validity and appearance of the floor cursor preview.</param>
    private void ShowFloorCursorPreview(Vector3 worldPosition, BuildOperationOutcome outcome)
    {
        if (!FloorPreviewInstance.IsInsideTree())
        {
            return;
        }

        FloorPreviewInstance.SetValidity(outcome);
        FloorPreviewInstance.SetWorldPosition(worldPosition);
        FloorPreviewInstance.Show();
    }

    /// <summary>
    /// Displays a preview of the wall cursor at the specified world position, updating its validity state based on the
    /// provided build outcome.
    /// </summary>
    /// <param name="worldPosition">The position in world coordinates where the wall cursor preview should be displayed.</param>
    /// <param name="outcome">The result of the build operation that determines the validity state of the wall cursor preview.</param>
    private void ShowWallCursorPreview(Vector3 worldPosition, BuildOperationOutcome outcome)
    {
        if (!WallPreviewInstance.IsInsideTree())
        {
            return;
        }

        WallPreviewInstance.SetValidity(outcome);
        WallPreviewInstance.SetWorldPosition(worldPosition);
        WallPreviewInstance.Show();
    }

    /// <summary>
    /// Hides the currently displayed floor preview, if one is visible.
    /// </summary>
    private void HideFloorCursorPreview()
    {
        FloorPreviewInstance.Hide();
    }

    /// <summary>
    /// Hides the currently displayed wall preview, if visible.
    /// </summary>
    private void HideWallCursorPreview()
    {
        WallPreviewInstance.Hide();
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
        preview.Reset();
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
        preview.Reset();
        preview.Hide();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Retrieves the build operation result for the specified cell or throws an exception if the result is not found.
    /// </summary>
    /// <param name="results">A read-only dictionary that maps map cell coordinates to their corresponding build operation results.</param>
    /// <param name="cell">The coordinate of the map cell for which to retrieve the build operation result.</param>
    /// <returns>The build operation result associated with the specified cell.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified cell does not exist in the results dictionary.</exception>
    private BuildOperationResult GetResultOrThrow(IReadOnlyDictionary<MapCellCoord, BuildOperationResult> results, MapCellCoord cell)
    {
        if (!results.TryGetValue(cell, out var result))
        {
            throw new InvalidOperationException($"Missing BuildOperationResult for cell {cell}.");
        }

        return result;
    }

    #endregion
}