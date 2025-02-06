using UnityEngine;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : MonoBehaviour, IBullet
    {
        [SerializeField] Transform _target;
        [SerializeField] float _speed;

        [Space]
        [SerializeField] Vector3 _accleration;
        [SerializeField] float period = 3;
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
        }
        void IBullet.Init(Transform target, float attackPower, LayerMask ignoreLayer)
        {
            _target = target;
            _hitDamage = attackPower;
            _ignoreLayer = ignoreLayer;
            _position = transform.position;
        }

        void Update()
        {
            //‰^“®•û’öŽ®‚ðŽg‚Á‚½missile

            var accleration = _accleration;

            var diff = _target.position - transform.position;
            accleration += (diff - _velocity * period) * 2 / (period * period);

            if (accleration.magnitude > 100f)
            {
                accleration = accleration.normalized * 100f;
            }

            period -= Time.deltaTime;
            if (period < 0)
            {
                return;
            }

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
            DethBehaviour();
            Destroy(this.gameObject);
        }
        void DethBehaviour()
        {
            Debug.Log("‚±‚±‚Å”š”­");
        }

    }
}
