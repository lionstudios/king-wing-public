using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.adjust.sdk;
using Events.Error.EventArgs;
using LionStudios.Suite.Analytics;
using UnityEngine;
using UnityEngine.Purchasing;
using AnalyticsProduct = LionStudios.Suite.Analytics.Product;
using Product = UnityEngine.Purchasing.Product;
using LionStudios.Suite.Purchasing;


/// <summary>
/// Singleton handling all IAPs.
/// Usage:
///  - Add an instance of this class in your scene (ideally in loading scene, in a persistent game object)
///  - Create all IAPData scriptable objects and fill the required info
///  - Implement and link reward functions (see IAPData for the 3 methods to link them)
///  - Call BuyProduct when the user requests a purchase
/// This will handle:
///  - Initializing the products with UnityPurchasing
///  - Initiating purchases and waiting for the response from the store
///  - Calling the reward functions linked
///  - Validating the purchase with remote validation
///  - Calling the required analytics events to LionAnalytics and Adjust
/// The reward can be given even if validation fails, or only if validation succeeds, depending on rewardMode. 
/// </summary>
public class PurchaseManager : MonoBehaviour, IStoreListener
{
    public static PurchaseManager Instance;

    #region Events

    public delegate void OnPurchaseStartDelegate();

    public OnPurchaseStartDelegate OnPurchaseStart;

    public delegate void OnPurchaseFailDelegate(PurchaseFailureReason reason);

    public OnPurchaseFailDelegate OnPurchaseFail;

    public delegate void PurchaseSuccessDelegate();

    public PurchaseSuccessDelegate OnPurchaseSuccess;

    #endregion

    #region Serialized Variables

    enum RewardMode
    {
        GiveIfReceiptIsValid,
        GiveAnyway
    }

    [SerializeField] private RewardMode rewardMode = RewardMode.GiveAnyway;

    [SerializeField] private List<IAP> iaps = new List<IAP>();

    #endregion

    #region Private Variables

    private static bool _isInitializing;
    private static bool _isPurchasing;
    private static bool _isVerifying;

    private IStoreController _controller;
    private IExtensionProvider _extensions;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }

        InitializePurchasing();
    }

    #endregion

    #region Initialize Methods

    public bool IsInitialized()
    {
        return _controller != null && _extensions != null;
    }

    private void InitializePurchasing()
    {
        if (IsInitialized() || _isInitializing) return;

        _isInitializing = true;
        OnPurchaseFail += ShowFailMessage;

        var purchasingModule = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
        purchasingModule.useFakeStoreAlways = true;
        purchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

        var builder = ConfigurationBuilder.Instance(purchasingModule);

        for (int i = 0; i < iaps.Count; i++)
        {
            builder.AddProduct(iaps[i].Id, iaps[i].ProductType);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    // Called when Unity IAP is ready to make purchases.
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;

        _isInitializing = false;
        Debug.Log("Unity Purchasing Initialized");
    }

    /// Called when Unity IAP encounters an unrecoverable initialization error.
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed reason:" + error);
        _isInitializing = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializeFailed reason:" + error + " Message: " + message);
        _isInitializing = false;
        ErrorEventArgs args = new ErrorEventArgs
        {
            Message = "Error" + error + " Message:" + message
        };
        LionAnalytics.ErrorEvent(args);
    }

    private void ShowFailMessage(PurchaseFailureReason reason)
    {
        Debug.LogError($"Purchase Failure: {reason}");
    }

    #endregion

    #region Buy Methods

    private bool CheckBuyRequirements()
    {
        if (!IsInitialized())
        {
            OnPurchaseFail?.Invoke(PurchaseFailureReason.PurchasingUnavailable);
            return false;
        }

        if (_isPurchasing || _isVerifying)
        {
            OnPurchaseFail?.Invoke(PurchaseFailureReason.ExistingPurchasePending);
            return false;
        }

        return true;
    }

    public void BuyProduct(IAP data)
    {
        BuyProduct(data.Id);
    }

    public void BuyProduct(string id)
    {
        if (!CheckBuyRequirements()) return;

        OnPurchaseStart?.Invoke();

        _isPurchasing = true;
        _controller.InitiatePurchase(id);
    }

    #endregion

    #region Process Methods

    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
    {
        _isPurchasing = false;

        Product product = args.purchasedProduct;
        Debug.Log("Product Receipt:" + product.receipt);
        Debug.Log($"OnProcessPurchase -> {product.definition.id}");


        if (!product.hasReceipt)
        {
            Debug.LogError("No Receipt");
            OnPurchaseFail?.Invoke(PurchaseFailureReason.Unknown);
        }
        else if (!product.availableToPurchase)
        {
            Debug.LogError("Not Available To Purchase");
            OnPurchaseFail?.Invoke(PurchaseFailureReason.ProductUnavailable);
        }
        else
        {
            //Do receipt validation
            IAPGameplayInfo iapGameplayInfo = GetIAPData(product.definition.id).GetGameplayInfo();
            if (rewardMode == RewardMode.GiveAnyway)
            {
                IAPValidation.ValidateAndLog(product, iapGameplayInfo);
                GiveReward(product);
            }
            else
            {
                _isVerifying = true;
                IAPValidation.ValidateAndLog(product, iapGameplayInfo,
                    () =>
                    {
                        GiveReward(product);
                        _isVerifying = false;
                    });
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    private void EventsOnPurchaseSuccess(Product product)
    {
        Dictionary<string, string> additionalData = new Dictionary<string, string>
        {
            { "priceLocal", product.metadata.localizedPriceString },
            { "currencyCode", product.metadata.isoCurrencyCode },
            { "transactionID", product.transactionID }
        };
        SatoriSDK.SatoriIAPPurchaseEvent(product.definition.id, additionalData);
        AdjustEvent adjustEvent = new AdjustEvent("iap_purchase");
        adjustEvent.setRevenue((int)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
        adjustEvent.setTransactionId(product.transactionID);
        adjustEvent.addCallbackParameter("productID", product.definition.id);
        adjustEvent.addCallbackParameter("transactionID", product.transactionID);
        Adjust.trackEvent(adjustEvent);
    }


    void GiveReward(Product product)
    {
        IAP iap = GetIAPData(product.definition.id);
        iap.OnPurchased();
        OnPurchaseSuccess?.Invoke();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        _isPurchasing = false;
        OnPurchaseFail?.Invoke(failureReason);
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases(Action<bool> onRestoreResult)
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        Debug.Log("RestorePurchases started ...");

        _extensions.GetExtension<IAppleExtensions>().RestoreTransactions((result) =>
        {
            onRestoreResult?.Invoke(result);
            Debug.Log("RestorePurchases continuing: " + result +
                      ". If no further messages, no purchases available to restore.");
        });
    }

    #endregion

    #region Util

    public string GetLocalizedPrice(IAP data)
    {
        if (data == null)
            return "null";

        if (!IsInitialized())
        {
            return $"${data.Price}";
        }

        Product iap = _controller.products.WithID(data.Id);
        return iap.metadata.localizedPriceString;
    }

    public string GetLocalizedPrice(string id)
    {
        return GetLocalizedPrice(GetIAPData(id));
    }

    public IAP GetIAPData(string id)
    {
        //TODO: optimize
        return iaps.Find(iap => iap.Id == id);
    }

    public void AddPurchaseListener(string id, Action onPurchased)
    {
        GetIAPData(id).AddPurchaseListener(onPurchased);
    }

    #endregion
}

[System.Serializable]
public class TransactionReceipt
{
    public string Store;
    public string TransactionID;
    public string Payload;
}

[System.Serializable]
public class GooglePlayReceipt
{
    public string json;
    public string signature;
}