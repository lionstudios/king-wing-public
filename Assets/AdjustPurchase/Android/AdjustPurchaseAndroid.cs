using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace com.adjust.sdk.purchase
{
#if UNITY_ANDROID
    public class AdjustPurchaseAndroid : IAdjustPurchase
    {
        #region Fields
        private const string sdkPrefix = "unity2.0.0";
        private AndroidJavaClass ajcAdjustPurchase;
        private VerificationInfoListener verificationInfoListener;
        private Action<AdjustPurchaseVerificationInfo> verificationInfoCallback;
        #endregion

        #region Proxy listener classes
        private class VerificationInfoListener : AndroidJavaProxy
        {
            private Action<AdjustPurchaseVerificationInfo> callback;

            public VerificationInfoListener(Action<AdjustPurchaseVerificationInfo> pCallback) : base("com.adjust.sdk.purchase.OnVerificationFinished")
            {
                this.callback = pCallback;
            }

            public void onVerificationFinished(AndroidJavaObject verificationInfo)
            {
                AdjustPurchaseVerificationInfo purchaseVerificationInfo = new AdjustPurchaseVerificationInfo();
                // message
                purchaseVerificationInfo.Message = verificationInfo.Get<string>(AdjustPurchaseUtils.KeyMessage);
                // verification status
                purchaseVerificationInfo.VerificationStatus = verificationInfo.Get<string>(AdjustPurchaseUtils.KeyVerificationStatus);
                // status code
                purchaseVerificationInfo.Code = verificationInfo.Get<int>(AdjustPurchaseUtils.KeyCode);

                if (callback != null)
                {
                    callback(purchaseVerificationInfo);
                }
            }
        }
        #endregion

        #region Constructors
        public AdjustPurchaseAndroid()
        {
            ajcAdjustPurchase = new AndroidJavaClass("com.adjust.sdk.purchase.AdjustPurchase");
        }
        #endregion

        #region Public methods
        public void Init(AdjustPurchaseConfig config)
        {
            // thank you, Unity 2019.2.0, for breaking this.
            // (code below written in PV SDK v1 syntax)
            // AndroidJavaObject ajoEnvironment = config.environment == ADJPEnvironment.Sandbox ? 
            //     new AndroidJavaClass("com.adjust.sdk.purchase.ADJPConstants").GetStatic<AndroidJavaObject>("ENVIRONMENT_SANDBOX") :
            //         new AndroidJavaClass("com.adjust.sdk.purchase.ADJPConstants").GetStatic<AndroidJavaObject>("ENVIRONMENT_PRODUCTION");

            // get environment
            string ajoEnvironment = config.environment == AdjustPurchaseEnvironment.Production ? "production" : "sandbox";

            // create config object
            AndroidJavaObject ajoConfig = new AndroidJavaObject("com.adjust.sdk.purchase.AdjustPurchaseConfig", config.appToken, ajoEnvironment);

            // check log level
            if (config.logLevel.HasValue)
            {
                AndroidJavaObject ajoLogLevel = new AndroidJavaClass("com.adjust.sdk.purchase.AdjustPurchaseLogLevel").GetStatic<AndroidJavaObject>(config.logLevel.Value.ToUppercaseString());

                if (ajoLogLevel != null)
                {
                    ajoConfig.Call("setLogLevel", ajoLogLevel);
                }
            }

            // set SDK prefix
            ajoConfig.Call("setSdkPrefix", sdkPrefix);

            // initialize and start the SDK
            ajcAdjustPurchase.CallStatic("init", ajoConfig);
        }

        public void VerifyPurchaseiOS(string receipt, string transactionId, string productId, string sceneName)
        {
        }

        public void VerifyPurchaseAndroid(string itemSku, string itemToken, Action<AdjustPurchaseVerificationInfo> verificationInfoCallback)
        {
            verificationInfoListener = new VerificationInfoListener(verificationInfoCallback);
            ajcAdjustPurchase.CallStatic("verifyPurchase", itemSku, itemToken, verificationInfoListener);
        }
        #endregion
    }
#endif
}
