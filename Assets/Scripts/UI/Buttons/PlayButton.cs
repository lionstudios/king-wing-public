using System;
using Utils;

public class PlayButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Game));
        UIManager.Instance.SetCountDownTimer(new CountDownArgs(3, GameManager.Instance.StartGame));

        AdsManager.HideCrossPromo();
        AdsManager.ShowBannerAd();
    }
}