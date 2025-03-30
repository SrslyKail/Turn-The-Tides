using TurnTheTides;
public class UrbanTile: LandTile
{
    /// <summary>
    /// Tile to track features unique to a City.
    /// Made by Corey Buchan
    /// </summary>
    protected override void SetPollution()
    {
        pollutionValue = 24000f;
    }
}
