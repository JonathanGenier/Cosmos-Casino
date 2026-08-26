using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Client-owned catalog that maps authoritative furniture definitions to presentation metadata.
/// </summary>
public sealed class FurniturePresentationCatalog
{
    #region Fields

    private readonly Dictionary<FurnitureDefinitionId, FurniturePresentationDefinition> _definitions;

    #endregion

    #region Initialization

    private FurniturePresentationCatalog(IReadOnlyList<FurniturePresentationDefinition> definitions)
    {
        _definitions = new Dictionary<FurnitureDefinitionId, FurniturePresentationDefinition>();

        foreach (FurniturePresentationDefinition definition in definitions)
        {
            _definitions.Add(definition.DefinitionId, definition);
        }
    }

    #endregion

    #region Factory

    /// <summary>
    /// Creates the default presentation catalog for built-in Core furniture.
    /// </summary>
    /// <returns>The default Client furniture presentation catalog.</returns>
    internal static FurniturePresentationCatalog CreateDefault()
    {
        return new FurniturePresentationCatalog(new[]
        {
            CreateCasinoTablePresentation()
        });
    }

    #endregion

    #region Query

    /// <summary>
    /// Attempts to get presentation metadata for a Core furniture definition identity.
    /// </summary>
    /// <param name="definitionId">The authoritative furniture definition identity.</param>
    /// <param name="definition">The presentation metadata when found.</param>
    /// <returns><c>true</c> when presentation metadata exists; otherwise, <c>false</c>.</returns>
    internal bool TryGetDefinition(
        FurnitureDefinitionId definitionId,
        out FurniturePresentationDefinition definition)
    {
        return _definitions.TryGetValue(definitionId, out definition!);
    }

    #endregion

    #region Built-ins

    private static FurniturePresentationDefinition CreateCasinoTablePresentation()
    {
        return new FurniturePresentationDefinition(
            FurnitureDefinitions.CasinoTableDefinitionId,
            GD.Load<PackedScene>("res://scenes/game/furniture/casino_table.tscn"),
            new Vector3(
                WorldGridMetrics.GridUnitSize,
                0f,
                WorldGridMetrics.GridUnitSize * 0.5f));
    }

    #endregion
}
