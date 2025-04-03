
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TurnCounterText : MonoBehaviour
{
    private TMP_Text textAsset;
    public void SetTurnText(int turn)
    {
        transform.GetComponent<TMP_Text>().SetText(turn.ToString());
    }

    public void Awake()
    {
        SetTurnText(2025);
    }


}
