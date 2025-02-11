using MFFrameWork.Utilities;
using UnityEngine;
using UnityEngine.VFX;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : Bullet_B, IMissile
    {
        [SerializeField] Transform _target;
        [SerializeField] float _speed;
        [SerializeField] float _lifeTime = 10;

        [Space]
        [SerializeField] Vector3 _initialVelocity;
        [SerializeField] float _period = 3;
        [SerializeField] float _maxAccleration;
        Vector3 _velocity;
        Vector3 _position;
        Vector3 _targetPos;

        VisualEffect _effect;
        public float LifeTime { get => _lifeTime; set => _lifeTime = value; }
        Rigidbody _rb;

        private async void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _effect = transform.GetComponentInChildren<VisualEffect>();
            await Pausable.PausableDestroy(gameObject, _lifeTime);

        }
        void IMissile.InitPhysicsProperties(Transform target, Vector3 initVelocity, float hitTime, float maxAcceleration)
        {
            _target = target;
            _initialVelocity = initVelocity;
            _period = hitTime + Random.Range(-1, 1f);
            _maxAccleration = maxAcceleration;
            _position = transform.position;
        }
        void IMissile.SetTargetVector(UnityEngine.Vector3 vector)
        {
            _targetPos = vector;
        }

        void Update()
        {
            //‰^“®•û’öŽ®‚ðŽg‚Á‚½missile

            Vector3 accleration = _initialVelocity;
            if (_target)
            {
                _targetPos = _target.position;
            }
            Vector3 diff = _targetPos - transform.position;
            accleration += (diff - _velocity * _period) * 2 / (_period * _period);

            if (accleration.magnitude > _maxAccleration)
            {
                accleration = accleration.normalized * _maxAccleration;
            }

            _period -= Time.deltaTime;

            _velocity += accleration * Time.deltaTime;
            _position += _velocity * Time.deltaTime;
            transform.position = _position;
            //    Vector3 targetVector = _target.position - transform.position;
            //    targetVector.Normalize();

            //    _rb.linearVelocity = targetVector * _speed;
        }

        protected override void OnHitTrigger(IDamageable damageable)
        {
            Debug.Log("hit");
            damageable.Damage(Damage, this.transform);
        }
        protected override async void BulletDestroy()
        {
            try
            {
                enabled = false;
                _effect.SendEvent("OnStopFlash");
                _effect?.Stop();
                while (_effect?.aliveParticleCount > 0)
                {
                    await Awaitable.EndOfFrameAsync();
                }
                base.BulletDestroy();
            }
            catch { }
        }
        protected override void DeathBehavior(float attackPower)
        {
        }
    }
}
