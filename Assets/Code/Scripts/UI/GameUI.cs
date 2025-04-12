using System;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Container component for the in-game GUI.
/// 
/// Written by Gurjeet Bhangoo, ported to a singleton by Corey Buchan.
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

    /// <summary>
    /// Whether the tile info panel is shown on screen.
    /// </summary>
    [Header("Tile Info Panel")]
    public bool TileInfoPanelActive = true;

    /// <summary>
    /// The starting year in the simulation.
    /// </summary>
    private static readonly int _startYear = System.DateTime.Today.Year;
    /// <summary>
    /// The current turn count in the simulation.
    /// </summary>
    private static int _turnCount = _startYear;

    /// <summary>
    /// Called once before the application starts.
    /// </summary>
    private void Awake()
    {
        SingletonCheck();
        TTTEvents.CreateNewMap += OnCreateNewMap;
        TTTEvents.FloodEvent += OnFlood;
        TTTEvents.NextTurnEvent += OnNextTurn;
        TTTEvents.CreateNewMap += OnCreateMap;
    }

    /// <summary>
    /// Called when the game map is created.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCreateMap(object sender, EventArgs e)
    {
        _turnCount = _startYear;
    }

    /// <summary>
    /// Called when the board is flooded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFlood(object sender, EventArgs e)
    {
        FloodEventArgs args = e as FloodEventArgs;
        CurrentSeaLevel = args.CurrentWaterLevel;
        UpdateTileInfoPanel();
    }

    /// <summary>
    /// Called when the current turn is ended and the next turn is started.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNextTurn(object sender, EventArgs e)
    {
        _turnCount++;
        turnCounterText.SetTurnText(_turnCount);
    }

    /// <summary>
    /// Called when a new map is created.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCreateNewMap(object sender, EventArgs e)
    {
        NewMapEventArgs args = e as NewMapEventArgs;
        SeaLevelIncrement = args.FloodAmount;
        waterLevelIndicator.SetSeaLevelIncrease(SeaLevelIncrement);
    }
    /// <summary>
    /// Checks if the singleton instance of this class is null. If it is, it sets the instance to this object. If not, it destroys this object.
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
    /// Updates the tile info panel with the current tile information.
    /// </summary>
    public void UpdateTileInfoPanel()
    {
        if (tileInfoPanel == null)
        {
            return;
        }
        TileInfoPanelActive = true;
        tileInfoPanel.UpdateTileInfo();
    }

    /// <summary>
    /// Updates the tile info panel with the specified tile information.
    /// </summary>
    /// <param name="tile"></param>
    public void UpdateTileInfoPanel(HexTile tile)
    {
        if (tileInfoPanel == null)
        {
            return;
        }

        TileInfoPanelActive = true;
        tileInfoPanel.UpdateTileInfo(tile);
    }

    /// <summary>
    /// Clears the tile info panel.
    /// </summary>
    public void ClearTileInfoPanel()
    {
        if (tileInfoPanel == null)
        {
            return;
        }
        TileInfoPanelActive = false;
        tileInfoPanel.ClearTileInfo();
    }

    /// <summary>
    /// Hides the tile info panel.
    /// </summary>
    public void HideTileInfoPanel()
    {
        if (tileInfoPanel == null)
        {
            return;
        }

        TileInfoPanelActive = false;
        tileInfoPanel.ClearTileInfo();
    }

    /// <summary>
    /// Called once per frame, once all other updates have been called.
    /// </summary>
    void LateUpdate()
    {
        pollutionMeter.SetProgress(PollutionProgress);
        waterLevelIndicator.SetSeaLevel(MinSeaLevel, MaxSeaLevel, CurrentSeaLevel);
        waterLevelIndicator.SetSeaLevelIncrease(SeaLevelIncrement);
        nextTurnButton.SetButtonEnabled(NextTurnButtonEnabled);
        MoveTileInfo(TileInfoPanelActive);
    }

    /// <summary>
    /// Animates the tile info panel to move on and off screen, depending on whether it is active or not.
    /// </summary>
    /// <param name="active"></param>
    private void MoveTileInfo(bool active)
    {
        if (tileInfoPanel == null)
        {
            return;
        }

        RectTransform rectTransform = tileInfoPanel.GetComponent<RectTransform>();
        Vector3 newPos = rectTransform.anchoredPosition;
        Vector2 panelSize = rectTransform.sizeDelta;

        float halfWidth = panelSize.x / 2;

        newPos.x = Mathf.Lerp(newPos.x, active ? -halfWidth : halfWidth, Time.deltaTime * 10f);

        rectTransform.anchoredPosition = newPos;
    }

    /// <summary>
    /// Called when the exit button is clicked.
    /// </summary>
    public void OnExitClick()
    {
        TTTEvents.QuitGame();
    }
}
