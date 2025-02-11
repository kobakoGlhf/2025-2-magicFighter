using MFFrameWork.MFSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace MFFrameWork
{
    public class InGameSystem : MonoBehaviour
    {
        [SerializeField] GameObject _player;
        [SerializeField] GameObject _enemy;

        PlayableDirector _director;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _director = GetComponent<PlayableDirector>();

            _player.GetComponent<Character_B>().OnDestory += CallTimeLine;
            _enemy.GetComponent<Character_B>().OnDestory += CallTimeLine;
        }

        // Update is called once per frame
        void Update()
        {

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
