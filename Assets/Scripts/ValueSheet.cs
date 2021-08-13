using System.Collections.Generic;
using UnityEngine;

public class ValueSheet
{
    public static int defaultScreenWidth = 1000;
    public static int defaultScreenHeight = 650;



    public static float minAllowedTrajectoryRadius = 0.9f;
    public static float maxAllowedTrajectoryRadius = 5.5f;

    public static float averagePlanetSize = 10.0f;
    public static float trajectoryOptimalDispersion = 0.05f; // Optimal distance between each consecutive 2 points on lines drawing, considering an average planet.



    public static float equilibriumDurationRequirementSeconds = 10.0f;



    public static Vector4 levelSecondsLeftColorStart = new Vector4(0, 1, 0, 1);
    public static Vector4 levelSecondsLeftColorEnd = new Vector4(1, 0, 0, 1);

    public static Vector4 countdownColorStart = new Vector4(0, 1, 0, 1);    // Starts green
    public static Vector4 countdownColorEnd = new Vector4(1, 0, 0, 1);      // Ends red

    public static Color unavailableLevelButtonTextColor = Color.black;



    public static string gameWonString = "Game won!";
    public static string gameLostString = "Game lost!";

    public static int pointsPerPlanetPlaced = 15;
    public static int pointsPenaltyPerPlanetDestroyed = 20;

    public static int totalPlanetsRewardDiv = 20;   // The number of all rewarded planet at the end of the level is going to be the total score divided by this variable (Mathf.Floor() must be perfomed).



    // The following many static variables describe how the gameplay UI windows and buttons are placed on the screen.
    // The preferred UI element description: the position of the element is in the middle of the element! (the middle = the mass center, the average of all corners' positions)
    // For a better understanging, check the UI elements' use in MainSceneUI.cs script.
    // ALL these dimensions and positions are relative to the screen size!
    public static float rotateBaseWindowDimensionX = 0.29f;
    public static float rotateBaseWindowDimensionY = 0.3f;
    public static float rotateBaseWindowPositionX = 0.83f;
    public static float rotateBaseWindowPositionY = 0.18f;

    // There are 2 buttons in the gameplay view. All of them (there're only 2) have the same X position as the rotateBase window. But the Y position is individual (thus, they are positioned vertically).
    public static float gameplayButtonsDimensionX = 0.15f;
    public static float gameplayButtonsDimensionY = 0.05f;
    public static float gameplayButtonsPositionX = rotateBaseWindowPositionX;
    //public static float gameplayTimeLeftButtonPositionY = 0.7f;
    public static float gameplayRetireLevelButtonPositonY = 0.58f;
    public static float gameplayPauseButtonPositionY = 0.45f;

    // The notifications window has the same X position and X dimension as the rotateBase window.
    public static float gameplayNotificationsWindowDimensionX = rotateBaseWindowDimensionX;
    public static float gameplayNotificationsWindowDimensionY = 0.2f;
    public static float gameplayNotificationsWindowPositionX = rotateBaseWindowPositionX;
    public static float gameplayNotificationsWindowPositionY = 0.85f;

    // The button to access the inventory window
    public static float inventoryChestButtonDimensionX = 0.2f;
    public static float inventoryChestButtonDimensionY = 0.15f;
    public static float inventoryChestButtonPositionX = 1.0f - (rotateBaseWindowPositionX + rotateBaseWindowDimensionX / 2.0f - inventoryChestButtonDimensionX / 2.0f);
    public static float inventoryChestButtonPositionY = rotateBaseWindowPositionY - rotateBaseWindowDimensionY / 2.0f + inventoryChestButtonDimensionY / 2.0f;

    // The next planet info window 
    public static float nextPlanetInfoWindowDimensionX = inventoryChestButtonDimensionX;
    public static float nextPlanetInfoWindowDimensionY = 0.2f;
    public static float nextPlanetInfoWindowPositionX = 1.0f - (gameplayNotificationsWindowPositionX + gameplayNotificationsWindowDimensionX / 2.0f - nextPlanetInfoWindowDimensionX / 2.0f);
    public static float nextPlanetInfoWindowPositionY = gameplayNotificationsWindowPositionY + gameplayNotificationsWindowDimensionY / 2.0f - nextPlanetInfoWindowDimensionY / 2.0f;



    // Notifications:
    public static int maxNumberOfRecentNotifications = 20;
    public static int maxNumberOfFittingNotifications = 4;
    public static Dictionary<Notifications.NotificationType, string> notificationTypes = new Dictionary<Notifications.NotificationType, string>()
    {
        /*// Rewards:
        { Notifications.NotificationType.PLANET_PLACED, "Planet successfully placed. You won " + pointsPerPlanetPlaced + " points!" },

        // Penalties:
        { Notifications.NotificationType.PLANET_DESTROYED, "A collision occured and some planets were destroyed! You lost " + pointsPenaltyPerPlanetDestroyed + " points per destruction!" },

        // Warnings:
        { Notifications.NotificationType.TRAJECTORY_NOT_ABLE_TO_CLOSE, "Your draw is incomplete. Trajectory not closing." },
        { Notifications.NotificationType.TRAJECTORY_TOO_SMALL, "Trajectory you've drawn is too small." },
        { Notifications.NotificationType.TRAJECTORY_TOO_LARGE, "Trajectory you've drawn is too large." },
        { Notifications.NotificationType.MAX_AMOUNT_PLANETS, "The required amount of planets has been reached. You cannot place more."},
        { Notifications.NotificationType.EMPTY_INVENTORY, "You cannot place any more planets because your inventory is empty." }*/


        // Rewards:
        { Notifications.NotificationType.PLANET_PLACED, "Planet successfully placed!" },

        // Penalties:
        { Notifications.NotificationType.PLANET_DESTROYED, "A collision occured!" },

        // Warnings:
        { Notifications.NotificationType.TRAJECTORY_NOT_ABLE_TO_CLOSE, "Your draw is incomplete." },
        { Notifications.NotificationType.TRAJECTORY_TOO_SMALL, "Trajectory is too small." },
        { Notifications.NotificationType.TRAJECTORY_TOO_LARGE, "Trajectory is too large." },
        { Notifications.NotificationType.MAX_AMOUNT_PLANETS, "No more planets required."},
        { Notifications.NotificationType.EMPTY_INVENTORY, "Empty inventory!" }
    };
}
