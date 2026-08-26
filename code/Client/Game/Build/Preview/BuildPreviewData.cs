using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Client-only presentation data used to render grid footprint previews.
/// </summary>
internal sealed class BuildPreviewData
{
    #region Initialization

    /// <summary>
    /// Initializes new build preview data.
    /// </summary>
    /// <param name="cells">The authoritative map cells to visualize.</param>
    /// <param name="validity">The visual validity state to apply to every preview cell.</param>
    internal BuildPreviewData(
        IReadOnlyList<MapCellCoord> cells,
        BuildPreviewValidity validity)
    {
        ArgumentNullException.ThrowIfNull(cells);

        Cells = cells.ToArray();
        Validity = validity;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the authoritative map cells to visualize.
    /// </summary>
    internal IReadOnlyList<MapCellCoord> Cells { get; }

    /// <summary>
    /// Gets the visual validity state to apply to every preview cell.
    /// </summary>
    internal BuildPreviewValidity Validity { get; }

    #endregion
}
