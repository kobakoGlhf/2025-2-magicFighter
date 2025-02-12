using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MFFrameWork
{
    public class MeleeWeapon : Weapon_B
    {
        Sword _meleeWeapon;
        [Space]

        [SerializeField] float _trackEndDirection;
        [SerializeField] float _maxTrackingTime;
        [SerializeField] float _speed = 10;
        [Space]
        [SerializeField] float _animationStopTime;
        [SerializeField] float _targetOffestY;

        Rigidbody _rb;
        CharacterMove _move;
        public CharacterAnimation _animation;

        Task _attackTask;

        public bool AnimationEnd;

        public event Action OnMeleeAttackEnd;
        private void Awake()
        {
            _meleeWeapon = FindAnyObjectByType<Sword>();
            _meleeWeapon.gameObject.SetActive(false);
        }
        protected override void Start_S()
        {
            _rb = GetComponent<Rigidbody>();
            _move = GetComponent<CharacterMove>();
        }
        protected override void Attack(Transform target, float attackPower, CancellationToken token, Action action)
        {
            if (_attackTask is null || _attackTask.Status != TaskStatus.Running)
            {
                AnimationEnd = false;
                _meleeWeapon.Init(attackPower, this.transform);
                OnAttacked?.Invoke(attackPower, target?target.position:transform.position);
                _attackTask = MeleeAttack(target ? target : transform, token);
                action?.Invoke();
            }
        }

        async Task MeleeAttack(Transform target, CancellationToken token)
        {
            Debug.LogWarning("Attack1");
            Vector3 targetPos;
            _meleeWeapon.CallSword();
            _animation.AnimationChange(AnimationKind.SubAttack, ChangeAnimMode.CrossFade, 0.5f);
            try
            {
                Debug.LogWarning("Attack2");
                var startTime = Time.time;
                while (startTime + _maxTrackingTime > Time.time)
                {
                    Debug.LogWarning("Attackaaaa");
                    targetPos = target.position;
                    targetPos.y += _targetOffestY;
                    if (Vector3.Distance(targetPos, transform.position) < _trackEndDirection)
                    {
                        Debug.LogWarning(target.name);
                        break;
                        Debug.LogWarning("Attack1");

                    }
                    _rb.linearVelocity = (targetPos - transform.position).normalized * _speed;

                    token.ThrowIfCancellationRequested();
                    await Awaitable.EndOfFrameAsync();
                }
                Debug.LogWarning("Attack1");
                _animation.SetTrigger(AnimationPropertys.AttackTrigger);
                while (!_animation.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idol"))
                {
                    token.ThrowIfCancellationRequested();
                    await Awaitable.EndOfFrameAsync();
                }
            }
            catch { }
            finally
            {
                Debug.LogWarning("AttackEnd");
                _meleeWeapon.RevokeSword();
                OnMeleeAttackEnd?.Invoke();
                _attackTask = null;
                AnimationEnd = false;
            }
        }
        public void AttackEnd()
        {
        }
    }
}
