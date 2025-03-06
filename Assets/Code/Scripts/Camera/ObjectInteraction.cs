using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Thresholds")]
    [Space]
    public float distanceToMoveMouseBeforeScrollingCamera = 1.0f;




    GameObject selectedObject = null;
    Vector3 mouseMovement = Vector3.zero;
    float distanceMouseMoved = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseMovement = Input.mousePositionDelta * Time.deltaTime;
            distanceMouseMoved += mouseMovement.magnitude;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (distanceMouseMoved < distanceToMoveMouseBeforeScrollingCamera)
            {
                SelectObject();
            }

            distanceMouseMoved = 0.0f;
        }
    }

    void SelectObject()
    {
        // Get the ray from the camera to the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If the ray hits an object
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // If we have already selected an object, deselect it
            if (selectedObject != null)
            {
                selectedObject.GetComponent<Renderer>().material.color = Color.white;
            }

            // Set the selected object to the object that was hit
            selectedObject = hit.collider.gameObject;

            // Change the color of the selected object
            selectedObject.GetComponent<Renderer>().material.color = Color.red;

            // Print the name of the selected object
            Debug.Log(selectedObject.name);
        }
    }

}
