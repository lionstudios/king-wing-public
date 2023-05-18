using System;
using System.Collections;
using System.Collections.Generic;
using LionStudios.Suite.Purchasing;
using UnityEngine;
using Utils;

public class PurchaseButton : BaseButton
{

    [SerializeField] private IAP _iap;

    protected override void Awake()
    {
        base.Awake();
        _dispatcher.Subscribe(EventId.RemoveNoAdsButtonClicked, RemoveNoAdsButton);
    }

    private void OnEnable()
    {
        if (GameManager.Instance.CheckNoAdPurchase())
        {
            RemoveNoAdsButton(EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        _dispatcher.Unsubscribe(EventId.RemoveNoAdsButtonClicked, RemoveNoAdsButton);
    }

    protected override void OnClick()
    {
        PurchaseManager.Instance.BuyProduct(_iap);
    }

    private void RemoveNoAdsButton(EventArgs args)
    {
        gameObject.SetActive(false);
    }
}