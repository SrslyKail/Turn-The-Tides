using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the current water level in the in-game UI. The water level is represented as a slider that smoothly interpolates to its target value.
/// 
/// Written by Gurjeet Bhangoo.
/// </summary>
public class WaterLevelIndicator : MonoBehaviour
{
    /// <summary>
    /// The speed at which the meter updates. Higher values make the meter update faster.
    /// </summary>
    public float MeterUpdateSpeed;

    /// <summary>
    /// The slider component used to visually represent the water level.
    /// </summary>
    [Header("References")]
    public Slider slider;
    /// <summary>
    /// The label that displays the water level in metres.
    /// </summary>
    public Text waterLevelLabel;

    /// <summary>
    /// The label that displays how much the water level will increase next turn.
    /// </summary>
    public Text waterLevelIncreaseLabel;

    private float _currentSeaLevel;
    private float _displayedSeaLevel;

    private float _currentSeaLevelMetres;
    private float _displayedSeaLevelMetres;

    private float _seaLevelIncrease = 0f;

    /// <summary>
    /// Sets the value of the water level meter.
    /// </summary>
    /// <param name="min">The minimum water level, in metres</param>
    /// <param name="max">The maximum water level, in metres</param>
    /// <param name="current">The current water in level, in metres - must be between min and max</param>
    public void SetSeaLevel(float min, float max, float current)
    {
        _currentSeaLevel = Mathf.InverseLerp(min, max, current);
        _currentSeaLevelMetres = current;
    }

    /// <summary>
    /// Sets the text indicating how much the water level will increase next turn.
    /// </summary>
    /// <param name="increase">The amount the water level will increase, in metres</param>
    public void SetSeaLevelIncrease(float increase)
    {
        _seaLevelIncrease = increase;
        waterLevelIncreaseLabel.text = $"+{_seaLevelIncrease:F2}m";

        waterLevelIncreaseLabel.gameObject.SetActive(_seaLevelIncrease > 0f);
    }

    /// <summary>
    /// Returns the current value of the water level meter.
    /// </summary>
    /// <returns>A value from 0 to 1</returns>
    public float GetSeaLevel()
    {
        return _currentSeaLevel;
    }

    /// <summary>
    /// Returns the current stored value of the water level meter in metres.
    /// </summary>
    /// <returns>A value from the previously specified min to max water level</returns>
    public float GetSeaLevelInMetres()
    {
        return _currentSeaLevelMetres;
    }

    /// <summary>
    /// Returns the current stored value of the next-turn water level increase.
    /// </summary>
    /// <returns></returns>
    public float GetSeaLevelIncrease()
    {
        return _seaLevelIncrease;
    }

    /// <summary>
    /// Called once per frame.
    /// Animates the water level slider to match the current water level.
    /// </summary>
    private void Update()
    {
        _displayedSeaLevel = Mathf.Lerp(_displayedSeaLevel, _currentSeaLevel, MeterUpdateSpeed * Time.deltaTime);
        _displayedSeaLevelMetres = Mathf.Lerp(_displayedSeaLevelMetres, _currentSeaLevelMetres, MeterUpdateSpeed * Time.deltaTime);
        slider.value = _displayedSeaLevel;
        waterLevelLabel.text = $"{_displayedSeaLevelMetres:F2}m";
    }
}
