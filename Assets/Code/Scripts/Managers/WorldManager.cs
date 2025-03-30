using System;
using TurnTheTides;
using UnityEditor;
using UnityEngine;

public class WorldManager: MonoBehaviour
{
    private static WorldManager _instance;

    [SerializeField]
    private double _pollutionLevel;
    public double PollutionLevel
    {
        get => _pollutionLevel; private set => _pollutionLevel = value;
    }
    public readonly double PollutionMax = double.MaxValue;

    [SerializeField]
    private MapData data;
    public MapData MapData
    {
        get => data; set => data = value;
    }

    [SerializeField]
    private GridManager gridManager;
    public GridManager GridManager
    {
        get
        {
            if (gridManager == null)
            {
                CreateNewGridManager();
            }

            return gridManager;
        }
        private set
        {
            gridManager = value;
        }
    }

    private void Start()
    {
        SingletonCheck();

        DontDestroyOnLoad(gameObject);

        if (gridManager == null)
        {
            CreateNewGridManager();
        }

        if (MapData != null)
        {
            gridManager.BuildMap(MapData);
        }
    }

    private void CreateNewGridManager()
    {
        GameObject gridManagerPrefab = Resources.Load("Prefabs/Managers/GridManager") as GameObject;
        //PrefabUtility.InstantiatePrefab(gridManagerPrefab); //This line is causing two errors: Dereferencing NULL PPtr! and Prefab was destroyed during instantiation. Are you calling DestroyImmediate() on the root GameObject?
        GameObject instantiatedGridManager = Instantiate(gridManagerPrefab);
        gridManager = instantiatedGridManager.GetComponent<GridManager>();
    }

    public GameObject GetTile(int row, int col)
    {
        return gridManager.GetTile(row, col);
    }

    private void SingletonCheck()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance != null && _instance != this)
        {
            Helper.SmartDestroy(gameObject);
        }
    }
    
    public void CreateNewLevel(
        TextAsset levelData,
        int mapSizeOffset,
        float flood_increment
    )
    {
        MapData = ScriptableObject.CreateInstance<MapData>();
        MapData.LoadData(levelData, mapSizeOffset, flood_increment);
        SetupWorld();
    }

    /// <summary>
    /// Resets the logic for the map, destroying all child objects, 
    /// regenerating geoData if needed, and creating the hexgrid.
    /// </summary>
    [ContextMenu("Refresh Game")]
    public void SetupWorld()
    {
        if (MapData == null)
        {
            EditorUtility.DisplayDialog(
                "No map data",
                "No map data has been given to the World Manager.",
                "Close"
                );
        }
        else
        {
            GridManager.BuildMap(MapData);
            PollutionLevel = 0;
        }
    }

    public void IncreasePollution(double amount)
    {
        PollutionLevel += amount;
    }

    public void DecreasePollution(double amount)
    {
        PollutionLevel -= amount;
    }

    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        //double newPollution = gridManager.Flood(floodIncrement);
        double newPollution = gridManager.Flood();
        newPollution += gridManager.CalculateNewPollution();
        PollutionLevel += newPollution;
        Debug.Log($"New pollution: {PollutionLevel}");
    }
}
