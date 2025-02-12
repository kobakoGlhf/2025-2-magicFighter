using MFFrameWork.Utilities;
using UnityEngine;

namespace MFFrameWork.MFSystem
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField] AudioSource _bgmSource;
        [SerializeField] AudioSource _seSource;

        [Space(10)]
        [SerializeField] AudioDataContainer _BGMData;
        [SerializeField] AudioDataContainer _seData;
        public void PlayBGM(int index)
        {
            var audioData = _BGMData.AudioData[index];
            _bgmSource.clip = audioData.Clip;
            _bgmSource.volume = audioData.Volume;
            _bgmSource.Play();
        }
        public void PlaySE(int index, AudioSource source = null)
        {

            if (index > _seData.AudioData.Length)
                Debug.Log("Audio is Null");

                var audioData = _seData.AudioData[index];
            if(source)
                source.PlayOneShot(audioData.Clip, audioData.Volume);

            else
            _seSource.PlayOneShot(audioData.Clip, audioData.Volume);
        }
    }

}
