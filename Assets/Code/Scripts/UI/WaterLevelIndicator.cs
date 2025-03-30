using UnityEngine;
using UnityEngine.UI;

public class WaterLevelIndicator : MonoBehaviour
{
    public float MeterUpdateSpeed;
    [Space]
    public Slider slider;
    public Text waterLevelLabel;
    public Text waterLevelIncreaseLabel;

    private float _currentSeaLevel;
    private float _displayedSeaLevel;

    private float _currentSeaLevelMetres;
    private float _displayedSeaLevelMetres;

    private float _seaLevelIncrease = 0f;

    public void SetSeaLevel(float min, float max, float current)
    {
        _currentSeaLevel = Mathf.InverseLerp(min, max, current);
        _currentSeaLevelMetres = current;
    }

    public void SetSeaLevelIncrease(float increase)
    {
        _seaLevelIncrease = increase;
        waterLevelIncreaseLabel.text = $"+{_seaLevelIncrease:F1}m";

        waterLevelIncreaseLabel.gameObject.SetActive(_seaLevelIncrease > 0f);
    }

    public float GetSeaLevel()
    {
        return _currentSeaLevel;
    }

    public float GetSeaLevelInMetres()
    {
        return _currentSeaLevelMetres;
    }

    public float GetSeaLevelIncrease()
    {
        return _seaLevelIncrease;
    }

    private void Update()
    {
        _displayedSeaLevel = Mathf.Lerp(_displayedSeaLevel, _currentSeaLevel, MeterUpdateSpeed * Time.deltaTime);
        _displayedSeaLevelMetres = Mathf.Lerp(_displayedSeaLevelMetres, _currentSeaLevelMetres, MeterUpdateSpeed * Time.deltaTime);
        slider.value = _displayedSeaLevel;
        waterLevelLabel.text = $"{_displayedSeaLevelMetres:F1}m";
    }
}
