using TurnTheTides;
public class AgricultureTile: LandTile
{
    /// <summary>
    /// Tile to track features unique to agriculture.
    /// Made by Corey Buchan
    /// </summary>
    protected override void SetPollution()
    {
        pollutionValue = 0.05f;
    }

}
