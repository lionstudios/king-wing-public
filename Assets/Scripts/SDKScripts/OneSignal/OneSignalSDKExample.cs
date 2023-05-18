using System.Collections.Generic;
using System.Threading.Tasks;
using Events.InGame.EventArgs;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Core;
using OneSignalSDK;
using OneSignalSDK.Debug.Models;
using OneSignalSDK.Notifications.Models;
using OneSignalSDK.User.Models;
using UnityEngine;

/// <summary>
/// This is a sample script for integrating One Signal in Project.
/// Includes settings and functionality for initializing One Signal SDK.
/// 
/// NOTE - REQUIRES PACKAGES:
///     One Signal SDK
///     Lion Suite - Core
/// </summary>
public class OneSignalSDKExample : ILionSdk
{
    [LabelOverride("OneSignal Settings")]
    public class OneSignalSettings : ILionSettingsInfo
    {
        public string OneSignalKey = "";
        public LogLevel alertLevel = LogLevel.Verbose;
        public LogLevel infoLevel = LogLevel.Verbose;
        public bool requiresPrivacyConsent = true;
        public bool shareLocation = true;
    }

    private static OneSignalSettings _settings;

    public int Priority => 2;

    public string[] GetPrivacyLinks()
    {
        return new string[] { };
    }

    private bool _isInitialized;

    public bool IsInitialized()
    {
        return _isInitialized;
    }

    public Task Initialize(LionCoreContext ctx)
    {
        Debug.Log("One Signal initialized!");
        _isInitialized = true;
        OneSignal.Default.Initialize(_settings.OneSignalKey);
        OneSignal.Default.Debug.AlertLevel = _settings.alertLevel;
        OneSignal.Default.Debug.LogLevel = _settings.infoLevel;
        OneSignal.Default.RequiresPrivacyConsent = _settings.requiresPrivacyConsent;
        OneSignal.Default.Location.IsShared = _settings.shareLocation;
        OneSignal.Default.PrivacyConsent = true;
        OneSignal.Default.Notifications.Clicked += NotificationOpened;
        OneSignal.Default.Notifications.WillShow += NotificationReceived;
        OneSignal.Default.Notifications.PermissionChanged += NotificationPermissionChanged;
        OneSignal.Default.User.PushSubscription.Changed += PushStateChanged;

        return Task.CompletedTask;
    }

    private void PushStateChanged(PushSubscriptionState current)
    {
        Debug.Log($"Push state changed to: {JsonUtility.ToJson(current)}");
    }

    private void NotificationPermissionChanged(bool current)
    {
        Debug.Log($"Notification Permissions changed to: {current}");
    }

    private Notification NotificationReceived(Notification notification)
    {
        var additionalData = notification.additionalData != null
            ? Json.Serialize(notification.additionalData)
            : null;
        
        Debug.Log($"Notification was received in foreground: {JsonUtility.ToJson(notification)}\n{additionalData}");
        return notification;
    }

    private void NotificationOpened(NotificationClickedResult result)
    {
        Debug.Log($"Notification Opened with status: {result}");
        NotificationOpenedEventArgs args = new NotificationOpenedEventArgs
        {
            NotificationID = int.Parse(result.notification.notificationId),
            NotificationName = result.notification.title,
#if UNITY_IOS
            CohortGroup = result.notification.threadId,

#elif UNITY_ANDROID
            CohortGroup = result.notification.groupKey,
#endif
            NotificationLaunch = result.notification.launchURL
        };
        Dictionary<string, object> additionalData = new Dictionary<string, object> { { "Raw Payload", result.notification.rawPayload } };
        LionAnalytics.NotificationOpened(args,additionalData);
    }

    public void OnPostInitialize(LionCoreContext ctx)
    {
    }

    public void OnPreInitialize(LionCoreContext ctx)
    {
    }

    public void ApplySettings(ILionSettingsInfo newSettings)
    {
        _settings = (OneSignalSettings)newSettings;
    }

    public ILionSettingsInfo GetSettings()
    {
        if (_settings == null)
        {
            _settings = new OneSignalSettings();
        }

        return _settings;
    }


    public string Name { get; }
}