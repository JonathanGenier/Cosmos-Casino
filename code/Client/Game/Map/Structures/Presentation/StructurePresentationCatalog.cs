using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Client-owned catalog that maps authoritative structure definitions to presentation metadata.
/// </summary>
public sealed class StructurePresentationCatalog
{
    #region Fields

    private const int BlockPresentationKeyValue = 1;
    private const int PillarPresentationKeyValue = 2;
    private const int DoorPresentationKeyValue = 3;

    private readonly Dictionary<StructureDefinitionId, StructurePresentationDefinition> _definitions;

    #endregion

    #region Initialization

    private StructurePresentationCatalog(
        IReadOnlyList<StructurePresentationDefinition> definitions)
    {
        _definitions = new Dictionary<StructureDefinitionId, StructurePresentationDefinition>();

        foreach (StructurePresentationDefinition definition in definitions)
        {
            _definitions.Add(definition.DefinitionId, definition);
        }
    }

    #endregion

    #region Factory

    /// <summary>
    /// Creates the default presentation catalog for built-in Core structures.
    /// </summary>
    /// <returns>The default Client presentation catalog.</returns>
    internal static StructurePresentationCatalog CreateDefault()
    {
        return new StructurePresentationCatalog(new[]
        {
            CreateBlockPresentation(),
            CreatePillarPresentation(),
            CreateDoorPresentation()
        });
    }

    #endregion

    #region Query

    /// <summary>
    /// Attempts to get presentation metadata for a Core structure definition identity.
    /// </summary>
    /// <param name="definitionId">The authoritative structure definition identity.</param>
    /// <param name="definition">The presentation metadata when found.</param>
    /// <returns><c>true</c> when presentation metadata exists; otherwise, <c>false</c>.</returns>
    internal bool TryGetDefinition(
        StructureDefinitionId definitionId,
        out StructurePresentationDefinition definition)
    {
        return _definitions.TryGetValue(definitionId, out definition!);
    }

    #endregion

    #region Built-ins

    private static StructurePresentationDefinition CreateBlockPresentation()
    {
        return new StructurePresentationDefinition(
            StructureDefinitions.BlockDefinitionId,
            new StructurePresentationKey(BlockPresentationKeyValue),
            StructureRenderStrategy.GeneratedSectionMesh,
            mesh: null,
            material: null,
            scene: null,
            localBoundsSize: new Vector3(
                WorldGridMetrics.GridUnitSize,
                WorldGridMetrics.VerticalGridUnitSize,
                WorldGridMetrics.GridUnitSize),
            sceneLocalOffset: Vector3.Zero);
    }

    private static StructurePresentationDefinition CreatePillarPresentation()
    {
        var material = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.72f, 0.70f, 0.64f, 1f)
        };

        var boundsSize = new Vector3(
            WorldGridMetrics.GridUnitSize * 0.36f,
            WorldGridMetrics.VerticalGridUnitSize,
            WorldGridMetrics.GridUnitSize * 0.36f);

        return new StructurePresentationDefinition(
            StructureDefinitions.PillarDefinitionId,
            new StructurePresentationKey(PillarPresentationKeyValue),
            StructureRenderStrategy.MultiMesh,
            new BoxMesh
            {
                Size = boundsSize
            },
            material,
            scene: null,
            localBoundsSize: boundsSize,
            sceneLocalOffset: Vector3.Zero);
    }

    private static StructurePresentationDefinition CreateDoorPresentation()
    {
        return new StructurePresentationDefinition(
            StructureDefinitions.DoorDefinitionId,
            new StructurePresentationKey(DoorPresentationKeyValue),
            StructureRenderStrategy.Scene,
            mesh: null,
            material: null,
            scene: GD.Load<PackedScene>("res://scenes/game/structures/door.tscn"),
            localBoundsSize: new Vector3(
                WorldGridMetrics.GridUnitSize,
                WorldGridMetrics.VerticalGridUnitSize,
                WorldGridMetrics.GridUnitSize),
            sceneLocalOffset: new Vector3(
                WorldGridMetrics.GridUnitSize * 0.5f,
                WorldGridMetrics.VerticalGridUnitSize * 0.5f,
                0f));
    }

    #endregion
}
