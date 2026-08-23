using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// UI controller responsible for presenting build selection controls
/// to the player during build mode.
/// <see cref="BuildUi"/> is a passive view component: it owns UI widgets,
/// listens to user interaction events, and emits high-level build intent
/// signals without directly modifying game state or interaction tools.
/// This class does not interpret gameplay meaning, apply builds, or
/// coordinate input routing. Those responsibilities are delegated to
/// higher-level UI managers and client-side build systems.
/// </summary>
public sealed partial class BuildUi : Control
{
    #region Inspector Nodes

    /// <summary>
    /// Button used to cancel the current build selection and exit build mode.
    /// </summary>
    [Export]
    private Button? _cancelButton;

    [Export]
    private Button? _blockButton;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a structure definition is selected.
    /// </summary>
    public event Action<StructureDefinition>? StructureDefinitionSelected;

    /// <summary>
    /// Raised when the player cancels the current build selection.
    /// Consumers are expected to clear build context and restore
    /// default interaction state.
    /// </summary>
    public event Action? BuildCancelled;

    #endregion

    #region Properties

    private Button CancelButton
    {
        get => _cancelButton ?? throw new InvalidOperationException("CancelButton not assigned.");
        set => _cancelButton = value;
    }

    private Button BlockButton
    {
        get => _blockButton ?? throw new InvalidOperationException("BlockButton not assigned.");
        set => _blockButton = value;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Connects UI input signals when the control enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        BlockButton.Toggled += OnBlockToggled;
        CancelButton.Pressed += ClearToggles;
    }

    /// <summary>
    /// Disconnects UI input signals when the control is removed from
    /// the scene tree to prevent dangling callbacks.
    /// </summary>
    public override void _ExitTree()
    {
        BlockButton.Toggled -= OnBlockToggled;
        CancelButton.Pressed -= ClearToggles;
    }

    #endregion

    #region Buttons Methods

    /// <summary>
    /// Clears all build selection toggles and signals that the current
    /// build operation has been cancelled.
    /// </summary>
    private void ClearToggles()
    {
        BlockButton.SetPressedNoSignal(false);
        BuildCancelled?.Invoke();
    }

    /// <summary>
    /// Handles toggle state changes for the structural block button.
    /// </summary>
    /// <param name="toggled">
    /// Whether the button was toggled on.
    /// </param>
    private void OnBlockToggled(bool toggled)
    {
        if (toggled)
        {
            SelectStructureDefinition(StructureDefinitions.Block);
        }
    }

    #endregion

    #region Selection

    private void SelectStructureDefinition(StructureDefinition definition)
    {
        BlockButton.SetPressedNoSignal(definition.Id == StructureDefinitions.BlockDefinitionId);

        StructureDefinitionSelected?.Invoke(definition);
    }

    #endregion
}
