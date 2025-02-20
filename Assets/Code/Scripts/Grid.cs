using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0f);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        int pointsPerQuad = 6;

        // Initialize vertices array to hold all our elements
        // Adjacent quads can share the same vertext, so we need 1 more vertex
        // than we have tiles in each direction
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];

        // i is the overall index
        // x and y for x/y coordinates of vertices
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        // 6 points for each quad
        int[] triangles = new int[xSize * ySize * pointsPerQuad];

        //We increment vi because theres 1 more vertex than quad per row
        for(int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += pointsPerQuad, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
            
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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
