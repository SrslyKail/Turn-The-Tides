using System;
using TurnTheTides;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Container component for the in-game GUI.
/// </summary>
public class GameUI : MonoBehaviour
{
    private static GameUI _instance;
    public static GameUI Instance
    {
        get
        {
            if (_instance == null)
            {
                GameUI found = Helper.FindOrCreateSingleton<GameUI>("Prefabs/Managers/GameUI");

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
    /// The reference to the pollution meter component.
    /// </summary>
    [Header("References")]
    public PollutionMeter pollutionMeter;

    /// <summary>
    /// The reference to the water level indicator component.
    /// </summary>
    public WaterLevelIndicator waterLevelIndicator;

    /// <summary>
    /// The reference to the next turn button component.
    /// </summary>
    public NextTurnButton nextTurnButton;

    /// <summary>
    /// The reference to the turn counter text component.
    /// </summary>
    public TurnCounterText turnCounterText;

    /// <summary>
    /// The reference to the tile info panel component.
    /// </summary>
    public TileInfoPanel tileInfoPanel;

    /// <summary>
    /// The progress of the pollution meter. A value of 0 represents no pollution, while a value of 1 represents maximum pollution.
    /// </summary>
    [Header("CO2 Meter")]
    [Range(0f, 1f)]
    public float PollutionProgress = 0.0f;

    /// <summary>
    /// The current minimum sea level in metres.
    /// </summary>
    [Header("Water Level Indicator")]
    public float MinSeaLevel = 0.0f;
    /// <summary>
    /// The current maximum sea level in metres.
    /// </summary>
    public float MaxSeaLevel = 100.0f;
    /// <summary>
    /// The current sea level in metres.
    /// </summary>
    public float CurrentSeaLevel = 0.0f;
    /// <summary>
    /// The amount the sea level will increase next turn.
    /// </summary>
    public float SeaLevelIncrement = 0.0f;

    /// <summary>
    /// Whether the next turn button is enabled.
    /// </summary>
    [Header("Next Turn Button")]
    public bool NextTurnButtonEnabled = true;

    [Header("Tile Info Panel")]
    public bool TileInfoPanelVisible = true;

    /// <summary>
    /// Event invoked when the next turn is requested.
    /// </summary>
    public UnityEvent NextTurnRequestedEvent;

    public UnityEvent ToggleFloodEvent;

    public void Start()
    {
        SingletonCheck();
    }
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
    /// Called when the next turn is requested.
    /// </summary>
    public void OnNextTurnRequested()
    {
        //Debug.Log("Next turn requested");
        NextTurnRequestedEvent.Invoke();
    }

    public void OnToggleFloodRequest()
    {
        ToggleFloodEvent.Invoke();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        pollutionMeter.SetProgress(PollutionProgress);
        waterLevelIndicator.SetSeaLevel(MinSeaLevel, MaxSeaLevel, CurrentSeaLevel);
        waterLevelIndicator.SetSeaLevelIncrease(SeaLevelIncrement);
        nextTurnButton.SetButtonEnabled(NextTurnButtonEnabled);
    }

    public void UpdateTileInfoPanel(HexTile tile)
    {
        if (tileInfoPanel == null)
        {
            return;
        }

        tileInfoPanel.UpdateTileInfo(tile);
    }
}
