using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using System;

/// <summary>
/// Provides a base class for build context objects that encapsulate player-facing build interaction state.
/// </summary>
/// <remarks>
/// This class owns interaction capability and rotation behavior only. Domain-specific request construction is exposed
/// by specialized capability interfaces implemented by concrete contexts.
/// </remarks>
public abstract class BuildContextBase
{
    #region Capabilities

    /// <summary>
    /// Determines whether this context supports the specified player-facing build operation.
    /// </summary>
    /// <param name="buildOperation">The requested player-facing build operation.</param>
    /// <returns><c>true</c> when this context can handle the operation; otherwise, <c>false</c>.</returns>
    public virtual bool SupportsBuildOperation(BuildOperation buildOperation)
    {
        _ = buildOperation;
        return false;
    }

    #endregion

    #region Rotation

    /// <summary>
    /// Attempts to rotate this build context clockwise.
    /// </summary>
    /// <returns><c>true</c> when the context rotation changed; otherwise, <c>false</c>.</returns>
    public virtual bool TryRotateClockwise()
    {
        return false;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the next clockwise quarter-turn rotation.
    /// </summary>
    /// <param name="rotation">The current footprint rotation.</param>
    /// <returns>The next clockwise footprint rotation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the rotation is unsupported.</exception>
    protected static FootprintRotation GetNextClockwiseRotation(FootprintRotation rotation)
    {
        return rotation switch
        {
            FootprintRotation.Deg0 => FootprintRotation.Deg90,
            FootprintRotation.Deg90 => FootprintRotation.Deg180,
            FootprintRotation.Deg180 => FootprintRotation.Deg270,
            FootprintRotation.Deg270 => FootprintRotation.Deg0,
            _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, "Unsupported footprint rotation.")
        };
    }

    #endregion
}
