using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// Abstract base for all HexTiles to extend from.
/// Allows us to store water and land tiles in the same lists.
/// </summary>
class LandTile : HexTile
{
    TerrainType _terrain;
    public override TerrainType Terrain => _terrain;

    public LandTile(TerrainType terrain)
    {
        _terrain = terrain;
    }
}