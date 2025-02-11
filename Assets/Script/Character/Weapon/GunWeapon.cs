using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public class GunWeapon : Weapon_B
    {
        [SerializeField] Transform _muzzle;
        [SerializeField] GameObject _bulletPrefab;
        [SerializeField] float _speed = 5;
        [SerializeField] float _lifeTime = 10;

        [SerializeField] float _nullTargetRange = 80;
        protected override void Attack(Transform target, float attackPower, CancellationToken token, System.Action action = null)
        {
            //èâä˙âªèàóù
            var bulletObj = Instantiate(_bulletPrefab, _muzzle.position, Quaternion.identity);
            if (bulletObj.TryGetComponent(out IBullet bullet))
            {
                bullet.Init(attackPower, this.gameObject.layer, _lifeTime);
            }

            //targetPosÇÃnullcheck
            Vector3 targetPos = Camera.main.transform.forward * _nullTargetRange;
            if (target)
            {
                targetPos = target.position;
            }
            OnAttacked?.Invoke(_useStopTime, targetPos);

            var direction = (targetPos - _muzzle.position).normalized;
            
            //ïŒç∑Ç§ÇøÇÃé¿ëï
            if(target && target.parent.TryGetComponent(out Rigidbody targetRb))
            {
                var distanse = Vector3.Distance(_muzzle.position, targetPos);
                var timeToReach = distanse / _speed;
                var n = targetPos + targetRb.linearVelocity * timeToReach;
                direction = Quaternion.LookRotation(n - _muzzle.position) * Vector3.forward * 1.1f;
            }

            //î≠éÀ
            bulletObj.transform.forward = direction;
            if (bulletObj.TryGetComponent(out Rigidbody BulletRb))
            {
                BulletRb.linearVelocity = direction * _speed;
            }

            Debug.DrawLine(_muzzle.position, _muzzle.position + direction * 100, Color.red);
        }
    }
}
