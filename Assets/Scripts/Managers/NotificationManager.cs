using System;
using System.Collections;
using System.Text;
using Events.InGame.EventArgs;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;
using Utils;

public class NotificationManager : MonoSingleton<NotificationManager>
{
    private const string DefaultNotifChannel = "channel_default";

    IEnumerator Start()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
        Application.deepLinkActivated += OnDeepLinkActivated;

#if UNITY_ANDROID
        AndroidNotificationCenter.NotificationReceivedCallback receivedCallback =
 delegate(AndroidNotificationIntentData data)
        {
            var msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            Debug.Log(msg);

            NotificationOpenedEventArgs args = new NotificationOpenedEventArgs();
            args.NotificationName = data.Notification.Title;
            args.NotificationID = data.Id;
            LionAnalytics.NotificationOpened(args);
        };

        AndroidNotificationCenter.OnNotificationReceived += receivedCallback;
#elif UNITY_IOS
        yield return RequestIosAuthorization();
#endif
        yield return null;
    }

    public int Schedule(TimeSpan time, string identifier, string title, string body,
        bool showInForeground = true, bool repeats = false, string channel_category = DefaultNotifChannel,
        string iosThread = "thread1", string subtitle = "KingWing")
    {
        Debug.Log($"Scheduling notification with id '{identifier}'");

#if UNITY_ANDROID
        CreateAndroidNotifChannel(channel_category, "KingWing", body, Importance.Default);
        AndroidNotification notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = System.DateTime.Now.Add(time)
        };

        if (repeats)
        {
            notification.RepeatInterval = time;
        }

        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_0";
        // notification tracking only available on marshmellow and above
        return AndroidNotificationCenter.SendNotification(notification, channel_category);

#elif UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = time,
            Repeats = repeats
        };

        var notification = new iOSNotification()
        {
            Identifier = identifier,
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = showInForeground,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = channel_category,
            ThreadIdentifier = iosThread,
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
        iOSNotificationCenter.OnNotificationReceived += OniOSNotificationReceived;
#endif
        return 0;
    }
#if UNITY_IOS
    private void OniOSNotificationReceived(iOSNotification iOSNotification)
    {
        NotificationOpenedEventArgs args = new NotificationOpenedEventArgs
        {
            NotificationID = iOSNotification.Badge,
            NotificationName = iOSNotification.Title,
            CampaignName = iOSNotification.CategoryIdentifier,
            CohortGroup = iOSNotification.ThreadIdentifier,
            CommunicationState = iOSNotification.Data,
            NotificationLaunch = iOSNotification.Identifier
        };
        LionAnalytics.NotificationOpened(args);
    }
#endif

    public void RemoveAllDeliveredNotifs()
    {
        try
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
        }
        catch (Exception e)
        {
            LionDebug.LogWarning("Failed to remove all delivered notifications. Error: " + e.Message);
        }
    }

    public void RemoveAllScheduledNotifs()
    {
        try
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
        }
        catch (Exception e)
        {
            LionDebug.LogWarning("Failed to remove all scheduled notifications. Error: " + e.Message);
        }
    }

#if UNITY_ANDROID
    void CreateAndroidNotifChannel(string id, string name, string description, Importance importance = Importance.High)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
        {
            return;
        }
        
        AndroidNotificationChannel newChannel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = name,
            Importance = importance,
            Description = description
        };
        
        AndroidNotificationCenter.RegisterNotificationChannel(newChannel);
    }

    public void Unschedule(int identifier)
    {
        try
        {
            AndroidNotificationCenter.CancelScheduledNotification(identifier);
        }
        catch (Exception e)
        {
            LionDebug.LogWarning("Failed to unschedule notification with identifier '" + identifier + "'. Error: " + e.Message);
        }
    }
#endif

#if UNITY_IOS
    IEnumerator RequestIosAuthorization()
    {
        LionDebug.Log("Requesting iOS notification authorization");
        using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Requesting Notif Authorization:");
            sb.AppendLine("Finish: " + req.IsFinished);
            sb.AppendLine("Granted: " + req.Granted);
            sb.AppendLine("Error: " + req.Error);
            sb.AppendLine("Device Token: " + req.DeviceToken);
            LionDebug.Log(sb.ToString());
        }
    }

    public void Unschedule(string identifier)
    {
        try
        {
            iOSNotificationCenter.RemoveScheduledNotification(identifier);
        }
        catch (Exception e)
        {
            LionDebug.LogWarning("Failed to unschedule notification with identifier '" + identifier + "'. Error: " +
                                 e.Message);
        }
    }
#endif

    private void OnDeepLinkActivated(string url)
    {
        LionDebug.Log("Deep Link Activated: " + url);
    }
}