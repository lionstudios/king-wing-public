

using AppLovinMax.Internal.API;

public class PrivacyButton : BaseButton
{
    void OnEnable()
    {
        if (CFService.CFType != CFType.Detailed)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void OnClick()
    {
        CFService.SCF(OnConsentFlowError);
    }

    private void OnConsentFlowError(CFError cfError)
    {
        switch (cfError.Code)
        {
            case CFError.ErrorCodeNotInGdprRegion:
            break;
            case CFError.ErrorCodeUnspecified :
                break;
            case CFError.ErrorCodeInvalidIntegration:
                break;
            case CFError.ErrorCodeFlowAlreadyInProgress:
                break;
            default:
                break;
        }
        if (cfError.Code == CFError.ErrorCodeNotInGdprRegion)
        {
            // Show another, non gdpr privacy thing, maybe open privacy url?
        }
    }
}