using MFFrameWork.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MFFrameWork
{

    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharactorMove))]
    public class Player : Charactor_B
    {
        GameObject _camera;
        [SerializeField] float _moveSpeed;
        [SerializeField] float _groundDistanse;
        [SerializeField] float _jumpPower;
        [SerializeField] float _dushPower;
        Rigidbody _rb;
        PlayerInput _playerInput;

        ICharactorMove _playerMove;
        IAttack _attack;


        Vector3 _moveDirection;
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _playerMove = GetComponent<CharactorMove>();
            _camera = Camera.main.gameObject;


            _playerMove.OnGroundChanged += (x) => _charactorAnimation.SetBool(AnimationPropertys.IsGround, x);

            _playerInput.actions["Move"].performed += x => OnMove(x);
            _playerInput.actions["Move"].canceled += x => CancelMove(x);
            _playerInput.actions["Jump"].started += x => OnJump(x);
            _playerInput.actions["Dush"].started += x => OnDush(x);
            _playerInput.actions["Attack"].started += x => OnAttack(x);
        }
        private void OnDisable()
        {
            _playerInput.actions["Move"].performed -= x => OnMove(x);
            _playerInput.actions["Move"].canceled -= x => CancelMove(x);
            _playerInput.actions["Jump"].started -= x => OnJump(x);
            _playerInput.actions["Dush"].started -= x => OnDush(x);
            _playerInput.actions["Attack"].started -= x => OnAttack(x);
            _playerMove.OnGroundChanged -= (x) => _charactorAnimation.SetBool(AnimationPropertys.IsGround, x);
        }

    }
    public class Charactor_B:MonoBehaviour, IDamageable
    {
        [SerializeField] ICharactorMove _playerMove;
        protected CharactorAnimation _charactorAnimation = new();
        protected Rigidbody _rb;
        protected IAttack _attack;
        protected Transform _originTransform;
        bool _attackCancel;
        Vector3 _moveDirection;

        [SerializeField] Status _status;
        int _currentHealth = 10;
        int AttackPower { get => _status.AttackPower; }
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _playerMove = GetComponent<CharactorMove>();
            _charactorAnimation.SetAnimator(GetComponent<Animator>());//ToDo:HERE　コンストラクタで代入するように変更したい
        }

        void FixedUpdate()
        {
            var cameraRotationY = Quaternion.Euler(0, _originTransform.transform.rotation.eulerAngles.y, 0);
            var moveSpeed = _playerMove?.Move(cameraRotationY * _moveDirection);

            _charactorAnimation.SetFloat(AnimationPropertys.MoveSpeed, moveSpeed.Value);
        }
        int IDamageable.HitPoint { get => _currentHealth; }
        void IDamageable.Damage(float damage)
        {
            Debug.Log($"{this} hit Damage {damage}");
        }

        void IDamageable.HealthHeel(float heel)//ToDo:HERE 回復が上限を突破しないように
        {
            Debug.Log($"{this} hit Damage {heel}");
        }

        void IDamageable.DeathBehavior()
        {
            Debug.Log($"{this} Death");
        }
        #region Move
        protected void OnMove(InputAction.CallbackContext context)
        {
            if ((_playerMove is null).ChackLog("move is null")) return;
            var moveDirection = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        }
        protected void CancelMove(InputAction.CallbackContext context)
        {
            if ((_playerMove is null).ChackLog("move is null")) return;
            _moveDirection = Vector3.zero;
        }
        protected void OnJump(InputAction.CallbackContext context)
        {
            if ((_playerMove is null).ChackLog("jump is null")) return;
            var jumped = _playerMove.Jump();
            if (jumped) _charactorAnimation.SetTrigger(AnimationPropertys.JumpTrigger);
        }
        protected void OnDush(InputAction.CallbackContext context)
        {
            if ((_playerMove is null).ChackLog("dush is null")) return;
            _playerMove.Dush(_moveDirection, () => { });
            _charactorAnimation.SetTrigger(AnimationPropertys.DushTrigger);
        }
        protected void OnAttack(InputAction.CallbackContext context)
        {
            if ((_attack is null).ChackLog("Attack is null")) return;
            _attack.OnAttack(AttackPower, _attackCancel);
        }
        #endregion

    }
    [Serializable]
    public struct Status
    {
        [SerializeField] int _maxHealth;
        public int MaxHealth { get => _maxHealth; }
        [SerializeField] int _attackPower;
        public int AttackPower { get => _attackPower; }
        Status(int maxHealth, int attackPower)
        {
            _maxHealth = maxHealth;
            _attackPower = attackPower;
        }
    }
    public interface ICharactorMove : IMove, IJump, IDush
    {
        bool IsGround { get; }
        event Action<bool> OnGroundChanged;
    }
    public interface IMove
    {
        float MoveSpeed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <returns>Velocity magnitude</returns>
        float Move(Vector3 moveDirection);
    }
    public interface IJump
    {
        float JumpPower { get; set; }
        bool Jump();
    }
    public interface IDush
    {
        float DushSpeed { get; set; }
        void Dush(Vector3 moveDirection, Action animatorAction);
    }
    public interface IAttack
    {
        int AttackPower { get; set; }
        void OnAttack(float attackPower, bool cancel);
    }
    public interface IDamageable
    {
        int HitPoint { get; }
        void HealthHeel(float heel);
        void Damage(float damage);
        void DeathBehavior();
    }

    public class CharactorAnimation
    {
        static readonly Dictionary<AnimationKind, string> ClipName = new Dictionary<AnimationKind, string>()
    {
        {AnimationKind.Move, "Move" },
        {AnimationKind.Jump, "Jump" },
        {AnimationKind.Attack, "Attack" }
    };
        static readonly Dictionary<AnimationPropertys, string> PropertysName = new Dictionary<AnimationPropertys, string>()
    {
        {AnimationPropertys.MoveSpeed,"MoveSpeed" },
        {AnimationPropertys.IsGround, "IsGround" },
        {AnimationPropertys.AttackTrigger,"AttackTrigger" },
        {AnimationPropertys.DamageTrigger,"DamageTrigger" },
        {AnimationPropertys.JumpTrigger,"JumpTrigger" },
        {AnimationPropertys.DushTrigger,"DushTrigger" },
    };

        Animator _animator;
        public void SetAnimator(Animator animator) => _animator = animator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="changeMod"></param>
        /// <param name="duration">アニメーションの遷移時間。ChangeModeがCrossFadeの時のみ使用される</param>
        public void AnimationChange(AnimationKind kind, ChangeAnimation changeMod, float duration = 1)
        {
            switch (changeMod)
            {
                case ChangeAnimation.Defalt:
                    _animator.Play(ClipName[kind]);
                    break;
                case ChangeAnimation.CrossFade:
                    _animator.CrossFade(ClipName[kind], duration);
                    break;
            }
        }
        public void SetFloat(AnimationPropertys kind, float value)
        {
            _animator.SetFloat(PropertysName[kind], value);
        }
        public void SetTrigger(AnimationPropertys kind)
        {
            _animator.SetTrigger(PropertysName[kind]);
        }
        public void SetBool(AnimationPropertys kind, bool frag)
        {
            _animator.SetBool(PropertysName[kind], frag);
        }
    }
    public enum ChangeAnimation
    {
        Defalt,
        CrossFade
    }
    public enum AnimationKind
    {
        Move,
        Jump,
        Attack
    }
    public enum AnimationPropertys
    {
        MoveSpeed,
        IsGround,

        AttackTrigger,
        DamageTrigger,
        JumpTrigger,
        DushTrigger
    }
}