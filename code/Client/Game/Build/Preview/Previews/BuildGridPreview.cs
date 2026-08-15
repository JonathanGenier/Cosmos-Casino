using CosmosCasino.Core.Game.Map;
using Godot;
using System;

/// <summary>
/// Provides a 3D grid preview node for visualizing and interacting with grid-based placement in the scene. Supports
/// configurable radius, cell size, and tile diameter for flexible grid visualization.
/// </summary>
/// <remarks>This class is intended for use in editor or runtime scenarios where a visual grid overlay is needed,
/// such as tile-based level design or placement previews. The grid preview automatically snaps its position to the
/// nearest grid cell and updates its appearance based on the configured parameters. The node requires a MeshInstance3D
/// child with a ShaderMaterial assigned to surface 0 for correct rendering. Exceptions may be thrown if the required
/// material is not present or if invalid parameters are provided.</remarks>
public sealed partial class BuildGridPreview : Node3D
{
    #region Fields

    /// <summary>
    /// Represents the default diameter, in tiles, used for grid preview.
    /// </summary>
    public const int DefaultTileDiameter = 15;

    /// <summary>
    /// Represents the minimum allowed radius, in tile, for grid preview.
    /// </summary>
    public const int MinTileDiameter = 9;

    /// <summary>
    /// Represents the maximum allowed radius, in tile, for grid preview.
    /// </summary>
    public const int MaxTileDiameter = 39;

    /// <summary>
    /// Specifies the step size, in tiles, used when resizing the radius of the grid preview.
    /// </summary>
    public const int DiameterResizeTileStep = 2;

    [Export]
    private float _radius = 0.0f;

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
        _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");

        var material = _meshInstance.GetSurfaceOverrideMaterial(0);

        if (material is not ShaderMaterial shader)
        {
            throw new InvalidOperationException("BuildGridPreview requires a ShaderMaterial on surface 0.");
        }

        _material = shader;

        ApplyStaticShaderParams();
        SetTileDiameter(DefaultTileDiameter);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Updates the object's global position to the center of the map cell containing the specified world position.
    /// </summary>
    /// <remarks>The cell conversion is resolved by Core map math; this view only translates the result to Godot.</remarks>
    /// <param name="worldPosition">The target position in world coordinates to which the object's global position will be updated.</param>
    public void UpdatePosition(Vector3 worldPosition)
    {
        MapCoord cell = MapMath.WorldToCell(worldPosition.ToWorldCoord());
        GlobalPosition = MapMath.CellToWorldCenter(cell).ToGodotVector3(worldPosition.Y);
    }

    /// <summary>
    /// Sets the tile diameter by specifying the number of tiles across the diameter. The diameter must be an odd
    /// integer greater than or equal to 1.
    /// </summary>
    /// <param name="tileCount">The number of tiles to use for the diameter. Must be an odd integer greater than or equal to 1.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="tileCount"/> is less than 1 or is not an odd number.</exception>
    public void SetTileDiameter(int tileCount)
    {
        if (tileCount < 1 || tileCount % 2 == 0)
        {
            throw new ArgumentException("Tile diameter must be an odd number >= 1.");
        }

        float radius = tileCount * 0.5f;
        SetRadius(radius);
    }

    #endregion

    #region Internal

    /// <summary>
    /// Applies static shader parameters to the associated material instance.
    /// </summary>
    /// <remarks>This method sets the 'radius' and 'cell_size' parameters on the material based on the current
    /// values of the corresponding fields. It should be called whenever these values change to ensure the material's
    /// shader remains in sync with the object's state.</remarks>
    private void ApplyStaticShaderParams()
    {
        _material!.SetShaderParameter("radius", _radius);
        UpdateScale();
    }

    /// <summary>
    /// Sets the radius value used for rendering and updates the object's scale accordingly.
    /// </summary>
    /// <remarks>If the specified radius is less than 0.1, the radius will be set to 0.1 to ensure a minimum
    /// size for rendering.</remarks>
    /// <param name="radius">The new radius to apply. Must be greater than or equal to 0.1.</param>
    private void SetRadius(float radius)
    {
        _radius = Mathf.Max(0.1f, radius);
        _material?.SetShaderParameter("radius", _radius);
        UpdateScale();
    }

    /// <summary>
    /// Updates the object's scale to match the current radius, ensuring the mesh size reflects the intended dimensions.
    /// </summary>
    /// <remarks>This method should be called whenever the radius changes to keep the object's visual
    /// representation consistent. The scale is set so that the mesh spans twice the radius along the X and Z
    /// axes.</remarks>
    private void UpdateScale()
    {
        // PlaneMesh is 1x1 by default
        Scale = new Vector3(_radius * 2f, 1f, _radius * 2f);
    }

    #endregion
}
