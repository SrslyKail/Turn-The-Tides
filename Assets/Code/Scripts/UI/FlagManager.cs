using UnityEngine;
using TMPro;

/// <summary>
/// Manages (glues) text flags to game objects (hex tiles
/// </summary>
public class TextFlagManager : MonoBehaviour
{
    public static void AttachTextFlag(GameObject targetObject, string message, GameObject textFlagPrefab)
    {
        // Create the flag and parent it to the target
        GameObject flag = Instantiate(textFlagPrefab);
        flag.transform.SetParent(targetObject.transform, false);

        // Offset above the object
        flag.transform.localPosition = new Vector3(0, 1f, 0); // adjust height if needed

        // Set the text
        TextMeshPro text = flag.GetComponent<TextMeshPro>();
        if (text != null)
        {
            text.text = message;
        }
    }
}
