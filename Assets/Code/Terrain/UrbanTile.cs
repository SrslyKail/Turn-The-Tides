using UnityEngine;
using TurnTheTides;
class UrbanTile : LandTile
{
    public UrbanTile(TerrainType terrain) : base(terrain)
    {
        pollutionValue = 24000f;
    }
}
