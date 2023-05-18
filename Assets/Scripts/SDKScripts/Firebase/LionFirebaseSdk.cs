#if HAS_LION_FIREBASE_SDK
using System.Threading.Tasks;
using UnityEngine;
using LionStudios.Suite.Core;
using LionStudios.Suite.Debugging;
using Firebase;
using Firebase.Crashlytics;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class LionFireBaseSDK : ILionSdk
{
    private string cachedGameKey;
    public int Priority => 0;

    private static LionFirebaseSettings _settings = new LionFirebaseSettings();

    public class LionFirebaseSettings : ILionSettingsInfo
    {
    }

    public void ApplySettings(ILionSettingsInfo newSettings)
    {
        _settings = (LionFirebaseSettings)newSettings;
    }

    public string Name { get; }

    public ILionSettingsInfo GetSettings()
    {
        if (_settings == null)
        {
            _settings = new LionFirebaseSettings();
        }

        return _settings;
    }

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
        LionDebug.Log("Resolving Firebase Dependencies");

        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log($"Firebase appid: {app.Options.AppId}");
                // Set a flag here for indicating that your project is ready to use Firebase.
                Crashlytics.IsCrashlyticsCollectionEnabled = true;
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        return Task.CompletedTask;
    }

    public void OnPostInitialize(LionCoreContext ctx)
    {
    }

    public void OnPreInitialize(LionCoreContext ctx)
    {
    }
}
#endif