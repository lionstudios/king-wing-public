using System;
using System.Collections;
using System.Collections.Generic;
using LionStudios.Suite.Purchasing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinBundleItem : MonoBehaviour
{

    [SerializeField] private CoinBundleIAP iap;

    [SerializeField] private Button button;

    [SerializeField] private Image iconImg;

    [SerializeField] private TextMeshProUGUI amountLbl;

    [SerializeField] private TextMeshProUGUI priceLbl;

    private ShopItem _item;
    
    protected virtual void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private IEnumerator Start()
    {
        UpdateContent();
        yield return new WaitUntil(() => PurchaseManager.Instance.IsInitialized());
        UpdateContent();
    }

    void UpdateContent()
    {
        priceLbl.text = PurchaseManager.Instance.GetLocalizedPrice(iap);
        iconImg.sprite = iap.Icon;
        amountLbl.text = $"{iap.CoinsAmount}";
    }

    protected void OnClick()
    {
        PurchaseManager.Instance.BuyProduct(iap);
    }
}
