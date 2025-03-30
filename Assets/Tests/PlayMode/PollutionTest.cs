using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PollutionTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.


    [UnityTest]
    public IEnumerator PollutionIncreaseTest()
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

        yield return null;

        Assert.True(newPollution > oldPollution);

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PollutionDecreaseTest()
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

        yield return null;

        Assert.True(newPollution < oldPollution);

    }
}
