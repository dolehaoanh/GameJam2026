using UnityEngine;

namespace AC.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        public static T Instance => _instance;
        [SerializeField]protected bool _isDontDestroyAndLoad;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            gameObject.name = typeof(T).FullName;
            _instance = this as T;
            if (_isDontDestroyAndLoad)
                DontDestroyOnLoad(gameObject);
        }
    }
}

