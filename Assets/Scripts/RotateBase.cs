using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RotateBase : MonoBehaviour
{
    public GUISkin sliderBoxStyle;
    private bool GUIwasSetup;
    private Rect windowRect0;

    public static float initialQuadPositionOffset = 0;          // referring only to the Y (relative) axis
    private float initialBasePositionOffset = 0;                // referring only to the Y (relative) axis


    public static float xRotationValue = 90.0f;
    public static float yRotationValue = 0;
    public static float zRotationValue = 0;
    public static float offsetPosition = 0;

    private float xRotationInit = 0;
    private float yRotationInit = 0;
    private float zRotationInit = 0;
    private float offsetPositionInit = 0;

    private int framesSinceChange = 0;
    const int VISIBLE_FOR_FRAMES = 200;
    private bool invisible;

    private float divWidth = 3.5f;
    //private float divHeight = 4.35f;
    private float divHeight = 3.35f;
    private float widthSize = 0.69f;
    private float heightSize = 0.03f;

    GameObject guidingQuad;

    void Start()
    {
        framesSinceChange = 0;
        guidingQuad = GameObject.Find("VisualBaseModifier");
        invisible = false;
        initialQuadPositionOffset = guidingQuad.transform.position.y;
        initialBasePositionOffset = transform.position.y;

        GUIwasSetup = false;
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (MainSceneUI.isIntoInventory())
            return;

        if (!GUIwasSetup)
        {
            GUI.skin = sliderBoxStyle;
            GUIwasSetup = true;
        }

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var windowWidth = screenWidth / divWidth;
        var windowHeight = screenHeight / divHeight;
        var windowX = screenWidth * widthSize;
        var windowY = screenHeight * heightSize;

        windowRect0 = new Rect(windowX, windowY, windowWidth, windowHeight);
        GUILayout.Window(0, windowRect0, SliderBoxForm, "Change orbit angle");
    }

    void SliderBoxForm(int formId)
    {
        if (framesSinceChange >= VISIBLE_FOR_FRAMES)
        {
            if (invisible == false)
            {
                guidingQuad.SetActive(false);
                invisible = true;
            }
        }
        else if (invisible == true)
        {
            guidingQuad.SetActive(true);
            invisible = false;
        }

        xRotationInit = xRotationValue;
        yRotationInit = yRotationValue;
        zRotationInit = zRotationValue;
        offsetPositionInit = offsetPosition;

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        xRotationValue = GUILayout.VerticalSlider(xRotationValue, 0.0f, 180.0f);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        yRotationValue = GUILayout.VerticalSlider(yRotationValue, -180.0f, 180.0f);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        zRotationValue = GUILayout.VerticalSlider(zRotationValue, -90.0f, 90.0f);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        offsetPosition = GUILayout.HorizontalSlider(offsetPosition, -0.5f, 0.5f);


        if (xRotationValue - xRotationInit != 0.0f 
            || yRotationValue - yRotationInit != 0.0f 
            || zRotationValue - zRotationInit != 0.0f
            || offsetPosition - offsetPositionInit != 0.0f)
        {
            transform.rotation = Quaternion.Euler(xRotationValue, yRotationValue, zRotationValue);
            transform.position = new Vector3(transform.position.x, 
                                            initialBasePositionOffset + offsetPosition * 5f, 
                                            transform.position.z);
            framesSinceChange = 0;
        }

        framesSinceChange++;
    }
}
