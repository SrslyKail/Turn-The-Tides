using System;
using System.Collections;
using TurnTheTides;
using UnityEngine;

public class WorldManager : MonoBehaviour
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
    
    [Header("References")]
    [SerializeField]
    private MapData data;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private GameUI _gameUI;
    [SerializeField]
    private MusicManager _musicManager;

    public MapData MapData
    {
        get => data; set => data = value;
    }
    
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

    public GameUI GameUI
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

    public MusicManager MusicManager
    {
        get
        {
            if (_musicManager == null)
            {
                _musicManager = MusicManager.Instance;
            }

            return _musicManager;
        }
        private set
        {
            _musicManager = value;
        }
    }


    [Header("Gameplay Variables")]
    [SerializeField]
    private double _pollutionLevel;
    private bool flooding = false;
    IEnumerator floodCoroutine;
    public static readonly int start_year = System.DateTime.Today.Year;
    public double PollutionTotal
    {
        get => _pollutionLevel; set => _pollutionLevel = value;
    }
    public readonly double PollutionMax = double.MaxValue;
    public static int turn_count = start_year;
    private static float waterElevation = 0;
    private BoardState boardState = BoardState.None;
    private int mapSizeOffset = 2;
    private float floodIncrement = 0.08f;
    private bool isCustomMap = false;

    private void OnMapScaleChange(object sender, EventArgs e)
    {
        MapScaleEventArgs args = e as MapScaleEventArgs;
        mapSizeOffset = args.MapScale;
    }

    private void OnFloodIncrementChange(object sender, EventArgs e)
    {
        FloodEventArgs args = e as FloodEventArgs;
        floodIncrement = args.FloodIncrement;
    }

    private void Start()
    {
        SingletonCheck();
        floodCoroutine = FloodCoroutine();
        MusicManager = MusicManager.Instance;

        DontDestroyOnLoad(gameObject);

        ConnectGameUIEvents();

        if(MapData == null)
        {
            CreateNewLevel(
                Resources.Load<TextAsset>("Maps/lowerMainland"),
                mapSizeOffset,
                floodIncrement
            );
        }
    }

    private void ConnectGameUIEvents()
    {
        TTTEvents.NextTurnRequestedEvent += OnNextTurn;
        TTTEvents.FloodEvent += OnFlood;
        TTTEvents.ToggleFloodEvent += OnToggleFlood;
        TTTEvents.FloodIncrementChangeEvent += OnFloodIncrementChange;
        TTTEvents.MapScaleChangeEvent += OnMapScaleChange;
        TTTEvents.CreateNewMap += OnCreateNewMap;
        TTTEvents.ChangeBoardState += OnChangeBoardState;
        TTTEvents.LoadCustomMap += OnLoadCustomMap;
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
    /// Resets the runtime variables for the currently loaded level and resets the map to its default state.
    /// </summary>
    [ContextMenu("Refresh Game")]
    public void SetupWorld()
    {
        while (MapData == null)
        {
            LoadExternalJson externalLoader = new();
            if(externalLoader.TryGetDataJson(out TextAsset textAsset))
            {
                CreateNewLevel(textAsset, mapSizeOffset, floodIncrement);
            }
        }
 
        GridManager.BuildMap(MapData, isCustomMap);
        PollutionTotal = 0;
        turn_count = start_year;
        GameUI.MaxSeaLevel = 70f;
        GameUI.SeaLevelIncrement = MapData.floodIncrement;
        UpdateGUI();
        UpdateWorldState(BoardState.NewBoard);
    }

    private void OnCreateNewMap(object sender, EventArgs e)
    {
        NewMapEventArgs args = e as NewMapEventArgs;
        CreateNewLevel(args.DataFile, args.MapScale, args.FloodAmount);
        TTTEvents.FinishCreatingMap.Invoke(this, EventArgs.Empty);
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
    }

    public void UpdateWorldState(BoardState newState)
    {
        if(boardState != newState)
        {
            this.boardState = newState;

            TTTEvents.ChangeBoardState.Invoke(
            this,
            new BoardStateEventArgs()
            {
                NewBoardState = newState
            });
        }

        
    }

    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        TTTEvents.NextTurnRequestedEvent.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Enact all logic needed to go to the next turn.
    /// Should flood the map if the pollution has passed the threshold,
    /// track new pollution freed by any potential tile destruction, and 
    /// then add new pollution from the existing tiles.
    /// </summary>
    private void OnNextTurn(object sender, EventArgs e)
    {
        Flood();
        
        float ratio = GridManager.GetFloodedRatio();
        Debug.Log($"Current ratio: {ratio}");
        if (ratio > 0.2)
        {
            UpdateWorldState(BoardState.HighPollution);
        }
        else if(ratio > 0.1)
        {
            UpdateWorldState(BoardState.ModeratePollution);
        }
        else if(ratio > 0.04)
        {
            UpdateWorldState(BoardState.LowPollution);
        }

        turn_count++;
        UpdateGUI();
    }

    public void Flood()
    {
        waterElevation += MapData.floodIncrement;

        TTTEvents.FloodEvent.Invoke(this, new FloodEventArgs
        {
            FloodIncrement = waterElevation
        });
    }

    private void OnChangeBoardState(object send, EventArgs e)
    {
        BoardStateEventArgs args = e as BoardStateEventArgs;
        if(args.NewBoardState == BoardState.Loading)
        {
            SetupWorld();
            UpdateWorldState(BoardState.NewBoard);
        }        
    }

    private void OnLoadCustomMap(object sender, EventArgs e)
    {
        isCustomMap = true;
    }


    private void OnFlood(object sender, EventArgs e)
    {
        double newPollution = GridManager.Flood();
        newPollution += GridManager.CalculatePollutionPerTurn();
        PollutionTotal += newPollution;
        UpdatePollutionState();
    }

    public void UpdatePollutionState()
    {
        //Logic to check the pollution levels vs the 
        //PollutionTotal and set the board state accordingly
    }

    // TODO: Turn this into an event for the GUI.
    private void UpdateGUI()
    {
        GameUI.turnCounterText.SetTurnText(turn_count);
        GameUI.CurrentSeaLevel = waterElevation;
        GameUI.UpdateTileInfoPanel();
    }

    [ContextMenu("Toggle Flooding")]
    public void ToggleFlood()
    {
        TTTEvents.ToggleFloodEvent.Invoke(this, EventArgs.Empty);
    }

    private void OnToggleFlood(object sender, EventArgs e)
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
    private IEnumerator FloodCoroutine()
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
        StartCoroutine(floodCoroutine);
    }

    /// <summary>
    /// Function to stop the flooding coroutine.
    /// </summary>
    [ContextMenu("Stop Flooding")]
    private void StopFlooding()
    {
        flooding = false;
        StopCoroutine(floodCoroutine);
    }
}