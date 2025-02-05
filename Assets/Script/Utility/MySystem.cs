using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MFFrameWork.Utilities
{
    public static class MFUtility
    {
        public static bool ChackLog(this bool value, string message)
        {
            if (value)
            {
                Debug.LogWarning(message);
            }
            return value;
        }
        public static void ChackLogAction(Action action, string message)
        {
            action();
            Debug.Log(message);
        }
    }
    public class Pausable
    {
        static bool _isPause;
        public static void Pause()=>_isPause = true.ChackLog("Paused");
        public static void Resume() => _isPause = false.ChackLog("Resumed");
        public async static Task PausableWaitForSeconds(float waitSeconds)
        {
            float timer = 0;
            while (waitSeconds > timer)
            {
                timer += Time.deltaTime;
                await Awaitable.EndOfFrameAsync();
            }
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
                Transform parent=instanse.transform;
                while(parent.parent != null)
                {
                    parent = parent.parent;
                }
                DontDestroyOnLoad(parent);
            }
            else 
                Destroy(this);
        }
    }
    
}
