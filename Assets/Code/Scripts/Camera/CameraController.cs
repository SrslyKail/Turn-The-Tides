using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls everything related to the camera, including movement, rotation, and tracking.
/// 
/// Written by Gurjeet Bhangoo.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float MovementSpeed = 10.0f;

    [Header("Rotation")]
    public float RotationSpeed = 60.0f;
    public Vector2 RotationVerticalBounds;
    public bool RotationUseHorizontalBounds = false;
    public Vector2 RotationHorizontalBounds;

    [Header("Tracking")]
    public float DistanceFromMarker = 10.0f;
    public float VerticalTrackingStartingHeightOffset = 128.0f;
    public float VerticalTrackingMaxScanDistance = 256.0f;
    public float VerticalTrackingUpdateSpeed = 10.0f;

    [Header("Object Selection")]
    public GameUI GameUi;
    public float DistanceToDragMouseBeforeIgnoringObjectSelection = 1.0f;

    [Header("Zoom")]
    public float DefaultZoomLevel = 60.0f;
    public float ZoomSpeed = 10.0f;
    public float ZoomScrollInterval = 5.0f;
    public float ZoomUpdateSpeed = 10.0f;
    public Vector2 ZoomBounds;

    private PlayerInput playerInput;

    private float zoomLevel;

    private Vector2 mouseMovement;
    private float distanceMouseDragged;

    private Vector3 cameraRotation;

    private Vector3 markerPosition;

    private float currentHeight;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private GameObject selectedObject;

    public void OnSelectButtonPressed(InputAction.CallbackContext context)
    {
        // if button is pressed (not released), select object at marker
        if (context.started)
        {
            SelectObjectAtMarker();
        }
    }

    private void MoveMarker(Vector3 delta)
    {
        // get transform as if x-rotation was 0
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        markerPosition += MovementSpeed * Time.fixedDeltaTime * ((delta.x * right) + (delta.y * forward));
    }

    private void RotateCamera(Vector2 delta)
    {
        cameraRotation.x -= delta.y * (RotationSpeed * Time.fixedDeltaTime);
        cameraRotation.y += delta.x * (RotationSpeed * Time.fixedDeltaTime);

        // clamp vertical rotation
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, RotationVerticalBounds.x, RotationVerticalBounds.y);

        // clamp horizontal rotation
        if (RotationUseHorizontalBounds)
        {
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, RotationHorizontalBounds.x, RotationHorizontalBounds.y);
        }

        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
    }

    private void ZoomCamera(float delta)
    {
        zoomLevel += delta;
        zoomLevel = Mathf.Clamp(zoomLevel, ZoomBounds.x, ZoomBounds.y);
    }

    private void SelectObject(GameObject target)
    {
        // If we have already selected an object, deselect it
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        // Set the selected object to the object that was hit
        selectedObject = target;

        // Change the color of the selected object
        target.GetComponent<Renderer>().material.color = Color.red;
    }

    private void SelectObjectAtMousePosition()
    {
        // Get the ray from the camera to the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If the ray hits an object
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Get the selected game object
            GameObject target = hit.collider.gameObject;

            if (target != null)
            {
                SelectObject(target);
            }
        }
    }

    private void SelectObjectAtMarker()
    {
        // raycast downwards from immediately above the marker
        RaycastHit hit;
        Vector3 raycastOrigin = markerPosition + (Vector3.up);
        Vector3 raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out hit))
        {
            Debug.Log("Selecting object at marker");
            // if raycast hits a GameObject, select it
            GameObject hitObject = hit.collider.gameObject;
            SelectObject(hitObject);
        }
    }

    private void MoveMarkerPositionToSurface()
    {
        // raycast from the sky downwards
        RaycastHit hit;
        Vector3 raycastOrigin = markerPosition + (Vector3.up * VerticalTrackingStartingHeightOffset);
        Vector3 raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, VerticalTrackingMaxScanDistance))
        {
            // if raycast hits a HexTile, set marker's y position to HexTile's elevation
            GameObject hitObject = hit.collider.gameObject;
            currentHeight = hitObject.transform.position.y;
        }

        // once the current height has been set, lerp the marker's y position to the current height
        markerPosition.y = Mathf.Lerp(markerPosition.y, currentHeight, Time.fixedDeltaTime * VerticalTrackingUpdateSpeed);
    }

    private void SyncCameraToMarker()
    {
        transform.SetPositionAndRotation(markerPosition + transform.forward * -DistanceFromMarker, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));
    }

    private void InterpolateCameraFovToZoomLevel()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomLevel, Time.fixedDeltaTime * ZoomUpdateSpeed);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        initialRotation = transform.rotation;
        initialPosition = transform.position;

        cameraRotation = initialRotation.eulerAngles;
        markerPosition = initialPosition;

        currentHeight = initialPosition.y;
        zoomLevel = DefaultZoomLevel;

        SyncCameraToMarker();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector2 rotationInput = playerInput.actions["Rotate"].ReadValue<Vector2>();

        if (movementInput.sqrMagnitude > 0.0f)
        {
            MoveMarker(movementInput);
        }

        if (rotationInput.sqrMagnitude > 0.0f)
        {
            RotateCamera(rotationInput);
        }

        float zoomInput = playerInput.actions["Zoom"].ReadValue<float>();
        if (zoomInput < 0.0f)
        {
            ZoomCamera(1.0f * ZoomSpeed * Time.fixedDeltaTime);
        }
        else if (zoomInput > 0.0f)
        {
            ZoomCamera(-1.0f * ZoomSpeed * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        // if UI is selected, do not move camera with mouse
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // if scroll-wheel is moved, zoom camera in or out
        var scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            ZoomCamera(-scrollDelta * ZoomScrollInterval);
        }

        // store current mouse movement
        mouseMovement = Input.mousePositionDelta;


        // if right-click is held, rotate camera with mouse movement
        if (playerInput.actions["RotateCameraWithMouse"].IsPressed())
        {
            RotateCamera(mouseMovement);
        }

        // if left-click is held, move camera along x and z axis with mouse movement
        if (playerInput.actions["DragCameraWithMouse"].IsPressed())
        {
            MoveMarker(-mouseMovement);

            // also keep track of how far the mouse has been dragged
            distanceMouseDragged += mouseMovement.magnitude;
        }

        // if left-click is released, check if the mouse hasn't been dragged too far
        if (playerInput.actions["DragCameraWithMouse"].WasCompletedThisFrame())
        {
            if (distanceMouseDragged < DistanceToDragMouseBeforeIgnoringObjectSelection)
            {
                // if the mouse hasn't been dragged too far, select the object
                SelectObjectAtMousePosition();
            }
            // reset distance mouse has been dragged
            distanceMouseDragged = 0.0f;
        }
    }

    void LateUpdate()
    {
        MoveMarkerPositionToSurface();
        SyncCameraToMarker();
        InterpolateCameraFovToZoomLevel();
    }

}
