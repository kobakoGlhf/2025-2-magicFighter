using MFFrameWork.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MFFrameWork
{
    public class CharacterMove : MonoBehaviour, ICharacterMove
    {
        [SerializeField] float _slopelimit = 45;
        [SerializeField] float _groundDistans = 0.5f;
        [SerializeField] bool _useCamera;

        //パラメーター
        [Space, Header("Movement parameter")]
        [SerializeField] float _moveSpeed = 10;
        [SerializeField] float _jumpPower = 10;
        [Space]
        [SerializeField] float _dushSpeed = 10;
        [SerializeField] float _dushCoolTime = .5f;
        [SerializeField] float _rotateSpeed = 1;

        [Space]
        [SerializeField] float _landingDelayTime;
        [SerializeField] float _landingPower;

        [Space]
        [SerializeField] int _fryJumpCount;
        int _jumpCount;

        Rigidbody _rb;
        bool _isGround;
        bool _isDisableMove = true;

        CapsuleCollider _capsuleCollider;
        Transform _toRotate;

        CancellationTokenSource _destoryTokenSouce = new();

        public event Action<bool> OnGroundChanged;
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float JumpPower { get => _jumpPower; set => _jumpPower = value; }
        public float DushSpeed { get => _dushSpeed; set => _dushSpeed = value; }

        public Vector3 Velocity { get => _rb.linearVelocity; }
        public bool IsGround
        {
            get => _isGround; set
            {
                if (_isGround != value)
                {
                    _isGround = value;
                    OnGroundChanged?.Invoke(_isGround);
                    Debug.Log("isGround:" + _isGround);
                    if (_isGround && _rb.linearVelocity.y < _landingPower)
                    {
                        UncontrollableSeconds(_landingDelayTime);
                    }
                }
            }
        }
        Transform _lookTarget;
        Vector3 _lookPos;
        Vector3 _moveVector;
        bool _isLooked;
        bool _isAttack;
        float _attackLookTimer;
        public Transform LockTarget
        {
            get => _lookTarget; set
            {
                _lookTarget = value;
                _isLooked = _lookTarget;
            }
        }



        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            if (_useCamera)
            {
                _toRotate = Camera.main.transform;
            }
        }

        void ICharacterMove.Init() { }

        private void Update()
        {
            Debug.DrawRay(transform.position, Vector3.down * _groundDistans, Color.blue);
            GroundCheck();
            LookRotation();

            if (IsGround)
            {
                _jumpCount = _fryJumpCount;
            }

            if (_attackLookTimer > 0)
            {
                _isAttack = true;
                _attackLookTimer -= Time.deltaTime;
            }
            else
            {
                _isAttack = false;
            }

        }
        private void OnDisable()
        {
            _destoryTokenSouce.Cancel();
        }

        void LookRotation()
        {
            Quaternion targetRotation = default;
            //characterの向き変更
            if (!_isLooked)
            {
                targetRotation = _isAttack ? Quaternion.LookRotation(Vector3.Scale(_lookPos, new Vector3(1, 0, 1))) :
                    _moveVector.sqrMagnitude != 0 ? Quaternion.LookRotation(Vector3.Scale(_moveVector, new Vector3(1, 0, 1))) : default;

            }
            else if (_lookTarget)
            {
                targetRotation = Quaternion.LookRotation(Vector3.Scale(_lookTarget.position - transform.position, new Vector3(1, 0, 1)));
            }
            if (targetRotation != default)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
        }
        public void OnAttacked(float time, Vector3 lookPos)
        {
            _lookPos = lookPos - transform.position;
            _attackLookTimer = time;
        }

        void IMove.Move(Vector3 moveDirection, Action<Vector2> action)//ToDo:HERE　RaycastをSphereChastに変更したい 解決
        {
            Debug.DrawRay(transform.position, moveDirection, Color.green);
            if (!_isDisableMove) return;
            if (_useCamera)
            {
                var rotation = Quaternion.Euler(0, _toRotate.eulerAngles.y, 0);
                moveDirection = rotation * moveDirection;
            }
            var moveVector = moveDirection.normalized * _moveSpeed;
            _rb.AddForce(moveVector * 100);
            //移動速度の制限
            if (_rb.linearVelocity.sqrMagnitude > _moveSpeed * _moveSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
            }

            _moveVector = moveVector.normalized;

            if (_lookTarget)
            {
                Vector3 rotateDirection = Quaternion.LookRotation(_lookTarget.position - transform.position) * moveDirection;

                action?.Invoke(new Vector2(transform.forward.x, transform.forward.z));
            }
        }
        void IJump.Jump(Action action)
        {
            if (!_isDisableMove) return;
            else if (_jumpCount != 0)
            {
                _jumpCount--;
                _rb.linearVelocity = Vector3.Scale(_rb.linearVelocity, new Vector3(1, 0, 1));
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }

            if (_isGround)
            {
                action?.Invoke();
            }
        }
        async void IDush.Dush(Vector3 moveDirection, Action animationAction, Action end)
        {
            if (!_isDisableMove || moveDirection.sqrMagnitude == 0) return;

            if (_useCamera)
            {
                var rotation = Quaternion.Euler(0, _toRotate.eulerAngles.y, 0);
                moveDirection = rotation * moveDirection;
            }

            _rb.linearVelocity = Vector3.zero;
            _rb.AddForce(moveDirection.normalized * DushSpeed, ForceMode.Impulse);

            animationAction?.Invoke();

            UncontrollableSeconds(_dushCoolTime, end);
        }
        /// <summary>
        /// worldの-y方向にrayを飛ばす。
        /// </summary>
        /// <returns></returns>
        RaycastHit GroundCheck()
        {
            Debug.DrawRay(transform.position, Vector3.down * _groundDistans, Color.red);

            var colliderRad = _capsuleCollider.radius;

            Vector3 startRay = transform.position;
            startRay.y += colliderRad;//rayの開始位置をずらす

            Ray underRay = new Ray(startRay, Vector3.down);
            IsGround = Physics.SphereCast(underRay, colliderRad, out RaycastHit hit, _groundDistans + colliderRad);

            //地面が指定した角度を超えていたら着地していない判定にする。
            if (Vector3.Dot(hit.normal, Vector3.up) < _slopelimit * Mathf.Deg2Rad)
            {
                IsGround = false;
            }
            return hit;
        }


        /// <summary>
        /// クールタイム分操作不能にする
        /// </summary>
        /// <param name="seconds"></param>
        public async Task UncontrollableSeconds(float seconds, Action end = null)
        {
            Debug.Log("Uncontrolle");
            _isDisableMove = false;
            await Pausable.PausableWaitForSeconds(seconds, _destoryTokenSouce.Token);
            _isDisableMove = true;
            end?.Invoke();
            Debug.Log("controlle");
        }
    }
}
