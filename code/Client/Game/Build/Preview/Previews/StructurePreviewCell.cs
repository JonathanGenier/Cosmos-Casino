using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Client-only translucent cell volume used to preview structure footprints.
/// </summary>
public sealed partial class StructurePreviewCell : Node3D
{
    #region Fields

    private StandardMaterial3D? _material;
    private bool _isInitialized;

    #endregion

    #region Public API

    /// <summary>
    /// Sets the preview cell's world position.
    /// </summary>
    /// <param name="worldPosition">The target world position.</param>
    public void SetWorldPosition(Vector3 worldPosition)
    {
        GlobalPosition = worldPosition;
    }

    /// <summary>
    /// Resets the preview cell for pooled reuse.
    /// </summary>
    public void Reset()
    {
        SetValidity(BuildPreviewValidity.NoOp);
        Position = Vector3.Zero;
        Hide();
    }

    /// <summary>
    /// Sets the preview cell color from a Client preview validity state.
    /// </summary>
    /// <param name="validity">The Client preview validity state.</param>
    /// <exception cref="InvalidOperationException">Thrown if the preview cell has not been initialized.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the validity state is unsupported.</exception>
    internal void SetValidity(BuildPreviewValidity validity)
    {
        if (_material == null)
        {
            throw new InvalidOperationException($"{nameof(StructurePreviewCell)} has not been initialized.");
        }

        _material.AlbedoColor = validity switch
        {
            BuildPreviewValidity.Valid => PreviewColors.ValidColor,
            BuildPreviewValidity.NoOp => PreviewColors.NoOpColor,
            BuildPreviewValidity.Invalid => PreviewColors.InvalidColor,
            _ => throw new ArgumentOutOfRangeException(nameof(validity), validity, "Unsupported preview validity.")
        };
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes this preview cell's disposable visual representation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the preview cell has already been initialized.</exception>
    internal void Initialize()
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException($"{nameof(StructurePreviewCell)} is already initialized.");
        }

        var material = new StandardMaterial3D
        {
            AlbedoColor = PreviewColors.NoOpColor,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        };

        var mesh = new MeshInstance3D
        {
            Mesh = new BoxMesh
            {
                Size = new Vector3(
                    WorldGridMetrics.GridUnitSize,
                    WorldGridMetrics.VerticalGridUnitSize,
                    WorldGridMetrics.GridUnitSize)
            },
            MaterialOverride = material
        };

        AddChild(mesh);
        _material = material;
        _isInitialized = true;
    }

    #endregion
}
