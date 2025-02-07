using UnityEngine;

namespace MFFrameWork
{
    public class MissileWeapon : Weapon_B
    {
        [SerializeField] GameObject _bulletObj;
        [SerializeField] Transform _muzzleTransform;
        [SerializeField] float _tracking;

        [SerializeField] int _missileCount;
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
        protected override void Attack(Transform target, float attackPower, bool cancel)
        {
            if (_bulletObj == null) Debug.Log("AttackObject is null");
            for (var i = 0; i < _missileCount; i++)
            {
                IBullet bullet = Instantiate(_bulletObj, _muzzleTransform.position, _muzzleTransform.rotation).GetComponent<IBullet>();
                bullet.InitProperties(target, attackPower, gameObject.layer);

                Vector3 vec = target.position - transform.position;
                var n = Vector3.Cross(vec, Vector3.up);
                n *= Random.Range(-1f, 1f);
                n.y+= Random.Range(0, 1f)*10;
                bullet.InitPhysicsProperties(n, 2, _tracking);
            }
        }
    }
}
