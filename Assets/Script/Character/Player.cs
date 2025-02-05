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
        [SerializeField] float _moveSpeed;
        [SerializeField] float _groundDistanse;
        [SerializeField] float _jumpPower;
        [SerializeField] float _dushPower;
        PlayerInput _playerInput;
        public override void Start_S()
        {
            _playerInput = GetComponent<PlayerInput>();

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
    public abstract class Charactor_B:MonoBehaviour, IDamageable
    {
        [SerializeField] protected ICharactorMove _playerMove;
        protected CharactorAnimation _charactorAnimation = new();
        protected Rigidbody _rb;
        protected IAttack _attack;
        [SerializeField] protected Transform _originTransform;
        protected bool _attackCancel;
        Vector3 _moveDirection;

        [SerializeField] Status _status;
        protected int _currentHealth = 10;
        protected int AttackPower { get => _status.AttackPower; }
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _playerMove = GetComponent<CharactorMove>();
            _charactorAnimation.SetAnimator(GetComponent<Animator>());//ToDo:HERE　コンストラクタで代入するように変更したい
            Start_S();
        }
        public virtual void Start_S() { }

        void FixedUpdate()
        {
            //var cameraRotationY = Quaternion.Euler(0, _originTransform.transform.rotation.eulerAngles.y, 0);
            //var moveSpeed = _playerMove?.Move(cameraRotationY * _moveDirection);
            var moveSpeed = _playerMove?.Move(_moveDirection);
            if (moveSpeed is not null) _charactorAnimation.SetFloat(AnimationPropertys.MoveSpeed, moveSpeed.Value);
            Fixed_S();
        }
        public virtual void Fixed_S() { }
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
            var cameraRotationY = Quaternion.Euler(0, _originTransform.transform.rotation.eulerAngles.y, 0);
            _moveDirection = cameraRotationY * new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
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

}