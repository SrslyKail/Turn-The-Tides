using System;
using TurnTheTides;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldManager: MonoBehaviour
{
    private static WorldManager _instance;
    public static WorldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                WorldManager found = Helper.FindOrCreateSingleton<WorldManager>("Prefabs/Managers/WorldManager");

                if (found.enabled == false)
                {
                    found.enabled = true;
                }

                _instance = found;
            }

            return _instance;
        }
    }

    [SerializeField]
    private double _pollutionLevel;
    public double PollutionLevel
    {
        get => _pollutionLevel; set => _pollutionLevel = value;
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
                gridManager = GridManager.Instance;
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

        //DontDestroyOnLoad(gameObject);

        if (MapData != null)
        {
            gridManager.BuildMap(MapData);
        }
    }

    /// <summary>
    /// Get a specific tile from the current hex grid.
    /// </summary>
    /// <param name="row">The 'y' coordinate of the tile you want to access.</param>
    /// <param name="col">The 'x' coordinate of the tile you want to access.</param>
    /// <returns>A GameObject encapsulating all the hextile information.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If either Row or Col are too low or high.</exception>
    public GameObject GetTile(int row, int col)
    {
        return gridManager.GetTile(row, col);
    }

    /// <summary>
    /// Helper function to setup the Singleton logic for this class.
    /// Should be called during Awake.
    /// </summary>
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
    
    /// <summary>
    /// Create a new level to play on.
    /// </summary>
    /// <param name="levelData">a JSON containing the data for the hexgrid.</param>
    /// <param name="mapSizeOffset">A scaler for the total map size. Effectively only uses every n tiles in each row and column.</param>
    /// <param name="flood_increment">The base amount you want the waters to rise by each flood action.</param>
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
    /// Resets the runtime variables for the currently loaded level and resets the map to its default state.
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

    /// <summary>
    /// Enact all logic needed to go to the next turn.
    /// Should flood the map if the pollution has passed the threshold,
    /// track new pollution freed by any potential tile destruction, and 
    /// then add new pollution from the existing tiles.
    /// </summary>
    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        double newPollution = gridManager.Flood();
        newPollution += gridManager.CalculatePollutionPerTurn();
        PollutionLevel += newPollution;
        Debug.Log($"New pollution: {PollutionLevel}");
    }
}