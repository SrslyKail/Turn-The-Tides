using UnityEngine;
using UnityEngine.UI;

public class PollutionMeter : MonoBehaviour
{
    public Slider ProgressSlider;

    public void SetProgress(float progress)
    {
        ProgressSlider.value = progress;
    }

    public float GetProgress()
    {
        return ProgressSlider.value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
