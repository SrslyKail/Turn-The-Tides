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

        /// <summary>
        /// Searches the scene to see if an object of type T already exists. If it doesn't, it will attempt to load and instanciate a prefab from the given path.
        /// <para>
        /// A prefab is required for this function to work, as all singletons should have a prefab assigned to them.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A class reference to the singleton type.</typeparam>
        /// <param name="prefabPath">A path to the prefab we may need to instanciate.</param>
        /// <returns>A reference to the object of type T; either pre-existing or newly created.</returns>
        public static T FindOrCreateSingleton<T>(string prefabPath) where T: class
        {
            if (FindFirstObjectByType(typeof(T), FindObjectsInactive.Include) is not T found)
            {
                GameObject newObj = Resources.Load(prefabPath) as GameObject;
                GameObject inst = Instantiate(newObj);
                found = inst.GetComponent<T>();
            }

            return found;
        }
    }
}
