
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Represents an inland body of fresh water.
    /// Made by Corey Buchan.
    /// </summary>
    class Lake : Water
    {
        public override TerrainType Terrain => TerrainType.Lake;

        public Lake()
        {
            pollutionValue = 0;
        }
    }

}

