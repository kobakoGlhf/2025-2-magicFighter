using UnityEngine;

namespace MFFrameWork.Charactor
{
    public class CharactorMove : Move_B
    {

    }
    public abstract class Move_B : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] float _jumpPower;
        [SerializeField] GameObject _referenceObj;
        Rigidbody _rb;
        [SerializeField] float _maxDistance;
        bool _isGround;
        public virtual void Init() { }
        public void SetReferencePoint()
        {

        }
        public virtual void Move(Vector2 vector)
        {
            Ray downRay = new Ray(gameObject.transform.position, Vector3.down);
            Physics.Raycast(downRay, out RaycastHit hit, _maxDistance);
            var groundParallelNormal = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal).normalized;
            Debug.Log(groundParallelNormal);

            vector = vector.normalized * _speed;
            var horizontalRotaion = Quaternion.AngleAxis(_referenceObj.transform.eulerAngles.y, Vector3.up);
            _rb.linearVelocity = horizontalRotaion * new Vector3(vector.x,0,vector.y) + new Vector3(0, _rb.linearVelocity.y, 0);
        }
        public virtual void Jump()
        {
            Debug.Log("jump!!");
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        }
        public virtual void Dush(Vector2 vector)
        {
            Debug.Log("Dush!!");
        }
        public void SetRigidbody(Rigidbody rb) => _rb = rb;
        public void SetSpeed(float speed) => _speed = speed;
        public void SetJumpPower(float jumpPower) => _jumpPower = jumpPower;
    }
}
