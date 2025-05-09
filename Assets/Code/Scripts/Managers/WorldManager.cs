using System;
using System.Collections;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Manages the over-all state of the game, and acts as a connective tissue between various game systems.
/// <para>
/// Should be a singleton, so do not instanciate directly. Instead, please use WorldManager.Instance to get a reference to the instance.
/// </para>
/// </summary>
public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    /// <summary>
    /// Accessor for the singleton of this class.
    /// </summary>
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
    private TextAsset defaultMap;

    public MapData MapData {get; set;}
    
    public GridManager GridManager
    {
        get
        {
            return GridManager.Instance;
        }
    }


    [Header("Gameplay Variables")]
    [SerializeField]
    private double _pollutionLevel;
    private bool flooding = false;
    IEnumerator floodCoroutine;
    public double PollutionTotal
    {
        get => _pollutionLevel; set => _pollutionLevel = value;
    }
    public readonly double PollutionMax = double.MaxValue;
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
        floodIncrement = args.CurrentWaterLevel;
    }

    private void Awake()
    {
        MusicManager music = MusicManager.Instance;
        UIManager ui = UIManager.Instance;
        GridManager gridManager = GridManager.Instance;

    }

    private void Start()
    {
        SingletonCheck();
        floodCoroutine = FloodCoroutine();

        DontDestroyOnLoad(gameObject);

        ConnectEvents();

        if(MapData == null)
        {
            TTTEvents.CreateNewMap(this, new NewMapEventArgs()
            {
                DataFile = Resources.Load<TextAsset>("Maps/lowerMainland"),
                MapScale = mapSizeOffset,
                FloodAmount = floodIncrement
            });
        }
    }

    private void ConnectEvents()
    {
        TTTEvents.NextTurnEvent += OnNextTurn;
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
                TTTEvents.CreateNewMap(this, new NewMapEventArgs()
                {
                    DataFile = textAsset,
                    MapScale = mapSizeOffset,
                    FloodAmount = floodIncrement
                });
            }
        }
 
        GridManager.BuildMap(MapData, isCustomMap);
        PollutionTotal = 0;
        UpdateWorldState(BoardState.NewBoard);
    }

    private void OnCreateNewMap(object sender, EventArgs e)
    {
        NewMapEventArgs args = e as NewMapEventArgs;
        CreateNewLevel(args.DataFile, args.MapScale, args.FloodAmount);
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
        isCustomMap = !levelData.Equals(defaultMap);
        TTTEvents.FinishCreatingMap.Invoke(this, EventArgs.Empty);
    }

    private void UpdateWorldState(BoardState newState)
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

    /// <summary>
    /// Enact all logic needed to go to the next turn.
    /// Should flood the map if the pollution has passed the threshold,
    /// track new pollution freed by any potential tile destruction, and 
    /// then add new pollution from the existing tiles.
    /// </summary>
    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        TTTEvents.NextTurnEvent.Invoke(this, EventArgs.Empty);
    }

    private void OnNextTurn(object sender, EventArgs e)
    {
        StartFlood();

        float ratio = GridManager.GetFloodedRatio();
        if (ratio > 0.2)
        {
            UpdateWorldState(BoardState.HighPollution);
        }
        else if (ratio > 0.1)
        {
            UpdateWorldState(BoardState.ModeratePollution);
        }
        else if (ratio > 0.04)
        {
            UpdateWorldState(BoardState.LowPollution);
        }
    }

    private void StartFlood()
    {
        waterElevation += MapData.floodIncrement;

        TTTEvents.FloodEvent.Invoke(this, new FloodEventArgs
        {
            CurrentWaterLevel = waterElevation
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

    private void UpdatePollutionState()
    {
        //Logic to check the pollution levels vs the 
        //PollutionTotal and set the board state accordingly
        // We ran out of time to implement this feature, but this
        // would be where the logic would go.
    }

    [ContextMenu("Toggle Flooding")]
    private void ToggleFlood()
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