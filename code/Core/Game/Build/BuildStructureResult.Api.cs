using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable structure-level effect planned or committed by a build intent.
    /// </summary>
    public sealed class BuildStructureResult
    {
        #region Initialization

        /// <summary>
        /// Initializes a new structure-level build result.
        /// </summary>
        /// <param name="kind">Whether the effect creates or removes a structure.</param>
        /// <param name="outcome">The outcome for this structure effect.</param>
        /// <param name="structureId">The authoritative structure identity.</param>
        /// <param name="definitionId">The stable structure definition identity.</param>
        /// <param name="anchor">The authoritative structure anchor.</param>
        /// <param name="rotation">The structure footprint rotation.</param>
        /// <param name="affectedCells">Every affected map cell in deterministic order.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="affectedCells"/> is null.</exception>
        internal BuildStructureResult(
            BuildStructureResultKind kind,
            BuildOperationOutcome outcome,
            StructureId structureId,
            StructureDefinitionId definitionId,
            MapCellCoord anchor,
            FootprintRotation rotation,
            IReadOnlyList<MapCellCoord> affectedCells)
        {
            ArgumentNullException.ThrowIfNull(affectedCells);

            Kind = kind;
            Outcome = outcome;
            StructureId = structureId;
            DefinitionId = definitionId;
            Anchor = anchor;
            Rotation = rotation;
            AffectedCells = affectedCells.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether this result represents creation or removal.
        /// </summary>
        public BuildStructureResultKind Kind { get; }

        /// <summary>
        /// Gets the structure-level outcome for this effect.
        /// </summary>
        public BuildOperationOutcome Outcome { get; }

        /// <summary>
        /// Gets the authoritative structure identity.
        /// </summary>
        public StructureId StructureId { get; }

        /// <summary>
        /// Gets the stable structure definition identity.
        /// </summary>
        public StructureDefinitionId DefinitionId { get; }

        /// <summary>
        /// Gets the authoritative structure anchor.
        /// </summary>
        public MapCellCoord Anchor { get; }

        /// <summary>
        /// Gets the structure footprint rotation.
        /// </summary>
        public FootprintRotation Rotation { get; }

        /// <summary>
        /// Gets every affected map cell in deterministic footprint order.
        /// </summary>
        public IReadOnlyList<MapCellCoord> AffectedCells { get; }

        #endregion
    }
}
