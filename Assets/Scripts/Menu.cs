using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
    Mesh mesh;
    public void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
        vertices = new Vector3[] {
            new Vector3(0, 0, 0), // 0
            new Vector3(1, 0, 0), // 1
            new Vector3(0, 0, 1), // 2
            new Vector3(1, 0, 1), // 3
            new Vector3(0, 1, 0), // 4
            new Vector3(1, 1, 0), // 5
            new Vector3(0, 1, 1), // 6
            new Vector3(1, 1, 1), // 7
            new Vector3(0, 1, 1), // 8(6)
            new Vector3(1, 1, 1), // 9(7)
            new Vector3(0, 1, 0), // 10(4)
            new Vector3(1, 1, 0), // 11(5)
            new Vector3(0, 1, 1), // 12(6)
            new Vector3(1, 1, 1)  // 13(7)
        };
        uvs = new Vector2[] {
            new Vector2(1f/3, 2f/4),
            new Vector2(2f/3, 2f/4),
            new Vector2(1f/3, 3f/4),
            new Vector2(2f/3, 3f/4),
            new Vector2(1f/3, 1f/4),
            new Vector2(2f/3, 1f/4),
            new Vector2(1f/3, 0f),
            new Vector2(2f/3, 0f),
            new Vector2(1f/3, 1f),
            new Vector2(2f/3, 1f),
            new Vector2(0f, 1f/2),
            new Vector2(1f, 1f/2),
            new Vector2(0f, 3f/4),
            new Vector2(1f, 3f/4)
        };
        triangles = new int[] {
            4, 0, 1,
            1, 5, 4,
            0, 12, 2,
            0, 10, 12,
            2, 9, 3,
            2, 8, 9,
            3, 11, 1,
            3, 13, 11,
            5, 6, 4,
            5, 7, 6,
            0, 3, 1,
            0, 2, 3
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    public void Update()
    {
    }
}
