using System;
using Events.Level.EventArgs;
using LionStudios.Suite.Analytics;
using Utils;

public class SkipButton : BaseButton
{
    protected override void OnClick()
    {
        var rewardArgs = new RewardArgs(RewardType.SkipReward);
        _dispatcher.Send(EventId.SendRewardData, rewardArgs);
        LevelEventArgs levelEventArgs = new LevelEventArgs
        {
            LevelNum = LevelManager.Instance.CurrentLevel,
            Score = GameManager.Instance.LevelMoney,
            AttemptNum = GameManager.Instance.Attempts,
            EventType = EventType.Ad
        };

        AdsManager.HideCrossPromo();
        AdsManager.ShowRewardedAd(levelEventArgs);
    }
}