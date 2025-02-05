using UnityEngine;

namespace MFFrameWork
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] float _speed;
        [SerializeField] GameObject _effect;
        float _damage;
        Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void Init(Transform target, float speed, GameObject effect, float damage)
        {
            _target = target;
            _speed = speed;
            _effect = effect;
            _damage = damage;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetVector = _target.position - transform.position;
            targetVector.Normalize();

            _rb.linearVelocity = targetVector * _speed;
        }
    }
}
