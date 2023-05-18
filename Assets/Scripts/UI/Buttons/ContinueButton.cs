using System;
using Utils;

public class ContinueButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Game));

        int countDownTimer = 3;
        if (SatoriSDK.levelStartTimer != 0)
        {
            countDownTimer = SatoriSDK.levelStartTimer;
        }

        UIManager.Instance.SetCountDownTimer(new CountDownArgs(countDownTimer, GameManager.Instance.StartGame));
        if (!GameManager.Instance.CheckNoAdPurchase())
        {
            AdsManager.ShowBannerAd();
        }
    }
}