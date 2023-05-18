
using System.Collections.Generic;
using LionStudios.Suite.Ads;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Core;
using LionStudios.Suite;

public static class SatoriSDK
{
    private const string ads_interstitial_interval_level = "ads_interstitial_interval_level";
    private const string progression_levelStartTimer = "progression_levelStartTimer";
    private const string offer_defaultCoinGive = "offer_defaultCoinGive";
    private const string ads_interstitialTimer = "ads_interstitialTimer";

    public static int interstitialLevelInterval => SatoriController.GetValue<int>(ads_interstitial_interval_level, 2);
    public static int levelStartTimer => SatoriController.GetValue<int>(progression_levelStartTimer, 0);
    public static int defaultCoins => SatoriController.GetValue<int>(offer_defaultCoinGive, 50);
    public static int interstitialTimerInterval => SatoriController.GetValue<int>(ads_interstitialTimer, 30);

    private static void KingWingEvent(string name, string value, Dictionary<string, string> metadata)
    {
        metadata ??= new Dictionary<string, string>();
        metadata.Add("LAN version", LionAnalytics.Version);
        metadata.Add("LCA version", LionCore.Version);
        metadata.Add("LAD version", LionAds.Version);
        SatoriController.LogEvent(name, value, metadata);
    }


    public static void SatoriLevelStartEvent(int levelNumber, Dictionary<string, string> additionalData)
    {
        KingWingEvent("levelStart", levelNumber.ToString(), additionalData);
    }

    public static void SatoriLevelAbandonedEvent(int levelNumber, Dictionary<string, string> additionalData)
    {
        KingWingEvent("levelAbandoned", levelNumber.ToString(), additionalData);
    }

    public static void SatoriLevelFailEvent(int levelNumber, Dictionary<string, string> additionalData)
    {
        KingWingEvent("levelFailed", levelNumber.ToString(), additionalData);
    }

    public static void SatoriLevelCompleteEvent(int levelNumber, Dictionary<string, string> additionalData)
    {
        KingWingEvent("levelComplete", levelNumber.ToString(), additionalData);
    }

    public static void SatoriIAPPurchaseEvent(string productId, Dictionary<string, string> additionalData)
    {
        KingWingEvent("iap_purchase", productId, additionalData);
    }

    public static void SatoriIAPPurchaseFailEvent(string productId, Dictionary<string, string> additionalData)
    {
        KingWingEvent("iap_fail", productId, additionalData);
    }
}