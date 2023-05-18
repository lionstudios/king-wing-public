using UnityEngine;
using Utils;

public class ResumeButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Game));

        UIManager.Instance.SetCountDownTimer(new CountDownArgs(2, () =>
        {
            GameManager.Instance.IsStarted = true;
        }));


    }
}
