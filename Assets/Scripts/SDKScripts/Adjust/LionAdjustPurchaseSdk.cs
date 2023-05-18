using System.Threading.Tasks;
using com.adjust.sdk.purchase;
using LionStudios.Suite.Core;
using UnityEngine;

/// <summary>
/// 
/// NOTE - REQUIRES PACKAGES:
///     Adjust Purchasing SDK
/// </summary>
public class LionAdjustPurchaseSdk : ILionSdk
{
    private static AdjustPurchaseSettings _settings = new AdjustPurchaseSettings();

    private class AdjustPurchaseSettings : ILionSettingsInfo
    {
        public string IOSToken = "";
        public string androidToken = "";
        public AdjustPurchaseEnvironment purchaseEnvironment = AdjustPurchaseEnvironment.Sandbox;
    }


    public string Name => "LionAdjustPurchase";

    private string appToken
    {
        get
        {
#if UNITY_IOS
            return _settings.IOSToken;
#elif UNITY_ANDROID
            return _settings.androidToken;
#endif
        }
    }

    private AdjustPurchaseEnvironment _environment => _settings.purchaseEnvironment;

    public int Priority => 2;

    public void ApplySettings(ILionSettingsInfo newSettings)
    {
        _settings = (AdjustPurchaseSettings)newSettings;
    }

    public ILionSettingsInfo GetSettings()
    {
        if (_settings == null)
        {
            _settings = new AdjustPurchaseSettings();
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
        AdjustPurchaseEnvironment purchaseEnvironment = _environment;
        AdjustPurchaseConfig purchaseConfig = new AdjustPurchaseConfig(appToken, purchaseEnvironment);
        purchaseConfig.SetLogLevel(AdjustPurchaseLogLevel.Info);
        new GameObject("AdjustPurchase").AddComponent<AdjustPurchase>();
        AdjustPurchase.Init(purchaseConfig);
        Debug.Log("Adjust purchase initialized!");
        _isInitialized = true;

        return Task.CompletedTask;
    }

    public void OnPostInitialize(LionCoreContext ctx)
    {
    }

    public void OnPreInitialize(LionCoreContext ctx)
    {
    }
}