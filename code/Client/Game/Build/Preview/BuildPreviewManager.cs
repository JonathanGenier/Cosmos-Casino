using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages generic structure footprint previews within the game world.
/// </summary>
/// <remarks>The manager displays Client-only preview cells from Core build evaluation data. Placement validity comes
/// from <see cref="BuildResult.Outcome"/>.</remarks>
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
    /// Displays a visual preview of the specified build result based on the current preview mode.
    /// </summary>
    /// <param name="buildResult">The Core build result to preview.</param>
    public void ShowPreview(BuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

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

    #endregion

    #region Preview

    private void ShowCursorPreview(BuildResult buildResult)
    {
        ShowStructurePreview(buildResult, _cursorPreviews);
    }

    private void ShowDragPreview(BuildResult buildResult)
    {
        ShowStructurePreview(buildResult, _dragPreviews);
    }

    private void ShowStructurePreview(
        BuildResult buildResult,
        List<StructurePreviewCell> activePreviews)
    {
        IReadOnlyList<MapCellCoord> cells = GetPreviewCells(buildResult);

        if (cells.Count == 0)
        {
            ClearPreviewCells(activePreviews);
            return;
        }

        RenderPreviewCells(activePreviews, cells, buildResult.Outcome);
    }

    private void RenderPreviewCells(
        List<StructurePreviewCell> activePreviews,
        IReadOnlyList<MapCellCoord> cells,
        BuildOperationOutcome outcome)
    {
        int i = 0;

        for (; i < cells.Count && i < activePreviews.Count; i++)
        {
            ShowPreviewCell(activePreviews[i], cells[i], outcome);
        }

        for (; i < cells.Count; i++)
        {
            StructurePreviewCell preview = StructurePreviewPool.Fetch();
            ShowPreviewCell(preview, cells[i], outcome);
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
        BuildOperationOutcome outcome)
    {
        preview.SetWorldPosition(cell.ToGodotCenter());
        preview.SetValidity(outcome);
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

    #region Helper Methods

    private IReadOnlyList<MapCellCoord> GetPreviewCells(BuildResult buildResult)
    {
        if (buildResult.Structures.Count > 0)
        {
            return buildResult.Structures
                .SelectMany(structure => structure.AffectedCells)
                .ToArray();
        }

        return buildResult.Intent.Operation switch
        {
            BuildOperation.Place => buildResult.Intent.PlacementRequests
                .SelectMany(GetPlacementPreviewCells)
                .ToArray(),
            BuildOperation.Remove => buildResult.Intent.RemovalRequests
                .Select(request => request.TargetCell)
                .ToArray(),
            _ => Array.Empty<MapCellCoord>()
        };
    }

    private IReadOnlyList<MapCellCoord> GetPlacementPreviewCells(StructurePlacementRequest request)
    {
        try
        {
            return request.Definition.Footprint.Resolve(request.Anchor, request.Rotation);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new[] { request.Anchor };
        }
    }

    #endregion
}
