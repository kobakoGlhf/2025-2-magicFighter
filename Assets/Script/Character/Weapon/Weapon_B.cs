using UnityEngine;

namespace MFFrameWork
{
    public abstract class Weapon_B : MonoBehaviour, IAttack
    {
        public virtual void OnAttack(Transform _targetPos, float attackPower, bool cancel)
        {
            throw new System.NotImplementedException();
        }
    }
}
