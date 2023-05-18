using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;


public enum Screen
{
    Launch,
    Game,
    Shop,
    Fail,
    Succeed,
    Settings,
    Bundles,
    Continue,
    Pause
}

[Serializable]
public class UiScreenData
{
    public Screen screen;
    public UIScreen ui;
}

public class UiSwitchArgs : EventArgs
{
    public readonly Screen ToScreen;


    public UiSwitchArgs(Screen switchToScreen)
    {
        ToScreen = switchToScreen;
    }
}

public class CountDownArgs : EventArgs
{
    public int timer;
    public Action onTimerCompleted;

    public CountDownArgs(int currentTimer, Action completeAction)
    {
        timer = currentTimer;
        onTimerCompleted = completeAction;
    }
}

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private List<UiScreenData> _screenData;
    public List<UiScreenData> screenData => _screenData;
    [SerializeField] private Screen initialScreen;
    [SerializeField] private CountdownTimer _countdownTimer;
    [SerializeField] private LeaderboardView _leaderboardView;

    public LeaderboardView LeaderboardView => _leaderboardView;
    private Dispatcher _dispatcher;

    private void Start()
    {
        _dispatcher = GameManager.Dispatcher;
        SetScreen(new UiSwitchArgs(initialScreen));
    }


    public void SetScreen(EventArgs args)
    {
        var uiArgument = (UiSwitchArgs)args;
        foreach (var uiScreen in _screenData)
        {
            uiScreen.ui.Show(uiScreen.screen == uiArgument.ToScreen);
        }
    }

    public UIScreen GetScreen(Screen screen)
    {
        return (from uiScreen in _screenData where uiScreen.screen == screen select uiScreen.ui).FirstOrDefault();
    }


    public void SetCountDownTimer(EventArgs args)
    {
        var countDownArgs = (CountDownArgs)args;
        _countdownTimer.InitiateCountdown(countDownArgs.timer, countDownArgs.onTimerCompleted);
    }
}