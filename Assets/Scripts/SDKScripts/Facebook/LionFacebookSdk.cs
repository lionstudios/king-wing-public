using System.Threading.Tasks;
using UnityEngine;
using LionStudios.Suite.Core;
using LionStudios.Suite.Debugging;
using Facebook.Unity;
using System.Runtime.InteropServices;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
public class LionFacebookSdk 
{
    private bool _isInitialized;
    public bool IsInitialized()
    {
        return _isInitialized;
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            LionDebug.Log("Failed to Initialize the Facebook SDK");
        }

    }


    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void FacebookLionSdkInitialize()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
            Debug.Log("FB INIT");
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void Initialize()
    {
        FacebookLionSdkInitialize();
    }

}
