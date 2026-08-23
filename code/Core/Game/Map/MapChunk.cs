using CosmosCasino.Core.Game.Map.Terrain.Tile;
using System.Diagnostics.CodeAnalysis;

namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Authoritative spatial aggregate for one horizontal X/Z map-chunk region.
    /// </summary>
    internal sealed class MapChunk
    {
        #region Fields

        private readonly Dictionary<MapChunkCellKey, Cell> _cells = new();
        private readonly TerrainTile?[] _terrainTiles;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new <see cref="MapChunk"/> with the specified chunk coordinate.
        /// </summary>
        /// <param name="coord">The global X/Z chunk coordinate identifying this map chunk.</param>
        internal MapChunk(MapChunkCoord coord)
        {
            Coord = coord;
            _terrainTiles = new TerrainTile[MapChunkMetrics.ChunkSize * MapChunkMetrics.ChunkSize];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this chunk's global X/Z identity.
        /// </summary>
        internal MapChunkCoord Coord { get; }

        /// <summary>
        /// Gets the authoritative chunk size used by this chunk.
        /// </summary>
        internal int ChunkSize => MapChunkMetrics.ChunkSize;

        /// <summary>
        /// Gets the number of sparse map cells currently stored in this chunk.
        /// </summary>
        internal int CellCount => _cells.Count;

        /// <summary>
        /// Gets the number of occupied terrain slots in this chunk.
        /// </summary>
        internal int TerrainTileCount { get; private set; }

        #endregion

        #region Spatial Queries

        /// <summary>
        /// Determines whether the specified global cell coordinate belongs to this chunk's X/Z region.
        /// </summary>
        /// <param name="cellCoord">The global logical cell coordinate to query.</param>
        /// <returns><c>true</c> if <paramref name="cellCoord"/> belongs to this chunk; otherwise, <c>false</c>.</returns>
        internal bool Contains(MapCellCoord cellCoord)
        {
            return MapMath.CellToChunk(cellCoord) == Coord;
        }

        #endregion

        #region Cell Storage

        /// <summary>
        /// Attempts to retrieve a sparse map cell from this chunk.
        /// </summary>
        /// <param name="local">The zero-based X/Z coordinate inside this map chunk.</param>
        /// <param name="y">The global vertical Y coordinate of the cell.</param>
        /// <param name="cell">The stored cell if one exists.</param>
        /// <returns><c>true</c> when a cell exists at the local X/Z and global Y coordinate; otherwise, <c>false</c>.</returns>
        internal bool TryGetCell(
            MapChunkLocalCoord local,
            int y,
            [NotNullWhen(true)] out Cell? cell)
        {
            return _cells.TryGetValue(new MapChunkCellKey(local, y), out cell);
        }

        /// <summary>
        /// Retrieves an existing sparse map cell or creates it when authoritative state needs storage.
        /// </summary>
        /// <param name="local">The zero-based X/Z coordinate inside this map chunk.</param>
        /// <param name="y">The global vertical Y coordinate of the cell.</param>
        /// <returns>The existing or newly created sparse map cell.</returns>
        internal Cell GetOrCreateCell(MapChunkLocalCoord local, int y)
        {
            var key = new MapChunkCellKey(local, y);

            if (_cells.TryGetValue(key, out var cell))
            {
                return cell;
            }

            cell = new Cell(MapMath.ChunkLocalToCell(Coord, local, y));
            _cells.Add(key, cell);

            return cell;
        }

        /// <summary>
        /// Removes the sparse map cell at the specified local X/Z and global Y coordinate.
        /// </summary>
        /// <param name="local">The zero-based X/Z coordinate inside this map chunk.</param>
        /// <param name="y">The global vertical Y coordinate of the cell.</param>
        /// <returns><c>true</c> when a cell was removed; otherwise, <c>false</c>.</returns>
        internal bool TryRemoveCell(MapChunkLocalCoord local, int y)
        {
            return _cells.Remove(new MapChunkCellKey(local, y));
        }

        #endregion

        #region Terrain Storage

        /// <summary>
        /// Attempts to retrieve the terrain tile stored at the specified chunk-local coordinate.
        /// </summary>
        /// <param name="local">The zero-based terrain coordinate inside this map chunk.</param>
        /// <param name="terrainTile">The terrain tile if one occupies the local slot.</param>
        /// <returns><c>true</c> when terrain exists at <paramref name="local"/>; otherwise, <c>false</c>.</returns>
        internal bool TryGetTerrain(
            MapChunkLocalCoord local,
            [NotNullWhen(true)] out TerrainTile terrainTile)
        {
            terrainTile = _terrainTiles[ToTerrainIndex(local)]!;
            return terrainTile != null;
        }

        /// <summary>
        /// Stores a generated terrain tile at the specified chunk-local coordinate.
        /// </summary>
        /// <param name="local">The zero-based terrain coordinate inside this map chunk.</param>
        /// <param name="terrainTile">The generated terrain tile to store.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when generated terrain already exists at <paramref name="local"/>.
        /// </exception>
        internal void StoreGeneratedTerrain(MapChunkLocalCoord local, TerrainTile terrainTile)
        {
            ArgumentNullException.ThrowIfNull(terrainTile);

            int index = ToTerrainIndex(local);

            if (_terrainTiles[index] != null)
            {
                throw new InvalidOperationException($"Terrain already exists at local coordinate {local} in chunk {Coord}.");
            }

            _terrainTiles[index] = terrainTile;
            TerrainTileCount++;
        }

        /// <summary>
        /// Replaces existing terrain at the specified chunk-local coordinate.
        /// </summary>
        /// <param name="local">The zero-based terrain coordinate inside this map chunk.</param>
        /// <param name="terrainTile">The replacement terrain tile.</param>
        /// <returns>
        /// <c>true</c> when existing terrain was replaced; otherwise, <c>false</c> when the slot is empty.
        /// </returns>
        internal bool TryReplaceTerrain(MapChunkLocalCoord local, TerrainTile terrainTile)
        {
            ArgumentNullException.ThrowIfNull(terrainTile);

            int index = ToTerrainIndex(local);

            if (_terrainTiles[index] == null)
            {
                return false;
            }

            _terrainTiles[index] = terrainTile;
            return true;
        }

        /// <summary>
        /// Enumerates occupied terrain slots in this chunk without exposing the backing array.
        /// </summary>
        /// <returns>The chunk-local coordinates that currently contain terrain.</returns>
        internal IEnumerable<MapChunkLocalCoord> EnumerateTerrainLocals()
        {
            int chunkSize = MapChunkMetrics.ChunkSize;

            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    var local = new MapChunkLocalCoord(x, z);

                    if (_terrainTiles[ToTerrainIndex(local)] != null)
                    {
                        yield return local;
                    }
                }
            }
        }

        private static int ToTerrainIndex(MapChunkLocalCoord local)
        {
            return (local.Z * MapChunkMetrics.ChunkSize) + local.X;
        }

        #endregion

        #region Cell Key

        private readonly struct MapChunkCellKey : IEquatable<MapChunkCellKey>
        {
            #region Initialization

            internal MapChunkCellKey(MapChunkLocalCoord local, int y)
            {
                LocalX = local.X;
                Y = y;
                LocalZ = local.Z;
            }

            #endregion

            #region Properties

            private int LocalX { get; }

            private int Y { get; }

            private int LocalZ { get; }

            #endregion

            #region Equality

            public bool Equals(MapChunkCellKey other)
            {
                return LocalX == other.LocalX && Y == other.Y && LocalZ == other.LocalZ;
            }

            public override bool Equals(object? obj)
            {
                return obj is MapChunkCellKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(LocalX, Y, LocalZ);
            }

            #endregion
        }

        #endregion
    }
}
