namespace TurnTheTides
{
    /// <summary>
    /// Represents an inland body of fresh water.
    /// Made by Corey Buchan.
    /// </summary>
    public class Lake: Water
    {
        public override TerrainType Terrain => TerrainType.Lake;

        private void Awake()
        {
            pollutionValue = 0;
        }
    }

}

