using System;

namespace com.adjust.sdk.purchase
{
    public class AdjustPurchaseConfig
    {
        #region Fields
        internal string appToken;
        internal AdjustPurchaseLogLevel? logLevel;
        internal AdjustPurchaseEnvironment environment;
        internal Action<AdjustPurchaseVerificationInfo> verificationInfoCallback;
        #endregion

        #region Constructors
        public AdjustPurchaseConfig(string appToken, AdjustPurchaseEnvironment environment)
        {
            this.appToken = appToken;
            this.environment = environment;
        }
        #endregion

        #region Public methods
        public void SetLogLevel(AdjustPurchaseLogLevel logLevel)
        {
            this.logLevel = logLevel;
        }
        #endregion
    }
}
