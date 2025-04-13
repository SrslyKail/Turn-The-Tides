using TMPro;
using UnityEngine;

/// <summary>
/// Class to manage the turn counter text.
/// The turn counter text displays the current year in the simulation.
/// Written by Corey Buchan.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TurnCounterText : MonoBehaviour
{
    private TMP_Text textAsset;
    public void SetTurnText(int turn)
    {
        transform.GetComponent<TMP_Text>().SetText(turn.ToString());
    }

    /// <summary>
    /// Called once before the application starts.
    /// </summary>
    public void Awake()
    {
        SetTurnText(2025);
    }


}
