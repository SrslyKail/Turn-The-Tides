using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [Header("Scrolling")]
    [Space]
    public float positionScrollSpeed = 5.0f;

    [Header("Rotation")]
    [Space]
    public float rotationSpeed = 60.0f;
    public Vector2 rotationHorizontalBounds = new(-90, 90);
    public Vector2 rotationVerticalBounds = new(-90, 90);

    private Vector2 mouseMovement;
    private Vector3 cameraRotation;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
        cameraRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        mouseMovement = Input.mousePositionDelta * Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            ScrollCamera();
        }

        if (Input.GetMouseButton(1))
        {
            RotateCamera();
        }

        if (Input.GetMouseButtonUp(1))
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, 0.5f);
        }
    }

    void ScrollCamera()
    {
        // Move the camera
        Camera.main.transform.position -= new Vector3(mouseMovement.x, 0, mouseMovement.y) * positionScrollSpeed;
    }

    void RotateCamera()
    {
        var rotationInput = mouseMovement.x * rotationSpeed * Vector3.up;
        rotationInput -= mouseMovement.y * rotationSpeed * Vector3.right;

        cameraRotation += rotationInput;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, rotationVerticalBounds.x, rotationVerticalBounds.y);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, rotationHorizontalBounds.x, rotationHorizontalBounds.y);
        cameraRotation.z = 0;

        transform.rotation = Quaternion.Euler(cameraRotation);

    }
}
