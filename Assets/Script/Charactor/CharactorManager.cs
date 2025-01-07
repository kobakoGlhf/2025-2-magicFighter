using MFFrameWork.Utilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MFFrameWork.Charactor
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharactorManager : MonoBehaviour
    {
        [SerializeField] CharactorData _charactorData;
        [SerializeField] Move_B _move;

        PlayerInput _playerInput;
        CharactorStats _stats;

        Action MoveAction;
        Action Attack;
        Action Fire1;
        Action Fire2;
        Action Jump;
        Action Dush;
        private void Start()
        {
            Init();
        }
        private void Init()
        {
            _move.SetRigidbody(GetComponent<Rigidbody>());
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.ActivateInput();
            if ((_charactorData == null).ChackLog("CharactorData is null"))
                _stats = new CharactorStats();
        }
        private void FixedUpdate()
        {
            var moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();
            _move.Move(moveInput);
        }
        //void OnMove(InputValue value)
        //{
        //    var vector = value.Get<Vector2>();
        //    _move.Move(vector);
        //    MoveAction?.Invoke();
        //}

        void OnJump()
        {
            _move.Jump();
            Jump?.Invoke();
        }
        void OnDush()
        {
            //_move.Dush(vector);
            Dush?.Invoke();
        }
    }
    enum MoveType
    {
        Defalt,
        LockTarget
    }
    public class CharactorStats
    {
        int _hitPoint = 100;
        public int HitPoint { get => _hitPoint; }
        int _energy = 100;
        public int Energy { get => _energy; }

        public event Action deathCallBack;

        public CharactorStats() { }

        public CharactorStats(int hitPoint, int energy)
        {
            _hitPoint = hitPoint;
            _energy = energy;
        }

        void Damage(int damage)
        {
            //É_ÉÅÅ[ÉWèàóù
            if (_hitPoint < 0)
                deathCallBack?.Invoke();
        }

        void UseEnergy(int useEnergy, Action action)
        {

        }
    }
}
