namespace TurnTheTides
{
    /// <summary>
    /// Tile to track features unique to a Forest.
    /// Made by Corey Buchan
    /// </summary>
    internal class ForestTile: LandTile
    {
        public ForestTile()
        {
            pollutionValue = -0.1f;
            storedPollution = 1.5f;
        }

        private void Awake()
        {
            pollutionValue = -0.1f;
            storedPollution = 1.5f;
        }
    }
}
