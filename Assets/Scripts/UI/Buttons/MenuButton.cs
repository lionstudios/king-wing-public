using System;
using LionStudios.Suite.Analytics;
using Utils;

public class MenuButton : BaseButton
{
    protected override void OnClick()
    {
        GameManager.Instance.PauseGamePlayElements();
        _dispatcher.Send(EventId.ResetLevel, EventArgs.Empty);
        AdsManager.ShowCrossPromo();
        AdsManager.ShowBannerAd();
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Launch));
        ShopItemSwitch shopItemSwitch = new ShopItemSwitch(ShopManager.Instance.CurrentEquippedItem,
            ShopManager.Instance.CurrentSelectedItem, true);

        _dispatcher.Send(EventId.ShopItemSelectedChange, shopItemSwitch);
        _dispatcher.Send(EventId.ShopItemEquippedChange,
            shopItemSwitch);
        GameManager.Instance.lionCharacter.ToggleCameraShopPosition(false);
        LionAnalytics.GameEnded();
        LionAnalytics.ItemActioned("MenuClicked", "123", "MenuButton", "Button");
        LionAnalytics.LevelAbandoned(LevelManager.Instance.CurrentLevel, GameManager.Instance.Attempts,
            GameManager.Instance.LevelMoney, "Level", "Level", "Level");
        SatoriSDK.SatoriLevelAbandonedEvent(LevelManager.Instance.CurrentLevel, null);
        LionAnalytics.MissionAbandoned(false, "Normal", "Level", "LevelID", GameManager.Instance.LevelMoney);
        LionAnalytics.Options("SettingsNavigation", "settings");
        LionAnalytics.UiInteraction("Click", "MenuButton", "CornerButtons", "menuNavigation");
    }
}