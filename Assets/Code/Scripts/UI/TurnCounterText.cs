
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TurnCounterText : MonoBehaviour
{
    private TMP_Text textAsset;
    public void SetTurnText(int turn)
    {
        textAsset.SetText(turn.ToString());
    }

    public void Awake()
    {
        textAsset = transform.GetComponent<TMP_Text>();
        SetTurnText(2025);
    }


}
