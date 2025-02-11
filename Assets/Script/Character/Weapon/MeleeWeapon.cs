using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MFFrameWork
{
    public class MeleeWeapon : Weapon_B
    {
        [SerializeField] float _trackEndDirection;
        [SerializeField] float _maxTrackingTime;
        [SerializeField] float _speed = 10;
        [Space]
        [SerializeField] float _animationStopTime;

        Rigidbody _rb;
        CharacterMove _move;

        Task _attackTask;

        public event Action OnMeleeAttackEnd;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _move = GetComponent<CharacterMove>();
        }
        protected override async void Attack(Transform target, float attackPower, CancellationToken token, Action action)
        {
            if (_attackTask is null || _attackTask.Status != TaskStatus.Running)
                _attackTask = MeleeAttack(target, token);
        }

        async Task MeleeAttack(Transform target, CancellationToken token)
        {
            var startTime = Time.time;
            while (startTime + _maxTrackingTime > Time.time)
            {
                if (Vector3.Distance(target.position, transform.position) < _trackEndDirection)
                {
                    break;
                }
                else _rb.linearVelocity = (target.position - transform.position).normalized * _speed;

                await Awaitable.EndOfFrameAsync();
            }
            OnMeleeAttackEnd?.Invoke();
        }
    }
}
