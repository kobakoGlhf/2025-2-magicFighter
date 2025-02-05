using UnityEngine;

namespace MFFrameWork
{
    public abstract class Weapon_B : MonoBehaviour, IAttack
    {
        [SerializeField] int _attackPower;
        [SerializeField] Transform _targetPos;
        int IAttack.AttackPower { get => _attackPower; set => _attackPower=value; }

        void IAttack.OnAttack(float attackPower, bool cancel)
        {
            throw new System.NotImplementedException();
        }
    }
}
