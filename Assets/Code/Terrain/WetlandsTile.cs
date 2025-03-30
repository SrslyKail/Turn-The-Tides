using TurnTheTides;
public class WetlandsTile: LandTile
{
    /// <summary>
    /// Tile to track features unique to swamps, marshes, bogs, etc.
    /// Made by Corey Buchan
    /// </summary>
    protected override void SetPollution()
    {
        pollutionValue = -0.2f;
        storedPollution = 17.5f;
    }
}
