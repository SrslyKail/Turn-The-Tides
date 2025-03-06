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
    public float minPanSpeed = 5;
    public float maxPanSpeed = 20;
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
    }

}