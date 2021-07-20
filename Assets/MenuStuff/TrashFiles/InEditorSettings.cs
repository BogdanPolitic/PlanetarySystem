using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Menu))]
public class InEditorSettings : Editor
{
    bool firstTime = true;
    public override void OnInspectorGUI()
    {
        Menu cube = (Menu)target;
        if (firstTime)
            cube.Awake();
    }
}
