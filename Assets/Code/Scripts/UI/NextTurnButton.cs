using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextTurnButton : MonoBehaviour
{
    [Header("References")]
    public Button button;

    [Header("Button")]
    public bool ButtonEnabled = true;

    public UnityEvent OnButtonClicked;

    public void SetButtonEnabled(bool enabled)
    {
        ButtonEnabled = enabled;
        button.interactable = enabled;
    }

    public void OnClick()
    {
        if (ButtonEnabled)
        {
            //Debug.Log("Next turn button clicked");
            OnButtonClicked?.Invoke();
        }
    }

    void Start()
    {
        OnButtonClicked ??= new UnityEvent();
    }
}
