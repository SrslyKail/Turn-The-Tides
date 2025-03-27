using System;
using TurnTheTides;
using Unity.Editor.Tasks;
using UnityEditorInternal;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    [SerializeField]
    private float _pollutionLevel;
    [SerializeField]
    private GridManager gridManager;
    public float PollutionLevel
    {
        get
        {
            return _pollutionLevel;
        }
        private set
        {
            _pollutionLevel = value;
        }
    }
    public readonly float PollutionMax = float.MaxValue;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        PollutionLevel = 0;
    }

    public static WorldManager GetInstance()
    {
        return _instance;
    }

    public void IncreasePollution(float amount)
    {
        PollutionLevel += amount;
    }

    public void DecreasePollution(float amount)
    {
        PollutionLevel -= amount;
    }

    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        float newPollution = gridManager.Flood();
        newPollution += gridManager.CalculateNewPollution();
        PollutionLevel += newPollution;
        Debug.Log($"New pollution: {PollutionLevel}");
    }
}
