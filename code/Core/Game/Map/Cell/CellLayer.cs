using CosmosCasino.Core.Game.Buildables;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Owns the buildable contents of one map cell at one discrete elevation.
    /// </summary>
    internal sealed class CellLayer
    {
        #region Properties

        /// <summary>
        /// Gets the floor contained by this layer, or <see langword="null"/> when no floor exists.
        /// </summary>
        internal Floor? Floor { get; private set; }

        /// <summary>
        /// Gets the wall contained by this layer, or <see langword="null"/> when no wall exists.
        /// </summary>
        internal Wall? Wall { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this layer contains a floor.
        /// </summary>
        internal bool HasFloor => Floor is not null;

        /// <summary>
        /// Gets a value indicating whether this layer contains a wall.
        /// </summary>
        internal bool HasWall => Wall is not null;

        /// <summary>
        /// Gets a value indicating whether this layer contains no buildable objects.
        /// </summary>
        internal bool IsEmpty => !HasFloor && !HasWall;

        #endregion

        #region Floor Methods

        /// <summary>
        /// Places the specified floor in this layer.
        /// </summary>
        /// <param name="floor">The floor to place.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="floor"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when this layer already contains a floor.</exception>
        internal void PlaceFloor(Floor floor)
        {
            ArgumentNullException.ThrowIfNull(floor);

            if (Floor is not null)
            {
                throw new InvalidOperationException("Cannot place floor: this cell layer already contains a floor.");
            }

            Floor = floor;
        }

        /// <summary>
        /// Removes the floor from this layer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this layer has no floor or when a wall still depends on the floor.
        /// </exception>
        internal void RemoveFloor()
        {
            if (Floor is null)
            {
                throw new InvalidOperationException("Cannot remove floor: this cell layer does not contain a floor.");
            }

            if (HasWall)
            {
                throw new InvalidOperationException("Cannot remove floor: this cell layer contains a wall.");
            }

            Floor = null;
        }

        #endregion

        #region Wall Methods

        /// <summary>
        /// Places the specified wall in this layer.
        /// </summary>
        /// <param name="wall">The wall to place.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wall"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when this layer has no supporting floor or already contains a wall.
        /// </exception>
        internal void PlaceWall(Wall wall)
        {
            ArgumentNullException.ThrowIfNull(wall);

            if (!HasFloor)
            {
                throw new InvalidOperationException("Cannot place wall: this cell layer does not contain a floor.");
            }

            if (Wall is not null)
            {
                throw new InvalidOperationException("Cannot place wall: this cell layer already contains a wall.");
            }

            Wall = wall;
        }

        /// <summary>
        /// Removes the wall from this layer.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this layer has no wall.</exception>
        internal void RemoveWall()
        {
            if (Wall is null)
            {
                throw new InvalidOperationException("Cannot remove wall: this cell layer does not contain a wall.");
            }

            Wall = null;
        }

        #endregion
    }
}
