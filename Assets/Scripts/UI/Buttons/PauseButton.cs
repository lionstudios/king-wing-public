using System;
using LionStudios.Suite.Analytics;
using Utils;

public class PauseButton : BaseButton
{
    protected override void OnClick()
    {
        //Time.timeScale = 0f;
        GameManager.Instance.PauseGamePlayElements();
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Pause));
        LionAnalytics.UiInteraction("Click", "PauseButton", "CornerButtons", "pauseGame");
    }
}
