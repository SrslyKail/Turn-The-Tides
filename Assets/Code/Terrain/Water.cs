using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Base class for all water tiles; rivers, lakes, and oceans.
    /// </summary>
    abstract class Water : HexTile
    {
        //CB: If we want to add more detailed terrain types then we can remove this.
        public override TerrainType Terrain { get { return TerrainType.Water; } }

        private int _height;

        /// <summary>
        /// Tracks the difference between the bottom and top of the water tile.
        /// </summary>
        public virtual int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// Change the current height of the water.
        /// </summary>
        /// <param name="newHeight">The new height.</param>
        public virtual void ChangeHeight(int newHeight)
        {
            _height = newHeight;
        }
    }
}


