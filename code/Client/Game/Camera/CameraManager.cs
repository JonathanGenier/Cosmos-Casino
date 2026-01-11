using Godot;
using System;

/// <summary>
/// Manages camera input and control for a scene, coordinating camera movement, rotation, and zoom operations through a
/// camera rig.
/// </summary>
/// <remarks>Use this class to enable or disable camera controls and to direct camera actions such as movement,
/// rotation, and zoom. CameraManager is intended to be attached to a scene node and requires a CameraRig child node
/// named "Camera_Rig" as its parent. All camera input is routed through this manager, which can be programmatically
/// enabled or disabled as needed.</remarks>
public sealed partial class CameraManager : NodeManager
{
    #region Fields

    private CameraRig _cameraRig;
    private bool _isEnabled = true;

    #endregion

    #region Godot Methods

    /// <summary>
    /// Called when the node enters the scene tree for the first time. Initializes references to required child nodes.
    /// </summary>
    /// <remarks>This method is invoked automatically by the Godot engine when the node is added to the scene.
    /// It retrieves and validates the required CameraRig child node. If the CameraRig node is not found, an exception
    /// is thrown and the node will not function correctly.</remarks>
    public override void _Ready()
    {
        _cameraRig = GetParent().GetNode<CameraRig>("Camera_Rig");
        ArgumentNullException.ThrowIfNull(_cameraRig);
    }

    #endregion

    #region Enable Camera

    /// <summary>
    /// Enables or disables the camera controls.
    /// </summary>
    /// <param name="enabled">true to enable the camera controls; false to disable them and reset movement, rotation, and zoom.</param>
    public void SetCameraEnabled(bool enabled)
    {
        _isEnabled = enabled;

        if (!_isEnabled)
        {
            _cameraRig.SetMove(Vector2.Zero);
            _cameraRig.SetRotate(0f);
            _cameraRig.SetZoom(0f);
        }
    }

    #endregion 

    #region Camera Methods

    /// <summary>
    /// Moves the camera in the specified direction.
    /// </summary>
    /// <param name="direction">A <see cref="Vector2"/> representing the direction and magnitude of movement. The X and Y components correspond
    /// to horizontal and vertical movement, respectively.</param>
    public void Move(Vector2 direction)
    {
        if (!_isEnabled)
        {
            return;
        }

        _cameraRig.SetMove(direction);
    }

    /// <summary>
    /// Rotates the camera rig in the specified direction.
    /// </summary>
    /// <remarks>This method has no effect if the component is not enabled.</remarks>
    /// <param name="direction">The direction and magnitude of rotation to apply. Positive values rotate in one direction; negative values
    /// rotate in the opposite direction. The unit and scale depend on the camera rig's implementation.</param>
    public void Rotate(float direction)
    {
        if (!_isEnabled)
        {
            return;
        }

        _cameraRig.SetRotate(direction);
    }

    /// <summary>
    /// Adjusts the camera zoom level based on the specified direction value.
    /// </summary>
    /// <remarks>This method has no effect if zoom functionality is currently disabled.</remarks>
    /// <param name="direction">A value indicating the direction and magnitude of the zoom. Positive values zoom in; negative values zoom out.
    /// The effect of the value depends on the camera rig's configuration.</param>
    public void Zoom(float direction)
    {
        if (!_isEnabled)
        {
            return;
        }

        _cameraRig.SetZoom(direction);
    }

    #endregion
}
