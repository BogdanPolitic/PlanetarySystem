using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeUIElements : MonoBehaviour
{
    [SerializeField] Text totalPoints;
    [SerializeField] Text secondsLeftText;
    [SerializeField] Text numberOfSecondsLeftText;
    [SerializeField] Text notificationsPanelTitleText;
    private static List<MainSceneUI.NotificationObject> notificationObjects;

    private int screenHeight = 0;
    private int screenWidth = 0;

    public static void InitializeNotificationsList(List<MainSceneUI.NotificationObject> objects)
    {
        notificationObjects = objects;
    }

    private void ResizeText(Text text, int defaultFontSize)
    {
        float heightRatio = Screen.height / (float)ValueSheet.defaultScreenHeight;
        float widthRatio = Screen.width / (float)ValueSheet.defaultScreenWidth;

        text.fontSize = (int)Mathf.Floor(defaultFontSize * Mathf.Min(heightRatio, widthRatio));
    }

    private void ResizeElements()
    {
        ResizeText(totalPoints, 33);
        ResizeText(secondsLeftText, 45);
        ResizeText(numberOfSecondsLeftText, 45);
        ResizeText(notificationsPanelTitleText, 22);

        foreach (MainSceneUI.NotificationObject notifObj in notificationObjects)
        {
            ResizeText(notifObj.textObject.GetComponent<Text>(), 20);
        }
    }

    private void Update()
    {
        if (Screen.height != screenHeight || Screen.width != screenWidth)
        {
            ResizeElements();
        }

        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }
}
