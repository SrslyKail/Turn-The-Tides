using System;
using TurnTheTides;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls everything related to the camera, including movement, rotation, and tracking.
/// 
/// Written primarily by Gurjeet Bhangoo, with some tweaks from Corey Buchan.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The speed at which the camera moves, in metres per second.
    /// </summary>
    [Header("Movement")]
    public float MovementSpeed = 10.0f;

    /// <summary>
    /// The speed at which the camera turns, in degrees per second.
    /// </summary>
    [Header("Rotation")]
    public float RotationSpeed = 60.0f;
    /// <summary>
    /// The vertical bounds of the camera rotation, in degrees.
    /// X is the minimum angle, Y is the maximum angle.
    /// </summary>
    public Vector2 RotationVerticalBounds;
    /// <summary>
    /// Whether to use horizontal bounds for the camera rotation.
    /// </summary>
    public bool RotationUseHorizontalBounds = false;
    /// <summary>
    /// The horizontal bounds of the camera rotation, in degrees.
    /// X is the minimum angle, Y is the maximum angle.
    /// </summary>
    public Vector2 RotationHorizontalBounds;

    /// <summary>
    /// The camera's distance from its target position.
    /// </summary>
    [Header("Tracking")]
    public float DistanceFromMarker = 10.0f;
    /// <summary>
    /// The height offset to start raycasting from when tracking the surface.
    /// </summary>
    public float VerticalTrackingStartingHeightOffset = 128.0f;
    /// <summary>
    /// The maximum distance to scan for the surface when tracking.
    /// </summary>
    public float VerticalTrackingMaxScanDistance = 256.0f;
    /// <summary>
    /// The speed at which the camera adjusts vertically to match the current surface, in metres per second.
    /// </summary>
    public float VerticalTrackingUpdateSpeed = 10.0f;

    /// <summary>
    /// The reference to the game's UI object..
    /// </summary>
    [Header("Object Selection")]
    public GameUI GameUi;
    /// <summary>
    /// The distance the mouse must be dragged before the object selection is ignored. (In case you couldn't tell.)
    /// </summary>
    public float DistanceToDragMouseBeforeIgnoringObjectSelection = 1.0f;

    /// <summary>
    /// The camera's default zoom level, in degrees.
    /// </summary>
    [Header("Zoom")]
    public float DefaultZoomLevel = 60.0f;
    /// <summary>
    /// The speed at which the camera changes its zoom level, in degrees per second.
    /// </summary>
    public float ZoomSpeed = 10.0f;
    /// <summary>
    /// The amount by which the zoom level changes whenever the zoom input is pressed, in degrees.
    /// </summary>
    public float ZoomScrollInterval = 5.0f;
    /// <summary>
    /// The speed at which the camera adapts to the new zoom level, in degrees per second.
    /// </summary>
    public float ZoomUpdateSpeed = 10.0f;
    /// <summary>
    /// The minimum and maximum zoom levels, in degrees.
    /// </summary>
    public Vector2 ZoomBounds;

    private PlayerInput playerInput;

    private float zoomLevel;    // the camera's current zoom level, in degrees

    private Vector2 mouseMovement;  // the current mouse movement, in pixels
    private float distanceMouseDragged; // the distance the mouse has been dragged, in pixels

    private Vector3 cameraRotation; // the camera's current rotation, in degrees

    private Vector3 markerPosition; // the camera's target position, in world space

    private float currentHeight;    // the camera's current height, in world space

    private Vector3 initialPosition;    // the camera's initial position, in world space
    private Quaternion initialRotation; // the camera's initial rotation, in world space

    private GameObject selectedObject;  // the most recently selected hex tile's game object
    private Color selectedMeshColor;    // the color of the selected hex tile's mesh

    private bool canMove = false;   // whether the camera can move or not
    /// <summary>
    /// Called when the script instance is being loaded. This is called only once during the lifetime of the script instance.
    /// </summary>
    private void Awake()
    {
        TTTEvents.ChangeBoardState += OnBoardStateChange;
    }

    /// <summary>
    /// Called when the board state changes. Used to determine whether the camera can move or not.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBoardStateChange(object sender, EventArgs e)
    {
        BoardStateEventArgs args = e as BoardStateEventArgs;
        switch (args.NewBoardState)
        {
            case BoardState.None:
            case BoardState.Loading:
            case BoardState.MainMenu:
                {
                    canMove = false;
                    break;
                }
            default:
                {
                    canMove = true;
                    break;
                }
        }
    }

    /// <summary>
    /// Called when the button for selecting the current tile is pressed. Used to select the object at the marker using keyboard and gamepad controls.
    /// </summary>
    /// <param name="context"></param>
    public void OnSelectButtonPressed(InputAction.CallbackContext context)
    {
        // if button is pressed (not released), select object at marker
        if (context.started)
        {
            SelectObjectAtMarker();
        }
    }

    /// <summary>
    /// Deselects the currently selected object, if any.
    /// </summary>
    public void DeselectCurrentObject()
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponent<Renderer>().material.color = selectedMeshColor;
            selectedObject = null;

            GameUi.HideTileInfoPanel();
            GameUi.ClearTileInfoPanel();
        }
    }
    /// <summary>
    /// Helper method to move the camera marker. The marker is used to determine the camera's current position, because RTS cameras orbit around a given point.
    /// </summary>
    /// <param name="delta"></param>
    private void MoveMarker(Vector3 delta)
    {
        if (canMove)
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

    }

    /// <summary>
    /// Helper method to rotate the camera. The camera rotates around the marker.
    /// </summary>
    /// <param name="delta"></param>
    private void RotateCamera(Vector2 delta)
    {
        if (canMove)
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

    }

    /// <summary>
    /// Helper method to zoom the camera. The camera zooms in and out by changing its field of view.
    /// </summary>
    /// <param name="delta"></param>
    private void ZoomCamera(float delta)
    {
        if (canMove)
        {
            zoomLevel += delta;
            zoomLevel = Mathf.Clamp(zoomLevel, ZoomBounds.x, ZoomBounds.y);
        }

    }

    /// <summary>
    /// Helper method to select an object. The object is selected by changing its color to red.
    /// </summary>
    /// <param name="target"></param>
    private void SelectObject(GameObject target)
    {
        // If the target is the same as the selected object, deselect it and return
        if (target == selectedObject)
        {
            DeselectCurrentObject();
            return;
        }
        // If the target is not a HexTile, return
        if (target.GetComponent<HexTile>() == null)
        {
            return;
        }

        // If we have already selected an object, deselect it
        DeselectCurrentObject();

        // Set the selected object to the object that was hit
        selectedObject = target;
        selectedMeshColor = selectedObject.GetComponent<Renderer>().material.color;

        // Change the color of the selected object
        target.GetComponent<Renderer>().material.color = Color.red;

        GameUi.UpdateTileInfoPanel(selectedObject.GetComponent<HexTile>());
    }

    /// <summary>
    /// Selects the object at the mouse position. This is used for selecting objects with the mouse.
    /// </summary>
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

    /// <summary>
    /// Selects the object at the marker position. This is used for selecting objects with keyboard and gamepad controls.
    /// </summary>
    private void SelectObjectAtMarker()
    {
        // raycast downwards from immediately above the marker
        Vector3 raycastOrigin = markerPosition + Vector3.up;
        Vector3 raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit))
        {
            Debug.Log("Selecting object at marker");
            // if raycast hits a GameObject, select it
            GameObject hitObject = hit.collider.gameObject;
            SelectObject(hitObject);
        }
    }

    /// <summary>
    /// Moves the marker's y position to the surface of the playing field. Done by raycasting downwards from the marker's position.
    /// </summary>
    private void MoveMarkerPositionToSurface()
    {
        // raycast from the sky downwards
        Vector3 raycastOrigin = markerPosition + (Vector3.up * VerticalTrackingStartingHeightOffset);
        Vector3 raycastDirection = Vector3.down;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out RaycastHit hit, VerticalTrackingMaxScanDistance))
        {
            // if raycast hits a HexTile, set marker's y position to HexTile's elevation
            GameObject hitObject = hit.collider.gameObject;
            currentHeight = hitObject.transform.position.y;
        }

        // once the current height has been set, lerp the marker's y position to the current height
        markerPosition.y = Mathf.Lerp(markerPosition.y, currentHeight, Time.fixedDeltaTime * VerticalTrackingUpdateSpeed);
    }

    /// <summary>
    /// Syncs the camera's position and rotation to the marker's position and rotation.
    /// </summary>
    private void SyncCameraToMarker()
    {
        transform.SetPositionAndRotation(
            markerPosition + (transform.forward * -DistanceFromMarker),
            Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            0));
    }

    /// <summary>
    /// Interpolates the camera's field of view to the current zoom level. Done by lerping the camera's field of view to the zoom level.
    /// </summary>
    private void InterpolateCameraFovToZoomLevel()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomLevel, Time.fixedDeltaTime * ZoomUpdateSpeed);
    }

    /// <summary>
    /// Called when the script instance is being loaded. This is called only once during the lifetime of the script instance.
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();
        initialRotation = transform.rotation;
        initialPosition = transform.position;

        cameraRotation = initialRotation.eulerAngles;
        markerPosition = initialPosition;

        currentHeight = initialPosition.y;
        zoomLevel = DefaultZoomLevel;

        SyncCameraToMarker();

        GameUi = GameUI.Instance;
    }

    /// <summary>
    /// Called once per physics frame. Runs at a fixed pace, separate from frame rate.
    /// </summary>
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

    /// <summary>
    /// Called once per frame. Runs at the same pace as the frame rate.
    /// </summary>
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

    /// <summary>
    /// Called once per frame, after all other updates have been called.
    /// </summary>
    void LateUpdate()
    {
        MoveMarkerPositionToSurface();
        SyncCameraToMarker();
        InterpolateCameraFovToZoomLevel();
    }

}
