using TurnTheTides;
public class WetlandsTile: LandTile
{
    protected override void SetPollution()
    {
        pollutionValue = -0.2f;
        storedPollution = 17.5f;
    }
}
