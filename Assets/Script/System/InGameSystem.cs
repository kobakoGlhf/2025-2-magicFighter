using MFFrameWork.MFSystem;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace MFFrameWork
{
    public class InGameSystem : MonoBehaviour
    {
        [SerializeField] GameObject _player;
        [SerializeField] GameObject _enemy;
        [SerializeField] float _limitTime;
        float _countDownTimer;
        public float CountDownTimer
        {
            get
            {
                return _countDownTimer;
            }
            private set
            {
                _countDownTimer = value;
                OnChangeTimer((int)_countDownTimer);
            }
        }
        public float TimeScore { get => _limitTime - _countDownTimer; }

        PlayableDirector _director;

        public event Action<int> OnChangeTimer;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CountDownTimer = _limitTime;

            _director = GetComponent<PlayableDirector>();

            _player.GetComponent<Character_B>().OnDestory += CallTimeLine;
            _enemy.GetComponent<Character_B>().OnDestory += CallTimeLine;
        }

        // Update is called once per frame
        void Update()
        {
            if (CountDownTimer > 0)
            {
                CountDownTimer -= Time.deltaTime;
            }
            else
            {
                CallTimeLine();
            }
        }
        public void LoadScene()
        {
            MainSystem.Instanse.LoadScene(SceneKind.Title);
        }
        void CallTimeLine()
        {
            _director?.Play();
        }
    }

}
