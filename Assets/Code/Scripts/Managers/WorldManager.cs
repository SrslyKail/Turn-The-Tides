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
        if (MapData != null)
        {
            gridManager.BuildMap(MapData);
        }
    }

    private void CreateNewGridManager()
    {
        GameObject gridManagerPrefab = Resources.Load("Prefabs/Managers/GridManager") as GameObject;
        Debug.Log("Instantiating the grid manager is angy");
        //PrefabUtility.InstantiatePrefab(gridManagerPrefab); //This line is causing two errors: Dereferencing NULL PPtr! and Prefab was destroyed during instantiation. Are you calling DestroyImmediate() on the root GameObject?
        GameObject instantiatedGridManager = Instantiate(gridManagerPrefab);
        gridManager = gridManagerPrefab.GetComponent<GridManager>();

        Debug.Log("Destroying happens before this?");
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
        Debug.Log("CreateNewLevel Script entered");
        MapData = ScriptableObject.CreateInstance<MapData>();
        Debug.Log("MapData created");
        MapData.LoadData(levelData, mapSizeOffset, flood_increment);
        Debug.Log("MapData loaded");
        SetupWorld();
        Debug.Log("World Set Up");
    }

    /// <summary>
    /// Resets the logic for the map, destroying all child objects, 
    /// regenerating geoData if needed, and creating the hexgrid.
    /// </summary>
    [ContextMenu("Refresh Game")]
    public void SetupWorld()
    {
        Debug.Log("SetupWorld Entered");
        if (MapData == null)
        {
            Debug.Log("MapData was null");
            EditorUtility.DisplayDialog(
                "No map data",
                "No map data has been given to the World Manager.",
                "Close"
                );
        }
        else
        {
            Debug.Log("MapData was not null");
            GridManager.BuildMap(MapData);
            Debug.Log("This is unreachable?");

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
