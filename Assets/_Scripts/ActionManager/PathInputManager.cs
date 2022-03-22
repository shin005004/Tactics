using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathInputManager : MonoBehaviour
{
    private Camera viewCamera;
    public LayerMask aimLayer;
    [HideInInspector] public RaycastHit hit;

    [HideInInspector] public int pointIndex;
    [HideInInspector] public bool isNearPath = false;
    [HideInInspector] public bool isNearPlayer = false;

    private PathMaker _pathMaker;
    [SerializeField] public float editDist = 0.7f;

    private void Start()
    {
        viewCamera = Camera.main;
        _pathMaker = GetComponent<PathMaker>();
    }

    private void Update()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 300f, aimLayer))
        {
            isNearPath = false;
            isNearPlayer = Vector3.Distance(hit.point, transform.position) < editDist;

            float findClosestPoint = 999f;
            for (int i = 0; i < _pathMaker.points.Count; i++)
            {
                if (Vector3.Distance(hit.point, _pathMaker.points[i].point) > findClosestPoint)
                    break;
                if (Vector3.Distance(hit.point, _pathMaker.points[i].point) < editDist)
                {
                    findClosestPoint = Vector3.Distance(hit.point, _pathMaker.points[i].point);
                    pointIndex = i;
                    isNearPath = true;
                }
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