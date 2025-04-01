using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHexTile
{
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
