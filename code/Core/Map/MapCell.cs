namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Represents a single logical cell in the map grid.
    /// A cell may contain a floor, a structure, or furniture,
    /// with enforced rules on which elements may coexist.
    /// </summary>
    internal sealed class MapCell
    {
        #region PROPERTIES

        /// <summary>
        /// The floor type assigned to this cell, if any.
        /// </summary>
        internal FloorType? Floor { get; private set; }

        /// <summary>
        /// The structure occupying this cell, if any.
        /// </summary>
        internal StructureType? Structure { get; private set; }

        /// <summary>
        /// The furniture occupying this cell, if any.
        /// </summary>
        internal FurnitureType? Furniture { get; private set; }

        /// <summary>
        /// Indicates whether this cell currently has a floor.
        /// </summary>
        internal bool HasFloor => Floor != null;

        /// <summary>
        /// Indicates whether this cell currently has a structure.
        /// </summary>
        internal bool HasStructure => Structure != null;

        /// <summary>
        /// Indicates whether this cell currently has furniture.
        /// </summary>
        internal bool HasFurniture => Furniture != null;

        /// <summary>
        /// Indicates whether the cell contains no floor, structure, or furniture.
        /// </summary>
        internal bool IsEmpty => !HasFloor && !HasStructure && !HasFurniture;

        #endregion

        #region METHODS

        // ===================================================================================
        // FLOOR

        /// <summary>
        /// Determines whether the floor can be removed from this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has a floor and contains no structure
        /// and no furniture; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveFloor()
        {
            return HasFloor && !HasStructure && !HasFurniture;
        }

        /// <summary>
        /// Attempts to set or replace the floor type for this cell.
        /// </summary>
        /// <param name="floor">
        /// The floor type to assign to the cell.
        /// </param>
        /// <returns>
        /// <c>true</c> if the floor was set or replaced;
        /// <c>false</c> if the specified floor type was already present.
        /// </returns>
        internal bool TrySetFloor(FloorType floor)
        {
            if (Floor == floor)
            {
                return false;
            }

            Floor = floor;
            return true;
        }

        /// <summary>
        /// Attempts to remove the floor from this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the floor was removed;
        /// <c>false</c> if the floor cannot be removed due to existing contents.
        /// </returns>
        internal bool TryRemoveFloor()
        {
            if (!CanRemoveFloor())
            {
                return false;
            }

            Floor = null;
            return true;
        }

        // ===================================================================================
        // STRUCTURE

        /// <summary>
        /// Determines whether a structure may be placed in this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has a floor and contains no structure
        /// and no furniture; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanPlaceStructure()
        {
            return HasFloor && !HasStructure && !HasFurniture;
        }

        /// <summary>
        /// Determines whether the existing structure in this cell may be replaced.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has a floor, has a structure,
        /// and contains no furniture; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanReplaceStructure()
        {
            return HasFloor && HasStructure && !HasFurniture;
        }

        /// <summary>
        /// Determines whether the structure in this cell may be removed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has a structure; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveStructure()
        {
            return HasStructure;
        }

        /// <summary>
        /// Attempts to place a structure in this cell.
        /// </summary>
        /// <param name="structure">
        /// The structure type to place.
        /// </param>
        /// <returns>
        /// <c>true</c> if the structure was placed;
        /// <c>false</c> if placement conditions were not met.
        /// </returns>
        internal bool TryPlaceStructure(StructureType structure)
        {
            if (!CanPlaceStructure())
            {
                return false;
            }

            Structure = structure;
            return true;
        }

        /// <summary>
        /// Attempts to replace the existing structure with a new structure type.
        /// </summary>
        /// <param name="newStructure">
        /// The new structure type to assign.
        /// </param>
        /// <returns>
        /// <c>true</c> if the structure was replaced;
        /// <c>false</c> if replacement conditions were not met or the
        /// structure type is unchanged.
        /// </returns>
        internal bool TryReplaceStructure(StructureType newStructure)
        {
            if (!CanReplaceStructure())
            {
                return false;
            }

            if (Structure == newStructure)
            {
                return false;
            }

            Structure = newStructure;
            return true;
        }

        /// <summary>
        /// Attempts to remove the structure from this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the structure was removed;
        /// <c>false</c> if no structure exists.
        /// </returns>
        internal bool TryRemoveStructure()
        {
            if (!CanRemoveStructure())
            {
                return false;
            }

            Structure = null;
            return true;
        }

        // ===================================================================================
        // FURNITURE

        /// <summary>
        /// Determines whether furniture may be placed in this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has a floor and contains no structure
        /// and no furniture; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanPlaceFurniture()
        {
            return HasFloor && !HasStructure && !HasFurniture;
        }

        /// <summary>
        /// Determines whether furniture may be removed from this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the cell has furniture; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanRemoveFurniture()
        {
            return HasFurniture;
        }

        /// <summary>
        /// Attempts to place furniture in this cell.
        /// </summary>
        /// <param name="newFurniture">
        /// The furniture type to place.
        /// </param>
        /// <returns>
        /// <c>true</c> if the furniture was placed;
        /// <c>false</c> if placement conditions were not met.
        /// </returns>
        internal bool TryPlaceFurniture(FurnitureType newFurniture)
        {
            if (!CanPlaceFurniture())
            {
                return false;
            }

            Furniture = newFurniture;
            return true;
        }

        /// <summary>
        /// Attempts to remove the furniture from this cell.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the furniture was removed;
        /// <c>false</c> if no furniture exists.
        /// </returns>
        internal bool TryRemoveFurniture()
        {
            if (!CanRemoveFurniture())
            {
                return false;
            }

            Furniture = null;
            return true;
        }

        #endregion
    }
}
