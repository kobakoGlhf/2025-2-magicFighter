using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFFrameWork
{
    public class PlayerCameraManager : MonoBehaviour
    {
        Player _player;

        List<EnemyManager> _enemyManager = new();

        [SerializeField]
        Vector3 _lockOnFov = new Vector3(.6f, .4f, 1f);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _enemyManager = FindObjectsByType<EnemyManager>(FindObjectsSortMode.None).ToList();
            _player = FindFirstObjectByType<Player>();
        }

        // Update is called once per frame
        void FixedUpdate()//ToDo:HERE åªç›ìGÇ™1ëÃÇµÇ©Ç¢Ç»Ç¢ÇΩÇﬂìKìñÇ≈Ç∑
        {
            foreach (var obj in _enemyManager)
            {
                if (!obj)
                {
                    _enemyManager.Remove(obj);
                }
                else if (CheckDirectionRange(obj.transform))
                {
                    _player.TargetTransform = obj.transform;
                    break;
                }
                else
                {
                    _player.TargetTransform = null;
                    break;
                }
            }
        }
        bool CheckDirectionRange(Transform target)
        {
            var direction = target.position - transform.position;
            var fovmin = transform.rotation * _lockOnFov;
            var fovmax = transform.rotation * Vector3.Scale(_lockOnFov, new Vector3(-1, -1, 1));

            Vector3 minCross = Vector3.Cross(direction, fovmin);
            Vector3 maxCross = Vector3.Cross(direction, fovmax);


            var n = Vector3.Scale(minCross, maxCross);

            Debug.DrawLine(transform.position, transform.rotation * Vector3.forward * 10, Color.blue);
            Debug.DrawLine(transform.position, transform.rotation * Vector3.Scale(_lockOnFov, new Vector3(1, 1, 1)) * 60, Color.green);
            Debug.DrawLine(transform.position, transform.rotation * Vector3.Scale(_lockOnFov, new Vector3(-1, 1, 1)) * 60, Color.green);
            Debug.DrawLine(transform.position, transform.rotation * Vector3.Scale(_lockOnFov, new Vector3(1, -1, 1)) * 60, Color.green);
            Debug.DrawLine(transform.position, transform.rotation * Vector3.Scale(_lockOnFov, new Vector3(-1, -1, 1)) * 60, Color.green);

            return n.x < 0 && n.y < 0;
        }

    }
}
