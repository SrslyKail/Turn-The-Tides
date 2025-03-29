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
        get => data; private set => data = value;
    }
    [SerializeField]
    private double _pollutionLevel;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private MapData data;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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

        if (gridManager == null)
        {
            GameObject gridManagerObj = new("Grid Manager", typeof(GridManager));
            gridManager = gridManagerObj.GetComponent<GridManager>();
            Instantiate(gridManagerObj);
        }
        else if (gridManager != null && gridManager.gameObject != null)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(gridManager.gameObject);
            }
            else
            {
                Destroy(gridManager.gameObject);
            }
        }

        PollutionLevel = 0;
    }

    public void CreateNewLevel(
        TextAsset levelData,
        int mapSizeOffset,
        float flood_increment
    )
    {
        data = new(levelData, mapSizeOffset, flood_increment);
    }

    /// <summary>
    /// Resets the logic for the map, destroying all child objects, 
    /// regenerating geoData if needed, and creating the hexgrid.
    /// </summary>
    [ContextMenu("Refresh Game")]
    private void SetupWorld()
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
            gridManager.BuildMap(MapData);
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
