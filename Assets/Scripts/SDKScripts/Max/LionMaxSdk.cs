using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;
using LionStudios.Suite.Core;
using LionStudios.Suite.Ads;
using LionStudios.Suite.Debugging;
using LionStudios.Suite.Analytics;
// using LionStudios.Suite.GDPR;
using LionStudios.Suite.Analytics.Events;
using IngameDebugConsole;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;
using Utils;

/// <summary>
/// This is a sample script for integrating MAX SDK with Lion Suite.
/// This sample includes loading and showing ads, and connection with GDPR
/// Includes settings and functionality for initializing MAX.
/// 
/// NOTE - REQUIRES PACKAGES:
///     AppLovin MAX
///     Lion Suite - Ads
///     Lion Suite - Analytics
///     Lion Suite - GDPR
///     RemoteConfig.cs
/// </summary>
public class LionMaxSdk : ILionSdk, ILionAdProvider
{
    private static ApplovinMaxSettings _settings = new ApplovinMaxSettings();

    private float lastInterstitialShowTime;
    private float lastRvInterstitialShowTime;

    public int Priority => 1;

    public string Name => "MaxSDK";

    [LabelOverride("ApplovinMax Settings")]
    public class ApplovinMaxSettings : ILionSettingsInfo
    {
        [Header("General")] public string sdkKey = "";

        // rewarded ads
        [Header("Rewarded Ads")] public bool rewardedAdsDisabled = false;
        public string rewardedAdUnitIdAndroid = "";
        public string rewardedAdUnitIdIos = "";

        // interstitial ads
        [Header("Interstitial Ads")] public bool interstitialsDisabled = false;
        public string interstitialAdUnitIdAndroid = "";
        public string interstitialAdUnitIdIos = "";

        public float interstitialTimer;
        public float rvInterstitialTimer;
        public float interstitialStartTimer;
        public int interstitialStartLevel;

        // banner ads
        [Header("Banner Ads")] public bool bannersDisabled = false;
        public string bannerAdUnitIdAndroid = "";
        public string bannerAdUnitIdIos = "";

        // cross promo ads
        [Header("Cross Promo Ads")] public string crossPromoAdUnitIdAndroid = "";
        public string crossPromoAdUnitIdIos = "";
    }

    private string InterstitialAdUnit
    {
        get
        {
#if UNITY_IOS
            return _settings.interstitialAdUnitIdIos;
#elif UNITY_ANDROID
            return _settings.interstitialAdUnitIdAndroid;
#endif
        }
    }

    private string RewardedAdUnit
    {
        get
        {
#if UNITY_IOS
            return _settings.rewardedAdUnitIdIos;
#elif UNITY_ANDROID
            return _settings.rewardedAdUnitIdAndroid;
#endif
        }
    }

    private string BannerAdUnit
    {
        get
        {
#if UNITY_IOS
            return _settings.bannerAdUnitIdIos;
#elif UNITY_ANDROID
            return _settings.bannerAdUnitIdAndroid;
#endif
        }
    }

    private string CrossPromoAdUnit
    {
        get
        {
#if UNITY_IOS
            return _settings.crossPromoAdUnitIdIos;
#elif UNITY_ANDROID
            return _settings.crossPromoAdUnitIdAndroid;
#endif
        }
    }

    public void ApplySettings(ILionSettingsInfo newSettings)
    {
        _settings = (ApplovinMaxSettings)newSettings;
    }

    public ILionSettingsInfo GetSettings()
    {
        if (_settings == null)
        {
            _settings = new ApplovinMaxSettings();
        }

        return _settings;
    }

    public bool IsInitialized()
    {
        return MaxSdk.IsInitialized();
    }

    public void OpenMediationDebugger()
    {
        MaxSdk.ShowMediationDebugger();
    }

    private void OnMaxInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
    {
        if (MaxSdk.IsInitialized())
        {
            LionFacebookSdk facebookSDK = new LionFacebookSdk();
            AppLovinCrossPromo.Init();
            //MaxSdk.ShowMediationDebugger();
#if UNITY_IOS
            var systemVersion = Device.systemVersion;
            if (MaxSdkUtils.CompareVersions(systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser
                || MaxSdkUtils.CompareVersions(systemVersion, "14.5-beta") != MaxSdkUtils.VersionComparisonResult.Lesser)
            {
                // Debug.Log("FB ATT Set");
                SetAudienceNetwork(sdkConfiguration.AppTrackingStatus ==
                                   MaxSdkBase.AppTrackingStatus.Authorized);
            }
#endif

            LoadAds(LionAdTypeFlag.All);


            Debug.Log("MaxSDK init Complete.  Consent Dialog State = " + sdkConfiguration.ConsentDialogState);
            Debug.Log("GA Initialized!");
            LionAnalytics.ErrorEvent(ErrorEventType.Info, "test_error_message");
            facebookSDK.Initialize();
        }
        else
        {
            Debug.Log("Failed to init MaxSDK");
        }
    }


    private void SetAudienceNetwork(bool flag)
    {
        FB.Mobile.SetAdvertiserTrackingEnabled(flag);
        FB.Mobile.SetDataProcessingOptions(new string[] { });
    }

    public Task Initialize(LionCoreContext ctx)
    {
        ApplovinMaxSettings maxSettings = ctx.GetSettings<ApplovinMaxSettings>();
        LionDebug.LionDebugSettings debugSettings = ctx.GetSettings<LionDebug.LionDebugSettings>();

        string[] adUnitIds = new string[]
        {
            // rewarded
            RewardedAdUnit,
            // interstitial
            InterstitialAdUnit,
            // banner
            BannerAdUnit
        };


        // init callback
        MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;

        // init
        MaxSdk.SetSdkKey(maxSettings.sdkKey);
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(debugSettings.debugLogLevel == LionDebug.DebugLogLevel.Verbose);
        MaxSdk.SetExtraParameter("eifc", "iOf8gUDWef");
        MaxSdk.InitializeSdk(adUnitIds);
        DebugLogConsole.AddCommand("openmaxconsole", "Opens max debugger console window",
            OpenMediationDebugger);
        // Re-init ads whenever the GDPR dialog is closed
        // LionGDPR.OnClosed += delegate { LoadAds(LionAdTypeFlag.All); };

        return Task.CompletedTask;
    }


    public void OnPostInitialize(LionCoreContext ctx)
    {
    }

    public void OnPreInitialize(LionCoreContext ctx)
    {
    }

    #region Privacy Links

    private readonly Dictionary<string, string> _privacyLinks = new Dictionary<string, string>
    {
        { "UNITY_NETWORK", "https://unity3d.com/fr/legal/privacy-policy" },
        { "APPLOVIN", "https://www.applovin.com/privacy/" },
        { "ADMOB_NETWORK", "https://policies.google.com/privacy/update" },
        { "ADCOLONY_NETWORK", "https://www.adcolony.com/privacy-policy/" },
        { "AMAZON_NETWORK", "https://advertising.amazon.com/resources/ad-policy/en/gdpr" },
        { "CHARTBOOST_NETWORK", "https://answers.chartboost.com/en-us/articles/200780269" },
        { "FYBER_NETWORK", "https://www.digitalturbine.com/Privacy-policy/" },
        { "INMOBI_NETWORK", "https://www.inmobi.com/privacy-policy/" },
        { "IRONSOURCE_NETWORK", "https://ironsource.mobi/privacypolicy.html" },
        { "MINTEGRAL_NETWORK", "https://www.mintegral.com/en/privacy/" },
        { "SMAATO_NETWORK", "https://www.smaato.com/privacy/" },
        { "TIKTOK_NETWORK", "https://www.tiktok.com/legal/privacy-policy?lang=en#privacy-eea" },
        { "YANDEX_NETWORK", "https://yandex.com/legal/confidential/" },
        { "VUNGLE_NETWORK", "https://vungle.com/privacy/" },
        { "FACEBOOK_MEDIATE", "https://www.facebook.com/policy.php" },
        { "OGURY_NETWORK", "https://ogury.com/privacy-policy/ " },
        { "ADJUST", "https://www.adjust.com/terms/privacy-policy/" },
        { "FIREBASE_ANALYTICS", "https://firebase.google.com/support/privacy" }
    };

    public string[] GetPrivacyLinks()
    {
        List<string> privacyLinks = new List<string>();
        foreach (var kvp in _privacyLinks)
        {
            privacyLinks.Add(kvp.Value);
        }

        return privacyLinks.ToArray();
    }

    #endregion

    #region Ads

    public void LoadAds(LionAdTypeFlag adType)
    {
        if (!_settings.interstitialsDisabled)
        {
            MaxSdk.LoadInterstitial(InterstitialAdUnit);
        }

        if (!_settings.rewardedAdsDisabled)
        {
            MaxSdk.LoadRewardedAd(RewardedAdUnit);
        }

        if (!_settings.bannersDisabled)
        {
            MaxSdk.CreateBanner(BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
        }
    }

    public void ShowAd(LionAdType adType, string placement = null,
        int? level = null,
        Dictionary<string, object> additionalData = null)
    {
        int interstitialTimer = SatoriSDK.interstitialTimerInterval;
        int interstitialStartLevel = SatoriSDK.interstitialLevelInterval;

        if (!IsInitialized()) return;
        switch (adType)
        {
            case LionAdType.Rewarded:
                if (!_settings.rewardedAdsDisabled)
                {
                    MaxSdk.ShowRewardedAd(RewardedAdUnit, placement);
                }

                break;
            case LionAdType.Interstitial:
                if (!_settings.interstitialsDisabled
                    && level >= interstitialStartLevel
                    && (Time.time - lastInterstitialShowTime) >= interstitialTimer)
                {
                    MaxSdk.ShowInterstitial(InterstitialAdUnit, placement);
                    lastInterstitialShowTime = Time.time;
                }

                break;
            case LionAdType.Banner:
                if (!_settings.bannersDisabled)
                {
                    MaxSdk.CreateBanner(BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
                    MaxSdk.LoadBanner(BannerAdUnit);
                    MaxSdk.ShowBanner(BannerAdUnit);
                }

                break;
        }
    }

    public void HideAd(LionAdType adType, string placement = null, int? level = null,
        Dictionary<string, object> additionalData = null)
    {
        if (adType == LionAdType.Banner)
        {
            MaxSdk.HideBanner(BannerAdUnit);
        }
    }


    public bool IsAdReady(LionAdType adType)
    {
        switch (adType)
        {
            case LionAdType.Banner:
                return true;
            case LionAdType.Interstitial:
                return MaxSdk.IsInterstitialReady(InterstitialAdUnit);
            case LionAdType.Rewarded:
                return MaxSdk.IsRewardedAdReady(RewardedAdUnit);
            case LionAdType.CrossPromo:
                return true;
            default:
                return false;
        }
    }

    #endregion
}