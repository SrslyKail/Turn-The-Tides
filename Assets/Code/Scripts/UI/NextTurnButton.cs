using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Button that advances the game to the next turn.
/// </summary>
public class NextTurnButton : MonoBehaviour
{
    /// <summary>
    /// The reference to the button component.
    /// </summary>
    [Header("References")]
    public Button button;

    /// <summary>
    /// Whether the button is enabled.
    /// </summary>
    [Header("Button")]
    public bool ButtonEnabled = true;

    /// <summary>
    /// Event invoked when the button is clicked.
    /// </summary>
    public UnityEvent OnButtonClicked;

    /// <summary>
    /// Sets the button enabled state.
    /// </summary>
    /// <param name="enabled">True if the button should be enabled, false otherwise</param>
    public void SetButtonEnabled(bool enabled)
    {
        ButtonEnabled = enabled;
        button.interactable = enabled;
    }

    /// <summary>
    /// Called when the button is clicked.
    /// </summary>
    public void OnClick()
    {
        if (ButtonEnabled)
        {
            OnButtonClicked?.Invoke();
        }
    }

    void Start()
    {
        OnButtonClicked.AddListener(() => { TTTEvents.NextTurnRequestedEvent.Invoke(this, EventArgs.Empty); });
    }
}
