using UnityEngine;

namespace MFFrameWork.Character
{
    public abstract class PlayerMove : MonoBehaviour
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
            var groundParallel = Vector3.Cross(Vector3.Cross(hit.normal, objFoward), hit.normal).normalized;

            var moveRotation = Quaternion.FromToRotation(moveVector, groundParallel);
            _rb.linearVelocity = moveRotation * objFoward * _speed;
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
    interface IMove
    {
        Vector2 MoveVector {  get; }
        void Move(Vector2 moveVector);
    }
    interface IJump
    {
        float JumpPower {  get; }
        void Jump(float jumpPower);
    }
    interface IDush
    {
        void Dush(float dushRange);
    }
}
