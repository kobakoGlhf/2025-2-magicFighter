using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFFrameWork
{
    public class SceneLoader
    {
        public static readonly Dictionary<SceneKind, string> SceneData = new Dictionary<SceneKind, string>()
        {
            {SceneKind.Title, "TitleScene" },
            {SceneKind.InGame, "InGameScene" },
        };

        public static void LoadScene(SceneKind sceneKind)
        {
            SceneManager.LoadScene(SceneData[sceneKind]);
        }
    }
    public enum SceneKind
    {
        Title,
        InGame,
    }
}
