using MFFrameWork.Utilities;
using UnityEngine;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : Bullet_B, IMissile
    {
        [SerializeField] Transform _target;
        [SerializeField] float _speed;
        [SerializeField] float _lifeTime=10;

        [Space]
        [SerializeField] Vector3 _initialVelocity;
        [SerializeField] float _period = 3;
        [SerializeField] float _maxAccleration;
        Vector3 _velocity;
        Vector3 _position;

        [SerializeField] GameObject _effect;
        public float LifeTime { get => _lifeTime; set => _lifeTime = value; }
        Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            Pausable.PausableDestroy(gameObject, _lifeTime);
        }
        void IMissile.InitPhysicsProperties(Transform target, Vector3 initVelocity, float hitTime, float maxAcceleration)
        {
            _target = target;
            _initialVelocity = initVelocity;
            _period = hitTime;
            _maxAccleration = maxAcceleration;
        }

        void Update()
        {
            //‰^“®•û’öŽ®‚ðŽg‚Á‚½missile

            var accleration = _initialVelocity;

            var diff = transform.position;
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
            damageable.Damage(Damage, this.transform);
        }
        protected override void DeathBehavior(float attackPower)
        {
            Debug.Log("‚±‚±‚Å”š”­");
        }

    }
}
