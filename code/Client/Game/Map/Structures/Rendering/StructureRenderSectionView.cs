using Godot;
using System;

/// <summary>
/// Thin Client-owned view for one generated structure render section.
/// </summary>
internal sealed partial class StructureRenderSectionView : Node3D
{
    #region Fields

    private MeshInstance3D? _meshInstance;
    private bool _isInitialized;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the render section represented by this view.
    /// </summary>
    internal StructureRenderSectionCoord Coord { get; private set; }

    private MeshInstance3D MeshInstance
    {
        get => _meshInstance ?? throw new InvalidOperationException($"{nameof(StructureRenderSectionView)} has not been initialized.");
        set => _meshInstance = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the view with its section coordinate and shared material.
    /// </summary>
    /// <param name="coord">The render section coordinate.</param>
    /// <param name="material">The shared material used for generated Block geometry.</param>
    internal void Initialize(StructureRenderSectionCoord coord, Material material)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructureRenderSectionView)} is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(material);

        Coord = coord;
        Position = StructureRenderSectionMath.ToSectionWorldOrigin(coord);
        MeshInstance = new MeshInstance3D
        {
            Name = "StructureMesh",
            MaterialOverride = material
        };

        AddChild(MeshInstance);
        _isInitialized = true;
    }

    #endregion

    #region Representation

    /// <summary>
    /// Applies generated section geometry to this view's single mesh instance.
    /// </summary>
    /// <param name="mesh">The generated section mesh.</param>
    internal void ApplyMesh(ArrayMesh mesh)
    {
        ArgumentNullException.ThrowIfNull(mesh);

        MeshInstance.Mesh = mesh;
    }

    /// <summary>
    /// Clears the generated mesh reference from this view.
    /// </summary>
    internal void ClearMesh()
    {
        if (_meshInstance != null)
        {
            _meshInstance.Mesh = null;
        }
    }

    #endregion
}
