using UnityEngine;

namespace MFFrameWork
{
    public class IKAnimation: MonoBehaviour
    {
        Animator _animator;
        [SerializeField]
        LayerMask _layerMask;
        [SerializeField]
        string _groundTagName;
        [SerializeField, Range(0, 1f)]
        float _groundDistanse;
        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) Debug.Log("Aniator is null");
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            //ë´Ç™ínñ Ç…ñÑÇ‹Ç¡ÇƒÇ¢ÇÈèÍçáÇÃëŒçÙÇ∆ÇµÇƒè„ï˚å¸Ç…Ç∏ÇÁÇµÇƒRayÇê∂ê¨Ç∑ÇÈ
            Ray ray;
            RaycastHit hit;

            ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, _groundDistanse + 1, _layerMask))
            {
                if (hit.transform.tag == _groundTagName)
                {
                    Vector3 footPos = hit.point;
                    footPos.y += _groundDistanse;
                    _animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                    //_animator.SetIKRotation(AvatarIKGoal.LeftFoot, _animator.GetIKRotation(AvatarIKGoal.LeftFoot) * Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
            }
            ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, _groundDistanse + 1, _layerMask))
            {
                if (hit.transform.tag == _groundTagName)
                {
                    Vector3 footPos = hit.point;
                    footPos.y += _groundDistanse;
                    _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                    //_animator.SetIKRotation(AvatarIKGoal.LeftFoot, _animator.GetIKRotation(AvatarIKGoal.LeftFoot) * Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
            }
        }
    }
}
