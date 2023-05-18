using System;
using System.Collections.Generic;
using LionStudios.Suite.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using LionStudios.Suite.Purchasing;

/// <summary>
/// This class represents an IAP.
/// The general section must correspond to the info set in AppStore Connect or Google Play.
/// The analytics section defines what will be sent to LionAnalytics and Adjust.
/// There are 3 ways to implement the reward-giving method:
///  1) onPurchased UnityEvent in SimpleIAP:
///    You can link the reward function to the onPurchased UnityEvent in SimpleIAP directly in the Inspector for this IAP
///  2) OnProductPurchased virtual method:
///    You can inherit IAPData and override OnProductPurchased
///  3) PurchaseManager.AddPurchaseListener:
///    You can call PurchaseManager.AddPurchaseListener at runtime, preferably on Awake()
///    Note you shouldn't call it only when the player clicks the buy button, as it will also be used for restoring purchases.
/// </summary>
public abstract class IAP : ScriptableObject
{
    public string Id;
    public double Price;

    public abstract ProductType ProductType { get; }
    
    public abstract List<Item> ReceivedItems { get; }
    public abstract List<VirtualCurrency> ReceivedCurrencies { get; }
    public abstract string PurchaseLocation { get; }

    protected abstract UnityEvent OnPurchasedEvent { get; }

    public void OnPurchased()
    {
        OnPurchasedEvent?.Invoke(); 
        OnProductPurchased();
    }

    /// <summary>
    /// Override this method in child classes for custom IAPs
    /// </summary>
    protected abstract void OnProductPurchased();
    
    public void AddPurchaseListener(Action onPurchased)
    {
        this.OnPurchasedEvent.AddListener(new UnityAction(onPurchased));
    }

    public IAPGameplayInfo GetGameplayInfo()
    {
        return new IAPGameplayInfo(this.ReceivedItems, this.ReceivedCurrencies, this.PurchaseLocation);
    }
    
}
