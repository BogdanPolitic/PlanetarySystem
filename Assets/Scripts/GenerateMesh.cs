using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    public Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;

    GameObject cameraObject;
    Vector3 offsetFromCamera;

    float x, z;

    private void positionVisualModifier(out float x, out float z, float distanceFromCamera)
    {
        float camX = cameraObject.transform.position.x;
        float camZ = cameraObject.transform.position.z;

        float camRotationTan = Mathf.Abs(camZ / camX);

        float sgnX = -Mathf.Sign(camX);
        float sgnZ = -Mathf.Sign(camZ);

        x = sgnX * Mathf.Sqrt(Mathf.Pow(distanceFromCamera, 2) / (1 + Mathf.Pow(camRotationTan, 2)));
        z = sgnZ * Mathf.Sqrt(Mathf.Pow(distanceFromCamera, 2) - Mathf.Pow(x, 2));

        x += camX;
        z += camZ;
    }

    private void Awake()
    {
        mesh = new Mesh();
        vertices = new Vector3[4];
        triangles = new int[12];
        uv = new Vector2[4];

        GetComponent<MeshFilter>().mesh = mesh;

        cameraObject = FindObjectOfType<Camera>().gameObject;
    }

    void Start()
    {
        CalculateMesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        cameraObject.transform.position = new Vector3(8.0f, 2.0f, 0);
        transform.position = new Vector3(7.0f, 1.0f, 0);

        //transform.position = Vector3.zero;
        //Debug.Log("transform position INIT = " + transform.position);
        offsetFromCamera = cameraObject.transform.position - transform.position;
        //Debug.Log("transform position = " + transform + " and camera pos = " + cameraObject.transform.position);
    }

    // 2 3
    // 0 1

    void CalculateMesh()
    {
        vertices[0] = new Vector3(-0.5f, 0, -0.5f);
        vertices[1] = new Vector3(-0.5f, 0, 0.5f);
        vertices[2] = new Vector3(0.5f, 0, -0.5f);
        vertices[3] = new Vector3(0.5f, 0, 0.5f);


        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 0);
        uv[3] = new Vector2(1, 1);


        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        triangles[6] = 2;
        triangles[7] = 1;
        triangles[8] = 0;

        triangles[9] = 2;
        triangles[10] = 3;
        triangles[11] = 1;
    }


    void Update()
    {
        if (MainSceneUI.IsIntoInventory())
            return;

        positionVisualModifier(out x, out z, offsetFromCamera.magnitude);
        float quadOffset = RotateBase.initialQuadPositionOffset + RotateBase.offsetPosition;
        transform.position = new Vector3(x, cameraObject.transform.position.y * 0.8f + RotateBase.offsetPosition, z);
    }
}
