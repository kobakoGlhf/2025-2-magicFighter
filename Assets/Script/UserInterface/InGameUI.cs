using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

namespace MFFrameWork
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField]
        public Character_B _player;
        [SerializeField]
        public Character_B _enemy;

        [SerializeField]
        RectTransform _canvas;

        [SerializeField] Slider _playerHpbar;
        [SerializeField] Slider _enemyHpbar;
        [SerializeField] Slider _playerStaminaBar;

        [SerializeField]
        Image _lockOnCursor;


        Transform _lockOnTarget;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _player.OnChangeHealth += (x, y) => ChangeSlider(_playerHpbar, x, y);
            _enemy.OnChangeHealth += (x, y) => ChangeSlider(_enemyHpbar, x, y);

            _player.OnChangeTarget += x => _lockOnTarget = x;
            _player.OnChangeStamina += (x, y) => ChangeSlider(_playerStaminaBar, x, y);
        }

        private void Update()
        {
            if (_lockOnTarget)
            {
                Debug.Log(_lockOnTarget.name);
                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _lockOnTarget.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, screenPos, Camera.main, out Vector2 pos);
                _lockOnCursor.rectTransform.localPosition = pos;
            }
            else
            {
                _lockOnCursor.rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        void ChangeSlider(Slider slider, float current, float max)
        {
            var normalize = current / max;
            slider.value = normalize;
        }
    }
}
