using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages generic build footprint previews within the game world.
/// </summary>
/// <remarks>The manager displays Client-only preview cells from domain-specific preview data.</remarks>
public sealed partial class BuildPreviewManager : InitializableNodeManager
{
    #region Fields

    private const int DefaultStructurePreviewPoolSize = 128;

    private readonly List<StructurePreviewCell> _cursorPreviews = new();
    private readonly List<StructurePreviewCell> _dragPreviews = new();

    private ClientPool<StructurePreviewCell>? _structurePreviewPool;

    private BuildPreviewMode _currentMode;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current build preview mode in use.
    /// </summary>
    public BuildPreviewMode CurrentMode => _currentMode;

    private ClientPool<StructurePreviewCell> StructurePreviewPool
    {
        get => _structurePreviewPool ?? throw new InvalidOperationException($"{nameof(ClientPool<StructurePreviewCell>)} not initialized.");
        set => _structurePreviewPool = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component with the specified preview resources.
    /// </summary>
    /// <param name="previewResources">The preview resources supplied by the game scene.</param>
    public void Initialize(PreviewResources previewResources)
    {
        ArgumentNullException.ThrowIfNull(previewResources);

        StructurePreviewPool = new ClientPool<StructurePreviewCell>(
            DefaultStructurePreviewPoolSize,
            CreateStructurePreviewCell,
            ResetStructurePreviewCell);

        _currentMode = BuildPreviewMode.Cursor;
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Enables drag mode for the build preview.
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
    /// Exits drag mode and returns to cursor mode.
    /// </summary>
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
    /// Removes any active cursor preview.
    /// </summary>
    public void ClearCursorPreview()
    {
        ClearPreviewCells(_cursorPreviews);
    }

    /// <summary>
    /// Clears any active drag preview and resets the preview to cursor mode.
    /// </summary>
    public void ClearDragPreview()
    {
        ClearPreviewCells(_dragPreviews);
        _currentMode = BuildPreviewMode.Cursor;
    }

    /// <summary>
    /// Displays a visual preview of the specified build preview data based on the current preview mode.
    /// </summary>
    /// <param name="previewData">The Client-only preview data to render.</param>
    internal void ShowPreview(BuildPreviewData previewData)
    {
        ArgumentNullException.ThrowIfNull(previewData);

        switch (_currentMode)
        {
            case BuildPreviewMode.Cursor:
                ShowCursorPreview(previewData);
                break;

            case BuildPreviewMode.Drag:
                ShowDragPreview(previewData);
                break;
        }
    }

    #endregion

    #region Preview

    private void ShowCursorPreview(BuildPreviewData previewData)
    {
        ShowBuildPreview(previewData, _cursorPreviews);
    }

    private void ShowDragPreview(BuildPreviewData previewData)
    {
        ShowBuildPreview(previewData, _dragPreviews);
    }

    private void ShowBuildPreview(
        BuildPreviewData previewData,
        List<StructurePreviewCell> activePreviews)
    {
        if (previewData.Cells.Count == 0)
        {
            ClearPreviewCells(activePreviews);
            return;
        }

        RenderPreviewCells(
            activePreviews,
            previewData.Cells,
            previewData.Validity,
            previewData.CellGeometry);
    }

    private void RenderPreviewCells(
        List<StructurePreviewCell> activePreviews,
        IReadOnlyList<MapCellCoord> cells,
        BuildPreviewValidity validity,
        BuildPreviewCellGeometry cellGeometry)
    {
        int i = 0;

        for (; i < cells.Count && i < activePreviews.Count; i++)
        {
            ShowPreviewCell(activePreviews[i], cells[i], validity, cellGeometry);
        }

        for (; i < cells.Count; i++)
        {
            StructurePreviewCell preview = StructurePreviewPool.Fetch();
            ShowPreviewCell(preview, cells[i], validity, cellGeometry);
            activePreviews.Add(preview);
        }

        for (int j = cells.Count; j < activePreviews.Count; j++)
        {
            StructurePreviewPool.Return(activePreviews[j]);
        }

        if (activePreviews.Count > cells.Count)
        {
            activePreviews.RemoveRange(cells.Count, activePreviews.Count - cells.Count);
        }
    }

    private void ShowPreviewCell(
        StructurePreviewCell preview,
        MapCellCoord cell,
        BuildPreviewValidity validity,
        BuildPreviewCellGeometry cellGeometry)
    {
        preview.SetCellSize(GetPreviewCellSize(cellGeometry));
        preview.SetWorldPosition(GetPreviewCellCenter(cell, cellGeometry));
        preview.SetValidity(validity);
        preview.Show();
    }

    private void ClearPreviewCells(List<StructurePreviewCell> activePreviews)
    {
        foreach (StructurePreviewCell preview in activePreviews)
        {
            StructurePreviewPool.Return(preview);
        }

        activePreviews.Clear();
    }

    #endregion

    #region Pooling

    private StructurePreviewCell CreateStructurePreviewCell()
    {
        var preview = new StructurePreviewCell();
        preview.Initialize();
        AddChild(preview);
        preview.Hide();
        return preview;
    }

    private void ResetStructurePreviewCell(StructurePreviewCell preview)
    {
        preview.Reset();
    }

    #endregion

    #region Geometry

    private Vector3 GetPreviewCellCenter(
        MapCellCoord cell,
        BuildPreviewCellGeometry cellGeometry)
    {
        return cellGeometry switch
        {
            BuildPreviewCellGeometry.WorldGrid => cell.ToGodotCenter(),
            BuildPreviewCellGeometry.StructureGrid => StructureGridMetrics.ToGodotCenter(cell),
            _ => throw new ArgumentOutOfRangeException(nameof(cellGeometry), cellGeometry, "Unsupported preview cell geometry.")
        };
    }

    private Vector3 GetPreviewCellSize(BuildPreviewCellGeometry cellGeometry)
    {
        return cellGeometry switch
        {
            BuildPreviewCellGeometry.WorldGrid => new Vector3(
                WorldGridMetrics.GridUnitSize,
                WorldGridMetrics.VerticalGridUnitSize,
                WorldGridMetrics.GridUnitSize),
            BuildPreviewCellGeometry.StructureGrid => StructureGridMetrics.CellBoundsSize,
            _ => throw new ArgumentOutOfRangeException(nameof(cellGeometry), cellGeometry, "Unsupported preview cell geometry.")
        };
    }

    #endregion
}
