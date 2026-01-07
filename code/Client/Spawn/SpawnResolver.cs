using CosmosCasino.Core.Floor;
using CosmosCasino.Core.Map;
using System;

/// <summary>
/// Resolves logical spawn keys and variants into concrete spawn identifiers
/// used to look up spawnable scenes.
/// </summary>
public static class SpawnResolver
{
    #region Resolver

    /// <summary>
    /// Resolves a spawn identifier for the specified spawn key and variant.
    /// </summary>
    /// <param name="key">
    /// Logical spawn key identifying where and in which slot the visual should appear.
    /// </param>
    /// <param name="variant">
    /// Variant describing which concrete visual should be spawned.
    /// </param>
    /// <returns>
    /// String identifier used to resolve a spawnable scene.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the spawn key type is not supported.
    /// </exception>
    public static string Resolve(ISpawnKey key, ISpawnVariant variant)
    {
        return key switch
        {
            CellSlotSpawnKey cellKey => ResolveCellSlot(cellKey, variant),
            _ => throw new InvalidOperationException(
                $"Unsupported spawn key type: {key.GetType().Name}")
        };
    }

    #endregion

    #region Resolve Cell Slots

    /// <summary>
    /// Resolves a spawn identifier for a cell slot spawn key.
    /// </summary>
    /// <param name="key">
    /// Cell slot spawn key describing the target map cell and slot.
    /// </param>
    /// <param name="variant">
    /// Variant describing which visual should be spawned in the slot.
    /// </param>
    /// <returns>
    /// String identifier used to resolve a spawnable scene.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the cell slot type is not supported.
    /// </exception>
    private static string ResolveCellSlot(CellSlotSpawnKey key, ISpawnVariant variant)
    {
        return key.Slot switch
        {
            MapCellSlot.Floor => ResolveFloorType(variant),
            _ => throw new InvalidOperationException(
                $"Unsupported cell slot: {key.Slot}")
        };
    }

    #endregion

    #region Resolve Floor Types

    /// <summary>
    /// Resolves a spawn identifier for a floor variant.
    /// </summary>
    /// <param name="variant">
    /// Variant describing the floor type to spawn.
    /// </param>
    /// <returns>
    /// String identifier used to resolve a floor spawnable scene.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the variant type or floor type is not supported.
    /// </exception>
    private static string ResolveFloorType(ISpawnVariant variant)
    {
        if (variant is not FloorSpawnVariant floorVariant)
        {
            throw new InvalidOperationException(
                $"Unsupported floor variant: {variant.GetType().Name}");
        }

        return floorVariant.FloorType switch
        {
            FloorType.Metal => "floor_metal",
            FloorType.Carbon => "floor_carbon",
            _ => throw new InvalidOperationException(
                $"Unsupported floor type: {floorVariant.FloorType}")
        };
    }

    #endregion
}