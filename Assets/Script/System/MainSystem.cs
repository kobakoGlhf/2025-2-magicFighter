using MFFrameWork.Utilities;
using System;
using UnityEngine;

namespace MFFrameWork.MFSystem
{
    public class MainSystem : SingletonMonoBehaviour<MainSystem>
    {
        float timer;
        bool _isPause;

        public event Action OnPause;
        public event Action OnResume;

        
        public event Action OnGameEnd;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!_isPause) timer += Time.deltaTime;
        }
        public void Pause(bool isPause)
        {
            if (isPause&&!_isPause)
            {
                _isPause = true;
                OnPause?.Invoke();
            }
            else if(_isPause)
            {
                _isPause = false;
                OnResume?.Invoke();
            }
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
}
