using MFFrameWork.MFSystem;
using System;
using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public abstract class Weapon_B : MonoBehaviour, IAttack
    {
        [SerializeField] protected float _useStopTime = .5f;
        [SerializeField] float _weaponCoolTime;
        [SerializeField] float _damageScale = 1;
        [SerializeField] int _audioID;
        float _lastExecutionTime;

        AudioSource _audioSource;

        public float WeaponCoolTime
        {
            get => _weaponCoolTime;
        }
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            Debug.Log(_audioSource);
        }
        protected virtual void Start_S()
        {

        }

        public Action OnAttackCoolBack;
        public Action<float, Vector3> OnAttacked;
        public void OnAttack(Transform targetPos, float attackPower, CancellationToken token = default, Action action = default)
        {
            attackPower *= _damageScale;
            if (Time.time - _lastExecutionTime > _weaponCoolTime)//クールタイムの確認
            {
                Attack(targetPos, attackPower, token, action);
                _lastExecutionTime = Time.time;
                OnAttackCoolBack?.Invoke();

                if (_audioSource)
                    AudioManager.Instanse.PlaySE(_audioID, _audioSource);
            }
            else
            {
                Debug.Log("Unused" + (Time.time - _lastExecutionTime).ToString());
            }
        }

        protected abstract void Attack(Transform target, float attackPower, CancellationToken token, Action action = null);
    }
}
