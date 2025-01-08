using UnityEngine;

namespace MFFrameWork.Utilities
{
    public static class MySystem
    {
        public static bool ChackLog(this bool value, string message)
        {
            if (!value)
            {
                Debug.LogWarning(message);
            }
            return value;
        }
    }
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T instanse;
        public static T Instanse
        {
            get
            {
                if (instanse == null)
                {
                    instanse = FindFirstObjectByType<T>();
                    (instanse != null).ChackLog($"{nameof(T)} is null");
                }
                return instanse;
            }
        }
        private void Awake()
        {
            SetUpSingleton();
            OnAwake();
        }
        protected virtual void OnAwake() { }
        void SetUpSingleton()
        {
            if( instanse == null)
            {
                instanse = this as T;
                DontDestroyOnLoad(instanse);
            }
            else 
                Destroy(this);
        }
    }
    
}
