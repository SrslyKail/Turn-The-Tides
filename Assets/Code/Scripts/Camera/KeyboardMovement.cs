using UnityEngine;

/**
 * Uses code from:
 * https://github.com/PanMig/Unity-RTS-Camera/tree/master
*/

[RequireComponent(typeof(Camera))]

public class KeyboardMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Space]
    [Range(5, 100)]
    public float minPanSpeed = 5;
    [Range(20, 100)]
    public float maxPanSpeed = 20;
    [Range(1, 5)]
    public float secToMaxSpeed = 3; //seconds taken to reach max speed;

    private float panSpeed;
    private Vector3 panMovement;
    private float panIncrease = 0.0f;

    void Update()
    {
        #region Movement

        panMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            panMovement += panSpeed * Time.deltaTime * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            panMovement -= panSpeed * Time.deltaTime * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            panMovement += panSpeed * Time.deltaTime * Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            panMovement += panSpeed * Time.deltaTime * Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            panMovement += panSpeed * Time.deltaTime * Vector3.up;
        }
        if (Input.GetKey(KeyCode.E))
        {
            panMovement += panSpeed * Time.deltaTime * Vector3.down;
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
    }

}