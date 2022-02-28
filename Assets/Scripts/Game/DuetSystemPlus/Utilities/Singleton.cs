using UnityEngine;

namespace DuetSystem.Utilities
{
    /// <summary>
    /// Allows a class to become globally accessible and be shared by all instances of the class, meaning that any script can access the singleton through its class name, without needing a reference to it first. Only one instance of the class can be spawned in the scene.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance => _instance;

        public static bool IsInitialized
        {
            get { return _instance != null; }
        }

        protected virtual void Awake()
        {
            // Check if another instance of a singleton is active in the scene
            if (_instance != null)
            {
                Debug.LogError("[" + nameof(Singleton<T>) + "] error GGJ0001: Instantiating a second instance of a singleton class is invalid.");
            }
        
            else
            {
                _instance = (T) this;
            }
        }

        protected virtual void OnDestroy()
        {
            // Check if there's an instance of this singleton so another instance of this type can be instantiated again
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
