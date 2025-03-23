using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// base for all land-based to extend from.
    /// Allows us to store all land terrain in the same collections.
    /// Made by Corey Buchan.
    /// </summary>
    class LandTile : HexTile
    {
        [SerializeField]
        TerrainType _terrain;
        public override TerrainType Terrain => _terrain;

        public LandTile(TerrainType terrain)
        {
            _terrain = terrain;
        }
    }
}
