using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMaker : MonoBehaviour
{
    // System Settings
    private PathInputManager _input;

    // PathMaker
    private PathMaker _pathMaker;
    private ActionManager _actionManager;

    // Action Logic
    private float editDist;
    private float dragDist = 1.5f;

    private void Start()
    {
        // System Settings
        _pathMaker = GetComponent<PathMaker>();
        _actionManager = GetComponent<ActionManager>();
        _input = GetComponent<PathInputManager>();

        editDist = _input.editDist;
    }


    private void Update()
    {
        MakeViewAction();
    }


    // ViewAction PlayerAction.Look
    private bool actionMaking = false;
    private bool dragFromPath = false;
    private Vector3 lastPosition;
    private Vector3 viewDirection;
    private void MakeViewAction()
    {
        RaycastHit hit = _input.hit;

        // MakeViewAction From Player
        if(_pathMaker.points.Count == 0)
        {
            if (Input.GetMouseButtonDown(1) && _input.isNearPlayer)
            {
                lastPosition = transform.position;
                dragFromPath = true;
                // Debug.Log(lastPosition);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1) && _input.isNearPath)
            {
                lastPosition = _pathMaker.points[_input.pointIndex].point;
                
                dragFromPath = true;
                // Debug.Log(lastPosition);
            }
        }
            

        if (Input.GetMouseButtonUp(1))
            dragFromPath = false;

        if (dragFromPath)
            if (Input.GetMouseButton(1))
                if (Vector3.Distance(lastPosition, hit.point) > dragDist)
                    actionMaking = true;

        if (actionMaking)
        {
            // this will send the direction of the target
            if (Input.GetMouseButton(1))
                viewDirection = hit.point - lastPosition;
                // viewDirection = hit.point; // send exact target position
            if (Input.GetMouseButtonUp(1))
            {
                actionMaking = false;
                if (_pathMaker.points.Count == 0)
                    _pathMaker.points.Add(new PathInfo(transform.position, PlayerAction.Look, viewDirection));
                else
                    for (int i = 0; i < _pathMaker.points.Count; i++)
                        if (Vector3.Distance(lastPosition, _pathMaker.points[i].point) < editDist)
                        {
                            _pathMaker.points[i] = new PathInfo(_pathMaker.points[i].point, PlayerAction.Look, viewDirection);
                            break;
                        }

                Debug.Log($"Look Action Generated on : {lastPosition}");
                _actionManager.UpdateMoverPath();
            }
        }
    }
}
