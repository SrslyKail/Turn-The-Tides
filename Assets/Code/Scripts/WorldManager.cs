using System;
using TurnTheTides;
using Unity.Editor.Tasks;
using UnityEditorInternal;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    public float PollutionLevel { get; private set; }
    public readonly float PollutionMax = float.MaxValue;

    private WorldManager()
    {
        if (_instance != null)
        {
            throw new ArgumentException("World Manager constructor was called when an instance already exists.");
        }
        _instance = this;
        PollutionLevel = 0;
    }

    public static WorldManager GetInstance()
    {
        return _instance == null ? new() : _instance;
    }

    public void IncreasePollution(float amount)
    {
        PollutionLevel += amount;
    }

    public void DecreaseAmount(float amount)
    {
        PollutionLevel -= amount;
    }

    public void NextTurn()
    {
        GridManager gridManager = GridManager.GetInstance();
        float newPollution = gridManager.Flood();
        newPollution += gridManager.CalculateNewPollution();
        PollutionLevel += newPollution;

    }
}
