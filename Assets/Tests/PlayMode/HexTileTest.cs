using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEngine;
using UnityEngine.TestTools;

public class HexTileTest
{
    [Test]
    public void GetHexLabel()
    {
        TextAsset json = Resources.Load("Maps/test_map_2") as TextAsset;
        WorldManager worldManager = WorldManager.Instance;
        //GameObject WorldManagerPrefab = Resources.Load("Prefabs/Managers/WorldManager") as GameObject;
        //PrefabUtility.InstantiatePrefab(WorldManagerPrefab);
        //WorldManager worldManager = WorldManagerPrefab.GetComponent<WorldManager>();
        //GridManager gridManager = GridManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();

        GameObject testTile = worldManager.GetTile(1, 2);
        GameObject testTile2 = worldManager.GetTile(2, 0);

        HexTile testHexTile = testTile.GetComponent<HexTile>();
        HexTile testHexTile2 = testTile2.GetComponent<HexTile>();

        Assert.AreEqual(testHexTile.landUseLabel, "Glaciers and Snow");
        Assert.AreEqual(testHexTile2.landUseLabel, "Mining");
    }

    [Test]
    public void GetElevation()
    {
        TextAsset json = Resources.Load("Maps/test_map_2") as TextAsset;
        WorldManager worldManager = WorldManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();

        GameObject testTile = worldManager.GetTile(1, 2);
        GameObject testTile2 = worldManager.GetTile(2, 0);

        HexTile testHexTile = testTile.GetComponent<HexTile>();
        HexTile testHexTile2 = testTile2.GetComponent<HexTile>();

        Assert.IsTrue(testHexTile.Elevation == 100);
        Assert.IsTrue(testHexTile2.Elevation == 50);


    }




}
