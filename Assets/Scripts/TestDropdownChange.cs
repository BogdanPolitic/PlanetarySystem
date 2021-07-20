using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDropdownChange : MonoBehaviour
{
    const float TOP_DROPDOWN_FADE_START = -13.3f;
    const float TOP_DROPDOWN_FADE_END = -12.2f;
    const float BOTTOM_DROPDOWN_FADE_START = -79.6f;
    const float BOTTOM_DROPDOWN_FADE_END = -80.7f;
    Material myMat;
    float dropdown;

    void Start()
    {
        myMat = gameObject.GetComponent<MeshRenderer>().material;
        dropdown = TOP_DROPDOWN_FADE_START;
    }

    void Update()
    {
        myMat.SetVector("DropdownFade", new Vector3(-30.5f, dropdown));
        dropdown += (TOP_DROPDOWN_FADE_END - TOP_DROPDOWN_FADE_START) * 0.01f;
    }
}
