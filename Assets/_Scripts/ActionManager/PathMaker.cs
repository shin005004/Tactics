using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMaker : MonoBehaviour
{
    // System Settings 
    private PathInputManager _input;
    private ActionManager _actionManager;

    // Path Logic
    [HideInInspector] public List<PathInfo> points = new List<PathInfo>();
    [SerializeField] private float pointDist = 1f;
    [SerializeField] private float editDist = 1.0f;


    // Line Rendering
    private List<Vector3> linePoints = new List<Vector3>();
    public LineRenderer lineRenderer;


    // Path Variables 
    private bool isEditing = false;

    private void Start()
    {
        points.Clear();
        linePoints.Clear();
        lineRenderer.positionCount = 0;

        // System Settings
        _input = GetComponent<PathInputManager>();
        _actionManager = GetComponent<ActionManager>();
    }

    private void Update()
    {
        // If Game State is Okay(TODO)
        HandlePathInput();
    }

    private void HandlePathInput()
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
                if (Vector3.Distance(hit.point, transform.position) < editDist)
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
        // lineRender Update
        linePoints.Clear();
        for (int i = 0; i < points.Count; i++)
            linePoints.Add(points[i].point + new Vector3(0, 1, 0));
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());

        // Update MoverScripts Path
        _actionManager.UpdateMoverPath();
    }

    private float DistanceToLastPoint(Vector3 point)
    {
        if (!points.Any())
            return Mathf.Infinity;
        return Vector3.Distance(points.Last().point, point);
    }

    
}
