using System;
using System.Collections.Generic;

namespace com.adjust.sdk.purchase
{
    public class AdjustPurchaseVerificationInfo
    {
        #region Properties
        public int Code { get; set; }
        public string Message { get; set; }
        public string VerificationStatus { get; set; }
        #endregion

        #region Constructors
        public AdjustPurchaseVerificationInfo()
        {
        }

        public AdjustPurchaseVerificationInfo (string jsonString)
        {
            var jsonNode = JSON.Parse (jsonString);

            if (jsonNode == null)
            {
                return;
            }

            string stringCode = AdjustPurchaseUtils.GetJsonString(jsonNode, AdjustPurchaseUtils.KeyCode);
            Code = Int32.Parse(stringCode);
            Message = AdjustPurchaseUtils.GetJsonString(jsonNode, AdjustPurchaseUtils.KeyMessage);
            VerificationStatus = AdjustPurchaseUtils.GetJsonString(jsonNode, AdjustPurchaseUtils.KeyVerificationStatus);
        }
        #endregion
    }
}
