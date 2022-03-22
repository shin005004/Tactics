using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Proto
{
    public class PlayerPath : MonoBehaviour
    {
        public LayerMask aimLayer;
        public Camera viewCamera;

        private PathInputManager _input;

        [HideInInspector] public List<PathInfo> points = new List<PathInfo>();
        [SerializeField] private float pointDist = 1f;
        private float editDist = 1.0f;

        private bool isEditing = false;

        // PathCreator
        private List<Vector3> linePoints = new List<Vector3>();
        public LineRenderer lineRenderer;

        // PathMover
        private Queue<PathInfo> pathPoints = new Queue<PathInfo>();
        public float movementSpeed;
        public float rotationSpeed;
        private NavAgentManager _navAgent;

        // ViewActionHanlding
        private Vector3 lastLookDir;
        private float lastLookTime = -1f;

        private void Awake()
        {
            points.Clear();
            _input = GetComponent<PathInputManager>();

            // PathCreator
            lineRenderer.positionCount = 0;

            // PathMover
            lastLookTime = -1f;
            _navAgent = GetComponent<NavAgentManager>();
        }

        private void Update()
        {
            MakePath();
        }

        // PathMover
        void LateUpdate()
        {
            // Action selection
            if (pathPoints.Count != 0)
            {
                _navAgent.isPath = true;
                // View action = 1
                PathInfo peekPath = pathPoints.Peek();

                if (peekPath.action == PlayerAction.Look)
                {
                    _navAgent.pathNextRotation = peekPath.viewDir;
                    _navAgent.rotationSpeed = 10f;
                    lastLookTime = Time.time;
                    lastLookDir = peekPath.viewDir;
                }
                else if (Time.time - lastLookTime < 1.5f)
                {
                    _navAgent.pathNextRotation = lastLookDir;
                    _navAgent.rotationSpeed = 10f;
                }
                else
                {
                    if (_navAgent.navMeshAgent.steeringTarget != null)
                    {
                        // var direction = _navAgent.navMeshAgent.steeringTarget - transform.position;
                        _navAgent.pathNextRotation = _navAgent.navMeshAgent.steeringTarget;
                        _navAgent.rotationSpeed = 5f;
                    }
                }
            }
            else
            {
                _navAgent.isPath = false;
                if (Time.time - lastLookTime < 1.5f)
                {
                    _navAgent.pathNextRotation = lastLookDir;
                    _navAgent.rotationSpeed = 10f;
                    _navAgent.isPath = true;
                }
            }

            if (ShouldSetDestination())
            {
                PathInfo peekPath = pathPoints.Peek();
                _navAgent.pathNextPosition = peekPath.point;
                ErasePassedPath();
            }
        }

        private bool ShouldSetDestination()
        {
            if (pathPoints.Count == 0)
                return false;

            if (_navAgent.navMeshAgent.hasPath == false || _navAgent.navMeshAgent.remainingDistance < 0.5f)
                return true;

            return false;
        }

        private PathInfo ErasePassedPath()
        {
            if (points.Count != 0)
            {
                var tempInfo = points[0];
                pathPoints.Dequeue();
                points.RemoveAt(0);
                UpdatePath();
                return tempInfo;
            }
            return new PathInfo(Vector3.zero);
        }

        // PathManager
        private void MakePath()
        {
            RaycastHit hit = _input.hit;

            if (Input.GetMouseButtonDown(0))
            {
                if (_input.isNearPath)
                {
                    points.RemoveRange(_input.pointIndex, points.Count - _input.pointIndex);
                    isEditing = _input.isNearPath;
                }

                if (points.Count == 0)
                {
                    var correctedPosition = transform.position;
                    correctedPosition.y = 0;
                    if (Vector3.Distance(hit.point, correctedPosition) < editDist)
                        isEditing = true;
                }
            }
            if (Input.GetMouseButton(0) && isEditing)
            {
                if (DistanceToLastPoint(hit.point) > pointDist)
                    points.Add(new PathInfo(hit.point));
                UpdatePath();
            }
            if (Input.GetMouseButtonUp(0))
            {
                isEditing = false;
                UpdatePath();
            }
        }

        public void UpdatePath()
        {
            // PathMover
            pathPoints.Clear();
            for (int i = 0; i < points.Count; i++)
                pathPoints.Enqueue(points[i]);

            // PathCreator
            linePoints.Clear();
            // linePoints.Add(transform.position + new Vector3(0, 1, 0));
            for (int i = 0; i < points.Count; i++)
                linePoints.Add(points[i].point + new Vector3(0, 1, 0));
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
        }

        private float DistanceToLastPoint(Vector3 point)
        {
            if (!points.Any())
                return Mathf.Infinity;
            return Vector3.Distance(points.Last().point, point);
        }
    }
}