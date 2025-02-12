using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MFFrameWork
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] public Character_B _player;
        [SerializeField] public Character_B _enemy;
        [SerializeField] Weapon_B _mainWeapon;
        [SerializeField] Weapon_B _subWeapon;
        [SerializeField] Weapon_B _skillWeapon;
        [SerializeField] InGameSystem _system;
        [Space]
        [SerializeField] RectTransform _canvas;
        [Space]
        [SerializeField] Text _timer;
        [SerializeField] Slider _playerHpbar;
        [SerializeField] Slider _enemyHpbar;
        [SerializeField] Slider _playerStaminaBar;

        [SerializeField] Slider _mainAttackCoolTime;
        [SerializeField] Slider _subAttackCoolTime;
        [SerializeField] Slider _skillAttackCoolTime;

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

            _skillWeapon.OnAttackCoolBack += () =>
            {
                ChangeSlider(_skillAttackCoolTime, 0, 1);
                SliderTimer(_skillAttackCoolTime, _skillWeapon.WeaponCoolTime, destroyCancellationToken);
            };
            _mainWeapon.OnAttackCoolBack += () =>
            {
                ChangeSlider(_mainAttackCoolTime, 0, 1);
                SliderTimer(_mainAttackCoolTime, _mainWeapon.WeaponCoolTime, destroyCancellationToken);
            };
            _subWeapon.OnAttackCoolBack += () =>
            {
                ChangeSlider(_subAttackCoolTime, 0, 1);
                SliderTimer(_subAttackCoolTime, _subWeapon.WeaponCoolTime, destroyCancellationToken);
            };
            ChangeSlider(_skillAttackCoolTime, 0, 1);
            ChangeSlider(_subAttackCoolTime, 0, 1);
            ChangeSlider(_mainAttackCoolTime, 0, 1);
            SliderTimer(_subAttackCoolTime, _subWeapon.WeaponCoolTime, destroyCancellationToken);
            SliderTimer(_mainAttackCoolTime, _mainWeapon.WeaponCoolTime, destroyCancellationToken);
            SliderTimer(_skillAttackCoolTime, _skillWeapon.WeaponCoolTime, destroyCancellationToken);

            _system.OnChangeTimer += x => _timer.text = x.ToString("00");
        }

        private void Update()
        {
            if (_lockOnTarget)
            {
                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _lockOnTarget.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, screenPos, Camera.main, out Vector2 pos);
                _lockOnCursor.rectTransform.localPosition = pos;
            }
            else
            {
                _lockOnCursor.rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        async void SliderTimer(Slider slider, float maxTime, CancellationToken token = default)
        {
            try
            {
                float timer = 0;
                while (timer < maxTime)
                {
                    token.ThrowIfCancellationRequested();
                    timer += Time.deltaTime;
                    ChangeSlider(slider, timer, maxTime);
                    await Awaitable.EndOfFrameAsync();
                }
            }
            catch { }
        }

        void ChangeSlider(Slider slider, float current, float max)
        {
            var normalize = current / max;
            if (normalize > 1) { normalize = 1; }
            slider.value = normalize;
        }
    }
}
