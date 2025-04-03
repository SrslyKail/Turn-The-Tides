using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class FloodTest
{
    /// <summary>
    /// Tests that a low elevation is consumed when the flood consumes it, and that a higher elevation tile is not.
    /// Ben made this
    /// </summary>
    [Test]
    public void FloodingTest()
    {
        TextAsset json = Resources.Load("Maps/test_map_3") as TextAsset;
        WorldManager worldManager = WorldManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();

        GameObject testTile = worldManager.GetTile(0, 0);
        GameObject testTile2 = worldManager.GetTile(1, 0);

        HexTile testHexTile = testTile.GetComponent<HexTile>();
        HexTile testHexTile2 = testTile2.GetComponent<HexTile>();

        Assert.NotNull(testHexTile);
        Assert.NotNull(testHexTile2);

        worldManager.NextTurn();
        worldManager.NextTurn();

        Assert.IsTrue(testHexTile == null);
        Assert.IsFalse(testHexTile2 == null);
    }
}
