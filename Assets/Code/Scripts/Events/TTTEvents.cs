using UnityEngine;
using System;
using TurnTheTides;

public class FloodEventArgs: EventArgs
{
    public float FloodIncrement { get; set; }
}

public class NewMapEventArgs: EventArgs
{
    public TextAsset DataFile { get; set; }
    public int MapScale { get; set; }
    public float FloodAmount { get; set; }
}

public class MapScaleEventArgs: EventArgs
{
    public int MapScale { get; set; }
}

public class BoardStateEventArgs: EventArgs
{
    public BoardState NewBoardState { get; set; }
}

public class TTTEvents: MonoBehaviour
{
    private static TTTEvents _instance;
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
    public static EventHandler NextTurnRequestedEvent;

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

    public static EventHandler CreateNewMap;

    public static EventHandler LoadCustomMap;

    public static EventHandler FinishCreatingMap;

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