using MFFrameWork.Utilities;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFFrameWork.MFSystem
{
    public class MainSystem : SingletonMonoBehaviour<MainSystem>
    {
        private AudioManager Audio;

        float timer;
        bool _isPause;

        public event Action OnPause;
        public event Action OnResume;


        public event Action OnGameEnd;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void OnAwake()
        {
            Audio = transform.GetComponentInChildren<AudioManager>();
            if (!Audio) Debug.Log("Audio is null");

            if (SceneManager.GetActiveScene().name == SceneLoader.SceneData[SceneKind.InGame])
            {
                HideCursor(true);
            }
        }

        public void LoadScene(SceneKind sceneKind)
        {
            SceneLoader.LoadScene(sceneKind);
            if (sceneKind == SceneKind.InGame)
            {
                HideCursor(true);
            }
            else
            {
                HideCursor(false);
            }
        }

        public void Pause(bool isPause)
        {
            if (isPause && !_isPause)
            {
                _isPause = true;
                OnPause?.Invoke();
            }
            else if (_isPause)
            {
                _isPause = false;
                OnResume?.Invoke();
            }
        }
        void HideCursor(bool frag)
        {
            Cursor.visible = frag;
            if (!frag)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
