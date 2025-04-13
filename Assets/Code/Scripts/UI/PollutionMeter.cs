using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the pollution meter in the in-game UI.
/// 
/// Written by Gurjeet Bhangoo.
/// </summary>
public class PollutionMeter : MonoBehaviour
{
    /// <summary>
    /// The slider component that represents the pollution meter.
    /// </summary>
    public Slider ProgressSlider;

    /// <summary>
    /// Sets the progress of the pollution meter.
    /// </summary>
    /// <param name="progress">A value from 0 to 1</param>
    public void SetProgress(float progress)
    {
        ProgressSlider.value = progress;
    }

    /// <summary>
    /// Gets the current progress of the pollution meter.
    /// </summary>
    /// <returns>A value from 0 to 1</returns>
    public float GetProgress()
    {
        return ProgressSlider.value;
    }
}
