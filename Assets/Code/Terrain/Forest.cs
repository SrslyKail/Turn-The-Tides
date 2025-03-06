using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Abstract base for all HexTiles to extend from.
    /// Allows us to store water and land tiles in the same lists.
    /// </summary>
    class ForestTile : LandTile
    {
        public ForestTile(TerrainType terrain): base(terrain)
        {
        }
    }
}
