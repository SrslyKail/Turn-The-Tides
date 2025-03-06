using UnityEngine;

public class CameraBoundaries : MonoBehaviour
{
    [Header("Position Boundaries")]
    public bool usePositionBounds;
    public Vector2 positionXBounds;
    public Vector2 positionZBounds;

    [Header("Rotation Boundaries")]
    public bool useRotationBounds;
    public Vector2 rotationHorizontalBounds;
    public Vector2 rotationVerticalBounds;

    // Update is called once per frame
    void Update()
    {

    }
}
