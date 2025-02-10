using MFFrameWork.Utilities;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharacterMove))]
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
            _playerInput.actions["MainAttack"].started += x => OnAttack();
            _playerInput.actions["SubAttack"].started += x => OnSubAttack();
            _playerInput.actions["SkillAttack"].started += x => OnSkillAttack();
        }
        private void OnDisable()
        {
            _playerInput.actions["Move"].performed -= x => OnMove(x.ReadValue<Vector2>());
            _playerInput.actions["Move"].canceled -= x => CancelMove();
            _playerInput.actions["Jump"].started -= x => OnJump();
            _playerInput.actions["Dush"].started -= x => OnDush();
            _playerInput.actions["MainAttack"].started -= x => OnAttack();
            _playerInput.actions["SubAttack"].started -= x => OnSubAttack();
            _playerInput.actions["SkillAttack"].started -= x => OnSkillAttack();
        }
    }

    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharacterMove))]
    public abstract class Character_B : MonoBehaviour, IDamageable
    {
        [SerializeField] protected ICharacterMove _playerMove;
        [SerializeField] protected Transform _originTransform;
        [SerializeField] Weapon_B _mainAttack;
        [SerializeField] Weapon_B _subAttack;
        [SerializeField] Weapon_B _skillAttack;
        protected CharacterAnimation _charactorAnimation = new();
        protected Rigidbody _rb;
        protected bool _attackCancel;
        protected Vector3 _moveDirection;
        private Transform _targetTransform;
        CancellationTokenSource _attackCancelToken = new();

        [SerializeField] Status _status;
        protected int _currentHealth = 10;
        bool _isInvincible;


        public Action<float, float> OnChangeHealth;
        public Action<Transform> OnChangeTarget;

        protected int AttackPower { get => _status.AttackPower; }
        public Transform TargetTransform
        {
            get => _targetTransform;
            set
            {
                if (_targetTransform != value)
                {
                    _targetTransform = value;
                    OnChangeTarget?.Invoke(_targetTransform);
                    Debug.Log("TargetName : " + (_targetTransform is not null ? _targetTransform.name : "is null"));
                }
            }
        }
        public int CurrentHealth
        {
            get => _currentHealth; set
            {
                Debug.Log($"HP:{_currentHealth}");
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    OnChangeHealth?.Invoke(_currentHealth, _status.MaxHealth);
                }
            }
        }
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _playerMove = GetComponent<ICharacterMove>();
            _charactorAnimation.SetAnimator(GetComponent<Animator>());//ToDo:HERE　コンストラクタで代入するように変更したい


            //パラメーターの初期化
            CurrentHealth = _status.MaxHealth;
            if (CurrentHealth <= 0)
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
        int IDamageable.HitPoint { get => CurrentHealth; }
        void IDamageable.Damage(float damage, Transform hitObjTransform)
        {
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                DeathBehavior();
            }
            else
            {
                CurrentHealth -= (int)damage;
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
            if ((_mainAttack is null).ChackLog("Attack is null")) return;
            _mainAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token);
        }

        protected void OnSubAttack()
        {
            if ((_subAttack is null).ChackLog("Attack is null")) return;
            _subAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token);
        }

        protected void OnSkillAttack()
        {
            if ((_skillAttack is null).ChackLog("Attack is null")) return;
            _skillAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token);
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
        void Init(float damage, LayerMask layer, float lifeTime);
    }

    interface IMissile : IBullet
    {
        void SetTargetVector(Vector3 vector);
        void InitPhysicsProperties(Transform target, Vector3 initVelocity, float hitTime, float maxAcceleration);
    }
}