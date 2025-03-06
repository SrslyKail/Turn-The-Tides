using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Abstract base for all HexTiles to extend from.
    /// Allows us to store water and land tiles in the same lists.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    abstract class HexTile : MonoBehaviour
    {
        public abstract TerrainType Terrain { get; }

        private int _elevation;
        /// <summary>
        /// Elevation represents the base of the tile.
        /// For land tiles, this should never change.
        /// For water tiles, we will implement a Height attribute to track
        /// the difference between the base elevation and the depth of the water.
        /// </summary>
        public virtual int Elevation
        {
            get { return _elevation; }
            private set { _elevation = value; }
        }
    }
}