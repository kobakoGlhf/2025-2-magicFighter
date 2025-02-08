using UnityEngine;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : MonoBehaviour, IBullet
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
        float _hitDamage;
        Rigidbody _rb;
        LayerMask _ignoreLayer;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            Destroy(gameObject, _lifeTime);
        }
        void IBullet.InitProperties(Transform target, float attackPower, LayerMask ignoreLayer)
        {
            _target = target;
            _hitDamage = attackPower;
            _ignoreLayer = ignoreLayer;
            _position = transform.position;
        }
        void IBullet.InitPhysicsProperties(Vector3 initVelocity, float hitTime, float maxAcceleration)
        {
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _ignoreLayer ||
                other.gameObject.layer == gameObject.layer) return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(_hitDamage, this.transform);
            }
            DeathBehavior(_hitDamage);
            Destroy(this.gameObject);
        }

        void DeathBehavior(float attackPower)
        {
            Debug.Log("‚±‚±‚Å”š”­");
        }

    }
}
