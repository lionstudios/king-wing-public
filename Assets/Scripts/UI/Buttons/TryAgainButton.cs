using System;
using LionStudios.Suite.Analytics;
using UnityEngine;
using Utils;

public class TryAgainButton : BaseButton
{
  

    protected override void OnClick()
    {
        _dispatcher.Send(EventId.AddMoney, new MoneyArgs(GameManager.Instance.LevelMoney));
        _dispatcher.Send(EventId.ResetLevel, EventArgs.Empty);
        LionAnalytics.LevelRestart(LevelManager.Instance.CurrentLevel, GameManager.Instance.Attempts,
            GameManager.Instance.LevelMoney, "Level", "Level", "Normal", "Level");
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Game));
        UIManager.Instance.SetCountDownTimer(new CountDownArgs(3, GameManager.Instance.StartGame));


        //Have the lion character return to sitting pose
        AdsManager.HideCrossPromo();
    }
}