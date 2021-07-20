using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCameraYAxis : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.Rotate(new Vector3(90, 90, 0));
    }
}
