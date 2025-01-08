using MFFrameWork.Utilities;
using UnityEngine;

namespace MFFrameWork.MFSystem
{
    public class MainSystem : SingletonMonoBehaviour<MainSystem>
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void HideCursor(bool frag)
        {
            Cursor.visible = frag;
            if (frag)
                Cursor.lockState = CursorLockMode.None;
            else 
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
    [CreateAssetMenu(menuName = "GameOptions",fileName = "System/GameOptions")]
    public class Options : ScriptableObject
    {
        [SerializeField] InGameOption _inGameOption;
        [SerializeField] OutGameOption _outGameOption;
        public InGameOption InGameOption { get => _inGameOption; }
        public OutGameOption OutGameOption { get => _outGameOption; }
    }
    [System.Serializable]
    public struct InGameOption
    {
        float _mouseSensitivity;
    }
    [System.Serializable]
    public struct OutGameOption
    {
        float _allAudioVolume;
        float _bgmAudioVolume;
        float _seAudioVolume;
    }
}
