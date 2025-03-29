using TurnTheTides;
using UnityEditor;
using UnityEngine;

public class WorldManager: MonoBehaviour
{
    private static WorldManager _instance;

    public double PollutionLevel
    {
        get => _pollutionLevel; private set => _pollutionLevel = value;
    }

    public readonly double PollutionMax = double.MaxValue;

    public MapData MapData
    {
        get => data; set => data = value;
    }
    [SerializeField]
    private double _pollutionLevel;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private MapData data;

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

    private void Awake()
    {
        SingletonCheck();

        DontDestroyOnLoad(gameObject);

        if (gridManager == null)
        {
            CreateNewGridManager();
        }

        PollutionLevel = 0;
    }

    private void CreateNewGridManager()
    {
        GameObject gridManagerPrefab = Resources.Load("Prefabs/Managers/GridManager") as GameObject;
        PrefabUtility.InstantiatePrefab(gridManagerPrefab);
        gridManager = gridManagerPrefab.GetComponent<GridManager>();
    }

    private void SingletonCheck()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance != null && _instance != this)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void CreateNewLevel(
        TextAsset levelData,
        int mapSizeOffset,
        float flood_increment
    )
    {
        MapData = new(levelData, mapSizeOffset, flood_increment);
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
        }

    }

    public static WorldManager GetInstance()
    {
        return _instance;
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
