using CosmosCasino.Core.Game.Structures;
using Godot;
using System;

/// <summary>
/// Manages the user interface for structure build selection and build cancellation within the game scene.
/// </summary>
/// <remarks>BuildUiManager coordinates the build UI lifecycle and exposes events for user intent, allowing other
/// components to respond to build-related actions. It is responsible for initializing the build UI, handling user input
/// for build operations, and ensuring proper event wiring when entering or exiting the scene tree. This class does not
/// perform build logic directly; it serves as an intermediary between the UI and the build system.</remarks>
public sealed partial class BuildUiManager : InitializableNodeManager
{
    #region Fields

    private BuildUi? _buildUi;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a structure definition is selected.
    /// </summary>
    /// <remarks>Subscribers are notified with the selected structure definition when the event is raised.</remarks>
    public event Action<StructureDefinition>? StructureDefinitionSelected;

    /// <summary>
    /// Raised when the player cancels the current build selection.
    /// Consumers are expected to clear build context and restore
    /// default interaction state.
    /// </summary>
    public event Action? BuildCancelled;

    #endregion

    #region Properties

    private BuildUi BuildUi
    {
        get => _buildUi ?? throw new InvalidOperationException($"{nameof(BuildUi)} has not been initialized.");
        set => _buildUi = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component and prepares it for use.
    /// </summary>
    public void Initialize()
    {
        MarkInitialized();
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Instantiates the build UI scene and wires UI intent events when
    /// this manager enters the scene tree.
    /// </summary>
    protected override void OnReady()
    {
        BuildUi = AddNode(GD.Load<PackedScene>(GameUiPaths.Build).Instantiate<BuildUi>());
        BuildUi.StructureDefinitionSelected += OnStructureDefinitionSelected;
        BuildUi.BuildCancelled += OnBuildCancelled;
    }

    /// <summary>
    /// Disconnects UI intent event bindings when this manager is removed
    /// from the scene tree to prevent dangling callbacks.
    /// </summary>
    protected override void OnExit()
    {
        BuildUi.StructureDefinitionSelected -= OnStructureDefinitionSelected;
        BuildUi.BuildCancelled -= OnBuildCancelled;
    }

    #endregion

    #region BuildUi Callbacks

    private void OnStructureDefinitionSelected(StructureDefinition definition)
    {
        StructureDefinitionSelected?.Invoke(definition);
    }

    /// <summary>
    /// Handles build cancellation intent emitted by the build UI by
    /// clearing the active build context and restoring the default
    /// interaction tool.
    /// </summary>
    private void OnBuildCancelled()
    {
        BuildCancelled?.Invoke();
    }

    #endregion
}
