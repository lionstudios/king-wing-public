using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SucceedButton : BaseButton
{
    protected override void OnClick()
    {
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Succeed));
    }
}