using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    private Camera viewCamera;
    public float movementSpeed;
    public float movementTime;
    public float mouseZoomSensitivity;
    public Vector3 zoomAmout;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private Vector3 newZoom;
    private Vector3 newPosition;

    private void Start()
    {
        newPosition = transform.position;
        newZoom = cameraTransform.localPosition;

        viewCamera = cameraTransform.GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    // KeyBoard Input and movement
    void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        // ZoomIn / ZoomOut
        if (Input.GetKey(KeyCode.R))
        {
            newZoom -= zoomAmout;
        }
        if(Input.GetKey(KeyCode.F))
        {
            newZoom += zoomAmout;
        }

        newZoom.y = Mathf.Clamp(newZoom.y, 80.0f, 230.0f);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom -= Input.mouseScrollDelta.y * zoomAmout * mouseZoomSensitivity;
        }    
        if(Input.GetMouseButtonDown(2))
        {
            Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            float hit;

            
            if(aimPlane.Raycast(ray, out hit))
            {
                dragStartPosition = ray.GetPoint(hit);
            }
        }
        if (Input.GetMouseButton(2))
        {
            Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            float hit;

            if (aimPlane.Raycast(ray, out hit))
            {
                dragCurrentPosition = ray.GetPoint(hit);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }
}
