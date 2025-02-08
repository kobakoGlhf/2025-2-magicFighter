using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public abstract class Weapon_B : MonoBehaviour, IAttack
    {
        [SerializeField] float _weaponCoolTime;
        float _lastExecutionTime;
        public void OnAttack(Transform _targetPos, float attackPower, CancellationToken token)
        {
            if (Time.time - _lastExecutionTime > _weaponCoolTime)
            {
                Attack(_targetPos, attackPower, token);
                _lastExecutionTime = Time.time;
            }
            else
            {
                Debug.Log("Unused" + (Time.time - _lastExecutionTime).ToString());
            }
        }

        protected abstract void Attack(Transform _targetPos, float attackPower, CancellationToken token);
    }
}
