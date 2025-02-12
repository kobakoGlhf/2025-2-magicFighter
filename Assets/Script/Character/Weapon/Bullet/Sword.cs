using UnityEngine;

namespace MFFrameWork
{
    public class Sword : MonoBehaviour
    {
        Transform _parent;
        float _attackPower;

        public void Init(float attackPower, Transform parent)
        {
            _attackPower = attackPower;
            _parent = parent;
            RevokeSword();
        }
        public void CallSword()
        {
            gameObject.SetActive(true);
        }
        public void RevokeSword()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable character) && other.transform != _parent)
            {
                character.Damage(_attackPower, _parent);

            }
        }
    }
}
