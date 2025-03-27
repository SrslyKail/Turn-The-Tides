
using UnityEngine;
namespace TurnTheTides
{
    /// <summary>
    /// Represents moving water. Can be used for industry.
    /// Made by Corey Buchan.
    /// </summary>
    class River : Water
    {
        public override TerrainType Terrain => TerrainType.River;

        public River()
        {
            pollutionValue = 0;
        }

        private void Awake()
        {
            pollutionValue = 0;
        }
    }

}

