using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PollutionTest
{
    /// <summary>
    /// Tests two different maps for the total tile contribution to pollution per turn.
    /// </summary>
    [Test]
    public void PerTurnPollutionAmountTest()
    {

        TextAsset json = Resources.Load("Maps/test_map_3") as TextAsset;
        GameObject WorldManagerPrefab = Resources.Load("Prefabs/Managers/WorldManager") as GameObject;
        PrefabUtility.InstantiatePrefab(WorldManagerPrefab);
        WorldManager worldManager = WorldManagerPrefab.GetComponent<WorldManager>();
        GridManager gridManager = GridManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();
        
        float testPollutionValue = gridManager.CalculatePollutionPerTurn();

        Assert.AreEqual(testPollutionValue, 192000f);

        TextAsset json2 = Resources.Load("Maps/test_map_2") as TextAsset;

        worldManager.CreateNewLevel(json2, 1, 1);

        float testPollutionValue2 = gridManager.CalculatePollutionPerTurn();

        Assert.AreEqual(testPollutionValue2, 23999.80078125);

    }

    /// <summary>
    /// Tests that the pollution increases as expected on test_map_3 when next turn is called.
    /// </summary>
    [Test]
    public void PollutionIncreaseTest()
    {

        TextAsset json = Resources.Load("Maps/test_map_3") as TextAsset;
        GameObject WorldManagerPrefab = Resources.Load("Prefabs/Managers/WorldManager") as GameObject;
        PrefabUtility.InstantiatePrefab(WorldManagerPrefab);
        WorldManager worldManager = WorldManagerPrefab.GetComponent<WorldManager>();
        GridManager gridManager = GridManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();
        double oldPollution = worldManager.PollutionLevel;
        worldManager.NextTurn();
        double newPollution = worldManager.PollutionLevel;

        Assert.True(newPollution > oldPollution);

    }

    /// <summary>
    /// Tests that pollution decreases as expected on test_map_4 when next turn is called.
    /// </summary>
    [Test]
    public void PollutionDecreaseTest()
    {

        TextAsset json = Resources.Load("Maps/test_map_4") as TextAsset;
        GameObject WorldManagerPrefab = Resources.Load("Prefabs/Managers/WorldManager") as GameObject;
        PrefabUtility.InstantiatePrefab(WorldManagerPrefab);
        WorldManager worldManager = WorldManagerPrefab.GetComponent<WorldManager>();
        GridManager gridManager = GridManager.Instance;
        worldManager.CreateNewLevel(json, 1, 1);
        worldManager.SetupWorld();
        double oldPollution = worldManager.PollutionLevel;
        worldManager.NextTurn();
        double newPollution = worldManager.PollutionLevel;

        Assert.True(newPollution < oldPollution);

    }
}
