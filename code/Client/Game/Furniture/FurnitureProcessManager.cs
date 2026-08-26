using CosmosCasino.Core.Game.Furniture;
using System;

/// <summary>
/// Client mutation adapter for authoritative Core furniture operations.
/// </summary>
public sealed partial class FurnitureProcessManager : InitializableNodeManager
{
    #region Fields

    private FurnitureManager? _furnitureManager;

    #endregion

    #region Events

    /// <summary>
    /// Occurs after a mutating furniture operation has completed in Core.
    /// </summary>
    public event Action<FurnitureOperationResult>? FurnitureCompleted;

    #endregion

    #region Properties

    private FurnitureManager FurnitureManager
    {
        get => _furnitureManager ?? throw new InvalidOperationException($"{nameof(FurnitureManager)} has not been initialized.");
        set => _furnitureManager = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the Client furniture process adapter.
    /// </summary>
    /// <param name="furnitureManager">The authoritative Core furniture manager.</param>
    public void Initialize(FurnitureManager furnitureManager)
    {
        ArgumentNullException.ThrowIfNull(furnitureManager);

        FurnitureManager = furnitureManager;
        MarkInitialized();
    }

    #endregion

    #region Evaluation

    /// <summary>
    /// Evaluates furniture placement without emitting a mutation-completed event.
    /// </summary>
    /// <param name="request">The authoritative furniture placement request.</param>
    /// <returns>The Core evaluation result.</returns>
    public FurnitureOperationResult EvaluatePlacement(FurniturePlacementRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ThrowIfUninitialized();

        return FurnitureManager.EvaluatePlacement(request);
    }

    /// <summary>
    /// Evaluates furniture removal without emitting a mutation-completed event.
    /// </summary>
    /// <param name="request">The authoritative furniture removal request.</param>
    /// <returns>The Core evaluation result.</returns>
    public FurnitureOperationResult EvaluateRemoval(FurnitureRemovalRequest request)
    {
        ThrowIfUninitialized();

        return FurnitureManager.EvaluateRemoval(request);
    }

    #endregion

    #region Mutation

    /// <summary>
    /// Places furniture through Core and emits the completed operation result.
    /// </summary>
    /// <param name="request">The authoritative furniture placement request.</param>
    /// <returns>The Core operation result.</returns>
    public FurnitureOperationResult Place(FurniturePlacementRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ThrowIfUninitialized();

        FurnitureOperationResult result = FurnitureManager.Place(request);
        FurnitureCompleted?.Invoke(result);
        return result;
    }

    /// <summary>
    /// Removes furniture through Core and emits the completed operation result.
    /// </summary>
    /// <param name="request">The authoritative furniture removal request.</param>
    /// <returns>The Core operation result.</returns>
    public FurnitureOperationResult Remove(FurnitureRemovalRequest request)
    {
        ThrowIfUninitialized();

        FurnitureOperationResult result = FurnitureManager.Remove(request);
        FurnitureCompleted?.Invoke(result);
        return result;
    }

    #endregion

    #region Helpers

    private void ThrowIfUninitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(FurnitureProcessManager)} is not initialized.");
        }
    }

    #endregion
}
