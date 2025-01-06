using UnityEngine;

namespace MFFrameWork.Utilities
{
    public static class MySystem
    {
        public static bool ChackLog(this bool value,string message)
        {
            if (!value)
            {
                Debug.LogWarning(message);
            }
            return value;
        }
    }
}
