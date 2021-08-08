using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour
{
    public static MainSceneUI instance;

    float screenWidth;
    float screenHeight;

    private bool showingUI;
    static bool paused;
    static bool intoInventory;
    float secondsLeft;

    string[] lastNotifications;
    int MAX_NOTIFICATIONS;

    public Texture inventoryChestTexture;
    public Texture currentPlanetTexture;
    [SerializeField] Text totalPointsText;
    [SerializeField] Text giveOrTakePointsText;
    public bool detectedTotalPointsChange;

    public Animator fadeAnimator;
    [SerializeField] Animator changeScoreTextAnimator;
    public bool playerIsRetiringLevel;

    public void ShowUI()
    {
        showingUI = true;
        RotateBase.GetInstance().ShowUI();
    }

    public void HideUI()
    {
        showingUI = false;
        RotateBase.GetInstance().HideUI();
    }

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

    public void SignalTotalPointsChange()
    {
        detectedTotalPointsChange = true;
    }

    private void Awake()
    {
        instance = this;

        showingUI = false;
        playerIsRetiringLevel = false;
        detectedTotalPointsChange = false;

        Notifications.Initialize();
    }

    public static MainSceneUI GetInstance()
    {
        return instance;
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
        currentPlanetTexture = (Texture)Resources.Load(LevelParameters.currentPlanet.pathToImage);

        if (LevelParameters.planetPlacedThisFrame || LevelParameters.planetsDestroyedThisFrame > 0)
            changeScoreTextAnimator.SetTrigger("changeText");
    }

    private void OnGUI()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        GUI.skin.window.clipping = TextClipping.Clip;

        if (intoInventory)
            return;

        MAX_NOTIFICATIONS = Screen.height / 150;

        if (detectedTotalPointsChange)
        {
            totalPointsText.text = "Total points: " + LevelParameters.totalPoints;
            giveOrTakePointsText.text = LevelParameters.planetPlacedThisFrame
                ? "+" + ValueSheet.pointsPerPlanetPlaced
                : (LevelParameters.planetsDestroyedThisFrame > 0)
                    ? "-" + (LevelParameters.planetsDestroyedThisFrame * ValueSheet.pointsPenaltyPerPlanetDestroyed)
                    : "+?"; // It should never reach this.
            detectedTotalPointsChange = false;
        }

        if (showingUI)
        {
            if (GUI.Button(
                new Rect(
                    screenWidth * (ValueSheet.inventoryChestButtonPositionX - ValueSheet.inventoryChestButtonDimensionX / 2.0f),
                    screenHeight * (ValueSheet.inventoryChestButtonPositionY - ValueSheet.inventoryChestButtonDimensionY / 2.0f),
                    screenWidth * ValueSheet.inventoryChestButtonDimensionX,
                    screenHeight * ValueSheet.inventoryChestButtonDimensionY
                ),
                inventoryChestTexture
            ))
            {
                intoInventory = true;
                setPauseMode(true);
            }

            GUI.Button(
                new Rect(
                    screenWidth * (ValueSheet.gameplayButtonsPositionX - ValueSheet.gameplayButtonsDimensionX / 2.0f),
                    screenHeight * (ValueSheet.gameplayTimeLeftButtonPositionY - ValueSheet.gameplayButtonsDimensionY / 2.0f),
                    screenWidth * ValueSheet.gameplayButtonsDimensionX,
                    screenHeight * ValueSheet.gameplayButtonsDimensionY
                ),
                "TIME LEFT: " + LevelParameters.secondsLeft
            );

            if (GUI.Button(
                new Rect(
                    screenWidth * (ValueSheet.gameplayButtonsPositionX - ValueSheet.gameplayButtonsDimensionX / 2.0f),
                    screenHeight * (ValueSheet.gameplayPauseButtonPositionY - ValueSheet.gameplayButtonsDimensionY / 2.0f),
                    screenWidth * ValueSheet.gameplayButtonsDimensionX,
                    screenHeight * ValueSheet.gameplayButtonsDimensionY
                ),
                pauseButtonState()
            ))
            {
                paused = !paused;
            }

            if (GUI.Button(
                new Rect(
                    screenWidth * (ValueSheet.gameplayButtonsPositionX - ValueSheet.gameplayButtonsDimensionX / 2.0f),
                    screenHeight * (ValueSheet.gameplayRetireLevelButtonPositonY - ValueSheet.gameplayButtonsDimensionY / 2.0f),
                    screenWidth * ValueSheet.gameplayButtonsDimensionX,
                    screenHeight * ValueSheet.gameplayButtonsDimensionY
                ),
                "RETIRE LEVEL"
            ))
            {
                playerIsRetiringLevel = true;
                HideUI();
                fadeAnimator.SetTrigger("ProceedToFadeOutAfterRetire");
            }

            Rect notificationWindowRect = new Rect(
                screenWidth * (ValueSheet.gameplayNotificationsWindowPositionX - ValueSheet.gameplayNotificationsWindowDimensionX / 2.0f),
                screenHeight * (ValueSheet.gameplayNotificationsWindowPositionY - ValueSheet.gameplayNotificationsWindowDimensionY / 2.0f),
                screenWidth * ValueSheet.gameplayNotificationsWindowDimensionX,
                screenHeight * ValueSheet.gameplayNotificationsWindowDimensionY
            );
            GUI.Window(1, notificationWindowRect, windowTextForm, "Notifications");

            Rect planetInfoAndPlanetsLeftRect = new Rect(
                screenWidth * (ValueSheet.nextPlanetInfoWindowPositionX - ValueSheet.nextPlanetInfoWindowDimensionX / 2.0f),
                screenHeight * (ValueSheet.nextPlanetInfoWindowPositionY - ValueSheet.nextPlanetInfoWindowDimensionY / 2.0f),
                screenWidth * ValueSheet.nextPlanetInfoWindowDimensionX,
                screenHeight * ValueSheet.nextPlanetInfoWindowDimensionY
            );
            GUI.Window(2, planetInfoAndPlanetsLeftRect, windowPlanetInfoAndPlanetsLeft, "Next planet");
        }
    }

    void windowTextForm(int formId)
    {
        GUILayout.BeginVertical();

        for (int i = 0; i < Mathf.Min(Notifications.GetCount(), MAX_NOTIFICATIONS); i++)
        {
            GUILayout.Label(Notifications.GetLatestNotificationAt(i));
        }

        //GUILayout.Box(aTexture);

        GUILayout.EndVertical();
    }

    void windowPlanetInfoAndPlanetsLeft(int formId)
    {
        GUILayout.BeginHorizontal();

        GUI.DrawTexture(new Rect(screenWidth * -0.03f, screenHeight * 0.04f, screenWidth * 0.15f, screenHeight * 0.15f), currentPlanetTexture, ScaleMode.ScaleToFit, true, LevelParameters.currentPlanet.representationScale);
        LevelParameters.numberOfPlanetsLeft = PathReader1.memory == null 
                                    ? LevelParameters.requiredNumberOfPlanets
                                    : LevelParameters.requiredNumberOfPlanets - PathReader1.memory.Count;
        GUI.Label(new Rect(screenWidth * 0.125f, screenHeight * 0.08f, screenWidth * 0.06f, screenHeight * 0.05f), LevelParameters.numberOfPlanetsLeft + " left");

        GUILayout.EndHorizontal();
    }
}
