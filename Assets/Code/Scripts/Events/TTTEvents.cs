using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using TurnTheTides;

public class FloodEventArgs: EventArgs
{
    public float FloodIncrement { get; set; }
}

public class BoardStateEventArgs: EventArgs
{
    public BoardState NewBoardState { get; set; }
}

public class TTTEvents: MonoBehaviour
{

    private static TTTEvents _instance;
    /// <summary>
    /// Get the current instance of the GridManager. 
    /// Will create one if one does not exist.
    /// </summary>
    public static TTTEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Helper.FindOrCreateSingleton<TTTEvents>("Prefabs/Managers/TTTEvents");
            }

            return _instance;
        }
    }

    public static EventHandler TileClickEvent;

    /// <summary>
    /// Event invoked when the next turn is requested.
    /// </summary>
    public static EventHandler NextTurnRequestedEvent;

    public static EventHandler ToggleFloodEvent;

    public static EventHandler FloodEvent;

    public static EventHandler ChangeBoardState;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}