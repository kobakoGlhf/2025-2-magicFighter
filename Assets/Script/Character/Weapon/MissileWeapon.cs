using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    public class MissileWeapon : Weapon_B
    {
        [SerializeField] GameObject _bulletObj;
        [SerializeField] Transform _muzzleTransform;
        [SerializeField] float _tracking;

        [SerializeField] float _lifeTime = 10;
        [SerializeField] float _hitTime = 2;

        [SerializeField] int _missileCount;

        [SerializeField] float _nullTargetRange=80;

        Vector3 _targetPos;
        private void Start()
        {
            if (!_bulletObj.TryGetComponent(out IMissile bullet))
            {
                Debug.LogError("AttackObjにはIBullet継承のオブジェクトをアサインしてください");
                _bulletObj = null;
            }
            if (!_muzzleTransform) _muzzleTransform = transform;
        }
        void Init()
        {

        }
        protected override void Attack(Transform target, float attackPower, CancellationToken token)
        {
            if (_bulletObj == null) Debug.Log("AttackObject is null");
            for (var i = 0; i < _missileCount; i++)
            {
                IMissile bullet = Instantiate(_bulletObj, _muzzleTransform.position, _muzzleTransform.rotation).GetComponent<IMissile>();
                bullet.Init(attackPower, gameObject.layer, _lifeTime);

                if (target)
                {
                    OnAttacked?.Invoke(_useStopTime, target.position);
                    _targetPos = target.position;
                }
                else
                {
                    OnAttacked?.Invoke(_useStopTime, transform.forward * _nullTargetRange);
                    bullet.SetTargetVector(transform.forward * _nullTargetRange);
                }

                Vector3 vec = _targetPos - transform.position;
                var n = Vector3.Cross(vec, Vector3.up);
                n *= Random.Range(-1f, 1f);
                n.y += Random.Range(0, 1f) * 10;

                bullet.InitPhysicsProperties(target, n, 2, _tracking);
            }
        }
    }
}
