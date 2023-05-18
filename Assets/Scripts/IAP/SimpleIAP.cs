using System;
using System.Collections.Generic;
using LionStudios.Suite.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

/// <summary>
/// This class represents an IAP where all settable properties are exposed in the inspector.
/// This allows you to setup an IAP without having to create a class inheriting the IAP class.
/// See the IAP class for more details about IAPs and their settings.
/// </summary>
[CreateAssetMenu(fileName = "IAP", menuName = "Lion/Purchasing/IAP")]
public class SimpleIAP : IAP
{
    public ProductType productType = ProductType.Consumable;
    
    [Header("Analytics")]
    [SerializeField] private List<Item> receivedItems;
    [SerializeField] private List<VirtualCurrency> receivedCurrencies;
    [SerializeField] private string purchaseLocation;
    
    [Header("Reward")]
    [SerializeField] private UnityEvent onPurchased;
    
    public override ProductType ProductType => productType;
    public override List<Item> ReceivedItems => receivedItems;
    public override List<VirtualCurrency> ReceivedCurrencies => receivedCurrencies;
    public override string PurchaseLocation => purchaseLocation;

    protected override UnityEvent OnPurchasedEvent => onPurchased;

    protected override void OnProductPurchased() { }
    
    
}
