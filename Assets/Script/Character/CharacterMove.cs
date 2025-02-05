using UnityEngine;

namespace MFFrameWork.Character
{
    public class CharacterMove : Move_B
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
        public virtual void Move(Vector2 input)
        {
            Vector3 objFoward = new Vector3(_referenceObj.transform.forward.x, 0, _referenceObj.transform.forward.z);
            Vector3 inputVector3 = new Vector3(input.x, 0, input.y).normalized;
            var horizontalRotaion = Quaternion.AngleAxis(_referenceObj.transform.eulerAngles.y, Vector3.up);
            var moveVector = (horizontalRotaion * inputVector3);

            Physics.Raycast(new Ray(gameObject.transform.position, Vector3.down), out RaycastHit hit, _maxDistance);
            var groundParallel = Vector3.Cross(Vector3.Cross(hit.normal, transform.forward), hit.normal).normalized;

            var moveRotation = Quaternion.FromToRotation(objFoward, groundParallel);
            _rb.linearVelocity = moveRotation * inputVector3 * _speed;
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
