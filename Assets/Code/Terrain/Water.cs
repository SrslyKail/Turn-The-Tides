namespace TurnTheTides
{
    /// <summary>
    /// Base class for all water tiles; rivers, lakes, and oceans.
    /// </summary>
    internal class Water: HexTile
    {
        private int _height;

        /// <summary>
        /// Tracks the difference between the bottom and top of the water tile.
        /// </summary>
        public virtual int Height => _height;

        public override TerrainType Terrain => TerrainType.Invalid;

        /// <summary>
        /// Change the current height of the water.
        /// </summary>
        /// <param name="newHeight">The new height.</param>
        public virtual void ChangeHeight(int newHeight)
        {
            _height = newHeight;
        }

        protected override void SetPollution()
        {
            pollutionValue = 0;
        }
    }
}


