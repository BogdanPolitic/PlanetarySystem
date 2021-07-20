using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyzer : MonoBehaviour
{
    Mesh mesh;
    Mesh sphereMesh;
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log("vertices = " + mesh.vertices.Length + " and mesh.triangles = " + mesh.triangles.Length + " and mesh.uvs = " + mesh.uv.Length);
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            //mesh.uv[i] = 
        }

        //sphereMesh = GameObject.Find("SferaObj").GetComponent<MeshFilter>().mesh;
        //sphereMesh = (Resources.Load("Scenes/SferaObj.obj") as GameObject).transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        //sphereMesh = GameObject.Find("default").GetComponent<MeshFilter>().mesh;
        //Debug.Log("sphere mesh has " + sphereMesh.uv.Length);

        //mesh.uv = sphereMesh.uv;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
