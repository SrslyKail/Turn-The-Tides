using UnityEngine;
using TurnTheTides;
class WetlandsTile : LandTile
{
    public WetlandsTile(TerrainType terrain) : base(terrain)
    {
        pollutionValue = -0.2f;
        storedPollution = 17.5f;
    }
}
