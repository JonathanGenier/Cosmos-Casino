using CosmosCasino.Core.Game.Build.Domain;
using Godot;
using System;

/// <summary>
/// Represents a 3D preview node for visualizing wall placement within a scene.
/// </summary>
/// <remarks>Use this class to display a visual representation of a wall before it is placed in the environment.
/// The preview automatically snaps to a grid for precise alignment, aiding users in positioning walls accurately during
/// level editing or construction workflows.</remarks>
public sealed partial class WallPreview : Node3D
{
    #region Fields

    private MeshInstance3D? _meshInstance;
    private ShaderMaterial? _material;

    #endregion

    #region Godot Processes

    /// <summary>
    /// Initializes the node when it enters the scene tree and prepares the grid preview by retrieving required
    /// resources.
    /// </summary>
    /// <remarks>This method is called by the Godot engine when the node is added to the scene tree. It
    /// ensures that the grid preview is set up with the correct shader material. If the expected material is not
    /// present, the method will throw an exception to indicate misconfiguration.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the surface 0 material of the MeshInstance3D node is not a ShaderMaterial.</exception>
    public override void _Ready()
    {
        var mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D")
        ?? throw new InvalidOperationException("MeshInstance3D not found.");

        ShaderMaterial material;

        if (mesh.MaterialOverride is ShaderMaterial overrideMat)
        {
            material = (ShaderMaterial)overrideMat.Duplicate();
        }
        else if (mesh.Mesh != null && mesh.GetSurfaceOverrideMaterial(0) is ShaderMaterial surfaceMat)
        {
            material = (ShaderMaterial)surfaceMat.Duplicate();
        }
        else
        {
            throw new InvalidOperationException(
                "No ShaderMaterial found on FloorPreview mesh."
            );
        }

        mesh.MaterialOverride = material;
        _material = material;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Sets the object's world position to the specified coordinates, snapping the position to the nearest grid point.
    /// </summary>
    /// <remarks>This method has no effect if the object is not currently part of the scene tree.</remarks>
    /// <param name="worldPosition">The target world position to set, in global coordinates. The position will be adjusted to align with the grid.</param>
    public void SetWorldPosition(Vector3 worldPosition)
    {
        if (!IsInsideTree())
        {
            return;
        }

        GlobalPosition = SnapToGrid(worldPosition);
    }

    /// <summary>
    /// Sets the preview color based on the specified build operation outcome.
    /// </summary>
    /// <param name="outcome">The outcome of the build operation that determines which preview color to apply.</param>
    /// <exception cref="InvalidOperationException">Thrown if the preview surface does not have an associated ShaderMaterial.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified outcome is not a valid value of the BuildOperationOutcome enumeration.</exception>
    public void SetValidity(BuildOperationOutcome outcome)
    {
        if (_material == null)
        {
            throw new InvalidOperationException("FloorPreview requires a ShaderMaterial on surface 0.");
        }

        var color = outcome switch
        {
            BuildOperationOutcome.Valid => PreviewColors.ValidColor,
            BuildOperationOutcome.NoOp => PreviewColors.NoOpColor,
            BuildOperationOutcome.Invalid => PreviewColors.InvalidColor,
            _ => throw new ArgumentOutOfRangeException(nameof(outcome))
        };

        _material.SetShaderParameter("color", color);
    }

    /// <summary>
    /// Resets the preview to its default color state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the required ShaderMaterial is not assigned to surface 0.</exception>
    public void Reset()
    {
        if (_material == null)
        {
            throw new InvalidOperationException("FloorPreview requires a ShaderMaterial on surface 0.");
        }

        _material.SetShaderParameter("color", PreviewColors.NoOpColor);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Snaps the specified world position to the center of the nearest grid cell on the XZ plane.
    /// </summary>
    /// <remarks>This method is useful for aligning objects to a grid in 3D space, such as in level editors or
    /// grid-based games. The grid cells are assumed to be 1 unit in size.</remarks>
    /// <param name="worldPos">The world position to be snapped to the grid. The Y component is preserved.</param>
    /// <returns>A <see cref="Vector3"/> representing the position aligned to the center of the nearest grid cell on the XZ
    /// plane, with the original Y value.</returns>
    private static Vector3 SnapToGrid(Vector3 worldPos)
    {
        return new Vector3(
            Mathf.Floor(worldPos.X) + 0.5f,
            worldPos.Y,
            Mathf.Floor(worldPos.Z) + 0.5f
        );
    }

    #endregion
}
