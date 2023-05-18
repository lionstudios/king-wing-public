using LionStudios.Suite.Purchasing;
using UnityEngine;

public class RestorePurchaseButton : BaseButton
{
    private void OnEnable()
    {
#if UNITY_ANDROID 
        gameObject.SetActive(false);
#endif
    }

    protected override void OnClick()
    {
        PurchaseManager.Instance.RestorePurchases(OnRestoreStatus);
    }

    private void OnRestoreStatus(bool status)
    {
        if (status)
        {
            GameManager.Instance.NoAdsPurchased();
            Debug.Log("Purchase Restore Success");
        }
        else
        {
            Debug.Log("Purchase Restore failed");
        }
    }
}