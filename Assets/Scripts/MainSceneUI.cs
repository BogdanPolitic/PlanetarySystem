using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    float screenWidth;
    float screenHeight;

    static bool paused;
    static bool intoInventory;
    float secondsLeft;

    string[] lastNotifications;
    int MAX_NOTIFICATIONS;

    public Texture inventoryChestTexture;
    public Texture currentPlanetTexture;

    public static bool isIntoInventory()
    {
        return intoInventory;
    }

    public static void exitInventory()
    {
        intoInventory = false;
    }

    public static bool gameIsPaused()
    {
        return paused;
    }

    public static void setPauseMode(bool mode)
    {
        paused = mode;
    }

    private string pauseButtonState()
    {
        if (paused) return "RESUME"; 
        return "PAUSE";
    }

    private void Start()
    {
        paused = false;
        intoInventory = false;
        secondsLeft = 60.0f;

        MAX_NOTIFICATIONS = Screen.height / 150;
        lastNotifications = new string[MAX_NOTIFICATIONS];
    }

    private void Update()
    {
        secondsLeft -= Time.deltaTime;
        currentPlanetTexture = (Texture)Resources.Load(SceneParameters.currentPlanet.pathToImage);
    }

    private void OnGUI()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        if (intoInventory)
            return;

        MAX_NOTIFICATIONS = Screen.height / 150;

        if (GUI.Button(new Rect(screenWidth * 0.5f, screenHeight * 0.1f, screenWidth * 0.075f, screenHeight * 0.05f), pauseButtonState()))
        {
            paused = !paused;
        }

        if (GUI.Button(new Rect(screenWidth * 0.05f, screenHeight * 0.05f, screenWidth * 0.2f, screenHeight * 0.15f), inventoryChestTexture))
        {
            intoInventory = true;
            setPauseMode(true);
        }

        GUI.Button(new Rect(screenWidth * 0.825f, screenHeight * 0.5f, screenWidth * 0.15f, screenHeight * 0.05f), "TIME LEFT: " + SceneParameters.secondsLeft);

        Rect notificationWindowRect = new Rect(screenWidth * 0.725f, screenHeight * 0.75f, screenWidth * 0.25f, screenHeight * 0.2f);
        GUI.Window(1, notificationWindowRect, windowTextForm, "Notifications");

        Rect planetInfoAndPlanetsLeftRect = new Rect(screenWidth * 0.01f, screenHeight * 0.775f, screenWidth * 0.15f, screenHeight * 0.2f);
        GUI.Window(2, planetInfoAndPlanetsLeftRect, windowPlanetInfoAndPlanetsLeft, "Next planet");
    }

    void windowTextForm(int formId)
    {
        GUILayout.BeginVertical();

        for (int i = 0; i < MAX_NOTIFICATIONS; i++)
        {
            GUILayout.Label("You won 15 points!");
        }

        //GUILayout.Box(aTexture);

        GUILayout.EndVertical();
    }

    void windowPlanetInfoAndPlanetsLeft(int formId)
    {
        GUILayout.BeginHorizontal();

        GUI.DrawTexture(new Rect(screenWidth * -0.03f, screenHeight * 0.04f, screenWidth * 0.15f, screenHeight * 0.15f), currentPlanetTexture, ScaleMode.ScaleToFit, true, SceneParameters.currentPlanet.representationScale);
        int numberOfPlanetsLeft = PathReader1.memory == null 
                                    ? SceneParameters.requiredNumberOfPlanets
                                    : SceneParameters.requiredNumberOfPlanets - PathReader1.memory.Count;
        GUI.Label(new Rect(screenWidth * 0.1f, screenHeight * 0.08f, screenWidth * 0.06f, screenHeight * 0.05f), numberOfPlanetsLeft + " left");

        GUILayout.EndHorizontal();
    }
}
