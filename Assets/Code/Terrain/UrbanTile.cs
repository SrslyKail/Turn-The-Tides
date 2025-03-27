using UnityEngine;
using TurnTheTides;
class UrbanTile : LandTile
{
    private void Awake()
    {
        pollutionValue = 24000f;
    }
    public UrbanTile(TerrainType terrain) : base(terrain)
    {
        pollutionValue = 24000f;
    }
}
