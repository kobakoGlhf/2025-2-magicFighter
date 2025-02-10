using MFFrameWork.Utilities;
using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public class GunWeapon : Weapon_B
    {
        [SerializeField] Transform _muzzle;
        [SerializeField] GameObject _bulletPrefab;
        [SerializeField] float _speed=5;
        [SerializeField] float _lifeTime=10;

        [SerializeField] float _nullTargetRange = 80;
        protected override void Attack(Transform _target, float attackPower, CancellationToken token)
        {
            var bulletObj = Instantiate(_bulletPrefab, _muzzle.position, Quaternion.identity);

            if(bulletObj.TryGetComponent(out IBullet bullet))
            {
                bullet.Init(attackPower, this.gameObject.layer, _lifeTime);
            }

            Vector3 target = transform.forward * _nullTargetRange;
            if (_target)
            {
                target = _target.position;
            }
            var direction = (target - _muzzle.position).normalized;
            bulletObj.transform.forward = direction;
            if (bulletObj.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = direction * _speed;
            }
        }
    }
}
