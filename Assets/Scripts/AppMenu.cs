using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppMenu : MonoBehaviour
{
    private void OnGUI()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        int windowWidth = 180;
        int windowHeight = 300;

        int windowX = (Screen.width - windowWidth) / 2;
        int windowY = (Screen.height - windowHeight) / 2;

        // pentru gui, decomentez astea doua:
        //Rect menuWindowRect = new Rect(windowX, windowY, windowWidth, windowHeight);
        //GUILayout.Window(0, menuWindowRect, drawWindowContent, "Menu");
    }

    void drawWindowContent(int windowId)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        int windowWidth = 180;
        int windowHeight = 300;

        int windowX = (Screen.width - windowWidth) / 2;
        int windowY = (Screen.height - windowHeight) / 2;

        int upperSpace = 10;
        int spaceInBetweenButtons = 15;
        int buttonHeight = 35;

        GUILayout.Label("", GUILayout.Height(upperSpace));
        GUILayout.BeginVertical();
        if (GUILayout.Button("New game", GUILayout.Height(buttonHeight)))
        {
            // game begins
        }
        GUILayout.Label("", GUILayout.Height(spaceInBetweenButtons));
        if (GUILayout.Button("Load game", GUILayout.Height(buttonHeight)))
        {
            // go to levels table
        }
        GUILayout.Label("", GUILayout.Height(spaceInBetweenButtons));
        if (GUILayout.Button("Statistics", GUILayout.Height(buttonHeight)))
        {
            // go to levels table
        }
        GUILayout.Label("", GUILayout.Height(spaceInBetweenButtons));
        if (GUILayout.Button("Settings", GUILayout.Height(buttonHeight)))
        {
            // go to levels table
        }
        GUILayout.Label("", GUILayout.Height(spaceInBetweenButtons));
        if (GUILayout.Button("Quit game", GUILayout.Height(buttonHeight)))
        {
            // go to levels table
        }
        GUILayout.EndVertical();
    }
}
