﻿using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public class EnemyManager : Character_B
    {

        [SerializeField] Transform _targetObj;
        [SerializeField] float _modeChangeDistanse;
        [SerializeField] float _time;//刈り

        [SerializeField] EnemyState _testState;

        CancellationTokenSource _cancelTSource;
        EnemyState _state;


        float _timer;
        Vector2 _randomDirection;
        public override void Start_S()
        {
            _state = _testState;
            _playerMove.Target = _targetObj;
            _playerMove.MoveSpeed *= 0.5f;
        }
        void EnemyLogicChange()
        {
            if (!_targetObj) _state = EnemyState.Idol;
            else if ((_targetObj.position - transform.position).sqrMagnitude > _modeChangeDistanse * _modeChangeDistanse)
            {
                _state = EnemyState.Tracking;
                _playerMove.MoveSpeed *= 10f;
            }
            else
            {
                _playerMove.MoveSpeed *= 0.1f;
                _state = EnemyState.Attack;
            }
        }

        public void Update()
        {
            switch (_state)
            {
                case EnemyState.Tracking:
                    TrackMode();
                    break;
                case EnemyState.Attack:
                    AttackMode();
                    break;
                default:
                    break;
            }
        }
        void TrackMode()
        {
            var direction = new Vector2(_targetObj.position.x - transform.position.x, _targetObj.position.z - transform.position.z);
            if (direction.sqrMagnitude > 10 * 10) OnMove(direction);
            else CancelMove();

        }
        void AttackMode()
        {
            var toTarget = new Vector2(_targetObj.position.x - transform.position.x, _targetObj.position.z - transform.position.z).normalized;
            //    Debug.Log("reft");
            //    var angle = Mathf.Atan2(direction.y, direction.x) + .2f;
            //    Vector3 circlePosition = new Vector3(
            //        Mathf.Cos(angle) * direction.magnitude,
            //        0,  // y座標は変えない
            //        Mathf.Sin(angle) * direction.magnitude
            //    );
            //    Debug.Log($"MovePos:{circlePosition}");

            //    Vector3 directionToCircle = (circlePosition - transform.position).normalized;
            //    OnMove(new Vector2(circlePosition.x, circlePosition.z));
            Vector2 finalDirection = (_randomDirection * 0.5f).normalized;
            OnMove(finalDirection);

            _timer += Time.deltaTime;

            if (_targetPos && _timer > 1.2f)
            {
                _timer = 0;
                ChangeRandomDirection();
                OnAttack();
            }
        }
        void ChangeRandomDirection()
        {
            // ランダムな方向を設定 (-1〜1の範囲)
            _randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }
    enum EnemyState
    {
        Idol,
        Attack,
        Tracking,
        avoidance
    }

}
