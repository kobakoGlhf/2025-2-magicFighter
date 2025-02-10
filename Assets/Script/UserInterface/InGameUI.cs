using UnityEngine;
using UnityEngine.UI;

namespace MFFrameWork
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField]
        public Character_B _player;
        [SerializeField]
        public Character_B _enemy;

        [SerializeField]
        Slider _playerHpbar;
        [SerializeField]
        Slider _enemyHpbar;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _player.OnChangeHealth += (x,y)=> ChangeSlider(_playerHpbar,x,y);
            _enemy.OnChangeHealth += (x, y) => ChangeSlider(_enemyHpbar, x, y);
        }
        void ChangeSlider(Slider slider, float current, float max)
        {
            var normalize = current/max;
            slider.value = normalize;
        }
    }
}
