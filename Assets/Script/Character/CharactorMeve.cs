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

        Rigidbody _rb;
        bool _isGround;
        bool _isDisableMove = true;

        CapsuleCollider _capsuleCollider;
        Transform _toRotate;
        Transform _target;

        CancellationTokenSource _destoryTokenSouce = new();

        public event Action<bool> OnGroundChanged;

        public Transform Target { get => _target; set => _target = value; }
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
        }
        private void OnDisable()
        {
            _destoryTokenSouce.Cancel();
        }

        void IMove.Move(Vector3 moveDirection, Action action)//ToDo:HERE　RaycastをSphereChastに変更したい 解決
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

            //characterの向き変更
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Target == null ?
                    Quaternion.LookRotation(moveDirection) :
                    Quaternion.LookRotation(Target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
            }

            action?.Invoke();
        }
        void IJump.Jump(Action action)
        {
            if (!_isDisableMove || !IsGround) return;
            _rb.linearVelocity = Vector3.Scale(_rb.linearVelocity, new Vector3(1, 0, 1));
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            action?.Invoke();
        }
        async void IDush.Dush(Vector3 moveDirection, Action animationAction)
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

            UncontrollableSeconds(_dushCoolTime);
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
        public async Task UncontrollableSeconds(float seconds)
        {
            Debug.Log("Uncontrolle");
            _isDisableMove = false;
            await Pausable.PausableWaitForSeconds(seconds, _destoryTokenSouce.Token);
            _isDisableMove = true;
            Debug.Log("controlle");
        }
    }
}
