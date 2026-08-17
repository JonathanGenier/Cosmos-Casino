using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Represents a 3D preview node for visualizing wall placement within a scene.
/// </summary>
/// <remarks>Use this class to display a visual representation of a wall before it is placed in the environment.
/// Placement coordinates are resolved before being passed into this view.</remarks>
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

        Scale = new Vector3(WorldGridMetrics.GridUnitSize, Scale.Y, WorldGridMetrics.GridUnitSize);

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
                "No ShaderMaterial found on WallPreview mesh."
            );
        }

        mesh.MaterialOverride = material;
        _material = material;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Sets the object's global position using a world position resolved by the authoritative map conversion layer.
    /// </summary>
    /// <remarks>This method has no effect if the object is not currently part of the scene tree.</remarks>
    /// <param name="worldPosition">The target world position to set, in global coordinates.</param>
    public void SetWorldPosition(Vector3 worldPosition)
    {
        if (!IsInsideTree())
        {
            return;
        }

        GlobalPosition = worldPosition;
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
            throw new InvalidOperationException("WallPreview requires a ShaderMaterial on surface 0.");
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
            throw new InvalidOperationException("WallPreview requires a ShaderMaterial on surface 0.");
        }

        _material.SetShaderParameter("color", PreviewColors.NoOpColor);
    }

    #endregion
}
