using UnityEngine;
using TurnTheTides;
class AgricultureTile : LandTile
{
    public AgricultureTile(TerrainType terrain) : base(terrain)
    {
        pollutionValue = 0.05f;
    }

    private void Awake()
    {
        pollutionValue = 0.05f;
    }
}
