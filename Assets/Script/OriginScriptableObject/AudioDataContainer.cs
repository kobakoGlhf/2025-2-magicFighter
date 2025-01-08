using UnityEngine;

namespace MFFrameWork.MFSystem
{
    [CreateAssetMenu(menuName = "AudioData", fileName = "Audio/AudioData")]
    public class AudioDataContainer : ScriptableObject
    {
        [SerializeField] AudioData[] _audioData;
        public AudioData[] AudioData { get => _audioData; }
    }
    [System.Serializable]
    public struct AudioData
    {
        [SerializeField] AudioClip _clip;
        [SerializeField] float _volume;
        public AudioClip Clip { get => _clip; }
        public float Volume { get => _volume; }
    }
}
