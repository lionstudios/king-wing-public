using System;
using Utils;

public class CoinBundlesButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Bundles));
        GameManager.Instance.PauseGamePlayElements();
    }
}
