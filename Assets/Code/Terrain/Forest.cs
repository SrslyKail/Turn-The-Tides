using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Tile to track features unique to a Forest.
    /// Made by Corey Buchan
    /// </summary>
    class ForestTile : LandTile
    {
        public ForestTile(TerrainType terrain): base(terrain)
        {
            pollutionValue = -0.1f;
            storedPollution = 1.5f;
        }

        private void Awake()
        {
            pollutionValue = -0.1f;
            storedPollution = 1.5f;
        }
    }
}
