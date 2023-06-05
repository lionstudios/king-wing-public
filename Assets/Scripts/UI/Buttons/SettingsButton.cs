using System;
using LionStudios.Suite.Analytics;
using Utils;
using LionStudios.Suite.SettingsMenu;
using LionStudios.Suite.Ads;
using UnityEngine;
using AppLovinMax.Internal.API;

public class SettingsButton : BaseButton
{
    private void Start()
    {
        LionSettingsMenu.OnSettingsOpened += () =>
        {
            AdsManager.HideBannerAd();
        };

        LionSettingsMenu.OnSettingsClosed += () =>
        {
            AdsManager.ShowBannerAd();
        };

        LionSettingsMenu.OnFeedbackButtonClicked += () =>
        {
            Debug.Log("Feedback button was clicked!");
        };

#if UNITY_IOS
        LionSettingsMenu.OnRestorePurchase += () =>
        {
            PurchaseManager.Instance.RestorePurchases(OnRestoreStatus);
        };
#endif

        // If a user is in a GDPR compliant country, then show GDPR instead of Lion privacy web page (Example is using MaxSDKs GDPR solution)
        if (CFService.CFType == CFType.Detailed)
        {
            LionSettingsMenu.PrivacyOpensGDPR(true);
            LionSettingsMenu.OnPrivacyButtonClicked += () =>
            {
                CFService.SCF(OnConsentFlowError);
            };
        }
    }

    protected override void OnClick()
    {
        GameManager.Instance.PauseGamePlayElements();
        LionAnalytics.UiInteraction("Click", "MenuButton", "CornerButtons", "menuNavigation");
        LionSettingsMenu.Show("TestEmail@gmail.com");
    }


    private void OnRestoreStatus(bool status)
    {
        if (status)
        {
            GameManager.Instance.NoAdsPurchased();
            Debug.Log("Purchase Restore Success");
        }
        else
        {
            Debug.Log("Purchase Restore failed");
        }
    }


    private void OnConsentFlowError(CFError cfError)
    {
        switch (cfError.Code)
        {
            case CFError.ErrorCodeNotInGdprRegion:
                break;
            case CFError.ErrorCodeUnspecified:
                break;
            case CFError.ErrorCodeInvalidIntegration:
                break;
            case CFError.ErrorCodeFlowAlreadyInProgress:
                break;
            default:
                break;
        }
    }

}