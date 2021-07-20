using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove0 : MonoBehaviour
{
    static GameObject box;

    private void Start()
    {
        box = GameObject.Find("QuitRequestNoBox");
        Vector3 pos = box.transform.position;
        pos.z = 0.9f;
        box.transform.position = pos;
    }
    void Update()
    {
        //box.transform.Translate(new Vector3(0, 0.01f, 0));
    }
}
