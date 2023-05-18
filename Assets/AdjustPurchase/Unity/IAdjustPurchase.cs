using System;
using System.Collections.Generic;

namespace com.adjust.sdk.purchase
{
    public interface IAdjustPurchase
    {
        void Init(AdjustPurchaseConfig config);

        // iOS specific methods
        void VerifyPurchaseiOS(string receipt, string transactionId, string productId, string sceneName = "AdjustPurchase");

        // android specific methods
        void VerifyPurchaseAndroid(string itemSku, string itemToken, Action<AdjustPurchaseVerificationInfo> verificationInfoCallback);
    }
}
