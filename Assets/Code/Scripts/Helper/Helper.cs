using UnityEngine;

namespace TurnTheTides
{
    public class Helper: MonoBehaviour
    {
        public static void SmartDestroy(GameObject otherObject)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(otherObject);
            }
            else
            {
                Destroy(otherObject);
            }
        }
    }
}
