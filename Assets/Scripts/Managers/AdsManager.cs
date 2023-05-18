using System;
using com.adjust.sdk;
using Events.Level.EventArgs;
using LionStudios.OfflineModule;
using LionStudios.Suite.Ads;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events;
using LionStudios.Suite.Analytics.Events.CrossPromo.EventArgs;
using LionStudios.Suite.Analytics.Events.EventArgs;
using Utils;

public class AdsManager : IDisposable
{
    public AdsManager(Dispatcher dispatcher)
    {
        SubscribeBannerAdCallbacks(true);
        SubscribeInterstitialAdCallbacks(true);
        SubscribeRewardAdCallbacks(true);

        if(GameManager.Instance.CheckNoAdPurchase())
        {
            OfflineDetection.SetNoAds(true);
        }
    }


    void IDisposable.Dispose()
    {
        SubscribeBannerAdCallbacks(false);
        SubscribeInterstitialAdCallbacks(false);
        SubscribeRewardAdCallbacks(false);
    }


    public static void ShowInterstitialAd(EventArgs args)
    {
        var levelArgs = (LevelEventArgs)args;

        if (levelArgs.LevelNum % SatoriSDK.interstitialLevelInterval == 0)
        {
            if (!GameManager.Instance.CheckNoAdPurchase())
            {
                LionAds.ShowInterstitial<LionMaxSdk>("interstitial_placement", levelArgs.LevelNum);
                LionAds.LoadInterstitial();
                LionAds.LoadAds(LionAdTypeFlag.Banner);
                LionAnalytics.InterstitialShowRequested(new AdEventArgs
                {
                    Level = levelArgs.LevelNum,
                    Placement = "interstitial_placement",
                    Network = "interstitial_network"
                });
            }
        }
    }

    public static void ShowRewardedAd(EventArgs args)
    {
        LionAds.ShowRewarded<LionMaxSdk>("top");
        LionAnalytics.RewardVideoShowRequested(new AdEventArgs
        {
            Placement = "reward_requested",
            Network = "reward_requested_network"
        });
        LionAds.LoadAds();
    }


    public static void HideBannerAd()
    {
        LionAds.HideBanner<LionMaxSdk>("bottom");
        LionAds.LoadAds(LionAdTypeFlag.Banner);
    }

    public static void ShowBannerAd()
    {
        if (GameManager.Instance.CheckNoAdPurchase()) return;
        LionAnalytics.BannerShowRequested(new BannerEventArgs
        {
            Placement = "banner_requested",
            Network = "banner_requested_network"
        });
        LionAds.LoadAds(LionAdTypeFlag.Banner);
        LionAds.ShowBanner<LionMaxSdk>("bottom");
    }


    private void SubscribeRewardAdCallbacks(bool status)
    {
        if (status)
        {
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardAdRewardReceived;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            return;
        }
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardAdDisplayed;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardAdClicked;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardAdHidden;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardAdRewardReceived;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;

    }



    private void SubscribeInterstitialAdCallbacks(bool status)
    {
        if (status)
        {
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialAdHidden;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            return;
        }
        
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialAdDisplayed;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialAdHidden;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialAdClicked;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;

    }

    private void SubscribeBannerAdCallbacks(bool status)
    {
        if (status)
        {
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdDisplayed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClicked;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            return;
        }
        MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnBannerAdDisplayed;
        MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClicked;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;

    }

    private AdErrorType currentErrorType(MaxSdkBase.ErrorCode code)
    {
        AdErrorType typeReturn = code switch
        {
            MaxSdkBase.ErrorCode.Unspecified => AdErrorType.Undefined,
            MaxSdkBase.ErrorCode.NoFill => AdErrorType.NoFill,
            MaxSdkBase.ErrorCode.AdLoadFailed => AdErrorType.UnableToPrecache,
            MaxSdkBase.ErrorCode.AdDisplayFailed => AdErrorType.InternalError,
            MaxSdkBase.ErrorCode.NetworkError => AdErrorType.Offline,
            MaxSdkBase.ErrorCode.NetworkTimeout => AdErrorType.InternalError,
            MaxSdkBase.ErrorCode.NoNetwork => AdErrorType.Offline,
            MaxSdkBase.ErrorCode.FullscreenAdAlreadyShowing => AdErrorType.UnableToPrecache,
            MaxSdkBase.ErrorCode.FullscreenAdNotReady => AdErrorType.InvalidRequest,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };

        return typeReturn;
    }

    #region Reward Callbacks

   

    private void OnRewardAdDisplayed(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.RewardVideoStart(adInfo.Placement, adInfo.NetworkName);
        LionAnalytics.RewardVideoShow(adInfo.Placement, adInfo.NetworkName);
    }
    
    private void OnRewardAdHidden(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.RewardVideoEnd(adInfo.Placement, adInfo.NetworkName);
        LionAnalytics.RewardVideoCollect(adInfo.Placement, adInfo.NetworkName);
    }

    private void OnRewardAdClicked(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.RewardVideoClick(adInfo.Placement, adInfo.NetworkName);
    }

    private void OnRewardAdRewardReceived(string adUnit, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.RewardVideoCollect(adInfo.Placement, adInfo.NetworkName);
    }
    
    // Attach callbacks based on the ad format(s) you are using
  
    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        
        var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        adRevenue.setRevenue(impressionData.Revenue, "USD");
        adRevenue.setAdRevenueNetwork(impressionData.NetworkName);
        adRevenue.setAdRevenueUnit(impressionData.AdUnitIdentifier);
        adRevenue.setAdRevenuePlacement(impressionData.Placement);
        Adjust.trackAdRevenue(adRevenue);
    }

    #endregion

    #region Interstitial Callbacks

  
    private void OnInterstitialAdDisplayed(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.InterstitialStart(adInfo.Placement, adInfo.NetworkName);
        LionAnalytics.InterstitialShow(adInfo.Placement, adInfo.NetworkName);
    }

    private void OnInterstitialAdHidden(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.InterstitialEnd(adInfo.Placement, adInfo.NetworkName);
    }

    private void OnInterstitialAdClicked(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.InterstitialClick(adInfo.Placement, adInfo.NetworkName);
    }

    #endregion

    #region Banner Callbacks

  
    private void OnBannerAdDisplayed(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.BannerShow(adInfo.Placement, adInfo.NetworkName);
    }
    

    private void OnBannerAdClicked(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        if (string.IsNullOrEmpty(adUnit))
        {
            return;
        }

        LionAnalytics.BannerHide(adInfo.Placement, adInfo.NetworkName);
    }

    #endregion

    #region Cross Promo Ad Methods

    public static void HideCrossPromo()
    {
        AppLovinCrossPromo.Instance().HideMRec();
        CrossPromoEventArgs crossPromoEventArgs = new CrossPromoEventArgs
        {
            Level = LevelManager.Instance.CurrentLevel,
            Placement = "crossPromoHide"
        };
        LionAnalytics.CrossPromoEnd(crossPromoEventArgs);
    }

    public static void ShowCrossPromo()
    {
        if (!GameManager.Instance.CheckNoAdPurchase())
        {
            LionAnalytics.CrossPromoClick("endOfLevel", "unknown", LevelManager.Instance.CurrentLevel);
            LionAnalytics.CrossPromoLoadFail("unknown", LevelManager.Instance.CurrentLevel);
            LionAnalytics.CrossPromoShowFail("endOfLevel", "unknown", AdErrorType.Undefined,
                LevelManager.Instance.CurrentLevel);
            LionAnalytics.CrossPromoStart("endOfLevel", "unknown", LevelManager.Instance.CurrentLevel);

            AppLovinCrossPromo.Instance().ShowMRec(80.0f, 20.0f, 20.0f, 20.0f, 0);

            CrossPromoEventArgs crossPromoEventArgs = new CrossPromoEventArgs
            {
                Level = LevelManager.Instance.CurrentLevel,
                Placement = "crossPromoShow"
            };
            LionAnalytics.CrossPromoLoad(crossPromoEventArgs);
            LionAnalytics.CrossPromoShow(crossPromoEventArgs);
        }
    }
    #endregion
}