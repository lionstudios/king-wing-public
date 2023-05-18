using System;
using System.Collections.Generic;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events.EventArgs;
using Utils;

public class ShopButton : BaseButton
{
    protected override void OnClick()
    {
        GameManager.Instance.PauseGamePlayElements();
        AdsManager.HideCrossPromo();
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Shop));
        GameManager.Instance.lionCharacter.ToggleCameraShopPosition(true);
        LionAnalytics.ProductViewed("Skins", "Cosmetics");
        LionAnalytics.ShopEntered("ItemShop", "skinIDs", "unlock");
        LionAnalytics.UiInteraction("Click", "MenuButton", "CornerButtons", "menuNavigation");
        FireSocialEvents();
    }

    public static void FireSocialEvents()
    {
        Dictionary<string, object> additionalData = new Dictionary<string, object>()
        {
            { "param_int", 1 },
            { "param_string", "test_string" },
            { "param_float", 56.70 }
        };
        Reward giftReward = new Reward("test", "testType", 5);
        LionAnalytics.PowerUpUsed("powerup_test_01", "powerup_test_type", 1, "test_p1", additionalData);
        LionAnalytics.FunnelEvent(1, "testLabel", 3, 4);
        LionAnalytics.GiftReceived(giftReward, "test_sender_ID", false, "test_tracking", additionalData);
        LionAnalytics.GiftSent(giftReward, "test_recipient_ID", "test_tracking", additionalData);
        LionAnalytics.InviteSent("test_unique_tracking", "test_invite_type", null, additionalData);
        LionAnalytics.InviteReceived("test_sender_ID", "test_unique_tracking", "test_invite_type", false, additionalData);
        SocialEventArgs socialEventArgs = new SocialEventArgs
        {
            SocialType = "test_social_type"
        };
        LionAnalytics.Social(socialEventArgs, additionalData);
        LionAnalytics.SocialConnect("test_user_id", "KingWingPlayer", "test_social_platform", additionalData);
        LionAnalytics.Support("testID");
        LionAnalytics.HandAction(1, "testID", "testHandID", "round", "roundName");
        LionAnalytics.CharacterCreated("testclass", "testGender", "testID", "testName", additionalData);
        LionAnalytics.CharacterDeleted("testclass", "testGender", "testID", additionalData);
        LionAnalytics.SkillUsed("test_ID", "test_name", true, "n/a", additionalData);
        LionAnalytics.SkillUpgraded(1, 1, "test_skill_ID", "test_skill_name", additionalData);
    }
}