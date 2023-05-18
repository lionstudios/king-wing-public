using System.Text.RegularExpressions;
using LionStudios.Suite.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseListeners : MonoBehaviour
{
    [SerializeField] private IAP noAds;
    
    private void Awake()
    {
        PurchaseManager.Instance.AddPurchaseListener(noAds.Id, NoAdsPurchased);
        PurchaseManager.Instance.OnPurchaseFail += OnPurchaseFail;
    }

    private void OnDestroy()
    {
        PurchaseManager.Instance.OnPurchaseFail -= OnPurchaseFail;
    }

    private void OnPurchaseFail(PurchaseFailureReason reason)
    {
        string reasonString = Regex.Replace(reason.ToString(), "([A-Z0-9]+)", " $1").Trim();
        PopupManager.Instance.ShowPopupTemporarily(reasonString, 1f);
    }

    private void NoAdsPurchased()
    {
        GameManager.Instance.NoAdsPurchased();
    }
}
