using MFFrameWork.Utilities;
using System;
using UnityEngine;

namespace MFFrameWork
{
    public class CharactorMove : MonoBehaviour, ICharactorMove
    {
        //全てのインターフェースで共通の部分
        Rigidbody _rb;
        bool _isGround;
        bool _isDisableMove = true;


        public bool IsGround
        {
            get => _isGround; set
            {
                if (_isGround != value)
                {
                    _isGround = value;
                    OnGroundChanged?.Invoke(_isGround);
                }
            }
        }

        [SerializeField] float _slopelimit = 45;
        [SerializeField] float _groundDistans = 0.5f;
        //パラメーター
        [Space, Header("Movement parameter")]
        [SerializeField] float _moveSpeed = 10;
        [SerializeField] float _jumpPower = 10;
        [Space]
        [SerializeField] float _dushSpeed = 10;
        [SerializeField] float _dushCoolTime = .5f;
        [SerializeField] float _rotateSpeed = 1;

        public event Action<bool> OnGroundChanged;

        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float JumpPower { get => _jumpPower; set => _jumpPower = value; }
        public float DushSpeed { get => _dushSpeed; set => _dushSpeed = value; }


        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            Debug.DrawRay(transform.position, Vector3.down * _groundDistans, Color.blue);
        }

        public void Init(Rigidbody rb, float moveSpeed = 10, float distanse = 1,
            float jumpPower = 100, float dushPower = 100)
        {
            _rb = rb;
            _moveSpeed = moveSpeed;
            _groundDistans = distanse;
            _jumpPower = jumpPower;
            _dushSpeed = dushPower;
        }

        float IMove.Move(Vector3 moveDirection)//ToDo:HERE　RaycastをSphereChastに変更したい
        {
            if (!_isDisableMove) return 0;
            moveDirection = moveDirection.normalized;
            Debug.DrawRay(transform.position, moveDirection, Color.green);
            //Ray moveRay = new Ray(transform.position, moveDirection);
            var groundHit = GroundCheck();

            var moveVector = moveDirection * _moveSpeed;
            _rb.AddForce(moveVector * 100);
            //移動速度の制限
            if (_rb.linearVelocity.sqrMagnitude > _moveSpeed * _moveSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
            }

            //characterの向き変更
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
            }
            //_rb.linearVelocity = moveVector;
            return _rb.linearVelocity.magnitude;
        }
        bool IJump.Jump()
        {
            if (!_isDisableMove || !IsGround) return false;
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            return true;
        }
        async void IDush.Dush(Vector3 moveDirection, Action animationAction)
        {
            if (!_isDisableMove) return;
            else if (moveDirection.sqrMagnitude == 0) return;
            animationAction?.Invoke();

            _rb.linearVelocity = Vector3.zero;
            _rb.AddForce(moveDirection.normalized * DushSpeed, ForceMode.Impulse);

            _isDisableMove = false;
            await Pausable.PausableWaitForSeconds(_dushCoolTime);
            _isDisableMove = true;
        }
        /// <summary>
        /// worldの-y方向にrayを飛ばす。
        /// </summary>
        /// <returns></returns>
        RaycastHit GroundCheck()//ToDo:HERE 接地判定をスフィアキャストなどできれいにとりたい。
        {
            Debug.DrawRay(transform.position, Vector3.down * _groundDistans, Color.blue);
            Ray underRay = new Ray(transform.position, transform.up * -1);
            IsGround = Physics.Raycast(underRay, out RaycastHit groundHit, _groundDistans);
            if (Vector3.Dot(groundHit.normal, Vector3.up) < _slopelimit * Mathf.Deg2Rad)
            {
                IsGround = false;
            }
            return groundHit;
        }
    }
}
