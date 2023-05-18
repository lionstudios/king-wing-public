using Utils;

public class FailButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Fail));
    }
}