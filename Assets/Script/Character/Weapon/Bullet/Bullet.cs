using MFFrameWork.Utilities;
using System.Threading;
using UnityEngine;

namespace MFFrameWork
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class Bullet : Bullet_B
    {
        protected override void OnHitTrigger(IDamageable damageable)
        {
            damageable.Damage(Damage, this.transform);
        }
    }

    public abstract class Bullet_B : MonoBehaviour, IBullet
    {
        protected CancellationTokenSource _tokenSource = new();
        LayerMask _excludedLayer;

        float _damage;
        protected float Damage { get => _damage; }

        public async void Init(float damage, LayerMask layer, float lifeTime)
        {
            _damage = damage;
            _excludedLayer = layer;


            await Pausable.PausableWaitForSeconds(lifeTime, _tokenSource.Token,()=> Destroy(gameObject));
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _excludedLayer ||
                other.gameObject.layer == gameObject.layer) { return; }

            if (other.TryGetComponent(out IDamageable damageable))
            {
                OnHitTrigger(damageable);
            }
            DeathBehavior(Damage);
            _tokenSource.Cancel();
            BulletDestroy();
        }
        protected virtual void DeathBehavior(float damage)
        {

        }
        protected virtual void BulletDestroy()
        {
            if(this!=null) Destroy(gameObject);
        }
        protected abstract void OnHitTrigger(IDamageable damageable);
    }
}