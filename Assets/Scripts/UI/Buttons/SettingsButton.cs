using System;
using LionStudios.Suite.Analytics;
using Utils;

public class SettingsButton : BaseButton
{
    protected override void OnClick()
    {
        GameManager.Instance.PauseGamePlayElements();
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Settings));
        LionAnalytics.UiInteraction("Click", "MenuButton", "CornerButtons", "menuNavigation");
    }
}