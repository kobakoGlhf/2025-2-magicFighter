using UnityEngine;

namespace MFFrameWork
{
    public class MissileWeapon : Weapon_B
    {
        [SerializeField] GameObject _bulletObj;
        [SerializeField] Transform _muzzleTransform;

        private void Start()
        {
            if(!_bulletObj.TryGetComponent<IBullet>(out IBullet bullet))
            {
                Debug.LogError("AttackObjにはIBullet継承のオブジェクトをアサインしてください");
                _bulletObj = null;
            }
            if (!_muzzleTransform) _muzzleTransform = transform;
        }
        void Init()
        {

        }
        public override void OnAttack(Transform target, float attackPower, bool cancel)
        {
            if (_bulletObj == null) Debug.Log("AttackObject is null");
            IBullet bullet = Instantiate(_bulletObj, _muzzleTransform.position, _muzzleTransform.rotation).GetComponent<IBullet>();
            bullet.Init(target, attackPower, gameObject.layer);
        }
    }
}
