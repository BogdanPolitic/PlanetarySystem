using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleRenderer : MonoBehaviour
{
    void setColor(float opacity)
    {
        Renderer r = GetComponent<Renderer>();
        Color color = r.material.color; color.a = opacity;
        r.material.color = color;
    }
    void Start()
    {
        setColor(-0.3f);
    }
}
