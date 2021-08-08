using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public enum NotificationType
    {
        // Rewards:
        PLANET_PLACED,

        // Penalties:
        PLANET_DESTROYED,

        // Warnings:
        MAX_AMOUNT_PLANETS,
        EMPTY_INVENTORY
    }

    private static List<NotificationType> notifications;

    public static void Initialize()
    {
        notifications = new List<NotificationType>();
    }

    public static void AddNotification(NotificationType notificationType)
    {
        notifications.Add(notificationType);
        if (notifications.Count > ValueSheet.maxNumberOfRecentNotifications)
            notifications.RemoveAt(0);
    }

    public static int GetCount()
    {
        return notifications.Count;
    }

    public static string GetLatestNotificationAt(int index)
    {
        return ValueSheet.notificationTypes[notifications[notifications.Count - index - 1]];
    }
}
