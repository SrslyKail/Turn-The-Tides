using System;
using System.Collections;
using TurnTheTides;
using UnityEngine;


public class WorldManager : MonoBehaviour
{
    public static readonly int start_year = 2025;
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

    public static int turn_count = start_year;
    private static float waterElevation = 0;

    private static GameUI _gameUI;
    public static GameUI GameUI
    {
        get
        {
            if (_gameUI == null)
            {
                GameUI found = Helper.FindOrCreateSingleton<GameUI>("Prefabs/Managers/GameUI");

                if (found.enabled == false)
                {
                    found.enabled = true;
                }

                _gameUI = found;
            }

            return _gameUI;
        }
        private set
        {
            _gameUI = value;
        }
    }

    private void Start()
    {
        SingletonCheck();

        //DontDestroyOnLoad(gameObject);
        if (MapData != null)
        {
            GridManager.BuildMap(MapData);
        }
        if (GridManager == null)
        {
            GridManager = GridManager.Instance;
        }
        if (GameUI == null)
        {
            GameUI = GameUI.Instance;
        }

        ConnectGameUIEvents();
    }

    private void ConnectGameUIEvents()
    {
        GameUI.NextTurnRequestedEvent.AddListener(NextTurn);
        GameUI.ToggleFloodEvent.AddListener(ToggleFlood);
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
        return GridManager.GetTile(row, col);
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
            //CB: TODO: Figure out how to make a runtime popup.
            //EditorUtility.DisplayDialog(
            //    "No map data",
            //    "No map data has been given to the World Manager.",
            //    "Close"
            //    );
        }
        else
        {
            GridManager.BuildMap(MapData);
            PollutionLevel = 0;
            turn_count = start_year;
            GameUI.MaxSeaLevel = 70f;
            GameUI.SeaLevelIncrement = MapData.floodIncrement;
            UpdateGUI();
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
        waterElevation += MapData.floodIncrement;
        double newPollution = GridManager.Flood();
        newPollution += GridManager.CalculatePollutionPerTurn();
        PollutionLevel += newPollution;
        turn_count++;
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        GameUI.turnCounterText.SetTurnText(turn_count);
        GameUI.CurrentSeaLevel = waterElevation;
    }

    bool flooding = false;
    Coroutine floodCoroutine;


    [ContextMenu("Toggle Flooding")]
    public void ToggleFlood()
    {
        flooding = !flooding;
        if (flooding)
        {
            StartFlooding();
        }
        else
        {
            StopFlooding();
        }
    }


    /// <summary>
    /// Coroutine for cycling next turn for simulation purposes.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FloodCoroutine()
    {

        while (flooding)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            NextTurn();
        }
    }

    /// <summary>
    /// Function to start the flooding coroutine.
    /// </summary>
    [ContextMenu("Start Flooding")]
    private void StartFlooding()
    {
        flooding = true;
        floodCoroutine = StartCoroutine(FloodCoroutine());
    }

    /// <summary>
    /// Function to stop the flooding coroutine.
    /// </summary>
    [ContextMenu("Stop Flooding")]
    private void StopFlooding()
    {
        flooding = false;
        StopCoroutine(floodCoroutine);
        floodCoroutine = null;
    }
}