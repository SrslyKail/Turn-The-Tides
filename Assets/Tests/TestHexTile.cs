using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHexTile
{

    [TestCase]
    public void TestHexTileGetElevation()
    {
        //The elevation should:
        //  be the expected level
        //  not be higher or lower than X amount from any of its neighbors

    }

    [TestCase]
    public void TestHexTileGetTerrain()
    {
        //The terrain:
        //  Should be the expected type
        //  Change to water when flooded
        //  Change to Barren when drained

    }

    [TestCase]
    public void TestHexTileFlood()
    {
        // The terrain type should change to water
        // The effects should cascade to neighboring tiles that are of equal or lower elevation

    }

    [TestCase]
    public void TestHexTileDrain()
    {
        // A tile that does not have Water type should be change its type
        // A tile that has water should have the water elevation reduced by 1
        // A tile that has 0 water after draining should change its type to Barren

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestHexTileWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
