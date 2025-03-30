using System;
using UnityEngine;
using UnityEngine.Events;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    public PollutionMeter pollutionMeter;
    public WaterLevelIndicator waterLevelIndicator;
    public NextTurnButton nextTurnButton;

    [Header("CO2 Meter")]
    [Range(0f, 1f)]
    public float PollutionProgress = 0.0f;

    [Header("Water Level Indicator")]
    public float MinSeaLevel = 0.0f;
    public float MaxSeaLevel = 100.0f;
    public float CurrentSeaLevel = 0.0f;
    public float SeaLevelIncrement = 0.0f;

    [Header("Next Turn Button")]
    public bool NextTurnButtonEnabled = true;
    public UnityEvent NextTurnRequestedEvent;

    public void OnNextTurnRequested()
    {
        //Debug.Log("Next turn requested");
        NextTurnRequestedEvent.Invoke();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        pollutionMeter.SetProgress(PollutionProgress);
        waterLevelIndicator.SetSeaLevel(MinSeaLevel, MaxSeaLevel, CurrentSeaLevel);
        waterLevelIndicator.SetSeaLevelIncrease(SeaLevelIncrement);
        nextTurnButton.SetButtonEnabled(NextTurnButtonEnabled);
    }
}
