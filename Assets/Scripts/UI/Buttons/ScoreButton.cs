using System;

public class ScoreButton : BaseButton
{
    protected override void OnClick()
    {
        GameManager.Instance.IncrementScore();
    }
}