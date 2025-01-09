using UnityEngine;

namespace MFFrameWork
{
    [CreateAssetMenu(menuName = "GameOptions", fileName = "System/GameOptions")]
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
