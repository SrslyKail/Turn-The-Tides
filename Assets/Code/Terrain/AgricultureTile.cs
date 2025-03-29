using TurnTheTides;
internal class AgricultureTile: LandTile
{
    public AgricultureTile()
    {
        pollutionValue = 0.05f;
    }

    private void Awake()
    {
        pollutionValue = 0.05f;
    }
}
