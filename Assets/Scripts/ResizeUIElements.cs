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

        // Generic resizing on all text components in the scene, the code is below.
        // The problem is that we can't know the default font size and there are two solutions for that.
        // 1) We have to remember the default font size from the beginning and store it somewhere (in a list). Additionally, whenever we add (gameObjects with) Text components, we have to add them to that list as well.
        // 2) We can take the actual screen size (width & height) and compare it to the last registered screen size (the last different state of the game window). At the time we're resizing, we check the ratio between the two, and we update the font size based on that ratio.
        // The second solution is less likely to work over big loads of window resizing, because the font size is an integer, thus the more we resize, the more we lose accuracy on the required text size. Indeed, the user cannot resize the screen that many times during a level, because the gameplay time is limited.
        // I've implemented none of these solutions as of now, 14 Aug 2021.
        // The basic technique from above, where we resize [SerializeField] Text components seems to be hardcode-ish, but the benefit is that there cannot be too many Texts on a game's window at a time, so it's all easily manageable.

        /*Object[] gameObjects = FindObjectsOfType(typeof(GameObject));
        foreach (Object o in gameObjects)
        {
            GameObject gO = (GameObject)o;
            if (gO.GetComponent<Text>() != null && gO.tag != "Unresizeable")
            {
                ResizeText(gO.GetComponent<Text>(), 45);    // defaultFontSize ??
            }
        }*/
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
