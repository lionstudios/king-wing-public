using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ClaimButton : BaseButton
{
    protected override void OnClick()
    {
        _dispatcher.Send(EventId.AddMoney, new MoneyArgs(GameManager.Instance.LevelMoney));
        _dispatcher.Send(EventId.ResetLevel, EventArgs.Empty);
        AdsManager.HideCrossPromo();
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Game));
        UIManager.Instance.SetCountDownTimer(new CountDownArgs(3, GameManager.Instance.StartGame));
    }
}