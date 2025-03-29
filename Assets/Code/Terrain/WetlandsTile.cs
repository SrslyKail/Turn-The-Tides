using TurnTheTides;
internal class WetlandsTile: LandTile
{
    public WetlandsTile()
    {
        pollutionValue = -0.2f;
        storedPollution = 17.5f;
    }

    private void Awake()
    {
        pollutionValue = -0.2f;
        storedPollution = 17.5f;
    }
}
