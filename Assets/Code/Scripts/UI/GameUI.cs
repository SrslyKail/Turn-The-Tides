using UnityEngine;

public class GameUI : MonoBehaviour
{
    public PollutionMeter pollutionMeter;

    public float PollutionProgress = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pollutionMeter.SetProgress(PollutionProgress);
    }
}
