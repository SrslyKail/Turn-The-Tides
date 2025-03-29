namespace TurnTheTides
{
    /// <summary>
    /// Represents moving water. Can be used for industry.
    /// Made by Corey Buchan.
    /// </summary>
    public class River: Water
    {
        public override TerrainType Terrain => TerrainType.River;

        private void Awake()
        {
            pollutionValue = 0;
        }
    }

}

