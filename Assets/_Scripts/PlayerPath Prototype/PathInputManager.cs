using UnityEngine;

namespace Proto
{
    public class PathInputManager : MonoBehaviour
    {
        public Camera viewCamera;
        public LayerMask aimLayer;
        public RaycastHit hit;

        public int pointIndex;
        public bool isNearPath = false;
        public bool isNearPlayer = false;

        private PlayerPath _playerPath;
        private float editDist = 0.7f;

        void Start()
        {
            _playerPath = GetComponent<PlayerPath>();
        }

        void Update()
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 300f, aimLayer))
            {
                isNearPath = false;
                isNearPlayer = Vector3.Distance(hit.point, transform.position) < editDist;
                for (int i = 0; i < _playerPath.points.Count; i++)
                    if (Vector3.Distance(hit.point, _playerPath.points[i].point) < editDist)
                    {
                        pointIndex = i;
                        isNearPath = true;
                        break;
                    }

                // Cant edit path
                if (!isNearPath)
                    if (Vector3.Distance(hit.point, transform.position) < editDist)
                    {
                        pointIndex = 0;
                        isNearPath = true;
                    }
                    else
                        pointIndex = -1;
            }
        }
    }
}