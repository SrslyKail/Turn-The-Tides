using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHexTile
{

    /// <summary>
    /// Test that we can get the elevation from a hex tile
    /// important because this data is used by other systems.
    /// </summary>
    [TestCase]
    public void TestHexTileGetElevation()
    {
        //The elevation should:
        //  be the expected level
        //  not be higher or lower than X amount from any of its neighbors

    }

    /// <summary>
    /// Test that we can get the terrain type from a hex tile
    /// important because this data is used by other systems.
    /// </summary>
    [TestCase]
    public void TestHexTileGetTerrain()
    {
        //The terrain:
        //  Should be the expected type
        //  Change to water when flooded
        //  Change to Barren when drained

    }


    /// <summary>
    /// Test that flooding a hex tile causes the attributes to change correctly
    /// and the effect propagates to adjacent tiles.
    /// </summary>
    [TestCase]
    public void TestHexTileFlood()
    {
        // The terrain type should change to water
        // The effects should cascade to neighboring tiles that are of equal or lower elevation

    }

    /// <summary>
    /// Test that draining a hex tile causes the attributes to change correctly
    /// and the effect propagates to adjacent tiles.
    /// </summary>
    [TestCase]
    public void TestHexTileDrain()
    {
        // A tile that does not have Water type should be change its type
        // A tile that has water should have the water elevation reduced by 1
        // A tile that has 0 water after draining should change its type to Barren

    }
}
