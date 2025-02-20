using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;
    private Vector3[] vertices;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        // Initialize vertices array to hold all our elements
        // Adjacent quads can share the same vertext, so we need 1 more vertex
        // than we have tiles in each direction
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        // i is the overall index
        // x and y for x/y coordinates of vertices
        for(int i = 0, y = 0; y <= ySize; y++)
        {
            for(int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
            }
        }
    }

    //Draw a sphere at each vertice so we can visualize where they are
    private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
