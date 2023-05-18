using System;
using System.Collections.Generic;

using UnityEngine;

namespace com.adjust.sdk.purchase
{
    public class AdjustPurchaseUtils
    {
        #region Constants
        public static string KeyMessage = "message";
        public static string KeyCode = "code";
        public static string KeyVerificationStatus = "verificationStatus";
        #endregion

        #region Public methods
        public static int ConvertLogLevel(AdjustPurchaseLogLevel? logLevel)
        {
            if (logLevel == null)
            {
                return -1;
            }

            return (int)logLevel;
        }

        public static String GetJsonString(JSONNode node, string key)
        {
            var jsonValue = GetJsonValue(node, key);

            if (jsonValue == null)
            {
                return null;
            }

            return jsonValue.Value;
        }

        public static JSONNode GetJsonValue(JSONNode node, string key)
        {
            if (node == null)
            {
                return null;
            }

            var nodeValue = node[key];

            if (nodeValue.GetType() == typeof(JSONLazyCreator))
            {
                return null;
            }

            return nodeValue;
        }
        #endregion
    }
}
