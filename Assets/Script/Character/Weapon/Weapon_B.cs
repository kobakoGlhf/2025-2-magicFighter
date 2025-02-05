using UnityEngine;

namespace MFFrameWork
{
    public class Weapon_B : MonoBehaviour, IAttack
    {
        [SerializeField] int _attackPower;
        int IAttack.AttackPower { get => _attackPower; set => _attackPower=value; }

        void IAttack.OnAttack(float attackPower, bool cancel)
        {
            throw new System.NotImplementedException();
        }
    }
}
