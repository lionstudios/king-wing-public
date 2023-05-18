namespace com.adjust.sdk.purchase
{
    public enum AdjustPurchaseLogLevel
    {
        Debug = 1,
        Info,
        Error,
        Suppress
    }

    public static class AdjustPurchaseLogLevelExtension
    {
        public static string ToLowercaseString(this AdjustPurchaseLogLevel logLevel)
        {
            switch (logLevel)
            {
                case AdjustPurchaseLogLevel.Debug:
                    return "debug";
                case AdjustPurchaseLogLevel.Info:
                    return "info";
                case AdjustPurchaseLogLevel.Error:
                    return "error";
                case AdjustPurchaseLogLevel.Suppress:
                    return "suppress";
                default:
                    return "unknown";
            }
        }

        public static string ToUppercaseString(this AdjustPurchaseLogLevel logLevel)
        {
            switch (logLevel)
            {
                case AdjustPurchaseLogLevel.Debug:
                    return "DEBUG";
                case AdjustPurchaseLogLevel.Info:
                    return "INFO";
                case AdjustPurchaseLogLevel.Error:
                    return "ERROR";
                case AdjustPurchaseLogLevel.Suppress:
                    return "SUPPRESS";
                default:
                    return "UNKNOWN";
            }
        }
    }
}
