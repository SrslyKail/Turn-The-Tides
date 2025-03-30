using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class FloodTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void FloodTestSimplePasses()
    {

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FloodTestWithEnumeratorPasses()
    {

        TextAsset json = Resources.Load("Maps/test_map_2") as TextAsset;
        GameObject WorldManagerPrefab = Resources.Load("Prefabs/Managers/WorldManager") as GameObject;
        PrefabUtility.InstantiatePrefab(WorldManagerPrefab);
        WorldManager worldManager = WorldManagerPrefab.GetComponent<WorldManager>();
        GridManager gridManager = GridManager.GetInstance();
        Debug.Log("Grid Manager Instance Acquired");
        worldManager.CreateNewLevel(json, 1, 1);
        Debug.Log("New Level Created");
        worldManager.SetupWorld();
        Debug.Log("World Set Up");
        Debug.Log("World pollution level: " + worldManager.PollutionLevel);

        yield return null;
    }
}
