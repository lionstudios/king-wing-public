namespace com.adjust.sdk.purchase
{
    public enum AdjustPurchaseEnvironment
    {
        Sandbox,
        Production
    }

    public static class AdjustPurchaseEnvironmentExtension
    {
        public static string ToLowercaseString(this AdjustPurchaseEnvironment environment)
        {
            switch (environment)
            {
                case AdjustPurchaseEnvironment.Sandbox:
                    return "sandbox";
                case AdjustPurchaseEnvironment.Production:
                    return "production";
                default:
                    return "unknown";
            }
        }
    }
}
