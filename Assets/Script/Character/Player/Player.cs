using MFFrameWork.Utilities;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MFFrameWork
{

    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharactorMove))]
    public class Player : Character_B
    {
        PlayerInput _playerInput;
        public override void Start_S()
        {
            _playerInput = GetComponent<PlayerInput>();


            _playerInput.actions["Move"].performed += x => OnMove(x.ReadValue<Vector2>());
            _playerInput.actions["Move"].canceled += x => CancelMove();
            _playerInput.actions["Jump"].started += x => OnJump();
            _playerInput.actions["Dush"].started += x => OnDush();
            _playerInput.actions["Attack"].started += x => OnAttack();
        }
        private void OnDisable()
        {
            _playerInput.actions["Move"].performed -= x => OnMove(x.ReadValue<Vector2>());
            _playerInput.actions["Move"].canceled -= x => CancelMove();
            _playerInput.actions["Jump"].started -= x => OnJump();
            _playerInput.actions["Dush"].started -= x => OnDush();
            _playerInput.actions["Attack"].started -= x => OnAttack();
        }
    }

    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharactorMove))]
    public abstract class Character_B : MonoBehaviour, IDamageable
    {
        [SerializeField] protected ICharacterMove _playerMove;
        [SerializeField] protected Transform _originTransform;
        [SerializeField] IAttack _attack;
        protected CharactorAnimation _charactorAnimation = new();
        protected Rigidbody _rb;
        protected bool _attackCancel;
        protected Vector3 _moveDirection;
        public Transform _targetPos;

        [SerializeField] Status _status;
        protected int _currentHealth = 10;
        bool _isInvincible;

        CancellationTokenSource _attackCancelToken = new();
        protected int AttackPower { get => _status.AttackPower; }
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _playerMove = GetComponent<ICharacterMove>();
            _attack = GetComponent<IAttack>();
            _charactorAnimation.SetAnimator(GetComponent<Animator>());//ToDo:HERE　コンストラクタで代入するように変更したい


            //パラメーターの初期化
            _currentHealth = _status.MaxHealth;
            if (_currentHealth <= 0)
            {
                DeathBehavior();
            }

            _playerMove.OnGroundChanged += x => _charactorAnimation.SetBool(AnimationPropertys.IsGround, x);

            _playerMove.Init();
            Start_S();
        }
        public virtual void Start_S() { }

        void FixedUpdate()
        {
            _playerMove?.Move(_moveDirection, () => _charactorAnimation.SetFloat(
                AnimationPropertys.MoveSpeed,
                Vector3.Scale(_playerMove.Velocity, new Vector3(1, 0, 1)).magnitude));
            Fixed_S();
        }
        public virtual void Fixed_S() { }
        int IDamageable.HitPoint { get => _currentHealth; }
        void IDamageable.Damage(float damage, Transform hitObjTransform)
        {
            _currentHealth -= (int)damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                DeathBehavior();
            }
        }

        void IDamageable.HealthHeel(float heel)//ToDo:HERE 回復が上限を突破しないように
        {
            Debug.Log($"{this} hit Damage {heel}");
        }

        public void DeathBehavior()
        {
            Destroy(gameObject);
        }
        #region Move
        protected void OnMove(Vector2 moveDirection)
        {
            if ((_playerMove is null).ChackLog("move is null")) return;
            //var cameraRotationY = Quaternion.Euler(0, _originTransform.transform.rotation.eulerAngles.y, 0);
            //_moveDirection = cameraRotationY * new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
            _moveDirection = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        }
        protected void CancelMove()
        {
            if ((_playerMove is null).ChackLog("move is null")) return;
            _moveDirection = Vector3.zero;
        }
        protected void OnJump()
        {
            if ((_playerMove is null).ChackLog("jump is null")) return;
            _playerMove.Jump(() => _charactorAnimation.SetTrigger(AnimationPropertys.JumpTrigger));
        }
        protected void OnDush()
        {
            if ((_playerMove is null).ChackLog("dush is null")) return;
            _playerMove.Dush(_moveDirection, () => _charactorAnimation.SetTrigger(AnimationPropertys.DushTrigger));
        }
        protected void OnAttack()
        {
            if ((_attack is null).ChackLog("Attack is null")) return;
            _attack.OnAttack(_targetPos, AttackPower, _attackCancelToken.Token);
        }
        #endregion

    }
    [Serializable]
    public struct Status
    {
        [SerializeField] int _maxHealth;
        [SerializeField] int _attackPower;
        [SerializeField] float _invincibleTime;
        public int MaxHealth { get => _maxHealth; }
        public int AttackPower { get => _attackPower; }
        public float InvincibleTime { get => _invincibleTime; }
        Status(int maxHealth, int attackPower, float invincibleTime)
        {
            _maxHealth = maxHealth;
            _attackPower = attackPower;
            _invincibleTime = invincibleTime;
        }
    }
    public interface ICharacterMove : IMove, IJump, IDush
    {
        bool IsGround { get; }
        event Action<bool> OnGroundChanged;
        Transform Target { get; set; }
        void Init();
    }
    public interface IMove
    {
        Vector3 Velocity { get; }
        float MoveSpeed { get; set; }
        void Move(Vector3 moveDirection, Action action = null);
    }
    public interface IJump
    {
        float JumpPower { get; set; }
        void Jump(Action action = null);
    }
    public interface IDush
    {
        float DushSpeed { get; set; }
        void Dush(Vector3 moveDirection, Action animatorAction = null);
    }
    public interface IAttack
    {
        void OnAttack(Transform target, float attackPower, CancellationToken token = default);
    }
    public interface IDamageable
    {
        int HitPoint { get; }
        void HealthHeel(float heel);
        void Damage(float damage, Transform hitObjTransform);
        void DeathBehavior();
    }
    interface IBullet
    {
        void InitProperties(Transform target, float attackPower, LayerMask dontIgnoreLayer);
        void InitPhysicsProperties(Vector3 initVelocity, float hitTime, float maxAcceleration);
    }
}