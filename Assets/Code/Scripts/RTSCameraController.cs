using UnityEngine;

/**
 * Uses code from:
 * https://github.com/PanMig/Unity-RTS-Camera/tree/master
*/

[RequireComponent(typeof(Camera))]

public class RTSCameraController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")]
    [Space]
    public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 lenghtLimit;
    public Vector2 widthLimit;
    private Vector2 zoomLimit;

    private float panSpeed;
    private Vector3 initialPos;
    private Vector3 panMovement;
    private Vector3 pos;
    private float panIncrease = 0.0f;





    // Use this for initialization
    void Start()
    {
        initialPos = transform.position;
        zoomLimit.x = 15;
        zoomLimit.y = 65;
    }


    void Update()
    {
        #region Movement

        panMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            panMovement += Vector3.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            panMovement -= Vector3.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            panMovement += Vector3.left * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            panMovement += Vector3.right * panSpeed * Time.deltaTime;
            //pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            panMovement += Vector3.up * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            panMovement += Vector3.down * panSpeed * Time.deltaTime;
        }

        transform.Translate(panMovement, Space.World);


        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q))
        {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        }
        else
        {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }

        #endregion

        #region Zoom

        Camera.main.fieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomLimit.x, zoomLimit.y);

        #endregion


        #region boundaries

        if (enableMovementLimits == true)
        {
            //movement limits
            pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, heightLimit.x, heightLimit.y);
            pos.z = Mathf.Clamp(pos.z, lenghtLimit.x, lenghtLimit.y);
            pos.x = Mathf.Clamp(pos.x, widthLimit.x, widthLimit.y);
            transform.position = pos;
        }



        #endregion

    }

}