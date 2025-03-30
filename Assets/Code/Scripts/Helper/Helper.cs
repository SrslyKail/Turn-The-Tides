using UnityEngine;

namespace TurnTheTides
{
    //CB: It is **super** important we don't add any reference FROM this assembly, lest we create circular dependencies.

    /// <summary>
    /// Class to hold static functions that are useful across different assemblies.
    /// </summary>
    /// <remarks>
    /// Made by Corey Buchan.
    /// </remarks>
    public class Helper: MonoBehaviour
    {
        /// <summary>
        /// Selects what destroy method to use, depending on if we're in editor or not.
        /// <para>
        /// Because we have live-editor functionality, its important to use this when you want
        /// to delete objects, but can't be sure if the functions will be called at runtime
        /// or in edit mode.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Warning: Do not call this function from any MonoBehaviors "Awake" method.
        ///         This can have unforseen sideeffects as you can't destroy objects 
        ///         during Awake.
        /// </remarks>
        /// <param name="otherObject"></param>
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
