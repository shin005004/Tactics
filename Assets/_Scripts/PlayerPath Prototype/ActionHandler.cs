using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto
{
    public class ActionHandler : MonoBehaviour
    {
        public LayerMask aimLayer;
        public Camera viewCamera;

        private PathInputManager _input;
        private PlayerPath _playerPath;
        private float editDist = 1.5f;
        private float dragDist = 1.5f;

        // MouseRightButton
        private Vector3 lastPosition;
        private Vector3 viewDirection;

        private bool actionMaking = false;
        private bool dragFromPath = false;

        private void Start()
        {
            _playerPath = GetComponent<PlayerPath>();
            _input = GetComponent<PathInputManager>();
        }

        private void Update()
        {
            if (_playerPath.points.Count == 0)
                MakeViewActionFromPlayer();
            else
                MakeViewAction();
        }

        private void MakeViewAction()
        {
            RaycastHit hit = _input.hit;

            if (Input.GetMouseButtonDown(1) && _input.isNearPath)
            {
                lastPosition = _playerPath.points[_input.pointIndex].point;
                dragFromPath = true;
                Debug.Log(lastPosition);
            }
            if (Input.GetMouseButtonUp(1))
                dragFromPath = false;

            if (dragFromPath)
                if (Input.GetMouseButton(1))
                    if (Vector3.Distance(lastPosition, hit.point) > dragDist)
                        actionMaking = true;

            if (actionMaking)
            {
                // this will send the exact world position of the target
                if (Input.GetMouseButton(1))
                    viewDirection = hit.point; // hit.point - lastPosition;
                if (Input.GetMouseButtonUp(1))
                {
                    actionMaking = false;
                    for (int i = 0; i < _playerPath.points.Count; i++)
                        if (Vector3.Distance(lastPosition, _playerPath.points[i].point) < editDist)
                        {
                            _playerPath.points[i] = new PathInfo(_playerPath.points[i].point, PlayerAction.Look, viewDirection);
                            Debug.Log("Overrided");
                            _playerPath.UpdatePath();
                            break;
                        }
                }
            }
        }

        // added 
        private void MakeViewActionFromPlayer()
        {
            RaycastHit hit = _input.hit;

            if (Input.GetMouseButtonDown(1) && _input.isNearPlayer)
            {
                lastPosition = transform.position;
                dragFromPath = true;
                Debug.Log(lastPosition);
            }
            if (Input.GetMouseButtonUp(1))
                dragFromPath = false;

            if (dragFromPath)
                if (Input.GetMouseButton(1))
                    if (Vector3.Distance(lastPosition, hit.point) > dragDist)
                        actionMaking = true;

            if (actionMaking)
            {
                // this will send the exact world position of the target
                if (Input.GetMouseButton(1))
                    viewDirection = hit.point; // hit.point - lastPosition;
                if (Input.GetMouseButtonUp(1))
                {
                    actionMaking = false;
                    _playerPath.points.Add(new PathInfo(transform.position, PlayerAction.Look, viewDirection));
                    Debug.Log("Overrided");
                    _playerPath.UpdatePath();
                }
            }
        }
    }
}