using UnityEngine;
using UnityEngine.PlayerLoop;

namespace FarmGame
{
    [RequireComponent(typeof(Initialization))]
    public abstract class Singleton<T> : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Singleton not registered! Make sure the GameObject running your singleton is active in your scene and has an Initialization component attached.");
                    return default;
                }

                return instance;
            }
        }

        [SerializeField] private bool dontDestroyOnLoad;
        private static T instance;
        
        protected virtual void OnRegistration() { }

        public void RegisterSingleton(T _instance)
        {
            instance = _instance;
        }

        protected void Initialize(T _instance)
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (dontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            instance = _instance;
            OnRegistration();
        }
    }
}