using System;
using System.Collections;
using System.Collections.Generic;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Purchasing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using Utils;

public class MoneyArgs : EventArgs
{
    public int amount { get; }

    public MoneyArgs(int coinAmount)
    {
        amount = coinAmount;
    }
}

public class CoinBundleIAP : IAP
{
    [SerializeField] private int coinsAmount;

    [SerializeField] private Sprite icon;

    public int CoinsAmount => coinsAmount;
    public Sprite Icon => icon;
    public override ProductType ProductType => ProductType.Consumable;
    public override List<Item> ReceivedItems => new List<Item>();

    public override List<VirtualCurrency> ReceivedCurrencies =>
        new() { new VirtualCurrency("coins", "coins", coinsAmount) };

    public override string PurchaseLocation => "menu";
    protected override UnityEvent OnPurchasedEvent { get; }

    protected override void OnProductPurchased()
    {
        GameManager.Dispatcher.Send(EventId.AddMoney, new MoneyArgs(coinsAmount));
    }
}