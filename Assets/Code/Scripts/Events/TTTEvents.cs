using UnityEngine;
using System;
using TurnTheTides;

/// <summary>
/// Event arguments to be passed with <see cref="TTTEvents.FloodEvent"/>
/// </summary>
public class FloodEventArgs: EventArgs
{
    /// <summary>
    /// The level of the water after the flood has completed.
    /// </summary>
    public float CurrentWaterLevel { get; set; }
}

/// <summary>
/// Event arguments to be passed with <see cref="TTTEvents.CreateNewMap"/>
/// </summary>
public class NewMapEventArgs: EventArgs
{
    /// <summary>
    /// The Json file that contains the map.
    /// </summary>
    public TextAsset DataFile { get; set; }
    /// <summary>
    /// The scale of the new map. Will only read every n-th tiles in a y-by-y map.
    /// </summary>
    public int MapScale { get; set; }
    /// <summary>
    /// The default amount the water will rise each turn.
    /// </summary>
    public float FloodAmount { get; set; }
}

/// <summary>
/// Event arguments to be passed with <see cref="TTTEvents.MapScaleChangeEvent"/>
/// </summary>
public class MapScaleEventArgs: EventArgs
{
    /// <summary>
    /// The new map scale.
    /// </summary>
    public int MapScale { get; set; }
}

/// <summary>
/// Event arguments to be passed with <see cref="TTTEvents.ChangeBoardState"/>
/// </summary>
public class BoardStateEventArgs: EventArgs
{
    /// <summary>
    /// The new board state.
    /// </summary>
    /// <seealso cref="TurnTheTides.BoardState"/>
    public BoardState NewBoardState { get; set; }
}

/// <summary>
/// The container for all events within Turn The Tides that should be globally accessible.
/// <para>
/// While the class is not explicitly static, due to needing to inherit from MonoBehavior for Unity reasons,
/// all methods, delegates, and attributes of the class should be set to static.
/// </para>
/// </summary>
public class TTTEvents: MonoBehaviour
{
    private static TTTEvents _instance;
    /// <summary>
    /// Checks if this class has been instanciated before; if it hasnt, creates an instance and returns it.
    /// <para>
    /// Effectively, a workaround for Unity lacking singletons.
    /// </para>
    /// </summary>
    public static TTTEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                TTTEvents found = Helper.FindOrCreateSingleton<TTTEvents>("Prefabs/Managers/EventManager");

                if (found.enabled == false)
                {
                    found.enabled = true;
                }

                _instance = found;
            }

            return _instance;
        }
    }
    /// <summary>
    /// Invoked when a tile is clicked.
    /// </summary>
    public static EventHandler TileClickEvent;

    /// <summary>
    /// Event invoked when the next turn is requested.
    /// </summary>
    public static EventHandler NextTurnEvent;

    /// <summary>
    /// Invoked when ToggleFlood is called
    /// </summary>
    public static EventHandler ToggleFloodEvent;

    /// <summary>
    /// Called when a flood is happening
    /// </summary>
    public static EventHandler FloodEvent;

    /// <summary>
    /// Called when the Board State changes
    /// </summary>
    public static EventHandler ChangeBoardState;

    /// <summary>
    /// Called when the Map Scale changes in the Main Menu
    /// </summary>
    public static EventHandler MapScaleChangeEvent;

    /// <summary>
    /// Called when the FloodIncrement is set in the Main Menu
    /// </summary>
    public static EventHandler FloodIncrementChangeEvent;

    /// <summary>
    /// Invoked when a new map has started being created.
    /// <para>
    /// Allows for logic that needs to be set up during map generation to be run.
    /// </para>
    /// </summary>
    public static EventHandler CreateNewMap;

    /// <summary>
    /// Invoked when the map being loaded is not the default map.
    /// Should be invoked <b>after</b> <see cref="CreateNewMap"/>
    /// </summary>
    public static EventHandler LoadCustomMap;

    /// <summary>
    /// Invoked when the a map has finished being created.
    /// <para>
    /// Allows for logic that needs to be updated **after** the map 
    /// generation logic has complete to be run.
    /// </para>
    /// </summary>
    public static EventHandler FinishCreatingMap;

    /// <summary>
    /// Invoked to assign the correct world locations for the name flags.
    /// </summary>
    public static EventHandler AssignLocationFlags;

    /// <summary>
    /// Global "Quit the game" function.
    /// </summary>
    public static void QuitGame()
    {
        Application.Quit();
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}