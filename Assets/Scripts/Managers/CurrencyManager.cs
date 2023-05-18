using System;
using UnityEngine;
using Utils;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    private const string MONEY_PREFS_KEY = "Money";

    private Dispatcher _dispatcher;
    private int _moneyCached = -1;

    public int TotalMoney
    {
        get
        {
            if (_moneyCached < 0)
                _moneyCached = PlayerPrefs.GetInt(MONEY_PREFS_KEY, 0);
            return _moneyCached;
        }
        private set
        {
            PlayerPrefs.SetInt(MONEY_PREFS_KEY, value);
            _moneyCached = value;
            // _dispatcher.Send(EventId.UIUpdateMoneyChange, EventArgs.Empty);
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _dispatcher = GameManager.Dispatcher;
        _dispatcher.Subscribe(EventId.AddMoney, EarnMoney);
        if (SatoriSDK.defaultCoins != 0 && PlayerPrefs.GetInt("GameSession") == 0)
        {
            PlayerPrefs.SetInt("GameSession", PlayerPrefs.GetInt("GameSession") + 1);
            _dispatcher.Send(EventId.AddMoney, new MoneyArgs(SatoriSDK.defaultCoins));
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _dispatcher.Unsubscribe(EventId.AddMoney, EarnMoney);
    }

    public void EarnMoney(EventArgs args)
    {
        var moneyArgs = (MoneyArgs)args;
        TotalMoney += moneyArgs.amount;
    }

    public void SpendMoney(EventArgs args)
    {
        var moneyArgs = (MoneyArgs)args;
        if (TotalMoney < moneyArgs.amount)
            throw new Exception("Trying to buy with not enough money.");
        TotalMoney -= moneyArgs.amount;
    }
}