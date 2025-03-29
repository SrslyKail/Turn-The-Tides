namespace TurnTheTides
{
    /// <summary>
    /// Represents an inland body of fresh water.
    /// Made by Corey Buchan.
    /// </summary>
    internal class Lake: Water
    {
        public override TerrainType Terrain => TerrainType.Lake;

        public Lake()
        {
            pollutionValue = 0;
        }

        private void Awake()
        {
            pollutionValue = 0;
        }
    }

}

