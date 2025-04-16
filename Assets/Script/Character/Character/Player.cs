using MFFrameWork.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            _playerInput.actions["MainAttack"].performed += x => OnAttack();
            _playerInput.actions["SubAttack"].started += x => OnSubAttack();
            _playerInput.actions["SkillAttack"].started += x => OnSkillAttack();
            _playerInput.actions["LookOn"].started += x => OnLookTarget();
        }
        private void OnDisable()
        {
            _playerInput.actions["Move"].performed -= x => OnMove(x.ReadValue<Vector2>());
            _playerInput.actions["Move"].canceled -= x => CancelMove();
            _playerInput.actions["Jump"].started -= x => OnJump();
            _playerInput.actions["Dush"].started -= x => OnDush();
            _playerInput.actions["MainAttack"].performed -= x => OnAttack();
            _playerInput.actions["SubAttack"].started -= x => OnSubAttack();
            _playerInput.actions["SkillAttack"].started -= x => OnSkillAttack();
            _playerInput.actions["LookOn"].started -= x => OnLookTarget();
        }
    }

    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(CharacterMove))]
    public abstract class Character_B : MonoBehaviour, IDamageable
    {
        [SerializeField] protected ICharacterMove _characterMove;
        [SerializeField] protected Transform _originTransform;
        [SerializeField] Weapon_B _mainAttack;
        [SerializeField] MeleeWeapon _subAttack;
        [SerializeField] Weapon_B _skillAttack;
        protected CharacterAnimation _charactorAnimation = new();
        protected Rigidbody _rb;
        protected bool _attackCancel;
        protected Vector3 _moveDirection;
        private Transform _targetTransform;
        CancellationTokenSource _attackCancelToken = new();
        bool _isLookTarget;

        [SerializeField] public Status _status;
        protected int _currentHealth = 10;
        bool _isInvincible;

        [SerializeField] float _currentStamina;
        [SerializeField] float _dushStamina = 60;
        [SerializeField, Tooltip("1秒ごとに*50回復(fixed)")] float _staminaRegeneration = 1;
        [SerializeField] bool _isStaminaRegene = true;


        [SerializeField]
        float _dushCoolTime = .5f;

        bool _isDisableMove;
        bool _isDisableInput;

        [SerializeField] float _hitTime;

        Task _disMoveTask;
        Task _disInputTask;

        public Action<float, float> OnChangeHealth;
        public Action<Transform> OnChangeTarget;
        public Action<float, float> OnChangeStamina;
        public event Action OnDestory;

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
                    if (_targetTransform is null)
                    {
                        RemoveLookTarget();
                    }
                }
            }
        }
        public int CurrentHealth
        {
            get => _currentHealth; set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    OnChangeHealth?.Invoke(_currentHealth, _status.MaxHealth);
                }
            }
        }

        public float CurrentStamina
        {
            get => _currentStamina;
            set
            {
                if (_currentStamina != value)
                {
                    _currentStamina = value;
                    OnChangeStamina?.Invoke(_currentStamina, _status.Stamina);
                }
            }
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _characterMove = GetComponent<ICharacterMove>();
            _charactorAnimation.SetAnimator(GetComponent<Animator>());//ToDo:HERE　コンストラクタで代入するように変更したい


            //パラメーターの初期化
            CurrentHealth = _status.MaxHealth; _currentStamina = _status.Stamina;
            if (CurrentHealth <= 0)
            {
                DeathBehavior();
            }

            _characterMove.OnGroundChanged += x => _charactorAnimation.SetBool(AnimationPropertys.IsGround, x);

            if (_mainAttack) _mainAttack.OnAttacked += _characterMove.OnAttacked;
            if (_skillAttack) _skillAttack.OnAttacked += _characterMove.OnAttacked;
            if (_subAttack)
            {
                _subAttack.OnAttacked += _characterMove.OnAttacked;
                _subAttack.OnMeleeAttackEnd += () => _isDisableMove = false;
                _subAttack._animation = _charactorAnimation;
            }

            _characterMove.Init();
            Start_S();
        }
        public virtual void Start_S() { }

        void FixedUpdate()
        {
            Debug.Log(_attackCancelToken);
            _charactorAnimation.SetFloat(
                AnimationPropertys.MoveSpeed,
                Vector3.Scale(_characterMove.Velocity, new Vector3(1, 0, 1)).magnitude);

            if (_isDisableMove || _isDisableInput) return;
            _characterMove?.Move(_moveDirection, vector =>
            {
                _charactorAnimation.SetFloat(AnimationPropertys.MoveX, _moveDirection.x);
                _charactorAnimation.SetFloat(AnimationPropertys.MoveY, _moveDirection.z);
            });
            if (_status.Stamina >= _currentStamina && _isStaminaRegene)
                CurrentStamina += _staminaRegeneration;

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
                _charactorAnimation.SetTrigger(AnimationPropertys.DamageTrigger);
            }
            if (damage > 5)
                _disInputTask = UncontrollableSeconds(_hitTime);
        }

        void IDamageable.HealthHeel(float heel)//ToDo:HERE 回復が上限を突破しないように
        {
            Debug.Log($"{this} hit Damage {heel}");
        }

        public void DeathBehavior()
        {
            OnDestory?.Invoke();
            Destroy(gameObject);
        }
        #region Action
        protected void OnMove(Vector2 moveDirection)
        {
            if ((_characterMove is null).ChackLog("move is null")) return;
            //var cameraRotationY = Quaternion.Euler(0, _originTransform.transform.rotation.eulerAngles.y, 0);
            //_moveDirection = cameraRotationY * new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
            _moveDirection = new Vector3(moveDirection.x, 0, moveDirection.y).normalized;
        }
        protected void CancelMove()
        {
            if ((_characterMove is null).ChackLog("move is null")) return;
            _moveDirection = Vector3.zero;
        }
        protected void OnJump()
        {
            if ((_characterMove is null).ChackLog("jump is null")) return;
            else if (_isDisableMove || _isDisableInput) return;
            _characterMove.Jump(() => _charactorAnimation.SetTrigger(AnimationPropertys.JumpTrigger));
        }
        protected void OnDush()
        {
            if ((_characterMove is null).ChackLog("dush is null")) return;
            else if (_isDisableInput) return;
            else if (CurrentStamina > _dushStamina && _moveDirection.sqrMagnitude != 0)
            {
                _attackCancelToken.Cancel();
                _attackCancelToken.Dispose();
                _attackCancelToken = new();

                _isStaminaRegene = false;
                _characterMove.Dush(_moveDirection, () => _charactorAnimation.SetTrigger(AnimationPropertys.DushTrigger),
                    () =>
                    {
                        _isStaminaRegene = true;
                    });
                CurrentStamina -= _dushStamina;

                _disInputTask = UncontrollableSeconds(_dushCoolTime);
                _disMoveTask = UnMovebleSeconds(_dushCoolTime);
            }
        }
        protected void OnAttack()//ToDo:HERE ここのアタック関係まとめる
        {
            if ((_mainAttack is null).ChackLog("Attack is null") || _isDisableInput) return;
            else if (_isDisableMove) return;
            _mainAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token);
        }

        protected void OnSubAttack()
        {
            if ((_subAttack is null).ChackLog("Attack is null") || _isDisableInput) return;
            else if (_isDisableMove) return;
            Debug.Log("Attack1");
            _subAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token, () => _isDisableMove = true);
        }

        protected void OnSkillAttack()
        {
            if ((_skillAttack is null).ChackLog("Attack is null") || _isDisableInput) return;
            else if (_isDisableMove) return;
            _skillAttack.OnAttack(_targetTransform, AttackPower, _attackCancelToken.Token);
        }

        protected void OnLookTarget()
        {
            if (_targetTransform && !_isLookTarget)
            {
                _characterMove.LockTarget = _targetTransform;
                _isLookTarget = true;
                _charactorAnimation.SetBool(AnimationPropertys.IsLookMode, true);
            }
            else if (_isLookTarget)
            {
                Debug.Log("LookCancel");
                _isLookTarget = false;
                _characterMove.LockTarget = null;
                _charactorAnimation.SetBool(AnimationPropertys.IsLookMode, false);
            }
        }
        protected void RemoveLookTarget()
        {
            _characterMove.LockTarget = null;
            _charactorAnimation.SetBool(AnimationPropertys.IsLookMode, false);
        }
        #endregion
        /// <summary>
        /// クールタイム分操作不能にする
        /// </summary>
        /// <param name="seconds"></param>
        public async Task UncontrollableSeconds(float seconds, Action end = null)
        {
            _isDisableInput = true;
            await Pausable.PausableWaitForSeconds(seconds);
            _isDisableInput = false;
            end?.Invoke();
        }
        public async Task UnMovebleSeconds(float seconds, Action end = null)
        {
            _isDisableMove = true;
            await Pausable.PausableWaitForSeconds(seconds);
            _isDisableMove = false;
            end?.Invoke();
        }
    }
    [Serializable]
    public struct Status
    {
        [SerializeField] int _maxHealth;
        [SerializeField] int _attackPower;
        [SerializeField] float _invincibleTime;
        [SerializeField] float _stamina;
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int AttackPower { get => _attackPower; set => _attackPower = value; }
        public float InvincibleTime { get => _invincibleTime; set => _invincibleTime = value; }
        public float Stamina { get => _stamina; set => _stamina = value; }
        Status(int maxHealth, int attackPower, float invincibleTime, float stamina)
        {
            _maxHealth = maxHealth;
            _attackPower = attackPower;
            _invincibleTime = invincibleTime;
            _stamina = stamina;
        }
    }
    public interface ICharacterMove : IMove, IJump, IDush
    {
        bool IsGround { get; }
        event Action<bool> OnGroundChanged;
        void OnAttacked(float time, Vector3 target);
        Transform LockTarget { get; set; }
        void Init();
    }
    public interface IMove
    {
        Vector3 Velocity { get; }
        float MoveSpeed { get; set; }
        void Move(Vector3 moveDirection, Action<Vector2> action = null);
    }
    public interface IJump
    {
        float JumpPower { get; set; }
        void Jump(Action action = null);
    }
    public interface IDush
    {
        float DushSpeed { get; set; }
        void Dush(Vector3 moveDirection, Action animatorAction = null, Action dushEndAction = null);
    }
    public interface IAttack
    {
        void OnAttack(Transform target, float attackPower, CancellationToken token = default, Action action = default);
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