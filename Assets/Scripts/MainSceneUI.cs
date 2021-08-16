using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour
{
    public static MainSceneUI instance;

    public enum SceneMode
    {
        GAMEPLAY,
        INVENTORY
    }

    public class NotificationObject
    {
        public GameObject containerObject;
        public GameObject backgroundObject;
        public GameObject textObject;

        public void SetActive(bool active)
        {
            containerObject.SetActive(active);
        }

        public void CopyTransformRect(GameObject original, GameObject destination)
        {
            RectTransform originalC = original.GetComponent<RectTransform>();
            RectTransform destinationC = destination.GetComponent<RectTransform>();

            destinationC.anchorMin = originalC.anchorMin;
            destinationC.anchorMax = originalC.anchorMax;
            destinationC.offsetMin = originalC.offsetMin;
            destinationC.offsetMax = originalC.offsetMax;
            destinationC.anchoredPosition = originalC.anchoredPosition;
            destinationC.sizeDelta = originalC.sizeDelta;
            destinationC.localScale = new Vector3(1, 1, 1);
        }

        public NotificationObject(GameObject containerObject, bool isTemplate)
        {
            this.containerObject = containerObject;

            foreach (Transform child in containerObject.transform)
            {
                if (child.gameObject.GetComponent<Text>() != null)
                    textObject = child.gameObject;
                if (child.gameObject.GetComponent<Image>() != null)
                    backgroundObject = child.gameObject;
            }

            if (!isTemplate)
            {
                CopyTransformRect(
                    instance.notificationTemplateObject.containerObject,
                    containerObject
                );

                CopyTransformRect(
                    instance.notificationTemplateObject.backgroundObject,
                    backgroundObject
                );

                CopyTransformRect(
                    instance.notificationTemplateObject.textObject,
                    textObject
                );
            }

            SetActive(false);   // At the start, notification objects will be there, but none will be active yet, because, obviously, no notification was issued yet.
        }
    }

    [SerializeField] Canvas canvas;
    [SerializeField] Button pauseButton;
    [SerializeField] Button retireLevelButton;

    float screenWidth = 0;
    float screenHeight = 0;

    private bool showingUI;
    static bool paused;
    static bool intoInventory;
    float secondsLeft;

    [SerializeField] GameObject notificationTemplate;
    [SerializeField] GameObject notificationsPanel;
    public NotificationObject notificationTemplateObject;
    List<NotificationObject> notificationObjects;

    public Texture inventoryChestTexture;
    public Texture currentPlanetTexture;
    [SerializeField] Text secondsLeftText;
    [SerializeField] Text totalPointsText;
    [SerializeField] Text giveOrTakePointsText;
    public bool detectedTotalPointsChange;

    public Animator fadeAnimator;
    [SerializeField] Animator secondsLeftAnimator;
    [SerializeField] Animator changeScoreTextAnimator;
    public bool playerIsRetiringLevel;

    public static bool IsIntoInventory()
    {
        return intoInventory;
    }

    public static void ExitInventory()
    {
        intoInventory = false;
    }

    public static bool GameIsPaused()
    {
        return paused;
    }

    public static void SetSceneMode(SceneMode mode)
    {
        paused = (mode == SceneMode.INVENTORY);
        Time.timeScale = paused ? 0 : 1;

        if (paused) instance.HideUI(); else instance.ShowUI();
    }

    // EVENT: After pressing the "Pause" button.
    public static void SwitchPauseState()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;

        //if (paused) instance.HideUI(); else instance.ShowUI();
    }

    // EVENT: After pressing the "Retire level" button.
    public void RetireLevel()
    {
        playerIsRetiringLevel = true;
        HideUI();
        fadeAnimator.SetTrigger("ProceedToFadeOutAfterRetire");
    }

    private string PauseButtonState()
    {
        if (paused) return "RESUME"; 
        return "PAUSE";
    }

    public void SignalTotalPointsChange()
    {
        detectedTotalPointsChange = true;
    }

    public void AddNotificationObject()
    {

    }

    private void Awake()
    {
        instance = this;

        showingUI = false;
        playerIsRetiringLevel = false;
        detectedTotalPointsChange = false;

        notificationObjects = new List<NotificationObject>();

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

        canvas.gameObject.SetActive(false);

        notificationTemplateObject = new NotificationObject(notificationTemplate, true);
        InitializeNotificationsList();
    }

    private void Update()
    {
        currentPlanetTexture = (Texture)Resources.Load(LevelParameters.currentPlanet.pathToImage);

        if (LevelParameters.planetPlacedThisFrame || LevelParameters.planetsDestroyedThisFrame > 0)
            changeScoreTextAnimator.SetTrigger("changeText");
    }

    public void HideUI()
    {
        showingUI = false;
        RotateBase.GetInstance().HideUI();  // If we would've had all the UI in the same script, this call would not have been necesarry.

        canvas.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        showingUI = true;
        RotateBase.GetInstance().ShowUI();

        canvas.gameObject.SetActive(true);
        secondsLeftAnimator.SetTrigger("StartLevelMainCountdown");
    }

    private void InitializeNotificationsList()
    {
        for (int notifIndex = 0; notifIndex < ValueSheet.maxNumberOfFittingNotifications; notifIndex++)
        {
            GameObject notificationGameObject = Instantiate(notificationTemplate, notificationsPanel.transform);
            notificationObjects.Add(new NotificationObject(notificationGameObject, false));
        }

        ResizeUIElements.InitializeNotificationsList(notificationObjects);
    }

    public void UpdateNotificationObjects(int currentNumberOfFittingNotifications)
    {
        if (notificationObjects == null || notificationObjects.Count == 0) return;

        RectTransform templateBackgroundRect = notificationTemplateObject.backgroundObject.GetComponent<RectTransform>();
        RectTransform templateTextRect = notificationTemplateObject.textObject.GetComponent<RectTransform>();

        int notifIndex = 0;
        foreach (NotificationObject notifObj in notificationObjects)
        {
            if (notifIndex >= currentNumberOfFittingNotifications)
                continue;

            notifObj.SetActive(true);

            RectTransform notifObjBackgroundRect = notifObj.backgroundObject.GetComponent<RectTransform>();
            notifObjBackgroundRect.anchorMin = new Vector2(
                templateBackgroundRect.anchorMin.x,
                templateBackgroundRect.anchorMin.y - notifIndex * 0.05f
            );
            notifObjBackgroundRect.anchorMax = new Vector2(
                templateBackgroundRect.anchorMax.x,
                templateBackgroundRect.anchorMax.y - notifIndex * 0.05f
            );

            RectTransform notifObjTextRect = notifObj.textObject.GetComponent<RectTransform>();
            notifObjTextRect.anchorMin = new Vector2(
                templateTextRect.anchorMin.x,
                templateTextRect.anchorMin.y - notifIndex * 0.05f
            );
            notifObjTextRect.anchorMax = new Vector2(
                templateTextRect.anchorMax.x,
                templateTextRect.anchorMax.y - notifIndex * 0.05f
            );

            notifObj.textObject.GetComponent<Text>().text = Notifications.GetLatestNotificationAt(notifIndex);
            notifIndex++;
        }

        notificationObjects[0].textObject.GetComponent<Animator>().SetTrigger("StretchText");
    }

    private void OnGUI()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        GUI.skin.window.clipping = TextClipping.Clip;

        if (intoInventory)
            return;

        secondsLeftText.text = LevelParameters.secondsLeft.ToString();
        LevelSpecification.Initialize();
        float levelDurationLeftPercentage = 1.0f - (float)LevelParameters.secondsLeft / LevelSpecification.levelSpecs[LevelParameters.currentLevel].totalTime;
        secondsLeftText.color = MyMath.InterpolateBetweenVector4s(
            ValueSheet.levelSecondsLeftColorStart, 
            ValueSheet.levelSecondsLeftColorEnd, 
            levelDurationLeftPercentage
        );
        secondsLeftText.GetComponent<Shadow>().effectColor = MyMath.InterpolateBetweenVector4s(
            ValueSheet.levelSecondsLeftColorEnd, 
            ValueSheet.levelSecondsLeftColorStart, 
            levelDurationLeftPercentage
        );

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
                SetSceneMode(SceneMode.INVENTORY);
            }

            Rect planetInfoAndPlanetsLeftRect = new Rect(
                screenWidth * (ValueSheet.nextPlanetInfoWindowPositionX - ValueSheet.nextPlanetInfoWindowDimensionX / 2.0f),
                screenHeight * (ValueSheet.nextPlanetInfoWindowPositionY - ValueSheet.nextPlanetInfoWindowDimensionY / 2.0f),
                screenWidth * ValueSheet.nextPlanetInfoWindowDimensionX,
                screenHeight * ValueSheet.nextPlanetInfoWindowDimensionY
            );
            GUI.Window(2, planetInfoAndPlanetsLeftRect, windowPlanetInfoAndPlanetsLeft, "Next planet");
        }
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
