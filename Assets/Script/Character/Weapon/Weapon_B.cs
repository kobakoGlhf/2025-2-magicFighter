using System;
using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public abstract class Weapon_B : MonoBehaviour, IAttack
    {
        [SerializeField] protected float _useStopTime = .5f;
        [SerializeField] float _weaponCoolTime;
        float _lastExecutionTime;

        public Action<float, Vector3> OnAttacked;
        public void OnAttack(Transform targetPos, float attackPower, CancellationToken token)
        {
            if (Time.time - _lastExecutionTime > _weaponCoolTime)
            {
                Attack(targetPos, attackPower, token);
                _lastExecutionTime = Time.time;
            }
            else
            {
                Debug.Log("Unused" + (Time.time - _lastExecutionTime).ToString());
            }
        }

        protected abstract void Attack(Transform target, float attackPower, CancellationToken token, Action action = null);
    }
}
